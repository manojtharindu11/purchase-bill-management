import { Component, input } from '@angular/core';

/** Compact row-wise item summary for the purchase bill sidebar. */
@Component({
  selector: 'app-bill-summary',
  standalone: true,
  template: `
    <div class="divide-y divide-slate-100 border-y border-slate-100">
      <div class="flex items-center justify-between gap-4 py-3">
        <span class="text-xs font-medium text-slate-500">Total Items</span>
        <strong class="text-sm font-semibold text-slate-900">{{ totalItems() }}</strong>
      </div>
      <div class="flex items-center justify-between gap-4 py-3">
        <span class="text-xs font-medium text-slate-500">Total Quantity</span>
        <strong class="text-sm font-semibold text-slate-900">{{ totalQuantity() }}</strong>
      </div>
      <div class="flex items-center justify-between gap-4 py-3">
        <span class="text-xs font-medium text-slate-500">Total Cost</span>
        <strong class="text-sm font-semibold text-slate-900">{{ totalCost().toFixed(2) }}</strong>
      </div>
      <div class="flex items-center justify-between gap-4 py-3">
        <span class="text-xs font-medium text-slate-500">Total Selling</span>
        <strong class="text-sm font-semibold text-slate-900">{{
          totalSelling().toFixed(2)
        }}</strong>
      </div>
    </div>
  `,
})
export class BillSummaryComponent {
  totalItems = input<number>(0);
  totalQuantity = input<number>(0);
  totalCost = input<number>(0);
  totalSelling = input<number>(0);
}
