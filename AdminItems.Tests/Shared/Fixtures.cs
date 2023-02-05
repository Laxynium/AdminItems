using AdminItems.Api.Colors;
using AdminItems.Tests.Fakes;

namespace AdminItems.Tests.Shared;

public class Fixtures
{
    public const long DefaultColorId = 1;
    public const string DefaultColor = "indigo";

    public static AdminItemsApi AnAdminItemsApi(InMemoryAdminItemsStore adminItemsStore) => 
        AnAdminItemsApiWith(adminItemsStore, new Color(DefaultColorId, DefaultColor));

    public static AdminItemsApi AnAdminItemsApiWith(InMemoryAdminItemsStore adminItemsStore, params Color[] colors)
    {
        var colorsStore = new InMemoryColorsStore();
        colorsStore.AddColors(colors);
        
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(adminItemsStore);
        apiFactory.UseStore(colorsStore);

        return apiFactory;
    }
}