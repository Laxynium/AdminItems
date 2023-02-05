using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api.AdminItems;

public static class ErrorResponses
{
    public static BadRequestObjectResult ColorNotFound(long colorId) =>
        new(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            {nameof(colorId), new []{$"Color with id {colorId} was not found"}}
        }));

    public static NotFoundObjectResult AdminItemNotFound(long adminItemId)
    {
        return new NotFoundObjectResult(new HttpValidationProblemDetails(new Dictionary<string, string[]>
        {
            {nameof(adminItemId), new []{$"Admin item with id {adminItemId} was not found"}}
        }));
    }
}