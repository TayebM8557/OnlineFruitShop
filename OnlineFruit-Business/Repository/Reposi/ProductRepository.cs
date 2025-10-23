using AutoMapper;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Data.Context;
using OnlineFruit_Data.Entity;
using OnlineFruit_Data.Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Business.Repository.Reposi
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _db;
        private readonly IMapper _mapper;

        public ProductRepository(DatabaseContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<APP.Product> Create(ProductDto productDto, CancellationToken cancellationToken)
        {
            var record = _mapper.Map<Product>(productDto);
            await _db.Products.AddAsync(record);
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
            var product = await _db.Products
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (product != null)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<APP.Product>> GetAll(CancellationToken cancellationToken)
        {
            var records = await _db.Products.Include(x => x.Category)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return records;
        }

        public async Task<Product> GetBy(int Id, CancellationToken cancellationToken)
        {
            var record = await _db.Products
                .Where(x => x.Id == Id).FirstOrDefaultAsync();


            return record;
        }

        public async Task Update(ProductDto productDto, CancellationToken cancellationToken)
        {
            var record = await _db.Products
                .Where(x => x.Id == productDto.Id)
                .FirstOrDefaultAsync();
            _mapper.Map(productDto, record);
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
