using AdminItems.Api.AdminItems;
using AdminItems.Api.Colors;
using AdminItems.Tests.Fakes;
using FluentAssertions;
using static AdminItems.Tests.Shared.Fixtures;

namespace AdminItems.Tests;

public class CreateAdminItemEndpointTests
{
    [Theory]
    [InlineData("GFJS1234", "First Admin Item", "This is a first admin item in system")]
    [InlineData("YTRA1235", "Another admin Item", "")]
    [InlineData("1234567890AB", "Another one", "Admin item with max 12 characters")]
    public async Task valid_admin_item_is_added_to_store(string? code, string? name, string? comments)
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1);

        var request = ARequest(code, name, comments);
        var response = await apiFactory.PostAdminItem(request);

        response.Should().Be201Created();
        adminItemsStore.Should().Contain(AdminItemId.Create(1), new AdminItem(
            request.Code!,
            request.Name!,
            request.Comments!,
            DefaultColor));
    }

    [Fact]
    public async Task valid_admin_item_gets_assigned_id()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(5);
        
        var request = ARequest("ABBAA123", "Some admin item name2", "Not so long comment");
        var response = await apiFactory.PostAdminItem(request);

        response.Should().Be201Created()
            .And.BeAs(new
            {
                id = 5
            });
    }
    
    [Fact]
    public async Task admin_item_with_200_chars_in_name_is_added_to_store()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApi(adminItemsStore);
        apiFactory.WillGenerateAdminItemId(1);

        var name = new string('A', 200);
        var request = ARequest(
            "ADSA321A",
            name,
            "200 characters admin item"
        );
        var response = await apiFactory.PostAdminItem(request);

        response.Should().Be201Created();
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

        var request = ARequest(
            "DSAD123",
            "Some admin X item",
            null
        );
        var response = await apiFactory.PostAdminItem(request);

        response.Should().Be201Created();
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

        var request = ARequest(code, name, comments);
        var response = await apiFactory.PostAdminItem(request);

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

        var request = ARequest(code, "NotRelevant", "NotRelevant");
        var response = await apiFactory.PostAdminItem(request);

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
        var request = ARequest("ASDBD123", name + 'Y', "NotRelevant");
        var response = await apiFactory.PostAdminItem(request);

        response.Should().Be400BadRequest()
            .And.HaveError("Name", "*max*");
    }

    [Fact]
    public async Task can_choose_color_from_store()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApiWith(adminItemsStore,
            new Color(2, "midnight"));
        apiFactory.WillGenerateAdminItemId(1);
        
        var request = ARequestWithColor(
            "DSAG1235",
            "Y admin item",
            "Some not important comment",
            2
        );
        var response = await apiFactory.PostAdminItem(request);

        response.Should().Be201Created();
        adminItemsStore.Should().Contain(AdminItemId.Create(1), new AdminItem(
            request.Code!,
            request.Name!,
            request.Comments!,
            "midnight"));
    }

    [Fact]
    public async Task invalid_request_when_color_is_not_in_store()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = AnAdminItemsApiWith(adminItemsStore,
            new Color(1,"indigo"),
            new Color(7,"papaya"));
        apiFactory.WillGenerateAdminItemId(1);

        var request = ARequestWithColor(
            "DSAG1235",
            "Y admin item",
            "Some not important comment",
            5
        );
        var response = await apiFactory.PostAdminItem(request);

        response.Should().Be400BadRequest()
            .And.HaveError("ColorId", "*not found*");
    }

    private static Request ARequest(
        string? code, string? name, string? comments) =>
        new (code, name, comments, DefaultColorId);
    private static Request ARequestWithColor(
        string? code, string? name, string? comments, long colorId) =>
        new (code, name, comments, colorId);
    
    private record Request(string? Code, string? Name, string? Comments, long ColorId);
}