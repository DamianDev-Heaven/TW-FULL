# Orders-TW API

API REST en .NET 8 para gestionar productos, clientes y órdenes de compra.

## Stack

- .NET 8 Web API
- Entity Framework Core
- MySQL (Pomelo)
- Swagger
- JWT
- Docker / Docker Compose
- xUnit (tests de integración de endpoints)

## Estructura

- `Controllers`
- `Services`
- `Repositories`
- `DTOs`
- `Validators` (FluentValidation + DataAnnotations en DTOs)
- `Middleware` (manejo global de errores)

## Requisitos previos

- .NET SDK 8+
- Docker Desktop (opcional, si se corre con contenedores)

## Ejecutar en local (sin Docker)

1. Configura conexión en User Secrets (recomendado):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=OrdersDb;user=orders_user;password=orders_pass123;"
```

2. Ejecuta la API:

```bash
dotnet run --project Orders-TW/Orders-TW.csproj
```

3. Swagger:

- `http://localhost:5000` o `https://localhost:5001` (según perfil de lanzamiento)

## Ejecutar con Docker

Desde la raíz del workspace:

```bash
docker compose up --build
```

Servicios:

- API: `http://localhost:8080`
- MySQL: `localhost:3306`

Para bajar servicios:

```bash
docker compose down
```

Para bajar y eliminar volumen de datos:

```bash
docker compose down -v
```

## Migraciones EF Core

Crear migración nueva:

```bash
dotnet ef migrations add NombreMigracion --project Orders-TW/Orders-TW.csproj
```

Aplicar migraciones:

```bash
dotnet ef database update --project Orders-TW/Orders-TW.csproj
```

> Nota: en entorno `Development`, la app también ejecuta `EnsureCreated` y carga datos semilla.

## Endpoints

### Auth

- `POST /api/auth/login` (obtiene JWT)

### Products

- `GET /api/products?page=1&pageSize=10`
- `GET /api/products/all`
- `GET /api/products/{id}`
- `POST /api/products` (JWT)
- `PUT /api/products/{id}` (JWT)
- `DELETE /api/products/{id}` (JWT)

### Customers

- `GET /api/customers?page=1&pageSize=10`
- `GET /api/customers/all`
- `GET /api/customers/{id}`
- `POST /api/customers` (JWT)

### Orders

- `GET /api/orders?page=1&pageSize=10`
- `GET /api/orders/all`
- `GET /api/orders/{id}`
- `POST /api/orders` (JWT)

## Validaciones y manejo de errores

- DataAnnotations en DTOs de entrada.
- FluentValidation para reglas de negocio/entrada.
- Middleware global para mapear excepciones a HTTP (`400`, `404`, `401`, `500`).

## Pruebas

Proyecto de tests: `Orders-TW.Tests`

Ejecutar:

```bash
dotnet test .\Orders-TW.Tests\Orders-TW.Tests.csproj
```

Incluye pruebas de integración para:

- Login JWT
- Endpoints públicos (`GET`) de products/customers/orders
- Endpoints protegidos (`POST/PUT/DELETE`) con y sin token
- Creación de órdenes validando cálculo de total y reducción de stock

## Estado respecto a la consigna

- Funcionalidad principal: ✅
- Arquitectura por capas con DTOs/repositorios/servicios: ✅
- EF Core: ✅
- Docker, Swagger, JWT: ✅
- Base de datos exigida (SQL Server o SQLite): ⚠️ pendiente (actualmente MySQL)

# 🛒 API REST - Sistema de Gestión de Órdenes de Compra

## 🚀 Tecnologías y Arquitectura

| Capa | Tecnología |
|------|-----------|
| **Framework** | .NET 8 Web API |
| **Base de Datos** | MySQL (`Pomelo.EntityFrameworkCore.MySql`) / Compatible con SQLite |
| **ORM** | Entity Framework Core 8 |
| **Arquitectura** | N-Capas (Controllers, Services, Repositories, DTOs, Models) |
| **Validaciones** | FluentValidation y DataAnnotations |
| **Manejo de Errores** | Middleware global personalizado (`GlobalExceptionMiddleware`) |

### Extras implementados

- 📄 Documentación interactiva con **Swagger**
- 🔐 Autenticación **JWT** (JSON Web Tokens)
- 📦 **Paginación** en endpoints de listado
- 🌱 **Data Seeder** (carga automática de datos de prueba en modo `Development`)

---

## 📋 Requerimientos Funcionales Cumplidos

### 🏷️ Products
- CRUD completo (Crear, Obtener, Actualizar, Eliminar)
- Endpoints de mutación protegidos por JWT

### 👤 Customers
- Crear y obtener listado de clientes

