using System.Linq;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using shopapp.entity;
using shopapp.webui.Helper;
using shopapp.webui.Models;

namespace shopapp.webui.Controllers
{
    public class ShopController : Controller
    {
        private IProductService _productService;

        public ShopController(IProductService productService)
        {
            this._productService = productService;
        }

        // localhost/products/telefon?page=1
        public IActionResult List(string category, int page = 1)
        {
            const int pageSize = 3;
            return View(new
                ProductListViewModel()
                {
                    Products = _productService.GetProductByCategory(category, pageSize, page),
                    PageInfo = new PageInfo(
                        _productService.GetCountByCategory(category),
                        pageSize,
                        page,
                        category)
                });
        }

        public IActionResult Details(string url)
        {
            if (url == null)
            {
                return NotFound();
            }

            var p = _productService.GetProductDetails(url);
            if (p == null)
            {
                return NotFound();
            }

            return View(new ProductDetailModel()
            {
                Product = p,
                Categories = p.ProductCategories.Select(i => i.Category).ToList()
            });
        }

        public IActionResult AddToCart(int id)
        {
            LocalCartHelper.AddCart(_productService.GetById(id));
            return Redirect("/");
        }

        public IActionResult RemoveToCart(int id)
        {
            LocalCartHelper.RemoveCart(id);
            return Redirect("/shop/cart");
        }

        // TODO The purchase screen can be added later but for now, it will be directed to the home screen.
        public IActionResult BuyCart()
        {
            LocalCartHelper.ClearCart();
            return Redirect("/");
        }

        public IActionResult Cart() => View(LocalCartHelper.CartList);

        [HttpGet]
        public IActionResult Edit(int id) => View(_productService.GetById(id));

        [HttpPost]
        public IActionResult Edit(Product p)
        {
            if (!ModelState.IsValid) return View(_productService.GetById(p.ProductId));
            _productService.Update(p);
            return Redirect("/");
        }

        public IActionResult RemoveProduct(int id)
        {
            _productService.Delete(_productService.GetById(id));
            return Redirect("/");
        }

        public IActionResult Search(string q) =>
            View(
                new ProductListViewModel()
                {
                    Products = _productService.GetSearchResult(q),
                    PageInfo = new PageInfo
                    (
                        _productService.GetAll().Count,
                        3,
                        1,
                        ""
                    )
                });
        
        public IActionResult CreateProduct() => Redirect("/");

        public IActionResult ProductList() => Redirect("/");
    }
}