using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using StoreExample.Models;

namespace StoreExample.Repository.DbContext;

public class OrderDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<OrderEntity> Orders { get; set; } // Orders table
    public DbSet<OutboxMessageEntity> OutboxMessages { get; set; } // OutboxMessages table

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
    
    // suppress warning for transactional transactions for in memory database
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderEntity>().HasKey(o => o.Id);
        modelBuilder.Entity<OutboxMessageEntity>().HasKey(e => e.Id);
    }

    public async Task SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        using var transaction = await Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // 1. get all domain events from all modified entries 
            var domainEntities = ChangeTracker.Entries<OrderEntity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity)
                .ToList();

            // 2. create outbox entries for each domain event
            var outboxMessages = domainEntities
                .SelectMany(order => order.DomainEvents)
                .Select(domainEvent => new OutboxMessageEntity()
                {
                    Id = Guid.NewGuid(),
                    Type = domainEvent.GetType().AssemblyQualifiedName!,
                    Content = JsonConvert.SerializeObject(domainEvent), // Use Json.NET for EF6
                    OccurredOn = domainEvent.OccurredOn,
                    Processed = false
                })
                .ToList();

            // 3. add outbox entries (atomic)
            OutboxMessages.AddRange(outboxMessages); // EF6 does not have AddRangeAsync
            
            // 4. EF generates atomic SQL commands and executes in a single transaction
            await SaveChangesAsync(cancellationToken);
            
            // 5. commit
            await transaction.CommitAsync(cancellationToken); // Synchronous commit for EF6
        }
        catch
        {
            // 5. rollback
            await transaction.RollbackAsync(cancellationToken); // Synchronous rollback for EF6
            
        }
    }
}
