using System.Collections.Concurrent;
using Riok.Mapperly.Abstractions;

namespace StoreExample.Models;

public class OutboxMessageEntity : Entity
{
    public bool Processed { get; set; }
    public string Type { get; set; } // Event Type
    public string Content { get; set; } // JSON Serialized Event Data
    public DateTime OccurredOn { get; set; }
    public DateTime? ProcessedAt { get; set; }
}