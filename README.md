# Cortado

Core API Build Status: ![Build Status](https://github.com/mhunt429/Cortado/actions/workflows/core-api.yml/badge.svg)
[![codecov](https://codecov.io/gh/mhunt429/Cortado/branch/main/graph/badge.svg)](https://codecov.io/gh/mhunt429/Cortado)

Cortado is a financial management application with a .NET Core API backend and Angular frontend.

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download) (version compatible with the project)
- [Docker](https://www.docker.com/get-started) and Docker Compose
- [Node.js](https://nodejs.org/) and npm (for Angular frontend)
- [Flyway](https://flywaydb.org/documentation/usage/commandline/) (for database migrations)
- [Angular CLI](https://angular.dev/tools/cli) (for frontend development)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd Cortado
```

### 2. Start Infrastructure Services

Start PostgreSQL and RabbitMQ using Docker Compose:

```bash
docker compose up -d
```

This will start:

- **PostgreSQL** on port `55432` (main database)
- **PostgreSQL** on port `65432` (test database)
- **RabbitMQ** on port `5672` (management UI on port `15672`)

### 3. Run Database Migrations

Execute the Flyway migrations to set up the database schema:

```bash
./scripts/run-flyway.sh
```

Or manually run Flyway:

```bash
flyway -url=jdbc:postgresql://localhost:55432/postgres -user=postgres -password=postgres -locations=filesystem:database/Migrations migrate
```

### 4. Configure Backend API

Navigate to the WebApi project and configure `appsettings.development.json`:

```bash
cd core.api/src/WebApi
```

Create or update `appsettings.development.json` with the following configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=55432;Database=postgres;User ID=postgres;Password=postgres"
  },
  "AwsConfig": {
    "ClientId": "your-aws-client-id",
    "ClientSecret": "your-aws-client-secret",
    "Region": "us-east-1"
  },
  "PlaidConfig": {
    "ClientId": "your-plaid-client-id",
    "ClientSecret": "your-plaid-client-secret",
    "BaseUrl": "https://sandbox.plaid.com",
    "SandboxAccessToken": "your-plaid-sandbox-access-token"
  },
  "SecurityConfig": {
    "JwtSecret": "your-jwt-secret-key-minimum-64-characters-long-for-security",
    "HashIdSalt": "your-hash-id-salt-key",
    "CryptoSecret": "your-crypto-secret-key",
    "AccessTokenExpiryMinutes": 30,
    "RefreshTokenExpiryDays": 7
  },
  "TwilioConfig": {
    "AccountSid": "your-twilio-account-sid",
    "AuthToken": "your-twilio-auth-token",
    "FromNumber": "+1234567890"
  },
  "CortadoDbConnectionString": "Host=localhost;Port=55432;Database=postgres;User ID=postgres;Password=postgres",
  "RabbitMQSettings": {
    "Username": "guest",
    "Password": "guest",
    "UseSsl": false,
    "host": "localhost"
  },
  "QuartzConfig": {
    "AccountBalanceRefreshJobCronExpression": "0 */5 * * * ?"
  }
}
```

#### Configuration Details

- **ConnectionStrings.DefaultConnection**: PostgreSQL connection string for Entity Framework (if used)
- **CortadoDbConnectionString**: Primary database connection string used by the application
- **AwsConfig**: AWS credentials for AWS services integration
- **PlaidConfig**: Plaid API credentials for financial account connections
  - Use `https://sandbox.plaid.com` for development/testing
  - Use `https://production.plaid.com` for production
- **SecurityConfig**: JWT and encryption settings
  - **JwtSecret**: Base64-encoded secret key for JWT token signing (minimum 64 characters recommended)
  - **HashIdSalt**: Salt for hashing user IDs
  - **CryptoSecret**: Secret key for encryption operations
  - **AccessTokenExpiryMinutes**: JWT access token expiration time
  - **RefreshTokenExpiryDays**: Refresh token expiration time
- **TwilioConfig**: Twilio SMS service configuration (optional)
- **RabbitMQSettings**: Message queue configuration
  - Default credentials are `guest/guest` for local development
- **QuartzConfig**: Scheduled job configuration
  - **AccountBalanceRefreshJobCronExpression**: Cron expression for account balance refresh job

#### Generating Security Keys

You need to generate secure keys for the `SecurityConfig` section. Use Node.js to generate cryptographically secure random keys:

**1. Generate AES Key (CryptoSecret) - 32 bytes:**

This key is used for encryption operations. Generate it with:

```bash
node -e "console.log(require('crypto').randomBytes(32).toString('base64'))"
```

Copy the output and use it as the value for `SecurityConfig.CryptoSecret`.

**2. Generate JWT Signing Key (JwtSecret) - 64 bytes:**

This key is used for signing JWT tokens. Generate it with:

```bash
node -e "console.log(require('crypto').randomBytes(64).toString('base64'))"
```

Copy the output and use it as the value for `SecurityConfig.JwtSecret`.

**Example output:**

- AES Key (32 bytes): `VThScBiMoqHx1GoIiDfCLxBIwDEHkPFDib/MWYEvBWw=`
- JWT Key (64 bytes): `TzQ+fsT13LSQ8oc6WQjHqiixB9wRYl8YrnECfoosORi7amkMD4z0eVf64eleZtiD5KBLelwenj6b94VOQCsjzA==`

> **Important**: Never commit these keys to version control. Each environment (development, staging, production) should have unique keys.

> **Note**: The application also supports loading configuration from a `.env` file in the parent directory (as referenced in `Program.cs`). You can create a `.env` file at the repository root if you prefer environment variable-based configuration.

### 5. Run the Backend API

From the `core.api/src/WebApi` directory:

```bash
dotnet run
```

Or from the solution root:

```bash
cd core.api
dotnet run --project src/WebApi/WebApi.csproj
```

The API will be available at `https://localhost:5001` (or the port configured in `launchSettings.json`).

In development mode, you can access:

- Swagger UI: `https://localhost:5001/swagger`
- Scalar API Reference: `https://localhost:5001/scalar/v1`
- Health Check: `https://localhost:5001/hc`

### 6. Set Up and Run the Frontend

Navigate to the Angular project:

```bash
cd cortado-angular
```

Install dependencies:

```bash
npm install
```

Start the development server:

```bash
ng serve
```

The Angular application will be available at `http://localhost:4200/`.

## Project Structure

```
Cortado/
├── core.api/              # .NET Core API solution
│   ├── src/
│   │   ├── Application/   # Application layer (services, validators)
│   │   ├── Domain/        # Domain models and contracts
│   │   ├── Infrastructure/# Infrastructure layer (repositories, services)
│   │   └── WebApi/        # API controllers and configuration
│   └── test/              # Unit and integration tests
├── database/
│   └── Migrations/        # Flyway database migrations
├── Cortado-angular/         # Angular frontend application
├── Cortado.Aspire/          # .NET Aspire orchestration (if used)
├── scripts/               # Utility scripts
└── compose.yaml           # Docker Compose configuration
```

## Development

### Running Tests

Run unit tests:

```bash
cd core.api
dotnet test
```

### Database Migrations

When creating new migrations, add them to `database/Migrations/` following the Flyway naming convention:

- `V{version}__{description}.sql`

Then run the migration script:

```bash
./scripts/run-flyway.sh
```

### RabbitMQ Management

Access the RabbitMQ management UI at `http://localhost:15672` with credentials:

- Username: `guest`
- Password: `guest`

## Additional Resources

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Angular Documentation](https://angular.dev)
- [Flyway Documentation](https://flywaydb.org/documentation/)
- [Plaid API Documentation](https://plaid.com/docs/)
