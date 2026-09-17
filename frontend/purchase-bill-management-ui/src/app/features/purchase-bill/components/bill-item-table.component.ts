import { Component, input, output } from '@angular/core';
import { BillItemDraft } from '../../../core/models/purchase-bill.models';

/** Reusable table of bill line items with per-row remove. */
@Component({
  selector: 'app-bill-item-table',
  standalone: true,
  template: `
    <div class="overflow-x-auto rounded-xl border border-slate-200">
      <table class="w-full min-w-[640px] text-left text-sm">
        <thead class="bg-slate-50 text-xs uppercase tracking-wide text-slate-500">
          <tr>
            <th class="px-4 py-2">Item</th>
            <th class="px-4 py-2 text-right">Std. Cost</th>
            <th class="px-4 py-2 text-right">Std. Price</th>
            <th class="px-4 py-2 text-right">Qty</th>
            <th class="px-4 py-2 text-right">Disc. %</th>
            <th class="px-4 py-2 text-right">Total Cost</th>
            <th class="px-4 py-2 text-right">Total Selling</th>
            <th class="px-4 py-2"><span class="sr-only">Actions</span></th>
          </tr>
        </thead>
        <tbody>
          @for (item of items(); track $index) {
            <tr class="border-t border-slate-100">
              <td class="px-4 py-2 font-medium text-slate-800">{{ item.itemName }}</td>
              <td class="px-4 py-2 text-right">{{ item.standardCost.toFixed(2) }}</td>
              <td class="px-4 py-2 text-right">{{ item.standardPrice.toFixed(2) }}</td>
              <td class="px-4 py-2 text-right">{{ item.quantity }}</td>
              <td class="px-4 py-2 text-right">{{ item.discountPercent }}</td>
              <td class="px-4 py-2 text-right font-medium">{{ item.totalCost.toFixed(2) }}</td>
              <td class="px-4 py-2 text-right font-medium">{{ item.totalSelling.toFixed(2) }}</td>
              <td class="px-4 py-2 text-right">
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
            <tr class="border-t border-slate-100">
              <td colspan="8" class="px-4 py-6 text-center text-slate-400">
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
}
