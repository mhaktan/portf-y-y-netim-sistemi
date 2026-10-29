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
import { CustomerFundHoldingCreate } from './CustomerFundHoldingCreate';
import { CustomerFundHoldingEdit } from './CustomerFundHoldingEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type CustomerFundHoldingRecord = {
  id: string | number;
  units: number;
  purchasePrice?: number;
  currentValue?: number;
  purchaseDate?: string;
  weight?: number;
  profitLoss?: number;
  customerId: string;
  fundId: string;
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
  { field: 'units', header: "Adet / Pay", sortable: true, html: (row: Record<string, unknown>) => row.units == null ? '\u2014' : Number(row.units).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'purchasePrice', header: "Alış Fiyatı", sortable: true, html: (row: Record<string, unknown>) => row.purchasePrice == null ? '\u2014' : Number(row.purchasePrice).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'currentValue', header: "Güncel Değer (₺)", sortable: true, html: (row: Record<string, unknown>) => row.currentValue == null ? '\u2014' : Number(row.currentValue).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'purchaseDate', header: "Alış Tarihi", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => formatDate(row.purchaseDate, false) },
  { field: 'weight', header: "Portföy Ağırlığı (%)", sortable: true, html: (row: Record<string, unknown>) => row.weight == null ? '\u2014' : Number(row.weight).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'profitLoss', header: "Kâr / Zarar (₺)", sortable: true, html: (row: Record<string, unknown>) => row.profitLoss == null ? '\u2014' : Number(row.profitLoss).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'customerDisplay', header: "Müşteri", sortable: false },
  { field: 'fundDisplay', header: "Fon", sortable: false },
];

// ---------------------------------------------------------------------------
// CustomerFundHoldingList
// ---------------------------------------------------------------------------

export const CustomerFundHoldingList: React.FC = () => {
  const list = useListQuery<CustomerFundHoldingRecord>({ resource: 'CustomerFundHolding' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<CustomerFundHoldingRecord | null>(null);

  const [selectedRows, setSelectedRows] = useState<CustomerFundHoldingRecord[]>([]);

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
    Promise.resolve(dataProvider.getOne('CustomerFundHolding', focusId))
      .then((rec) => { if (rec) setEditRecord(rec as CustomerFundHoldingRecord); })
      .catch(() => { /* kayit bulunamadi — liste normal acilir */ })
      .finally(clear);
  }, [focusId]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('CustomerFundHolding', (ids) => { setSelectedRows([]); ids.forEach(id => triggerFlows('delete', 'CustomerFundHolding', { id })); });


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
        title={"Müşteri Fon Pozisyonu"}
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
        onSelectionChange={(rows) => setSelectedRows(rows as CustomerFundHoldingRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <Button variant="danger" appearance="filled" onClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} kayıt`)}>{`Seçilenleri sil (${selectedRows.length})`}</Button>
          )}

          <Button variant="primary" appearance="filled" onClick={() => setShowCreate(true)}>{"+ Yeni Müşteri Fon Pozisyonu"}</Button>
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
      <CustomerFundHoldingCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <CustomerFundHoldingEdit record={editRecord} onClose={() => { setEditRecord(null); }} onSuccess={list.invalidate} />

    </>
  );
};

