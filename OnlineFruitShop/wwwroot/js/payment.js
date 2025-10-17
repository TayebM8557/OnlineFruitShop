//// JavaScript برای مدیریت پرداخت
//document.addEventListener('DOMContentLoaded', function() {
//    // دکمه پرداخت
//    const payButton = document.getElementById('payButton');
//    if (payButton) {
//        payButton.addEventListener('click', function(e) {
//            e.preventDefault();
            
//            const orderId = this.getAttribute('data-order-id');
//            if (!orderId) {
//                alert('خطا: شناسه سفارش یافت نشد');
//                return;
//            }

//            // نمایش لودینگ
//            this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> در حال پردازش...';
//            this.disabled = true;

//            // ارسال درخواست پرداخت
//            fetch('/Payment/InitiatePayment', {
//                method: 'POST',
//                headers: {
//                    'Content-Type': 'application/json',
//                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
//                },
//                body: JSON.stringify({ orderId: parseInt(orderId) })
//            })
//            .then(response => response.json())
//            .then(data => {
//                if (data.success) {
//                    // هدایت به درگاه پرداخت
//                    window.location.href = data.paymentUrl;
//                } else {
//                    alert('خطا: ' + data.message);
//                    // بازگرداندن دکمه به حالت اولیه
//                    this.innerHTML = '<i class="fas fa-credit-card"></i> پرداخت';
//                    this.disabled = false;
//                }
//            })
//            .catch(error => {
//                console.error('Error:', error);
//                alert('خطا در ارتباط با سرور');
//                // بازگرداندن دکمه به حالت اولیه
//                this.innerHTML = '<i class="fas fa-credit-card"></i> پرداخت';
//                this.disabled = false;
//            });
//        });
//    }

//    // نمایش پیام‌های موفقیت یا خطا
//    const successMessage = document.querySelector('.alert-success');
//    const errorMessage = document.querySelector('.alert-danger');
    
//    if (successMessage || errorMessage) {
//        setTimeout(() => {
//            const alerts = document.querySelectorAll('.alert');
//            alerts.forEach(alert => {
//                alert.style.opacity = '0';
//                setTimeout(() => alert.remove(), 300);
//            });
//        }, 5000);
//    }
//});

//// تابع برای نمایش loading
//function showLoading(element) {
//    const originalText = element.innerHTML;
//    element.innerHTML = '<i class="fas fa-spinner fa-spin"></i> در حال پردازش...';
//    element.disabled = true;
    
//    return function hideLoading() {
//        element.innerHTML = originalText;
//        element.disabled = false;
//    };
//}

//// تابع برای افزودن دکمه پرداخت به سبد خرید
//function addPaymentButton(orderId) {
//    const paymentButton = document.createElement('button');
//    paymentButton.id = 'payButton';
//    paymentButton.setAttribute('data-order-id', orderId);
//    paymentButton.className = 'btn btn-primary py-3 px-4';
//    paymentButton.innerHTML = '<i class="fas fa-credit-card"></i> پرداخت';
    
//    return paymentButton;
//}
document.addEventListener('DOMContentLoaded', function () {
    const payButton = document.getElementById('payButton');
    const paymentMessage = document.getElementById('paymentMessage');
    const token = document.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"]').value;

    if (!payButton) return;

    payButton.addEventListener('click', async function (e) {
        e.preventDefault();

        const orderId = this.getAttribute('data-order-id');
        if (!orderId) {
            paymentMessage.innerHTML = `<span class="text-danger">خطا: شناسه سفارش یافت نشد</span>`;
            return;
        }

        // نمایش لودینگ
        const originalText = this.innerHTML;
        this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> در حال پردازش...';
        this.disabled = true;
        paymentMessage.innerHTML = '';

        try {
            const response = await fetch('/Cart?handler=OnPost', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({ orderId: parseInt(orderId) })
            });

            if (!response.ok) throw new Error("خطا در ایجاد درخواست پرداخت");

            const data = await response.json();

            if (data.isSuccess) {
                // هدایت کاربر به درگاه پرداخت
                window.location.href = data.paymentUrl;
            } else {
                paymentMessage.innerHTML = `<span class="text-danger">${data.errorMessage}</span>`;
                this.innerHTML = originalText;
                this.disabled = false;
            }
        } catch (err) {
            console.error(err);
            paymentMessage.innerHTML = `<span class="text-danger">خطا در ارتباط با سرور</span>`;
            this.innerHTML = originalText;
            this.disabled = false;
        }
    });
});

