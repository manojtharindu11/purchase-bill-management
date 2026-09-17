import { computed, Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private readonly _pending = signal(0);

  readonly isLoading = computed(() => this._pending() > 0);

  show(): void {
    this._pending.update((count) => count + 1);
  }

  hide(): void {
    this._pending.update((count) => Math.max(0, count - 1));
  }
}
