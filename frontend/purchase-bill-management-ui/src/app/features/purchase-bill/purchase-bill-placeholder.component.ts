import { Component } from '@angular/core';

@Component({
  selector: 'app-purchase-bill-placeholder',
  standalone: true,
  template: `<div class="min-h-screen bg-slate-100 flex items-center justify-center px-4">
    <div class="rounded-2xl bg-white p-8 shadow-lg">
      <h1 class="text-xl font-semibold text-slate-900">Purchase Bill</h1>
      <p class="mt-2 text-sm text-slate-500">Login worked. The purchase-bill form lands in Stage 5b.</p>
    </div>
  </div>`,
})
export class PurchaseBillPlaceholderComponent {}
