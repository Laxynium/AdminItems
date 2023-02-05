using AdminItems.Api;
using FluentAssertions;

namespace AdminItems.Tests;

public class CreateAdminItemEndpointTests
{
    [Theory]
    [InlineData("GFJS1234", "First Admin Item", "This is a first admin item in system")]
    [InlineData("YTRA1235", "Another admin Item", "")]
    public async Task valid_admin_item_is_added_to_store(string? code, string? name, string? comments)
    {
        var fakeStore = new FakeAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(fakeStore);

        var request = new { code, name, comments };
        var response = await apiFactory.PostAdminItem(request);

        response.Should().Be200Ok();
        fakeStore.Should().Contain(new AdminItem(
            request.code!,
            request.name!,
            request.comments!));
    }

    [Fact]
    public async Task null_comments_are_normalized_to_empty_string()
    {
        var fakeStore = new FakeAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(fakeStore);
        
        var request = new
        {
            code = "DSAD123", 
            name = "Some admin X item", 
            comments = (string)null!
        };
        var response = await apiFactory.PostAdminItem(request);
        
        response.Should().Be200Ok();
        fakeStore.Should().Contain(new AdminItem(
            request.code!,
            request.name!,
            ""));
    }
    
    [Theory]
    [InlineData(null, "Second admin item", "This is second admin item", "Code")]
    [InlineData("", "Second admin item", "This is second admin item", "Code")]
    [InlineData("321ADFA", null, "Another description", "Name")]
    [InlineData("321ADFA", "", "Another description", "Name")]
    public async Task invalid_admin_item(string? code, string? name, string? comments, string expectedField)
    {
        var fakeStore = new FakeAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(fakeStore);
        
        var request = new { code, name, comments };
        var response = await apiFactory.PostAdminItem(request);

        response.Should().Be400BadRequest()
            .And.HaveError(expectedField,"*required*");
    }
}