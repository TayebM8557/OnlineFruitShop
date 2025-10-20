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
    public interface IPaymentRepository
    {
        Task<List<APP.Payment>> GetAll(CancellationToken cancellationToken);
        Task Update(PaymentDto paymentDto, CancellationToken cancellationToken);
        Task Delete(int Id, CancellationToken cancellationToken);
        Task<APP.Payment> Create(PaymentDto paymentDto, CancellationToken cancellationToken);
        Task<PaymentDto> GetBy(int Id, CancellationToken cancellationToken);
        Task<int> GetCurrentOrderId(int userId, CancellationToken cancellationToken);
    }
}
