# Veltrix Expense Management System

A web-based expense and payment tracking platform for managing organizational spending across teams. Built with ASP.NET Core MVC and .NET 8.

---

## Features

- **Authentication & Authorization** — Session-based login with role-based access control
- **Role System** — Two roles: Manager (`Gerente`) and Employee (`Empleado`), each with different budget multipliers and permissions
- **Three Payment Types:**
  - `Único` (One-time) — 10% discount; additional 10% discount when paid in cash
  - `Cuotas` (Installments) — 3–10% surcharge depending on number of months
  - `Suscripción` (Recurring/Subscription) — 3% monthly surcharge
- **Budget Management** — Personal and team budgets with status tracking (`Allowed`, `Close`, `Over`)
- **Dashboard & Analytics** — Monthly expense summaries with charts for the last 6 months
- **Team Management** — Managers can view all team members and team-level expenses
- **Expense Categories** — Managers can create and manage expense types (e.g., Software, Hardware, Training)
- **Payment Methods** — Credit card, debit card, and cash

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 8.0 |
| Web | ASP.NET Core MVC |
| Templating | Razor (`.cshtml`) |
| Frontend | HTML5, CSS3, JavaScript |
| Icons | Font Awesome 4.7.0 |
| Data | In-memory (no database) |
| Auth | Session-based (`HttpContext.Session`) |

---

## Project Structure

```
Veltrix-Expense-Management-System/
├── ClassLibrary/          # Domain logic and business entities
│   ├── Sistema.cs         # Central singleton — data store and business logic
│   ├── Usuarios/          # User, Team, and Role entities
│   └── Pagos/             # Payment types and budget validation
├── client/                # ASP.NET Core MVC web application
│   ├── Controllers/       # UsuarioController, PagoController, TipoDeGastoController
│   ├── Views/             # Razor templates
│   ├── Filters/           # Auth and authorization action filters
│   ├── DTO/               # Data Transfer Objects
│   └── Mappers/           # DTO <-> domain mappers
├── SistemaDePagos/        # Console app for testing
├── documentation/         # Project analysis and requirements (PDF)
└── Obligatorio01.sln      # Visual Studio solution file
```

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (recommended) or VS Code with the C# Dev Kit extension

---

## Getting Started

### Option A — Visual Studio (recommended)

1. Open `Obligatorio01.sln` in Visual Studio.
2. Right-click the solution → **Restore NuGet Packages**.
3. Set `client` (`webApp`) as the startup project.
4. Press **F5** to run.

### Option B — Command Line

```bash
dotnet restore
dotnet run --project client/webApp.csproj
```

Then open `https://localhost:<port>` in your browser.

### Console Application (for testing)

```bash
dotnet run --project SistemaDePagos/ConsoleApp.csproj
```

---

## Usage

The app starts at the login page (`/Usuario/Login`). After logging in, users are redirected to their personal expense dashboard (`/Pago/Index`).

The system is pre-loaded with sample data: 22 users across 8 teams, active subscriptions, and various payments. User emails follow the pattern `name.surname@veltrix.com`.

### Role-Based Access

| Feature | Employee | Manager |
|---|---|---|
| View & create personal expenses | Yes | Yes |
| View profile and team members | Yes | Yes |
| View team expense dashboard | — | Yes |
| Manage expense categories | — | Yes |

---

## Architecture Notes

- **Singleton Pattern** — `Sistema` uses lazy initialization as the central data repository
- **Abstract Base Class** — `Pago` is polymorphic; each payment type overrides `CalcTotal` and `DelMes`
- **Action Filters** — `UserLoggedFilter` and `UserHasAccessFilter` handle authentication and role-based authorization
- **IValidable Interface** — Consistent validation contract across domain entities

---

## Authors

Developed by **Bonora & Rossi** as an academic assignment for Universidad ORT Uruguay (Obligatorio 01, August 2025).

---
---

# Veltrix — Sistema de Gestión de Gastos

Una plataforma web para el seguimiento de gastos y pagos organizacionales por equipos. Desarrollada con ASP.NET Core MVC y .NET 8.

---

## Funcionalidades

