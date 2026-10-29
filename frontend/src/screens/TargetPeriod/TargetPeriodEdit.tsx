import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparCheckbox, SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';
import { ChildRecords } from '../../shared/ChildRecords';

type TargetPeriodRecord = {
  id: string | number;
  periodName: string;
  periodType: string;
  startDate: string;
  endDate: string;
  year: number;
  isActive?: boolean;
};

interface TargetPeriodEditProps {
  record: TargetPeriodRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const TargetPeriodEdit: React.FC<TargetPeriodEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<TargetPeriodRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<TargetPeriodRecord>) =>
      dataProvider.update('TargetPeriod', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'TargetPeriod', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, display: 'flex', alignItems: 'center' }}>{"Hedef Dönemi düzenle"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>

            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Dönem Adı *" value={String(form.periodName ?? '')} onChange={(v) => setField('periodName', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Dönem Tipi *" value={String(form.periodType ?? '')} onChange={(v) => setField('periodType', v ? Number(v) : null)} searchable={false} options={[{ label: "Günlük", value: "0" }, { label: "Haftalık", value: "1" }, { label: "Aylık", value: "2" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Başlangıç *" value={String(form.startDate ?? '')} onChange={(v) => setField('startDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Bitiş *" value={String(form.endDate ?? '')} onChange={(v) => setField('endDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Yıl *" value={String(form.year ?? '')} onChange={(v) => setField('year', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparCheckbox label="Aktif" checked={!!form.isActive} onChange={(v) => setField('isActive', v)} />
                </div>
              {record && [{"resource":"ManagerTarget","label":"Portföy Yöneticisi Hedefi","fk":"targetPeriodId","columns":[{"field":"description","label":"Hedef Açıklaması","type":"text"},{"field":"targetType","label":"Hedef Türü","type":"select"},{"field":"targetValue","label":"Hedef Değer","type":"currency"}]},{"resource":"PerformanceReport","label":"Performans Raporu","fk":"targetPeriodId","columns":[{"field":"reportName","label":"Rapor Adı","type":"text"},{"field":"reportType","label":"Rapor Tipi","type":"select"},{"field":"reportDate","label":"Rapor Tarihi","type":"date"}]}].map((c) => <ChildRecords key={c.resource + c.fk} {...c} parentId={record.id as string | number} />)}
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <Button variant="primary" appearance="outlined" onClick={onClose}>{"Vazgeç"}</Button>
            <Button variant="primary" appearance="filled" type="submit" disabled={mutation.isPending}>{mutation.isPending ? "Kaydediliyor…" : "Kaydet"}</Button>
          </div>
        </form>
      </div>
    </div>
  );
};
