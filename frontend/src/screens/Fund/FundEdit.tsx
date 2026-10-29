import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparCheckbox, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';
import { ChildRecords } from '../../shared/ChildRecords';

type FundRecord = {
  id: string | number;
  fundCode: string;
  fundName: string;
  fundType: string;
  currency: string;
  benchmarkIndex?: string;
  managementFeeRate?: number;
  isActive?: boolean;
};

interface FundEditProps {
  record: FundRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const FundEdit: React.FC<FundEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<FundRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<FundRecord>) =>
      dataProvider.update('Fund', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'Fund', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, display: 'flex', alignItems: 'center' }}>{"Fon düzenle"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>

            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Fon Kodu *" value={String(form.fundCode ?? '')} onChange={(v) => setField('fundCode', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Fon Adı *" value={String(form.fundName ?? '')} onChange={(v) => setField('fundName', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Fon Tipi *" value={String(form.fundType ?? '')} onChange={(v) => setField('fundType', v ? Number(v) : null)} searchable={false} options={[{ label: "Hisse Senedi Fonu", value: "0" }, { label: "Tahvil Fonu", value: "1" }, { label: "Para Piyasası", value: "2" }, { label: "Karma Fon", value: "3" }, { label: "Endeks Fonu", value: "4" }, { label: "Hedge Fon", value: "5" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Para Birimi *" value={String(form.currency ?? '')} onChange={(v) => setField('currency', v ? Number(v) : null)} searchable={false} options={[{ label: "TL", value: "0" }, { label: "Dolar", value: "1" }, { label: "Euro", value: "2" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Karşılaştırma Endeksi" value={String(form.benchmarkIndex ?? '')} onChange={(v) => setField('benchmarkIndex', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Yönetim Ücreti (%)" value={String(form.managementFeeRate ?? '')} onChange={(v) => setField('managementFeeRate', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparCheckbox label="Aktif" checked={!!form.isActive} onChange={(v) => setField('isActive', v)} />
                </div>
              {record && [{"resource":"CustomerFundHolding","label":"Müşteri Fon Pozisyonu","fk":"fundId","columns":[{"field":"units","label":"Adet / Pay","type":"currency"},{"field":"purchasePrice","label":"Alış Fiyatı","type":"currency"},{"field":"currentValue","label":"Güncel Değer (₺)","type":"currency"}]},{"resource":"MarketData","label":"Piyasa Verisi","fk":"fundId","columns":[{"field":"instrumentName","label":"Enstrüman Adı","type":"text"},{"field":"instrumentCode","label":"Enstrüman Kodu","type":"text"},{"field":"instrumentType","label":"Enstrüman Tipi","type":"select"}]}].map((c) => <ChildRecords key={c.resource + c.fk} {...c} parentId={record.id as string | number} />)}
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
