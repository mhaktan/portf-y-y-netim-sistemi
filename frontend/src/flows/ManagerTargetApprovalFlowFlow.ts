// Auto-generated flow: ManagerTarget Approval Flow
// Auto-generated approval flow for ManagerTarget. Customize email templates and add conditions as needed.
// Resource: ManagerTarget
// Enabled: true
//
// Nodes:
  // trigger: On ManagerTarget Submit
  // condition: Status = PendingApproval?
  // approval: ManagerTarget Approval
  // action: Send Approval Email
  // trigger: On ManagerTarget Approved
  // action: Send Completion Email
//
// Edges:
  // On ManagerTarget Submit → Status = PendingApproval?
  // Status = PendingApproval? → ManagerTarget Approval (true)
  // ManagerTarget Approval → Send Approval Email
  // On ManagerTarget Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_MANAGERTARGET_APPROVAL_FLOW_ID = 'flow-ManagerTarget-approval';
