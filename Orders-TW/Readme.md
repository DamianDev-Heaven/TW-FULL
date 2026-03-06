# PruebaTechTechy Orders API

API REST profesional desarrollada en .NET 8 para gestionar un sistema completo de órdenes de compra con autenticación JWT y arquitectura empresarial.

> 🚀 **Quick Start**: Ver [QUICK_START.md](QUICK_START.md) para empezar en 5 minutos

## 📑 Índice de Documentación

| Documento | Descripción |
|-----------|-------------|
| **[QUICK_START.md](QUICK_START.md)** | 🚀 Inicio rápido en 5 minutos |
| **[EXECUTIVE_SUMMARY.md](EXECUTIVE_SUMMARY.md)** | 📊 Resumen ejecutivo completo |
| **[BUENAS_PRACTICAS.md](BUENAS_PRACTICAS.md)** | 💎 Patrones y buenas prácticas implementadas |
| **[DEPLOYMENT.md](DEPLOYMENT.md)** | 🌐 Guía completa de despliegue (Azure/Docker/IIS) |
| **[PRODUCTION_ENV.md](PRODUCTION_ENV.md)** | 🔐 Configuración de variables de entorno |
| **[QUALITY_CHECKLIST.md](QUALITY_CHECKLIST.md)** | ✅ Checklist de calidad y testing |

## 🚀 Características

- ✅ **Products**: CRUD completo de productos con autenticación en operaciones de modificación
- ✅ **Customers**: Gestión de clientes
- ✅ **Orders**: Creación y consulta de órdenes con validación de stock y transacciones
- ✅ **JWT Authentication**: Servicio dedicado de autenticación con tokens seguros
- ✅ **Swagger**: Documentación interactiva completa con soporte JWT
- ✅ **Entity Framework Core**: Acceso a datos con MySQL y manejo de errores
- ✅ **Arquitectura en capas**: Controllers → Services → Repositories (Patrón Repository)
- ✅ **DTOs**: Separación estricta de modelos de datos y validaciones
- ✅ **FluentValidation**: Validadores profesionales para todos los DTOs
- ✅ **DataAnnotations**: Validaciones en modelos
- ✅ **Logging estructurado**: ILogger en todos los servicios y repositorios
- ✅ **Manejo de errores**: Middleware global con logging detallado
- ✅ **Inyección de dependencias**: Uso correcto de interfaces y DI
- ✅ **Async/Await**: Todos los métodos asíncronos correctamente implementados
- ✅ **Seguridad**: CORS configurado, JWT validado, endpoints protegidos

## 📋 Requisitos Previos

- .NET 8 SDK
- MySQL Server o MariaDB
- Visual Studio 2022 / VS Code / Rider

## 🔧 Configuración Inicial

### 1. Clonar el repositorio

```bash
git clone <tu-repositorio>
cd Orders-TW
```

### 2. Configurar la cadena de conexión

Usar **User Secrets** para configurar la base de datos:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=ordersdb;user=root;password=tupassword"
```

### 3. Aplicar migraciones

```bash
dotnet ef database update
```

Si no existen las migraciones, crearlas:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Ejecutar el proyecto

**Opción A: Visual Studio**
1. Seleccionar el perfil **"Orders-TW"** o **"http"** en la barra de herramientas (NO usar "https")
2. Presionar F5 o click en el botón Run

**Opción B: Línea de comandos**
```bash
dotnet run --launch-profile http
```

**Opción C: Default**
```bash
dotnet run
```

La API estará disponible en: **http://localhost:5221**

⚠️ **IMPORTANTE**: Si se abre en `https://localhost:7131`, cambia el perfil de ejecución a "Orders-TW" o "http"

## 📚 Endpoints Disponibles

### 🔐 Autenticación

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/auth/login` | Obtener token JWT (user: admin, pass: admin) |

### 📦 Products

| Método | Endpoint | Autenticación | Descripción |
|--------|----------|---------------|-------------|
| GET | `/api/products` | No | **Obtener productos paginados** (query: page, pageSize) |
| GET | `/api/products/all` | No | Obtener TODOS los productos sin paginación |
| GET | `/api/products/{id}` | No | Obtener producto por ID |
| POST | `/api/products` | ✅ Sí | Crear producto |
| PUT | `/api/products/{id}` | ✅ Sí | Actualizar producto |
| DELETE | `/api/products/{id}` | ✅ Sí | Eliminar producto |

### 👥 Customers

| Método | Endpoint | Autenticación | Descripción |
|--------|----------|---------------|-------------|
| GET | `/api/customers` | No | **Obtener clientes paginados** (query: page, pageSize) |
| GET | `/api/customers/all` | No | Obtener TODOS los clientes sin paginación |
| GET | `/api/customers/{id}` | No | Obtener cliente por ID |
| POST | `/api/customers` | ✅ Sí | Crear cliente |

### 🛒 Orders

| Método | Endpoint | Autenticación | Descripción |
|--------|----------|---------------|-------------|
| GET | `/api/orders` | No | **Obtener órdenes paginadas** (query: page, pageSize) |
| GET | `/api/orders/all` | No | Obtener TODAS las órdenes sin paginación |
| GET | `/api/orders/{id}` | No | Obtener orden por ID |
| POST | `/api/orders` | ✅ Sí | Crear nueva orden |

### 📄 Paginación

Todos los endpoints GET principales soportan paginación:

**Parámetros:**
- `page` (opcional, default: 1) - Número de página
- `pageSize` (opcional, default: 10, max: 100) - Items por página

**Ejemplo:**
```
GET /api/products?page=2&pageSize=20
```

**Respuesta:**
```json
{
  "items": [...],
  "page": 2,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8,
  "hasPreviousPage": true,
  "hasNextPage": true
}
```

## 🧪 Probar la API

### Opción 1: Swagger UI

1. Abrir navegador en `http://localhost:5221`
2. Click en **Authorize**
3. Primero hacer login en `/api/auth/login` con:
   ```json
   {
     "username": "admin",
     "password": "admin"
   }
   ```
