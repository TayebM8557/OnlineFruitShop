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
    public class CartItemRepository : ICartItemRepository
    {
        private readonly DatabaseContext _db;
        private readonly IMapper _mapper;

        public CartItemRepository(DatabaseContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<APP.CartItem> Create(CartItemDto cartItemDto, CancellationToken cancellationToken)
        {
            var record = _mapper.Map<CartItem>(cartItemDto);
            await _db.CartItems.AddAsync(record);
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
            var order = await _db.CartItems
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (order != null)
            {
                _db.CartItems.Remove(order);
                await _db.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw (new Exception());
            }
        }

        public async Task<List<APP.CartItem>> GetAll( CancellationToken cancellationToken)
        {
            var records = await _db.CartItems.Include(x => x.User).Include(y => y.Product)
               .AsNoTracking()
               .ToListAsync(cancellationToken);
            return records;
        }

        public async Task<int> GetUserCartItemTotalQuantity(int userId, CancellationToken cancellationToken)
        {
            var totalQuantity = await _db.CartItems
               .AsNoTracking()
               .Where(c => c.UserId == userId)
               .CountAsync(cancellationToken); ;// شرط: برای کاربر خاص
                                                // جمع مقدار ستون Quantity
            return totalQuantity;
        }

        public Task<CartItemDto> GetBy(int Id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Update(CartItemDto cartItemDto, CancellationToken cancellationToken)
        {
            var record = await _mapper.ProjectTo<CartItemDto>(_db.Set<CartItem>())
                .Where(x => x.Id == cartItemDto.Id).FirstOrDefaultAsync();
            _mapper.Map(cartItemDto, record);
            await _db.SaveChangesAsync(cancellationToken);
        }

        

        public async Task<List<CartItem>> GetAllInUserId(int userId, CancellationToken cancellationToken)
        {
            var records = await _db.CartItems.Include(x => x.User).Include(y => y.Product)
             .AsNoTracking()
             .Where(c => c.UserId == userId)
             .ToListAsync(cancellationToken);
            return records;
        }
    }
}
