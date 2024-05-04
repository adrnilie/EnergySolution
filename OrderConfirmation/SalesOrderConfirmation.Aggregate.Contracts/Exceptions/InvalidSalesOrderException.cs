namespace SalesOrderConfirmation.Aggregate.Contracts.Exceptions;

public sealed class InvalidSalesOrderException : Exception
{
    public InvalidSalesOrderException()
    {
        
    }

    public InvalidSalesOrderException(string message) : base(message)
    {
        
    }
}