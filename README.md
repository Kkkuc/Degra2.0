# Degra 2.0 — Deployment & Configuration Guide

---

## Getting Started

### 1. Clone the Repository

Run the following commands to clone the repository and navigate into the project directory:

```bash
git clone git@github.com:Kkkuc/Degra2.0.git
cd Degra2.0

```

### 2. Build and Run the Containers

To build the application images and start both the web application and the database services simultaneously, execute:

```bash
docker compose up --build

```

---

## Accessing the Application

Once the containers are up and running, you can connect to the services using the details below:

| Service | Access Link / Address | Notes |
| --- | --- | --- |
| **Web Application** | [http://localhost](https://www.google.com/search?q=http://localhost) | Port `80` maps to the internal port `8080` of the container. |
| **Oracle Database** | `localhost:1521` | Accessible externally for database tools/clients. |

---

## Stopping the Services

Depending on whether you want to preserve your data, choose one of the following options:

* **To stop containers (preserving data):**

```bash
docker compose down

```

* **To stop containers and delete all data volumes:**

```bash
docker compose down -v

```

---

## Configuration & Environment Variables

### Web Application (`web-app`)

| Environment Variable | Value / Description |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | `User Id=system;Password=system_password;Data Source=db:1521/FREEPDB1;` |
| `Authentication__Google__ClientId` | Google OAuth Client ID for production use. |
| `Authentication__Google__ClientSecret` | Google OAuth Client Secret for production use. |

### Database (`db`)

| Parameter | Configuration Details |
| --- | --- |
| `ORACLE_PWD` | `system_password` *(Sets the password for the administrative users: SYS, SYSTEM, and PDBADMIN)* |
| **Volume Mapping** | `oracle-data` is mapped to `/opt/oracle/oradata` to ensure database persistence across container restarts. |

---

## Sensitive Data Management (Google OAuth)

The safest way to manage sensitive data (such as API keys or Client Secrets) in ASP.NET Core is to use **User Secrets** in the local development environment and **Environment Variables** in production. This strategy ensures confidential information is never committed to the Git repository.

> ⚠️ **Security Notice:** Because the `ClientSecret` shown below was previously exposed, it must be rotated (regenerated) in the Google Cloud Console as soon as possible.

### Step 1: Update Application Code (`Program.cs`)

Replace hardcoded credential strings with values dynamically retrieved from the configuration provider:

```csharp
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

```

### Step 2: Local Configuration (User Secrets)

The User Secrets tool stores configuration JSON files outside of your project directory (inside your operating system user profile folder). This isolates the data entirely from `git status`.

Open a terminal in your project folder (where the `.csproj` file is located) and initialize the secrets provider:

```bash
dotnet user-secrets init

```

Save your credentials locally by running the following commands:

```bash
dotnet user-secrets set "Authentication:Google:ClientId" "<yourClientId>"
dotnet user-secrets set "Authentication:Google:ClientSecret" "<yourClientSecret>"

```

When running the application locally, ASP.NET Core automatically merges these values into the `IConfiguration` object, keeping the source code clean and safe to push to GitHub.

### Step 3: Production Configuration

When deploying the application to a production server (via Docker, Azure, or a Linux VPS), pass these values as environment variables. ASP.NET Core automatically maps double underscores (`__`) to hierarchical sections in the configuration provider tree.

Configure the following variables on your production environment:

* `Authentication__Google__ClientId`
* `Authentication__Google__ClientSecret`

---

## Docker Deployment Details

### Multi-Stage Dockerfile

The application utilizes a multi-stage build process to guarantee that the final production image remains lightweight and secure:

> 🛠️ **Build Stage:** Uses `mcr.microsoft.com/dotnet/sdk:10.0` to restore dependencies, build the solution, and publish the release binaries.
> 🚀 **Final Stage:** Uses `mcr.microsoft.com/dotnet/aspnet:10.0` as the runtime environment, copying only the published output from the build stage to minimize image size.