- **Autenticación y Autorización** — Login con sesiones y control de acceso basado en roles
- **Sistema de Roles** — Dos roles: Gerente y Empleado, con distintos multiplicadores de presupuesto y permisos
- **Tres Tipos de Pago:**
  - `Único` — 10% de descuento; 10% adicional si se paga en efectivo
  - `Cuotas` — Recargo del 3% al 10% según la cantidad de meses
  - `Suscripción` (Recurrente) — Recargo mensual del 3%
- **Gestión de Presupuesto** — Presupuestos personales y de equipo con estados: `Permitido`, `Cercano`, `Superado`
- **Dashboard y Análisis** — Resumen mensual de gastos con gráficos de los últimos 6 meses
- **Gestión de Equipos** — Los gerentes pueden ver a todos los miembros del equipo y los gastos grupales
- **Categorías de Gasto** — Los gerentes pueden crear y administrar tipos de gasto (ej: Software, Hardware, Capacitación)
- **Métodos de Pago** — Tarjeta de crédito, débito y efectivo

---

## Tecnologías

| Capa | Tecnología |
|---|---|
| Framework | .NET 8.0 |
| Web | ASP.NET Core MVC |
| Templates | Razor (`.cshtml`) |
| Frontend | HTML5, CSS3, JavaScript |
| Íconos | Font Awesome 4.7.0 |
| Datos | En memoria (sin base de datos) |
| Autenticación | Basada en sesiones (`HttpContext.Session`) |

---

## Estructura del Proyecto

```
Veltrix-Expense-Management-System/
├── ClassLibrary/          # Lógica de dominio y entidades del negocio
│   ├── Sistema.cs         # Singleton central — almacén de datos y lógica
│   ├── Usuarios/          # Entidades de Usuario, Equipo y Rol
│   └── Pagos/             # Tipos de pago y validación de presupuesto
├── client/                # Aplicación web ASP.NET Core MVC
│   ├── Controllers/       # UsuarioController, PagoController, TipoDeGastoController
│   ├── Views/             # Templates Razor
│   ├── Filters/           # Filtros de autenticación y autorización
│   ├── DTO/               # Objetos de transferencia de datos
│   └── Mappers/           # Mapeadores DTO <-> dominio
├── SistemaDePagos/        # Aplicación de consola para pruebas
├── documentation/         # Análisis y requerimientos del proyecto (PDF)
└── Obligatorio01.sln      # Archivo de solución de Visual Studio
```

---

## Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (recomendado) o VS Code con la extensión C# Dev Kit

---

## Cómo Ejecutar

### Opción A — Visual Studio (recomendado)

1. Abrir `Obligatorio01.sln` en Visual Studio.
2. Clic derecho en la solución → **Restaurar paquetes NuGet**.
3. Establecer `client` (`webApp`) como proyecto de inicio.
4. Presionar **F5** para ejecutar.

### Opción B — Línea de Comandos

```bash
dotnet restore
dotnet run --project client/webApp.csproj
```

Luego abrir `https://localhost:<puerto>` en el navegador.

### Aplicación de Consola (para pruebas)

```bash
dotnet run --project SistemaDePagos/ConsoleApp.csproj
```

---

## Uso

La aplicación inicia en la pantalla de login (`/Usuario/Login`). Al iniciar sesión, el usuario es redirigido a su dashboard de gastos personales (`/Pago/Index`).

El sistema viene pre-cargado con datos de prueba: 22 usuarios distribuidos en 8 equipos, suscripciones activas y distintos pagos. Los correos siguen el patrón `nombre.apellido@veltrix.com`.

### Acceso por Rol

| Funcionalidad | Empleado | Gerente |
|---|---|---|
| Ver y crear gastos personales | Sí | Sí |
| Ver perfil y miembros del equipo | Sí | Sí |
| Ver dashboard de gastos del equipo | — | Sí |
| Administrar categorías de gasto | — | Sí |

---

## Notas de Arquitectura

- **Patrón Singleton** — `Sistema` usa inicialización lazy como repositorio central de datos
- **Clase Base Abstracta** — `Pago` es polimórfica; cada tipo de pago sobreescribe `CalcTotal` y `DelMes`
- **Filtros de Acción** — `UserLoggedFilter` y `UserHasAccessFilter` gestionan la autenticación y autorización por rol
- **Interfaz IValidable** — Contrato de validación consistente para las entidades del dominio

---

## Autores

Desarrollado por **Bonora & Rossi** como trabajo obligatorio para la Universidad ORT Uruguay (Obligatorio 01, agosto 2025).
