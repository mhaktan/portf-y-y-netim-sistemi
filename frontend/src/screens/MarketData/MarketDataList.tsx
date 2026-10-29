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
import { MarketDataCreate } from './MarketDataCreate';
import { MarketDataEdit } from './MarketDataEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type MarketDataRecord = {
  id: string | number;
  instrumentName: string;
  instrumentCode: string;
  instrumentType: string;
  dataDate: string;
  openPrice?: number;
  closePrice: number;
  highPrice?: number;
  lowPrice?: number;
  volume?: number;
  navPerUnit?: number;
  dailyReturnRate?: number;
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
  { field: 'instrumentName', header: "Enstrüman Adı", sortable: true, searchable: true, filterType: 'text' },
  { field: 'instrumentCode', header: "Enstrüman Kodu", sortable: true, searchable: true, filterType: 'text' },
  { field: 'instrumentType', header: "Enstrüman Tipi", sortable: true, filterType: 'radio', filterOptions: [{ label: "Fon", value: "0" }, { label: "Hisse Endeksi", value: "1" }, { label: "Döviz Çifti", value: "2" }, { label: "Emtia Endeksi", value: "3" }, { label: "Faiz Oranı", value: "4" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Fon", ["1"]: "Hisse Endeksi", ["2"]: "Döviz Çifti", ["3"]: "Emtia Endeksi", ["4"]: "Faiz Oranı"}; return escHtml(m[String(row.instrumentType ?? '')] ?? String(row.instrumentType ?? '\u2014')); } },
  { field: 'dataDate', header: "Tarih", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => formatDate(row.dataDate, false) },
  { field: 'openPrice', header: "Açılış Fiyatı", sortable: true, html: (row: Record<string, unknown>) => row.openPrice == null ? '\u2014' : Number(row.openPrice).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'closePrice', header: "Kapanış Fiyatı", sortable: true, html: (row: Record<string, unknown>) => row.closePrice == null ? '\u2014' : Number(row.closePrice).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'highPrice', header: "Günlük En Yüksek", sortable: true, html: (row: Record<string, unknown>) => row.highPrice == null ? '\u2014' : Number(row.highPrice).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'lowPrice', header: "Günlük En Düşük", sortable: true, html: (row: Record<string, unknown>) => row.lowPrice == null ? '\u2014' : Number(row.lowPrice).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'volume', header: "İşlem Hacmi", sortable: true, html: (row: Record<string, unknown>) => row.volume == null ? '\u2014' : Number(row.volume).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'navPerUnit', header: "Birim Pay Değeri (NAV)", sortable: true, html: (row: Record<string, unknown>) => row.navPerUnit == null ? '\u2014' : Number(row.navPerUnit).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'dailyReturnRate', header: "Günlük Getiri (%)", sortable: true, html: (row: Record<string, unknown>) => row.dailyReturnRate == null ? '\u2014' : Number(row.dailyReturnRate).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'fundDisplay', header: "Fon", sortable: false },
];

// ---------------------------------------------------------------------------
// MarketDataList
// ---------------------------------------------------------------------------

export const MarketDataList: React.FC = () => {
  const list = useListQuery<MarketDataRecord>({ resource: 'MarketData' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<MarketDataRecord | null>(null);

  const [selectedRows, setSelectedRows] = useState<MarketDataRecord[]>([]);

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
    Promise.resolve(dataProvider.getOne('MarketData', focusId))
      .then((rec) => { if (rec) setEditRecord(rec as MarketDataRecord); })
      .catch(() => { /* kayit bulunamadi — liste normal acilir */ })
      .finally(clear);
  }, [focusId]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('MarketData', (ids) => { setSelectedRows([]); ids.forEach(id => triggerFlows('delete', 'MarketData', { id })); });


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
        title={"Piyasa Verisi"}
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
        onSelectionChange={(rows) => setSelectedRows(rows as MarketDataRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <Button variant="danger" appearance="filled" onClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} kayıt`)}>{`Seçilenleri sil (${selectedRows.length})`}</Button>
          )}

          <Button variant="primary" appearance="filled" onClick={() => setShowCreate(true)}>{"+ Yeni Piyasa Verisi"}</Button>
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
      <MarketDataCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <MarketDataEdit record={editRecord} onClose={() => { setEditRecord(null); }} onSuccess={list.invalidate} />

    </>
  );
};

