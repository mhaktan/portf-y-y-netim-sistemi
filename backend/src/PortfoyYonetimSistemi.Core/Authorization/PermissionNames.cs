namespace PortfoyYonetimSistemi.Authorization
{
    public static class PermissionNames
    {
        public const string Pages = "Pages";

        // Branch
        public const string Branch_Read = "Branch.Read";
        public const string Branch_Create = "Branch.Create";
        public const string Branch_Update = "Branch.Update";
        public const string Branch_Delete = "Branch.Delete";

        // PortfolioManager
        public const string PortfolioManager_Read = "PortfolioManager.Read";
        public const string PortfolioManager_Create = "PortfolioManager.Create";
        public const string PortfolioManager_Update = "PortfolioManager.Update";
        public const string PortfolioManager_Delete = "PortfolioManager.Delete";

        // Customer
        public const string Customer_Read = "Customer.Read";
        public const string Customer_Create = "Customer.Create";
        public const string Customer_Update = "Customer.Update";
        public const string Customer_Delete = "Customer.Delete";

        // CustomerManagerTransfer
        public const string CustomerManagerTransfer_Read = "CustomerManagerTransfer.Read";
        public const string CustomerManagerTransfer_Create = "CustomerManagerTransfer.Create";
        public const string CustomerManagerTransfer_Update = "CustomerManagerTransfer.Update";
        public const string CustomerManagerTransfer_Delete = "CustomerManagerTransfer.Delete";

        // Fund
        public const string Fund_Read = "Fund.Read";
        public const string Fund_Create = "Fund.Create";
        public const string Fund_Update = "Fund.Update";
        public const string Fund_Delete = "Fund.Delete";

        // CustomerFundHolding
        public const string CustomerFundHolding_Read = "CustomerFundHolding.Read";
        public const string CustomerFundHolding_Create = "CustomerFundHolding.Create";
        public const string CustomerFundHolding_Update = "CustomerFundHolding.Update";
        public const string CustomerFundHolding_Delete = "CustomerFundHolding.Delete";

        // MarketData
        public const string MarketData_Read = "MarketData.Read";
        public const string MarketData_Create = "MarketData.Create";
        public const string MarketData_Update = "MarketData.Update";
        public const string MarketData_Delete = "MarketData.Delete";

        // TargetPeriod
        public const string TargetPeriod_Read = "TargetPeriod.Read";
        public const string TargetPeriod_Create = "TargetPeriod.Create";
        public const string TargetPeriod_Update = "TargetPeriod.Update";
        public const string TargetPeriod_Delete = "TargetPeriod.Delete";

        // ManagerTarget
        public const string ManagerTarget_Read = "ManagerTarget.Read";
        public const string ManagerTarget_Create = "ManagerTarget.Create";
        public const string ManagerTarget_Update = "ManagerTarget.Update";
        public const string ManagerTarget_Delete = "ManagerTarget.Delete";
        public const string ManagerTarget_ChangeStatus = "ManagerTarget.ChangeStatus";

        // WorkAssignment
        public const string WorkAssignment_Read = "WorkAssignment.Read";
        public const string WorkAssignment_Create = "WorkAssignment.Create";
        public const string WorkAssignment_Update = "WorkAssignment.Update";
        public const string WorkAssignment_Delete = "WorkAssignment.Delete";
        public const string WorkAssignment_ChangeStatus = "WorkAssignment.ChangeStatus";

        // PerformanceReport
        public const string PerformanceReport_Read = "PerformanceReport.Read";
        public const string PerformanceReport_Create = "PerformanceReport.Create";
        public const string PerformanceReport_Update = "PerformanceReport.Update";
        public const string PerformanceReport_Delete = "PerformanceReport.Delete";
        public const string PerformanceReport_ChangeStatus = "PerformanceReport.ChangeStatus";

        // RBAC management
        public const string AppUser_Read = "AppUser.Read";
        public const string AppRole_Read = "AppRole.Read";
        public const string AppUser_Create = "AppUser.Create";
        public const string AppRole_Create = "AppRole.Create";
        public const string AppUser_Update = "AppUser.Update";
        public const string AppRole_Update = "AppRole.Update";
        public const string AppUser_Delete = "AppUser.Delete";
        public const string AppRole_Delete = "AppRole.Delete";
        public const string AppRole_AssignPermissions = "AppRole.AssignPermissions";

    }
}
