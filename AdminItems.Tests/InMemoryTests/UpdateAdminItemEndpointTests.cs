using AdminItems.Api.AdminItems;
using AdminItems.Api.Colors;
using AdminItems.Tests.Fakes;
using FluentAssertions;
using static AdminItems.Tests.Shared.Fixtures;

namespace AdminItems.Tests.InMemoryTests;

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
        await apiFactory.ThereIsAnAdminItem(ARequest(
            "ABCA123",
            "Some item name",
            "Some comments"
        ));

        var response = await apiFactory.PutAdminItem(1, ARequest(
            code,
            name,
            comments
        ));

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
        await apiFactory.ThereIsAnAdminItem(ARequest(
            "ABCA123",
            "Some item name",
            "Some comments"
        ));
        await apiFactory.ThereIsAnAdminItem(ARequest(
            "GFDS123",
            "Second admin item",
            "Another comment"
        ));
        await apiFactory.ThereIsAnAdminItem(ARequest(
            "HGFF312",
            "Thrid one",
            "Yet another comment"
        ));
        
        var response = await apiFactory.PutAdminItem(2, ARequest(
            "UUA123",
            "Updated name",
            "Some update comment"
        ));

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
        var id = await apiFactory.ThereIsAnAdminItem(ARequest(
            "DSA123",
            "Some item name",
            "Some comments"
        ));

        var name = new string('A', 200);
        var request = ARequest(
            "ADSA321A",
            name,
            "200 characters admin item"
        );
        var response = await apiFactory.PutAdminItem(id, request);

        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(AdminItemId.Create(1), new AdminItem(
            request.Code!,
            request.Name!,
            request.Comments!,
            DefaultColor));
    }

    [Fact]
    public async Task null_comments_are_normalized_to_empty_string()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1);
        var id = await apiFactory.ThereIsAnAdminItem(ARequest(
            "DSA123",
            "Some item name",
            "Some comments"
        ));

        var request = ARequest(
            "DSAD123",
            "Some admin X item",
            null
        );
        var response = await apiFactory.PutAdminItem(id, request);

        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(AdminItemId.Create(1), new AdminItem(
            request.Code!,
            request.Name!,
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

        var request = ARequest(
            code,
            name,
            comments
        );
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

        var request = ARequest(
            code,
            "NotRelevant",
            "NotRelevant"
        );
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
        var request = ARequest(
            "ASDBD123",
            name + "Y",
            "NotRelevant"
        );
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
        var id = await apiFactory.ThereIsAnAdminItem(ARequestWithColor(
            "GAD1235",
            "Some X item name",
            "Some X item comments",
            1
        ));

        var response = await apiFactory.PutAdminItem(id, ARequestWithColor(
            "GAD1235",
            "Some X item name",
            "Some X item comments",
            3
        ));
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
        var id = await apiFactory.ThereIsAnAdminItem(ARequestWithColor(
            "GAD1235",
            "Some X item name",
            "Some X item comments",
            1
        ));

        var response = await apiFactory.PutAdminItem(id, ARequestWithColor(
            "GAD1235",
            "Some X item name",
            "Some X item comments",
            133
        ));

        response.Should().Be400BadRequest()
            .And.HaveError("ColorId", "*not found*");
    }

    [Fact]
    public async Task cannot_update_not_existing_admin_item()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1);
        var id = await apiFactory.ThereIsAnAdminItem(ARequest(
            "GAD1235",
            "Some X item name",
            "Some X item comments"
        ));
        
        var response = await apiFactory.PutAdminItem(315, ARequest(
            "FDAS123",
            "Whatever it should fail anyways",
            "Whatever it should fail anyways"
        ));

        response.Should().Be404NotFound();
    }
    
    private static Request ARequest(
        string? code, string? name, string? comments) =>
        new (code, name, comments, DefaultColorId);
    private static Request ARequestWithColor(
        string? code, string? name, string? comments, long colorId) =>
        new (code, name, comments, colorId);
    
    private record Request(string? Code, string? Name, string? Comments, long ColorId);
}