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
| **Web Application** | [http://localhost](http://localhost) | Port `80` maps to the internal port `8080` of the container. |
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

### Database (`db`)

| Parameter | Configuration Details |
| --- | --- |
| `ORACLE_PWD` | `system_password` *(Sets the password for the administrative users: SYS, SYSTEM, and PDBADMIN)* |
| **Volume Mapping** | `oracle-data` is mapped to `/opt/oracle/oradata` to ensure database persistence across container restarts. |

---

## Docker Deployment Details

### Multi-Stage Dockerfile

The application utilizes a multi-stage build process to guarantee that the final production image remains lightweight and secure:

> 🛠️ **Build Stage:** Uses `mcr.microsoft.com/dotnet/sdk:10.0` to restore dependencies, build the solution, and publish the release binaries.
> 🚀 **Final Stage:** Uses `mcr.microsoft.com/dotnet/aspnet:10.0` as the runtime environment, copying only the published output from the build stage to minimize image size.
