using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CartItemRepository> _logger;

        public CartItemRepository(DatabaseContext db, IMapper mapper, ILogger<CartItemRepository> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CartItem> Create(CartItemDto cartItemDto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<CartItem>(cartItemDto);
            await _db.CartItems.AddAsync(entity, cancellationToken);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد آیتم سبد خرید");
                throw new Exception("خطا در ذخیره‌سازی آیتم سبد خرید", ex);
            }

            return entity;
        }

        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            var entity = await _db.CartItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("آیتم با شناسه {Id} یافت نشد", id);
                throw new KeyNotFoundException("آیتم مورد نظر برای حذف یافت نشد");
            }

            _db.CartItems.Remove(entity);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<CartItem>> GetAll(CancellationToken cancellationToken)
        {
            return await _db.CartItems
                .Include(x => x.User)
                .Include(x => x.Product)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CartItem>> GetAllInUserId(int userId, CancellationToken cancellationToken)
        {
            return await _db.CartItems
                .Include(x => x.User)
                .Include(x => x.Product)
                .Where(c => c.UserId == userId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetUserCartItemTotalQuantity(int userId, CancellationToken cancellationToken)
        {
            return await _db.CartItems
                .Where(c => c.UserId == userId)
                .CountAsync(cancellationToken);
        }

        public async Task<CartItemDto> GetBy(int id, CancellationToken cancellationToken)
        {
            var entity = await _db.CartItems
                .Include(x => x.Product)
                .Include(x => x.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                _logger.LogWarning("آیتم با شناسه {Id} یافت نشد", id);
                throw new KeyNotFoundException("آیتم یافت نشد");
            }

            return _mapper.Map<CartItemDto>(entity);
        }

        public async Task Update(CartItemDto cartItemDto, CancellationToken cancellationToken)
        {
            var entity = await _db.CartItems.FirstOrDefaultAsync(x => x.Id == cartItemDto.Id, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("آیتم برای بروزرسانی یافت نشد: {Id}", cartItemDto.Id);
                throw new KeyNotFoundException("آیتم برای بروزرسانی یافت نشد");
            }

            _mapper.Map(cartItemDto, entity);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

}
