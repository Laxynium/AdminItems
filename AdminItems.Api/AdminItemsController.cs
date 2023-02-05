using Microsoft.AspNetCore.Mvc;

namespace AdminItems.Api;

[ApiController]
[Route("[controller]")]
public class AdminItemsController : ControllerBase
{
    private readonly IAdminItemsStore _adminItemsStore;

    public AdminItemsController(IAdminItemsStore adminItemsStore)
    {
        _adminItemsStore = adminItemsStore;
    }
    [HttpPost]
    public Task Post()
    {
        _adminItemsStore.Add(new AdminItem(
            "GFJS1234",
            "First Admin Item",
            "This is a first admin item in system"));
        return Task.CompletedTask;
    }
}