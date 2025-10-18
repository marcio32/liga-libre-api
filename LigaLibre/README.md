# Liga Libre API ⚽

API REST para la gestión de una liga de fútbol, desarrollada con .NET 8 y arquitectura limpia (Clean Architecture).

## 📋 Descripción

Liga Libre es una API completa para administrar una liga de fútbol que permite gestionar clubes, jugadores, árbitros, partidos y estadísticas. Implementa autenticación JWT, caché con Redis, logging con Serilog, y validaciones con FluentValidation.

## 🏗️ Arquitectura

El proyecto sigue los principios de Clean Architecture con separación en capas:

```
LigaLibre/
├── LigaLibre.API/          # Capa de presentación (Controllers, Middlewares)
├── LigaLibre.Application/  # Lógica de aplicación (Services, DTOs, Validators)
├── LigaLibre.Domain/        # Entidades de dominio e interfaces
├── LigaLibre.Infrastructure/# Implementación de infraestructura (Repositories, DbContext)
└── LigaLibre.Tests/         # Pruebas unitarias
```

## 🚀 Tecnologías

- **.NET 8.0**
- **Entity Framework Core 8.0** - ORM
- **SQL Server** - Base de datos
- **Redis** - Caché distribuido
- **JWT Bearer** - Autenticación
- **Serilog** - Logging
- **FluentValidation** - Validación de datos
- **Mapster** - Mapeo de objetos
- **Swagger/OpenAPI** - Documentación de API
- **xUnit** - Testing
- **AWS SQS** - Mensajería (integración)

## 📦 Características

### Módulos Principales

- **Clubes**: CRUD completo de clubes con información de estadio y socios
- **Jugadores**: Gestión de jugadores con estadísticas (goles, asistencias, tarjetas)
- **Árbitros**: Administración de árbitros con categorías y licencias
- **Partidos**: Programación y seguimiento de partidos con resultados
- **Estadísticas**: Consulta de estadísticas generales de la liga
- **Autenticación**: Sistema de login y registro con JWT

### Funcionalidades Técnicas

- ✅ Autenticación y autorización con JWT
- ✅ Caché distribuido con Redis
- ✅ Rate limiting personalizado
- ✅ Logging estructurado con Serilog
- ✅ Manejo global de errores
- ✅ Health checks
- ✅ CORS configurado
- ✅ Validaciones con FluentValidation
- ✅ Documentación con Swagger

## 🛠️ Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (o Docker)
- [Redis](https://redis.io/) (o Docker)

## ⚙️ Configuración

### 1. Clonar el repositorio

```bash
git clone <repository-url>
cd LigaLibre
```

### 2. Configurar la base de datos

Actualiza la cadena de conexión en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,11433;Database=LigaLibre;User Id=sa;Password=<tu-password>;MultipleActiveResultSets=true;TrustServerCertificate=True;",
    "Redis": "localhost:6379"
  }
}
```

### 3. Ejecutar migraciones

```bash
cd LigaLibre
dotnet ef database update
```

### 4. Ejecutar con Docker (Opcional)

```bash
# SQL Server
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P@ssw0rd2025!" -p 11433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# Redis
docker run -d -p 6379:6379 redis:latest
```

### 5. Ejecutar la aplicación

```bash
dotnet run --project LigaLibre
```

La API estará disponible en: `https://localhost:7000` (o el puerto configurado)

## 📚 Documentación API

Una vez ejecutada la aplicación, accede a Swagger en:

```
https://localhost:7000/swagger
```

### Endpoints Principales

#### Autenticación
- `POST /api/auth/register` - Registrar usuario
- `POST /api/auth/login` - Iniciar sesión

#### Clubes
- `GET /api/club` - Listar clubes
- `GET /api/club/{id}` - Obtener club por ID
- `POST /api/club` - Crear club
- `PUT /api/club/{id}` - Actualizar club
- `DELETE /api/club/{id}` - Eliminar club

#### Jugadores
- `GET /api/players` - Listar jugadores
- `GET /api/players/{id}` - Obtener jugador por ID
- `GET /api/players/club/{clubId}` - Jugadores por club
- `POST /api/players` - Crear jugador
- `PUT /api/players/{id}` - Actualizar jugador
- `DELETE /api/players/{id}` - Eliminar jugador

#### Árbitros
- `GET /api/referee` - Listar árbitros
- `GET /api/referee/{id}` - Obtener árbitro por ID
- `POST /api/referee` - Crear árbitro
- `PUT /api/referee/{id}` - Actualizar árbitro
- `DELETE /api/referee/{id}` - Eliminar árbitro

#### Partidos
- `GET /api/match` - Listar partidos
- `GET /api/match/{id}` - Obtener partido por ID
- `POST /api/match` - Crear partido
- `PUT /api/match/{id}` - Actualizar partido
- `DELETE /api/match/{id}` - Eliminar partido

#### Estadísticas
- `GET /api/statistics` - Obtener estadísticas generales

#### Health Check
- `GET /api/health` - Estado de la aplicación

## 🧪 Pruebas

Ejecutar todas las pruebas:

```bash
dotnet test
```

Ejecutar con cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 🔐 Autenticación

La API utiliza JWT Bearer tokens. Para acceder a endpoints protegidos:

1. Registra un usuario en `/api/auth/register`
2. Inicia sesión en `/api/auth/login` para obtener el token
3. Incluye el token en el header: `Authorization: Bearer <token>`

## 📝 Estructura de Entidades

### Club
- Información básica del club
- Estadio y capacidad
- Relación con jugadores y partidos

### Player
- Datos personales y físicos
- Estadísticas (goles, asistencias, tarjetas)
- Relación con club

### Referee
- Información del árbitro
- Categoría y licencia
- Relación con partidos

### Match
- Equipos local y visitante
- Resultado y estadio
- Estado del partido (Programado, En Curso, Finalizado, Cancelado)
- Árbitro asignado

## 🔧 Middlewares

- **ErrorLoggingMiddleware**: Captura y registra errores globales
- **RateLimitingMiddleware**: Limita peticiones por IP
- **RequestLoggingMiddleware**: Registra todas las peticiones HTTP

## 📊 Logging

Los logs se almacenan en:
- Consola (desarrollo)
- Archivos en `/logs/log{fecha}.txt` (rotación diaria)

## 🤝 Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT.

## 👥 Autores

- Tu Nombre - Desarrollo inicial

## 🙏 Agradecimientos

- Comunidad .NET
- Contribuidores del proyecto
