using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages.Admin.ProductPage
{
    public class ProductListModel(IProductRepository productRepository) : PageModel
    {
        public IEnumerable<Product> products { get; set; }
        public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            products = await productRepository.GetAll(cancellationToken);
            return Page();
        }

        public async Task<IActionResult> OnGetDelete(int Id,CancellationToken cancellationToken)
        {
            await productRepository.Delete(Id, cancellationToken);

            return LocalRedirect("/Admin/ProductPage/ProductList");
        }
    }
}
