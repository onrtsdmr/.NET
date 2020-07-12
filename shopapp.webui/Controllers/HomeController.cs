using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using shopapp.dataaccess.Abstract;
using shopapp.webui.Models;


namespace shopapp.webui.Controllers
{
    // localhost:5000/home
    public class HomeController: Controller
    {
        private IProductService _productService;

        public HomeController(IProductService productService)
        {
            this._productService = productService;
        }

        // localhost:5000/home/index
        public IActionResult Index() =>  View(
            new ProductListViewModel()
            {
                Products = _productService.GetHomePageProducts(),
                PageInfo = new PageInfo
                (
                    _productService.GetAll().Count(),
                    3,
                    1,
                    ""
                )
            });

        // localhost:5000/home/about
        public IActionResult About() => View();
        public IActionResult Contact() => View("MyView");
    }
}