### 📑 Orders
- Creación de órdenes validando la existencia del cliente y los productos
- **Validación de stock**: verifica que el stock del producto sea suficiente antes de procesar
- **Cálculo automático**: calcula el total de la orden en base a la cantidad y precio del producto
- **Reducción de stock**: descuenta el stock de los productos automáticamente al generar la orden
- Obtención de órdenes con sus detalles completos

---

## ⚙️ Cómo correr el proyecto

### 1. Prerrequisitos

- SDK de **.NET 8** instalado
- Servidor **MySQL** ejecutándose (local o en Docker)

### 2. Configurar la Base de Datos

Debes configurar tu cadena de conexión a MySQL. Puedes hacerlo de dos formas:

**Opción A — User Secrets (Recomendado para desarrollo)**

Abre una terminal en la carpeta del proyecto (`Orders-TW`) y ejecuta:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost;Port=3306;Database=OrdersDB;User=tu_usuario;Password=tu_contraseña;"
```

**Opción B — `appsettings.Development.json`**

Edita el archivo y agrega la cadena de conexión directamente:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=OrdersDB;User=tu_usuario;Password=tu_contraseña;"
}
```

### 3. Migraciones y Base de Datos

El proyecto incluye un **DataSeeder** configurado. Si ejecutas la aplicación en entorno `Development`, la base de datos se creará automáticamente y se poblará con datos de prueba:

- 10 productos
- 8 clientes
- 5 órdenes

Si prefieres aplicar las migraciones manualmente:

```bash
dotnet ef database update
```

### 4. Ejecutar la API

Desde la carpeta raíz del proyecto (`Orders-TW`), ejecuta:

```bash
dotnet run
```

La API se levantará por defecto en `http://localhost:5221`.  
Puedes acceder a la interfaz interactiva de Swagger en: 👉 [http://localhost:5221/swagger](http://localhost:5221/swagger)

---

## 🔐 Autenticación (JWT)

Para utilizar los endpoints protegidos (Crear/Actualizar/Eliminar productos o Crear órdenes), necesitas un token JWT.

El proyecto incluye un login básico de prueba:

| Campo | Valor |
|-------|-------|
| **Usuario** | `admin` |
| **Contraseña** | `admin` |

**Pasos:**
1. Haz un `POST` a `/api/auth/login` con las credenciales anteriores
2. Copia el token devuelto en la respuesta
3. En Swagger, haz clic en el botón **"Authorize"** e ingresa: `Bearer <tu_token>`  
   O envíalo en el header: `Authorization: Bearer <tu_token>`

---

## 📡 Endpoints Disponibles

> Para ver los esquemas completos y probar los endpoints de forma interactiva, utiliza **Swagger** en `http://localhost:5221/swagger`.

### 🔑 Autenticación

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/auth/login` | Obtiene el token JWT |

### 🏷️ Productos (Products)

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/products` | Obtiene productos paginados (`?page=1&pageSize=10`) | — |
| `GET` | `/api/products/all` | Obtiene todos los productos sin paginación | — |
| `GET` | `/api/products/{id}` | Obtiene un producto por ID | — |
| `POST` | `/api/products` | Crea un producto | 🔒 |
| `PUT` | `/api/products/{id}` | Actualiza un producto | 🔒 |
| `DELETE` | `/api/products/{id}` | Elimina un producto | 🔒 |

### 👤 Clientes (Customers)

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/customers` | Obtiene clientes paginados | — |
| `GET` | `/api/customers/all` | Obtiene todos los clientes | — |
| `GET` | `/api/customers/{id}` | Obtiene un cliente por ID | — |
| `POST` | `/api/customers` | Crea un nuevo cliente | 🔒 |

### 📑 Órdenes (Orders)

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/orders` | Obtiene las órdenes con sus detalles paginadas | — |
| `GET` | `/api/orders/all` | Obtiene todas las órdenes | — |
| `GET` | `/api/orders/{id}` | Obtiene el detalle de una orden específica | — |
| `POST` | `/api/orders` | Crea una orden, calcula totales y reduce el stock | 🔒 |

---

## 🧪 Pruebas Rápidas con archivo `.http`

El proyecto incluye el archivo `Orders-TW.http` con ejemplos listos para ejecutar desde Visual Studio o VS Code (extensión REST Client).

```http
@Orders_TW_HostAddress = http://localhost:5221
@token = <pegar_token_aquí>

### Login - Obtener token JWT
POST {{Orders_TW_HostAddress}}/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin"
}

### Obtener productos paginados
GET {{Orders_TW_HostAddress}}/api/products
Accept: application/json

### Crear una nueva orden
POST {{Orders_TW_HostAddress}}/api/orders
Content-Type: application/json
Authorization: Bearer {{token}}

{
  "customerId": 1,
  "orderItems": [
    { "productId": 1, "quantity": 2 },
    { "productId": 2, "quantity": 1 }
  ]
}
```
