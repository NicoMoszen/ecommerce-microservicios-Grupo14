# E-Commerce Microservicios - Grupo 14

Desarrollado con .NET 10.

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

## Comunicación entre servicios

Los microservicios se comunican entre sí mediante HTTP para validar datos en tiempo real. Orders.API consulta a Users.API y Products.API al crear una orden. Cart.API consulta a Products.API para verificar stock. Notifications.API consulta a Users.API para verificar el usuario destinatario. Products.API consulta a Orders.API antes de eliminar un producto para verificar que no tenga órdenes activas.

**Orden de arranque recomendado:** Users.API y Products.API primero, luego el resto.

## Infraestructura compartida

Todos los microservicios utilizan ECommerce.Shared, una librería interna que centraliza Serilog, el middleware de Correlation ID y las extensiones de logging.

## Base de datos

Cada microservicio utiliza una base de datos SQLite independiente que se inicializa automáticamente al arrancar.

## Health Checks

Todos los microservicios exponen /health, /health/ready y /health/live con respuesta JSON que incluye el estado de la base de datos.
