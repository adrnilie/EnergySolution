namespace Common.TableStorage.Contracts;

public sealed record SegmentedResult<T>
{
    public IReadOnlyList<T> Results { get; set; } = default!;
    public string? ContinuationToken { get; set; }
}