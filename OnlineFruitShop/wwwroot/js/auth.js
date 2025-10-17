// JavaScript برای مدیریت احراز هویت
document.addEventListener('DOMContentLoaded', function() {
    
    // Validation برای فرم لاگین
    const loginForm = document.querySelector('form[asp-page-handler="Login"]');
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            const emailOrPhone = document.getElementById('inputEmailOrPhone');
            const password = document.getElementById('inputPass');
            
            if (!emailOrPhone.value.trim()) {
                e.preventDefault();
                alert('لطفاً ایمیل یا شماره موبایل را وارد کنید');
                emailOrPhone.focus();
                return false;
            }
            
            if (!password.value.trim()) {
                e.preventDefault();
                alert('لطفاً رمز عبور را وارد کنید');
                password.focus();
                return false;
            }
            
            // بررسی فرمت شماره موبایل
            const phoneRegex = /^09\d{9}$/;
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            
            if (!phoneRegex.test(emailOrPhone.value) && !emailRegex.test(emailOrPhone.value)) {
                e.preventDefault();
                alert('لطفاً ایمیل یا شماره موبایل معتبر وارد کنید');
                emailOrPhone.focus();
                return false;
            }
        });
    }
    
    // Validation برای فرم رجیستر
    const registerForm = document.querySelector('form[asp-page-handler="Create"]');
    if (registerForm) {
        registerForm.addEventListener('submit', function(e) {
            const email = document.getElementById('email');
            const phone = document.getElementById('phone');
            const password = document.getElementById('password');
            const confirmPassword = document.getElementById('confirmPassword');
            const firstName = document.getElementById('firstName');
            const lastName = document.getElementById('lastName');
            
            // بررسی اینکه حداقل یکی از ایمیل یا شماره موبایل وارد شده باشد
            if (!email.value.trim() && !phone.value.trim()) {
                e.preventDefault();
                alert('حداقل یکی از ایمیل یا شماره موبایل الزامی است');
                return false;
            }
            
            // بررسی فرمت ایمیل
            if (email.value.trim()) {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(email.value)) {
                    e.preventDefault();
                    alert('لطفاً ایمیل معتبر وارد کنید');
                    email.focus();
                    return false;
                }
            }
            
            // بررسی فرمت شماره موبایل
            if (phone.value.trim()) {
                const phoneRegex = /^09\d{9}$/;
                if (!phoneRegex.test(phone.value)) {
                    e.preventDefault();
                    alert('لطفاً شماره موبایل معتبر وارد کنید (مثال: 09123456789)');
                    phone.focus();
                    return false;
                }
            }
            
            // بررسی رمز عبور
            if (!password.value.trim()) {
                e.preventDefault();
                alert('لطفاً رمز عبور را وارد کنید');
                password.focus();
                return false;
            }
            
            if (password.value.length < 6) {
                e.preventDefault();
                alert('رمز عبور باید حداقل 6 کاراکتر باشد');
                password.focus();
                return false;
            }
            
            // بررسی تکرار رمز عبور
            if (password.value !== confirmPassword.value) {
                e.preventDefault();
                alert('رمز عبور و تکرار آن مطابقت ندارند');
                confirmPassword.focus();
                return false;
            }
        });
    }
    
    // نمایش پیام‌های موفقیت و خطا
    const successMessage = document.querySelector('.alert-success');
    const errorMessage = document.querySelector('.alert-danger');
    
    if (successMessage || errorMessage) {
        setTimeout(() => {
            const alerts = document.querySelectorAll('.alert');
            alerts.forEach(alert => {
                alert.style.opacity = '0';
                setTimeout(() => alert.remove(), 300);
            });
        }, 5000);
    }
    
    // Auto-format شماره موبایل
    const phoneInputs = document.querySelectorAll('input[name="PhoneNumber"], input[id="phone"]');
    phoneInputs.forEach(input => {
        input.addEventListener('input', function(e) {
            let value = e.target.value.replace(/\D/g, ''); // حذف کاراکترهای غیر عددی
            if (value.length > 11) {
                value = value.substring(0, 11);
            }
            e.target.value = value;
        });
        
        input.addEventListener('blur', function(e) {
            let value = e.target.value;
            if (value.length === 10 && value.startsWith('9')) {
                e.target.value = '0' + value;
            }
        });
    });
    
    // بهبود UX برای فرم‌ها
    const formInputs = document.querySelectorAll('.form-control');
    formInputs.forEach(input => {
        input.addEventListener('focus', function() {
            this.classList.add('border-primary');
        });
        
        input.addEventListener('blur', function() {
            this.classList.remove('border-primary');
        });
    });
});

// تابع برای نمایش پیام خطا
function showError(message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'alert alert-danger alert-dismissible fade show';
    errorDiv.innerHTML = `
        <i class="fas fa-exclamation-triangle me-2"></i>
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert">
            <span>&times;</span>
        </button>
    `;
    
    const container = document.querySelector('.card-body') || document.body;
    container.insertBefore(errorDiv, container.firstChild);
    
    setTimeout(() => {
        errorDiv.remove();
    }, 5000);
}

// تابع برای نمایش پیام موفقیت
function showSuccess(message) {
    const successDiv = document.createElement('div');
    successDiv.className = 'alert alert-success alert-dismissible fade show';
    successDiv.innerHTML = `
        <i class="fas fa-check-circle me-2"></i>
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert">
            <span>&times;</span>
        </button>
    `;
    
    const container = document.querySelector('.card-body') || document.body;
    container.insertBefore(successDiv, container.firstChild);
    
    setTimeout(() => {
        successDiv.remove();
    }, 5000);
}

// تابع برای نمایش loading
function showLoading(element) {
    const originalText = element.innerHTML;
    element.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>در حال پردازش...';
    element.disabled = true;
    
    return function hideLoading() {
        element.innerHTML = originalText;
        element.disabled = false;
    };
}