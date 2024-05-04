namespace Common.TableStorage;

public readonly record struct TableStorageConfiguration
{
    public string ConnectionString { get; }

    public TableStorageConfiguration(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Table Storage ConnectionString is required", nameof(connectionString));
        }

        ConnectionString = connectionString;
    }
}
