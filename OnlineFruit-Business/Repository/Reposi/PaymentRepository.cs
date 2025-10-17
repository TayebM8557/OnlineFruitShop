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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DatabaseContext _db;
        private readonly IMapper _mapper;

        public PaymentRepository(DatabaseContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<APP.Payment> Create(PaymentDto paymentDto, CancellationToken cancellationToken)
        {
            var record = _mapper.Map<APP.Payment>(paymentDto);
            await _db.Payments.AddAsync(record);
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
            var order = await _db.Payments
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync(cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<APP.Payment>> GetAll(CancellationToken cancellationToken)
        {
            var records = await _db.Payments
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return records;
        }

        public Task<PaymentDto> GetBy(int Id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Update(PaymentDto paymentDto, CancellationToken cancellationToken)
        {
            var record = await _mapper.ProjectTo<PaymentDto>(_db.Set<APP.Payment>())
                .Where(x => x.Id == paymentDto.Id).FirstOrDefaultAsync();
            _mapper.Map(paymentDto, record);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
