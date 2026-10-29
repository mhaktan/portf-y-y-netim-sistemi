import React from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { dataProvider } from '../dataProvider';
import { formatDate } from './format';

interface Props {
  resource: string;
  label: string;
  fk: string;
  parentId: string | number;
  columns: Array<{ field: string; label: string; type: string }>;
  /** Satir ici ekleme alanlari (yoksa yalniz liste) */
  addFields?: Array<{ field: string; label: string; type: string; required?: boolean }>;
  /** Ust kayit kilitliyse (onayda) ekleme kapali */
  readOnly?: boolean;
}

const inputType = (t: string) => (t === 'number' || t === 'currency' ? 'number' : t === 'date' ? 'date' : t === 'datetime' ? 'datetime-local' : 'text');

const cell = (v: unknown, type: string): string =>
  v == null || v === '' ? '—'
    : type === 'date' || type === 'datetime' ? formatDate(v, type === 'datetime')
    : typeof v === 'boolean' ? (v ? "Evet" : "Hayır")
    : type === 'currency' ? Number(v).toLocaleString("tr-TR", { minimumFractionDigits: 2 })
    : String(v);

export const ChildRecords: React.FC<Props> = ({ resource, label, fk, parentId, columns, addFields, readOnly }) => {
  const qc = useQueryClient();
  const [draft, setDraft] = React.useState<Record<string, unknown>>({});
  const add = useMutation({
    mutationFn: () => dataProvider.create(resource, { ...draft, [fk]: parentId }),
    onSuccess: () => { setDraft({}); void qc.invalidateQueries({ queryKey: [resource] }); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });
  const canAdd = !readOnly && !!addFields?.length;
  const missing = addFields?.some((f) => f.required && (draft[f.field] === undefined || draft[f.field] === ''));
  const { data, isLoading } = useQuery({
    queryKey: [resource, 'children', fk, parentId],
    queryFn: () => dataProvider.list<Record<string, unknown>>(resource, { pagination: { page: 1, perPage: 5 }, filter: { [fk]: String(parentId) } }),
  });
  const rows = data?.data ?? [];
  const total = data?.total ?? rows.length;
  const href = `/${resource}?${encodeURIComponent(fk)}=${encodeURIComponent(String(parentId))}`;
  return (
    <section style={{ gridColumn: '1 / -1', marginTop: 8, border: '1px solid #eee', borderRadius: 8, padding: 12 }} aria-label={label}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 }}>
        <strong style={{ fontSize: 14 }}>{label} ({total})</strong>
        <Link to={href} style={{ fontSize: 13, color: '#1976d2', fontWeight: 600, textDecoration: 'none' }}>Tümünü yönet →</Link>
      </div>
      {isLoading ? <div style={{ fontSize: 13, color: '#999' }}>{"Yükleniyor…"}</div>
        : rows.length === 0 ? <div style={{ fontSize: 13, color: '#999' }}>{"Kayıt yok"}</div>
        : (
          <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
            <thead><tr>{columns.map((c) => <th key={c.field} style={{ textAlign: 'left', padding: '4px 6px', color: '#666', fontWeight: 600, borderBottom: '1px solid #eee' }}>{c.label}</th>)}</tr></thead>
            <tbody>{rows.map((r, i) => (
              <tr key={String(r.id ?? i)}>{columns.map((c) => <td key={c.field} style={{ padding: '4px 6px', borderBottom: '1px solid #f5f5f5' }}>{cell(r[c.field], c.type)}</td>)}</tr>
            ))}</tbody>
          </table>
        )}
      {canAdd && (
        <div style={{ display: 'flex', gap: 6, alignItems: 'flex-end', flexWrap: 'wrap', marginTop: 8 }}>
          {addFields!.map((f) => (
            <label key={f.field} style={{ display: 'flex', flexDirection: 'column', gap: 2, fontSize: 12, color: '#666', flex: f.type === 'boolean' ? '0 0 auto' : '1 1 120px' }}>
              {f.label}{f.required ? ' *' : ''}
              {f.type === 'boolean'
                ? <input type="checkbox" checked={!!draft[f.field]} onChange={(e) => setDraft((d) => ({ ...d, [f.field]: e.target.checked }))} />
                : <input type={inputType(f.type)} value={String(draft[f.field] ?? '')}
                    onChange={(e) => setDraft((d) => ({ ...d, [f.field]: inputType(f.type) === 'number' ? (e.target.value === '' ? '' : Number(e.target.value)) : e.target.value }))}
                    style={{ padding: '6px 8px', border: '1px solid #ccc', borderRadius: 6, fontSize: 13 }} />}
            </label>
          ))}
          <button type="button" onClick={() => add.mutate()} disabled={add.isPending || missing}
            style={{ padding: '7px 14px', borderRadius: 6, border: 'none', background: missing ? '#bbb' : '#1976d2', color: '#fff', fontWeight: 600, cursor: missing ? 'default' : 'pointer' }}>
            {add.isPending ? '…' : "Ekle"}
          </button>
        </div>
      )}
    </section>
  );
};
