using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DatabaseContext _db;
        private readonly IMapper _mapper;

        public UserRepository(DatabaseContext db, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IdentityResult> Create(UserDto userDto, CancellationToken cancellationToken)
        {
            var user = new User();
            IdentityResult result;
            // تعیین UserName بر اساس ایمیل یا شماره موبایل
            string userName = !string.IsNullOrEmpty(userDto.Email) ? userDto.Email : userDto.PhoneNumber;
            
            if (userDto.Email == "tayeb.mora69@gmail.com")
            {
                user = new User
                {
                    UserName = userName,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName
                };
                result = await _userManager.CreateAsync(user, userDto.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
            else
            {
                if (userDto.Role == "Customer")
                {
                    user = new User
                    {
                        UserName = userName,
                        Email = userDto.Email,
                        PhoneNumber = userDto.PhoneNumber,
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName
                    };
                }
                if (userDto.Role == "Admin")
                {
                    user = new User
                    {
                        UserName = userName,
                        Email = userDto.Email,
                        PhoneNumber = userDto.PhoneNumber,
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName
                    };
                }

                result = await _userManager.CreateAsync(user, userDto.Password);
                if (result.Succeeded)
                {
                    if (userDto.Role == null)
                    {
                        await _userManager.AddToRoleAsync(user, "Customer");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, userDto.Role);
                    }
                }
                return result;
            }


            return result;
      
        }


        public async Task Delete(int Id, CancellationToken cancellationToken)
        {
            var User = await _db.Users.Where(x => x.Id == Id).FirstOrDefaultAsync(cancellationToken);
            
            await _db.SaveChangesAsync();
        }

        public async Task<List<APP.User>> GetAll(CancellationToken cancellationToken)
        {
            var records = await _db.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return records;
        }

        public async Task<User> GetByEmail(string Email, CancellationToken cancellationToken)
        {
            var User = await _userManager.Users.Where(e => e.Email == Email).FirstOrDefaultAsync();
            return User;
        }

        public async Task<User> GetById(int Id, CancellationToken cancellationToken)
        {
            var User = await _userManager.Users.Include(x => x.Address).Where(e => e.Id == Id).FirstOrDefaultAsync();
            return User;
        }

        public async Task<User> GetByPhoneNumber(string PhoneNumber, CancellationToken cancellationToken)
        {
            var User = await _userManager.Users.Where(e => e.PhoneNumber == PhoneNumber).FirstOrDefaultAsync();
            return User;
        }

        public async Task<User> GetByEmailOrPhone(string EmailOrPhone, CancellationToken cancellationToken)
        {
            // اگر شامل @ است، ایمیل است
            if (EmailOrPhone.Contains("@"))
            {
                return await GetByEmail(EmailOrPhone, cancellationToken);
            }
            // در غیر این صورت شماره موبایل است
            else
            {
                return await GetByPhoneNumber(EmailOrPhone, cancellationToken);
            }
        }

        public async Task<SignInResult> Login(UserDto userDto, CancellationToken cancellationToken)
        {
            User user = null;

            // جستجو بر اساس Email
            if (!string.IsNullOrEmpty(userDto.EmailOrPhone))
            {
                user = await _userManager.FindByEmailAsync(userDto.EmailOrPhone);
            }
            // جستجو بر اساس شماره موبایل
            else if (!string.IsNullOrEmpty(userDto.EmailOrPhone))
            {
                user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == userDto.PhoneNumber, cancellationToken);
            }

            if (user == null)
                return SignInResult.Failed;

            //// بررسی تایید
            //if (!user.EmailConfirmed && !user.PhoneNumberConfirmed)
            //    return SignInResult.NotAllowed;

            // لاگین با نام کاربری واقعی
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                userDto.Password,
                isPersistent: true,
                lockoutOnFailure: false);

            return result;
        }


        public async Task Update(UserDto userDto, CancellationToken cancellationToken)
        {
            var hasher = new PasswordHasher<User>();
            var record1 = await _db.Users
                .Where(x => x.Id == userDto.Id)
                .FirstOrDefaultAsync();
            record1.PasswordHash = hasher.HashPassword(null, userDto.Password);

            var record = _mapper.Map<User>(record1);
            var result = await _userManager.UpdateAsync(record);

        }
    }
}
