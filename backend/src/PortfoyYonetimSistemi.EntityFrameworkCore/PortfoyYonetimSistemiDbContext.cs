using Microsoft.EntityFrameworkCore;
using Abp.EntityFrameworkCore;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.EntityFrameworkCore
{
    public class PortfoyYonetimSistemiDbContext : AbpDbContext
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<PortfolioManager> PortfolioManagers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerManagerTransfer> CustomerManagerTransfers { get; set; }
        public DbSet<Fund> Funds { get; set; }
        public DbSet<CustomerFundHolding> CustomerFundHoldings { get; set; }
        public DbSet<MarketData> MarketDatas { get; set; }
        public DbSet<TargetPeriod> TargetPeriods { get; set; }
        public DbSet<ManagerTarget> ManagerTargets { get; set; }
        public DbSet<WorkAssignment> WorkAssignments { get; set; }
        public DbSet<PerformanceReport> PerformanceReports { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<ApprovalRecord> ApprovalRecords { get; set; }
        public DbSet<StatusChangeLog> StatusChangeLogs { get; set; }


        public PortfoyYonetimSistemiDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kayit basina tek bekleyen onay: es zamanli iki gonderim iki "Pending" acip onayciya
            // bir adimi atlatabiliyordu. Filtreli tekil indeks MySQL'de yok — orada uygulama kontrolu yeterli.
            var provider = Database.ProviderName ?? "";
            var pendingFilter = provider.Contains("Npgsql") ? "\"Status\" = 'Pending'" : provider.Contains("SqlServer") ? "[Status] = 'Pending'" : null;
            if (pendingFilter != null)
                modelBuilder.Entity<ApprovalRecord>()
                    .HasIndex(a => new { a.EntityType, a.EntityId })
                    .IsUnique()
                    .HasFilter(pendingFilter);

            // Branch 1:N PortfolioManager
            modelBuilder.Entity<PortfolioManager>()
                .HasOne(x => x.Branch)
                .WithMany(x => x.PortfolioManagers)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // PortfolioManager 1:N Customer
            modelBuilder.Entity<Customer>()
                .HasOne(x => x.PortfolioManager)
                .WithMany(x => x.Customers)
                .HasForeignKey(x => x.PortfolioManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer 1:N CustomerManagerTransfer
            modelBuilder.Entity<CustomerManagerTransfer>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.CustomerManagerTransfers)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // PortfolioManager 1:N CustomerManagerTransfer
            modelBuilder.Entity<CustomerManagerTransfer>()
                .HasOne(x => x.PortfolioManager)
                .WithMany(x => x.CustomerManagerTransfers)
                .HasForeignKey(x => x.PortfolioManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer 1:N CustomerFundHolding
            modelBuilder.Entity<CustomerFundHolding>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.CustomerFundHoldings)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Fund 1:N CustomerFundHolding
            modelBuilder.Entity<CustomerFundHolding>()
                .HasOne(x => x.Fund)
                .WithMany(x => x.CustomerFundHoldings)
                .HasForeignKey(x => x.FundId)
                .OnDelete(DeleteBehavior.Restrict);

            // Fund 1:N MarketData
            modelBuilder.Entity<MarketData>()
                .HasOne(x => x.Fund)
                .WithMany(x => x.MarketDatas)
                .HasForeignKey(x => x.FundId)
                .OnDelete(DeleteBehavior.Restrict);

            // TargetPeriod 1:N ManagerTarget
            modelBuilder.Entity<ManagerTarget>()
                .HasOne(x => x.TargetPeriod)
                .WithMany(x => x.ManagerTargets)
                .HasForeignKey(x => x.TargetPeriodId)
                .OnDelete(DeleteBehavior.Restrict);

            // PortfolioManager 1:N ManagerTarget
            modelBuilder.Entity<ManagerTarget>()
                .HasOne(x => x.PortfolioManager)
                .WithMany(x => x.ManagerTargets)
                .HasForeignKey(x => x.PortfolioManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // PortfolioManager 1:N WorkAssignment
            modelBuilder.Entity<WorkAssignment>()
                .HasOne(x => x.PortfolioManager)
                .WithMany(x => x.WorkAssignments)
                .HasForeignKey(x => x.PortfolioManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // PortfolioManager 1:N PerformanceReport
            modelBuilder.Entity<PerformanceReport>()
                .HasOne(x => x.PortfolioManager)
                .WithMany(x => x.PerformanceReports)
                .HasForeignKey(x => x.PortfolioManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // TargetPeriod 1:N PerformanceReport
            modelBuilder.Entity<PerformanceReport>()
                .HasOne(x => x.TargetPeriod)
                .WithMany(x => x.PerformanceReports)
                .HasForeignKey(x => x.TargetPeriodId)
                .OnDelete(DeleteBehavior.Restrict);


            // RBAC: AppUser N:N AppRole via UserRole junction
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // RolePermission: AppRole 1:N RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionName })
                .IsUnique();

            // AppRole.Name unique
            modelBuilder.Entity<AppRole>()
                .HasIndex(r => r.Name)
                .IsUnique();

        }
    }
}
