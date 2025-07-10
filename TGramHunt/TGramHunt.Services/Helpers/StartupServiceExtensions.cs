using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using TGramHunt.Configurations;
using TGramHunt.Data.MongoContext;
using TGramHunt.Data.Repositories;
using TGramHunt.Data.Repositories.IRepositories;
using TGramHunt.Repositories;
using TGramHunt.Services.Helpers;
using TGramHunt.Services.Helpers.IHelpers;
using TGramHunt.Services.Middleware;
using TGramHunt.Services.Services;
using TGramHunt.Services.Services.IServices;

namespace TGramHunt.Helpers
{
    public static class StartupServiceExtensions
    {
        public static void ConfigureMongo(this IServiceCollection services, string connection, string dbName)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            services.Configure<Mongosettings>(opt =>
            {
                opt.Connection = connection;
                opt.DatabaseName = dbName;
            });

            services.AddSingleton<IMongoDBContext, MongoDBContext>();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserVotesForProductsServices, UserVotesForProductsServices>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductVoteService, ProductVoteService>();
            services.AddScoped<ISmallFilesService, SmallFilesService>();
            services.AddScoped<IImageHelper, ImageHelper>();
            services.AddScoped<ITelegramUserService, TelegramUserService>();
            services.AddScoped<ILoggService, LoggService>();
            services.AddScoped<ISystemSettingsService, SystemSettingsService>();
            services.AddScoped<ITgTagHelper, TgTagHelper>();
            services.AddScoped<ITgTagHelperExtensions, TgTagHelperExtensions>();
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<
                IUserVotesForProductsRepositories,
                UserVotesForProductsRepositories>();
            services.AddScoped<ISmallFilesRepository, SmallFilesRepository>();
            services.AddScoped<ILoggRepository, LoggRepository>();
            services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();
        }

        public static void ConfigureAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(x => configuration.GetSection(nameof(AppSettings)).Bind(x));
        }

        public static void ConfigureMiddleware(this IServiceCollection services)
        {
            services.AddScoped<LoggerMiddleware>();
        }
    }
}