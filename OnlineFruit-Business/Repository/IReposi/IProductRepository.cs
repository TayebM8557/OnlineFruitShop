using OnlineFruit_Data.Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Business.Repository.IReposi
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAll(CancellationToken cancellationToken);
        Task Update(ProductDto productDto, CancellationToken cancellationToken);
        Task Delete(int Id, CancellationToken cancellationToken);
        Task<Product> Create(ProductDto productDto, CancellationToken cancellationToken);
        Task<Product> GetBy(int Id, CancellationToken cancellationToken);
    }
}
