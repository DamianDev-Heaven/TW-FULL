# Tests Unitarios - Orders API

## ?? Tests Implementados

### ProductServiceTests
- ? GetAllAsync_ReturnsAllProducts
- ? GetByIdAsync_ExistingId_ReturnsProduct
- ? GetByIdAsync_NonExistingId_ReturnsNull
- ? CreateAsync_ValidProduct_ReturnsCreatedProduct
- ? UpdateAsync_ExistingProduct_ReturnsUpdatedProduct
- ? UpdateAsync_NonExistingProduct_ReturnsNull
- ? DeleteAsync_ExistingProduct_ReturnsTrue
- ? DeleteAsync_NonExistingProduct_ReturnsFalse

### OrderServiceTests
- ? CreateAsync_ValidOrder_CalculatesTotal
- ? CreateAsync_InsufficientStock_ThrowsException
- ? CreateAsync_NonExistentCustomer_ThrowsException
- ? CreateAsync_NonExistentProduct_ThrowsException
- ? CreateAsync_EmptyOrderItems_ThrowsException

## ?? Ejecutar Tests

```bash
# Ejecutar todos los tests
dotnet test

# Con más detalles
dotnet test --logger "console;verbosity=detailed"

# Con cobertura
dotnet test /p:CollectCoverage=true
```

## ?? Cobertura

Los tests cubren:
- ? ProductService: 100% métodos críticos
- ? OrderService: 100% reglas de negocio
- ? Validaciones de stock
- ? Cálculo de totales
- ? Manejo de excepciones

## ?? Tecnologías

- xUnit
- Moq (para mocks)
- FluentAssertions (opcional)
