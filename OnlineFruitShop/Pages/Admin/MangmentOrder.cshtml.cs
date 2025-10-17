using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages.Admin
{
    public class MangmentOrderModel : PageModel
    {
        private readonly IOrderItemRepository _orderRepository;
        private readonly IOrderRepository _order;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;

        public MangmentOrderModel(IOrderItemRepository orderRepository , IOrderRepository order, UserManager<User> userManager, IHttpContextAccessor httpContext, IUserRepository userRepository)
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

            Orders = await _orderRepository.GetAll( cancellationToken);
            return Page();
        }

        public async Task<IActionResult> OnGetChange(int Id, CancellationToken cancellationToken)
        {
            var getUserId = _userManager.GetUserId(_httpContext.HttpContext.User);
            var GetId = Convert.ToInt32(getUserId);

            Orders = await _orderRepository.GetAll(cancellationToken);
            await _order.ChangeStatus(GetId, Id, cancellationToken);
            return Page();
        }
    }
}
