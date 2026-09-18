import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { LoadingService } from '../../core/services/loading.service';
import { PurchaseBillApiService } from '../../core/services/purchase-bill-api.service';
import { LocationBrief } from '../../core/models/auth.models';
import {
  ALLOWED_ITEMS,
  BillItemDraft,
  computeItemTotals,
  computeMarginPercent,
  PurchaseBillRequest,
  PurchaseBillResponse,
} from '../../core/models/purchase-bill.models';
import { AutocompleteComponent } from '../../shared/components/autocomplete/autocomplete.component';
import { BillItemTableComponent } from './components/bill-item-table.component';
import { BillSummaryComponent } from './components/bill-summary.component';

@Component({
  selector: 'app-purchase-bill',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    AutocompleteComponent,
    BillItemTableComponent,
    BillSummaryComponent,
  ],
  templateUrl: './purchase-bill.component.html',
})
export class PurchaseBillComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(PurchaseBillApiService);
  protected readonly auth = inject(AuthService);
  protected readonly loading = inject(LoadingService);

  protected readonly itemOptions = [...ALLOWED_ITEMS];
  protected readonly locations = signal<LocationBrief[]>([]);
  protected readonly items = signal<BillItemDraft[]>([]);
  protected readonly loadError = signal<string | null>(null);
  protected readonly submitError = signal<string | null>(null);
  protected readonly success = signal<PurchaseBillResponse | null>(null);
  protected readonly itemSubmitAttempted = signal(false);

  protected readonly totalItems = computed(() => this.items().length);
  protected readonly totalQuantity = computed(() =>
    this.items().reduce((sum, i) => sum + i.quantity, 0),
  );
  protected readonly totalCost = computed(() =>
    this.items().reduce((sum, i) => sum + i.totalCost, 0),
  );
  protected readonly totalSelling = computed(() =>
    this.items().reduce((sum, i) => sum + i.totalSelling, 0),
  );
  protected readonly preview = computed(() => {
    const v = this.itemValues();
    return computeItemTotals(
      Number(v.standardCost) || 0,
      Number(v.standardPrice) || 0,
      Number(v.quantity) || 0,
      Number(v.discountPercent) || 0,
    );
  });
  protected readonly marginPercent = computed(() => {
    const v = this.itemValues();
    return computeMarginPercent(Number(v.standardCost) || 0, Number(v.standardPrice) || 0);
  });

  protected readonly itemForm = this.fb.nonNullable.group({
    itemName: ['', Validators.required],
    batchLocationName: ['', Validators.required],
    standardCost: [0, [Validators.required, Validators.min(0)]],
    standardPrice: [0, [Validators.required, Validators.min(0)]],
    freeQuantity: [0, [Validators.required, Validators.min(0)]],
    quantity: [1, [Validators.required, Validators.min(0.01)]],
    discountPercent: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
  });

  private readonly itemValues = toSignal(this.itemForm.valueChanges, {
    initialValue: this.itemForm.getRawValue(),
  });
  protected readonly isSubmitting = signal(false);

  ngOnInit(): void {
    this.api
      .getLocations()
      .pipe(finalize(() => undefined))
      .subscribe({
        next: (locations) => {
          this.locations.set(locations);
          this.loadError.set(null);
        },
        error: () => this.loadError.set('Could not load locations. Please refresh the page.'),
      });
  }

  protected onItemNameChange(next: string): void {
    this.itemForm.controls.itemName.setValue(next);
    this.itemForm.controls.itemName.markAsTouched();
  }

  protected addItem(): void {
    this.itemSubmitAttempted.set(true);
    if (this.itemForm.invalid) {
      this.itemForm.markAllAsTouched();
      return;
    }
    const v = this.itemForm.getRawValue();
    const itemName = v.itemName.trim();
    if (!this.itemOptions.some((o) => o.toLowerCase() === itemName.toLowerCase())) {
      this.itemForm.controls.itemName.setErrors({ notAllowed: true });
      return;
    }
    const { totalCost, totalSelling } = computeItemTotals(
      v.standardCost,
      v.standardPrice,
      v.quantity,
      v.discountPercent,
    );
    this.items.update((rows) => [
      ...rows,
      {
        ...v,
        itemName,
        margin: computeMarginPercent(v.standardCost, v.standardPrice),
        totalCost,
        totalSelling,
      },
    ]);
    this.submitError.set(null);
    this.success.set(null);
    this.itemSubmitAttempted.set(false);
    this.itemForm.reset({
      itemName: '',
      batchLocationName: v.batchLocationName,
      standardCost: 0,
      standardPrice: 0,
      freeQuantity: 0,
      quantity: 1,
      discountPercent: 0,
    });
  }

  protected removeItem(index: number): void {
    this.items.update((rows) => rows.filter((_, i) => i !== index));
  }

  protected submitBill(): void {
    if (this.isSubmitting()) return;
    this.submitError.set(null);
    this.success.set(null);
    if (this.items().length === 0) {
      this.submitError.set('Add at least one item before submitting the bill.');
      return;
    }
    const batchLocationName = this.itemForm.controls.batchLocationName.value;
    if (!batchLocationName) {
      this.itemForm.controls.batchLocationName.markAsTouched();
      return;
    }
    const payload: PurchaseBillRequest = {
      items: this.items().map((i) => ({
        itemName: i.itemName,
        batchLocationName: i.batchLocationName,
        standardCost: i.standardCost,
        standardPrice: i.standardPrice,
        quantity: i.quantity,
        discountPercent: i.discountPercent,
        freeQuantity: i.freeQuantity,
        margin: computeMarginPercent(i.standardCost, i.standardPrice),
      })),
    };
    console.log('Purchase bill final payload:', payload);
    this.isSubmitting.set(true);
    this.api
      .createBill(payload)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: (bill) => {
          this.success.set(bill);
          this.items.set([]);
        },
        error: (err: { error?: { message?: string } }) =>
          this.submitError.set(err?.error?.message ?? 'Could not save the bill. Please try again.'),
      });
  }

  protected logout(): void {
    this.auth.logout();
  }
}
