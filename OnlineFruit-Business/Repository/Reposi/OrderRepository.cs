using AutoMapper;
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
    public class OrderRepository : IOrderRepository
    {
        private readonly DatabaseContext _db;
        private readonly IMapper _mapper;

        public OrderRepository(DatabaseContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> ChangeStatus(int UserId, int OrderId, CancellationToken cancellationToken)
        {
            var record = await _db.Orders
        .Include(c => c.User)
        .FirstOrDefaultAsync(x => x.Id == OrderId , cancellationToken);

            if (record != null)
            {
                record.Status = OrderStatus.Processing;
                await _db.SaveChangesAsync(cancellationToken); // ذخیره تغییرات در دیتابیس
                return true;
            }

            return false;
        }

        public async Task<APP.Order> Create(OrderDto orderItemDto, CancellationToken cancellationToken)
        {
            var record = _mapper.Map<Order>(orderItemDto);
            await _db.Orders.AddAsync(record);
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
            var order = await _db.Orders
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync(cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<APP.Order>> GetAll(CancellationToken cancellationToken)
        {
            var records = await _db.Orders
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return records;
        }

        public async Task<List<APP.Payment>> GetAllByUser(int Id, CancellationToken cancellationToken)
        {
            //var records = await _db.Orders.Where(x => x.User.Id == Id).Include(x=>x.OrderItems)
            //    .AsNoTracking()
            //    .ToListAsync(cancellationToken);
            //return records;
            var payments = await _db.Payments
                  .Where(p => p.Order.UserId == Id) // فیلتر بر اساس کاربر
                  .Include(p => p.Order) // اطلاعات سفارش مربوطه
                  .ToListAsync();




            return payments;
        }

        public Task<OrderDto> GetBy(int Id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Update(OrderDto orderDto, CancellationToken cancellationToken)
        {
            var record = await _mapper.ProjectTo<OrderDto>(_db.Set<Order>())
                .Where(x => x.Id == orderDto.Id).FirstOrDefaultAsync();
            _mapper.Map(orderDto, record);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
