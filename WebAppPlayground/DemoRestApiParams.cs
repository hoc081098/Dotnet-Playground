using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace WebAppPlayground;

public sealed record DemoResourceBody(string Name);

public static class DemoRestApiParams
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public void MapDemoRestApiParamsEndPoints1()
        {
            // Get token
            endpoints.MapGet("antiforgery/token", (IAntiforgery forgeryService, HttpContext context) =>
            {
                var tokens = forgeryService.GetAndStoreTokens(context);
                var xsrfToken = tokens.RequestToken!;
                return TypedResults.Content(xsrfToken, contentType: "text/plain");
            });

            // Use attributes
            var group1 = endpoints.MapGroup("/demo-rest-api-params-1");

            // 1. Route param (Path param)
            group1.MapGet("/demo-resources/{id:guid}",
                ([FromRoute] Guid id) =>
                    Results.Ok(new { Id = id }));

            // 2. Query param
            group1.MapGet("/demo-resources/search",
                (
                    [FromQuery] string name,
                    [FromQuery] int page = 1,
                    [FromQuery] int perPage = 30
                ) => Results.Ok(new { Name = name, Page = page, PerPage = perPage }));

            // 3. Header param
            group1.MapGet("/demo-resources/header",
                (
                    [FromHeader(Name = "X-Api-Key")] string? apiKey
                ) => Results.Ok(new { ApiKey = apiKey }));

            // 4. Request body
            group1.MapPost("/demo-resources",
                ([FromBody] DemoResourceBody res) =>
                {
                    var id = Guid.CreateVersion7();
                    return Results.Created($"/demo-resources/{id}", res);
                });

            // 5. Form data
            group1.MapPost("/demo-resources/avatar1",
                async ([FromForm] string fileName,
                    [FromForm] IFormFile file,
                    CancellationToken cancellationToken) =>
                {
                    if (file.Length == 0)
                        return Results.BadRequest("Empty file.");

                    // Read first up to 16 bytes
                    await using var stream = file.OpenReadStream();
                    var buffer = new byte[Math.Min(16, file.Length)];
                    await stream.ReadExactlyAsync(buffer, cancellationToken);

                    return Results.Ok(new
                    {
                        FileName = fileName,
                        UploadedFileName = file.FileName,
                        file.ContentType,
                        file.Length,
                        FirstBytesHex = Convert.ToHexString(buffer),
                    });
                });

            group1.MapPost("/demo-resources/avatar2",
                async ([FromForm] string fileName,
                    [FromForm] IFormFile file,
                    CancellationToken cancellationToken) =>
                {
                    switch (file.Length)
                    {
                        case 0:
                            return Results.BadRequest("Empty file.");
                        case > 5 * 1024 * 1024:
                            return Results.BadRequest("File too large. Max 5 MB.");
                    }

                    // Copy file to memory stream
                    using var memoryStream = new MemoryStream(capacity: (int)file.Length);
                    await file.CopyToAsync(memoryStream, cancellationToken);
                    var bytes = memoryStream.ToArray();

                    return Results.Ok(new
                    {
                        FileName = fileName,
                        UploadedFileName = file.FileName,
                        file.ContentType,
                        file.Length,
                        Bytes = Convert.ToHexString(bytes),
                    });
                });

            group1.MapPost("/demo-resources/avatar3",
                async ([FromForm] string fileName,
                    [FromForm] IFormFile file,
                    CancellationToken cancellationToken) =>
                {
                    switch (file.Length)
                    {
                        case 0:
                            return Results.BadRequest("Empty file.");
                        case > 5 * 1024 * 1024:
                            return Results.BadRequest("File too large. Max 5 MB.");
                    }

                    // Save file to disk

                    // Create directory if not exists
                    var dirPath = Path.Combine(AppContext.BaseDirectory, "UploadedFiles");
                    Directory.CreateDirectory(dirPath);
                    Console.WriteLine(">>> AppContext.BaseDirectory: " + AppContext.BaseDirectory);
                    Console.WriteLine(">>> Upload directory: " + dirPath);

                    // Generate file path
                    var safeFileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(dirPath, $"{Guid.NewGuid()}_{fileName}_{safeFileName}");
                    Console.WriteLine($">>> safeFileName: {safeFileName}");
                    Console.WriteLine($">>> filePath: {filePath}");

                    // Copy to output stream
                    await using var outputStream = File.Create(filePath);
                    await file.CopyToAsync(outputStream, cancellationToken);
                    Console.WriteLine(">>> Uploaded file saved to: " + Path.GetFullPath(filePath));

                    return Results.Ok(new
                    {
                        FileName = fileName,
                        UploadedFileName = file.FileName,
                        file.ContentType,
                        file.Length,
                        SavedAs = Path.GetFileName(filePath)
                    });
                });

            // 6. Cookie param.
            // Header: "Cookie: Session-Id=abc123; theme=dark; lang=vi"
            group1.MapGet("/demo-resources/cookie",
                (HttpRequest request) => request.Cookies["Session-Id"] is { } sessionId
                    ? Results.Ok(new { SessionId = sessionId })
                    : Results.BadRequest("Missing 'session-id' cookie."));

            // 6. Route data via request.RouteValues
            group1.MapGet("/demo-resources/route-data/{additional:alpha}",
                (string additional, HttpRequest request) =>
                {
                    var additionalViaRouteValues = request.RouteValues["additional"];
                    if (additionalViaRouteValues is not string v || v != additional)
                    {
                        return Results.BadRequest(additionalViaRouteValues);
                    }

                    return Results.Ok(new { Additional = additional });
                });
        }

        public void MapDemoRestApiParamsEndPoints2()
        {
            // Do not use attributes for some parameters
            var group2 = endpoints.MapGroup("/demo-rest-api-params-2");

            // 1. Route param (Path param) - without attribute
            group2.MapGet("/demo-resources/{id:guid}",
                (Guid id) =>
                    Results.Ok(new { Id = id }));

            // 2. Query param - without attributes
            group2.MapGet("/demo-resources/search",
                (
                    string name,
                    int page = 1,
                    int perPage = 30
                ) => Results.Ok(new { Name = name, Page = page, PerPage = perPage }));

            // 3. Header param - still use attribute
            group2.MapGet("/demo-resources/header",
                (
                    [FromHeader(Name = "X-Api-Key")] string? apiKey
                ) => Results.Ok(new { ApiKey = apiKey }));

            // 4. Request body - without attribute
            group2.MapPost("/demo-resources",
                (DemoResourceBody res) =>
                {
                    var id = Guid.CreateVersion7();
                    return Results.Created($"/demo-resources/{id}", res);
                });

            // 5. Form data - without attributes
            group2.MapPost("/demo-resources/avatar1",
                async (string fileName,
                    IFormFile file,
                    CancellationToken cancellationToken) =>
                {
                    if (file.Length == 0)
                        return Results.BadRequest("Empty file.");

                    // Read first up to 16 bytes
                    await using var stream = file.OpenReadStream();
                    var buffer = new byte[Math.Min(16, file.Length)];
                    await stream.ReadExactlyAsync(buffer, cancellationToken);

                    return Results.Ok(new
                    {
                        FileName = fileName,
                        UploadedFileName = file.FileName,
                        file.ContentType,
                        file.Length,
                        FirstBytesHex = Convert.ToHexString(buffer),
                    });
                });

            group2.MapPost("/demo-resources/avatar2",
                async (string fileName,
                    IFormFile file,
                    CancellationToken cancellationToken) =>
                {
                    switch (file.Length)
                    {
                        case 0:
                            return Results.BadRequest("Empty file.");
                        case > 5 * 1024 * 1024:
                            return Results.BadRequest("File too large. Max 5 MB.");
                    }

                    // Copy file to memory stream
                    using var memoryStream = new MemoryStream(capacity: (int)file.Length);
                    await file.CopyToAsync(memoryStream, cancellationToken);
                    var bytes = memoryStream.ToArray();

                    return Results.Ok(new
                    {
                        FileName = fileName,
                        UploadedFileName = file.FileName,
                        file.ContentType,
                        file.Length,
                        Bytes = Convert.ToHexString(bytes),
                    });
                });

            group2.MapPost("/demo-resources/avatar3",
                async (string fileName,
                    IFormFile file,
                    CancellationToken cancellationToken) =>
                {
                    switch (file.Length)
                    {
                        case 0:
                            return Results.BadRequest("Empty file.");
                        case > 5 * 1024 * 1024:
                            return Results.BadRequest("File too large. Max 5 MB.");
                    }

                    // Save file to disk

                    // Create directory if not exists
                    var dirPath = Path.Combine(AppContext.BaseDirectory, "UploadedFiles");
                    Directory.CreateDirectory(dirPath);
                    Console.WriteLine(">>> AppContext.BaseDirectory: " + AppContext.BaseDirectory);
                    Console.WriteLine(">>> Upload directory: " + dirPath);

                    // Generate file path
                    var safeFileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(dirPath, $"{Guid.NewGuid()}_{fileName}_{safeFileName}");
                    Console.WriteLine($">>> safeFileName: {safeFileName}");
                    Console.WriteLine($">>> filePath: {filePath}");

                    // Copy to output stream
                    await using var outputStream = File.Create(filePath);
                    await file.CopyToAsync(outputStream, cancellationToken);
                    Console.WriteLine(">>> Uploaded file saved to: " + Path.GetFullPath(filePath));

                    return Results.Ok(new
                    {
                        FileName = fileName,
                        UploadedFileName = file.FileName,
                        file.ContentType,
                        file.Length,
                        SavedAs = Path.GetFileName(filePath)
                    });
                });

            // 6. Cookie param.
            // Header: "Cookie: Session-Id=abc123; theme=dark; lang=vi"
            group2.MapGet("/demo-resources/cookie",
                (HttpRequest request) => request.Cookies["Session-Id"] is { } sessionId
                    ? Results.Ok(new { SessionId = sessionId })
                    : Results.BadRequest("Missing 'session-id' cookie."));

            // 6. Route data via request.RouteValues
            group2.MapGet("/demo-resources/route-data/{additional:alpha}",
                (string additional, HttpRequest request) =>
                {
                    var additionalViaRouteValues = request.RouteValues["additional"];
                    if (additionalViaRouteValues is not string v || v != additional)
                    {
                        return Results.BadRequest(additionalViaRouteValues);
                    }

                    return Results.Ok(new { Additional = additional });
                });
        }
    }
}