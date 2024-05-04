namespace Common.TableStorage.Contracts;

public sealed class InvalidTableStorageAccountException : Exception
{
    public InvalidTableStorageAccountException()
    {
        
    }

    public InvalidTableStorageAccountException(string message) : base(message)
    {
        
    }

    public InvalidTableStorageAccountException(string message, Exception exception) : base(message, exception)
    {

    }
}
