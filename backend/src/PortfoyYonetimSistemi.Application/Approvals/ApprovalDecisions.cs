using System.Collections.Generic;

namespace PortfoyYonetimSistemi.Approvals
{
    public static class ApprovalDecisions
    {
        public static readonly Dictionary<string, string> RejectAction = new Dictionary<string, string>
        {
            ["ManagerTarget"] = "Revise",
            ["WorkAssignment"] = "Revise",
            ["PerformanceReport"] = "Revise",
        };
    }
}
