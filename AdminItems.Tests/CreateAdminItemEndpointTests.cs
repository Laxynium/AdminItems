using AdminItems.Api.AdminItems;
using AdminItems.Api.Colors;
using AdminItems.Tests.Fakes;
using AdminItems.Tests.Shared;
using FluentAssertions;

namespace AdminItems.Tests;

public class CreateAdminItemEndpointTests
{
    private const long DefaultColorId = 1;
    private const string DefaultColor = "indigo";
    [Theory]
    [InlineData("GFJS1234", "First Admin Item", "This is a first admin item in system")]
    [InlineData("YTRA1235", "Another admin Item", "")]
    [InlineData("1234567890AB", "Another one", "Admin item with max 12 characters")]
    public async Task valid_admin_item_is_added_to_store(string? code, string? name, string? comments)
    {
        var colorsStore = new InMemoryColorsStore();
        colorsStore.AddColors(new []{new Color(DefaultColorId, DefaultColor)});
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(adminItemsStore);
        apiFactory.UseStore(colorsStore);

        var request = new
        {
            code, 
            name, 
            colorId = DefaultColorId,
            comments
        };
        var response = await apiFactory.PostAdminItem(request);

        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(new AdminItem(
            request.code!,
            request.name!,
            request.comments!,
            DefaultColor));
    }
    
    [Fact]
    public async Task admin_item_with_200_chars_in_name_is_added_to_store()
    {
        var colorsStore = new InMemoryColorsStore();
        colorsStore.AddColors(new []{new Color(DefaultColorId, DefaultColor)});
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(adminItemsStore);
        apiFactory.UseStore(colorsStore);

        var name = new string('A', 200);
        var request = new
        {
            code = "ADSA321A", 
            name,
            colorId = DefaultColorId,
            comments = "200 characters admin item"
        };
        var response = await apiFactory.PostAdminItem(request);
        
        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(new AdminItem(
            request.code!,
            request.name!,
            request.comments!,
            DefaultColor));
    }
    
    [Fact]
    public async Task null_comments_are_normalized_to_empty_string()
    {
        var colorsStore = new InMemoryColorsStore();
        colorsStore.AddColors(new []{new Color(DefaultColorId, DefaultColor)});
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(adminItemsStore);
        apiFactory.UseStore(colorsStore);
        
        var request = new
        {
            code = "DSAD123", 
            name = "Some admin X item",
            colorId = DefaultColorId,
            comments = (string)null!
        };
        var response = await apiFactory.PostAdminItem(request);
        
        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(new AdminItem(
            request.code!,
            request.name!,
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
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(adminItemsStore);
        
        var request = new { code, name, comments };
        var response = await apiFactory.PostAdminItem(request);

        response.Should().Be400BadRequest()
            .And.HaveError(expectedField,"*required*");
    }

    [Theory]
    [InlineData("ASADBDAS1235X")]
    [InlineData("GSGEZDAS1235YZ")]
    public async Task code_cannot_have_more_than_12_characters(string code)
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(adminItemsStore);
        
        var request = new { code, name = "NotRelevant", comments = "NotRelevant" };
        var response = await apiFactory.PostAdminItem(request);
        
        response.Should().Be400BadRequest()
            .And.HaveError("Code","*max*");
    }
    
    [Fact]
    public async Task name_cannot_have_more_than_200_characters()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(adminItemsStore);

        var name = new string('X', 200);
        var request = new { code = "ASDBD123", name = name+'Y', comments = "NotRelevant" };
        var response = await apiFactory.PostAdminItem(request);
        
        response.Should().Be400BadRequest()
            .And.HaveError("Name","*max*");
    }

    [Fact]
    public async Task can_choose_color_from_store()
    {
        var colorsStore = new InMemoryColorsStore();
        colorsStore.AddColors(new []
        {
            new Color(2, "midnight")
        });
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(adminItemsStore);
        apiFactory.UseStore(colorsStore);

        var request = new
        {
            code = "DSAG1235",
            name = "Y admin item",
            comments = "Some not important comment",
            colorId = 2
        };
        var response = await apiFactory.PostAdminItem(request);
        
        response.Should().Be200Ok();
        adminItemsStore.Should().Contain(new AdminItem(
            request.code,
            request.name,
            request.comments,
            "midnight"));
    }

    [Fact]
    public async Task invalid_request_when_color_is_not_in_store()
    {
        var colorsStore = new InMemoryColorsStore();
        colorsStore.AddColors(Array.Empty<Color>());
        var adminItemsStore = new InMemoryAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(adminItemsStore);
        apiFactory.UseStore(colorsStore);

        var request = new
        {
            code = "DSAG1235",
            name = "Y admin item",
            comments = "Some not important comment",
            colorId = 5
        };
        var response = await apiFactory.PostAdminItem(request);
        
        response.Should().Be400BadRequest()
            .And.HaveError("ColorId", "*not found*");
    }
}