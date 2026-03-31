# Verd

A plant care companion app with AI chat, real-time weather integration, and personalized care recommendations.

**Stack:** Vue 3 + TypeScript (frontend) · ASP.NET Core 9 (backend) · PostgreSQL · JWT auth

---

## Prerequisites

| Tool | Version | Install |
|---|---|---|
| Node.js | ^20.19 or ≥22.12 | [nodejs.org](https://nodejs.org) |
| .NET SDK | 9.x | `brew install --cask dotnet-sdk` |
| Docker | any | [docker.com](https://docker.com) |

---

## Quick Start

### 1. Start the database

```bash
docker compose up -d
```

Starts PostgreSQL on `localhost:5432` with database `verd`.

### 2. Start the backend

```bash
cd Verd.Api
dotnet restore

# First time only — creates and applies DB migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

dotnet run
```

API runs at **http://localhost:5000**
Swagger UI at **http://localhost:5000/swagger**

### 3. Start the frontend

```bash
# From repo root
npm install
npm run dev
```

Frontend runs at **http://localhost:5173**

---

## Environment

The frontend reads `VITE_API_URL` from `.env.development` (already set to `http://localhost:5000`). Change it if your API runs on a different port.

To override the database connection or JWT secret, edit `Verd.Api/appsettings.json`. Never commit production secrets — use `Verd.Api/appsettings.Production.json` (gitignored).

---

## Common Commands

### Frontend

```bash
npm run dev          # Dev server with HMR
npm run build        # Type-check + production build
npm run type-check   # TypeScript check only
npm run lint         # Fix lint issues (oxlint + eslint)
npm run format       # Format with Prettier
```

### Backend

```bash
cd Verd.Api
dotnet run                              # Start API
dotnet watch                            # Start with hot reload
dotnet ef migrations add <Name>         # New migration
dotnet ef database update               # Apply migrations
dotnet ef migrations remove             # Remove last migration
```

### Database

```bash
docker compose up -d      # Start Postgres
docker compose down       # Stop Postgres
docker compose down -v    # Stop and delete data
```

---

## API Endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | — | Create account |
| POST | `/api/auth/login` | — | Sign in, returns JWT |
| GET | `/api/users/profile` | ✓ | Get current user |
| PATCH | `/api/users/settings` | ✓ | Update name / location / alerts |
| GET | `/api/plants` | ✓ | List your plants |
| POST | `/api/plants` | ✓ | Add a plant |
| PUT | `/api/plants/:id` | ✓ | Update a plant |
| DELETE | `/api/plants/:id` | ✓ | Delete a plant |
| GET | `/api/weather` | ✓ | Current weather + forecast |

Authenticated routes require `Authorization: Bearer <token>` header.

---

## Demo Mode

If the backend isn't running, the register/login pages will offer **Continue in demo mode** — this bypasses auth and lets you explore the app with mock data.

---

## Project Structure

```
Verd/
├── src/                  # Vue 3 frontend
│   ├── views/            # Page components
│   ├── components/       # UI components
│   ├── stores/           # Pinia state (user, weather, plants)
│   └── router/           # Vue Router + auth guards
├── Verd.Api/             # ASP.NET Core 9 backend
│   ├── Controllers/      # API controllers
│   ├── Models/           # EF Core entities
│   ├── DTOs/             # Request/response types
│   ├── Services/         # JwtService
│   └── Data/             # AppDbContext
├── docker-compose.yml    # PostgreSQL
└── Verd.sln              # .NET solution
```
