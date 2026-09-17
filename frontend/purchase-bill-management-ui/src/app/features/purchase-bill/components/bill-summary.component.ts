import { Component, input } from '@angular/core';

/** Item Summary section: total rows + summed quantity (+ money totals). */
@Component({
  selector: 'app-bill-summary',
  standalone: true,
  template: `
    <div class="grid grid-cols-2 gap-3 sm:grid-cols-4">
      <div class="rounded-xl bg-slate-50 p-4">
        <p class="text-xs font-medium uppercase tracking-wide text-slate-500">Total Items</p>
        <p class="mt-1 text-2xl font-bold text-slate-900">{{ totalItems() }}</p>
      </div>
      <div class="rounded-xl bg-slate-50 p-4">
        <p class="text-xs font-medium uppercase tracking-wide text-slate-500">Total Quantity</p>
        <p class="mt-1 text-2xl font-bold text-slate-900">{{ totalQuantity() }}</p>
      </div>
      <div class="rounded-xl bg-slate-50 p-4">
        <p class="text-xs font-medium uppercase tracking-wide text-slate-500">Total Cost</p>
        <p class="mt-1 text-2xl font-bold text-slate-900">{{ totalCost().toFixed(2) }}</p>
      </div>
      <div class="rounded-xl bg-slate-50 p-4">
        <p class="text-xs font-medium uppercase tracking-wide text-slate-500">Total Selling</p>
        <p class="mt-1 text-2xl font-bold text-slate-900">{{ totalSelling().toFixed(2) }}</p>
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
