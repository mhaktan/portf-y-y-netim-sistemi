import React, { useState } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom';
import { isAuthenticated, clearAuth } from './dataProvider';
import { LoginPage } from './screens/LoginPage';
import { GlobalMenu } from './components/GlobalMenu';
import { FlowProvider } from './flows/FlowProvider';
import { DemoBar } from './shared/DemoBar';
import './shared/clientErrors';
import { BranchList } from './screens/Branch/BranchList';
import { PortfolioManagerList } from './screens/PortfolioManager/PortfolioManagerList';
import { CustomerList } from './screens/Customer/CustomerList';
import { CustomerManagerTransferList } from './screens/CustomerManagerTransfer/CustomerManagerTransferList';
import { FundList } from './screens/Fund/FundList';
import { CustomerFundHoldingList } from './screens/CustomerFundHolding/CustomerFundHoldingList';
import { MarketDataList } from './screens/MarketData/MarketDataList';
import { TargetPeriodList } from './screens/TargetPeriod/TargetPeriodList';
import { ManagerTargetList } from './screens/ManagerTarget/ManagerTargetList';
import { WorkAssignmentList } from './screens/WorkAssignment/WorkAssignmentList';
import { PerformanceReportList } from './screens/PerformanceReport/PerformanceReportList';
import { DashboardScreen } from './screens/dashboard/DashboardScreen';
import TaskInboxScreen from './screens/tasks/TaskInboxScreen';
import UserListScreen from './admin/UserListScreen';
import RoleListScreen from './admin/RoleListScreen';
import SystemLogsScreen from './admin/SystemLogsScreen';

const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: 1, staleTime: 0 } },
});

export const App: React.FC = () => {
  const [authenticated, setAuthenticated] = useState(isAuthenticated());


  const handleLogout = () => {
    clearAuth();
    setAuthenticated(false);
    queryClient.clear();
  };

  return (
    <QueryClientProvider client={queryClient}>
      <FlowProvider>
      <BrowserRouter>
        <Routes>
          <Route path="*" element={
            authenticated
              ? (
                <GlobalMenu>
                  <DemoBar />
                  <Routes>
                    <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<DashboardScreen />} />
          <Route path="/Branch" element={<BranchList />} />
          <Route path="/PortfolioManager" element={<PortfolioManagerList />} />
          <Route path="/Customer" element={<CustomerList />} />
          <Route path="/CustomerManagerTransfer" element={<CustomerManagerTransferList />} />
          <Route path="/Fund" element={<FundList />} />
          <Route path="/CustomerFundHolding" element={<CustomerFundHoldingList />} />
          <Route path="/MarketData" element={<MarketDataList />} />
          <Route path="/TargetPeriod" element={<TargetPeriodList />} />
          <Route path="/ManagerTarget" element={<ManagerTargetList />} />
          <Route path="/WorkAssignment" element={<WorkAssignmentList />} />
          <Route path="/PerformanceReport" element={<PerformanceReportList />} />
          <Route path="/tasks" element={<TaskInboxScreen />} />
          <Route path="/users" element={<UserListScreen />} />
          <Route path="/roles" element={<RoleListScreen />} />
          <Route path="/system-logs" element={<SystemLogsScreen />} />
                  </Routes>
                </GlobalMenu>
              )
              : <LoginPage onLogin={() => setAuthenticated(true)} />
          } />
        </Routes>
      </BrowserRouter>
        </FlowProvider>
    </QueryClientProvider>
  );
};
