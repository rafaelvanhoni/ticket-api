# 🎫 Ticket API

> ✅ **Status:** Version 1.0.0 Released

A simple REST API for managing tickets, built with **ASP.NET Core Minimal APIs** and **Entity Framework Core (SQLite)**.

This project is part of my transition from legacy ERP development (**Progress 4GL / Datasul**) to modern backend development using **C#** and **.NET**.

---

## 🎯 Purpose

The main goal of this project is to practice modern backend concepts and build a solid foundation in REST API development, including:

- Dependency Injection
- Separation of Concerns
- DTOs
- Repository Pattern
- Entity Framework Core
- Database migrations
- Minimal APIs
- Async/Await
- Standardized API responses

Additionally, the project applies the **Result Pattern** using `OperationResult<T>`, ensuring standardized and predictable API responses.

The project initially used in-memory storage to focus on architecture and API design.

It has since evolved to use **Entity Framework Core with SQLite**, enabling persistent data storage and database versioning through migrations.

This project was built as a hands-on learning journey to practice backend engineering concepts progressively, focusing on architecture, testing, REST API design, and clean code practices.

---

## 🚀 Features

Current features:

- Create tickets
- Retrieve all tickets
- Retrieve a ticket by ID
- Update tickets
- Delete tickets
- Filter tickets by status
- Filter tickets by priority
- Business rule: completed tickets cannot be deleted
- Persistent data storage using SQLite
- Database migrations with Entity Framework Core
- Validate ticket status and priority values
- Async repository and service operations
- Standardized HTTP responses using OperationResult<T>
- Structured logging with ILogger<T>
- Integration tests using WebApplicationFactory
- Isolated SQLite in-memory database for integration testing
- RESTful `201 Created` responses with `Location` header

---

## 🧪 Tests

This project includes both **unit tests** and **integration tests** using **xUnit**.

### Unit tests cover:

- Ticket creation validation
- Ticket updates
- Business rules
- Ticket filtering
- Enum validation
- Repository interaction verification using Moq

### Integration tests cover:

- Real HTTP requests against API endpoints
- Ticket persistence and retrieval
- Validation responses
- Update and delete flows
- Standardized API responses
- SQLite in-memory database isolation

Integration tests use:

- `WebApplicationFactory`
- `Microsoft.AspNetCore.Mvc.Testing`
- SQLite in-memory database

---

## 🧱 Project Structure

```text
TicketApi/
│
├── src/
│   └── TicketApi/
│       ├── Data/          # DbContext and EF Core migrations
│       ├── DTOs/          # API contracts (input/output models)
│       ├── Interfaces/    # Abstractions (Repository, Validation)
│       ├── Models/        # Domain entities
│       ├── Repositories/  # Data access layer (EF + in-memory)
│       ├── Services/      # Business logic and rules
│       ├── Extensions/    # Extension methods
│       ├── Response/      # Standardized API response models
│       ├── Shared/        # Shared utilities and result patterns
│       ├── Swagger/       # Swagger schema customization
│       ├── Properties/    # App settings (launchSettings, etc)
│       └── Program.cs     # API endpoints (Minimal API)
│
├── tests/
│   └── TicketApi.Tests/
│       ├── Integration/   # Integration tests and API test infrastructure
│       │   ├── TicketApiFactory.cs
│       │   └── TicketIntegrationTests.cs
│       │
│       └── TicketServiceTests.cs # Unit tests for business logic
│
├── .gitignore
├── README.md
└── TicketApi.sln
```

---

## 🧠 Design Decisions

- **Minimal API** approach for simplicity and focus on core concepts
- **Service layer** as the central point for business rules (not just data flow)
- **Repository layer** to abstract data access
- **Entity Framework Core** for persistence
- **SQLite** for lightweight local database
- **DTOs** to separate API contracts from domain entities
- **Enums** used for `Status` and `Priority`
- **Result Pattern (`OperationResult<T>`)** for consistent API responses
- **Extension Methods** for centralized HTTP response handling
- **Async/Await** across repositories and services
- **Dual repository strategy (in-memory + EF Core)** for learning, testing, and flexibility

This design makes it easier to evolve the project later, including replacing the persistence layer if needed.

---

## 🛠 Tech Stack

- .NET 8
- ASP.NET Core Minimal APIs
- Entity Framework Core
- SQLite
- Swagger / OpenAPI
- System.Text.Json
- Dependency Injection
- xUnit
- Moq
- Microsoft.AspNetCore.Mvc.Testing
- SQLite In-Memory
- ILogger<T>

---

