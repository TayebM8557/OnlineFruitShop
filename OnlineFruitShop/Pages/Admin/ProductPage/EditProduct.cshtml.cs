using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineFruit_Business.Repository.IReposi;
using OnlineFruit_Data.Entity.Dtos;
using System.Threading;
using static OnlineFruit_Data.Entity.APP;

namespace OnlineFruitShop.Pages.Admin.ProductPage
{

    public class EditProductModel : PageModel
    {

        private readonly IProductRepository _productRepository;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private ICategoryRepository _categoryRepository;
        public EditProductModel(IProductRepository productRepository, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
            ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public IFormFile Upload { get; set; }
        public Product Product { get; set; }


        public IEnumerable<Category> Categories { get; set; }

        public async Task<IActionResult> OnGet(int Id, CancellationToken cancellationToken)
        {
            Product = await _productRepository.GetBy(Id, cancellationToken);
            Categories = await _categoryRepository.GetAll(cancellationToken);
            return Page();
        }

        public async Task<IActionResult> OnPostEdit(ProductDto productDto, CancellationToken cancellationToken)
        {
            Categories = await _categoryRepository.GetAll(cancellationToken);
            if (Upload != null)
            {
                var file = string.Format("{0}{1}\\{2}", _hostingEnvironment.ContentRootPath, "\\wwwroot\\images", Upload.FileName);
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await Upload.CopyToAsync(fileStream);
                }
            }

            if (productDto.ImageUrl != null && Upload == null)
            {
                productDto.ImageUrl = productDto.ImageUrl;
            }
            else
            {

                productDto.ImageUrl = Upload.FileName;
            }

           
            //if (ModelState.IsValid)
            //{

            await _productRepository.Update(productDto, cancellationToken);

            //}
            return LocalRedirect("/Admin/ProductPage/ProductList");
        }
    }
}
