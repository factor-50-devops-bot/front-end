using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HelpMyStreetFE.Models.Yoti;
using HelpMyStreetFE.Models.Email;

namespace HelpMyStreetFE
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                });
            services.AddControllersWithViews();
            services.Configure<YotiOptions>(Configuration.GetSection("Yoti"));
            services.Configure<EmailConfig>(Configuration.GetSection("SendGrid"));
            services.AddHttpClient<IUserRepository, UserRepository>();
            services.AddHttpClient<IValidationRepository, ValidationRepository>();
            services.AddHttpClient<IAddressRepository, AddressRepository>();
            services.AddHttpClient<IAddressService, AddressService>();
            services.AddHttpClient<IValidationService, ValidationService>();            
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IAuthService, AuthService>();            
            services.AddControllers();
            services.AddRazorPages()
            .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "about",
                    pattern: "about-us",
                    defaults: new { controller = "Pages", action = "AboutUs" });
                endpoints.MapControllerRoute(
                    name: "community",
                    pattern: "community",
                    defaults: new { controller = "Pages", action = "Community" });
                endpoints.MapControllerRoute(
                    name: "privacy",
                    pattern: "privacy-policy",
                    defaults: new { controller = "Pages", action = "PrivacyPolicy" });
                endpoints.MapControllerRoute(
                    name: "terms",
                    pattern: "terms-conditions",
                    defaults: new { controller = "Pages", action = "Terms" });
                endpoints.MapControllerRoute(
                    name: "resources",
                    pattern: "resources",
                    defaults: new { controller = "Pages", action = "Resources" });
                endpoints.MapControllerRoute(
                    name: "questions",
                    pattern: "questions",
                    defaults: new { controller = "Pages", action = "Questions" });
                endpoints.MapControllerRoute(
                    name: "contact",
                    pattern: "contact-us",
                    defaults: new { controller = "Pages", action = "ContactUs" });

                // Enable attribute routing
                //endpoints.MapControllers();
            });
        }
    }
}
