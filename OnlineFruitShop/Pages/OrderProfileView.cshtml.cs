using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using System.Threading;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages
{
    public class OrderProfileViewModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderProfileViewModel(UserManager<User> userManager, IHttpContextAccessor httpContext, IOrderItemRepository orderItemRepository) 
        {
            _userManager = userManager;
            _httpContext = httpContext;
            _orderItemRepository = orderItemRepository;
        }

        public OrderItem OrderItem { get; set; }
        public async Task<IActionResult> OnGet(int Id,CancellationToken cancellationToken)
        {
            var getUserId = _userManager.GetUserId(_httpContext.HttpContext.User);
            var GetId = Convert.ToInt32(getUserId);

            OrderItem = await _orderItemRepository.OrderItemByOrder(GetId, cancellationToken);

            
            return Page();
        }
    }
}
