

## 1️⃣ README — *User Management API*


# User Management API

This application provides a RESTful API for user management and authentication.
It supports user registration, login with JWT authentication, user listing, update,
and deletion.

## Features

- User authentication using JWT
- User CRUD operations
- Role-based user structure
- Secure password handling
- RESTful API design

## Base URL

```

[http://localhost:5000](http://localhost:5000)

```

### run migrations 
```bash
dotnet ef migrations add AddUserRole --project backend.Infrastructure\backend.Infrastructure.csproj --startup-project backend\backend.csproj
dotnet ef database update --project backend.Infrastructure\backend.Infrastructure.csproj --startup-project backend\backend.csproj
```

## Authentication

Authentication is performed using JWT (JSON Web Token).

After a successful login, the token must be sent in the `Authorization` header:

```

Authorization: Bearer <token>

````

## Endpoints

### Login

**POST** `/api/Users/login`

Authenticates a user and returns a JWT token.

**Request Body**
```json
{
  "email": "user@email.com",
  "password": "password123"
}
````

**Response**

```json
{
  "success": true,
  "message": "Login successful",
  "token": "jwt-token",
  "user": {
    "id": "guid",
    "name": "John Doe",
    "email": "user@email.com",
    "role": "admin"
  }
}
```

---

### Get All Users

**GET** `/api/Users`

Requires authentication.

---

### Create User

**POST** `/api/Users`

Creates a new user.

**Request Body**

```json
{
  "name": "John Doe",
  "email": "user@email.com",
  "password": "password123"
}
```

---

### Get User By ID

**GET** `/api/Users/{id}`

---

### Update User

**PUT** `/api/Users/{id}`

**Request Body**

```json
{
  "name": "New Name",
  "email": "new@email.com"
}
```

---

### Delete User

**DELETE** `/api/Users/{id}`

---

## Schemas

### AuthResponseDto

* success: boolean
* message: string
* token: string
* user: UserResponseDto

### CreateUserDto

* name: string
* email: string
* password: string

### LoginDto

* email: string
* password: string

### UpdateUserDto

* name: string
* email: string

### UserResponseDto

* id: string
* name: string
* email: string
* role: string
* createdAt: datetime
* updatedAt: datetime

## Technologies

* .NET
* Entity Framework Core
* JWT Authentication
* SQL Server (or compatible relational database)

## Architecture

The application follows a layered architecture:

* API (Controllers)
* Application / Services
* Infrastructure (Data, EF Core)
* Domain (Entities, Rules)



---

## 2️⃣ Postman Collection (JSON)

👉 **Pode copiar e importar direto no Postman**

```json
{
  "info": {
    "name": "User Management API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Login",
      "request": {
        "method": "POST",
        "header": [{ "key": "Content-Type", "value": "application/json" }],
        "url": { "raw": "{{baseUrl}}/api/Users/login", "host": ["{{baseUrl}}"], "path": ["api", "Users", "login"] },
        "body": {
          "mode": "raw",
          "raw": "{\n  \"email\": \"user@email.com\",\n  \"password\": \"password123\"\n}"
        }
      }
    },
    {
      "name": "Get Users",
      "request": {
        "method": "GET",
        "header": [{ "key": "Authorization", "value": "Bearer {{token}}" }],
        "url": { "raw": "{{baseUrl}}/api/Users", "host": ["{{baseUrl}}"], "path": ["api", "Users"] }
      }
    },
    {
      "name": "Create User",
      "request": {
        "method": "POST",
        "header": [{ "key": "Content-Type", "value": "application/json" }],
        "url": { "raw": "{{baseUrl}}/api/Users", "host": ["{{baseUrl}}"], "path": ["api", "Users"] },
        "body": {
          "mode": "raw",
          "raw": "{\n  \"name\": \"John Doe\",\n  \"email\": \"user@email.com\",\n  \"password\": \"password123\"\n}"
        }
      }
    },
    {
      "name": "Get User By Id",
      "request": {
        "method": "GET",
        "header": [{ "key": "Authorization", "value": "Bearer {{token}}" }],
        "url": { "raw": "{{baseUrl}}/api/Users/{{id}}", "host": ["{{baseUrl}}"], "path": ["api", "Users", "{{id}}"] }
      }
    },
    {
      "name": "Update User",
      "request": {
        "method": "PUT",
        "header": [
          { "key": "Content-Type", "value": "application/json" },
          { "key": "Authorization", "value": "Bearer {{token}}" }
        ],
        "url": { "raw": "{{baseUrl}}/api/Users/{{id}}", "host": ["{{baseUrl}}"], "path": ["api", "Users", "{{id}}"] },
        "body": {
          "mode": "raw",
          "raw": "{\n  \"name\": \"Updated Name\",\n  \"email\": \"updated@email.com\"\n}"
        }
      }
    },
    {
      "name": "Delete User",
      "request": {
        "method": "DELETE",
        "header": [{ "key": "Authorization", "value": "Bearer {{token}}" }],
        "url": { "raw": "{{baseUrl}}/api/Users/{{id}}", "host": ["{{baseUrl}}"], "path": ["api", "Users", "{{id}}"] }
      }
    }
  ],
  "variable": [
    { "key": "baseUrl", "value": "http://localhost:5000" },
    { "key": "token", "value": "" },
    { "key": "id", "value": "" }
  ]
}
````

---








