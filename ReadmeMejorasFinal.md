# Sistema de Detección de Fraude Petrolrios - Documentación Completa

## 📋 Descripción General
Sistema de detección temprana de fraudes para la empresa petrolera Petrolrios. La plataforma identifica y alerta automáticamente sobre comportamientos anómalos en ingresos, facturación, consumo de gasolina y pérdidas de materia prima.

## 🏗️ Arquitectura del Sistema

### Backend - ASP.NET Core MVC + Web API
- **Framework**: ASP.NET Core 9.0
- **Base de Datos**: SQL Server
- **ORM**: Entity Framework Core
- **Documentación API**: Swagger/OpenAPI

### Frontend - React
- **Framework**: React 18.2
- **Estilos**: Tailwind CSS
- **Gráficos**: Recharts
- **Iconos**: Lucide React
- **HTTP Client**: Fetch API

## 🎯 Mejoras Implementadas

### 1. Principios SOLID Aplicados

#### 1.1 Single Responsibility Principle (SRP)
- **Antes**: El `MotorDeteccion` tenía múltiples responsabilidades
- **Después**: 
  - Creamos estrategias específicas para cada tipo de detección
  - `EstrategiaDeteccionFacturas`: Solo detecta facturas duplicadas
  - `EstrategiaDeteccionCombustible`: Solo detecta pérdidas de combustible
  - `EstrategiaDeteccionPrecios`: Solo detecta discrepancias de precios

#### 1.2 Open/Closed Principle (OCP)
- El sistema está abierto para extensión pero cerrado para modificación
- Se pueden agregar nuevas estrategias de detección sin modificar el código existente

#### 1.3 Dependency Inversion Principle (DIP)
- **Antes**: Los controladores dependían de implementaciones concretas
- **Después**: 
  - Creamos la interfaz `IMotorDeteccion`
  - Los controladores dependen de abstracciones, no de implementaciones concretas
  - Facilita las pruebas unitarias y el mantenimiento

### 2. Patrones de Diseño Implementados

#### 2.1 Factory Method Pattern
```csharp
AlertaFraudeFactory (clase abstracta)
├── AlertaFacturacionDuplicadaFactory
├── AlertaDesaparicionCombustibleFactory
├── AlertaAnomaliaVentasFactory
└── AlertaDiscrepanciaPreciosFactory
```
**Beneficio**: Centraliza la creación de alertas y permite agregar nuevos tipos fácilmente

#### 2.2 Strategy Pattern
```csharp
IEstrategiaDeteccion (interfaz)
├── EstrategiaDeteccionFacturas
├── EstrategiaDeteccionCombustible
└── EstrategiaDeteccionPrecios
```
**Beneficio**: Permite cambiar algoritmos de detección dinámicamente

## 📡 API REST Implementada

### Endpoints Principales

#### 1. Estaciones API
```
GET    /api/EstacionesApi          - Listar todas las estaciones
GET    /api/EstacionesApi/{id}     - Obtener estación por ID
POST   /api/EstacionesApi          - Crear nueva estación
PUT    /api/EstacionesApi/{id}     - Actualizar estación
DELETE /api/EstacionesApi/{id}     - Eliminar estación (soft delete)
```

#### 2. Alertas API
```
GET    /api/AlertasApi             - Listar alertas (con filtros)
GET    /api/AlertasApi/estadisticas - Obtener estadísticas
POST   /api/AlertasApi/analizar    - Analizar fecha específica
PUT    /api/AlertasApi/{id}/resolver - Resolver alerta
```

#### 3. Dashboard API
```
GET    /api/DashboardApi/resumen    - Métricas generales
GET    /api/DashboardApi/ventas-por-dia - Ventas por día
GET    /api/DashboardApi/top-estaciones - Top 5 estaciones
GET    /api/DashboardApi/alertas-recientes - Últimas alertas
GET    /api/DashboardApi/alertas-por-estacion - Alertas por estación
```

## 🖥️ Aplicación React

### Características Implementadas

#### 1. Dashboard Principal
- **Métricas en tiempo real**: Estaciones activas, alertas pendientes, ventas del mes
- **Gráficos interactivos**: 
  - Línea de tiempo de ventas
  - Barras de top estaciones
- **Tabla de alertas recientes**
- **Ranking de estaciones por alertas generadas**

#### 2. Gestión de Alertas
- **Listado completo** con paginación
- **Filtros** por tipo, estado y fecha
- **Acciones rápidas**: Confirmar fraude o marcar como falso positivo
- **Análisis de fechas**: Ejecutar motor de detección para fechas específicas

#### 3. Gestión de Estaciones
- **Visualización** de todas las estaciones
- **Estados**: Activas/Inactivas
- **Información detallada**: Código, ubicación

