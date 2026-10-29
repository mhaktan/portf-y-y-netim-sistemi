import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import { CustomerManagerTransferList } from './CustomerManagerTransferList';

// Mock dataProvider
vi.mock('../../dataProvider', () => ({
  dataProvider: {
    list: vi.fn().mockResolvedValue({ data: [], total: 0 }),
    create: vi.fn().mockResolvedValue({}),
    update: vi.fn().mockResolvedValue({}),
    delete: vi.fn().mockResolvedValue(undefined),
  },
  isAuthenticated: () => true,
  getAuthToken: () => 'test-token',
}));



const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: false } },
});

const wrapper = ({ children }: { children: React.ReactNode }) => (
  <MemoryRouter>
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  </MemoryRouter>
);

describe('CustomerManagerTransferList', () => {
  it('should render without crashing', () => {
    const { container } = render(<CustomerManagerTransferList />, { wrapper });
    expect(container).toBeDefined();
  });

  it('should show title', () => {
    render(<CustomerManagerTransferList />, { wrapper });
    expect(screen.getByText('CustomerManagerTransfer')).toBeDefined();
  });

  it('should have create button', () => {
    render(<CustomerManagerTransferList />, { wrapper });
    expect(screen.getByText('+ Create CustomerManagerTransfer')).toBeDefined();
  });

});
