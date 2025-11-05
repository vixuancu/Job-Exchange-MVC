using Microsoft.EntityFrameworkCore;
using JobExchangeMvc.Data;
using JobExchangeMvc.DTOs;
using JobExchangeMvc.Helpers;
using JobExchangeMvc.Models;
using JobExchangeMvc.Services.Interfaces;

namespace JobExchangeMvc.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtTokenHelper _jwtTokenHelper;
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;

    public AuthService(
        ApplicationDbContext context,
        JwtTokenHelper jwtTokenHelper,
        ILogger<AuthService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _jwtTokenHelper = jwtTokenHelper;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Kiểm tra email đã tồn tại
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email đã được sử dụng"
                };
            }

            // Validate role
            if (registerDto.Role != "Applicant" && registerDto.Role != "Employer")
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Vai trò không hợp lệ"
                };
            }

            // Mã hóa VerifyKey trước khi lưu
            var encryptionKey = _configuration["VerifyKeyEncryption:Key"] ?? "DefaultKey32CharactersLong!!!";
            var encryptedVerifyKey = VerifyKeyEncryptor.Encrypt(registerDto.VerifyKey, encryptionKey);

            // Tạo user mới
            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = PasswordHasher.HashPassword(registerDto.Password),
                FullName = registerDto.FullName,
                PhoneNumber = registerDto.PhoneNumber,
                Role = registerDto.Role,
                VerifyKey = encryptedVerifyKey,  // ← Lưu VerifyKey đã mã hóa
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User registered successfully: {Email}", user.Email);

            // Tạo tokens
            var accessToken = _jwtTokenHelper.GenerateAccessToken(user);
            var refreshToken = _jwtTokenHelper.GenerateRefreshToken();
            var refreshTokenExpiration = _jwtTokenHelper.GetRefreshTokenExpiration();

            // Lưu refresh token
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = refreshTokenExpiration,
                CreatedAt = DateTime.UtcNow
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Đăng ký thành công",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = refreshTokenExpiration,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    AvatarUrl = user.AvatarUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return new AuthResponseDto
            {
                Success = false,
                Message = "Đã xảy ra lỗi trong quá trình đăng ký"
            };
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Tìm user theo email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email hoặc mật khẩu không đúng"
                };
            }

            // Kiểm tra mật khẩu
            if (!PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email hoặc mật khẩu không đúng"
                };
            }

            // Kiểm tra trạng thái tài khoản
            if (!user.IsActive)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Tài khoản đã bị khóa"
                };
            }

            _logger.LogInformation("User logged in successfully: {Email}", user.Email);

            // Tạo tokens
            var accessToken = _jwtTokenHelper.GenerateAccessToken(user);
            var refreshToken = _jwtTokenHelper.GenerateRefreshToken();
            var refreshTokenExpiration = _jwtTokenHelper.GetRefreshTokenExpiration();

            // Revoke old refresh tokens nếu không remember me
            if (!loginDto.RememberMe)
            {
                var oldTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
                    .ToListAsync();

                foreach (var oldToken in oldTokens)
                {
                    oldToken.IsRevoked = true;
                    oldToken.RevokedAt = DateTime.UtcNow;
                }
            }

            // Lưu refresh token mới
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = refreshTokenExpiration,
                CreatedAt = DateTime.UtcNow
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Đăng nhập thành công",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = refreshTokenExpiration,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    AvatarUrl = user.AvatarUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return new AuthResponseDto
            {
                Success = false,
                Message = "Đã xảy ra lỗi trong quá trình đăng nhập"
            };
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        try
        {
            // Validate access token (không check expiration)
            var principal = _jwtTokenHelper.GetPrincipalFromToken(accessToken);
            if (principal == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Token không hợp lệ"
                };
            }

            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Token không hợp lệ"
                };
            }

            // Tìm refresh token
            var storedRefreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

            if (storedRefreshToken == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Refresh token không tồn tại"
                };
            }

            // Kiểm tra refresh token
            if (storedRefreshToken.IsRevoked)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Refresh token đã bị thu hồi"
                };
            }

            if (storedRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Refresh token đã hết hạn"
                };
            }

            // Tạo tokens mới
            var user = storedRefreshToken.User;
            var newAccessToken = _jwtTokenHelper.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenHelper.GenerateRefreshToken();
            var newRefreshTokenExpiration = _jwtTokenHelper.GetRefreshTokenExpiration();

            // Revoke old refresh token
            storedRefreshToken.IsRevoked = true;
            storedRefreshToken.RevokedAt = DateTime.UtcNow;

            // Lưu refresh token mới
            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiresAt = newRefreshTokenExpiration,
                CreatedAt = DateTime.UtcNow
            };

            await _context.RefreshTokens.AddAsync(newRefreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "Token được làm mới thành công",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = newRefreshTokenExpiration,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    AvatarUrl = user.AvatarUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return new AuthResponseDto
            {
                Success = false,
                Message = "Đã xảy ra lỗi khi làm mới token"
            };
        }
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        try
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null || token.IsRevoked)
            {
                return false;
            }

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            return false;
        }
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