## 📡 Endpoints

### `GET /tickets`
Retrieve all tickets.

Optional query parameters:

- `status` → `Open`, `Closed`, `Completed`
- `priority` → `Low`, `Medium`, `High`

Example:

```http
GET /tickets?status=Open&priority=High
```

#### Example response

```json
[
  {
    "id": 1,
    "title": "System failure",
    "description": "Error when processing order",
    "status": "open",
    "priority": "high",
    "assignedTo": "Rafael",
    "createdAt": "2026-04-15T16:02:54.075332-03:00",
    "updatedAt": null,
    "completedAt": null
  }
]
```

---

### `GET /tickets/{id}`
Retrieve a ticket by ID.

#### Example response

```json
{
  "id": 1,
  "title": "System failure",
  "description": "Error when processing order",
  "status": "open",
  "priority": "high",
  "assignedTo": "Rafael",
  "createdAt": "2026-04-15T16:02:54.075332-03:00",
  "updatedAt": null,
  "completedAt": null
}
```

---

### `POST /tickets`
Create a new ticket.

#### Request body

```json
{
  "title": "System failure",
  "description": "Error when processing order",
  "status": "open",
  "priority": "high",
  "assignedTo": "Rafael"
}
```

#### Example response

```json
{
  "id": 5,
  "title": "System failure",
  "description": "Error when processing order",
  "status": "open",
  "priority": "high",
  "assignedTo": "Rafael",
  "createdAt": "2026-04-15T16:02:54.075332-03:00",
  "updatedAt": null,
  "completedAt": null
}
```

---

### `PUT /tickets/{id}`
Update an existing ticket.

> ⚠️ This endpoint performs a full update. All required fields must be provided in the request body.

#### Request body

```json
{
  "title": "Updated title",
  "description": "Updated description",
  "status": "completed",
  "priority": "high",
  "assignedTo": "Rafael"
}
```

#### Possible responses

- `200 OK` → Ticket updated successfully  
- `404 Not Found` → Ticket does not exist  
- `400 Bad Request` → Validation error  

---

### `DELETE /tickets/{id}`
Delete a ticket.

Example:

```http
DELETE /tickets/3
```

#### Possible responses

- `200 OK` → Ticket deleted  
- `404 Not Found` → Ticket not found  
- `400 Bad Request` → Ticket is already `Completed`  

---

## ⚠️ Notes

- `status` and `priority` must be sent as **strings** in JSON
- Enum values are handled by the API as domain-constrained values
- SQLite database files are ignored by Git (`*.db`, `*.db-shm`, `*.db-wal`)  
- Database schema is managed through Entity Framework Core migrations  
- The project uses a local SQLite database (no external setup required)  

---

## ▶️ Running the Project

1. Clone the repository  
2. Go to the project folder  

3. Restore dependencies:

```bash
dotnet restore
```

4. Apply database migrations (requires EF CLI):

```bash
dotnet ef database update
```

5. Run the application:

```bash
dotnet run
```

6. Open Swagger UI:

```text
http://localhost:5138/swagger
```

---

## 📈 Evolution

This project went through a full evolution process:

- Initial CRUD implementation using in-memory storage  
- Refactoring focused on clean code and naming standardization (Portuguese → English)  
- Introduction of DTOs and separation of concerns  
- Implementation of Result Pattern (`OperationResult<T>`)  
- Addition of unit tests using xUnit and Moq  
- Introduction of repository abstraction with `ITicketRepository`  
- Implementation of dual repository strategy (in-memory + EF Core)  
- Migration from in-memory storage to SQLite persistence  
- Introduction of Entity Framework Core and database migrations
- Refactoring repositories and services to async/await
- Standardization of API responses using extension methods  
- Added integration tests using WebApplicationFactory
- Added isolated SQLite in-memory database for integration tests
- Introduced structured logging with ILogger<T>
- Improved RESTful semantics with 201 Created responses
- Added Created result status support to OperationResult<T>

---

## 🗺 Roadmap

- [x] Implement basic CRUD operations
- [x] Refactor codebase to English
- [x] Improve PUT semantics
- [x] Add stronger validation rules
- [x] Improve error handling
- [x] Add automated tests
- [x] Replace in-memory storage with SQLite database
- [x] Introduce Entity Framework Core and migrations
- [x] Improve enum handling in query parameters  
- [x] Improve Swagger documentation for enum values
- [x] Introduce async/await in repositories and services
- [x] Improve API response standardization  
- [x] Add logging and basic observability
- [x] Add integration tests

---

## 📄 License

This project is for educational purposes.