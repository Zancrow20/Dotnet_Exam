using WebApi.ApplicationServices;
using WebApi.Configuration;
using WebApi.Endpoints;
using WebApi.HostedServices;
using WebApi.Hubs;

const string MyPolicy = "MyPolicy";
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var mongoConfig = new MongoDbConfig();
var rabbitMqConfig = new RabbitMqConfig();

builder.Configuration.GetSection(MongoDbConfig.MongoDb).Bind(mongoConfig);
builder.Configuration.GetSection(RabbitMqConfig.RabbitMqConfigString).Bind(rabbitMqConfig);

builder.Services.AddHostedService<DbContextMigration>();
builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddApplicationDb(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddIdentity()
    .AddJWTAuthorization(config)
    .AddMediatr()
    .AddMongo(mongoConfig)
    .AddApplicationServices()
    .AddMassTransitAndRabbitMq(rabbitMqConfig)
    .AddSwagger()
    .AddCors(options => options.AddPolicy(MyPolicy, pb 
        => pb.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    ))
    .AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton(_ => new Store());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors(MyPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapRatingEndpoints();
app.MapGameEndpoints();

app.MapHub<GameHub>("/gamehub");

app.Run();