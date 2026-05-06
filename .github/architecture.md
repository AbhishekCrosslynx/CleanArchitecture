
# 🧱 Clean Architecture + Blazor Solution Overview

## 📌 Purpose of This Document

This document provides a **complete understanding of the project structure, architectural decisions, and responsibilities of each layer**. It is intended for:

* Developers onboarding to the project
* AI tools like GitHub Copilot
* Maintaining architectural consistency

---

# 🏗️ High-Level Architecture

This solution follows **Clean Architecture principles** with strict separation of concerns:

```
Presentation (UI)
    ↓
Web API (Entry Point)
    ↓
Application (Use Cases)
    ↓
Domain (Core Business Logic)
    ↓
Infrastructure (External Concerns)
```

Additionally:

* `SharedKernel` contains reusable primitives
* `Tests` enforce architectural rules

---

# 📁 Solution Structure

```
src/
 ├── Web.Api
 ├── Application
 ├── Domain
 ├── Infrastructure
 ├── SharedKernel
 ├── UI (Blazor apps + Shared RCL)
 └── Aspire (Service orchestration)

tests/
 ├── Application.Tests.L0
 ├── ArchitectureTests
 └── WebApi.Tests.L1
```

---

# 🧠 Core Architectural Principles

## 1. Dependency Rule

* Dependencies **only point inward**
* Outer layers depend on inner layers
* Inner layers **never depend on outer layers**

## 2. Separation of Concerns

Each layer has a **single responsibility**:

* Domain → Business logic
* Application → Use cases
* Infrastructure → External systems
* Presentation → UI / API

## 3. CQRS Pattern

* Commands → mutate state
* Queries → read state
* Implemented in `Application` layer

---

# 🔷 Domain Layer (`Domain`)

## 📌 Purpose

Contains **core business logic and rules**. This is the most important and stable layer.

## 📂 Structure

* `Users/`
* `Todos/`

## ✅ Contents

* Entities (`User`, `TodoItem`)
* Value Objects (`Priority`)
* Domain Events
* Domain Errors

## 🚫 Rules

* No external dependencies
* No database or framework logic

## 💡 Example Responsibilities

* Business rules enforcement
* Raising domain events
* Entity invariants

---

# 🔷 SharedKernel (`SharedKernel`)

## 📌 Purpose

Reusable primitives across all layers.

## ✅ Contains

* `Entity` base class
* `Result` pattern
* `Error`, `ErrorType`
* `IDomainEvent`
* `IDateTimeProvider`

## 💡 Why It Exists

Avoid duplication and keep **cross-cutting domain concepts centralized**

---

# 🔷 Application Layer (`Application`)

## 📌 Purpose

Implements **use cases** and orchestrates domain logic.

## 📂 Structure

* `Users/`
* `Todos/`
* `Abstractions/`

## ✅ Key Concepts

### 1. Commands & Queries (CQRS)

* Commands → `CreateTodoCommand`
* Queries → `GetTodosQuery`

### 2. Handlers

* Each command/query has a handler
* Example:

  * `CreateTodoCommandHandler`
  * `GetUserByIdQueryHandler`

### 3. Validators

* Input validation using validators

### 4. Behaviors (Decorators)

* Logging
* Validation

### 5. Interfaces

Defined in `Abstractions`:

* `IApplicationDbContext`
* `IUserContext`
* `ITokenProvider`

## 🚫 Rules

* No direct infrastructure usage
* Only depends on Domain + SharedKernel

---

# 🔷 Infrastructure Layer (`Infrastructure`)

## 📌 Purpose

Handles **external concerns**:

* Database
* Authentication
* Authorization

## 📂 Structure

* `Database/`
* `Authentication/`
* `Authorization/`
* `DomainEvents/`

## ✅ Responsibilities

### Database

* `ApplicationDbContext`
* Entity configurations
* Migrations

### Authentication

* Token generation
* Password hashing
* User context

### Authorization

* Permission-based system
* Custom policies

### Domain Events Dispatcher

* Handles domain event publishing

