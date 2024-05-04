namespace SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

public readonly record struct SalesOrderTenantId
{
    public string Value { get; }

    private SalesOrderTenantId(string tenantId)
    {
        Value = tenantId;
    }

    public static SalesOrderTenantId Create(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentException($"Invalid tenant identifier value '{tenantId}'.");
        }

        return new SalesOrderTenantId(tenantId);
    }

    public static implicit operator string(SalesOrderTenantId streamId) => streamId.Value;
}