using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Abp.AspNetCore;
using Serilog;

namespace PortfoyYonetimSistemi.Web.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddNewtonsoftJson();
            // Is kurali hatalari (UserFriendlyException) 400 doner — ABP varsayilani 500
            services.PostConfigure<Microsoft.AspNetCore.Mvc.MvcOptions>(o =>
            {
                for (var i = 0; i < o.Filters.Count; i++)
                    if (o.Filters[i] is Microsoft.AspNetCore.Mvc.ServiceFilterAttribute f && f.ServiceType == typeof(Abp.AspNetCore.Mvc.ExceptionHandling.AbpExceptionFilter))
                        o.Filters[i] = new Microsoft.AspNetCore.Mvc.ServiceFilterAttribute(typeof(ExceptionHandling.UserFriendlyExceptionFilter));
            });

            // JWT Authentication
            var jwtConfig = Configuration.GetSection("Authentication:JwtBearer");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig["Issuer"],
                    ValidAudience = jwtConfig["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtConfig["SecurityKey"]))
                };
                // Pasiflestirilen/silinen kullanicinin token'i suresi dolana kadar (24 saat) gecerli kalmasin
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async ctx =>
                    {
                        var idClaim = ctx.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                        if (!long.TryParse(idClaim, out var userId)) { ctx.Fail("Gecersiz token"); return; }
                        var connStr = Configuration.GetConnectionString("Default");
                        var optionsBuilder = new DbContextOptionsBuilder();
                        optionsBuilder.UseNpgsql(connStr);
                        using var db = new PortfoyYonetimSistemi.EntityFrameworkCore.PortfoyYonetimSistemiDbContext(optionsBuilder.Options);
                        var active = await db.AppUsers.IgnoreQueryFilters().AnyAsync(u => u.Id == userId && u.IsActive && !u.IsDeleted);
                        if (!active) ctx.Fail("Kullanici pasif");
                    },
                };
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "PortfoyYonetimSistemi API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                // Aynı isimli ama farklı namespace'teki DTO'lar (örn her state-machine entity'nin
                // ChangeStatusInput'u) default schemaId çakışması yaratıp swagger.json'u 500'e
                // düşürüyordu — FullName ile benzersizleştir.
                options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer {token}'"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddCors();
            services.AddHostedService<MigrationHostedService>();
            services.AddHttpClient();
            // Zamanlanmis akislar — bu kayit yokken JWT projelerinde schedule tetikleyicileri hic calismiyordu
            services.AddHostedService<PortfoyYonetimSistemi.Flows.FlowSchedulerService>();
            // Required by JwtPermissionChecker — reads JWT "permission" claims from HttpContext.User
            services.AddHttpContextAccessor();

            // Sistem loglari: kuyruktaki Warning+ kayitlari veritabanina yazar + saklama suresi temizligi
            services.AddHostedService<PortfoyYonetimSistemi.Web.Host.Logging.SystemLogWriter>();
            // MS ILogger → Serilog (Program'daki Log.Logger) + istek logu icin DiagnosticContext
            services.AddSerilog();
            // ABP Logger → Serilog (yapilandirilmazsa NullLogger: hatalar hicbir yere yazilmiyordu)
            return services.AddAbp<PortfoyYonetimSistemiWebHostModule>(options =>
                options.IocManager.IocContainer.AddFacility<Castle.Facilities.Logging.LoggingFacility>(f => f.LogUsing<PortfoyYonetimSistemi.Web.Host.Logging.SerilogCastleFactory>()));
        }

        public void Configure(IApplicationBuilder app)
        {
            // CORS must run AFTER UseRouting and BEFORE UseAuthentication
            // (ASP.NET Core middleware ordering). With UseAbp/UseCors first,
            // preflight OPTIONS gets intercepted before CORS headers are written.
            app.UseAbp();
            app.UseRouting();
            app.UseCors(b => b
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            app.UseAuthentication();
            // Log baglami: her log satirinda kullanici, yol ve istek kimligi
            app.Use(async (ctx, next) =>
            {
                using (Serilog.Context.LogContext.PushProperty("UserId", ctx.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value))
                using (Serilog.Context.LogContext.PushProperty("RequestPath", ctx.Request.Path.Value))
                using (Serilog.Context.LogContext.PushProperty("RequestId", ctx.TraceIdentifier))
                    await next();
            });
            app.UseSerilogRequestLogging();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "PortfoyYonetimSistemi API V1");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
