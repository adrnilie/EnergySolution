namespace SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;

public readonly record struct SalesOrderStreamId
{
    public Guid Value { get; }

    private SalesOrderStreamId(Guid streamId)
    {
        Value = streamId;
    }

    public static SalesOrderStreamId Create(Guid streamId)
    {
        if (streamId == Guid.Empty)
        {
            throw new ArgumentException($"Invalid identifier value '{streamId}'.");
        }

        return new SalesOrderStreamId(streamId);
    }

    public static SalesOrderStreamId Parse(string streamId)
    {
        if (string.IsNullOrWhiteSpace(streamId))
        {
            throw new ArgumentException($"Invalid identifier value '{streamId}'.");
        }

        if (!Guid.TryParse(streamId, out Guid streamIdGuid))
        {
            throw new ArgumentException($"Invalid identifier value '{streamId}'.");
        }

        return Create(streamIdGuid);
    }

    public static SalesOrderStreamId Generate() => Create(Guid.NewGuid());

    public static implicit operator string(SalesOrderStreamId streamId) => streamId.Value.ToString("D");
}