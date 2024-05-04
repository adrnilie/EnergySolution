using FluentAssertions;
using SalesOrderConfirmation.Aggregate;
using SalesOrderConfirmation.Aggregate.Contracts.Commands;
using SalesOrderConfirmation.Aggregate.Contracts.Exceptions;
using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;
using SalesOrderConfirmation.Messages;

namespace SalesOrderConfirmation.Testing.ComponentTests.Aggregate;

public sealed class SalesOrderTests
{
    [Fact]
    public void GivenValidStreamIdAndTenantId_WhenCreatingNewSalesOrder_ThenReturnExpectedOrder()
    {
        // Arrange
        var streamId = SalesOrderStreamId.Generate();
        var tenantId = SalesOrderTenantId.Create(Guid.NewGuid().ToString("N"));
        


        // Act
        var salesOrder = new SalesOrder(streamId, tenantId);

        // Assert
        salesOrder.StreamId.Should().Be(streamId);
        salesOrder.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public void GivenConfirmOrderCommand_WhenConfirmingOrder_ThenReturnExpectedOrder()
    {
        // Arrange
        var streamId = SalesOrderStreamId.Generate();
        var tenantId = SalesOrderTenantId.Create(Guid.NewGuid().ToString("N"));

        var salesOrder = new SalesOrder(streamId, tenantId);

        var confirmOrderCommand = new ConfirmOrderCommand(streamId, tenantId);

        // Act
        salesOrder.ConfirmOrder(confirmOrderCommand);

        // Assert
        var salesOrderConfirmed = salesOrder.Commit().Should().ContainSingle(x => x is SalesOrderConfirmed).Subject;
        salesOrderConfirmed.Version.Should().Be(1);
    }

    [Fact]
    public void GivenConfirmOrderCommand_WhenStreamIdIsDifferent_ThenThrowInvalidSalesOrderException()
    {
        // Arrange
        var streamId = SalesOrderStreamId.Generate();
        var tenantId = SalesOrderTenantId.Create(Guid.NewGuid().ToString("N"));

        var salesOrder = new SalesOrder(streamId, tenantId);

        var confirmOrderCommand = new ConfirmOrderCommand(SalesOrderStreamId.Generate(), tenantId);

        // Act
        var act = () => salesOrder.ConfirmOrder(confirmOrderCommand);

        // Assert
        act.Should().ThrowExactly<InvalidSalesOrderException>();
    }

    [Fact]
    public void GivenConfirmOrderCommand_WhenTenantIdIsDifferent_ThenThrowInvalidSalesOrderException()
    {
        // Arrange
        var streamId = SalesOrderStreamId.Generate();
        var tenantId = SalesOrderTenantId.Create(Guid.NewGuid().ToString("N"));

        var salesOrder = new SalesOrder(streamId, tenantId);

        var confirmOrderCommand = new ConfirmOrderCommand(streamId, SalesOrderTenantId.Create("invalid"));

        // Act
        var act = () => salesOrder.ConfirmOrder(confirmOrderCommand);

        // Assert
        act.Should().ThrowExactly<InvalidSalesOrderException>();
    }
}