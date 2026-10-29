// Auto-generated flow: WorkAssignment Approval Flow
// Auto-generated approval flow for WorkAssignment. Customize email templates and add conditions as needed.
// Resource: WorkAssignment
// Enabled: true
//
// Nodes:
  // trigger: On WorkAssignment Submit
  // condition: Status = PendingApproval?
  // approval: WorkAssignment Approval
  // action: Send Approval Email
  // trigger: On WorkAssignment Approved
  // action: Send Completion Email
//
// Edges:
  // On WorkAssignment Submit → Status = PendingApproval?
  // Status = PendingApproval? → WorkAssignment Approval (true)
  // WorkAssignment Approval → Send Approval Email
  // On WorkAssignment Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_WORKASSIGNMENT_APPROVAL_FLOW_ID = 'flow-WorkAssignment-approval';