### Componentes Principales
```javascript
App.js
├── Dashboard
│   ├── Métricas (Cards)
│   ├── Gráfico de Ventas (LineChart)
│   ├── Top Estaciones (BarChart)
│   ├── Alertas Recientes (Table)
│   └── Estaciones con Más Alertas (Table)
├── Alertas
│   ├── Barra de herramientas
│   └── Tabla de alertas
└── Estaciones
    └── Grid de estaciones
```

## 🚀 Instalación y Ejecución

### Requisitos Previos
- Visual Studio 2022 o VS Code
- .NET 9.0 SDK
- SQL Server
- Node.js (v14 o superior)
- npm o yarn

### Backend (ASP.NET Core)

1. **Clonar el repositorio**
```bash
git clone https://github.com/tuusuario/PetrolriosFraudeDetection.git
cd PetrolriosFraudeDetection
```

2. **Configurar la base de datos**
- Actualizar la cadena de conexión en `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=PetrolriosFraudeDetection;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. **Ejecutar migraciones**
```bash
dotnet ef database update
```

4. **Ejecutar el proyecto**
```bash
dotnet run
```

El backend estará disponible en:
- HTTP: `http://localhost:5073`
- HTTPS: `https://localhost:7260`
- Swagger: `http://localhost:5073/swagger`

### Frontend (React)

1. **Navegar a la carpeta del cliente**
```bash
cd ../petrolrios-client
```

2. **Instalar dependencias**
```bash
npm install
```

3. **Configurar la URL del API**
En `src/App.js`, verificar:
```javascript
const API_BASE_URL = 'http://localhost:5073/api';
```

4. **Ejecutar la aplicación**
```bash
npm start
```

La aplicación estará disponible en `http://localhost:3000`

## 📊 Estructura del Proyecto

### Backend
```
PetrolriosFraudeDetection/
├── Controllers/
│   ├── Api/
│   │   ├── EstacionesApiController.cs
│   │   ├── AlertasApiController.cs
│   │   └── DashboardApiController.cs
│   └── MVC Controllers...
├── Models/
│   └── Entities/
├── Services/
│   ├── MotorDeteccionMejorado.cs
│   └── Estrategias/
├── Factories/
│   └── AlertaFraudeFactory.cs
├── Interfaces/
│   ├── IMotorDeteccion.cs
│   └── IEstrategiaDeteccion.cs
└── Data/
    └── ApplicationDbContext.cs
```

### Frontend
```
petrolrios-client/
├── src/
│   ├── App.js
│   ├── index.js
│   └── index.css
├── public/
│   └── index.html
├── package.json
└── tailwind.config.js
```

## 🔧 Configuración CORS

El backend está configurado para aceptar peticiones desde `http://localhost:3000`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});
```

## 📈 Funcionalidades del Sistema

### 1. Detección de Fraudes
- **Facturación Duplicada**: Detecta facturas con montos idénticos sospechosos
- **Desaparición de Combustible**: Identifica pérdidas no justificadas en inventario
- **Anomalías Ventas-Movimientos**: Compara ventas registradas vs. movimientos físicos
- **Discrepancia de Precios**: Detecta precios fuera del rango oficial

### 2. Gestión de Alertas
- Estados: Pendiente, Confirmado, Falso Positivo
- Resolución con registro de usuario y fecha
- Historial completo de alertas

### 3. Dashboard Analítico
- Métricas en tiempo real
- Análisis de tendencias
- Identificación de estaciones problemáticas
- Visualización de datos históricos

## 🛠️ Tecnologías Utilizadas

### Backend
- **ASP.NET Core 9.0**: Framework principal
- **Entity Framework Core**: ORM
- **SQL Server**: Base de datos
- **Swagger**: Documentación API
- **Dependency Injection**: IoC container nativo

### Frontend
- **React 18.2**: Library UI
- **Tailwind CSS**: Framework CSS
- **Recharts**: Librería de gráficos
- **Lucide React**: Iconos
- **Fetch API**: Cliente HTTP

## 🔐 Consideraciones de Seguridad

1. **CORS** configurado para orígenes específicos
2. **Validación** en backend y frontend
3. **Soft Delete** para mantener integridad referencial
4. **DTOs** para no exponer entidades directamente

## 📝 Mejoras Futuras Sugeridas

1. **Autenticación y Autorización**
   - Implementar JWT tokens
   - Roles de usuario (Admin, Analista, Visor)

2. **Notificaciones en Tiempo Real**
   - SignalR para alertas instantáneas
   - Email/SMS para alertas críticas

3. **Machine Learning**
   - Predicción de fraudes con ML.NET
   - Análisis predictivo de patrones

4. **Reportes Avanzados**
   - Exportación a PDF/Excel
   - Dashboards personalizables
   - Métricas KPI avanzadas

5. **Auditoría**
   - Log de todas las acciones
   - Trazabilidad completa

## 👥 Contribuidores
- Steven Carrillo Loor

## 📄 Licencia
Este proyecto es parte del trabajo académico para la Universidad de las Américas (UDLA).

---

**Última actualización**: Enero 2025