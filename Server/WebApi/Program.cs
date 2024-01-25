using WebApi.ApplicationServices;
using WebApi.Configuration;
using WebApi.Endpoints;
using WebApi.HostedServices;

const string MyPolicy = "MyPolicy";
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var mongoConfig = new MongoDbConfig();

builder.Configuration.GetSection(MongoDbConfig.MongoDb).Bind(mongoConfig);

builder.Services.AddHostedService<DbContextMigration>();
builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddApplicationDb(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddIdentity()
    .AddJWTAuthorization(config)
    .AddMediatr()
    .AddMongo(mongoConfig)
    .AddSwagger()
    .AddCors(options => options.AddPolicy(MyPolicy, pb 
        => pb.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    ));

builder.Services.AddEndpointsApiExplorer();


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

app.Run();