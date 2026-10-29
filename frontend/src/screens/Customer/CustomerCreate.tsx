import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

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
};

const DEFAULTS: Partial<CustomerRecord> = {} as Partial<CustomerRecord>;

interface CustomerCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: (created?: CustomerRecord) => void;
}

export const CustomerCreate: React.FC<CustomerCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<CustomerRecord>>(DEFAULTS);
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<CustomerRecord>) => dataProvider.create('Customer', values),
    onSuccess: (data, values) => { triggerFlows('create', 'Customer', values as Record<string, unknown>); onSuccess(data as CustomerRecord); onClose(); setForm(DEFAULTS); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>{"Yeni Müşteri"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Ad Soyad / Unvan *" value={String(form.fullName ?? '')} onChange={(v) => setField('fullName', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Müşteri No *" value={String(form.customerCode ?? '')} onChange={(v) => setField('customerCode', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Müşteri Tipi *" value={String(form.customerType ?? '')} onChange={(v) => setField('customerType', v ? Number(v) : null)} searchable={false} options={[{ label: "Bireysel", value: "0" }, { label: "Kurumsal", value: "1" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Telefon" value={String(form.phone ?? '')} onChange={(v) => setField('phone', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="E-posta" value={String(form.email ?? '')} onChange={(v) => setField('email', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Risk Profili" value={String(form.riskProfile ?? '')} onChange={(v) => setField('riskProfile', v ? Number(v) : null)} searchable={false} options={[{ label: "Muhafazakâr", value: "0" }, { label: "Dengeli", value: "1" }, { label: "Agresif", value: "2" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Toplam AUM (₺)" value={String(form.totalAum ?? '')} onChange={(v) => setField('totalAum', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Müşteri Başlangıç Tarihi" value={String(form.onboardingDate ?? '')} onChange={(v) => setField('onboardingDate', v)} />
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
