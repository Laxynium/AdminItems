﻿using AdminItems.Tests.Fakes;
using FluentAssertions;
using static AdminItems.Tests.Shared.Fixtures;

namespace AdminItems.Tests;

public class GetAdminItemsEndpointTests
{
    [Theory]
    [InlineData("ADBD123", "Some random admin item name", null)]
    [InlineData("432DAD", "Another random one", "Some descriptive comment")]
    public async Task response_contains_created_admin_item(string code, string name, string? comments)
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var api = AnAdminItemsApi(adminItemsStore);
        await api.ThereIsAnAdminItem(new
        {
            code,
            name,
            colorId = DefaultColorId,
            comments
        });
        
        var response = await api.GetAdminItems();
        
        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new
                    {
                        code,
                        name,
                        color = DefaultColor,
                        comments = comments ?? string.Empty
                    }
                }
            });
    }

    [Fact]
    public async Task by_default_admin_items_are_ordered_by_code()
    {
        var adminItemsStore = new InMemoryAdminItemsStore();
        var api = AnAdminItemsApi(adminItemsStore);
        await api.ThereIsAnAdminItem(new
        {
            code = "BBB13",
            name = "Admin Item1",
            colorId = DefaultColorId,
            comments = "Some comment 1"
        });
        await api.ThereIsAnAdminItem(new
        {
            code = "BBB12",
            name = "Admin Item2",
            colorId = DefaultColorId,
            comments = "Some comment 2"
        });
        await api.ThereIsAnAdminItem(new
        {
            code = "AAA11",
            name = "Admin Item3",
            colorId = DefaultColorId,
            comments = "Some comment 3"
        });
        
        var response = await api.GetAdminItems();
        
        response.Should().Be200Ok()
            .And.BeAs(new
            {
                items = new[]
                {
                    new
                    {
                        code = "AAA11",
                        name = "Admin Item3",
                        color = DefaultColor,
                        comments = "Some comment 3"
                    },
                    new
                    {
                        code = "BBB12",
                        name = "Admin Item2",
                        color = DefaultColor,
                        comments = "Some comment 2"
                    },
                    new
                    {
                        code = "BBB13",
                        name = "Admin Item1",
                        color = DefaultColor,
                        comments = "Some comment 1"
                    }
                }
            }, opt => opt.WithStrictOrderingFor(x => x.items));
    }
}