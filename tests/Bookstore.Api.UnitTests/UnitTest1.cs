using Bookstore.Api.Endpoints;

namespace Bookstore.Api.UnitTests;

public class BookRoutesTests
{
    [Fact]
    public void CreateHealthResponse_ReturnsHealthyStatus()
    {
        var response = BookRoutes.CreateHealthResponse();

        Assert.Equal("healthy", response.Status);
    }
}
