using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortfoyYonetimSistemi.EntityFrameworkCore;
using PortfoyYonetimSistemi.EntityFrameworkCore.Seed;
using PortfoyYonetimSistemi.Entities;
using Serilog;

namespace PortfoyYonetimSistemi.Web.Host
{
    /// <summary>
    /// Background service that runs migration + seed once at startup
    /// without blocking the HTTP pipeline.
    /// </summary>
    public class MigrationHostedService : IHostedService
    {
        private readonly IConfiguration _config;

        public MigrationHostedService(IConfiguration config)
        {
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var connStr = _config.GetConnectionString("Default") ?? "";
            if (string.IsNullOrEmpty(connStr)) return;

            // Run in background to avoid blocking host startup (prevents EF tooling timeout)
            _ = Task.Run(async () =>
            {
                // Small delay to ensure host is fully started before DB operations
                await Task.Delay(1000, cancellationToken);
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder();
                    optionsBuilder.UseNpgsql(connStr);

                    using (var db = new PortfoyYonetimSistemiDbContext(optionsBuilder.Options))
                    {
                        // EnsureCreated bos veritabaninda en guncel semayi kurar ve true doner.
                        // Dolu veritabaninda hicbir sey yapmaz — o durumda bekleyen migration'lar
                        // SchemaMigrations.Apply icinde sirayla calistirilir.
                        var freshlyCreated = db.Database.EnsureCreated();
                        Console.WriteLine("[Migration] Migration tanimi yok — sema EnsureCreated ile yonetiliyor.");
                        // Sistem log tablolari (sonradan eklenen surumde dolu DB'de EnsureCreated calismaz)
                        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS \"SystemLogs\" (\"Id\" bigserial PRIMARY KEY, \"Time\" timestamp NOT NULL, \"Level\" text NULL, \"Message\" text NULL, \"Exception\" text NULL, \"Source\" text NULL, \"UserId\" bigint NULL, \"Path\" text NULL, \"RequestId\" text NULL)");
                        db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS \"SystemSettings\" (\"Id\" bigserial PRIMARY KEY, \"Key\" text NULL, \"Value\" text NULL)");
                        Console.WriteLine("[Migration] Database is up to date.");

                        // Seed sample data — wrapped in its own try so a failure here doesn't block RBAC seed below.
                        try
                        {
                    if (!db.Branches.Any())
                    {
                        db.Branches.AddRange(
                    new Branch { Id = 1, Name = "Şube 1", Code = "KOD-001", Region = "Örnek 1" },
                    new Branch { Id = 2, Name = "Şube 2", Code = "KOD-002", Region = "Örnek 2" }
                        );
                    }
                    if (!db.PortfolioManagers.Any())
                    {
                        db.PortfolioManagers.AddRange(
                    new PortfolioManager { Id = 3, FullName = "Portföy Yöneticisi 1", EmployeeCode = "KOD-001", Title = "Pazarlama bütçesi 2026", Phone = "+90 532 111 22 33", Email = "ayse.yilmaz@ornek.com.tr", IsActive = true, BranchId = 1 },
                    new PortfolioManager { Id = 4, FullName = "Portföy Yöneticisi 2", EmployeeCode = "KOD-002", Title = "Yazılım lisans yenileme", Phone = "+90 533 444 55 66", Email = "mehmet.demir@ornek.com.tr", IsActive = false, BranchId = 2 }
                        );
                    }
                    if (!db.Customers.Any())
                    {
                        db.Customers.AddRange(
                    new Customer { Id = 5, FullName = "Ayşe Yılmaz", CustomerCode = "KOD-001", CustomerType = (CustomerCustomerType)0, Phone = "+90 532 111 22 33", Email = "ayse.yilmaz@ornek.com.tr", RiskProfile = (CustomerRiskProfile)0, TotalAum = 12500.00m, OnboardingDate = DateTime.Today.AddDays(-3), PortfolioManagerId = 3 },
                    new Customer { Id = 6, FullName = "Mehmet Demir", CustomerCode = "KOD-002", CustomerType = (CustomerCustomerType)1, Phone = "+90 533 444 55 66", Email = "mehmet.demir@ornek.com.tr", RiskProfile = (CustomerRiskProfile)1, TotalAum = 48250.00m, OnboardingDate = DateTime.Today.AddDays(-24), PortfolioManagerId = 4 }
                        );
                    }
                    if (!db.CustomerManagerTransfers.Any())
                    {
                        db.CustomerManagerTransfers.AddRange(
                    new CustomerManagerTransfer { Id = 7, TransferDate = DateTime.Today.AddDays(-3), TransferReason = "Örnek 1", Notes = "Yıllık plan kapsamında hazırlandı.", CustomerId = 5, PortfolioManagerId = 3 },
                    new CustomerManagerTransfer { Id = 8, TransferDate = DateTime.Today.AddDays(-24), TransferReason = "Örnek 2", Notes = "Departman talebi doğrultusunda güncellendi.", CustomerId = 6, PortfolioManagerId = 4 }
                        );
                    }
                    if (!db.Funds.Any())
                    {
                        db.Funds.AddRange(
                    new Fund { Id = 9, FundCode = "KOD-001", FundName = "Fon 1", FundType = (FundFundType)0, Currency = (FundCurrency)0, BenchmarkIndex = "Örnek 1", ManagementFeeRate = 12500.00m, IsActive = true },
                    new Fund { Id = 10, FundCode = "KOD-002", FundName = "Fon 2", FundType = (FundFundType)1, Currency = (FundCurrency)1, BenchmarkIndex = "Örnek 2", ManagementFeeRate = 48250.00m, IsActive = false }
                        );
                    }
                    if (!db.CustomerFundHoldings.Any())
                    {
                        db.CustomerFundHoldings.AddRange(
                    new CustomerFundHolding { Id = 11, Units = 12500.00m, PurchasePrice = 12500.00m, CurrentValue = 12500.00m, PurchaseDate = DateTime.Today.AddDays(-3), Weight = 12500.00m, ProfitLoss = 12500.00m, CustomerId = 5, FundId = 9 },
                    new CustomerFundHolding { Id = 12, Units = 48250.00m, PurchasePrice = 48250.00m, CurrentValue = 48250.00m, PurchaseDate = DateTime.Today.AddDays(-24), Weight = 48250.00m, ProfitLoss = 48250.00m, CustomerId = 6, FundId = 10 }
                        );
                    }
                    if (!db.MarketDatas.Any())
                    {
                        db.MarketDatas.AddRange(
                    new MarketData { Id = 13, InstrumentName = "Piyasa Verisi 1", InstrumentCode = "KOD-001", InstrumentType = (MarketDataInstrumentType)0, DataDate = DateTime.Today.AddDays(-3), OpenPrice = 12500.00m, ClosePrice = 12500.00m, HighPrice = 12500.00m, LowPrice = 12500.00m, Volume = 12500.00m, NavPerUnit = 12500.00m, DailyReturnRate = 12500.00m, FundId = 9 },
                    new MarketData { Id = 14, InstrumentName = "Piyasa Verisi 2", InstrumentCode = "KOD-002", InstrumentType = (MarketDataInstrumentType)1, DataDate = DateTime.Today.AddDays(-24), OpenPrice = 48250.00m, ClosePrice = 48250.00m, HighPrice = 48250.00m, LowPrice = 48250.00m, Volume = 48250.00m, NavPerUnit = 48250.00m, DailyReturnRate = 48250.00m, FundId = 10 }
                        );
                    }
                    if (!db.TargetPeriods.Any())
                    {
                        db.TargetPeriods.AddRange(
                    new TargetPeriod { Id = 15, PeriodName = "Hedef Dönemi 1", PeriodType = (TargetPeriodPeriodType)0, StartDate = DateTime.Today.AddDays(-3), EndDate = DateTime.Today.AddDays(-3), Year = 42, IsActive = true },
                    new TargetPeriod { Id = 16, PeriodName = "Hedef Dönemi 2", PeriodType = (TargetPeriodPeriodType)1, StartDate = DateTime.Today.AddDays(-24), EndDate = DateTime.Today.AddDays(-24), Year = 17, IsActive = false }
                        );
                    }
                    if (!db.ManagerTargets.Any())
                    {
                        db.ManagerTargets.AddRange(
                    new ManagerTarget { Id = 17, Description = "Yıllık plan kapsamında hazırlandı.", TargetType = (ManagerTargetTargetType)0, TargetValue = 12500.00m, ActualValue = 12500.00m, Unit = "Örnek 1", AchievementRate = 12500.00m, Status = (ManagerTargetStatus)0, Notes = "Yıllık plan kapsamında hazırlandı.", RevisionNote = "Yıllık plan kapsamında hazırlandı.", TargetPeriodId = 15, PortfolioManagerId = 3 },
                    new ManagerTarget { Id = 18, Description = "Departman talebi doğrultusunda güncellendi.", TargetType = (ManagerTargetTargetType)1, TargetValue = 48250.00m, ActualValue = 48250.00m, Unit = "Örnek 2", AchievementRate = 48250.00m, Status = (ManagerTargetStatus)2, Notes = "Departman talebi doğrultusunda güncellendi.", RevisionNote = "Departman talebi doğrultusunda güncellendi.", TargetPeriodId = 16, PortfolioManagerId = 4 }
                        );
                    }
                    if (!db.WorkAssignments.Any())
                    {
                        db.WorkAssignments.AddRange(
                    new WorkAssignment { Id = 19, Title = "Pazarlama bütçesi 2026", Description = "Yıllık plan kapsamında hazırlandı.", AssignmentType = (WorkAssignmentAssignmentType)0, Priority = (WorkAssignmentPriority)0, DueDate = DateTime.Today.AddDays(-3), CompletedDate = DateTime.Today.AddDays(-3), Status = (WorkAssignmentStatus)0, CompletionNote = "Yıllık plan kapsamında hazırlandı.", RevisionNote = "Yıllık plan kapsamında hazırlandı.", PortfolioManagerId = 3 },
                    new WorkAssignment { Id = 20, Title = "Yazılım lisans yenileme", Description = "Departman talebi doğrultusunda güncellendi.", AssignmentType = (WorkAssignmentAssignmentType)1, Priority = (WorkAssignmentPriority)1, DueDate = DateTime.Today.AddDays(-24), CompletedDate = DateTime.Today.AddDays(-24), Status = (WorkAssignmentStatus)3, CompletionNote = "Departman talebi doğrultusunda güncellendi.", RevisionNote = "Departman talebi doğrultusunda güncellendi.", PortfolioManagerId = 4 }
                        );
                    }
                    if (!db.PerformanceReports.Any())
                    {
                        db.PerformanceReports.AddRange(
                    new PerformanceReport { Id = 21, ReportName = "Performans Raporu 1", ReportType = (PerformanceReportReportType)0, ReportDate = DateTime.Today.AddDays(-3), StartDate = DateTime.Today.AddDays(-3), EndDate = DateTime.Today.AddDays(-3), TotalAum = 12500.00m, AumChange = 12500.00m, AumChangeRate = 12500.00m, PortfolioReturnRate = 12500.00m, BenchmarkReturnRate = 12500.00m, AlphaValue = 12500.00m, CustomerCount = 42, NewCustomerCount = 42, TargetAchievementRate = 12500.00m, Summary = "Örnek 1", Status = (PerformanceReportStatus)0, RevisionNote = "Yıllık plan kapsamında hazırlandı.", PortfolioManagerId = 3, TargetPeriodId = 15 },
                    new PerformanceReport { Id = 22, ReportName = "Performans Raporu 2", ReportType = (PerformanceReportReportType)1, ReportDate = DateTime.Today.AddDays(-24), StartDate = DateTime.Today.AddDays(-24), EndDate = DateTime.Today.AddDays(-24), TotalAum = 48250.00m, AumChange = 48250.00m, AumChangeRate = 48250.00m, PortfolioReturnRate = 48250.00m, BenchmarkReturnRate = 48250.00m, AlphaValue = 48250.00m, CustomerCount = 17, NewCustomerCount = 17, TargetAchievementRate = 48250.00m, Summary = "Örnek 2", Status = (PerformanceReportStatus)2, RevisionNote = "Departman talebi doğrultusunda güncellendi.", PortfolioManagerId = 4, TargetPeriodId = 16 }
                        );
                    }
                            db.SaveChanges();
                            Console.WriteLine("[Seed] Sample data created.");
                        }
                        catch (Exception sampleEx)
                        {
                            Console.WriteLine($"[Seed] Sample data skipped: {sampleEx.GetType().Name}: {sampleEx.Message}");
                            // Carry on — RBAC seed must still run so admin/123qwe is usable.
                        }
                        // Sync identity sequences to MAX(Id). Seeded rows carry explicit Ids which do NOT
                        // advance Postgres identity sequences → nextval collides with a seed row and the
                        // first few inserts fail with a duplicate-key 500. Runs every startup; idempotent.
                        try
                        {
                            foreach (var et in db.Model.GetEntityTypes())
                            {
                                var pk = et.FindPrimaryKey();
                                var table = et.GetTableName();
                                if (pk == null || pk.Properties.Count != 1 || table == null) continue;
                                var pkType = Nullable.GetUnderlyingType(pk.Properties[0].ClrType) ?? pk.Properties[0].ClrType;
                                if (pkType != typeof(int) && pkType != typeof(long)) continue;
                                var col = pk.Properties[0].GetColumnName();
                                try
                                {
                                    db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"" + table + "\"', '" + col + "'), (SELECT COALESCE(MAX(\"" + col + "\"), 0) FROM \"" + table + "\") + 1, false);");
                                }
                                catch (Exception seqTableEx)
                                {
                                    Console.WriteLine($"[Seed] Sequence sync skipped for {table}: {seqTableEx.Message}");
                                }
                            }
                            Console.WriteLine("[Seed] Identity sequences synced.");
                        }
                        catch (Exception seqEx)
                        {
                            Console.WriteLine($"[Seed] Sequence sync skipped: {seqEx.GetType().Name}: {seqEx.Message}");
                        }
                    }
                    // RBAC seed (Admin/User roles + permissions + admin user) runs through ABP DI
                    // so PermissionRegistry can be injected. SeedHelper is idempotent.
                    SeedHelper.SeedHostDb(Abp.Dependency.IocManager.Instance);
                    Console.WriteLine("[Seed] RBAC seed complete (Admin role + admin user).");
                }
                catch (Exception ex)
                {
                    // Full diagnostic — surface the real cause so silent seed failures are debuggable.
                    Console.WriteLine($"[Migration] FAILED: {ex.GetType().Name}: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"[Migration] InnerException: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                    Console.WriteLine("[Migration] StackTrace:");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("[Migration] App continues without migration — admin user will not exist.");
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public class Program
    {
        // Runtime entry: WebHost is required because ABP Startup returns IServiceProvider.
        public static void Main(string[] args)
        {
            // Npgsql 7+ requires UTC DateTimes — enable legacy behavior for ABP compatibility
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // Loglar: konsola JSON (Railway toplar) + Warning ve ustu SystemLogs tablosuna (admin "Sistem Loglari")
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
                // ABP denetim kaydi ayni hatanin kopyasi ve istek parametrelerini icerebilir (kisisel veri) — yalniz Error
                .MinimumLevel.Override("Abp.Auditing", Serilog.Events.LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.Console(formatter: new Serilog.Formatting.Compact.RenderedCompactJsonFormatter())
                .WriteTo.Sink(new PortfoyYonetimSistemi.Web.Host.Logging.SystemLogSink())
                .CreateLogger();
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        // Design-time entry for EF Core tools (dotnet ef migrations).
        // Without this, EF tools wait 5 minutes for IHost build (resolver default timeout)
        // and then SIGTERM any running dotnet process — killing live dev servers.
        // We expose a minimal IHost that EF tools resolve in milliseconds; the actual
        // DbContext is built by IDesignTimeDbContextFactory in the EntityFrameworkCore project.
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);
    }
}
