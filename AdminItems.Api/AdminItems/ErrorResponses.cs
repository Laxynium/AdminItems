using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api.AdminItems;

public static class ErrorResponses
{
    public static BadRequestObjectResult ColorNotFound(long colorId) =>
        new(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            {nameof(colorId), new []{$"Color with id {colorId} was not found"}}
        }));
}