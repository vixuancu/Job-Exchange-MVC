// Job Exchange MVC - Custom JavaScript

// Document ready
$(document).ready(function () {
    console.log('Job Exchange MVC loaded successfully!');

    // Initialize tooltips
    initializeTooltips();

    // Initialize AJAX search
    initializeJobSearch();

    // Initialize file upload preview
    initializeFileUpload();

    // Auto-hide alerts
    autoHideAlerts();
});

/**
 * Initialize Bootstrap tooltips
 */
function initializeTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

/**
 * Initialize AJAX job search
 */
function initializeJobSearch() {
    const searchForm = $('#jobSearchForm');
    if (searchForm.length === 0) return;

    // Handle form submit
    searchForm.on('submit', function (e) {
        e.preventDefault();
        performJobSearch();
    });

    // Handle search input with debounce
    let searchTimeout;
    $('#searchTerm').on('keyup', function () {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(function () {
            performJobSearch();
        }, 500);
    });

    // Handle category change
    $('#categoryId').on('change', function () {
        performJobSearch();
    });

    // Handle location change
    $('#location').on('change', function () {
        performJobSearch();
    });
}

/**
 * Perform AJAX job search
 */
function performJobSearch() {
    const searchTerm = $('#searchTerm').val();
    const categoryId = $('#categoryId').val();
    const location = $('#location').val();

    // Show loading spinner
    $('#jobList').html('<div class="spinner-wrapper"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');

    // AJAX request
    $.ajax({
        url: '/Jobs/SearchPartial',
        type: 'GET',
        data: {
            searchTerm: searchTerm,
            categoryId: categoryId,
            location: location
        },
        success: function (response) {
            $('#jobList').html(response);
            // Add fade-in animation
            $('#jobList .job-card').addClass('fade-in');
        },
        error: function (xhr, status, error) {
            console.error('Search error:', error);
            $('#jobList').html('<div class="alert alert-danger"><i class="fas fa-exclamation-circle"></i> Đã xảy ra lỗi khi tìm kiếm. Vui lòng thử lại.</div>');
        }
    });
}

/**
 * Initialize file upload preview
 */
function initializeFileUpload() {
    // Avatar preview
    $('#avatarInput').on('change', function () {
        previewFile(this, '#avatarPreview');
    });

    // CV upload
    $('#cvInput').on('change', function () {
        displayFileName(this, '#cvFileName');
    });

    // Company logo preview
    $('#logoInput').on('change', function () {
        previewFile(this, '#logoPreview');
    });
}

/**
 * Preview image file
 */
function previewFile(input, previewSelector) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            $(previewSelector).attr('src', e.target.result).show();
        };

        reader.readAsDataURL(input.files[0]);
    }
}

/**
 * Display selected file name
 */
function displayFileName(input, displaySelector) {
    if (input.files && input.files[0]) {
        const fileName = input.files[0].name;
        $(displaySelector).text(fileName).show();
    }
}

/**
 * Auto-hide alerts after 5 seconds
 */
function autoHideAlerts() {
    setTimeout(function () {
        $('.alert').fadeOut('slow', function () {
            $(this).remove();
        });
    }, 5000);
}

/**
 * Confirm delete action
 */
function confirmDelete(message) {
    return confirm(message || 'Bạn có chắc chắn muốn xóa?');
}

/**
 * Apply for job (AJAX)
 */
function quickApply(jobId) {
    if (!confirm('Bạn có chắc chắn muốn ứng tuyển vào công việc này?')) {
        return;
    }

    $.ajax({
        url: '/Applications/QuickApply',
        type: 'POST',
        data: {
            jobId: jobId,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success) {
                showNotification('success', response.message);
                // Reload page or update UI
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                showNotification('error', response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Apply error:', error);
            showNotification('error', 'Đã xảy ra lỗi. Vui lòng thử lại.');
        }
    });
}

/**
 * Show notification
 */
function showNotification(type, message) {
    const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
    const icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';

    const alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="fas ${icon}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;

    // Prepend to container
    $('.container').first().prepend(alertHtml);

    // Auto-hide after 5 seconds
    setTimeout(function () {
        $('.alert').first().fadeOut('slow', function () {
            $(this).remove();
        });
    }, 5000);
}

/**
 * Update application status (AJAX)
 */
function updateApplicationStatus(applicationId, status) {
    const statusMessages = {
        'Accepted': 'chấp nhận',
        'Rejected': 'từ chối'
    };

    if (!confirm(`Bạn có chắc chắn muốn ${statusMessages[status]} đơn ứng tuyển này?`)) {
        return;
    }

    $.ajax({
        url: '/Applications/UpdateStatus',
        type: 'POST',
        data: {
            id: applicationId,
            status: status,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success) {
                showNotification('success', response.message);
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                showNotification('error', response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Update status error:', error);
            showNotification('error', 'Đã xảy ra lỗi. Vui lòng thử lại.');
        }
    });
}

/**
 * Format currency
 */
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

/**
 * Format date
 */
function formatDate(dateString) {
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    return new Date(dateString).toLocaleDateString('vi-VN', options);
}

/**
 * Validate form before submit
 */
function validateForm(formSelector) {
    const form = $(formSelector);
    let isValid = true;

    // Check required fields
    form.find('[required]').each(function () {
        if ($(this).val().trim() === '') {
            $(this).addClass('is-invalid');
            isValid = false;
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    // Check email format
    form.find('[type="email"]').each(function () {
        const email = $(this).val();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (email && !emailRegex.test(email)) {
            $(this).addClass('is-invalid');
            isValid = false;
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    return isValid;
}

/**
 * Copy to clipboard
 */
function copyToClipboard(text) {
    const textarea = document.createElement('textarea');
    textarea.value = text;
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    showNotification('success', 'Đã sao chép vào clipboard!');
}

/**
 * Scroll to top
 */
function scrollToTop() {
    $('html, body').animate({ scrollTop: 0 }, 'smooth');
}

// Add scroll to top button
$(window).scroll(function () {
    if ($(this).scrollTop() > 100) {
        $('#scrollTopBtn').fadeIn();
    } else {
        $('#scrollTopBtn').fadeOut();
    }
});

// Global error handler for AJAX
$(document).ajaxError(function (event, xhr, settings, error) {
    console.error('AJAX Error:', error);
    if (xhr.status === 401) {
        showNotification('error', 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.');
        setTimeout(function () {
            window.location.href = '/Account/Login';
        }, 2000);
    } else if (xhr.status === 403) {
        showNotification('error', 'Bạn không có quyền thực hiện thao tác này.');
    } else if (xhr.status === 500) {
        showNotification('error', 'Đã xảy ra lỗi server. Vui lòng thử lại sau.');
    }
});

// Export functions for global use
window.jobExchange = {
    confirmDelete,
    quickApply,
    updateApplicationStatus,
    formatCurrency,
    formatDate,
    validateForm,
    copyToClipboard,
    scrollToTop,
    showNotification
};
