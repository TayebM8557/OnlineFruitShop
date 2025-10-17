using OnlineFruit_Data.Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Business.Repository.IReposi
{
    public interface IOrderItemRepository
    {
        Task<List<OrderItem>> GetAll(CancellationToken cancellationToken); 
        Task<List<OrderItem>> GetAllStatus(CancellationToken cancellationToken);
        Task Update(OrderItemDto orderItemDto, CancellationToken cancellationToken);
        Task Delete(int Id, CancellationToken cancellationToken);
        Task<OrderItem> Create(OrderItemDto orderItemDto, CancellationToken cancellationToken);
        Task<OrderItem> OrderItemByOrder(int Id, CancellationToken cancellationToken);
        Task<OrderItemDto> GetBy(int Id, CancellationToken cancellationToken);
    }
}
