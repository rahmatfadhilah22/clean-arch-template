using System.Diagnostics;
using CleanArchitecture.Repository;
using CleanArchitecture.Service;
using CleanArchitecture.Web.Api;
using Npgsql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

LoadDotEnv(builder);

// Serilog mengambil alih logger bawaan; konfigurasi ada di appsettings "Serilog".
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "CleanArchitecture.Web.Api"));

builder.Services.AddControllers();
builder.Services.AddCors(); // ponytail: allow-all dulu, persempit ke origin FE saat sudah ada
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Layer wiring: Web.Api → Service → Repository → Domain
builder.Services.AddRepository(BuildConnectionString(builder.Configuration));
builder.Services.AddService();

var app = builder.Build();

app.UseExceptionHandler();

// Setiap log dalam satu request membawa traceId yang sama —
// nilainya sama dengan "traceId" di body ProblemDetails.
app.Use(async (context, next) =>
{
    string traceId = Activity.Current?.Id ?? context.TraceIdentifier;
    using (app.Logger.BeginScope(new Dictionary<string, object> { ["traceId"] = traceId }))
    {
        await next();
    }
});

// Satu baris per request: method, path, status, durasi (menggantikan HttpLogging).
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Di dev profil http tidak punya port https → middleware ini cuma bikin warning.
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

// Semua origin boleh — cukup untuk tahap belajar / FE lokal.
// Nanti production: ganti jadi WithOrigins("https://app.kamu.com").
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseAuthorization();
app.MapControllers();

app.Run();

// .env berisi kunci terpisah (DB__Host, DB__Port, ...) supaya mudah dibaca.
// Connection string Npgsql dibangun dari nilai-nilai itu.
static string BuildConnectionString(IConfiguration configuration)
{
    string database = configuration["DB:Database"]
        ?? throw new InvalidOperationException(
            "DB__Database belum diatur. Salin .env.example menjadi .env lalu isi nilainya.");

    NpgsqlConnectionStringBuilder builder = new()
    {
        Host = configuration["DB:Host"] ?? "localhost",
        Port = int.TryParse(configuration["DB:Port"], out int port) ? port : 5432,
        Database = database,
        Username = configuration["DB:Username"],
        Password = configuration["DB:Password"]
    };

    return builder.ConnectionString;
}

// .NET tidak punya pembaca .env bawaan. Formatnya sederhana, jadi dibaca langsung —
// tanpa dependency tambahan. "DB__Host" dipetakan ke section "DB:Host".
static void LoadDotEnv(WebApplicationBuilder builder)
{
    string path = Path.Combine(builder.Environment.ContentRootPath, ".env");
    if (!File.Exists(path))
    {
        return;
    }

    Dictionary<string, string> values = [];
    foreach (string rawLine in File.ReadAllLines(path))
    {
        string line = rawLine.Trim();
        if (line.Length == 0 || line.StartsWith('#'))
        {
            continue; // baris kosong / komentar
        }

        int separator = line.IndexOf('=');
        if (separator <= 0)
        {
            continue; // bukan pasangan KEY=VALUE
        }

        string key = line.Substring(0, separator).Trim().Replace("__", ":");
        string value = line.Substring(separator + 1).Trim().Trim('"');
        values[key] = value;
    }

    // Ditambahkan terakhir → menang atas appsettings.json.
    builder.Configuration.AddInMemoryCollection(values);
}
