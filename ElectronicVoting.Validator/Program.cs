using ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger;
using ElectronicVoting.Validator.Infrastructure.HostedServices;
using ElectronicVoting.Validator.Infrastructure.Rafit;
using ElectronicVoting.Validator.Infrastructure.Services;
using ElectronicVoting.Validator.Infrastructure.Wolverine;
using Wolverine.Http;
using Wolverine.Runtime;

Console.WriteLine("Hello, World!");
var builder = WebApplication.CreateBuilder(args);
var applicationAssembly = typeof(LeaderInitiateVoteValidationHandler).Assembly;

builder.Services.AddInfrastructureServices();
builder.Services.AddHostedServices();
builder.Services.AddSwaggerGen();
builder.Services.AddRafit();
builder.Services.AddWolverine(builder.Configuration, applicationAssembly);
builder.Services.AddEntityFramework(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElectronicVoting.Validator API V1");
        c.RoutePrefix = "swagger"; // Ustawia URL na /swagger zamiast /swagger/index.html
    });

    app.MapOpenApi();
    using var scope = app.Services.CreateScope();
    
    using var dbContextElection = scope.ServiceProvider.GetRequiredService<ElectionDbContext>();
    dbContextElection.Database.EnsureCreated();
    
    using var dbContextValidatorLedger = scope.ServiceProvider.GetRequiredService<ValidatorLedgerDbContext>();
    dbContextValidatorLedger.Database.EnsureCreated();
    
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapWolverineEndpoints(opts =>
{
    opts.WarmUpRoutes = RouteWarmup.Eager;
});
app.Run();
