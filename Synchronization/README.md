# Synchronization Primitives in CSharpUtils

This folder contains custom synchronization primitives designed to facilitate safe concurrent programming in .NET applications. The components here provide fine-grained control over concurrent access to shared resources, with a focus on reader-writer lock patterns.

## Overview

The `Synchronization` components enable multiple threads to safely access shared data, allowing for high concurrency on reads while ensuring exclusive access for writes. These primitives are useful in scenarios where read operations vastly outnumber writes, improving performance over simple mutual exclusion locks.

## Lock Implementations

### 1. `ReaderWriterLock`
- **Type:** Synchronous reader-writer lock
- **Mechanism:**
  - Uses `Monitor` (i.e., `lock`, `Monitor.Wait`, `Monitor.PulseAll`) for thread synchronization.
  - Allows multiple concurrent readers, but only one writer at a time.
  - Writers are given priority to avoid starvation.
- **Reference:** [`Synchronization/ReaderWriterLock.cs`](./ReaderWriterLock.cs)

### 2. `ReaderWriterLockSemaphore`
- **Type:** Synchronous reader-writer lock using semaphores
- **Mechanism:**
  - Utilizes `SemaphoreSlim` to manage access for readers and writers.
  - Ensures that only one writer or multiple readers can access the resource concurrently.
  - Can be more scalable than monitor-based locks under high contention.
- **Reference:** [`Synchronization/ReaderWriterLockSemaphore.cs`](./ReaderWriterLockSemaphore.cs)

### 3. `ReaderWriterLockAsync`
- **Type:** Asynchronous reader-writer lock
- **Mechanism:**
  - Built on top of `SemaphoreSlim` to support `async`/`await` patterns.
  - Enables asynchronous code to safely acquire and release locks without blocking threads.
  - Suitable for modern .NET applications using asynchronous workflows.
- **Reference:** [`Synchronization/ReaderWriterLockAsync.cs`](./ReaderWriterLockAsync.cs)

## Underlying Primitives

- **`SemaphoreSlim`:**
  - Lightweight semaphore for controlling access to a resource pool.
  - Used in `ReaderWriterLockSemaphore` and `ReaderWriterLockAsync` for efficient thread signaling.
- **`Monitor.Wait` / `Monitor.PulseAll`:**
  - Low-level synchronization primitives for waiting and signaling between threads.
  - Used in `ReaderWriterLock` to coordinate access between readers and writers.

## Usage Example

Below is a practical example demonstrating how to use `ReaderWriterLock` to allow multiple concurrent readers and exclusive writers:

```csharp
using Synchronization;

var rwLock = new ReaderWriterLock();
int sharedResource = 0;

// Reader thread
void Read()
{
    rwLock.EnterReadLock();
    try {
        Console.WriteLine($"Read: {sharedResource}");
    } finally {
        rwLock.ExitReadLock();
    }
}

// Writer thread
void Write(int value)
{
    rwLock.EnterWriteLock();
    try {
        sharedResource = value;
        Console.WriteLine($"Wrote: {sharedResource}");
    } finally {
        rwLock.ExitWriteLock();
    }
}
```

For asynchronous scenarios, use `ReaderWriterLockAsync`:

```csharp
using Synchronization;

var rwLockAsync = new ReaderWriterLockAsync();
int sharedResource = 0;

async Task ReadAsync()
{
    await rwLockAsync.EnterReadLockAsync();
    try {
        Console.WriteLine($"Read: {sharedResource}");
    } finally {
        rwLockAsync.ExitReadLock();
    }
}

async Task WriteAsync(int value)
{
    await rwLockAsync.EnterWriteLockAsync();
    try {
        sharedResource = value;
        Console.WriteLine($"Wrote: {sharedResource}");
    } finally {
        rwLockAsync.ExitWriteLock();
    }
}
```

## Cross-References

- **Interface:** [`IReaderWriterLock`](./IReaderWriterLock.cs) defines the contract for all lock implementations.
- **Usage Example:** See [`StoreExample/Worker.cs`](../StoreExample/Worker.cs) for real-world usage of these synchronization primitives.
- **Project File:** [`Synchronization.csproj`](./Synchronization.csproj)

---

For further details, refer to the inline documentation in each class. Contributions and improvements to these synchronization primitives are welcome!
