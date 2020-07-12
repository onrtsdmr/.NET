using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using shopapp.webui.EmailServices;
using shopapp.webui.Extensions;
using shopapp.webui.Helper;
using shopapp.webui.Identity;
using shopapp.webui.Models;

namespace shopapp.webui.Controllers
{
    [AutoValidateAntiforgeryToken] // Tüm post methodlarında token kontrolü yapılır.
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            IEmailSender emailSender)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginModel()
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Token kontrolü
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // var user = await _userManager.FindByNameAsync(model.UserName);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Message = "There is no account for this user name.",
                    Type = "danger"
                });
                // TempData["message"] = AppHelper.CreateMessage("There is no account for this user name.", "danger");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                TempData["message"] = AppHelper.CreateMessage("Please confirm your account.", "warning");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, // Kullanıcı adına göre bir sorgu yapılacak.
                model.Password, // Kullanıcı şifresi
                false, // Tarayıcı kapandığında çerez silinir.
                false // Hesap kilitleme işlemi.
            );

            if (result.Succeeded) return Redirect(model.ReturnUrl ?? "~/");
            TempData["message"] = AppHelper.CreateMessage("Incorrect email or password, please try again.", "danger");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Generate Token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code
                });
                // Email
                await _emailSender.SendEmailAsync
                (
                    model.Email,
                    "Confirm your account.",
                    $"<a href='https://localhost:5001{url}'>Click</a> on the link to confirm your account."
                );

                return Redirect("/Account/Login");
            }

            ModelState.AddModelError("", "Unexpected error. Please try again.");
            TempData["Message"] = AppHelper.CreateMessage("Email or username registered in the system.", "danger");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        public IActionResult Manage() => View();


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData["message"] = AppHelper.CreateMessage("Invalid token.", "danger");
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    TempData["message"] = AppHelper.CreateMessage("Your account has been approved.", "success");
                    return View();
                }

                TempData["message"] = AppHelper.CreateMessage("Unexpected error.", "danger");
                return View();
            }

            TempData["message"] = AppHelper.CreateMessage("Your account has not been approved.", "danger");
            return View();
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["message"] = AppHelper.CreateMessage("Email not be null.", "danger");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user!=null)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var url = Url.Action("ResetPassword", "Account", new
                {
                    userId = user.Id,
                    token = resetToken,
                    email = user.Email
                });
                await _emailSender.SendEmailAsync
                (
                    email,
                    "Reset your password.",
                    $"<a href='https://localhost:5001{url}'>Click</a> on the link to reset your pass."
                );
                TempData["message"] = AppHelper.CreateMessage("Email sent, check your inbox.", "success");
                return Redirect("~/");
            }
            TempData["message"] = AppHelper.CreateMessage("Email is not registered in the system.", "danger");
            return View(email);
        }
        [HttpGet]
        public IActionResult ResetPassword(string userId, string token, string email)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return Redirect("~/");
            
            var model = new ResetPasswordModel()
            {
                Token = token,
                Email = email
            };
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user!=null)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                {
                    return Redirect("~/Account/Login");
                }
                
                return View(model);
            }

            return Redirect("~/Account/Login");
        }
    }
}