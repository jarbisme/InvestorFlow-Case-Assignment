# InvestorFlow-Case-Assignment

A Contact Management API for Investment Funds built with .NET 8.

## Overview

This project is a RESTful API that enables basic contact management operations for investment funds. It allows creating, reading, updating, and deleting contact records, as well as assigning contacts to funds and retrieving fund-specific contact information.

## Technologies

- .NET 8
- Minimal API pattern
- SQL Server (in-memory database)
- FluentResults for error handling
- Swagger for API documentation and testing

## Architecture

The application follows a layered architecture pattern:
- Presentation Layer: API endpoints and request handlers
- Business Layer: Services implementing business logic
- Data Layer: Repositories for database operations

## Dependency Flow

Endpoints → Services → Repositories → DbContext → InMemory DB

## Error Handling

The application uses the Result pattern via FluentResults library to communicate errors and statuses between layers, providing a consistent approach to error handling.

## API Response Structure

All API responses follow a standardized format:

```
{
  "status": "Success|Fail|Error",
  "data": {}, // Response data or null
  "message": "Operation result message",
  "errors": [] // Any error messages
}
```

## Project Structure

```
MinimalAPI/
├── Data/             # Data Access Layer/Repositories and DbContext
├── Models/           # Domain Models/DTOs/ApiResponse  
├── Services/         # Business Logic Layer
├── Endpoints/        # Presentation Layer
├── Validators/       # Requests Validation 
└── Program.cs        # Entry Point
```

## API Endpoints

**Contact Operations**
| Endpoint | Method | Description | Request | Response | 
|----------|--------|-------------|---------|----------| 
| /api/contacts | GET | Get all contacts | None | List of contacts | 
| /api/contacts/{id} | GET | Get contact by ID | None | Single contact |
| /api/contacts | POST | Create contact | Contact info | Created contact |
| /api/contacts/{id} | PUT | Update contact | Contact info | None |
| /api/contacts/{id} | DELETE | Delete contact | None | None |

**Fund-Contact Operations**
| Endpoint | Method | Description | Request | Response | 
|----------|--------|-------------|---------|----------| 
| /api/funds | GET | Get all funds | None | List of funds | 
| /api/funds/{fundId} | GET | Get fund with contacts | None | Fund with contacts | 
| /api/funds/{fundId}/contacts | POST | Assign contact to fund | Contact ID | None | 
| /api/funds/{fundId}/contacts/{contactId} | DELETE | Remove contact from fund | None | None |

## Business Rules
- Contacts must have a name (mandatory field)
- A contact can only be assigned to a fund once
- Contacts assigned to funds cannot be deleted
