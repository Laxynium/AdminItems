using AdminItems.Api;
using FluentAssertions;

namespace AdminItems.Tests;

public class CreateAdminItemEndpointTests
{
    [Theory]
    [InlineData("GFJS1234", "First Admin Item", "This is a first admin item in system")]
    [InlineData("YTRA1235", "Another admin Item", "")]
    [InlineData("1234567890AB", "Another one", "Admin item with max 12 characters")]
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
    public async Task admin_item_with_200_chars_in_name_is_valid()
    {
        var fakeStore = new FakeAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(fakeStore);

        var name = new string('A', 200);
        var request = new { code = "ADSA321A", name, comments = "200 characters admin item" };
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

    [Theory]
    [InlineData("ASADBDAS1235X")]
    [InlineData("GSGEZDAS1235YZ")]
    public async Task code_cannot_have_more_than_12_characters(string code)
    {
        var fakeStore = new FakeAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(fakeStore);
        
        var request = new { code, name = "NotRelevant", comments = "NotRelevant" };
        var response = await apiFactory.PostAdminItem(request);
        
        response.Should().Be400BadRequest()
            .And.HaveError("Code","*max*");
    }
    
    [Fact]
    public async Task name_cannot_have_more_than_200_characters()
    {
        var fakeStore = new FakeAdminItemsStore();
        var apiFactory = new AdminItemsApi();
        apiFactory.UseStore(fakeStore);

        var name = new string('X', 200);
        var request = new { code = "ASDBD123", name = name+'Y', comments = "NotRelevant" };
        var response = await apiFactory.PostAdminItem(request);
        
        response.Should().Be400BadRequest()
            .And.HaveError("Name","*max*");
    }
}