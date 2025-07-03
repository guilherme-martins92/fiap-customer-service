using Fiap.CustomerService.Application.Extensions;
using Fiap.CustomerService.Application.UseCases.CreateCustomerUseCase;
using Fiap.CustomerService.Application.UseCases.GetAllCustomersUseCase;
using Fiap.CustomerService.Application.UseCases.GetCustomerByDocumentNumberUseCase;
using Fiap.CustomerService.Application.UseCases.GetCustomerByIdUseCase;
using Fiap.CustomerService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/customers", async (CreateCustomerInput input, CreateCustomerUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync(input);
        if (result.IsSuccess)
        {
            return Results.Created($"/customers/{result.Data!.Id}", result.Data);
        }
        else
        {
            return Results.BadRequest(result.Errors);
        }
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapGet("/customers", async (GetAllCustomersUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync();

        return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Errors);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapGet("/customers/{id}", async (int id, GetCustomerByIdUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync(id);
        return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Errors);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapGet("/customers/document/{documentNumber}", async (string documentNumber, GetCustomerByDocumentNumberUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync(documentNumber);
        return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Errors);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

await app.RunAsync();