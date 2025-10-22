using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using System.Threading;
using OnlineFruit_Data.Context;
using static OnlineFruit_Data.Entity.APP;
using Microsoft.EntityFrameworkCore;

namespace OnlineFruitShop.Pages
{
    public class OrderProfileModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;
        private readonly DatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;

        public OrderProfileModel(IOrderRepository orderRepository ,DatabaseContext databaseContext ,UserManager<User> userManager, IHttpContextAccessor httpContext, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _databaseContext = databaseContext;
            _userManager = userManager;
            _httpContext = httpContext;
            _userRepository = userRepository;
        }

        public List<Order> Orders { get; set; }

        public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            var getUserId = _userManager.GetUserId(_httpContext.HttpContext.User);
            var userId = Convert.ToInt32(getUserId);

            //Orders = await _databaseContext.Orders
            //    .Include(o => o.)
            //    .Where(o => o.UserId == userId)
            //    .OrderByDescending(o => o.OrderDate)
            //    .ToListAsync(cancellationToken);

            return Page();
        }
    }
}
