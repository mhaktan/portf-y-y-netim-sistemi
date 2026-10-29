// ---------------------------------------------------------------------------
// Enum definitions — auto-generated from ER model
// Maps integer values to display labels for enum fields
// ---------------------------------------------------------------------------

// Customer.customerType
export const CustomerCustomerTypeMap: Record<string, string> = {
  "0": "Bireysel",
  "1": "Kurumsal"
};
export const CustomerCustomerTypeOptions = [
  { label: "Bireysel", value: "0" },
  { label: "Kurumsal", value: "1" }
];

// Customer.riskProfile
export const CustomerRiskProfileMap: Record<string, string> = {
  "0": "Muhafazakâr",
  "1": "Dengeli",
  "2": "Agresif"
};
export const CustomerRiskProfileOptions = [
  { label: "Muhafazakâr", value: "0" },
  { label: "Dengeli", value: "1" },
  { label: "Agresif", value: "2" }
];

// Fund.fundType
export const FundFundTypeMap: Record<string, string> = {
  "0": "Hisse Senedi Fonu",
  "1": "Tahvil Fonu",
  "2": "Para Piyasası",
  "3": "Karma Fon",
  "4": "Endeks Fonu",
  "5": "Hedge Fon"
};
export const FundFundTypeOptions = [
  { label: "Hisse Senedi Fonu", value: "0" },
  { label: "Tahvil Fonu", value: "1" },
  { label: "Para Piyasası", value: "2" },
  { label: "Karma Fon", value: "3" },
  { label: "Endeks Fonu", value: "4" },
  { label: "Hedge Fon", value: "5" }
];

// Fund.currency
export const FundCurrencyMap: Record<string, string> = {
  "0": "TL",
  "1": "Dolar",
  "2": "Euro"
};
export const FundCurrencyOptions = [
  { label: "TL", value: "0" },
  { label: "Dolar", value: "1" },
  { label: "Euro", value: "2" }
];

// MarketData.instrumentType
export const MarketDataInstrumentTypeMap: Record<string, string> = {
  "0": "Fon",
  "1": "Hisse Endeksi",
  "2": "Döviz Çifti",
  "3": "Emtia Endeksi",
  "4": "Faiz Oranı"
};
export const MarketDataInstrumentTypeOptions = [
  { label: "Fon", value: "0" },
  { label: "Hisse Endeksi", value: "1" },
  { label: "Döviz Çifti", value: "2" },
  { label: "Emtia Endeksi", value: "3" },
  { label: "Faiz Oranı", value: "4" }
];

// TargetPeriod.periodType
export const TargetPeriodPeriodTypeMap: Record<string, string> = {
  "0": "Günlük",
  "1": "Haftalık",
  "2": "Aylık"
};
export const TargetPeriodPeriodTypeOptions = [
  { label: "Günlük", value: "0" },
  { label: "Haftalık", value: "1" },
  { label: "Aylık", value: "2" }
];

// ManagerTarget.targetType
export const ManagerTargetTargetTypeMap: Record<string, string> = {
  "0": "AUM Büyümesi",
  "1": "Yeni Müşteri",
  "2": "Müşteri Koruma",
  "3": "Getiri Oranı",
  "4": "İşlem Adedi",
  "5": "Özel Hedef"
};
export const ManagerTargetTargetTypeOptions = [
  { label: "AUM Büyümesi", value: "0" },
  { label: "Yeni Müşteri", value: "1" },
  { label: "Müşteri Koruma", value: "2" },
  { label: "Getiri Oranı", value: "3" },
  { label: "İşlem Adedi", value: "4" },
  { label: "Özel Hedef", value: "5" }
];

// ManagerTarget.status
export const ManagerTargetStatusMap: Record<string, string> = {
  "0": "Taslak",
  "1": "Onay Bekliyor",
  "2": "Aktif",
  "3": "Tamamlandı",
  "4": "İptal"
};
export const ManagerTargetStatusOptions = [
  { label: "Taslak", value: "0" },
  { label: "Onay Bekliyor", value: "1" },
  { label: "Aktif", value: "2" },
  { label: "Tamamlandı", value: "3" },
  { label: "İptal", value: "4" }
];

// WorkAssignment.assignmentType
export const WorkAssignmentAssignmentTypeMap: Record<string, string> = {
  "0": "Müşteri Ziyareti",
  "1": "Rapor Hazırlama",
  "2": "Portföy İnceleme",
  "3": "Eğitim Görevi",
  "4": "Uyum Görevi",
  "5": "Diğer"
};
export const WorkAssignmentAssignmentTypeOptions = [
  { label: "Müşteri Ziyareti", value: "0" },
  { label: "Rapor Hazırlama", value: "1" },
  { label: "Portföy İnceleme", value: "2" },
  { label: "Eğitim Görevi", value: "3" },
  { label: "Uyum Görevi", value: "4" },
  { label: "Diğer", value: "5" }
];

// WorkAssignment.priority
export const WorkAssignmentPriorityMap: Record<string, string> = {
  "0": "Düşük",
  "1": "Orta",
  "2": "Yüksek",
  "3": "Kritik"
};
export const WorkAssignmentPriorityOptions = [
  { label: "Düşük", value: "0" },
  { label: "Orta", value: "1" },
  { label: "Yüksek", value: "2" },
  { label: "Kritik", value: "3" }
];

// WorkAssignment.status
export const WorkAssignmentStatusMap: Record<string, string> = {
  "0": "Atandı",
  "1": "Devam Ediyor",
  "2": "Onay Bekliyor",
  "3": "Tamamlandı",
  "4": "İptal"
};
export const WorkAssignmentStatusOptions = [
  { label: "Atandı", value: "0" },
  { label: "Devam Ediyor", value: "1" },
  { label: "Onay Bekliyor", value: "2" },
  { label: "Tamamlandı", value: "3" },
  { label: "İptal", value: "4" }
];

// PerformanceReport.reportType
export const PerformanceReportReportTypeMap: Record<string, string> = {
  "0": "Günlük",
  "1": "Haftalık",
  "2": "Aylık",
  "3": "Çeyreklik",
  "4": "Yıllık"
};
export const PerformanceReportReportTypeOptions = [
  { label: "Günlük", value: "0" },
  { label: "Haftalık", value: "1" },
  { label: "Aylık", value: "2" },
  { label: "Çeyreklik", value: "3" },
  { label: "Yıllık", value: "4" }
];

// PerformanceReport.status
export const PerformanceReportStatusMap: Record<string, string> = {
  "0": "Taslak",
  "1": "Onay Bekliyor",
  "2": "Onaylandı",
  "3": "Revizyon",
  "4": "İptal"
};
export const PerformanceReportStatusOptions = [
  { label: "Taslak", value: "0" },
  { label: "Onay Bekliyor", value: "1" },
  { label: "Onaylandı", value: "2" },
  { label: "Revizyon", value: "3" },
  { label: "İptal", value: "4" }
];
