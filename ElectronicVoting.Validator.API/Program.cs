
using ElectronicVoting.Validator.Application.Service;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.Environments;
using ElectronicVoting.Validator.Infrastructure.Exceptions;
using ElectronicVoting.Validator.Infrastructure.Hangfire;
using ElectronicVoting.Validator.Infrastructure.MediatR;
using ElectronicVoting.Validator.Infrastructure.Repository;
using Hangfire;
using ElectronicVoting.Validator.Infrastructure.Refit;
using ElectronicVoting.Validator.Infrastructure.Startup;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
var applicationAssembly = typeof(ElectronicVoting.Validator.Application.AssemblyReference)?.Assembly;

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Dodano ponieweważ narazie nie są zainstalowane certyfikaty SSL
builder.Services.AddHttpClient("CustomClient");

builder.Services.AddRafit();
builder.Services.AddHangfire();
builder.Services.AddServices();
builder.Services.AddStartupTasks();
builder.Services.AddRepositories();
builder.Services.AddEntityFramework();
builder.Services.AddGlobalException();
builder.Services.AddMediatR(applicationAssembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policyBuilder =>
    {
        policyBuilder.AllowAnyHeader().AllowAnyMethod()
                     .WithOrigins("http://localhost:4200", "https://localhost:4200", "https://localhost:8443")
                     .AllowCredentials()
                     .SetIsOriginAllowed((host) => true);
    });
});

if (builder.Environment.IsDevelopmentDockerTestOrDevelopment())
{
    var certificationPath = "/usr/local/share/ca-certificates/";
    var containerName = "localhost";
    var certification = certificationPath + containerName + ".pfx"; 
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(443, listenOptions =>
        {
            listenOptions.UseHttps(certification, "changeit");
        });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopmentDockerTestOrDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    using var scope = app.Services.CreateScope();
    using var dbContext = scope.ServiceProvider.GetRequiredService<ValidatorDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseExceptionHandler();
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireOpenAuthorizationFilter()]
});

//RecurringJob.AddOrUpdate<IMediator>("CreateBlock", mediator => mediator.Send(new CreateBlock(), CancellationToken.None), Cron.Minutely);

app.Run();

