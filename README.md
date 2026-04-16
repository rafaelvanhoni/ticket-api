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

The project intentionally uses **in-memory storage** at this stage so the focus stays on architecture, domain modeling, and API behavior instead of database setup.

---

## 🚀 Features

Current features:

- Create tickets
- Retrieve all tickets
- Retrieve a ticket by ID
- Filter tickets by status
- Filter tickets by priority
- In-memory data storage

> ⚠️ **Note:** Update (`PUT`) and Delete (`DELETE`) operations are planned for future versions.

---

## 🧱 Project Structure

```text
TicketApi/
│
├── DTOs/
├── Interfaces/
├── Models/
├── Repositories/
├── Services/
└── Program.cs
```

---

## 🧠 Design Decisions

- **Minimal API** approach for simplicity and focus on core concepts
- **Service layer** for business logic
- **Repository layer** to abstract data access
- **DTOs** to separate API contracts from domain entities
- **Enums** used for `Status` and `Priority`
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
- `prioridade` → `Low`, `Medium`, `High`

Example:

```http
GET /tickets?status=Open&prioridade=High
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

## 🗺 Roadmap

- [ ] Implement `PUT /tickets/{id}`
- [ ] Implement `DELETE /tickets/{id}`
- [ ] Add stronger validation rules
- [ ] Improve error handling
- [ ] Add automated tests
- [ ] Add logging and basic observability
- [ ] Replace in-memory storage with a database in a future version

---

## 📄 License

This project is for educational purposes.
