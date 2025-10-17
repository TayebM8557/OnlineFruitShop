using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineFruit_Data.Entity;
using OnlineFruit_Data.Entity.Dtos;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Business.Repository.IReposi
{
    public interface IUserRepository 
    {
        Task<List<APP.User>> GetAll(CancellationToken cancellationToken);
        Task<SignInResult> Login(UserDto userDto, CancellationToken cancellationToken);
        Task Update(UserDto userDto, CancellationToken cancellationToken);
        Task Delete(int Id, CancellationToken cancellationToken);
        Task<IdentityResult> Create(UserDto userDto, CancellationToken cancellationToken);
        Task<User> GetByEmail(string Email, CancellationToken cancellationToken);
        Task<User> GetById(int Id, CancellationToken cancellationToken);
        Task<User> GetByPhoneNumber(string PhoneNumber, CancellationToken cancellationToken);
        Task<User> GetByEmailOrPhone(string EmailOrPhone, CancellationToken cancellationToken);

    }
}
