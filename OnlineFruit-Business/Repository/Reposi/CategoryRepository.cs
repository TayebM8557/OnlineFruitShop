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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DatabaseContext _db;
        private readonly IMapper _mapper;

        public CategoryRepository(DatabaseContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<APP.Category> Create(CategoryDto categoryDto, CancellationToken cancellationToken)
        {
            var record = _mapper.Map<Category>(categoryDto);
            await _db.Categories.AddAsync(record);
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
            var order = await _db.Categories
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync(cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<APP.Category>> GetAll(CancellationToken cancellationToken)
        {
            var records = await _db.Categories
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return records;
        }

        public Task<CategoryDto> GetBy(int Id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Update(CategoryDto categoryDto, CancellationToken cancellationToken)
        {
            var record = await _mapper.ProjectTo<CategoryDto>(_db.Set<Category>())
                .Where(x => x.Id == categoryDto.Id).FirstOrDefaultAsync();
            _mapper.Map(categoryDto, record);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
