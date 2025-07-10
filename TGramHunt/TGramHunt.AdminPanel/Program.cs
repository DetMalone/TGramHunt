using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using TGramHunt.Contract;
using TGramHunt.Helpers;
using TGramHunt.Services.Services;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.AdminPanel
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthentication(o =>
             {
                 o.DefaultScheme = IdentityConstants.ApplicationScheme;
                 o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
             })
            .AddIdentityCookies(o => { });

            builder.Services.ConfigureMongo(
                builder.Configuration
                .GetSection("MongoSettings:Connection")
                .Value,
                builder.Configuration
                .GetSection("MongoSettings:DatabaseName")
                .Value);

            builder.Services.AddIdentityCore<AdminUser>()
                .AddMongoDbStores<AdminUser, Role, Guid>(builder.Configuration.GetSection("MongoSettings:Connection").Value,
                builder.Configuration.GetSection("MongoSettings:DatabaseName").Value)
                .AddRoles<Role>()
                .AddSignInManager()
                .AddRoleManager<RoleManager<Role>>()
                .AddUserManager<UserManager<AdminUser>>();

            builder.Services.AddScoped<IAdminUserService, AdminUserService>();

            var app = builder.Build();

            _CreateRolesIfNotExist(app.Services).Wait();
            _CreateSuperAdminIfNotExist(app.Services, app.Configuration).Wait();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Products}/{action=Index}/{id?}");

            app.Run();
        }

        private static async Task _CreateRolesIfNotExist(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                //initializing custom roles 
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                string[] roleNames = { Constants.Admin, Constants.Superadmin, Constants.Moderator };
                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        //create the roles and seed them to the database: Question 1
                        roleResult = await roleManager.CreateAsync(
                            new Role { Name = roleName, NormalizedName = roleName.ToUpper() });
                    }
                }
            }
        }
        private static async Task _CreateSuperAdminIfNotExist(IServiceProvider services, IConfiguration configuration)
        {
            using (var scope = services.CreateScope())
            {
                //Here you could create a super user who will maintain the web app
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AdminUser>>();
                var superAdmin = await userManager.FindByNameAsync(configuration.GetSection("MongoSettings:Superadmin:Name").Value);
                if (superAdmin == null)
                {
                    var superAdminUser = new AdminUser(
                        configuration.GetSection("MongoSettings:Superadmin:Name").Value,
                        configuration.GetSection("MongoSettings:Superadmin:Email").Value);

                    string password = configuration.GetSection("MongoSettings:Superadmin:Password").Value;
                    var createSuperadmin = await userManager.CreateAsync(superAdminUser, password);
                    if (createSuperadmin.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superAdminUser, Constants.Superadmin);
                    }
                }
            }
        }
    }
}