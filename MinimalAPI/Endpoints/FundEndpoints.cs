using MinimalAPI.Models;
using MinimalAPI.Models.DTOs;

namespace MinimalAPI.Endpoints
{
    /// <summary>
    /// Extension method to map Fund-related endpoints to the IEndpointRouteBuilder.
    /// </summary>
    public static class FundEndpoints
    {
        public static void MapFundEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var fundsGroup = endpoints.MapGroup("/api");

            fundsGroup.MapGet("/funds", (IServiceProvider sp) =>
                sp.GetRequiredService<FundEndpointHandlers>().GetAllFunds())
               .WithName("GetAllFunds")
               .Produces<ApiResponse<List<Fund>>>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status500InternalServerError)
               .WithTags("Funds");
            fundsGroup.MapGet("/funds/{id:int}", (IServiceProvider sp, int id) =>
                sp.GetRequiredService<FundEndpointHandlers>().GetFundById(id))
               .WithName("GetFundById")
               .Produces<ApiResponse<Fund>>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status500InternalServerError)
               .WithTags("Funds");
            fundsGroup.MapPost("/funds/{fundId:int}/contacts", (IServiceProvider sp, int fundId, AddContactToFundRequest request) =>
                sp.GetRequiredService<FundEndpointHandlers>().AddContactToFund(fundId, request))
               .WithName("AddContactToFund")
               .Accepts<AddContactToFundRequest>("application/json")
               .Produces<ApiResponse<EmptyResponse>>(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status500InternalServerError)
               .WithTags("Funds");
        }
    }
}
