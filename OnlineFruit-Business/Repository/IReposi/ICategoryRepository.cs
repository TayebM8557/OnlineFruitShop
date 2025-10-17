using OnlineFruit_Data.Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Business.Repository.IReposi
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAll(CancellationToken cancellationToken);
        Task Update(CategoryDto categoryDto, CancellationToken cancellationToken);
        Task Delete(int Id, CancellationToken cancellationToken);
        Task<Category> Create(CategoryDto categoryDto, CancellationToken cancellationToken);
        Task<CategoryDto> GetBy(int Id, CancellationToken cancellationToken);
    }
}
