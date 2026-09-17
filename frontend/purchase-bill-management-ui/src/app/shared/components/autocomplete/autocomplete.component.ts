import { Component, computed, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

/**
 * Reusable autocomplete input (filters options as the user types).
 * Usage: <app-autocomplete [options]="items" [(value)]="selected" placeholder="..." />
 */
@Component({
  selector: 'app-autocomplete',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="relative">
      <input
        type="text"
        [placeholder]="placeholder()"
        [ngModel]="value()"
        (ngModelChange)="onInput($event)"
        (focus)="open.set(true)"
        (blur)="onBlur()"
        class="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
      />
      @if (open() && filtered().length > 0) {
        <ul
          class="absolute z-10 mt-1 max-h-48 w-full overflow-auto rounded-lg border border-slate-200 bg-white py-1 shadow-lg"
        >
          @for (option of filtered(); track option) {
            <li>
              <button
                type="button"
                (mousedown)="select(option)"
                class="block w-full px-3 py-2 text-left text-sm text-slate-700 hover:bg-blue-50"
              >
                {{ option }}
              </button>
            </li>
          }
        </ul>
      }
    </div>
  `,
})
export class AutocompleteComponent {
  options = input<string[]>([]);
  value = input<string>('');
  valueChange = output<string>();
  placeholder = input<string>('Start typing…');

  protected readonly open = signal(false);

  protected readonly filtered = computed(() => {
    const query = this.value().trim().toLowerCase();
    const all = this.options();
    if (!query) return all;
    return all.filter((o) => o.toLowerCase().includes(query));
  });

  protected onInput(next: string): void {
    this.valueChange.emit(next);
    this.open.set(true);
  }

  protected onBlur(): void {
    // Delay close so the (mousedown) on an option wins over blur.
    setTimeout(() => this.open.set(false), 120);
  }

  protected select(option: string): void {
    this.valueChange.emit(option);
    this.open.set(false);
  }
}
