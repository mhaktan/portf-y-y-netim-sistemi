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
import { ManagerTargetCreate } from './ManagerTargetCreate';
import { ManagerTargetEdit } from './ManagerTargetEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type ManagerTargetRecord = {
  id: string | number;
  description: string;
  targetType: string;
  targetValue: number;
  actualValue?: number;
  unit?: string;
  achievementRate?: number;
  status: string;
  notes?: string;
  revisionNote?: string;
  targetPeriodId: string;
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
  { field: 'description', header: "Hedef Açıklaması", sortable: true, searchable: true, filterType: 'text' },
  { field: 'targetType', header: "Hedef Türü", sortable: true, filterType: 'radio', filterOptions: [{ label: "AUM Büyümesi", value: "0" }, { label: "Yeni Müşteri", value: "1" }, { label: "Müşteri Koruma", value: "2" }, { label: "Getiri Oranı", value: "3" }, { label: "İşlem Adedi", value: "4" }, { label: "Özel Hedef", value: "5" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "AUM Büyümesi", ["1"]: "Yeni Müşteri", ["2"]: "Müşteri Koruma", ["3"]: "Getiri Oranı", ["4"]: "İşlem Adedi", ["5"]: "Özel Hedef"}; return escHtml(m[String(row.targetType ?? '')] ?? String(row.targetType ?? '\u2014')); } },
  { field: 'targetValue', header: "Hedef Değer", sortable: true, html: (row: Record<string, unknown>) => row.targetValue == null ? '\u2014' : Number(row.targetValue).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'actualValue', header: "Gerçekleşen Değer", sortable: true, html: (row: Record<string, unknown>) => row.actualValue == null ? '\u2014' : Number(row.actualValue).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'unit', header: "Birim", sortable: true, searchable: true, filterType: 'text' },
  { field: 'achievementRate', header: "Gerçekleşme Oranı (%)", sortable: true, html: (row: Record<string, unknown>) => row.achievementRate == null ? '\u2014' : Number(row.achievementRate).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 }) },
  { field: 'status', header: "Durum", sortable: true, filterType: 'radio', filterOptions: [{ label: "Taslak", value: "0" }, { label: "Onay Bekliyor", value: "1" }, { label: "Aktif", value: "2" }, { label: "Tamamlandı", value: "3" }, { label: "İptal", value: "4" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Taslak", ["1"]: "Onay Bekliyor", ["2"]: "Aktif", ["3"]: "Tamamlandı", ["4"]: "İptal"}; return escHtml(m[String(row.status ?? '')] ?? String(row.status ?? '\u2014')); } },
  { field: 'notes', header: "Notlar", sortable: true, searchable: true, filterType: 'text' },
  { field: 'revisionNote', header: "Revizyon Notu", sortable: true, searchable: true, filterType: 'text' },
  { field: 'targetPeriodDisplay', header: "Hedef Dönemi", sortable: false },
  { field: 'portfolioManagerDisplay', header: "Portföy Yöneticisi", sortable: false },
];

// ---------------------------------------------------------------------------
// ManagerTargetList
// ---------------------------------------------------------------------------

export const ManagerTargetList: React.FC = () => {
  const list = useListQuery<ManagerTargetRecord>({ resource: 'ManagerTarget' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<ManagerTargetRecord | null>(null);
  const [justCreated, setJustCreated] = useState(false);
  const [selectedRows, setSelectedRows] = useState<ManagerTargetRecord[]>([]);

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
    Promise.resolve(dataProvider.getOne('ManagerTarget', focusId))
      .then((rec) => { if (rec) setEditRecord(rec as ManagerTargetRecord); })
      .catch(() => { /* kayit bulunamadi — liste normal acilir */ })
      .finally(clear);
  }, [focusId]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('ManagerTarget', (ids) => { setSelectedRows([]); ids.forEach(id => triggerFlows('delete', 'ManagerTarget', { id })); });


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
        title={"Portföy Yöneticisi Hedefi"}
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
        onSelectionChange={(rows) => setSelectedRows(rows as ManagerTargetRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <Button variant="danger" appearance="filled" onClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} kayıt`)}>{`Seçilenleri sil (${selectedRows.length})`}</Button>
          )}

          <Button variant="primary" appearance="filled" onClick={() => setShowCreate(true)}>{"+ Yeni Portföy Yöneticisi Hedefi"}</Button>
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
      <ManagerTargetCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={(created) => { list.invalidate(); if (created) { setEditRecord(created); setJustCreated(true); } }} />
      <ManagerTargetEdit record={editRecord} onClose={() => { setEditRecord(null); setJustCreated(false); }} onSuccess={list.invalidate} justCreated={justCreated} />

    </>
  );
};

