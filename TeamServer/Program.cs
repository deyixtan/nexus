using TeamServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IListenerService, ListenerService>();
builder.Services.AddSingleton<IAgentService, AgentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

// to make it visible to test projects
// https://peterdaugaardrasmussen.com/2022/02/05/how-to-make-integration-tests-in-net-6-without-a-startup-cs-file/
public partial class Program { }