4. Copiar el token de la respuesta
5. En Authorize pegar: `Bearer {tu-token}`
6. Probar los endpoints

### Opción 2: Archivo .http (VS Code / Visual Studio)

Abrir el archivo `Orders-TW.http` y ejecutar las peticiones en orden:

1. **Login**: Ejecutar el endpoint de login
2. **Copiar el token** de la respuesta
3. **Pegar el token** en la variable `@token = tu_token_aqui`
4. Ejecutar los demás endpoints

### Opción 3: cURL

```bash
# 1. Obtener token
curl -X POST http://localhost:5221/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin"}'

# 2. Crear producto (usar el token obtenido)
curl -X POST http://localhost:5221/api/products \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {TU_TOKEN}" \
  -d '{"name":"Laptop","description":"Laptop HP","price":999.99,"stock":10}'
```

## 🏗️ Arquitectura del Proyecto

```
Orders-TW/
├── Controllers/          # Endpoints de la API
│   ├── ProductsController.cs
│   ├── CustomersController.cs
│   ├── OrdersController.cs
│   └── AuthController.cs
├── Services/             # Lógica de negocio
│   └── Interfaces/
├── Repositories/         # Acceso a datos
│   └── Interfaces/
├── DTOs/                 # Data Transfer Objects
│   ├── Products/
│   ├── Customers/
│   └── Orders/
├── Models/               # Entidades de BD
├── Data/                 # DbContext
├── Middleware/           # Manejo global de errores
└── Validators/           # Validaciones
```

## 🔒 Seguridad

- **JWT Token** con expiración de 60 minutos
- **CORS** configurado para desarrollo
- **HTTPS Redirection** deshabilitado para desarrollo (habilitar en producción)
- Las credenciales de BD se manejan con **User Secrets**

## ⚠️ Solución de Problemas

### Error: "Se abre en https://localhost:7131 en lugar de http://localhost:5221"

**Causa**: Está usando el perfil HTTPS en lugar de HTTP.

**Solución Visual Studio**:
1. Click en el dropdown junto al botón de Run (donde dice el nombre del proyecto)
2. Seleccionar **"Orders-TW"** o **"http"** (NO seleccionar "https (Producción)")
3. Ejecutar nuevamente

**Solución Línea de comandos**:
```bash
dotnet run --launch-profile http
```

**O modificar temporalmente Program.cs**:
- Verificar que `app.UseHttpsRedirection();` esté comentado (ya lo está)

### Error: "NetworkError when attempting to fetch resource"

**Causa**: HTTPS Redirection o problema de CORS.

**Solución**: 
- Usar `http://` NO `https://`
- Verificar que el proyecto esté corriendo en `http://localhost:5221`
- HTTPS Redirection está deshabilitado para desarrollo

### Error: "No se encontró la cadena de conexión"

**Solución**: Configurar User Secrets:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "tu-cadena-aqui"
```

### Error: "401 Unauthorized"

**Solución**: 
1. Primero hacer login en `/api/auth/login`
2. Copiar el token de la respuesta
3. Agregar header: `Authorization: Bearer {token}`

### Error de base de datos

**Solución**: 
```bash
dotnet ef database update
```

## 📊 Modelos de Datos

### Product
```json
{
  "id": 1,
  "name": "Laptop",
  "description": "Laptop HP EliteBook",
  "price": 999.99,
  "stock": 10
}
```

### Customer
```json
{
  "id": 1,
  "fullName": "Juan Pérez",
  "email": "juan@example.com"
}
```

### Order
```json
{
  "customerId": 1,
  "orderItems": [
    {
      "productId": 1,
      "quantity": 2
    }
  ]
}
```

## 🛠️ Tecnologías Utilizadas

- **.NET 8**
- **Entity Framework Core 8.0**
- **MySQL** con Pomelo.EntityFrameworkCore.MySql
- **JWT Bearer Authentication**
- **Swagger/OpenAPI**
- **FluentValidation**

## 📝 Notas de Desarrollo

- La autenticación JWT es básica (user: admin, pass: admin)
- En producción, implementar Identity o base de datos de usuarios
- El stock se reduce automáticamente al crear una orden
- Se valida stock disponible antes de crear órdenes
- El total de la orden se calcula automáticamente

## 📚 Documentación Adicional

- 📘 **[BUENAS_PRACTICAS.md](BUENAS_PRACTICAS.md)** - Documentación detallada de patrones y buenas prácticas implementadas
- 🚀 **[DEPLOYMENT.md](DEPLOYMENT.md)** - Guía completa de despliegue en Azure, Docker e IIS
- 🔐 **[PRODUCTION_ENV.md](PRODUCTION_ENV.md)** - Configuración de variables de entorno para producción
- 🏗️ **Arquitectura**: Clean Architecture con separación de responsabilidades

## 👨‍💻 Autor

Desarrollado para **PruebaTechTechy**  
Versión: 1.0  
Framework: .NET 8
