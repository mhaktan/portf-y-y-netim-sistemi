import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Button } from '@takeoff-ui/react-spar';
import { SparDateField, SparTextField } from '../../shared/ui';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type MarketDataRecord = {
  id: string | number;
  instrumentName: string;
  instrumentCode: string;
  instrumentType: string;
  dataDate: string;
  openPrice?: number;
  closePrice: number;
  highPrice?: number;
  lowPrice?: number;
  volume?: number;
  navPerUnit?: number;
  dailyReturnRate?: number;
  fundId: string;
};

interface MarketDataEditProps {
  record: MarketDataRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const MarketDataEdit: React.FC<MarketDataEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<MarketDataRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<MarketDataRecord>) =>
      dataProvider.update('MarketData', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'MarketData', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700, display: 'flex', alignItems: 'center' }}>{"Piyasa Verisi düzenle"}</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>

            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Enstrüman Adı *" value={String(form.instrumentName ?? '')} onChange={(v) => setField('instrumentName', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Enstrüman Kodu *" value={String(form.instrumentCode ?? '')} onChange={(v) => setField('instrumentCode', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Enstrüman Tipi *" value={String(form.instrumentType ?? '')} onChange={(v) => setField('instrumentType', v ? Number(v) : null)} searchable={false} options={[{ label: "Fon", value: "0" }, { label: "Hisse Endeksi", value: "1" }, { label: "Döviz Çifti", value: "2" }, { label: "Emtia Endeksi", value: "3" }, { label: "Faiz Oranı", value: "4" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparDateField label="Tarih *" value={String(form.dataDate ?? '')} onChange={(v) => setField('dataDate', v)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Açılış Fiyatı" value={String(form.openPrice ?? '')} onChange={(v) => setField('openPrice', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Kapanış Fiyatı *" value={String(form.closePrice ?? '')} onChange={(v) => setField('closePrice', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Günlük En Yüksek" value={String(form.highPrice ?? '')} onChange={(v) => setField('highPrice', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Günlük En Düşük" value={String(form.lowPrice ?? '')} onChange={(v) => setField('lowPrice', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="İşlem Hacmi" value={String(form.volume ?? '')} onChange={(v) => setField('volume', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Birim Pay Değeri (NAV)" value={String(form.navPerUnit ?? '')} onChange={(v) => setField('navPerUnit', v === '' ? null : Number(v))} type="number" />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <SparTextField label="Günlük Getiri (%)" value={String(form.dailyReturnRate ?? '')} onChange={(v) => setField('dailyReturnRate', v === '' ? null : Number(v))} type="number" />
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
