import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_BASE_URL, API_PREFIX } from '../config/api.config';
import { LocationBrief } from '../models/auth.models';
import {
  PurchaseBillRequest,
  PurchaseBillResponse,
} from '../models/purchase-bill.models';

@Injectable({ providedIn: 'root' })
export class PurchaseBillApiService {
  private readonly http = inject(HttpClient);

  getLocations() {
    return this.http.get<LocationBrief[]>(`${API_BASE_URL}${API_PREFIX}/locations`);
  }

  createBill(request: PurchaseBillRequest) {
    return this.http.post<PurchaseBillResponse>(
      `${API_BASE_URL}${API_PREFIX}/purchasebills`,
      request
    );
  }
}
