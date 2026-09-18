import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoadingIndicatorComponent } from './shared/components/loading-indicator/loading-indicator.component';

@Component({
  imports: [RouterOutlet, LoadingIndicatorComponent],
  selector: 'app-root',
  templateUrl: './app.html',
})
export class App {}
