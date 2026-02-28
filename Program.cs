using ePermitsApp.Data;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ePermitsApp.Services.Interfaces;
using ePermitsApp.Services;
using ePermitsApp.Mappings;

using ePermits.Data;
using ePermits.Services;
using ePermits.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ePermitsApp.Helpers;
using ePermitsApp.Models;
using Microsoft.OpenApi.Models;

namespace ePermitsApp
{
    public class Program
    {        
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.Configure<List<SampleUser>>(
                builder.Configuration.GetSection("SampleUsers"));

            builder.Services.Configure<FileStorageSettings>(
                builder.Configuration.GetSection("FileStorage"));

            builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
            builder.Services.AddScoped<IProvinceService, ProvinceService>();
            builder.Services.AddScoped<ILGURepository, LGURepository>();
            builder.Services.AddScoped<ILGUService, LGUService>();
            builder.Services.AddScoped<IBarangayRepository, BarangayRepository>();
            builder.Services.AddScoped<IBarangayService, BarangayService>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IRequirementClassificationRepository, RequirementClassificationRepository>();
            builder.Services.AddScoped<IRequirementClassificationService, RequirementClassificationService>();
            builder.Services.AddScoped<IRequirementCategoryRepository, RequirementCategoryRepository>();
            builder.Services.AddScoped<IRequirementCategoryService, RequirementCategoryService>();
            builder.Services.AddScoped<IRequirementRepository, RequirementRepository>();
            builder.Services.AddScoped<IRequirementService, RequirementService>();
            builder.Services.AddScoped<IPermitApplicationTypeRepository, PermitApplicationTypeRepository>();
            builder.Services.AddScoped<IPermitApplicationTypeService, PermitApplicationTypeService>();
            builder.Services.AddScoped<IOccupancyNatureRepository, OccupancyNatureRepository>();
            builder.Services.AddScoped<IOccupancyNatureService, OccupancyNatureService>();
            builder.Services.AddScoped<IProjectClassificationRepository, ProjectClassificationRepository>();
            builder.Services.AddScoped<IProjectClassificationService, ProjectClassificationService>();
            builder.Services.AddScoped<IApplicantTypeRepository, ApplicantTypeRepository>();
            builder.Services.AddScoped<IApplicantTypeService, ApplicantTypeService>();
            builder.Services.AddScoped<IOwnershipTypeRepository, OwnershipTypeRepository>();
            builder.Services.AddScoped<IOwnershipTypeService, OwnershipTypeService>();
            builder.Services.AddScoped<IBuildingPermitRepository, BuildingPermitRepository>();
            builder.Services.AddScoped<IBuildingPermitService, BuildingPermitService>();
            builder.Services.AddScoped<ICoOAppRepository, CoOAppRepository>();
            builder.Services.AddScoped<ICoOAppService, CoOAppService>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<IUserRoleService, UserRoleService>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();

            // Chat additions
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Fix: Use the correct overload of AddAutoMapper that accepts an assembly marker type
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ProvinceProfile).Assembly));
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(LGUProfile).Assembly));
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(RequirementClassificationProfile).Assembly));
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(RequirementCategoryProfile).Assembly));
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(PermitApplicationTypeProfile).Assembly));
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(OccupancyNatureProfile).Assembly));
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ProjectClassificationProfile).Assembly));
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ApplicantTypeProfile).Assembly));
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(OwnershipTypeProfile).Assembly));
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(BuildingPermitProfile).Assembly));
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(CoOAppProfile).Assembly));

            builder.Services.AddControllers();

            // SignalR
            builder.Services.AddSignalR();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ePermits API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            var jwtKey = builder.Configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("The JWT key is not configured. Please ensure 'Jwt:Key' is set in the configuration.");
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                        )
                    };

                    // Allow JWT for SignalR
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/chatHub"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:8080")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/", () => Results.Ok("running"));

            app.MapControllers();

            // SignalR hub
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
