using BakongHealthCheck;
using BakongHealthCheck.APIConnection;
using BakongHealthCheck.Configures;
using BakongHealthCheck.Data;
using BakongHealthCheck.Mapping;
using BakongHealthCheck.Middleware;
using BakongHealthCheck.Repositories;
using BakongHealthCheck.Repository;
using BakongHealthCheck.Services;
using BakongHealthCheck.Services.Bakong;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inject Mapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


// Hosting Service
builder.Services.AddHttpClient();
builder.Services.AddScoped<IBakongRepository, BakongRepository>();
builder.Services.AddScoped<IBCService, BCService>();
builder.Services.AddSingleton<IMyHttpRequest, MyHttpRequest>();
builder.Services.AddSingleton<IBakongService, BakongService>();


// Add Configuration
builder.Services.Configure<ConfigureBakong>(config.GetSection(nameof(ConfigureBakong)));
builder.Services.AddSingleton<IConfigureBakong>(conf => conf.GetRequiredService<IOptions<ConfigureBakong>>().Value);


builder.Services.AddHostedService<MainAPP>();

// Connection DB
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Util Library Log,.....
//builder.Configuration["Configure:LogPath"]
Log.Logger = new LoggerConfiguration()
             .Enrich.WithThreadId()
             .Enrich.FromLogContext()
             .ReadFrom.Configuration(config)
             .CreateLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Middleware API KEY
//app.UseMiddleware<ApiKeyMiddleware>(); // comment no need to use auth because we have 3scale red hat

app.Run();
