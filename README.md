# Book API

A RESTful API built with ASP.NET Core 8 for managing books and categories.

## Features

- JWT Authentication
- Role-Based Authorization
- Refresh Tokens
- CRUD Operations
- Pagination
- Searching
- Sorting
- Specification Pattern
- Rate Limiting
- Memory Caching
- Global Exception Handling
- File Upload (Book Images)
- Integration Testing
- Docker Support
- SQL Server

## Technologies

- ASP.NET Core 8
- Entity Framework Core
- SQL Server
- AutoMapper
- FluentValidation
- Serilog
- Docker
- JWT Authentication

## Running with Docker

```bash
docker compose up --build
```

API:

http://localhost:8080/swagger

## API Endpoints

### Authentication

- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/refresh-token

### Books

- GET /api/books
- GET /api/books/{id}
- POST /api/books
- PUT /api/books/{id}
- DELETE /api/books/{id}
- POST /api/books/{id}/upload-image

### Categories

- GET /api/categories
- POST /api/categories

## Pagination Example

```http
GET /api/books?page=1&pageSize=5
```

## Search Example

```http
GET /api/books?search=clean
```

## Sorting Example

```http
GET /api/books?sortBy=price
```

## Project Structure

```text
Controllers
Services
Repositories
DTOs
Models
Validators

## What I Learned

- ASP.NET Core Web API
- Entity Framework Core
- JWT Authentication
- Docker
- File Uploads
- Rate Limiting
- Specification Pattern
- Integration Testing
- Caching
Middleware
Specifications
Tests
```
