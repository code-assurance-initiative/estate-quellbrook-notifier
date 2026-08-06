# Architecture — notifier

## Context

The notifier is one of five parts of the Quellbrook operations platform and the only one that talks to consignees.

```mermaid
C4Context
  title System context — notifier
  Person_Ext(consignee, "Consignee", "Receives the parcels and the notifications")
  System_Boundary(platform, "Quellbrook operations platform") {
    System(orders, "Order service", "estate-quellbrook-orders")
    System(dispatch, "Dispatch service", "estate-quellbrook-dispatch")
    System(notifier, "Notifier", "this repository")
  }
  System_Ext(email, "E-mail provider", "Transactional e-mail API")
  Rel(orders, notifier, "orders.order-placed.v1", "RabbitMQ")
  Rel(notifier, email, "Sends e-mail", "HTTPS")
  Rel(email, consignee, "Delivers e-mail")
```

## Containers

```mermaid
C4Container
  title Containers — notifier
  ContainerQueue(broker, "Event broker", "RabbitMQ", "queue notifier.events")
  Container_Boundary(service, "Notifier") {
    Container(worker, "Notifier worker", ".NET 10 worker", "Consumes events, keeps recipients, sends notifications")
    ContainerDb(db, "Notifier database", "PostgreSQL 16", "recipients, processed_messages, notification_log")
  }
  System_Ext(email, "E-mail provider", "SendGrid v3 API")
  Rel(broker, worker, "Delivers order events", "AMQPS")
  Rel(worker, db, "Reads and writes", "EF Core / Npgsql")
  Rel(worker, email, "POST v3/mail/send", "HTTPS, API key")
```

## Inside the worker

| Folder | Responsibility |
|---|---|
| `Messaging/` | the RabbitMQ consumer, the inbox, the notifier's copies of the consumed message types |
| `Notifications/` | the handlers per event, the notification service and log, templates, contact masking |
| `Channels/` | the provider clients behind `IEmailSender` (resilience: timeouts, retries, circuit breaker) |
| `Persistence/` | EF Core context and migrations |
| `Hosting/` | service registration, OpenTelemetry, the heartbeat the probes read |

The worker serves no HTTP. Kubernetes reads its health from a heartbeat file (`/tmp/heartbeat`) that is rewritten
after each health-check round.

## Message contracts

| Routing key | Fields the notifier reads | Producer |
|---|---|---|
| `orders.order-placed.v1` | order id, consignee name, contact e-mail and phone | order service |
