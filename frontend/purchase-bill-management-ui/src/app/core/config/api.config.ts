/**
 * Base URL for API calls.
 *
 * Left empty during development so requests stay relative and go through the
 * Angular dev-server proxy (see proxy.conf.json). That keeps the browser
 * same-origin and avoids CORS entirely. Set this to the deployed API origin
 * for production builds.
 */
export const API_BASE_URL = '';

/**
 * Versioned API prefix. Kept in one place so a version bump cannot drift
 * between services (this previously caused 404s against /api/v1 endpoints).
 */
export const API_PREFIX = '/api/v1';
