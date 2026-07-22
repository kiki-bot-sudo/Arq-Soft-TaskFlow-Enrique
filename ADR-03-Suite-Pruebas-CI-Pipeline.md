# ADR-03: Suite de Pruebas Automatizadas y Pipeline CI/CD

| Campo   | Valor              |
|---------|--------------------| 
| Autor   | Enrique Zavala     |
| Fecha   | 17/06/2026         |
| Estado  | Aprobado           |
| Basado en | ADR-02           |

---

## Contexto

Como parte de la mejora continua del sistema **TaskFlow**, se requiere implementar una suite de pruebas automatizadas con xUnit y configurar un pipeline de Integración Continua que ejecute automáticamente estas pruebas en cada cambio al repositorio.

---

## Decisión

Se implementó:

1. **Suite de Pruebas xUnit** con cobertura en 3 clases críticas
2. **Workflow GitHub Actions** que compila y ejecuta tests en cada push
3. **Proyecto TaskFlow.Tests** separado con estructura Arrange-Act-Assert

---

## Clases Testeadas

### 1. **ActivityService** ✅
**Ubicación:** `TaskFlow.Application/Services/ActivityService.cs`

**Por qué se eligió:**
- Es el corazón de la lógica de aplicación
- Orquesta todas las operaciones sobre actividades
- Interactúa con repositorio (punto de fallo común)

**Pruebas implementadas:**
- `GetTodayActivitiesAsync_ReturnsActivities_WhenDataExists` - Verifica que retorna actividades del día
- `CreateActivityAsync_CreatesActivity_WithTimestamp` - Valida que se asigna timestamp
- `DeleteActivityAsync_ReturnsFalse_WhenActivityNotFound` - Maneja caso de no encontrado

**Cobertura:** 3 métodos críticos

---

### 2. **TaskService** ✅
**Ubicación:** `TaskFlow.Application/Services/TaskService.cs`

**Por qué se eligió:**
- Maneja lógica de sub-tareas dentro de actividades
- Usa repositorio inyectado (testeable con mocks)
- Casos de uso variados (crear, actualizar, leer)

**Pruebas implementadas:**
- `GetTasksByActivityAsync_ReturnsTasks_WhenActivityHasTasks` - Obtiene tareas de una actividad
- `CreateTaskAsync_SetsCreatedAtTimestamp` - Valida timestamps
- `UpdateTaskAsync_UpdatesTaskProperties` - Actualización de propiedades

**Cobertura:** 3 métodos del CRUD

---

### 3. **CalculadoraService** ✅
**Ubicación:** Operaciones matemáticas básicas

**Por qué se eligió:**
- Lógica pura y determinística
- Fácil de testear sin dependencias complejas
- Cubre casos normales y excepciones

**Pruebas implementadas:**
- `Sumar_ReturnsSumOfTwoNumbers` - Caso positivo
- `Restar_ReturnsSubtractionOfTwoNumbers` - Resta correcta
- `Dividir_ThrowsException_WhenDividingByZero` - Manejo de excepciones

**Cobertura:** Operaciones base

---

## Estrategia de Testing: Arrange-Act-Assert

Todas las pruebas siguen el patrón AAA:

```csharp
[Fact]
public async Task GetTodayActivitiesAsync_ReturnsActivities_WhenDataExists()
{
    // ARRANGE: Preparar datos y mocks
    var activities = new List<Activity> { ... };
    _mockRepository.Setup(r => r.GetActivitiesByDateAsync(It.IsAny<DateTime>()))
        .ReturnsAsync(activities);

    // ACT: Ejecutar la acción
    var result = await _service.GetTodayActivitiesAsync();

    // ASSERT: Verificar el resultado
    Assert.NotNull(result);
    Assert.Equal(2, result.Count());
}
```

---

## Pipeline CI/CD - GitHub Actions

### Workflow: `tests.yml`

**Trigger:** Push a ramas, Pull Requests

**Pasos:**
1. Checkout del código
2. Setup de .NET 8
3. Restore de dependencias
4. Build del proyecto
5. Ejecución de tests xUnit
6. Upload de resultados

**Archivo:** `.github/workflows/tests.yml`

---

## Estructura del Proyecto de Pruebas

```
TaskFlow.Tests/
├── TaskFlow.Tests.csproj
├── Usings.cs
├── ActivityServiceTests.cs
├── TaskServiceTests.cs
└── CalculadoraServiceTests.cs
```

**Dependencias:**
- xunit 2.6.2
- xunit.runner.visualstudio
- Microsoft.NET.Test.Sdk

---

## Beneficios de esta Implementación

1. **Confiabilidad:** Tests automáticos previenen regresos en código crítico
2. **Velocidad:** Pipeline CI ejecuta en < 2 minutos
3. **Feedback rápido:** Desarrolladores ven errores inmediatamente
4. **Documentación viva:** Tests sirven como ejemplos de uso correcto
5. **Confianza en deployment:** Solo código testeado llega a producción

---

## Próximas Mejoras

- [ ] Aumentar cobertura a 80%+
- [ ] Agregar integration tests con BD real
- [ ] Implement Code Coverage reporting
- [ ] Notificaciones en Slack para test failures
- [ ] Performance benchmarks

---

## Conclusión

La suite de pruebas xUnit y el pipeline CI/CD automatizan la verificación de calidad, permitiendo desarrollo más rápido y seguro sin sacrificar estabilidad.
