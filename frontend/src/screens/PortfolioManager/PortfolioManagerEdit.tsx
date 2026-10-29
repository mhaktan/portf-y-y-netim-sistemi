import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparCheckbox, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';
import { ChildRecords } from '../../shared/ChildRecords';

type PortfolioManagerRecord = {
  id: string | number;
  fullName: string;
  employeeCode: string;
  title?: string;
  phone?: string;
  email: string;
  isActive?: boolean;
  userId?: string;
  branchId: string;
};

interface PortfolioManagerEditProps {
  record: PortfolioManagerRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const PortfolioManagerEdit: React.FC<PortfolioManagerEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<PortfolioManagerRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<PortfolioManagerRecord>) =>
      dataProvider.update('PortfolioManager', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'PortfolioManager', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, display: 'flex', alignItems: 'center' }}>{"Portföy Yöneticisi düzenle"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>

            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Ad Soyad *" value={String(form.fullName ?? '')} onChange={(v) => setField('fullName', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Sicil No *" value={String(form.employeeCode ?? '')} onChange={(v) => setField('employeeCode', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Unvan" value={String(form.title ?? '')} onChange={(v) => setField('title', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Telefon" value={String(form.phone ?? '')} onChange={(v) => setField('phone', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="E-posta *" value={String(form.email ?? '')} onChange={(v) => setField('email', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparCheckbox label="Aktif" checked={!!form.isActive} onChange={(v) => setField('isActive', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Kullanıcı (Sistem)" resource="User" value={String(form.userId ?? '')} onChange={(v) => setField('userId', v)} displayField="name" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Şube *" resource="Branch" value={String(form.branchId ?? '')} onChange={(v) => setField('branchId', v)} displayField="name" />
                </div>
              {record && [{"resource":"Customer","label":"Müşteri","fk":"portfolioManagerId","columns":[{"field":"fullName","label":"Ad Soyad / Unvan","type":"text"},{"field":"customerCode","label":"Müşteri No","type":"text"},{"field":"customerType","label":"Müşteri Tipi","type":"select"}]},{"resource":"CustomerManagerTransfer","label":"Müşteri Devir Kaydı","fk":"portfolioManagerId","columns":[{"field":"transferDate","label":"Devir Tarihi","type":"date"},{"field":"transferReason","label":"Devir Nedeni","type":"text"},{"field":"notes","label":"Notlar","type":"text"}]},{"resource":"ManagerTarget","label":"Portföy Yöneticisi Hedefi","fk":"portfolioManagerId","columns":[{"field":"description","label":"Hedef Açıklaması","type":"text"},{"field":"targetType","label":"Hedef Türü","type":"select"},{"field":"targetValue","label":"Hedef Değer","type":"currency"}]},{"resource":"WorkAssignment","label":"İş Ataması","fk":"portfolioManagerId","columns":[{"field":"title","label":"İş Başlığı","type":"text"},{"field":"description","label":"Açıklama","type":"text"},{"field":"assignmentType","label":"İş Tipi","type":"select"}]},{"resource":"PerformanceReport","label":"Performans Raporu","fk":"portfolioManagerId","columns":[{"field":"reportName","label":"Rapor Adı","type":"text"},{"field":"reportType","label":"Rapor Tipi","type":"select"},{"field":"reportDate","label":"Rapor Tarihi","type":"date"}]}].map((c) => <ChildRecords key={c.resource + c.fk} {...c} parentId={record.id as string | number} />)}
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
