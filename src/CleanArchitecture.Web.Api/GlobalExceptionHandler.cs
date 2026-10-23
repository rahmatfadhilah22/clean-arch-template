using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace CleanArchitecture.Web.Api;

// Satu pintu untuk semua exception yang lolos dari controller:
// detail asli hanya masuk log server, client dapat pesan generik.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, message) = exception switch
        {
            PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
                => (StatusCodes.Status409Conflict, "Data sudah ada."),
            PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation }
                => (StatusCodes.Status409Conflict, "Data masih dipakai data lain."),
            NpgsqlException => (StatusCodes.Status503ServiceUnavailable, "Database sedang tidak tersedia."),
            _ => (StatusCodes.Status500InternalServerError, "Terjadi kesalahan pada server.")
        };

        // ponytail: log penuh di server; client tidak pernah lihat stack trace / kredensial.
        // TraceId ditulis eksplisit supaya bisa di-grep dari nilai yang diterima client.
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        logger.LogError(exception, "Unhandled exception on {Method} {Path} (traceId={TraceId})",
            httpContext.Request.Method, httpContext.Request.Path, traceId);

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = message,
            Instance = httpContext.Request.Path,
            // Activity.Id dipakai agar sama dengan traceId yang dikirim framework untuk 400/404.
            Extensions = { ["traceId"] = traceId }
        }, cancellationToken);
        return true;
    }
}
