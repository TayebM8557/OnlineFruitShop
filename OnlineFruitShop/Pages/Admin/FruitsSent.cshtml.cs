using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages.Admin
{
    public class FruitsSentModel : PageModel
    {
        private readonly IOrderItemRepository _orderRepository;
        private readonly IOrderRepository _order;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;

        public FruitsSentModel(IOrderItemRepository orderRepository, IOrderRepository order, UserManager<User> userManager, IHttpContextAccessor httpContext, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _order = order;
            _userManager = userManager;
            _httpContext = httpContext;
            _userRepository = userRepository;
        }
        public IEnumerable<OrderItem> Orders { get; set; }
        public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            var getUserId = _userManager.GetUserId(_httpContext.HttpContext.User);
            var GetId = Convert.ToInt32(getUserId);

            Orders = await _orderRepository.GetAllStatus(cancellationToken);
            return Page();
        }
    }
}
