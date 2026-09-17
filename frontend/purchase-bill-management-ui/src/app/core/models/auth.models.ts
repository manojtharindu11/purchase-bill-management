export interface LoginRequest {
  username: string;
  password: string;
}

export interface LocationBrief {
  locationCode: string;
  locationName: string;
}

export interface LoginResponse {
  token: string;
  userCode: string;
  userDisplayName: string;
  email: string;
  locations: LocationBrief[];
}