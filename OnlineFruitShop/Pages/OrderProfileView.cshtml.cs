using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using OnlineFruit_Data.Context;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages
{
    public class OrderProfileViewModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly DatabaseContext _databaseContext;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderProfileViewModel(UserManager<User> userManager,DatabaseContext databaseContext, IHttpContextAccessor httpContext, IOrderItemRepository orderItemRepository) 
        {
            _userManager = userManager;
            _databaseContext = databaseContext;
            _httpContext = httpContext;
            _orderItemRepository = orderItemRepository;

        }
    
    public Order Order { get; set; }
    public Payment Payment { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Order = await _databaseContext.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (Order == null)
            return NotFound();

        Payment = await _databaseContext.Set<Payment>()
            .FirstOrDefaultAsync(p => p.OrderId == id);

        return Page();
        }

    }
}
