using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using shopapp.business.Abstract;
using shopapp.business.Concrete;
using shopapp.dataaccess.Abstract;
using shopapp.dataaccess.Concrete.EfCore;
using shopapp.webui.EmailServices;
using shopapp.webui.Identity;

namespace shopapp.webui
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            // use appcontext
            services.AddDbContext<ApplicationContext>(options =>
                options.UseMySql(@"server=localhost;port=3306;database=ShopAppDB;user=root;password=onur123123"));

            // add identity
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();


            services.Configure<IdentityOptions>(options =>
            {
                // Password
                options.Password.RequireDigit = true; // Mutlaka sayısal veri girilmeli
                options.Password.RequireLowercase = true; // Mutlaka küçük harf olmak zorunda
                options.Password.RequireUppercase = true; // Mutlaka büyük harf olmak zorunda
                options.Password.RequiredLength = 6; // min 6 karakter olması gerekiyor
                options.Password.RequireNonAlphanumeric = true; // _ , @ gibi ifadeler olmak zorunda

                // Lockout
                options.Lockout.MaxFailedAccessAttempts = 5; // Kaç kere hatalı girebilir
                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(5); // kaç dk sonra tekrar giriş yapabilir.
                options.Lockout.AllowedForNewUsers = true; // Lockout aktif hale getirmek için.

                // User
                // options.User.AllowedUserNameCharacters = "";  // Olmasını istediğimiz karakterler.
                options.User.RequireUniqueEmail = true; // her kullanıcının farklı mail adresi olmalı.
                options.SignIn.RequireConfirmedEmail = true; // Email onayı
                options.SignIn.RequireConfirmedPhoneNumber = false; // Telefon Onayı
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login"; // Login ui path
                options.LogoutPath = "/account/logout"; // Logout ui path
                options.AccessDeniedPath = "/account/accessdenied"; // Access Denied ui path
                options.SlidingExpiration = true; // Cookie her istekten sonra 20dk verir.
                options.ExpireTimeSpan = TimeSpan.FromDays(365); // 20 dk default, Kaç dk saat gün cookie aktif olsun.
                options.Cookie = new CookieBuilder()
                {
                    HttpOnly = true, // Sadece tarayıcı http cookie erişebilir.
                    Name = ".ShopApp.Security.Cookie", // Cookie ismim 
                    SameSite = SameSiteMode.Strict, // Cookie sahip olsa bile aynı lokasyondan işlem yapmalı.
                    // (Cross-side attack?) burada token işlemi yapılıyor. 
                };
            });


            // MVC
            // Razor Pages
            // services.AddScoped<IProductRepository, MySQLProductRepository>();
            services.AddScoped<IProductRepository, EfCoreProductRepository>();
            services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();

            services.AddScoped<IEmailSender, SmtpEmailSender>(i =>
                new SmtpEmailSender(
                    _configuration["EmailSender:Host"],
                    _configuration.GetValue<int>("EmailSender:Port"),
                    _configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    _configuration["EmailSender:UserName"],
                    _configuration["EmailSender:Password"]
                )
            ); // Mail Service

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                SeedDatabase.Seed();
                app.UseDeveloperExceptionPage();
            }


            app.UseStaticFiles(); // wwwroot activate.

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "node_modules")),
                RequestPath = "/modules"
            });

            // identity service added.
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // localhost/Shop/Details/1 -> localhost/samsung-s5
                // endpoints.MapControllerRoute
                // (
                //     name:"details",
                //     pattern:"{url}",
                //     defaults: new {controller="Shop", action="Details"}
                // );
                
                endpoints.MapControllerRoute(
                    name: "adminroles",
                    pattern: "admin/role/list",
                    defaults: new {controller = "Admin", action = "RoleList"}
                );
                endpoints.MapControllerRoute(
                    name: "adminroles",
                    pattern: "admin/role/create",
                    defaults: new {controller = "Admin", action = "RoleCreate"}
                );
                endpoints.MapControllerRoute(
                    name: "adminroles",
                    pattern: "admin/role/edit/{id?}",
                    defaults: new {controller = "Admin", action = "RoleEdit"}
                );
                endpoints.MapControllerRoute(
                    name: "adminroles",
                    pattern: "admin/user/{id?}",
                    defaults: new {controller = "Admin", action = "UserEdit"}
                );
                endpoints.MapControllerRoute(
                    name: "adminusers",
                    pattern: "admin/user/list",
                    defaults: new {controller = "Admin", action = "UserList"}
                );
                
                endpoints.MapControllerRoute(
                    name: "adminproductlist",
                    pattern: "admin/products",
                    defaults: new {controller = "Admin", action = "ProductList"}
                );
                endpoints.MapControllerRoute(
                    name: "adminproductlist",
                    pattern: "admin/products/{id?}",
                    defaults: new {controller = "Admin", action = "Edit"}
                );

                // localhost/Search?q=Samsung
                endpoints.MapControllerRoute
                (
                    name: "search",
                    pattern: "{search}",
                    defaults: new {controller = "Shop", action = "Search"}
                );
                // localhost/Shop/List/1 -> localhost/products/telefon
                endpoints.MapControllerRoute
                (
                    name: "products",
                    pattern: "products/{category?}",
                    defaults: new {controller = "Shop", action = "List"}
                );

                // localhost:5000
                // localhost:5000/home
                // localhost:5000/product/details/2
                // localhost:5000/category/list
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}