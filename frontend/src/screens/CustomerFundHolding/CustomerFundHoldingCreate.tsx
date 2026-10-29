import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

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
};

const DEFAULTS: Partial<CustomerFundHoldingRecord> = {} as Partial<CustomerFundHoldingRecord>;

interface CustomerFundHoldingCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: (created?: CustomerFundHoldingRecord) => void;
}

export const CustomerFundHoldingCreate: React.FC<CustomerFundHoldingCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<CustomerFundHoldingRecord>>(DEFAULTS);
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<CustomerFundHoldingRecord>) => dataProvider.create('CustomerFundHolding', values),
    onSuccess: (data, values) => { triggerFlows('create', 'CustomerFundHolding', values as Record<string, unknown>); onSuccess(data as CustomerFundHoldingRecord); onClose(); setForm(DEFAULTS); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>{"Yeni Müşteri Fon Pozisyonu"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Adet / Pay *" value={String(form.units ?? '')} onChange={(v) => setField('units', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Alış Fiyatı" value={String(form.purchasePrice ?? '')} onChange={(v) => setField('purchasePrice', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Güncel Değer (₺)" value={String(form.currentValue ?? '')} onChange={(v) => setField('currentValue', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Alış Tarihi" value={String(form.purchaseDate ?? '')} onChange={(v) => setField('purchaseDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Portföy Ağırlığı (%)" value={String(form.weight ?? '')} onChange={(v) => setField('weight', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Kâr / Zarar (₺)" value={String(form.profitLoss ?? '')} onChange={(v) => setField('profitLoss', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Müşteri *" resource="Customer" value={String(form.customerId ?? '')} onChange={(v) => setField('customerId', v)} displayField="fullName" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Fon *" resource="Fund" value={String(form.fundId ?? '')} onChange={(v) => setField('fundId', v)} displayField="fundName" />
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
