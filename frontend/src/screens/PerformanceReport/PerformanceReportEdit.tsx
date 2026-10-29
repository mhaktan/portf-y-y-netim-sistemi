import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type PerformanceReportRecord = {
  id: string | number;
  reportName: string;
  reportType: string;
  reportDate: string;
  startDate: string;
  endDate: string;
  totalAum?: number;
  aumChange?: number;
  aumChangeRate?: number;
  portfolioReturnRate?: number;
  benchmarkReturnRate?: number;
  alphaValue?: number;
  customerCount?: number;
  newCustomerCount?: number;
  targetAchievementRate?: number;
  summary?: string;
  status: string;
  revisionNote?: string;
  portfolioManagerId: string;
  targetPeriodId: string;
};
const WORKFLOW: { statusField: string; statuses: string[]; actions: Array<{ action: string; label?: string; from: number[]; anyFrom?: boolean; needsNote?: boolean }>; locked: number[] } = {"statusField":"status","statuses":["Taslak","Onay Bekliyor","Onaylandı","Revizyon","İptal"],"actions":[{"action":"Submit","label":"Gönder","from":[0]},{"action":"Resubmit","label":"Yeniden gönder","from":[3]},{"action":"Cancel","label":"İptal et","from":[],"anyFrom":true}],"locked":[1,2]};

interface PerformanceReportEditProps {
  record: PerformanceReportRecord | null;
  onClose: () => void;
  onSuccess: () => void;
  /** Talep yeni kaydedildi: sonraki adim yonlendirmesi gosterilir */
  justCreated?: boolean;
}

export const PerformanceReportEdit: React.FC<PerformanceReportEditProps> = ({ record, onClose, onSuccess, justCreated }) => {
  const [form, setForm] = useState<Partial<PerformanceReportRecord>>(record ?? {});
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
      await dataProvider.changeStatus('PerformanceReport', record!.id, a.action, note);
      onSuccess(); onClose();
    } catch (err) {
      window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: (err as Error).message } }));
    } finally { setWfBusy(false); }
  };

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<PerformanceReportRecord>) =>
      dataProvider.update('PerformanceReport', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'PerformanceReport', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, display: 'flex', alignItems: 'center' }}>{"Performans Raporu düzenle"}
          <span style={{ marginLeft: 12, fontSize: 12, fontWeight: 600, padding: '2px 10px', borderRadius: 12, background: locked ? '#fff3e0' : '#e8f5e9', color: locked ? '#e65100' : '#2e7d32' }}>{WORKFLOW.statuses[status] ?? ''}</span></h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>

            {justCreated && !locked && <div role="status" style={{ marginBottom: 16, padding: '8px 12px', borderRadius: 8, background: '#e8f5e9', color: '#2e7d32', fontSize: 13 }}>Kaydedildi. Hazır olunca alttaki gönderme düğmesine basın.</div>}
            {locked && <div role="status" style={{ marginBottom: 16, padding: '8px 12px', borderRadius: 8, background: '#fff3e0', color: '#e65100', fontSize: 13 }}>Bu kayıt onay sürecinde ya da onaylanmış; düzenlenemez.</div>}
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Rapor Adı *" value={String(form.reportName ?? '')} onChange={(v) => setField('reportName', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Rapor Tipi *" value={String(form.reportType ?? '')} onChange={(v) => setField('reportType', v ? Number(v) : null)} searchable={false} options={[{ label: "Günlük", value: "0" }, { label: "Haftalık", value: "1" }, { label: "Aylık", value: "2" }, { label: "Çeyreklik", value: "3" }, { label: "Yıllık", value: "4" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Rapor Tarihi *" value={String(form.reportDate ?? '')} onChange={(v) => setField('reportDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Dönem Başı *" value={String(form.startDate ?? '')} onChange={(v) => setField('startDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Dönem Sonu *" value={String(form.endDate ?? '')} onChange={(v) => setField('endDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Toplam AUM (₺)" value={String(form.totalAum ?? '')} onChange={(v) => setField('totalAum', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="AUM Değişimi (₺)" value={String(form.aumChange ?? '')} onChange={(v) => setField('aumChange', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="AUM Değişim Oranı (%)" value={String(form.aumChangeRate ?? '')} onChange={(v) => setField('aumChangeRate', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Portföy Getiri Oranı (%)" value={String(form.portfolioReturnRate ?? '')} onChange={(v) => setField('portfolioReturnRate', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Kıyaslama Getiri Oranı (%)" value={String(form.benchmarkReturnRate ?? '')} onChange={(v) => setField('benchmarkReturnRate', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Alfa Değeri" value={String(form.alphaValue ?? '')} onChange={(v) => setField('alphaValue', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Müşteri Sayısı" value={String(form.customerCount ?? '')} onChange={(v) => setField('customerCount', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Yeni Müşteri Sayısı" value={String(form.newCustomerCount ?? '')} onChange={(v) => setField('newCustomerCount', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Hedef Gerçekleşme (%)" value={String(form.targetAchievementRate ?? '')} onChange={(v) => setField('targetAchievementRate', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Yönetici Özeti" value={String(form.summary ?? '')} onChange={(v) => setField('summary', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Portföy Yöneticisi *" resource="PortfolioManager" value={String(form.portfolioManagerId ?? '')} onChange={(v) => setField('portfolioManagerId', v)} displayField="fullName" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Hedef Dönemi *" resource="TargetPeriod" value={String(form.targetPeriodId ?? '')} onChange={(v) => setField('targetPeriodId', v)} displayField="periodName" />
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
