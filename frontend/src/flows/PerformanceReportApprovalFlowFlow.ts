// Auto-generated flow: PerformanceReport Approval Flow
// Auto-generated approval flow for PerformanceReport. Customize email templates and add conditions as needed.
// Resource: PerformanceReport
// Enabled: true
//
// Nodes:
  // trigger: On PerformanceReport Submit
  // condition: Status = PendingApproval?
  // approval: PerformanceReport Approval
  // action: Send Approval Email
  // trigger: On PerformanceReport Approved
  // action: Send Completion Email
//
// Edges:
  // On PerformanceReport Submit → Status = PendingApproval?
  // Status = PendingApproval? → PerformanceReport Approval (true)
  // PerformanceReport Approval → Send Approval Email
  // On PerformanceReport Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_PERFORMANCEREPORT_APPROVAL_FLOW_ID = 'flow-PerformanceReport-approval';
