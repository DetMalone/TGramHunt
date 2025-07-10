using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TGramHunt.Contract;
using TGramHunt.Helpers;
using TGramHunt.Middleware;
using TGramHunt.Services.Helpers;
using TGramHunt.Services.Middleware;

namespace TGramHunt
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services
                .AddRazorPages(options =>
                {
                    options.Conventions
                        .AuthorizePage("/Products/New");
                    options.Conventions
                        .ConfigureFilter(new SanitizerFilter());

                    options.Conventions
                        .AddPageRoute("/Index", "/home");
                    options.Conventions
                        .AddPageRoute(
                            LinkHelper.notFound,
                            "{url:regex(^*$)}");
                    options.Conventions
                        .AddPageRoute(
                            LinkHelper.notFound,
                            "{*url:regex(^*$)}");
                })
                .AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                    fv.ImplicitlyValidateChildProperties = false;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowForClose",
                    policy =>
                    policy.Requirements
                    .Add(new AllowForCloseRequirement()));
            });
            services.AddControllers();
            services.AddOptions();

            services.AddAntiforgery(o =>
            {
                o.HeaderName = "XSRF-TOKEN";
            });

            ConfigureAutoMapper(services);

            services.ConfigureRepositories();
            services.ConfigureAppSettings(this.Configuration);
            services.AddAuthentication();

            services.ConfigureMongo(
                Configuration
                .GetSection("MongoSettings:Connection")
                .Value,
                Configuration
                .GetSection("MongoSettings:DatabaseName")
                .Value);

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(o => { });

            services.AddIdentityCore<User>(opts =>
            {
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;

                opts.User.RequireUniqueEmail = false;
                opts.User.AllowedUserNameCharacters = null;
            })
                   .AddRoles<Role>()
                   .AddMongoDbStores<User, Role, Guid>(Configuration.GetSection("MongoSettings:Connection").Value,
                   Configuration.GetSection("MongoSettings:DatabaseName").Value)
                   .AddSignInManager()
                   .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "TGramHunt";
                options.Cookie.HttpOnly = true;
                options.LogoutPath = LinkHelper.authLogout;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });

            const int maxRequestLimit = 25000000;
            // If using IIS
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = maxRequestLimit;
            });

            // If using Kestrel
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = maxRequestLimit;
            });

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = maxRequestLimit;
                x.MultipartBodyLengthLimit = maxRequestLimit;
                x.MultipartHeadersLengthLimit = maxRequestLimit;
            });

            services.AddHttpClient();

            services.ConfigureRepositories();
            services.ConfigureServices();
            services.ConfigureMiddleware();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        private static void ConfigureAutoMapper(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AllowNullCollections = true;
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<LogOutHelper>();
            services
                .AddScoped<
                IAuthorizationMiddlewareResultHandler,
                AuthorizationMiddlewareResultHandlerCust>();
            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");
            app.UseMiddleware<LoggerMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}