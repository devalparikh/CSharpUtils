# Store Example

A sample of a distributed systems application to handle online store order.

When creating a large-scale system to handle orders, 
often times a system is composed of several smaller systems that are orchestrated.

This is to achieve: 

- Separation of concerns
- Horizontal scaling (instances, shards, etc)
- Flexibility (stacks, tools, etc)
- Fault tolerance
- Maintenance

However, this introduces a new set of challenges when orchestrating many components to achieve a single larger task.

Challenges such as:

- Data inconsistencies. Consistency in the sense of: 
  - CAP: between systems - network partition causes 
  - (not) ACID: within a system - a failure during a transaction   
- Partial executions of task (atomicity)
- Tightly coupled faults

The following sections will solve these issues.

## Domain Events Pattern
Domain events are used to encapsulate business logic events the to domain.
Domain events are typically used to capture state changes.

## Outbox Pattern
Outbox pattern ensures that events (domain events) that need to be processed/sent from a transactional system (like a database or message queue) are stored reliably before they are published or sent to an external system (e.g., message queues, other services, etc.). 

This ensures that the system maintains strong consistency between the database and the messaging system even in case of failures, crashes, or retries.

Example of the issue being solved:
- the task is fulfilling a return of an order
- 2 events must be processed:
  - customer's money is returned
  - order shipping is cancelled
- if these events are partially complete (1 completes, the other fails), this results in a corrupted state which can be catastrophic for large scale systems

With outbox pattern the domain state change and domain event publishing are atomic.

Pattern usage:

1. A background job can consume the unprocessed outbox messages (the domain event + metadata) publish messages to a reliable message queue 
2. The 2 systems that handle money return and shipping cancellation can consume these events in a reliable and fault tolerant way (dead letter queue and idempotency)

This solved a few issues:
- services are now decouples
- guarantee of atomic transactions

## Fault Tolerance Within State
Can a transaction fall between the gaps within a node processing it?

- Caller -> Node: a transaction can fail between client and server due to many reasons
  - reasons:
    - DNS resolution
    - Network access issues, usually results in a connection timeout
    - SSL/TLS handshake failure
    - etc
  - results:
    - not corrupt state as caller gets indication of failure
- Node process - internal failure before/while creating domain event
  - Caller gets failure and all changes are rolled back (atomic)
- Node process - domain event created but failed to create/persist outbox message
  - Caller gets failure and all changes are rolled back (atomic)
- Node process - outbox message created successfully, background service fails during processing
  - Another background process execution will process it since the outbox message "isProcessed" flag is still false
  - note: use idempotency for exactly once tasks 
- Background service - publishes outbox message to message queue but fails before updating outbox message processed flag
  - Another background process execution will process it since the outbox message "isProcessed" flag is still false
  - note: use idempotency for exactly once tasks

## Stateful Architecture (not in this example)
Stateful architecture is not needed here since a message queue is used.
It may be used here if the caller can not perform retries on errors for whatever reason (such as expensive operations) and is critical to process the request at least once.

Stateful architecture may be useful when message queue is not an option (interaction with a HTTP system). 
When a client does not handle server responses and a critical state change is accepted by the node but crashes before persisting the state change.

More information on stateful:
- https://learn.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-introduction#stateful-reliable-services
- https://dzimchuk.net/service-fabric-stateful-services/
