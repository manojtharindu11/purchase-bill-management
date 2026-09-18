import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { vi } from 'vitest';
import { PurchaseBillComponent } from './purchase-bill.component';
import { PurchaseBillApiService } from '../../core/services/purchase-bill-api.service';
import { AuthService } from '../../core/services/auth.service';
import { PurchaseBillResponse } from '../../core/models/purchase-bill.models';

describe('PurchaseBillComponent', () => {
  let fixture: ComponentFixture<PurchaseBillComponent>;
  let element: HTMLElement;
  let response: Subject<PurchaseBillResponse>;
  const api = {
    getLocations: vi.fn(),
    createBill: vi.fn(),
  };

  beforeEach(async () => {
    response = new Subject<PurchaseBillResponse>();
    api.getLocations
      .mockReset()
      .mockReturnValue(of([{ locationCode: 'LOC-1', locationName: 'Head Office' }]));
    api.createBill.mockReset().mockReturnValue(response);
    await TestBed.configureTestingModule({
      imports: [PurchaseBillComponent],
      providers: [
        { provide: PurchaseBillApiService, useValue: api },
        { provide: AuthService, useValue: { currentUser: () => null, logout: vi.fn() } },
      ],
    }).compileComponents();
    fixture = TestBed.createComponent(PurchaseBillComponent);
    element = fixture.nativeElement;
    fixture.detectChanges();
  });

  function enter(selector: string, value: string): void {
    const input = element.querySelector<HTMLInputElement>(selector)!;
    input.value = value;
    input.dispatchEvent(new Event('input', { bubbles: true }));
    fixture.detectChanges();
  }

  function click(label: string): void {
    const button = Array.from(element.querySelectorAll('button')).find(
      (b) => b.textContent?.trim() === label,
    )!;
    button.click();
    fixture.detectChanges();
  }

  function addMango(): void {
    const batch = element.querySelector<HTMLSelectElement>('#batch')!;
    batch.value = batch.options[1].value;
    batch.dispatchEvent(new Event('change', { bubbles: true }));
    enter('app-autocomplete input', 'Mango');
    enter('#standardCost', '100');
    enter('#standardPrice', '150');
    enter('#quantity', '5');
    enter('#discountPercent', '20');
    click('Add +');
  }

  it('updates the preview when form values change', () => {
    enter('#standardCost', '100');
    enter('#standardPrice', '150');
    enter('#quantity', '5');
    enter('#discountPercent', '20');
    expect(element.querySelector<HTMLInputElement>('[data-testid="preview-cost"]')?.value).toBe(
      '400.00',
    );
    expect(element.querySelector<HTMLInputElement>('[data-testid="preview-selling"]')?.value).toBe(
      '750.00',
    );
    enter('#quantity', '2');
    expect(element.querySelector<HTMLInputElement>('[data-testid="preview-cost"]')?.value).toBe(
      '160.00',
    );
  });

  it('adds and removes rows and updates the summary', () => {
    addMango();
    expect(element.querySelector('tbody')?.textContent).toContain('Mango');
    const totals = Array.from(element.querySelectorAll('app-bill-summary strong')).map((node) =>
      node.textContent?.trim(),
    );
    expect(totals).toEqual(['1', '5', '400.00', '750.00']);
    click('Remove');
    expect(element.querySelector('tbody')?.textContent).toContain('No items yet');
  });

  it('submits inputs without trusting client totals and prevents duplicate clicks', () => {
    addMango();
    click('Save Purchase Bill');
    expect(api.createBill).toHaveBeenCalledExactlyOnceWith({
      items: [
        {
          itemName: 'Mango',
          batchLocationName: 'Head Office',
          standardCost: 100,
          standardPrice: 150,
          quantity: 5,
          discountPercent: 20,
          freeQuantity: 0,
          margin: 33.33333333333333,
        },
      ],
    });
    const saving = Array.from(element.querySelectorAll('button')).find((b) =>
      b.textContent?.includes('Saving'),
    )!;
    expect(saving.disabled).toBe(true);
    saving.click();
    expect(api.createBill).toHaveBeenCalledTimes(1);
    response.next({
      id: 1,
      billNumber: 'PB-TEST',
      totalItems: 1,
      totalQuantity: 5,
      totalCost: 400,
      totalSelling: 750,
      createdAt: '2026-09-17T00:00:00Z',
      items: [],
    });
    response.complete();
    fixture.detectChanges();
    expect(element.querySelector('output')?.textContent).toContain('PB-TEST');
    expect(element.querySelector('tbody')?.textContent).toContain('No items yet');
  });

  it('keeps the draft and displays an error when saving fails', () => {
    addMango();
    const batch = element.querySelector<HTMLSelectElement>('#batch')!;
    batch.value = batch.options[1].value;
    batch.dispatchEvent(new Event('change', { bubbles: true }));
    fixture.detectChanges();
    click('Save Purchase Bill');
    response.error({ error: { message: 'Unable to save.' } });
    fixture.detectChanges();
    expect(element.querySelector('[role="alert"]')?.textContent).toContain('Unable to save.');
    expect(element.querySelector('tbody')?.textContent).toContain('Mango');
  });
});
