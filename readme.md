

# Enterprise Auth & User Management API

Enterprise-ready authentication and user management module built with .NET, Clean Architecture and JWT security.

This API provides:

- Enterprise authentication
- Device-aware login
- TOTP/MFA support
- User security tracking
- Audit logging
- JWT authentication
- User lifecycle management
- Clean architecture modular design

---

## Features

- JWT Access Token authentication
- Device-based authentication
- MFA/TOTP support
- Audit logging system
- UserSecurity lockout tracking
- Enterprise auth module isolated
- Role-based authorization
- Clean Architecture
- Docker ready
- PostgreSQL ready

---

## Base URL

```

[http://localhost:5022](http://localhost:5022)

```

Swagger:

```

[http://localhost:5022/swagger](http://localhost:5022/swagger)

````

---

## Database Migration

```bash
dotnet ef migrations add InitAuthModule \
 --project backend.Infrastructure \
 --startup-project backend

dotnet ef database update \
 --project backend.Infrastructure \
 --startup-project backend
````

---

## Authentication

JWT Bearer Token:

```
Authorization: Bearer <access_token>
```

---

# Endpoints

---

## AUTH

---

### Login

POST `/api/auth/login`

Enterprise login with device tracking and optional TOTP.

#### Request

```json
{
  "email": "user@email.com",
  "password": "123",
  "totpCode": "123456",
  "deviceId": "device-001"
}
```

#### Response

```json
{
  "accessToken": "jwt",
  "expiresIn": 3600,
  "user": {
    "id": "guid",
    "name": "Bruno",
    "email": "user@email.com",
    "role": "User"
  }
}
```

---

## USERS

---

### Create User

POST `/api/Users`

Creates:

* User
* UserSecurity
* AuditLog entry

```json
{
  "name": "Bruno",
  "email": "bruno@email.com",
  "password": "123"
}
```

---

### Get All Users

GET `/api/Users`

Requires JWT.

---

### Get User By Id

GET `/api/Users/{id}`

---

### Update User

PUT `/api/Users/{id}`

```json
{
  "name": "Updated Name",
  "email": "updated@email.com"
}
```

---

### Delete User

DELETE `/api/Users/{id}`

---

# Domain Entities

---

## User

| Field        | Type   |
| ------------ | ------ |
| Id           | Guid   |
| Name         | string |
| Email        | string |
| PasswordHash | string |
| Role         | string |

---

## UserSecurity

| Field            | Type      |
| ---------------- | --------- |
| UserId           | Guid      |
| FailedLoginCount | int       |
| LockoutEnd       | DateTime? |
| MfaEnabled       | bool      |
| TOTPSecret       | string?   |

---

## AuditLog

| Field        | Type     |
| ------------ | -------- |
| Id           | Guid     |
| UserId       | Guid?    |
| Event        | string   |
| Ip           | string   |
| DeviceId     | string   |
| MetadataJson | string?  |
| CreatedAt    | DateTime |

---

# DTO Schemas

---

## CreateUserDto

```json
{
  "name": "string",
  "email": "string",
  "password": "string"
}
```

---

## LoginRequest

```json
{
  "email": "string",
  "password": "string",
  "totpCode": "string",
  "deviceId": "string"
}
```

---

## UserResponseDto

```json
{
  "id": "guid",
  "name": "string",
  "email": "string",
  "role": "string"
}
```

---

# Technologies

* .NET 8
* ASP.NET Core
* Entity Framework Core
* PostgreSQL
* JWT Bearer Authentication
* Docker
* Clean Architecture

---

# Architecture

```
backend.API
backend.Application
backend.Domain
backend.Infrastructure
```

### Layers

* Controllers → HTTP Layer
* Application → Services / Use Cases
* Domain → Entities / Rules
* Infrastructure → EF Core / Persistence / External

---

# Security Model

* Password Hashing
* JWT Access Tokens
* MFA Support
* Login Lockout
* Device Tracking
* Audit Logging
* Role Authorization

---

# Postman Collection

```json
{
  "info": {
    "name": "Enterprise Auth API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Login",
      "request": {
        "method": "POST",
        "header": [
          { "key": "Content-Type", "value": "application/json" }
        ],
        "url": {
          "raw": "{{baseUrl}}/api/auth/login",
          "host": ["{{baseUrl}}"],
          "path": ["api","auth","login"]
        },
        "body": {
          "mode": "raw",
          "raw": "{\n  \"email\":\"user@email.com\",\n  \"password\":\"123\",\n  \"totpCode\":\"\",\n  \"deviceId\":\"device-001\"\n}"
        }
      }
    },
    {
      "name": "Create User",
      "request": {
        "method": "POST",
        "header":[
          {"key":"Content-Type","value":"application/json"},
          {"key":"Authorization","value":"Bearer {{token}}"}
        ],
        "url":{
          "raw":"{{baseUrl}}/api/Users",
          "host":["{{baseUrl}}"],
          "path":["api","Users"]
        },
        "body":{
          "mode":"raw",
          "raw":"{\n \"name\":\"Bruno\",\n \"email\":\"bruno@email.com\",\n \"password\":\"123\"\n}"
        }
      }
    },
    {
      "name": "Get Users",
      "request": {
        "method": "GET",
        "header":[
          {"key":"Authorization","value":"Bearer {{token}}"}
        ],
        "url":{
          "raw":"{{baseUrl}}/api/Users",
          "host":["{{baseUrl}}"],
          "path":["api","Users"]
        }
      }
    }
  ],
  "variable":[
    {"key":"baseUrl","value":"http://localhost:5022"},
    {"key":"token","value":""}
  ]
}
```

---


