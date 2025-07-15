using Fiap.CustomerService.Application.Extensions;
using Fiap.CustomerService.Application.UseCases.CreateCustomerUseCase;
using Fiap.CustomerService.Application.UseCases.DeleteCustomerUseCase;
using Fiap.CustomerService.Application.UseCases.GetAllCustomersUseCase;
using Fiap.CustomerService.Application.UseCases.GetCustomerByDocumentNumberUseCase;
using Fiap.CustomerService.Application.UseCases.GetCustomerByEmailUseCase;
using Fiap.CustomerService.Application.UseCases.GetCustomerByIdUseCase;
using Fiap.CustomerService.Application.UseCases.UpdateCustomerUseCase;
using Fiap.CustomerService.Infrastructure.AwsServices;
using Fiap.CustomerService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add AWS Lambda support. When running the application as an AWS Serverless application, Kestrel is replaced
// with a Lambda function contained in the Amazon.Lambda.AspNetCoreServer package, which marshals the request into the ASP.NET Core hosting framework.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AWS"));

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

app.MapGet("/customers/{id}", async (Guid id, GetCustomerByIdUseCase useCase) =>
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

app.MapGet("/customers/email/{email}", async (string email, GetCustomerByEmailUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync(email);
        return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Errors);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapDelete("/customers/{id}", async (Guid id, DeleteCustomerUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync(id);
        return result.IsSuccess ? Results.NoContent() : Results.NotFound(result.Errors);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

app.MapPut("/customers/{id}", async (Guid id, UpdateCustomerInput input, UpdateCustomerUseCase useCase) =>
{
    try
    {
        var result = await useCase.ExecuteAsync(id, input);
        return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.Errors);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
});

await app.RunAsync();