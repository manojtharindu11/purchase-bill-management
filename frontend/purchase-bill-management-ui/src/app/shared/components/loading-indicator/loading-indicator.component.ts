import { Component, inject } from '@angular/core';
import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-loading-indicator',
  standalone: true,
  template: `
    @if (loading.isLoading()) {
      <div
        class="fixed inset-x-0 top-0 z-50 h-1 bg-blue-100"
        role="progressbar"
        aria-label="Loading"
        aria-valuetext="Loading"
      >
        <div class="h-full w-1/3 animate-pulse bg-blue-600"></div>
      </div>
    }
  `,
})
export class LoadingIndicatorComponent {
  protected readonly loading = inject(LoadingService);
}
