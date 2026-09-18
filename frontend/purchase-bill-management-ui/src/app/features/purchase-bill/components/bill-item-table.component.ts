import { Component, input, output } from '@angular/core';
import { BillItemDraft, computeMarginPercent } from '../../../core/models/purchase-bill.models';

/** Reusable table of bill line items with per-row remove. */
@Component({
  selector: 'app-bill-item-table',
  standalone: true,
  template: `
    <div class="overflow-x-auto">
      <table class="w-full min-w-190 text-left text-xs sm:text-sm">
        <thead class="border-b border-slate-200 text-[11px] uppercase tracking-wide text-slate-500">
          <tr>
            <th class="px-3 py-3 font-medium">Item</th>
            <th class="px-3 py-3 font-medium">Batch</th>
            <th class="px-3 py-3 text-right font-medium">Std. Cost</th>
            <th class="px-3 py-3 text-right font-medium">Std. Price</th>
            <th class="px-3 py-3 text-right font-medium">Margin</th>
            <th class="px-3 py-3 text-right font-medium">Qty</th>
            <th class="px-3 py-3 text-right font-medium">Free Qty</th>
            <th class="px-3 py-3 text-right font-medium">Discount</th>
            <th class="px-3 py-3 text-right font-medium">Total Cost</th>
            <th class="px-3 py-3 text-right font-medium">Total Selling</th>
            <th class="px-3 py-3"><span class="sr-only">Actions</span></th>
          </tr>
        </thead>
        <tbody>
          @for (item of items(); track $index) {
            <tr class="border-b border-slate-100 last:border-b-0">
              <td class="px-3 py-3 font-medium text-slate-800">{{ item.itemName }}</td>
              <td class="px-3 py-3 text-slate-600">{{ item.batchLocationName }}</td>
              <td class="px-3 py-3 text-right">{{ item.standardCost.toFixed(2) }}</td>
              <td class="px-3 py-3 text-right">{{ item.standardPrice.toFixed(2) }}</td>
              <td class="px-3 py-3 text-right">{{ marginPercent(item) }}%</td>
              <td class="px-3 py-3 text-right">{{ item.quantity }}</td>
              <td class="px-3 py-3 text-right">{{ item.freeQuantity }}</td>
              <td class="px-3 py-3 text-right">{{ item.discountPercent }}</td>
              <td class="px-3 py-3 text-right font-medium">{{ item.totalCost.toFixed(2) }}</td>
              <td class="px-3 py-3 text-right font-medium">{{ item.totalSelling.toFixed(2) }}</td>
              <td class="px-3 py-3 text-right">
                <button
                  type="button"
                  (click)="remove.emit($index)"
                  class="rounded-lg px-2 py-1 text-xs font-semibold text-red-600 hover:bg-red-50"
                >
                  Remove
                </button>
              </td>
            </tr>
          } @empty {
            <tr class="border-b border-slate-100">
              <td colspan="11" class="px-3 py-6 text-center text-slate-400">
                No items yet. Fill the form above and click “Add Item”.
              </td>
            </tr>
          }
        </tbody>
      </table>
    </div>
  `,
})
export class BillItemTableComponent {
  items = input<BillItemDraft[]>([]);
  remove = output<number>();

  protected marginPercent(item: BillItemDraft): string {
    return computeMarginPercent(item.standardCost, item.standardPrice).toFixed(2);
  }
}
