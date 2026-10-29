import React, { useState, useCallback, useEffect, useRef } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Button } from '@takeoff-ui/react-spar';
import { dataProvider } from '../../dataProvider';
import { useListQuery } from '../../shared/useListQuery';
import { useDeleteMutation } from '../../shared/useDeleteMutation';
import { DeleteConfirmDialog } from '../../shared/DeleteConfirmDialog';
import { ListPageLayout } from '../../shared/ListPageLayout';
import type { TableColumn } from '../../shared/ListPageLayout';
import { actionColumn } from '../../shared/ActionButtons';
import { escHtml, formatDate } from '../../shared/format';
import { CustomerCreate } from './CustomerCreate';
import { CustomerEdit } from './CustomerEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type CustomerRecord = {
  id: string | number;
  fullName: string;
  customerCode: string;
  customerType: string;
  phone?: string;
  email?: string;
  riskProfile?: string;
  totalAum?: number;
  onboardingDate?: string;
  portfolioManagerId: string;
  [key: string]: unknown;
};

// ---------------------------------------------------------------------------
// Column definition — edit this array to add/remove/reorder columns
// ---------------------------------------------------------------------------
//
// Override examples:
//   • Hide a column:     remove its entry from COLUMNS
//   • Add a custom col:  { field: 'fullName', header: 'Full Name', html: (row) => `${row.firstName} ${row.lastName}` }
//   • Enable filtering:  add searchable: true  or  filterType: 'text' | 'checkbox' | 'radio' | 'datepicker'
//   • Custom cell render: html: (row) => `<span style="color:green">${row.status}</span>`
//
// Shared components (src/shared/) can be edited to change behavior globally:
//   • ListPageLayout  — table wrapper, pagination, header layout
//   • ActionButtons   — edit/delete button styles, labels, and behavior
//   • useListQuery    — data fetching, sorting, filtering logic
//   • useDeleteMutation / DeleteConfirmDialog — delete flow
//
// Action buttons override: edit src/shared/ActionButtons.ts DEFAULT_CONFIG
// or pass custom config: actionColumn('id', { hasEdit: true, config: { edit: { label: 'View', style: '...' } } })
//

const COLUMNS: TableColumn[] = [
  { field: 'id', header: 'ID', sortable: true },
  { field: 'fullName', header: "Ad Soyad / Unvan", sortable: true, searchable: true, filterType: 'text' },
  { field: 'customerCode', header: "Müşteri No", sortable: true, searchable: true, filterType: 'text' },
  { field: 'customerType', header: "Müşteri Tipi", sortable: true, filterType: 'radio', filterOptions: [{ label: "Bireysel", value: "0" }, { label: "Kurumsal", value: "1" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Bireysel", ["1"]: "Kurumsal"}; return escHtml(m[String(row.customerType ?? '')] ?? String(row.customerType ?? '\u2014')); } },
  { field: 'phone', header: "Telefon", sortable: true, searchable: true, filterType: 'text' },
  { field: 'email', header: "E-posta", sortable: true, searchable: true, filterType: 'text' },
  { field: 'riskProfile', header: "Risk Profili", sortable: true, filterType: 'radio', filterOptions: [{ label: "Muhafazakâr", value: "0" }, { label: "Dengeli", value: "1" }, { label: "Agresif", value: "2" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Muhafazakâr", ["1"]: "Dengeli", ["2"]: "Agresif"}; return escHtml(m[String(row.riskProfile ?? '')] ?? String(row.riskProfile ?? '\u2014')); } },
  { field: 'totalAum', header: "Toplam AUM (₺)", sortable: true, html: (row: Record<string, unknown>) => row.totalAum == null ? '\u2014' : Number(row.totalAum).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'onboardingDate', header: "Müşteri Başlangıç Tarihi", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => formatDate(row.onboardingDate, false) },
  { field: 'portfolioManagerDisplay', header: "Portföy Yöneticisi", sortable: false },
];

// ---------------------------------------------------------------------------
// CustomerList
// ---------------------------------------------------------------------------

export const CustomerList: React.FC = () => {
  const list = useListQuery<CustomerRecord>({ resource: 'Customer' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<CustomerRecord | null>(null);

  const [selectedRows, setSelectedRows] = useState<CustomerRecord[]>([]);

  // ?focus=<id> ile gelindiginde kayit dogrudan acilir (My Tasks -> kayit)
  const [searchParams, setSearchParams] = useSearchParams();
  const focusId = searchParams.get('focus');
  const focusedRef = useRef<string | null>(null);
  useEffect(() => {
    if (!focusId || focusedRef.current === focusId) return;
    focusedRef.current = focusId;
    const clear = () => {
      const next = new URLSearchParams(searchParams);
      next.delete('focus');
      setSearchParams(next, { replace: true });
    };
    Promise.resolve(dataProvider.getOne('Customer', focusId))
      .then((rec) => { if (rec) setEditRecord(rec as CustomerRecord); })
      .catch(() => { /* kayit bulunamadi — liste normal acilir */ })
      .finally(clear);
  }, [focusId]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('Customer', (ids) => { setSelectedRows([]); ids.forEach(id => triggerFlows('delete', 'Customer', { id })); });


  const columns = [...COLUMNS, ...actionColumn('id', { hasEdit: true, hasDelete: true })];

  const handleCrudAction = useCallback((action: string, id: string) => {
    const row = list.records.find((r) => String(r.id) === id);
    if (!row) return;
    if (action === 'edit') setEditRecord(row);
    if (action === 'delete') del.requestSingleDelete(row.id);

  }, [list.records]);

  return (
    <>
      <ListPageLayout
        title={"Müşteri"}
        subtitle={Object.keys(list.displayParams).length > 0 ? (
          <div style={{ fontSize: 13, color: '#666', marginTop: 4 }}>
            {Object.entries(list.displayParams).map(([k, v]) => (
              <span key={k} style={{ marginRight: 12 }}>{k}: <strong>{v}</strong></span>
            ))}
          </div>
        ) : undefined}
        records={list.records}
        columns={columns}
        dataKey="id"
        total={list.total}
        loading={list.isLoading}
        page={list.page}
        perPage={list.perPage}
        onPageChange={list.setPage}
        onPerPageChange={list.setPerPage}
        onTableRequest={list.handleTableRequest}
        selectionMode="checkbox"
        selectedRows={selectedRows}
        onSelectionChange={(rows) => setSelectedRows(rows as CustomerRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <Button variant="danger" appearance="filled" onClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} kayıt`)}>{`Seçilenleri sil (${selectedRows.length})`}</Button>
          )}

          <Button variant="primary" appearance="filled" onClick={() => setShowCreate(true)}>{"+ Yeni Müşteri"}</Button>
        </>}
      />

      {list.isError && (
        <div style={{ padding: '10px 14px', background: '#fff3f3', border: '1px solid #f5c6c6', borderRadius: 6, color: '#c62828', fontSize: 13, marginBottom: 12 }}>
          Failed to load data: {(list.error as Error).message}
        </div>
      )}

      <DeleteConfirmDialog
        visible={!!del.deleteTarget}
        label={del.deleteTarget?.label ?? ''}
        isPending={del.isPending}
        onConfirm={del.confirmDelete}
        onCancel={() => del.setDeleteTarget(null)}
      />
      <CustomerCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <CustomerEdit record={editRecord} onClose={() => { setEditRecord(null); }} onSuccess={list.invalidate} />

    </>
  );
};

