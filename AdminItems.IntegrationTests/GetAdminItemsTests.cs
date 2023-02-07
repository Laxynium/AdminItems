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

    [Fact]
    public async Task desc_order_is_applied()
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(2),
            new AdminItem("BBB12", "Admin Item2", "Some comment 2", "blue")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(3),
            new AdminItem("AAA11", "Admin Item3", "Some comment 3", "green")));
        
        var response = await Api.GetAdminItems(("orderBy","code desc"));
        
        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new { id = 1, code = "BBB13", name = "Admin Item1", color = "red" },
                    new { id = 2, code = "BBB12", name = "Admin Item2", color = "blue" },
                    new { id = 3, code = "AAA11", name = "Admin Item3", color = "green" }
                }
            }, opt => opt.WithStrictOrderingFor(x => x.items));
    }
    
    [Fact]
    public async Task desc_order_is_applied_for_color()
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(2),
            new AdminItem("BBB12", "Admin Item2", "Some comment 2", "blue")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(3),
            new AdminItem("AAA11", "Admin Item3", "Some comment 3", "green")));
        
        var response = await Api.GetAdminItems(("orderBy","color desc"));

        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new { id = 1, code = "BBB13", name = "Admin Item1", color = "red" },
                    new { id = 3, code = "AAA11", name = "Admin Item3", color = "green" },
                    new { id = 2, code = "BBB12", name = "Admin Item2", color = "blue" }
                }
            }, opt => opt.WithStrictOrderingFor(x => x.items));
    }

    [Fact]
    public async Task cursor_based_pagination_on_color_going_forward()
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(2),
            new AdminItem("BBB12", "Admin Item2", "Some comment 2", "blue")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(3),
            new AdminItem("AAA11", "Admin Item3", "Some comment 3", "green")));
        
        //when
        var response = await Api.GetAdminItems((after: null, before: null, 2), ("orderBy","color desc"));
        //then
        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new { id = 1, code = "BBB13", name = "Admin Item1", color = "red" },
                    new { id = 3, code = "AAA11", name = "Admin Item3", color = "green" }
                },
                after = "green",
                before = "red"
            }, opt => opt.WithStrictOrderingFor(x => x.items));
        
        //when
        response = await Api.GetAdminItems((after: "green", before: null, 2), ("orderBy","color desc"));
        //then
        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new { id = 2, code = "BBB12", name = "Admin Item2", color = "blue" }
                },
                after = "blue",
                before = "blue"
            }, opt => opt.WithStrictOrderingFor(x => x.items));
    }
    
    [Fact]
    public async Task cursor_based_pagination_on_name_going_forward()
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(2),
            new AdminItem("BBB12", "Admin Item2", "Some comment 2", "blue")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(3),
            new AdminItem("AAA11", "Admin Item3", "Some comment 3", "green")));
        
        //when
        var response = await Api.GetAdminItems((after: null, before: null, 1), ("orderBy","name asc"));
        //then
        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new { id = 1, code = "BBB13", name = "Admin Item1", color = "red" }
                },
                after = "Admin Item1",
                before = "Admin Item1"
            }, opt => opt.WithStrictOrderingFor(x => x.items));
        
        //when
        response = await Api.GetAdminItems((after: "Admin Item1", before: null, 2), ("orderBy","name asc"));
        //then
        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new { id = 2, code = "BBB12", name = "Admin Item2", color = "blue" },
                    new { id = 3, code = "AAA11", name = "Admin Item3", color = "green" }
                },
                after = "Admin Item3",
                before = "Admin Item2"
            }, opt => opt.WithStrictOrderingFor(x => x.items));
    }
    
    [Fact]
    public async Task cursor_based_pagination_on_code_going_backwards()
    {
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(1),
            new AdminItem("BBB13", "Admin Item1", "Some comment 1", "red")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(2),
            new AdminItem("BBB12", "Admin Item2", "Some comment 2", "blue")));

        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(3),
            new AdminItem("AAA11", "Admin Item3", "Some comment 3", "green")));
        
        await Run<IAdminItemsStore>(store => store.Add(AdminItemId.Create(4),
            new AdminItem("AAA05", "Admin Item4", "Some comment 4", "black")));
        
        var response = await Api.GetAdminItems((after: null, before: null, 2), ("orderBy","code asc"));
        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new { id = 4, code = "AAA05", name = "Admin Item4", color = "black" },
                    new { id = 3, code = "AAA11", name = "Admin Item3", color = "green" }
                },
                after = "AAA11",
                before = "AAA05"
            }, opt => opt.WithStrictOrderingFor(x => x.items));
        
        response = await Api.GetAdminItems((after: "AAA11", before: null, 1), ("orderBy","code asc"));
        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new { id = 2, code = "BBB12", name = "Admin Item2", color = "blue" },
                },
                after = "BBB12",
                before = "BBB12"
            }, opt => opt.WithStrictOrderingFor(x => x.items));
        
        //when
        response = await Api.GetAdminItems((after: null, before: "BBB12", 2), ("orderBy","code asc"));
        //then
        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new { id = 4, code = "AAA05", name = "Admin Item4", color = "black" },
                    new { id = 3, code = "AAA11", name = "Admin Item3", color = "green" }
                },
                after = "AAA11",
                before = "AAA05"
            }, opt => opt.WithStrictOrderingFor(x => x.items));
    }


}