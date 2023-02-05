using AdminItems.Api.AdminItems;
using AdminItems.Api.Colors;
using AdminItems.Tests.Fakes;
using FluentAssertions;
using static AdminItems.Tests.Shared.Fixtures;

namespace AdminItems.Tests;

public class UpdateAdminItemEndpointTests
{
    [Theory]
    [InlineData("GFJS1234", "First Admin Item", "This is a first admin item in system")]
    [InlineData("YTRA1235", "Another admin Item", "")]
    [InlineData("1234567890AB", "Another one", "Admin item with max 12 characters")]
    public async Task put_a_valid_admin_item(string? code, string? name, string? comments)
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1);
        await apiFactory.ThereIsAnAdminItem(new
        {
            code = "ABCA123",
            name = "Some item name",
            colorId = DefaultColorId,
            comments = "Some comments"
        });

        var response = await apiFactory.PutAdminItem(1, new
        {
            code = code,
            name = name,
            colorId = DefaultColorId,
            comments = comments
        });

        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(AdminItemId.Create(1), new AdminItem(
            code!,
            name!,
            comments!,
            DefaultColor));
    }
    
    [Fact]
    public async Task put_a_valid_admin_item_when_there_are_many_admin_items()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1, 2, 3);
        await apiFactory.ThereIsAnAdminItem(new
        {
            code = "ABCA123",
            name = "Some item name",
            colorId = DefaultColorId,
            comments = "Some comments"
        });
        await apiFactory.ThereIsAnAdminItem(new
        {
            code = "GFDS123",
            name = "Second admin item",
            colorId = DefaultColorId,
            comments = "Another comment"
        });
        await apiFactory.ThereIsAnAdminItem(new
        {
            code = "HGFF312",
            name = "Thrid one",
            colorId = DefaultColorId,
            comments = "Yet another comment"
        });
        
        var response = await apiFactory.PutAdminItem(2, new
        {
            code = "UUA123",
            name = "Updated name",
            colorId = DefaultColorId,
            comments = "Some update comment"
        });

        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(AdminItemId.Create(2), new AdminItem(
            "UUA123",
            "Updated name",
            "Some update comment",
            DefaultColor));
        adminItemsStore.Should().NotContain(AdminItemId.Create(2), new AdminItem(
            "GFDS123",
            "Second admin item",
            "Another comment",
            DefaultColor));
    }
    
    [Fact]
    public async Task admin_item_with_200_chars_in_name()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1);
        var id = await apiFactory.ThereIsAnAdminItem(new
        {
            code = "DSA123",
            name = "Some item name",
            colorId = DefaultColorId,
            comments = "Some comments"
        });

        var name = new string('A', 200);
        var request = new
        {
            code = "ADSA321A",
            name = name,
            colorId = DefaultColorId,
            comments = "200 characters admin item"
        };
        var response = await apiFactory.PutAdminItem(id, request);

        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(AdminItemId.Create(1), new AdminItem(
            request.code!,
            request.name!,
            request.comments!,
            DefaultColor));
    }

    [Fact]
    public async Task null_comments_are_normalized_to_empty_string()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1);
        var id = await apiFactory.ThereIsAnAdminItem(new
        {
            code = "DSA123",
            name = "Some item name",
            colorId = DefaultColorId,
            comments = "Some comments"
        });

        var request = new
        {
            code = "DSAD123",
            name = "Some admin X item",
            colorId = DefaultColorId,
            comments = (string?)null
        };
        var response = await apiFactory.PutAdminItem(id, request);

        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(AdminItemId.Create(1), new AdminItem(
            request.code,
            request.name,
            "",
            DefaultColor));
    }
    
    [Theory]
    [InlineData(null, "Second admin item", "This is second admin item", "Code")]
    [InlineData("", "Second admin item", "This is second admin item", "Code")]
    [InlineData("321ADFA", null, "Another description", "Name")]
    [InlineData("321ADFA", "", "Another description", "Name")]
    public async Task invalid_admin_item(string? code, string? name, string? comments, string expectedField)
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1);

        var request = new
        {
            code = code,
            name = name,
            colorId = DefaultColorId,
            comments = comments
        };
        var response = await apiFactory.PutAdminItem(1, request);

        response.Should().Be400BadRequest()
            .And.HaveError(expectedField, "*required*");
    }
    
    [Theory]
    [InlineData("ASADBDAS1235X")]
    [InlineData("GSGEZDAS1235YZ")]
    public async Task code_cannot_have_more_than_12_characters(string code)
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1);

        var request = new
        {
            code = code,
            name = "NotRelevant",
            colorId = DefaultColorId,
            comments = "NotRelevant"
        };
        var response = await apiFactory.PutAdminItem(1, request);

        response.Should().Be400BadRequest()
            .And.HaveError("Code", "*max*");
    }

    [Fact]
    public async Task name_cannot_have_more_than_200_characters()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1);

        var name = new string('X', 200);
        var request = new
        {
            code = "ASDBD123",
            name = name + "Y",
            colorId = DefaultColorId,
            comments = "NotRelevant"
        };
        var response = await apiFactory.PutAdminItem(1, request);

        response.Should().Be400BadRequest()
            .And.HaveError("Name", "*max*");
    }

    [Fact]
    public async Task color_is_updated()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApiWith(adminItemsStore,
            new Color(1, "puce"),
            new Color(2, "rust"),
            new Color(3, "ruby"));
        apiFactory.WillGenerateAdminItemId(1);
        var id = await apiFactory.ThereIsAnAdminItem(new
        {
            code = "GAD1235",
            name = "Some X item name",
            colorId = 1,
            comments = "Some X item comments"
        });

        var response = await apiFactory.PutAdminItem(id, new
        {
            code = "GAD1235",
            name = "Some X item name",
            colorId = 3,
            comments = "Some X item comments"
        });
        response.Should().Be200Ok();

        adminItemsStore.Should().Contain(id, new AdminItem(
            "GAD1235", "Some X item name", "Some X item comments", "ruby"));
    }
    
    [Fact]
    public async Task invalid_request_when_color_is_not_in_store()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApiWith(adminItemsStore,
            new Color(1, "puce"),
            new Color(2, "rust"),
            new Color(3, "ruby"));
        apiFactory.WillGenerateAdminItemId(1);
        var id = await apiFactory.ThereIsAnAdminItem(new
        {
            code = "GAD1235",
            name = "Some X item name",
            colorId = 1,
            comments = "Some X item comments"
        });

        var response = await apiFactory.PutAdminItem(id, new
        {
            code = "GAD1235",
            name = "Some X item name",
            colorId = 133,
            comments = "Some X item comments"
        });

        response.Should().Be400BadRequest()
            .And.HaveError("ColorId", "*not found*");
    }
}