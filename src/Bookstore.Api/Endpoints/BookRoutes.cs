using Bookstore.Api.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bookstore.Api.Endpoints;

public static class BookRoutes
{
    public static RouteGroupBuilder MapBookRoutes(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/books")
            .WithTags("Books");

        group.MapGet("/health", HealthAsync)
            .WithName("GetBooksHealth")
            .WithSummary("Returns the books API health status.")
            .WithOpenApi();

        return group;
    }

    public static HealthResponse CreateHealthResponse() => new("healthy");

    public static async Task<Ok<HealthResponse>> HealthAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return TypedResults.Ok(CreateHealthResponse());
    }
}
