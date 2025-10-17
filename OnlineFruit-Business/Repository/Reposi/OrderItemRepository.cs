using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Data.Context;
using OnlineFruit_Data.Entity;
using OnlineFruit_Data.Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Business.Repository.Reposi
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly DatabaseContext _db;
        private readonly IMapper _mapper;

        public OrderItemRepository(DatabaseContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<APP.OrderItem> Create(OrderItemDto orderItemDto, CancellationToken cancellationToken)
        {
            var record = _mapper.Map<OrderItem>(orderItemDto);
            await _db.OrderItems.AddAsync(record);
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return record;
        }

        public async Task Delete(int Id, CancellationToken cancellationToken)
        {
            var order = await _db.OrderItems
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync(cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<APP.OrderItem>> GetAll(CancellationToken cancellationToken)
        {
            var purchasedProducts = await _db.OrderItems
            .Where(oi => _db.Payments
                .Any(p => p.OrderId == oi.OrderId && p.Status == PaymentStatus.Completed)) // بررسی پرداخت موفق
                .Include(oi => oi.Product) // دریافت اطلاعات محصول
                .Include(oi => oi.Order)// دریافت اطلاعات سفارش
                .ThenInclude(o => o.User).ThenInclude(a=>a.Address).Where(q => q.Order.Status == OrderStatus.Pending)  // دریافت اطلاعات خریدار
            .ToListAsync();

            return purchasedProducts;
        }

        public async Task<List<OrderItem>> GetAllStatus(CancellationToken cancellationToken)
        {
            var purchasedProducts = await _db.OrderItems
           .Where(oi => _db.Payments
               .Any(p => p.OrderId == oi.OrderId && p.Status == PaymentStatus.Completed)) // بررسی پرداخت موفق
               .Include(oi => oi.Product) // دریافت اطلاعات محصول
               .Include(oi => oi.Order)// دریافت اطلاعات سفارش
               .ThenInclude(o => o.User).ThenInclude(a => a.Address).Where(q => q.Order.Status == OrderStatus.Processing)  // دریافت اطلاعات خریدار
           .ToListAsync();

            return purchasedProducts;
        }

        public Task<OrderItemDto> GetBy(int Id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderItem> OrderItemByOrder(int Id, CancellationToken cancellationToken)
        {
            var orderItems = await _db.OrderItems
             .Where(oi => oi.OrderId == Id) // فیلتر بر اساس سفارش خاص
             .Include(oi => oi.Product) // دریافت اطلاعات محصول مرتبط
             .FirstOrDefaultAsync();
            return orderItems;
        }

        public async Task Update(OrderItemDto orderItemDto, CancellationToken cancellationToken)
        {
            var record = await _mapper.ProjectTo<OrderItemDto>(_db.Set<OrderItem>())
                .Where(x => x.Id == orderItemDto.Id).FirstOrDefaultAsync();
            _mapper.Map(orderItemDto, record);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
