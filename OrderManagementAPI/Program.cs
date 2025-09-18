using System.Diagnostics;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderManagementAPI.Config;
using OrderManagementAPI.Infrastructure.Authentication;
using OrderManagementAPI.Middleware;
using OrderManagementAPI.Services;
using OrderManagementAPI.Services.Impl;
using OrderManagementAPI.Utilizes;


var builder = WebApplication.CreateBuilder(args);

// -------------- Load cloud config -------------- //
//builder.Configuration
//    .AddJsonStream(
//        new MemoryStream(Encoding.UTF8.GetBytes(ConfigUtilize.LoadConfig(builder.Configuration))))
//    .AddEnvironmentVariables();
// -------------- End load cloud config -------------- //

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging());

// -------------- Log4Net and Telemetry log config -------------- //
XmlConfigurator.Configure(new FileInfo("log4net.config"));

//--------

// Configure OpenTelemetry Logging  
// it allows your logs to be collected, enriched, and exported in a standardized way for observability.
builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
    options.ParseStateValues = true;
});
// -------------- End Log4Net and Telemetry log config -------------- //


//------------------ http Request ------------//
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


// -------------- Register Dao Utilize -------------- //
builder.Services.AddScoped<DapperDaoUtil>();
builder.Services.AddScoped<EntityFrmwkDaoUtil>();
// -------------- End Register Dao Utilize -------------- //

// -------------- Register repositories -------------- //
//builder.Services.AddScoped<ScheduleRepo>();
//builder.Services.AddScoped<HolidayRepo>();
//builder.Services.AddScoped<PersonRepo>();
//builder.Services.AddScoped<CommonCodeRepo>();
//builder.Services.AddScoped<DeviceRepo>();
//builder.Services.AddScoped<AccesslevelRepo>();
//builder.Services.AddScoped<AssignDeviceWithWorkLocationRepo>();
//builder.Services.AddScoped<AssignPersonWithAccessLevelRepo>();
//builder.Services.AddScoped<AccessRecordRepo>();
//builder.Services.AddHostedService<AutomationUpdateScheduleEveryday>();
// -------------- End register repositories -------------- //

// -------------- Register services -------------- //
builder.Services.AddScoped<IAuthenticationService, AuthenticationServiceImpl>();
//builder.Services.AddScoped<ICommonCodeService, CommonCodeServiceImpl>();
//builder.Services.AddScoped<IHolidayService, HolidayImpl>();
//builder.Services.AddScoped<IPersonSevice, PersonServiceImpl>();
//builder.Services.AddScoped<IDeviceService, DeviceServiceImpl>();
//builder.Services.AddScoped<IAccessLevelService, AccesslevelImplService>();
//builder.Services.AddScoped<IAssignDeviceWithWorkLocationService, AssignDeviceWithWorkLocationServiceImpl>();
//builder.Services.AddScoped<IAccessLevelWithPersonService, AccessLevelWithPersonServiceImpl>();
//builder.Services.AddScoped<IAccessRecordService, AccessRecordImplService>();
//builder.Services.AddScoped<IWorkerExcuteApiService, WorkerExcuteApiService>();
// -------------- End register services -------------- //


// -------------- Force to response snakes case -------------- //
builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.IncludeFields = true; });

// Add JWT Authentication from extension
builder.Services.AddJwtAuthentication(builder.Configuration);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Enable Annotation of Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OrderManagementAPI",
        Version = "v1",
        Description = "OrderManagementAPI",
        Contact = new OpenApiContact
        {
            Name = "OrderManagementAPI"
        }
    });

    // Add security definitions
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter token",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

    // Add security requirement
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

app.UseMiddleware<GlobalExceptionHandler>();

// -------------- Log4Net and Telemetry log config (Add TraceId and SpanId) -------------- //
app.Use(async (_, next) =>
{
    var traceId = Activity.Current?.TraceId.ToString() ?? "NoTrace";
    var spanId = Activity.Current?.SpanId.ToString() ?? "NoSpan";
    ThreadContext.Properties["traceId"] = traceId;
    ThreadContext.Properties["spanId"] = spanId;
    await next();
});
// -------------- End Log4Net and Telemetry log config (Add TraceId and SpanId) -------------- //

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderManagement API v1"); });
}

app.UseRouting();

app.MapControllers();

app.UseHttpsRedirection();

await app.RunAsync();