using System.Collections.Generic;
using Abp.Dependency;

namespace PortfoyYonetimSistemi.Authorization
{
    /// <summary>Single permission descriptor — name, group (entity), description.</summary>
    public class PermissionInfo
    {
        public string Name { get; }
        public string Group { get; }
        public string Description { get; }
        public bool IsRbac { get; }

        public PermissionInfo(string name, string group, string description, bool isRbac)
        {
            Name = name; Group = group; Description = description; IsRbac = isRbac;
        }
    }

    public interface IPermissionRegistry
    {
        IReadOnlyList<PermissionInfo> All { get; }
    }

    public class PermissionRegistry : IPermissionRegistry, ISingletonDependency
    {
        public IReadOnlyList<PermissionInfo> All { get; } = new List<PermissionInfo>
        {
            new PermissionInfo("Branch.Read", "Branch", "Read Branch", false),
            new PermissionInfo("Branch.Create", "Branch", "Create Branch", false),
            new PermissionInfo("Branch.Update", "Branch", "Update Branch", false),
            new PermissionInfo("Branch.Delete", "Branch", "Delete Branch", false),
            new PermissionInfo("PortfolioManager.Read", "PortfolioManager", "Read PortfolioManager", false),
            new PermissionInfo("PortfolioManager.Create", "PortfolioManager", "Create PortfolioManager", false),
            new PermissionInfo("PortfolioManager.Update", "PortfolioManager", "Update PortfolioManager", false),
            new PermissionInfo("PortfolioManager.Delete", "PortfolioManager", "Delete PortfolioManager", false),
            new PermissionInfo("Customer.Read", "Customer", "Read Customer", false),
            new PermissionInfo("Customer.Create", "Customer", "Create Customer", false),
            new PermissionInfo("Customer.Update", "Customer", "Update Customer", false),
            new PermissionInfo("Customer.Delete", "Customer", "Delete Customer", false),
            new PermissionInfo("CustomerManagerTransfer.Read", "CustomerManagerTransfer", "Read CustomerManagerTransfer", false),
            new PermissionInfo("CustomerManagerTransfer.Create", "CustomerManagerTransfer", "Create CustomerManagerTransfer", false),
            new PermissionInfo("CustomerManagerTransfer.Update", "CustomerManagerTransfer", "Update CustomerManagerTransfer", false),
            new PermissionInfo("CustomerManagerTransfer.Delete", "CustomerManagerTransfer", "Delete CustomerManagerTransfer", false),
            new PermissionInfo("Fund.Read", "Fund", "Read Fund", false),
            new PermissionInfo("Fund.Create", "Fund", "Create Fund", false),
            new PermissionInfo("Fund.Update", "Fund", "Update Fund", false),
            new PermissionInfo("Fund.Delete", "Fund", "Delete Fund", false),
            new PermissionInfo("CustomerFundHolding.Read", "CustomerFundHolding", "Read CustomerFundHolding", false),
            new PermissionInfo("CustomerFundHolding.Create", "CustomerFundHolding", "Create CustomerFundHolding", false),
            new PermissionInfo("CustomerFundHolding.Update", "CustomerFundHolding", "Update CustomerFundHolding", false),
            new PermissionInfo("CustomerFundHolding.Delete", "CustomerFundHolding", "Delete CustomerFundHolding", false),
            new PermissionInfo("MarketData.Read", "MarketData", "Read MarketData", false),
            new PermissionInfo("MarketData.Create", "MarketData", "Create MarketData", false),
            new PermissionInfo("MarketData.Update", "MarketData", "Update MarketData", false),
            new PermissionInfo("MarketData.Delete", "MarketData", "Delete MarketData", false),
            new PermissionInfo("TargetPeriod.Read", "TargetPeriod", "Read TargetPeriod", false),
            new PermissionInfo("TargetPeriod.Create", "TargetPeriod", "Create TargetPeriod", false),
            new PermissionInfo("TargetPeriod.Update", "TargetPeriod", "Update TargetPeriod", false),
            new PermissionInfo("TargetPeriod.Delete", "TargetPeriod", "Delete TargetPeriod", false),
            new PermissionInfo("ManagerTarget.Read", "ManagerTarget", "Read ManagerTarget", false),
            new PermissionInfo("ManagerTarget.Create", "ManagerTarget", "Create ManagerTarget", false),
            new PermissionInfo("ManagerTarget.Update", "ManagerTarget", "Update ManagerTarget", false),
            new PermissionInfo("ManagerTarget.Delete", "ManagerTarget", "Delete ManagerTarget", false),
            new PermissionInfo("ManagerTarget.ChangeStatus", "ManagerTarget", "Change ManagerTarget status", false),
            new PermissionInfo("WorkAssignment.Read", "WorkAssignment", "Read WorkAssignment", false),
            new PermissionInfo("WorkAssignment.Create", "WorkAssignment", "Create WorkAssignment", false),
            new PermissionInfo("WorkAssignment.Update", "WorkAssignment", "Update WorkAssignment", false),
            new PermissionInfo("WorkAssignment.Delete", "WorkAssignment", "Delete WorkAssignment", false),
            new PermissionInfo("WorkAssignment.ChangeStatus", "WorkAssignment", "Change WorkAssignment status", false),
            new PermissionInfo("PerformanceReport.Read", "PerformanceReport", "Read PerformanceReport", false),
            new PermissionInfo("PerformanceReport.Create", "PerformanceReport", "Create PerformanceReport", false),
            new PermissionInfo("PerformanceReport.Update", "PerformanceReport", "Update PerformanceReport", false),
            new PermissionInfo("PerformanceReport.Delete", "PerformanceReport", "Delete PerformanceReport", false),
            new PermissionInfo("PerformanceReport.ChangeStatus", "PerformanceReport", "Change PerformanceReport status", false),
            new PermissionInfo("AppUser.Read", "AppUser", "Read users", true),
            new PermissionInfo("AppRole.Read", "AppRole", "Read roles", true),
            new PermissionInfo("AppUser.Create", "AppUser", "Create users", true),
            new PermissionInfo("AppRole.Create", "AppRole", "Create roles", true),
            new PermissionInfo("AppUser.Update", "AppUser", "Update users", true),
            new PermissionInfo("AppRole.Update", "AppRole", "Update roles", true),
            new PermissionInfo("AppUser.Delete", "AppUser", "Delete users", true),
            new PermissionInfo("AppRole.Delete", "AppRole", "Delete roles", true),
            new PermissionInfo("AppRole.AssignPermissions", "AppRole", "Assign permissions to roles", true),
        };
    }
}
