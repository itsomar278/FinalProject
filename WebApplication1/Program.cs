using DataAcess;
using DataAcess.Repositories;
using DataAcess.Repositories.ArticleRepository;
using DataAcess.Repositories.CommentRepository;
using DataAcess.Repositories.FavoriteRepository;
using DataAcess.Repositories.FollowRepository;
using DataAcess.Repositories.RefreshTokenRepository;
using DataAcess.Repositories.UsersRepository;
using DataAcess.UnitOfWorks;
using Domain;
using Domain.Services.ArticleService;
using Domain.Services.AuthService;
using Domain.Services.CommentService;
using Domain.Services.SessionService;
using Domain.Services.UsersService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;
using WebApplication1.Mapping;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try {
                var builder = WebApplication.CreateBuilder(args);
                builder.Logging.AddSerilog();
                builder.Host.UseSerilog();
                var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();
                // Add services to the container.
                builder.Services.AddControllers()
                    .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                builder.Services.AddAutoMapper(typeof(ArticlesProfile), typeof(CommentsProfile), typeof(UsersProfile) ,typeof(AuthProfile));
                builder.Services.AddDbContext<ProjectDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("FinalProject.DAL")));
                builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
                builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
                builder.Services.AddTransient<IUsersRepository, UsersRepository>();
                builder.Services.AddTransient<IFollowRepository, FollowRepository>();
                builder.Services.AddTransient<IArticleRepository, ArticleRepository>();
                builder.Services.AddTransient<ICommentsRepository, CommentsRepository>();
                builder.Services.AddTransient<IFavouriteRepository, FavouriteRepository>();
                builder.Services.AddTransient<IRefreshTokenRepository, RefreshTokensRepository>();
                builder.Services.AddTransient<ISessionDataManagment, SessionDataManagment>();
                builder.Services.AddTransient<ICommentsService, CommentsService>();
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
                builder.Services.AddScoped<IArticleService, ArticlesService>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<MyExceptionFilter>();
                builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromHours(1);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });
                builder.Services.AddDistributedMemoryCache();
                builder.Services.AddControllers().AddJsonOptions(x =>
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
                builder.Services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });

                    options.OperationFilter<SecurityRequirementsOperationFilter>();
                });
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
               .GetBytes(configuration.GetValue<string>("SecurityKey"))),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
                builder.Services.AddCors(options => options.AddPolicy(name: "NgOrigins", policy =>
                        {
                            policy.WithOrigins("https://localhost:7248/").AllowAnyMethod().AllowAnyHeader();
                        }));
                builder.Services.AddMvc( options => 
                { 
                    options.Filters.Add<MyExceptionFilter>();
                });
                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
                app.UseSession();

                app.UseSerilogRequestLogging();

                app.UseCookiePolicy();

                app.UseHttpsRedirection();

                app.UseAuthentication();

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            catch(Exception ex )
            {
                Log.Fatal(ex, " couldn't even start ");
            }
            finally
            {
                Log.CloseAndFlush();
            }
    }   }
    
  

}
