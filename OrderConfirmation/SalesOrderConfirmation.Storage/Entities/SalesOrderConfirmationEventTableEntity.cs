﻿using Common.TableStorage.Models;

namespace SalesOrderConfirmation.Storage.Entities;

internal sealed class SalesOrderConfirmationEventTableEntity : AzureTableEntity
{
    public string StreamId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string EventNamespace { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Version { get; set; }
    public bool IsProcessed { get; set; }
    public bool IsPublished { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset PublishDate { get; set; }
}