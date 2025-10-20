using AutoMapper;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Data.Context;
using OnlineFruit_Data.Entity;
using OnlineFruit_Data.Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static OnlineFruit_Data.Entity.APP;
using Microsoft.Extensions.Logging;

namespace OnlineFruit_Business.Repository.Reposi
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DatabaseContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentRepository> _logger;

        public PaymentRepository(DatabaseContext db, IMapper mapper, ILogger<PaymentRepository> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<APP.Payment> Create(PaymentDto paymentDto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<APP.Payment>(paymentDto);
            await _db.Payments.AddAsync(entity, cancellationToken);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد پرداخت");
                throw new Exception("خطا در ذخیره‌سازی پرداخت", ex);
            }

            return entity;
        }
        public async Task<int> GetCurrentOrderId(int userId, CancellationToken cancellationToken)
        {
            try
            {
                // بررسی وجود سفارش باز برای کاربر
                var existingOrder = await _db.Orders
                    .Where(o => o.UserId == userId &&
                                (o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing))
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingOrder != null)
                {
                    _logger.LogInformation("سفارش باز برای کاربر {UserId} یافت شد: {OrderId}", userId, existingOrder.Id);
                    return existingOrder.Id;
                }

                // اگر سفارش باز نبود، ایجاد سفارش جدید
                var newOrder = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TotalAmount = 0, // بعداً با آیتم‌ها پر می‌شه
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _db.Orders.AddAsync(newOrder, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("سفارش جدید برای کاربر {UserId} ایجاد شد: {OrderId}", userId, newOrder.Id);
                return newOrder.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت یا ایجاد سفارش جاری برای کاربر {UserId}", userId);
                throw new Exception("خطا در دریافت سفارش جاری", ex);
            }
        }
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            var entity = await _db.Payments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("پرداخت با شناسه {Id} یافت نشد", id);
                throw new KeyNotFoundException("پرداخت مورد نظر برای حذف یافت نشد");
            }

            _db.Payments.Remove(entity);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<APP.Payment>> GetAll(CancellationToken cancellationToken)
        {
            return await _db.Payments
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<PaymentDto> GetBy(int id, CancellationToken cancellationToken)
        {
            var entity = await _db.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                _logger.LogWarning("پرداخت با شناسه {Id} یافت نشد", id);
                throw new KeyNotFoundException("پرداخت یافت نشد");
            }

            return _mapper.Map<PaymentDto>(entity);
        }

        public async Task Update(PaymentDto paymentDto, CancellationToken cancellationToken)
        {
            var entity = await _db.Payments.FirstOrDefaultAsync(x => x.Id == paymentDto.Id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("پرداخت برای بروزرسانی یافت نشد: {Id}", paymentDto.Id);
                throw new KeyNotFoundException("پرداخت برای بروزرسانی یافت نشد");
            }

            _mapper.Map(paymentDto, entity);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

}
