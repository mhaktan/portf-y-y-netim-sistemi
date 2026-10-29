import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type CustomerManagerTransferRecord = {
  id: string | number;
  transferDate: string;
  transferReason?: string;
  notes?: string;
  customerId: string;
  portfolioManagerId: string;
};

const DEFAULTS: Partial<CustomerManagerTransferRecord> = {} as Partial<CustomerManagerTransferRecord>;

interface CustomerManagerTransferCreateProps {
  open: boolean;
  onClose: () => void;
  onSuccess: (created?: CustomerManagerTransferRecord) => void;
}

export const CustomerManagerTransferCreate: React.FC<CustomerManagerTransferCreateProps> = ({ open, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<CustomerManagerTransferRecord>>(DEFAULTS);
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  const mutation = useMutation({
    mutationFn: (values: Partial<CustomerManagerTransferRecord>) => dataProvider.create('CustomerManagerTransfer', values),
    onSuccess: (data, values) => { triggerFlows('create', 'CustomerManagerTransfer', values as Record<string, unknown>); onSuccess(data as CustomerManagerTransferRecord); onClose(); setForm(DEFAULTS); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!open) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>{"Yeni Müşteri Devir Kaydı"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Devir Tarihi *" value={String(form.transferDate ?? '')} onChange={(v) => setField('transferDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Devir Nedeni" value={String(form.transferReason ?? '')} onChange={(v) => setField('transferReason', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Notlar" value={String(form.notes ?? '')} onChange={(v) => setField('notes', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Müşteri *" resource="Customer" value={String(form.customerId ?? '')} onChange={(v) => setField('customerId', v)} displayField="fullName" />
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
