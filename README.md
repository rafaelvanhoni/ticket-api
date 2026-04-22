# 🎫 Ticket API

> 🚧 **Status:** In Development

A simple REST API for managing tickets, built with **ASP.NET Core Minimal APIs**.

This project is part of my transition from legacy ERP development (**Progress 4GL / Datasul**) to modern backend development using **C#** and **.NET**.

---

## 🎯 Purpose

The main goal of this project is to practice modern backend concepts and build a solid foundation in REST API development, including:

- Dependency Injection
- Separation of Concerns
- DTOs
- Repository Pattern
- Enums for domain consistency
- Minimal APIs

Additionally, the project applies the **Result Pattern** using `OperationResult<T>`, ensuring standardized and predictable API responses.

The project intentionally uses **in-memory storage** at this stage so the focus stays on architecture, domain modeling, and API behavior instead of database setup.

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
- In-memory data storage

---

## 🧱 Project Structure

```text
TicketApi/
│
├── DTOs/          # API contracts (input/output models)
├── Interfaces/    # Abstractions (Repository, Validation)
├── Models/        # Domain entities
├── Repositories/  # Data access layer (in-memory)
├── Services/      # Business logic and rules
├── Shared/        # Shared utilities (OperationResult)
└── Program.cs     # API endpoints (Minimal API)
```

---

## 🧠 Design Decisions

- **Minimal API** approach for simplicity and focus on core concepts
- **Service layer** as the central point for business rules (not just data flow)
- **Repository layer** to abstract data access
- **DTOs** to separate API contracts from domain entities
- **Enums** used for `Status` and `Priority`
- **Result Pattern (`OperationResult<T>`)** for consistent API responses
- **In-memory repository** to keep the project lightweight and educational

This design makes it easier to evolve the project later, including replacing the in-memory repository with a real database implementation.

---

## 🛠 Tech Stack

- .NET 8
- ASP.NET Core Minimal APIs
- Swagger / OpenAPI
- System.Text.Json
- Dependency Injection

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
- Enum values are currently handled by the API as domain-constrained values
- No database configuration is required to run this project

---

## ▶️ Running the Project

1. Clone the repository  
2. Go to the project folder  
3. Run the application:

```bash
dotnet run
```

4. Open Swagger UI:

```text
http://localhost:5138/swagger
```

---

## 📈 Evolution

This project went through a full **refactoring (cleanup)** phase:

- Renamed all methods and variables to English  
- Removed legacy study/test code  
- Standardized naming conventions  
- Improved code organization and readability  

---

## 🗺 Roadmap

- [x] Implement `PUT /tickets/{id}`
- [x] Implement `DELETE /tickets/{id}`
- [x] Refactor codebase to English
- [ ] Improve PUT semantics
- [ ] Add stronger validation rules
- [ ] Improve error handling
- [ ] Add automated tests
- [ ] Add logging and basic observability
- [ ] Replace in-memory storage with a database in a future version

---

## 📄 License

This project is for educational purposes.