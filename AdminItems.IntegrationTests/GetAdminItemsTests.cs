using AdminItems.Api.AdminItems;
using AdminItems.IntegrationTests.Shared;
using FluentAssertions;

namespace AdminItems.IntegrationTests;

public class GetAdminItemsTests : IntegrationTest
{
    public GetAdminItemsTests(PostgresDatabase postgresDatabase) : base(postgresDatabase)
    {
    }

    [Theory]
    [InlineData("ADBD123", "Some random admin item name", null, "red")]
    [InlineData("432DAD", "Another random one", "Some descriptive comment", "blue")]
    public async Task response_contains_created_admin_item(string code, string name, string? comments, string color)
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1), new AdminItem(
            code, name, comments ?? string.Empty, color)));

        var response = await Api.GetAdminItems();

        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new
                    {
                        id = 1,
                        code,
                        name,
                        color = color
                    }
                }
            });
    }

    [Fact]
    public async Task by_default_admin_items_are_ordered_by_code()
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(2),
            new AdminItem("BBB12", "Admin Item2", "Some comment 2", "blue")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(3),
            new AdminItem("AAA11", "Admin Item3", "Some comment 3", "green")));

        var response = await Api.GetAdminItems();

        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new { id = 3, code = "AAA11", name = "Admin Item3", color = "green" },
                    new { id = 2, code = "BBB12", name = "Admin Item2", color = "blue" },
                    new { id = 1, code = "BBB13", name = "Admin Item1", color = "red" }
                }
            }, opt => opt.WithStrictOrderingFor(x => x.items));
    }

    [Theory]
    [InlineData("wrong_column asc")]
    [InlineData("code asscc")]
    [InlineData("code")]
    [InlineData("code asc aa")]
    public async Task order_by_is_invalid(string orderBy)
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));
        
        var response = await Api.GetAdminItems(("orderBy", orderBy));

        response.Should().Be400BadRequest()
            .And.HaveError("orderBy", "*");
    }

    [Theory]
    [InlineData("code asc")]
    [InlineData("code desc")]
    [InlineData("name desc")]
    [InlineData("color asc")]
    public async Task order_by_is_valid(string orderBy)
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));
        
        var response = await Api.GetAdminItems(("orderBy", orderBy));

        response.Should().Be200Ok();
    }
}