using Microsoft.AspNetCore.Mvc;
using SalesOrderConfirmation.Aggregate.Contracts.Commands;
using SalesOrderConfirmation.Aggregate.Contracts.ValueObjects;
using SalesOrderConfirmation.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSalesOrderConfirmationStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/salesOrder", async (ISalesOrderConfirmationStorage storage) =>
    {
        var streamId = SalesOrderStreamId.Generate();
        var tenantId = SalesOrderTenantId.Create(Guid.NewGuid().ToString("D"));

        var salesOrder = await storage.LoadOrCreateAsync(streamId, tenantId);
        salesOrder.ConfirmOrder(new ConfirmOrderCommand
        {
            StreamId = streamId,
            TenantId = tenantId,
        });

        await storage.SaveChangesAsync(salesOrder);

        return Results.Ok(salesOrder);
    })
.WithName("CreateSalesOrder")
.WithOpenApi();

app.MapGet("/salesOrder/{streamId:guid}/{tenantId}", async ([FromRoute] Guid streamId, [FromRoute] string tenantId, ISalesOrderConfirmationStorage storage) =>
    {
        var salesOrder = await storage.LoadOrCreateAsync(SalesOrderStreamId.Create(streamId), SalesOrderTenantId.Create(tenantId));
        return Results.Ok(salesOrder);
    })
.WithName("GetSalesOrder")
.WithOpenApi();

app.Run();