## 💡 Key Rule

Implements interfaces defined in **Application layer**

---

# 🔷 Web API Layer (`Web.Api`)

## 📌 Purpose

Acts as the **entry point for backend**

## 📂 Structure

* `Endpoints/`
* `Middleware/`
* `Extensions/`

## ✅ Features

### Minimal APIs

* Each feature is an endpoint class
* Example:

  * `CreateTodo`
  * `Login`
  * `Register`

### Middleware

* Logging
* Exception handling

### Extensions

* Dependency injection setup
* Endpoint mapping

## 💡 Responsibilities

* HTTP handling
* Request/Response mapping
* Delegating to Application layer

---

# 🔷 UI Layer (`UI`)

## 📌 Purpose

Contains **Blazor-based frontends**

## 🧩 Projects

### 1. Shared (Razor Class Library)

Reusable UI components and services

#### Contains:

* Layouts
* Components
* Services (e.g., Notifications, Preferences)
* Themes
* JS Interop

---

### 2. Test (Blazor WebAssembly)

* Sample/test UI
* Demonstrates usage of shared components

---

### 3. Client

* Lightweight frontend
* Likely consumer-facing

---

### 4. Operations

* Internal/admin UI

---

## 💡 Design Approach

* Shared UI logic lives in RCL
* Apps consume shared components

---

# 🔷 Aspire (`Aspire`)

## 📌 Purpose

Used for **service orchestration and distributed app setup**

## Contains:

* `AppHost`
* Service defaults

---

# 🧪 Tests (`ArchitectureTests`)

## 📌 Purpose

Ensure architectural integrity

## ✅ Validations

* Layer dependency rules
* No forbidden references

## 💡 Example

* Domain must not depend on Infrastructure

---

# 🔄 Request Flow

```
Blazor UI
   ↓
Web.Api Endpoint
   ↓
Application Command/Query
   ↓
Domain Logic
   ↓
Infrastructure (DB, Auth)
   ↓
Response back to UI
```

---

# 🔐 Cross-Cutting Concerns

## Logging

* Implemented via decorators

## Validation

* Command validators

## Error Handling

* Result pattern
* Global exception middleware

---

# ⚙️ Dependency Injection

Each layer registers dependencies via:

* `DependencyInjection.cs`

---

# 📏 Naming Conventions

| Type      | Pattern          |
| --------- | ---------------- |
| Command   | `CreateXCommand` |
| Query     | `GetXQuery`      |
| Handler   | `XHandler`       |
| Validator | `XValidator`     |
| Endpoint  | `X.cs`           |

---

# 🚀 Key Design Decisions

## 1. Minimal APIs Instead of Controllers

* Lightweight
* Feature-based structure

## 2. CQRS Without MediatR (Custom Implementation)

* More control
* Less abstraction overhead

## 3. Razor Class Library for Shared UI

* Avoid duplication
* Reusable components

## 4. Domain Events

* Decoupled side effects

---

# ⚠️ Important Rules for Contributors

## ❌ Do NOT:

* Reference Infrastructure from Domain
* Put business logic in UI or API
* Skip validation

## ✅ ALWAYS:

* Use Application layer for use cases
* Keep Domain pure
* Follow CQRS

---

# 🧭 How to Add a New Feature

## Example: Add "Project" Feature

### 1. Domain

* Create Entity
* Add domain rules

### 2. Application

* Create Commands/Queries
* Add Handlers + Validators

### 3. Infrastructure

* Add DB configuration

### 4. Web API

* Add Endpoint

### 5. UI

* Add pages/components

---

# 📚 Summary

This project is designed to be:

* ✅ Scalable
* ✅ Maintainable
* ✅ Testable
* ✅ Cleanly separated

It enforces:

* Strict layering
* Clear responsibilities
* Minimal coupling

---

# 🧠 Final Note for AI Tools (Copilot)

* Business logic lives in **Domain**
* Use cases live in **Application**
* External systems live in **Infrastructure**
* UI is purely **presentation**
* Always respect **dependency direction**

---