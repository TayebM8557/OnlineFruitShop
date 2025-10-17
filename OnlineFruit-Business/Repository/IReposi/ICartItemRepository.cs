using OnlineFruit_Data.Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Business.Repository.IReposi
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetAll(CancellationToken cancellationToken);
        Task<List<CartItem>> GetAllInUserId(int userId, CancellationToken cancellationToken);
        Task<int> GetUserCartItemTotalQuantity(int userId, CancellationToken cancellationToken);
        Task Update(CartItemDto cartItemDto, CancellationToken cancellationToken);
        Task Delete(int Id, CancellationToken cancellationToken);
        Task<CartItem> Create(CartItemDto cartItemDto, CancellationToken cancellationToken);
        Task<CartItemDto> GetBy(int Id, CancellationToken cancellationToken);
    }
}
