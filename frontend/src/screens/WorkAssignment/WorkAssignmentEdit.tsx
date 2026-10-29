import React, { useState, useEffect } from 'react';
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
const WORKFLOW: { statusField: string; statuses: string[]; actions: Array<{ action: string; label?: string; from: number[]; anyFrom?: boolean; needsNote?: boolean }>; locked: number[] } = {"statusField":"status","statuses":["Atandı","Devam Ediyor","Onay Bekliyor","Tamamlandı","İptal"],"actions":[{"action":"Start","label":"Başlat","from":[0]},{"action":"Submit","label":"Gönder","from":[1]},{"action":"Cancel","label":"İptal et","from":[],"anyFrom":true}],"locked":[2,3]};

interface WorkAssignmentEditProps {
  record: WorkAssignmentRecord | null;
  onClose: () => void;
  onSuccess: () => void;
  /** Talep yeni kaydedildi: sonraki adim yonlendirmesi gosterilir */
  justCreated?: boolean;
}

export const WorkAssignmentEdit: React.FC<WorkAssignmentEditProps> = ({ record, onClose, onSuccess, justCreated }) => {
  const [form, setForm] = useState<Partial<WorkAssignmentRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();
  const [wfBusy, setWfBusy] = useState(false);
  const status = Number((record as Record<string, unknown> | null)?.[WORKFLOW.statusField]);
  const locked = WORKFLOW.locked.includes(status);
  const wfActions = WORKFLOW.actions.filter((a) => a.anyFrom || a.from.includes(status));
  const runAction = async (a: { action: string; label?: string; needsNote?: boolean }) => {
    const note = a.needsNote ? window.prompt(`${a.label ?? a.action}: açıklama`) ?? '' : undefined;
    if (a.needsNote && !note?.trim()) return;
    setWfBusy(true);
    try {
      await dataProvider.changeStatus('WorkAssignment', record!.id, a.action, note);
      onSuccess(); onClose();
    } catch (err) {
      window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: (err as Error).message } }));
    } finally { setWfBusy(false); }
  };

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<WorkAssignmentRecord>) =>
      dataProvider.update('WorkAssignment', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'WorkAssignment', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, display: 'flex', alignItems: 'center' }}>{"İş Ataması düzenle"}
          <span style={{ marginLeft: 12, fontSize: 12, fontWeight: 600, padding: '2px 10px', borderRadius: 12, background: locked ? '#fff3e0' : '#e8f5e9', color: locked ? '#e65100' : '#2e7d32' }}>{WORKFLOW.statuses[status] ?? ''}</span></h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>

            {justCreated && !locked && <div role="status" style={{ marginBottom: 16, padding: '8px 12px', borderRadius: 8, background: '#e8f5e9', color: '#2e7d32', fontSize: 13 }}>Kaydedildi. Hazır olunca alttaki gönderme düğmesine basın.</div>}
            {locked && <div role="status" style={{ marginBottom: 16, padding: '8px 12px', borderRadius: 8, background: '#fff3e0', color: '#e65100', fontSize: 13 }}>Bu kayıt onay sürecinde ya da onaylanmış; düzenlenemez.</div>}
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
            <div style={{ marginRight: 'auto', display: 'flex', gap: 8 }}>
              {wfActions.map((a) => (
                <React.Fragment key={a.action}>
                  <Button variant="primary" appearance="outlined" onClick={() => void runAction(a)} disabled={wfBusy}>{`${a.label ?? a.action}`}</Button>
                </React.Fragment>
              ))}
            </div>
            <Button variant="primary" appearance="outlined" onClick={onClose}>{"Vazgeç"}</Button>
            {!locked && <Button variant="primary" appearance="filled" type="submit" disabled={mutation.isPending}>{mutation.isPending ? "Kaydediliyor…" : "Kaydet"}</Button>}
          </div>
        </form>
      </div>
    </div>
  );
};
