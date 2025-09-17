using System.Diagnostics;
using log4net;
using log4net.Config;
using OrderManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// -------------- Load cloud config -------------- //
//builder.Configuration
//    .AddJsonStream(
//        new MemoryStream(Encoding.UTF8.GetBytes(ConfigUtilize.LoadConfig(builder.Configuration))))
//    .AddEnvironmentVariables();
// -------------- End load cloud config -------------- //



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


// -------------- Force to response snakes case -------------- //
builder.Services.AddControllers();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseRouting();

app.UseHttpsRedirection();

await app.RunAsync();
