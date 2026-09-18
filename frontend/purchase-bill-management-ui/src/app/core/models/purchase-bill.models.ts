export interface PurchaseBillItemRequest {
  itemName: string;
  batchLocationName: string;
  standardCost: number;
  standardPrice: number;
  quantity: number;
  discountPercent: number;
  freeQuantity: number;
  margin: number;
}

export interface PurchaseBillRequest {
  items: PurchaseBillItemRequest[];
}

export interface PurchaseBillItemResponse extends PurchaseBillItemRequest {
  id: number;
  totalCost: number;
  totalSelling: number;
}

export interface PurchaseBillResponse {
  id: number;
  billNumber: string;
  totalItems: number;
  totalQuantity: number;
  totalCost: number;
  totalSelling: number;
  createdAt: string;
  items: PurchaseBillItemResponse[];
}

/** Draft row kept client-side before submit. Totals are computed locally. */
export interface BillItemDraft extends PurchaseBillItemRequest {
  totalCost: number;
  totalSelling: number;
}

export const ALLOWED_ITEMS = ['Mango', 'Apple', 'Banana', 'Orange', 'Grapes'] as const;

export function computeItemTotals(
  standardCost: number,
  standardPrice: number,
  quantity: number,
  discountPercent: number,
): { totalCost: number; totalSelling: number } {
  const grossCost = standardCost * quantity;
  const totalCost = grossCost - (grossCost * discountPercent) / 100;
  const totalSelling = standardPrice * quantity;
  return { totalCost, totalSelling };
}

export function computeMarginPercent(standardCost: number, standardPrice: number): number {
  if (standardPrice <= 0) return 0;
  return ((standardPrice - standardCost) / standardPrice) * 100;
}
