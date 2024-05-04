namespace Common.TableStorage.Contracts;

public sealed class TableStorageException : Exception
{
    public TableStorageException()
    {
        
    }

    public TableStorageException(string message) : base(message) 
    {
        
    }

    public TableStorageException(string message, Exception exception) : base(message, exception) 
    {
        
    }
}