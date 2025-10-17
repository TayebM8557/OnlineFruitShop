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
    public class AddressRepository : IAddressRepository
    {
        private readonly DatabaseContext _db;
        private readonly IMapper _mapper;

        public AddressRepository(DatabaseContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<APP.Address> Create(AddressDto addressDto, CancellationToken cancellationToken)
        {
            var record = _mapper.Map<Address>(addressDto);
            await _db.Addresses.AddAsync(record);
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
            var order = await _db.Addresses
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync(cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<APP.Address>> GetAll(CancellationToken cancellationToken)
        {
            var records = await _db.Addresses
               .AsNoTracking()
               .ToListAsync(cancellationToken);
            return records;
        }

        public Task<AddressDto> GetBy(int Id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Update(AddressDto addressDto, CancellationToken cancellationToken)
        {
            var record = await _mapper.ProjectTo<AddressDto>(_db.Set<AddressDto>())
                .Where(x => x.Id == addressDto.Id).FirstOrDefaultAsync();
            _mapper.Map(addressDto, record);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
