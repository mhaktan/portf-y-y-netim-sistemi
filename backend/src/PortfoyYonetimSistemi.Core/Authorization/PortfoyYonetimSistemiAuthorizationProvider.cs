using Abp.Authorization;
using Abp.Localization;

namespace PortfoyYonetimSistemi.Authorization
{
    public class PortfoyYonetimSistemiAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull("Pages") ?? context.CreatePermission("Pages", L("Pages"));

            // Branch
            pages.CreateChildPermission(PermissionNames.Branch_Read, L("Branch.Read"));
            pages.CreateChildPermission(PermissionNames.Branch_Create, L("Branch.Create"));
            pages.CreateChildPermission(PermissionNames.Branch_Update, L("Branch.Update"));
            pages.CreateChildPermission(PermissionNames.Branch_Delete, L("Branch.Delete"));

            // PortfolioManager
            pages.CreateChildPermission(PermissionNames.PortfolioManager_Read, L("PortfolioManager.Read"));
            pages.CreateChildPermission(PermissionNames.PortfolioManager_Create, L("PortfolioManager.Create"));
            pages.CreateChildPermission(PermissionNames.PortfolioManager_Update, L("PortfolioManager.Update"));
            pages.CreateChildPermission(PermissionNames.PortfolioManager_Delete, L("PortfolioManager.Delete"));

            // Customer
            pages.CreateChildPermission(PermissionNames.Customer_Read, L("Customer.Read"));
            pages.CreateChildPermission(PermissionNames.Customer_Create, L("Customer.Create"));
            pages.CreateChildPermission(PermissionNames.Customer_Update, L("Customer.Update"));
            pages.CreateChildPermission(PermissionNames.Customer_Delete, L("Customer.Delete"));

            // CustomerManagerTransfer
            pages.CreateChildPermission(PermissionNames.CustomerManagerTransfer_Read, L("CustomerManagerTransfer.Read"));
            pages.CreateChildPermission(PermissionNames.CustomerManagerTransfer_Create, L("CustomerManagerTransfer.Create"));
            pages.CreateChildPermission(PermissionNames.CustomerManagerTransfer_Update, L("CustomerManagerTransfer.Update"));
            pages.CreateChildPermission(PermissionNames.CustomerManagerTransfer_Delete, L("CustomerManagerTransfer.Delete"));

            // Fund
            pages.CreateChildPermission(PermissionNames.Fund_Read, L("Fund.Read"));
            pages.CreateChildPermission(PermissionNames.Fund_Create, L("Fund.Create"));
            pages.CreateChildPermission(PermissionNames.Fund_Update, L("Fund.Update"));
            pages.CreateChildPermission(PermissionNames.Fund_Delete, L("Fund.Delete"));

            // CustomerFundHolding
            pages.CreateChildPermission(PermissionNames.CustomerFundHolding_Read, L("CustomerFundHolding.Read"));
            pages.CreateChildPermission(PermissionNames.CustomerFundHolding_Create, L("CustomerFundHolding.Create"));
            pages.CreateChildPermission(PermissionNames.CustomerFundHolding_Update, L("CustomerFundHolding.Update"));
            pages.CreateChildPermission(PermissionNames.CustomerFundHolding_Delete, L("CustomerFundHolding.Delete"));

            // MarketData
            pages.CreateChildPermission(PermissionNames.MarketData_Read, L("MarketData.Read"));
            pages.CreateChildPermission(PermissionNames.MarketData_Create, L("MarketData.Create"));
            pages.CreateChildPermission(PermissionNames.MarketData_Update, L("MarketData.Update"));
            pages.CreateChildPermission(PermissionNames.MarketData_Delete, L("MarketData.Delete"));

            // TargetPeriod
            pages.CreateChildPermission(PermissionNames.TargetPeriod_Read, L("TargetPeriod.Read"));
            pages.CreateChildPermission(PermissionNames.TargetPeriod_Create, L("TargetPeriod.Create"));
            pages.CreateChildPermission(PermissionNames.TargetPeriod_Update, L("TargetPeriod.Update"));
            pages.CreateChildPermission(PermissionNames.TargetPeriod_Delete, L("TargetPeriod.Delete"));

            // ManagerTarget
            pages.CreateChildPermission(PermissionNames.ManagerTarget_Read, L("ManagerTarget.Read"));
            pages.CreateChildPermission(PermissionNames.ManagerTarget_Create, L("ManagerTarget.Create"));
            pages.CreateChildPermission(PermissionNames.ManagerTarget_Update, L("ManagerTarget.Update"));
            pages.CreateChildPermission(PermissionNames.ManagerTarget_Delete, L("ManagerTarget.Delete"));
            pages.CreateChildPermission(PermissionNames.ManagerTarget_ChangeStatus, L("ManagerTarget.ChangeStatus"));

            // WorkAssignment
            pages.CreateChildPermission(PermissionNames.WorkAssignment_Read, L("WorkAssignment.Read"));
            pages.CreateChildPermission(PermissionNames.WorkAssignment_Create, L("WorkAssignment.Create"));
            pages.CreateChildPermission(PermissionNames.WorkAssignment_Update, L("WorkAssignment.Update"));
            pages.CreateChildPermission(PermissionNames.WorkAssignment_Delete, L("WorkAssignment.Delete"));
            pages.CreateChildPermission(PermissionNames.WorkAssignment_ChangeStatus, L("WorkAssignment.ChangeStatus"));

            // PerformanceReport
            pages.CreateChildPermission(PermissionNames.PerformanceReport_Read, L("PerformanceReport.Read"));
            pages.CreateChildPermission(PermissionNames.PerformanceReport_Create, L("PerformanceReport.Create"));
            pages.CreateChildPermission(PermissionNames.PerformanceReport_Update, L("PerformanceReport.Update"));
            pages.CreateChildPermission(PermissionNames.PerformanceReport_Delete, L("PerformanceReport.Delete"));
            pages.CreateChildPermission(PermissionNames.PerformanceReport_ChangeStatus, L("PerformanceReport.ChangeStatus"));

            // RBAC
            pages.CreateChildPermission(PermissionNames.AppUser_Read, L("AppUser.Read"));
            pages.CreateChildPermission(PermissionNames.AppRole_Read, L("AppRole.Read"));
            pages.CreateChildPermission(PermissionNames.AppUser_Create, L("AppUser.Create"));
            pages.CreateChildPermission(PermissionNames.AppRole_Create, L("AppRole.Create"));
            pages.CreateChildPermission(PermissionNames.AppUser_Update, L("AppUser.Update"));
            pages.CreateChildPermission(PermissionNames.AppRole_Update, L("AppRole.Update"));
            pages.CreateChildPermission(PermissionNames.AppUser_Delete, L("AppUser.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_Delete, L("AppRole.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_AssignPermissions, L("AppRole.AssignPermissions"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, PortfoyYonetimSistemiConsts.LocalizationSourceName);
        }
    }
}
