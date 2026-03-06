# ?? Solución de Problemas - Base de Datos

## ? Error: "Unknown column 'p.Description' in 'field list'"

### Causa
El modelo `Product` tiene el campo `Description` pero la base de datos no tiene esa columna.

### ? Soluciones (Elegir una)

#### Opción 1: Ejecutar Script SQL Directo (RÁPIDO ?)

1. Abrir tu cliente MySQL (Workbench, phpMyAdmin, etc.)
2. Ejecutar el archivo `add-description-column.sql`:

```sql
USE ordersdb; -- Cambiar por tu base de datos

ALTER TABLE Products 
ADD COLUMN Description VARCHAR(500) NULL;
```

#### Opción 2: Recrear Base de Datos (LIMPIO ??)

```bash
# 1. Borrar base de datos actual
DROP DATABASE ordersdb;

# 2. Crear nueva
CREATE DATABASE ordersdb CHARACTER SET utf8mb4;

# 3. Ejecutar migraciones
dotnet ef database update --project Orders-TW
```

#### Opción 3: Migración Manual (PROFESIONAL ??)

```bash
# Ver migraciones aplicadas
dotnet ef migrations list --project Orders-TW

# Aplicar migración específica
dotnet ef database update AddDescriptionToProduct --project Orders-TW
```

## ?? Datos de Prueba

### Automático (Al ejecutar en Development)
El proyecto carga automáticamente:
- ? 10 productos
- ? 8 clientes  
- ? 5 órdenes de ejemplo

### Manual
Si necesitas cargar datos manualmente, puedes llamar:

```csharp
// En Program.cs está configurado para cargar automáticamente
DataSeeder.SeedData(context);
```

## ?? Verificar que Funciona

```bash
# 1. Ejecutar proyecto
dotnet run --project Orders-TW --launch-profile http

# 2. Abrir Swagger
# http://localhost:5221

# 3. Probar GET /api/products
# Deberías ver 10 productos con Description
```

## ?? Si aún tienes problemas

### Verificar conexión
```bash
# Tu connection string en user secrets:
dotnet user-secrets list --project Orders-TW

# Debería mostrar:
# ConnectionStrings:DefaultConnection = server=localhost;...
```

### Verificar tabla
```sql
-- Ver estructura de tabla Products
DESCRIBE Products;

-- Debería incluir:
-- | Description | varchar(500) | YES  |     | NULL    |       |
```

## ? Checklist

- [ ] Base de datos creada
- [ ] Migración AddDescriptionToProduct aplicada
- [ ] Columna Description existe en Products
- [ ] Datos de prueba cargados
- [ ] API responde correctamente en /api/products
