# Sistema de Detección de Fraude Petrolrios

## Descripción
Sistema de detección temprana de fraudes para la empresa petrolera Petrolrios. La plataforma identifica y alerta automáticamente sobre comportamientos anómalos en ingresos, facturación, consumo de gasolina y pérdidas de materia prima.

## Mejoras Implementadas
1. Principios SOLID Aplicados
1.1 Single Responsibility Principle (SRP)

Antes: El MotorDeteccion tenía múltiples responsabilidades (detectar diferentes tipos de fraude, crear alertas, aplicar lógica de negocio).
Después:

Creamos estrategias específicas para cada tipo de detección (EstrategiaDeteccionFacturas, EstrategiaDeteccionCombustible, EstrategiaDeteccionPrecios).
Cada clase tiene una única responsabilidad bien definida.
El MotorDeteccionMejorado solo coordina las estrategias.

1.2 Dependency Inversion Principle (DIP)

Antes: Los controladores dependían directamente de implementaciones concretas.
Después:

Creamos la interfaz IMotorDeteccion.
Los controladores ahora dependen de abstracciones, no de implementaciones concretas.
Esto facilita las pruebas unitarias y el mantenimiento.



2. Patrones de Diseño Implementados
2.1 Factory Method Pattern

Implementación: Creamos factories para cada tipo de alerta de fraude:

AlertaFraudeFactory (clase abstracta)
AlertaFacturacionDuplicadaFactory
AlertaDesaparicionCombustibleFactory
AlertaAnomaliaVentasFactory
AlertaDiscrepanciaPreciosFactory

Beneficio: Centraliza la creación de alertas y permite agregar nuevos tipos fácilmente.

2.2 Strategy Pattern

Implementación:

Interfaz IEstrategiaDeteccion define el contrato.
Cada tipo de detección es una estrategia concreta.
El MotorDeteccionMejorado puede usar diferentes estrategias dinámicamente.

Beneficio: Permite agregar nuevos algoritmos de detección sin modificar el código existente.

## Tecnologías
- ASP.NET Core MVC 6.0
- Entity Framework Core
- SQL Server
- Bootstrap 5
- jQuery y DataTables

## Características Principales
- Gestión de estaciones, ventas, facturas e inventarios
- Motor de detección de anomalías
- Alertas automáticas
- Dashboard para supervisión
- Validación avanzada en el backend

## Requisitos
- Visual Studio 2022 o Visual Studio Code
- SQL Server (STEVEN-ALIENWAR\SQLTRABAJO)
- .NET 6.0 SDK

## Instalación

1. Clonar el repositorio


## Links
- Link GoogleDrive Video Principios SOLID Y Patrones De Diseno: https://drive.google.com/file/d/1J7O8pDc7hxh25OAPsCADwmns7hTsSDg2/view?usp=sharing
- Link GoogleDrive(Viejo): https://drive.google.com/file/d/1Ro5EpqhSlp12Ix-ZmYOkQzoGxhEYoNgu/view?usp=sharing
- Link Youtube(Viejo): https://youtu.be/SVXuwTBmaNE
- link GitHub: https://github.com/StevenCarrilloLoor/PetrolriosFraudeDetection.git
- Link Deploy: https://petrolriosfraudedetection.onrender.com

