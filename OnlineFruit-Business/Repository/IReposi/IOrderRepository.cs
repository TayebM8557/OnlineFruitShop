using OnlineFruit_Data.Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineFruit_Data.Entity;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Business.Repository.IReposi
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAll(CancellationToken cancellationToken);
        Task<List<APP.Payment>> GetAllByUser(int Id ,CancellationToken cancellationToken);
        Task Update(OrderDto orderDto, CancellationToken cancellationToken);
        Task Delete(int Id, CancellationToken cancellationToken);
        Task<Order> Create(OrderDto orderItemDto, CancellationToken cancellationToken);
        Task<OrderDto> GetBy(int Id, CancellationToken cancellationToken);
        Task<bool> ChangeStatus (int UserId , int OrderId,CancellationToken cancellationToken);
    }
}
