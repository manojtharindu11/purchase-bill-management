/**
 * Base URL for API calls.
 *
 * Deployed API origin. Requests use the versioned prefix below.
 */
export const API_BASE_URL = 'https://purchase-bill-management-backend.onrender.com';

/**
 * Versioned API prefix. Kept in one place so a version bump cannot drift
 * between services (this previously caused 404s against /api/v1 endpoints).
 */
export const API_PREFIX = '/api/v1';
