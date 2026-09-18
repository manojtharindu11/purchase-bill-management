# Purchase Bill Management

Purchase Bill Management is a full-stack application for authenticating users through an external POS API, maintaining location data, and creating purchase bills.

## Stack

- Backend: ASP.NET Core .NET 8 Web API
- Frontend: Angular 22
- Database: Microsoft SQL Server with Entity Framework Core
- Local orchestration: Docker Compose
- Production hosting: Docker images on Render with Azure SQL Database

## Repository Layout

```text
backend/PurchaseBillManagement.Api/   ASP.NET Core API and EF Core migrations
frontend/purchase-bill-management-ui/ Angular application and Nginx image
database/                             SQL Server image and schema script
docker-compose.yml                    Local multi-container setup
.env.example                          Environment variable template
Screen recording.mkv                   Project walkthrough
```

## Walkthrough Video

[![Watch the Purchase Bill Management walkthrough](https://img.youtube.com/vi/v6XOkbO_4QU/maxresdefault.jpg)](https://youtu.be/v6XOkbO_4QU)

Click the preview to watch the walkthrough on YouTube. The original MKV recording is also available as [Screen recording.mkv](Screen%20recording.mkv).

## Prerequisites

- Docker Desktop with Docker Compose
- .NET SDK 8, only required for local API development outside Docker
- Node.js and npm, only required for local frontend development outside Docker
- An accessible SQL Server instance

## Local Setup With Docker Compose

1. Copy the environment template:

   ```powershell
   Copy-Item .env.example .env
   ```

2. Edit `.env` and replace all placeholder values. At minimum, set a strong SQL Server password and JWT key:

   ```dotenv
   MSSQL_SA_PASSWORD=replace-with-a-strong-password
   JWT_KEY=replace-with-a-long-random-signing-key
   JWT_ISSUER=PurchaseBillManagement.Api
   JWT_AUDIENCE=PurchaseBillManagement.UI
   JWT_EXPIRY_MINUTES=120
   ASPNETCORE_ENVIRONMENT=Production
   EXTERNAL_API_BASE_URL=https://ez-staging-api.azurewebsites.net
   FRONTEND_URL=http://localhost:4200
   API_URL=http://api:8080
   ```

3. Start the stack:

   ```powershell
   docker compose up --build
   ```

4. Open the application:
   - Frontend: http://localhost:4200
   - API: http://localhost:5104
   - SQL Server: localhost,1433

The `database-init` service creates the `PurchaseBillManagement` database and applies `database/purchase_bill_management.sql` when the current EF migration is not present. SQL Server data is stored in the named Docker volume `purchase_bill_sql_data`.

Stop the stack:

```powershell
docker compose down
```

Remove the database volume only when you intentionally want to delete local data:

```powershell
docker compose down -v
```

## Local Development Without Compose

### Backend

```powershell
dotnet restore backend/PurchaseBillManagement.Api/PurchaseBillManagement.Api.csproj
dotnet run --project backend/PurchaseBillManagement.Api/PurchaseBillManagement.Api.csproj
```

The API listens on the URL configured by the project launch settings, normally `http://localhost:5104`.

### Frontend

```powershell
Set-Location frontend/purchase-bill-management-ui
npm ci
npm start
```

The Angular development server runs at http://localhost:4200 and uses `proxy.conf.json` to forward `/api` requests to the local API.

## Environment Variables

### API

The ASP.NET Core configuration uses the double-underscore form when values are supplied through Render or Docker:

| Variable                               | Purpose                                 |
| -------------------------------------- | --------------------------------------- |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string            |
| `Jwt__Key`                             | JWT signing key                         |
| `Jwt__Issuer`                          | JWT issuer                              |
| `Jwt__Audience`                        | JWT audience                            |
| `Jwt__ExpiryMinutes`                   | Token lifetime in minutes               |
| `ExternalApi__BaseUrl`                 | External POS API base URL               |
| `Frontend__Url`                        | Allowed hosted frontend origin for CORS |
| `ASPNETCORE_ENVIRONMENT`               | ASP.NET Core environment name           |
| `ASPNETCORE_HTTP_PORTS`                | Container HTTP port, normally `8080`    |

### Frontend container

| Variable  | Purpose                                                                                 |
| --------- | --------------------------------------------------------------------------------------- |
| `API_URL` | Nginx upstream API URL. Compose uses `http://api:8080`; Render uses the public API URL. |

The current Angular source also contains a compiled API origin in `frontend/purchase-bill-management-ui/src/app/core/config/api.config.ts`. Before building a production frontend image, set `API_BASE_URL` to the deployed API origin, or change it to an empty string if all requests should use the Nginx `/api` proxy.

## Database Schema

The current schema is defined by:

```text
database/purchase_bill_management.sql
```

It creates:

- `Location_Details`
- `Purchase_Bills`
- `Purchase_Bill_Items`
- `__EFMigrationsHistory`

The current EF migration is:

```text
20260918131407_InitialMigration
```

To apply the script to an external SQL Server, connect to the `PurchaseBillManagement` database and run the SQL file. Do not run the script repeatedly against an already initialized database unless you have reset the database first.

## Azure SQL Setup

For Render free web services, use an Azure SQL public endpoint rather than a private endpoint.

1. Create an Azure SQL logical server and database named `PurchaseBillManagement`.
2. Select a public network endpoint.
3. Use the `Proxy` connection policy for compatibility with port `1433`.
4. Configure Azure SQL firewall rules for the API service's outbound IP addresses, where available.
5. Run `database/purchase_bill_management.sql` in Azure Query Editor.
6. Verify the tables and migration:

   ```sql
   SELECT name
   FROM sys.tables
   WHERE name IN ('Location_Details', 'Purchase_Bills', 'Purchase_Bill_Items');

   SELECT * FROM __EFMigrationsHistory;
   ```

Use a connection string similar to:

```text
Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=PurchaseBillManagement;User ID=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

Store this only in the Render API environment variables. Never commit credentials to Git.

## Building Docker Images

Build the API image:

```powershell
docker build -f backend/PurchaseBillManagement.Api/Dockerfile `
  -t purchase-bill-management-backend:local `
  backend/PurchaseBillManagement.Api
```

Build the frontend image:

```powershell
docker build -f frontend/purchase-bill-management-ui/Dockerfile `
  -t purchase-bill-management-frontend:local `
  frontend/purchase-bill-management-ui
```

The API image listens on port `8080`. The frontend image listens on port `80`.

## Render Deployment

The GitHub Actions workflow publishes these Docker Hub images when the corresponding folders change on `main`:

```text
YOUR_DOCKERHUB_USERNAME/purchase-bill-management-backend:latest
YOUR_DOCKERHUB_USERNAME/purchase-bill-management-frontend:latest
YOUR_DOCKERHUB_USERNAME/purchase-bill-management-database:latest
```

Configure these GitHub repository secrets for image publishing:

```text
DOCKERHUB_USERNAME
DOCKERHUB_TOKEN
```

### Deploy the API

Create a Render Web Service using the backend image. Configure:

```text
Image: YOUR_DOCKERHUB_USERNAME/purchase-bill-management-backend:latest
Port: 8080
```

Set these environment variables:

```text
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_HTTP_PORTS=8080
ConnectionStrings__DefaultConnection=YOUR_AZURE_SQL_CONNECTION_STRING
Jwt__Key=YOUR_NEW_LONG_RANDOM_SECRET
Jwt__Issuer=PurchaseBillManagement.Api
Jwt__Audience=PurchaseBillManagement.UI
Jwt__ExpiryMinutes=120
ExternalApi__BaseUrl=https://ez-staging-api.azurewebsites.net
Frontend__Url=https://YOUR_FRONTEND_SERVICE.onrender.com
```

Copy the API's public Render URL after deployment, for example:

```text
https://purchase-bill-management-backend.onrender.com
```

### Deploy the frontend

Create a second Render Web Service using the frontend image:

```text
Image: YOUR_DOCKERHUB_USERNAME/purchase-bill-management-frontend:latest
Port: 80
API_URL=https://YOUR_BACKEND_SERVICE.onrender.com
```

After changing `API_URL`, trigger a new deployment so the container entrypoint generates the correct Nginx configuration. The Nginx container forwards `/api/*` to the API URL and serves Angular routes with an `index.html` fallback.

Because the current Angular source has `API_BASE_URL` set to a deployed API hostname, verify that it matches the actual Render API URL before publishing a new frontend image. A stale hostname causes login and location requests to fail even when the Nginx container is configured correctly.

### CORS

Set the API's `Frontend__Url` to the exact frontend origin, without a path or trailing slash:

```text
Frontend__Url=https://purchase-bill-management-frontend.onrender.com
```

The API also allows `http://localhost:4200` and `http://127.0.0.1:4200` for local development.

### Database hosting

Do not run SQL Server as a normal free Render Web Service. It has no suitable persistent database storage and is not an HTTP service. Use Azure SQL Database or another managed SQL Server provider. The database Docker image is intended for local Compose use and contains the schema script.

## API Endpoints

| Method | Endpoint                | Authentication |
| ------ | ----------------------- | -------------- |
| `POST` | `/api/v1/auth/login`    | Public         |
| `GET`  | `/api/v1/locations`     | Bearer token   |
| `GET`  | `/api/v1/purchasebills` | Bearer token   |
| `POST` | `/api/v1/purchasebills` | Bearer token   |

## Troubleshooting

### Login works but locations return 503

Login authenticates through the external POS API and may still succeed if the database synchronization fails. Check:

1. Render API logs using the response `traceId`.
2. `ConnectionStrings__DefaultConnection` spelling and value.
3. Azure SQL firewall rules and public network access.
4. Database name `PurchaseBillManagement`.
5. The schema script and `20260918131407_InitialMigration` entry.
6. That the API is using port `1433` and the Azure SQL hostname, not `localhost`.

### Frontend returns 502

Check:

1. The frontend `API_URL` points to the backend Render URL.
2. The API service is running and listens on port `8080`.
3. The frontend image was redeployed after changing `API_URL`.
4. `API_BASE_URL` in the Angular source is not an old or incorrect hostname.

### API returns 401

Check that the API and frontend use the same JWT settings:

```text
Jwt__Key
Jwt__Issuer
Jwt__Audience
```

After changing JWT settings, log in again to obtain a new token.

### Error responses and logs

API errors include a `traceId` and the same value is returned in the `X-Trace-Id` response header. Use that value to locate the full exception in the Render API logs. Database credentials and passwords are never returned to clients.

## Security

- Never commit `.env` or production secrets.
- Rotate any credentials that have been exposed during development or troubleshooting.
- Use a strong random JWT key and SQL password.
- Restrict Azure SQL firewall access as much as your hosting provider permits.
- Do not expose SQL Server port `1433` publicly in production unless required.
