using System.Reflection;

namespace SalesOrderConfirmation.Messages;

public abstract record SalesOrderConfirmationMessages
{
    public static Assembly Instance => typeof(SalesOrderConfirmationMessages).Assembly;
}