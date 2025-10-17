using OnlineFruit_Data.Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Business.Repository.IReposi
{
    public interface IAddressRepository
    {
        Task<List<Address>> GetAll(CancellationToken cancellationToken);
        Task Update(AddressDto addressDto, CancellationToken cancellationToken);
        Task Delete(int Id, CancellationToken cancellationToken);
        Task<Address> Create(AddressDto addressDto, CancellationToken cancellationToken);
        Task<AddressDto> GetBy(int Id, CancellationToken cancellationToken);
    }
}
