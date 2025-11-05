// Admin Users - VerifyKey AJAX Handler

$(document).ready(function() {
    // AJAX Show VerifyKey (giải mã)
    $('.btn-show-verify-key').on('click', function(e) {
        e.preventDefault();
        
        const userId = $(this).data('id');
        const btn = $(this);
        const originalIcon = btn.find('i').attr('class');
        const originalText = btn.html();
        
        // Show loading
        btn.prop('disabled', true);
        btn.html('<span class="spinner-border spinner-border-sm"></span>');
        
        $.ajax({
            url: '/Admin/DecryptVerifyKey',
            type: 'POST',
            data: { 
                id: userId,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function(response) {
                if (response.success) {
                    // Show modal with original key
                    showVerifyKeyModal(response.encryptedKey, response.originalKey);
                } else {
                    showToast('error', response.message || 'Không thể giải mã VerifyKey');
                }
                
                // Restore button
                btn.prop('disabled', false).html(originalText);
            },
            error: function(xhr, status, error) {
                console.error('AJAX error:', error);
                showToast('error', 'Có lỗi xảy ra khi giải mã VerifyKey');
                
                // Restore button
                btn.prop('disabled', false).html(originalText);
            }
        });
    });
});

// Show VerifyKey Modal
function showVerifyKeyModal(encryptedKey, originalKey) {
    const modalHtml = `
        <div class="modal fade" id="verifyKeyModal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title">
                            <i class="fas fa-key"></i> VerifyKey Gốc
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="alert alert-secondary">
                            <strong><i class="fas fa-lock"></i> Encrypted:</strong>
                            <br>
                            <code class="small">${encryptedKey}</code>
                        </div>
                        <div class="alert alert-success">
                            <strong><i class="fas fa-unlock"></i> Original:</strong>
                            <br>
                            <code class="fs-4 text-success">${originalKey}</code>
                            <button class="btn btn-sm btn-outline-success ms-2" 
                                    onclick="copyToClipboard('${originalKey}')">
                                <i class="fas fa-copy"></i> Copy
                            </button>
                        </div>
                        <div class="alert alert-warning mb-0">
                            <i class="fas fa-exclamation-triangle"></i>
                            <strong>Lưu ý:</strong> Đây là thông tin nhạy cảm. Không chia sẻ với người khác.
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                            <i class="fas fa-times"></i> Đóng
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    // Remove existing modal
    $('#verifyKeyModal').remove();
    
    // Add new modal
    $('body').append(modalHtml);
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('verifyKeyModal'));
    modal.show();
    
    // Remove modal from DOM after hidden
    $('#verifyKeyModal').on('hidden.bs.modal', function() {
        $(this).remove();
    });
}

// Copy to clipboard
function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(function() {
        showToast('success', 'Đã copy VerifyKey vào clipboard!');
    }, function(err) {
        console.error('Copy failed:', err);
        showToast('error', 'Không thể copy. Vui lòng copy thủ công.');
    });
}

// Toast notification helper
function showToast(type, message) {
    const bgClass = type === 'success' ? 'bg-success' : 'bg-danger';
    const icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';
    
    const toastHtml = `
        <div class="toast align-items-center text-white ${bgClass} border-0" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="fas ${icon} me-2"></i> ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;
    
    let toastContainer = $('#toastContainer');
    if (toastContainer.length === 0) {
        $('body').append('<div id="toastContainer" class="toast-container position-fixed top-0 end-0 p-3"></div>');
        toastContainer = $('#toastContainer');
    }
    
    toastContainer.append(toastHtml);
    const toastElement = toastContainer.find('.toast').last();
    const toast = new bootstrap.Toast(toastElement[0], { delay: 3000 });
    toast.show();
    
    // Auto remove after hidden
    toastElement.on('hidden.bs.toast', function() {
        $(this).remove();
    });
}
