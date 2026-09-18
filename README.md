# Purchase Bill Management

A full-stack purchase bill management platform for authenticating users through an external POS API, synchronizing location data, and creating and managing purchase bills.

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-22-DD0031?logo=angular&logoColor=white)](https://angular.dev/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![Hosted on Render](https://img.shields.io/badge/Hosted%20on-Render-46E3B7?logo=render&logoColor=000)](https://render.com/)

## Overview

Purchase Bill Management provides a simple workflow for:

- Authenticating users through an external POS API
- Synchronizing and maintaining location data
- Creating purchase bills with multiple line items
- Persisting data with SQL Server and Entity Framework Core
- Running the complete stack locally with Docker Compose
- Deploying containerized services to Render with Azure SQL Database

## Live Demo

Visit the deployed application:

**[Open Purchase Bill Management](https://purchase-bill-management-frontend.onrender.com/)**

> The live service may take a short time to respond when running on a free hosting plan.

## Project Walkthrough

Click the preview below to watch the project walkthrough on YouTube.

[![Watch the Purchase Bill Management walkthrough](https://img.youtube.com/vi/v6XOkbO_4QU/maxresdefault.jpg)](https://youtu.be/v6XOkbO_4QU)

The original recording is also available in the repository: [Screen recording.mkv](Screen%20recording.mkv).

## Technology Stack

| Layer | Technology |
| --- | --- |
| Backend | ASP.NET Core .NET 8 Web API |
| Frontend | Angular 22 |
| Data access | Entity Framework Core |
| Database | Microsoft SQL Server / Azure SQL Database |
| Local orchestration | Docker Compose |
| Web server | Nginx |
| Hosting | Render |
| Containerization | Docker |

## Repository Structure

```text
backend/
└── PurchaseBillManagement.Api/       ASP.NET Core API and EF Core migrations

frontend/
└── purchase-bill-management-ui/     Angular application and Nginx image

database/                             SQL Server image and schema script
docker-compose.yml                    Local multi-container setup
.env.example                          Environment variable template
Screen recording.mkv                   Project walkthrough
```

## Prerequisites

For the Docker-based setup:

- Docker Desktop with Docker Compose
- An accessible SQL Server instance, if not using the included database container

For development outside Docker:

- .NET SDK 8
- Node.js and npm
- An accessible SQL Server instance

## Quick Start with Docker Compose

### 1. Configure the environment

Copy the environment template:

```powershell
Copy-Item .env.example .env
```

Update `.env` with secure values. At minimum, configure a strong SQL Server password and JWT signing key:

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

### 2. Start the application

```powershell
docker compose up --build
```

### 3. Open the services

| Service | URL |
| --- | --- |
| Frontend | http://localhost:4200 |
| API | http://localhost:5104 |
| SQL Server | `localhost,1433` |

The `database-init` service creates the `PurchaseBillManagement` database and applies `database/purchase_bill_management.sql` when the current EF migration is not present. SQL Server data is persisted in the Docker volume created by Compose.

### Stop the application

```powershell
docker compose down
```

To remove the database volume and delete local database data:

```powershell
docker compose down -v
```

## Local Development Without Docker Compose

### Backend

```powershell
dotnet restore backend/PurchaseBillManagement.Api/PurchaseBillManagement.Api.csproj
dotnet run --project backend/PurchaseBillManagement.Api/PurchaseBillManagement.Api.csproj
```

The API normally listens on `http://localhost:5104`, according to the project launch settings.

### Frontend

```powershell
Set-Location frontend/purchase-bill-management-ui
npm ci
npm start
```

The Angular development server runs at `http://localhost:4200` and uses `proxy.conf.json` to forward `/api` requests to the local API.

## Configuration

### API environment variables

ASP.NET Core configuration uses the double-underscore format when values are supplied through Render or Docker:

| Variable | Description |
| --- | --- |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string |
| `Jwt__Key` | JWT signing key |
| `Jwt__Issuer` | JWT issuer |
| `Jwt__Audience` | JWT audience |
| `Jwt__ExpiryMinutes` | Token lifetime in minutes |
| `ExternalApi__BaseUrl` | External POS API base URL |
| `Frontend__Url` | Allowed hosted frontend origin for CORS |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment name |
| `ASPNETCORE_HTTP_PORTS` | Container HTTP port, normally `8080` |

### Frontend container variables

| Variable | Description |
| --- | --- |
| `API_URL` | Nginx upstream API URL. Compose uses `http://api:8080`; Render uses the public API URL. |

The Angular source also contains an API origin in `frontend/purchase-bill-management-ui/src/app/core/config/api.config.ts`. Verify `API_BASE_URL` before creating a production frontend image so it matches the deployed API URL.

## Database Schema

The schema is defined in:

```text
database/purchase_bill_management.sql
```

The script creates:

- `Location_Details`
- `Purchase_Bills`
- `Purchase_Bill_Items`
- `__EFMigrationsHistory`

The current EF Core migration is:

```text
20260918131407_InitialMigration
```

To apply the script to an external SQL Server, connect to the `PurchaseBillManagement` database and execute the SQL file. Do not repeatedly run the script against an already initialized database unless you intentionally understand the resulting changes.

## API Endpoints

| Method | Endpoint | Authentication |
| --- | --- | --- |
| `POST` | `/api/v1/auth/login` | Public |
| `GET` | `/api/v1/locations` | Bearer token |
| `GET` | `/api/v1/purchasebills` | Bearer token |
| `POST` | `/api/v1/purchasebills` | Bearer token |

## Build Docker Images

### Backend image

```powershell
docker build -f backend/PurchaseBillManagement.Api/Dockerfile `
  -t purchase-bill-management-backend:local `
  backend/PurchaseBillManagement.Api
```

### Frontend image

```powershell
docker build -f frontend/purchase-bill-management-ui/Dockerfile `
  -t purchase-bill-management-frontend:local `
  frontend/purchase-bill-management-ui
```

The API image listens on port `8080`; the frontend image listens on port `80`.

## Deployment to Render

The GitHub Actions workflow publishes Docker images when the corresponding folders change on `main`:

```text
YOUR_DOCKERHUB_USERNAME/purchase-bill-management-backend:latest
YOUR_DOCKERHUB_USERNAME/purchase-bill-management-frontend:latest
YOUR_DOCKERHUB_USERNAME/purchase-bill-management-database:latest
```

Configure these repository secrets for Docker Hub publishing:

```text
DOCKERHUB_USERNAME
DOCKERHUB_TOKEN
```

### Deploy the API

Create a Render Web Service using the backend image:

```text
Image: YOUR_DOCKERHUB_USERNAME/purchase-bill-management-backend:latest
Port: 8080
```

Set the following environment variables:

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

After deployment, copy the API's public Render URL, for example:

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

Redeploy the frontend after changing `API_URL` so the container entrypoint can generate the correct Nginx configuration. The Nginx container forwards `/api/*` requests to the API and serves the Angular application.

### CORS

Set `Frontend__Url` to the exact frontend origin without a path or trailing slash:

```text
Frontend__Url=https://purchase-bill-management-frontend.onrender.com
```

The API also allows `http://localhost:4200` and `http://127.0.0.1:4200` for local development.

### Database hosting

Do not run SQL Server as a normal free Render Web Service. It is not an HTTP service and does not provide suitable persistent database storage for this use case. Use Azure SQL Database or another managed SQL Server provider instead.

## Azure SQL Setup

For Render free web services, use an Azure SQL public endpoint rather than a private endpoint.

1. Create an Azure SQL logical server and a database named `PurchaseBillManagement`.
2. Select a public network endpoint.
3. Use the `Proxy` connection policy for compatibility with port `1433`.
4. Configure Azure SQL firewall rules for the API service's outbound IP addresses, where available.
5. Run `database/purchase_bill_management.sql` in Azure Query Editor.
6. Verify the tables and migration history:

```sql
SELECT name
FROM sys.tables
WHERE name IN ('Location_Details', 'Purchase_Bills', 'Purchase_Bill_Items');

SELECT * FROM __EFMigrationsHistory;
```

Example connection string:

```text
Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=PurchaseBillManagement;User ID=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

Store the connection string only in the Render API environment variables. Never commit credentials to Git.

## Troubleshooting

### Login works, but locations return `503`

Login authenticates through the external POS API and may still succeed if database synchronization fails. Check:

1. Render API logs using the response `traceId`.
2. The spelling and value of `ConnectionStrings__DefaultConnection`.
3. Azure SQL firewall rules and public network access.
4. The database name: `PurchaseBillManagement`.
5. The schema script and `20260918131407_InitialMigration` entry.
6. That the API uses port `1433` and the Azure SQL hostname rather than `localhost`.

### Frontend returns `502`

Check:

1. `API_URL` points to the backend Render URL.
2. The API service is running on port `8080`.
3. The frontend image was redeployed after changing `API_URL`.
4. `API_BASE_URL` is not an old or incorrect hostname.

### API returns `401`

Ensure the API and frontend use the same JWT settings:

```text
Jwt__Key
Jwt__Issuer
Jwt__Audience
```

After changing JWT settings, log in again to obtain a new token.

### Error responses and logs

API errors include a `traceId`, which is also returned in the `X-Trace-Id` response header. Use this value to locate the corresponding exception in the Render API logs. Never include database credentials or secret values in logs, screenshots, or issue reports.

## Security Guidelines

- Never commit `.env` files or production secrets.
- Rotate credentials that may have been exposed during development or troubleshooting.
- Use a strong, randomly generated JWT key and SQL Server password.
- Restrict Azure SQL firewall access as much as your hosting provider permits.
- Avoid exposing SQL Server port `1433` publicly in production unless it is required.

## License

No license has been specified for this repository yet. Add a license before distributing or reusing the project publicly.
