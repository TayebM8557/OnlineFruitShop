using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using System.Threading;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages
{
    public class OrderProfileModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;

        public OrderProfileModel(IOrderRepository orderRepository ,UserManager<User> userManager, IHttpContextAccessor httpContext, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _httpContext = httpContext;
            _userRepository = userRepository;
        }

        public IEnumerable<Payment> Orders { get; set; }
        public async Task<IActionResult> OnGet( CancellationToken cancellationToken)
        {
            var getUserId = _userManager.GetUserId(_httpContext.HttpContext.User);
            var GetId = Convert.ToInt32(getUserId);

            Orders = await _orderRepository.GetAllByUser(GetId, cancellationToken);
            return Page();
        }
    }
}
