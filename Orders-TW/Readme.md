# Orders-TW API

API REST en .NET 8 para gestión de productos, clientes y órdenes de compra.

## ✅ Forma principal de probar: Docker (recomendado)

Este proyecto está preparado para que lo pruebes de punta a punta con Docker Compose (API + MySQL) sin configurar nada extra en local.

## 1) Prerrequisitos

- Docker Desktop instalado y encendido
- Puerto `8080` libre (API)
- Puerto `3306` libre (MySQL)

## 2) Levantar todo con Docker

Desde la raíz del workspace (`Orders-TW`):

```bash
docker compose up --build -d
```

Verificar estado:

```bash
docker compose ps
```

Debes ver:
- `orders-db` en `healthy`
- `orders-api` en `Up`

## 3) Probar que la API está viva

```bash
curl http://localhost:8080/health
```

Respuesta esperada:
- `Healthy`

Swagger:
- http://localhost:8080

## 4) Probar autenticación JWT

### Login

`POST /api/auth/login`

Body:

```json
{
  "username": "admin",
  "password": "admin"
}
```

Respuesta esperada:
- `200 OK`
- Devuelve `token`, `expiresIn`, `username`

## 5) Probar endpoints protegidos (con token)

Con el token JWT, prueba:

- `POST /api/products`
- `PUT /api/products/{id}`
- `DELETE /api/products/{id}`
- `POST /api/customers`
- `POST /api/orders`

Sin token, deben responder `401 Unauthorized`.

## 6) Prueba funcional crítica de órdenes

Flujo recomendado:

1. Consultar un producto: `GET /api/products/{id}` y anotar `stock`.
2. Crear orden en `POST /api/orders` con ese producto y cantidad válida.
3. Consultar de nuevo `GET /api/products/{id}`.
4. Verificar que el stock bajó exactamente en la cantidad ordenada.

También valida:
- Si el stock no alcanza, retorna `400`.
- El total de la orden se calcula automáticamente.

## 7) Parar entorno Docker

```bash
docker compose down
```

Si quieres borrar también los datos:

```bash
docker compose down -v
```

---

## Endpoints disponibles

### Auth
- `POST /api/auth/login`

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

---

## Migraciones EF Core

Crear migración:

```bash
dotnet ef migrations add NombreMigracion --project Orders-TW/Orders-TW.csproj
```

Aplicar migraciones:

```bash
dotnet ef database update --project Orders-TW/Orders-TW.csproj
```

> En `Development`, la app ejecuta `EnsureCreated` y carga datos semilla.

---

## Ejecutar sin Docker (opcional)

1. Configurar cadena de conexión en User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=OrdersDb;user=orders_user;password=orders_pass123;"
```

2. Ejecutar API:

```bash
dotnet run --project Orders-TW/Orders-TW.csproj
```

3. Swagger local:
- http://localhost:5000 o https://localhost:5001

---

## Tests automáticos

Proyecto: `Orders-TW.Tests`

```bash
dotnet test .\Orders-TW.Tests\Orders-TW.Tests.csproj
```

Cubre:
- Login JWT
- Endpoints públicos (`GET`) de products/customers/orders
- Endpoints protegidos con y sin token
- Creación de órdenes validando total y reducción de stock

---

## Estado respecto a la consigna

### Cumplido
- .NET 8 Web API
- Entity Framework Core
- Módulos `Products`, `Customers`, `Orders`
- Arquitectura por capas (`Controllers`, `Services`, `Repositories`, `DTOs`)
- Validaciones (DataAnnotations + FluentValidation)
- Middleware global de errores
- Extras: Docker, Swagger, JWT
- README con ejecución, migraciones y endpoints

### Observación pendiente
- La consigna pide **SQL Server o SQLite**.
- Actualmente el proyecto usa **MySQL (Pomelo)**.
