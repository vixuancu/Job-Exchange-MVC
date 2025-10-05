namespace JobExchangeMvc.Helpers;

/// <summary>
/// Helper class để hash và verify password sử dụng BCrypt
/// </summary>
public static class PasswordHasher
{
    /// <summary>
    /// Hash password sử dụng BCrypt
    /// </summary>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    /// <summary>
    /// Verify password với hash
    /// </summary>
    public static bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            return false;
        }
    }
}
