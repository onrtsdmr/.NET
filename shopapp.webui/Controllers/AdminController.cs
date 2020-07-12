using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.entity;
using shopapp.webui.Models;
using shopapp.webui.Helper;

namespace shopapp.webui.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;

        public AdminController(IProductService productService, ICategoryService categoryService)
        {
            this._productService = productService;
            this._categoryService = categoryService;
        }

        public IActionResult ProductList() => View(new ProductListViewModel() {Products = _productService.GetAll()});

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductModel p)
        {
            if (!ModelState.IsValid) return View(p);
            var entity = new Product()
            {
                Name = p.Name,
                Url = p.Url,
                Price = p.Price,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
            };
            if (_productService.Create(entity))
            {
                TempData["Message"] = AppHelper.CreateMessage($"{entity.Name} isimli ürün eklendi.", "success");
                return RedirectToAction("ProductList");
            }

            return View(p);
        }

        [HttpGet]
        public IActionResult EditProduct(int? id)
        {
            if (id == null)
            {
                NotFound();
            }

            var entity = _productService.GetByIdWithCategory((int) id);
            if (entity == null)
            {
                NotFound();
            }

            var model = new ProductModel()
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Url = entity.Url,
                Description = entity.Description,
                Price = entity.Price,
                ImageUrl = entity.ImageUrl,
                IsApproved = entity.IsApproved,
                IsHome = entity.IsHome,
                SelectedCategories = entity.ProductCategories.Select(i => i.Category).ToList()
            };

            ViewBag.Categories = _categoryService.GetAll();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel p, int[] categoryIds, bool isHomes, bool isApproveds,
            IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetAll();
                return View(p);
            }

            var entity = _productService.GetById(p.ProductId);
            if (entity == null)
            {
                NotFound();
            }

            entity.Name = p.Name;
            entity.Url = p.Url;
            entity.Description = p.Description;
            entity.Price = p.Price;
            entity.IsApproved = p.IsApproved;
            entity.IsHome = p.IsHome;

            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extension}");
                entity.ImageUrl = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            if (_productService.Update(entity, categoryIds))
            {
                TempData["Message"] = AppHelper.CreateMessage($"{entity.Name} isimli ürün değiştirildi.", "warning");
                return RedirectToAction("ProductList");
            }

            TempData["Message"] = AppHelper.CreateMessage(_productService.ErrorMessage, "danger");
            ViewBag.Categories = _categoryService.GetAll();
            return View(p);
        }

        public IActionResult RemoveProduct(int id)
        {
            var entity = _productService.GetById(id);
            if (entity != null)
            {
                TempData["Message"] = AppHelper.CreateMessage($"{entity.Name} isimli ürün silindi.", "danger");
                _productService.Delete(entity);
            }

            return RedirectToAction("ProductList");
        }

        public IActionResult CategoryList() => View(new CategoryListViewModel()
            {CategoryModels = _categoryService.GetAll()});

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel c)
        {
            if (!ModelState.IsValid) return View(c);
            var entity = new Category()
            {
                Name = c.Name,
                Url = c.Url
            };
            if (_categoryService.Create(entity))
            {
                TempData["Message"] = AppHelper.CreateMessage($"{entity.Name} isimli kategori eklendi.", "success");
                return RedirectToAction("CategoryList");
            }

            TempData["Message"] = AppHelper.CreateMessage(_categoryService.ErrorMessage, "danger");

            return View(c);
        }

        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            if (id == null)
            {
                NotFound();
            }

            var entity = _categoryService.GetByIdWithProduct((int) id);
            return View(
                new CategoryModel()
                {
                    CategoryId = entity.CategoryId,
                    Name = entity.Name,
                    Url = entity.Url,
                    Products = entity.ProductCategories.Select(p => p.Product).ToList()
                }
            );
        }

        [HttpPost]
        public IActionResult EditCategory(CategoryModel c)
        {
            if (!ModelState.IsValid) return View(c);
            var entity = _categoryService.GetById(c.CategoryId);
            if (entity == null)
            {
                NotFound();
            }

            entity.Name = c.Name;
            entity.Url = c.Url;
            _categoryService.Update(entity);
            TempData["Message"] = AppHelper.CreateMessage($"{entity.Name} isimli kategori değiştirildi.", "warning");
            return RedirectToAction("CategoryList");
        }

        public IActionResult RemoveCategory(int id)
        {
            var entity = _categoryService.GetById(id);
            if (entity != null)
            {
                TempData["Message"] = AppHelper.CreateMessage($"{entity.Name} isimli kategori silindi.", "danger");
                _categoryService.Delete(entity);
            }

            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteCategoryRelation(int productId, int categoryId)
        {
            _categoryService.DeleteFromCategory(productId, categoryId);
            return RedirectToAction("CategoryList");
        }
    }
}