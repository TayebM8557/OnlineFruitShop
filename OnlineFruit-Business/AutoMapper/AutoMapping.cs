using AutoMapper;
using OnlineFruit_Data.Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineFruit_Data.Entity;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruit_Business.AutoMapper

{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Address, AddressDto>().ReverseMap();
 
            CreateMap<Category, CategoryDto>().ReverseMap();
  
            CreateMap<Order, OrderDto>().ReverseMap();
        
            CreateMap<User, UserDto>().ReverseMap();
        
            CreateMap<Product, ProductDto>().ReverseMap();

            CreateMap<OrderItem, OrderItemDto>().ReverseMap();

            CreateMap<APP.Payment, PaymentDto>().ReverseMap();

            CreateMap<CartItem, CartItemDto>().ReverseMap();




        }
    }
}
