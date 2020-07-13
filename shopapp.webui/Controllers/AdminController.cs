using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using shopapp.entity;
using shopapp.webui.Extensions;
using shopapp.webui.Models;
using shopapp.webui.Helper;
using shopapp.webui.Identity;

namespace shopapp.webui.Controllers
{
    // Onur Taşdemir => Admin
    // Onur => Customer
    // OnurBaba8 =>  
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;

        public AdminController(IProductService productService, ICategoryService categoryService,
            RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            this._productService = productService;
            this._categoryService = categoryService;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(i => i.Name);

                ViewBag.Roles = roles;
                return View(new UserDetailsModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles
                });
            }

            return Redirect("~/admin/user/list");
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailsModel model, string[] selectedRoles)
        {
            // ViewBag.SelectedRoles = selectedRoles;
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    selectedRoles = selectedRoles ?? new string[] { };
                    await _userManager.AddToRolesAsync(
                        user,
                        selectedRoles.Except(userRoles).ToArray<string>()// Except => Hariç
                    );

                    await _userManager.RemoveFromRolesAsync(
                        user,
                        userRoles.Except(selectedRoles).ToArray<string>()
                    );

                    return Redirect("/admin/user/list");
                }
            }
            return Redirect("/admin/user/list");
        }


        public async Task<IActionResult> UserDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                    await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("UserList");
        }

        public IActionResult UserList() =>
            View(_userManager.Users);


        public async Task<IActionResult> RemoveRole(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var identityRole = await _roleManager.FindByIdAsync(id);
                if (identityRole != null)
                    await _roleManager.DeleteAsync(identityRole);
            }

            return RedirectToAction("RoleList");
        }

        [HttpGet]
        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var members = new List<User>();
            var nonMembers = new List<User>();

            if (role == null) return RedirectToAction("RoleList");

            foreach (var user in _userManager.Users.ToList())
            {
                var list = await _userManager.IsInRoleAsync(user, role.Name)
                    ? members
                    : nonMembers;
                list.Add(user);
            }

            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            if (!ModelState.IsValid) return View(model.RoleId);

            foreach (var userId in model.IdsToAdd ?? new string[] { })
            {
                var user = await _userManager.FindByIdAsync(userId);

                var result = await _userManager.AddToRoleAsync(user, model.RoleName);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            foreach (var userId in model.IdsToRemove ?? new string[] { })
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            return Redirect("/admin/role/edit/" + model.RoleId);
        }

        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        [HttpGet]
        public IActionResult RoleCreate() => View();

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));

            if (result.Succeeded) return RedirectToAction("RoleList");

            foreach (var item in result.Errors)
            {
                TempData.Put("message", $"{item}\n");
            }

            return View();
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