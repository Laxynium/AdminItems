using AdminItems.Api.AdminItems;
using AdminItems.IntegrationTests.Shared;
using FluentAssertions;

namespace AdminItems.IntegrationTests.GetAdminItems;

public class FilterAdminItemsTests : IntegrationTest
{
    public FilterAdminItemsTests(PostgresDatabase postgresDatabase) : base(postgresDatabase)
    {
    }

    [Theory]
    [InlineData("DDD12")]
    [InlineData("%12")]
    [InlineData("%DD%")]
    public async Task matches_code_pattern(string code)
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(2),
            new AdminItem("DDD12", "Another admin item", "Some not important comment", "blue")));

        var response = await Api.GetAdminItems(("code", code));
        response.Should().Be200Ok().And.BeAs(new
        {
            items = new[]
            {
                new { id = 2, code = "DDD12", name = "Another admin item", color = "blue" }
            }
        });
    }
    
    [Theory]
    [InlineData("BBB14")]
    [InlineData("DDD17")]
    [InlineData("1DDD17")]
    [InlineData("%15%")]
    [InlineData("%Dx%")]
    public async Task does_not_match_code_pattern(string code)
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(2),
            new AdminItem("DDD12", "Another admin item", "Some not important comment", "blue")));

        var response = await Api.GetAdminItems(("code", code));
        response.Should().Be200Ok().And.BeAs(new
        {
            items = Array.Empty<object>()
        });
    }

    [Theory]
    [InlineData("Another%")]
    [InlineData("%r a%")]
    public async Task matches_name_pattern(string name)
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(2),
            new AdminItem("DDD12", "Another admin item", "Some not important comment", "blue")));

        var response = await Api.GetAdminItems(("name", name));
        response.Should().Be200Ok().And.BeAs(new
        {
            items = new[]
            {
                new { id = 2, code = "DDD12", name = "Another admin item", color = "blue" }
            }
        });
    }
    
    [Theory]
    [InlineData("n I")]
    [InlineData("n i")]
    [InlineData("Anot")]
    [InlineData("Xyz")]
    public async Task does_not_match_name_pattern(string name)
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(2),
            new AdminItem("DDD12", "Another admin item", "Some not important comment", "blue")));

        var response = await Api.GetAdminItems(("name", name));
        response.Should().Be200Ok().And.BeAs(new
        {
            items = Array.Empty<object>()
        });
    }
    
}