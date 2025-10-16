using Content_App_POC.CommentsMgt;
using Xunit;

namespace CommentsMgt.DTOs.Tests;

public class PaginatedResultTests
{
    [Fact]
    public void Constructor_SetsItems_Correctly()
    {
        // Arrange
        var items = new List<string> { "Item1", "Item2", "Item3" };
        int totalCount = 10;
        int page = 1;
        int pageSize = 3;

        // Act
        var result = new PaginatedResult<string>(items, totalCount, page, pageSize);

        // Assert
        Assert.Equal(items, result.Items);
    }

    [Fact]
    public void Constructor_SetsTotalCount_Correctly()
    {
        // Arrange
        var items = new List<string> { "Item1", "Item2" };
        int totalCount = 50;
        int page = 2;
        int pageSize = 10;

        // Act
        var result = new PaginatedResult<string>(items, totalCount, page, pageSize);

        // Assert
        Assert.Equal(50, result.TotalCount);
    }

    [Fact]
    public void Constructor_SetsPage_Correctly()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        int totalCount = 100;
        int page = 5;
        int pageSize = 20;

        // Act
        var result = new PaginatedResult<int>(items, totalCount, page, pageSize);

        // Assert
        Assert.Equal(5, result.Page);
    }

    [Fact]
    public void Constructor_SetsPageSize_Correctly()
    {
        // Arrange
        var items = new List<double> { 1.5, 2.5, 3.5 };
        int totalCount = 30;
        int page = 1;
        int pageSize = 15;

        // Act
        var result = new PaginatedResult<double>(items, totalCount, page, pageSize);

        // Assert
        Assert.Equal(15, result.PageSize);
    }

    [Fact]
    public void Constructor_WithEmptyList_StoresEmptyItems()
    {
        // Arrange
        var items = new List<string>();
        int totalCount = 0;
        int page = 1;
        int pageSize = 10;

        // Act
        var result = new PaginatedResult<string>(items, totalCount, page, pageSize);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public void Constructor_WithNullItems_StoresNull()
    {
        // Arrange
        IEnumerable<string> items = null;
        int totalCount = 0;
        int page = 1;
        int pageSize = 10;

        // Act
        var result = new PaginatedResult<string>(items, totalCount, page, pageSize);

        // Assert
        Assert.Null(result.Items);
    }

    [Fact]
    public void Constructor_WithComplexType_StoresCorrectly()
    {
        // Arrange
        var items = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "Test1" },
            new TestModel { Id = 2, Name = "Test2" }
        };
        int totalCount = 20;
        int page = 3;
        int pageSize = 5;

        // Act
        var result = new PaginatedResult<TestModel>(items, totalCount, page, pageSize);

        // Assert
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(20, result.TotalCount);
        Assert.Equal(3, result.Page);
        Assert.Equal(5, result.PageSize);
    }

    [Fact]
    public void Constructor_WithZeroPage_StoresZero()
    {
        // Arrange
        var items = new List<string> { "Item1" };
        int totalCount = 1;
        int page = 0;
        int pageSize = 10;

        // Act
        var result = new PaginatedResult<string>(items, totalCount, page, pageSize);

        // Assert
        Assert.Equal(0, result.Page);
    }

    [Fact]
    public void Constructor_WithNegativeTotalCount_StoresNegative()
    {
        // Arrange
        var items = new List<string>();
        int totalCount = -1;
        int page = 1;
        int pageSize = 10;

        // Act
        var result = new PaginatedResult<string>(items, totalCount, page, pageSize);

        // Assert
        Assert.Equal(-1, result.TotalCount);
    }

    [Fact]
    public void Constructor_AllPropertiesSet_AllValuesCorrect()
    {
        // Arrange
        var items = new List<string> { "A", "B", "C", "D", "E" };
        int totalCount = 100;
        int page = 4;
        int pageSize = 25;

        // Act
        var result = new PaginatedResult<string>(items, totalCount, page, pageSize);

        // Assert
        Assert.Equal(items, result.Items);
        Assert.Equal(100, result.TotalCount);
        Assert.Equal(4, result.Page);
        Assert.Equal(25, result.PageSize);
    }

    // Helper class for testing with complex types
    private class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
