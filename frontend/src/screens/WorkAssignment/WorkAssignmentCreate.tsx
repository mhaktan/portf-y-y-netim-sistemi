import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type WorkAssignmentRecord = {
  id: string | number;
  title: string;
  description?: string;
  assignmentType: string;
  priority: string;
  dueDate: string;
  completedDate?: string;
  status: string;
  completionNote?: string;
  revisionNote?: string;
  portfolioManagerId: string;
};

const DEFAULTS: Partial<WorkAssignmentRecord> = {} as Partial<WorkAssignmentRecord>;

interface WorkAssignmentCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: (created?: WorkAssignmentRecord) => void;
}

export const WorkAssignmentCreate: React.FC<WorkAssignmentCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<WorkAssignmentRecord>>(DEFAULTS);
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<WorkAssignmentRecord>) => dataProvider.create('WorkAssignment', values),
    onSuccess: (data, values) => { triggerFlows('create', 'WorkAssignment', values as Record<string, unknown>); onSuccess(data as WorkAssignmentRecord); onClose(); setForm(DEFAULTS); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>{"Yeni İş Ataması"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="İş Başlığı *" value={String(form.title ?? '')} onChange={(v) => setField('title', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Açıklama" value={String(form.description ?? '')} onChange={(v) => setField('description', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="İş Tipi *" value={String(form.assignmentType ?? '')} onChange={(v) => setField('assignmentType', v ? Number(v) : null)} searchable={false} options={[{ label: "Müşteri Ziyareti", value: "0" }, { label: "Rapor Hazırlama", value: "1" }, { label: "Portföy İnceleme", value: "2" }, { label: "Eğitim Görevi", value: "3" }, { label: "Uyum Görevi", value: "4" }, { label: "Diğer", value: "5" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Öncelik *" value={String(form.priority ?? '')} onChange={(v) => setField('priority', v ? Number(v) : null)} searchable={false} options={[{ label: "Düşük", value: "0" }, { label: "Orta", value: "1" }, { label: "Yüksek", value: "2" }, { label: "Kritik", value: "3" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Teslim Tarihi *" value={String(form.dueDate ?? '')} onChange={(v) => setField('dueDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Tamamlanma Tarihi" value={String(form.completedDate ?? '')} onChange={(v) => setField('completedDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Tamamlama Notu" value={String(form.completionNote ?? '')} onChange={(v) => setField('completionNote', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Portföy Yöneticisi *" resource="PortfolioManager" value={String(form.portfolioManagerId ?? '')} onChange={(v) => setField('portfolioManagerId', v)} displayField="fullName" />
                </div>
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
