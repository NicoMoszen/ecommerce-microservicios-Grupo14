# E-Commerce Microservicios - Grupo 14

## Requisitos

- .NET 10 SDK
- Visual Studio 2026 Community

## Ejecución

Clonar el repositorio y abrir `MiniApi.slnx`. Configurar múltiples proyectos de inicio y ejecutar con F5, o alternativamente ejecutar `dotnet run` en cada carpeta de microservicio.

Users.API debe iniciarse antes que Notifications.API ya que esta última realiza llamadas HTTP a Users para validar la existencia del usuario destinatario.

## Puertos

- Users.API: https://localhost:7087/swagger
- Products.API: https://localhost:7010/swagger
- Cart.API: https://localhost:7008/swagger
- Orders.API: https://localhost:7125/swagger
- Notifications.API: https://localhost:7149/swagger

## Endpoints

### Users.API
- POST /api/users/register
- POST /api/users/login
- GET /api/users/{id}

### Products.API
- GET /api/products
- GET /api/products/{id}
- POST /api/products
- PUT /api/products/{id}
- DELETE /api/products/{id}

### Cart.API
- GET /api/cart/{userId}
- POST /api/cart
- PUT /api/cart/{itemId}
- DELETE /api/cart/{userId}

### Orders.API
- GET /api/orders
- GET /api/orders/{id}
- POST /api/orders
- PUT /api/orders/{id}/estado

### Notifications.API
- POST /api/notifications/send
- GET /api/notifications/{userId}

## Base de datos

Cada microservicio utiliza una base de datos SQLite independiente que se inicializa automáticamente al arrancar la aplicación.

## Health Checks

Todos los microservicios exponen /health, /health/ready y /health/live.
