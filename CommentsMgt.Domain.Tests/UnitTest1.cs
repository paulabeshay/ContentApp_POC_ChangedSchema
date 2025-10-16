using Content_App_POC.CommentsMgt;
using Xunit;

namespace CommentsMgt.Domain.Tests;

public class CommentEntityTests
{
    #region Default Value Tests

    [Fact]
    public void NewComment_DefaultId_IsEmptyGuid()
    {
        // Act
        var comment = new Comment();

        // Assert
        Assert.Equal(Guid.Empty, comment.Id);
    }

    [Fact]
    public void NewComment_DefaultShownInPortal_IsFalse()
    {
        // Act
        var comment = new Comment();

        // Assert
        Assert.False(comment.ShownInPortal);
    }

    [Fact]
    public void NewComment_DefaultIsDeleted_IsFalse()
    {
        // Act
        var comment = new Comment();

        // Assert
        Assert.False(comment.IsDeleted);
    }

    [Fact]
    public void NewComment_DefaultCreatedBy_IsAnonymous()
    {
        // Act
        var comment = new Comment();

        // Assert
        Assert.Equal("Anonymous", comment.CreatedBy);
    }

    [Fact]
    public void NewComment_DefaultModifiedBy_IsAnonymous()
    {
        // Act
        var comment = new Comment();

        // Assert
        Assert.Equal("Anonymous", comment.ModifiedBy);
    }

    [Fact]
    public void NewComment_DefaultCreatedOn_IsUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var comment = new Comment();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(comment.CreatedOn, beforeCreation, afterCreation);
    }

    [Fact]
    public void NewComment_DefaultModifiedOn_IsUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var comment = new Comment();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(comment.ModifiedOn, beforeCreation, afterCreation);
    }

    [Fact]
    public void NewComment_DefaultChildren_IsEmptyList()
    {
        // Act
        var comment = new Comment();

        // Assert
        Assert.NotNull(comment.Children);
        Assert.Empty(comment.Children);
    }

    [Fact]
    public void NewComment_DefaultContentParentAlias_IsEmptyString()
    {
        // Act
        var comment = new Comment();

        // Assert
        Assert.Equal(string.Empty, comment.ContentParentAlias);
    }

    [Fact]
    public void NewComment_DefaultCommentText_IsEmptyString()
    {
        // Act
        var comment = new Comment();

        // Assert
        Assert.Equal(string.Empty, comment.CommentText);
    }

    [Fact]
    public void NewComment_DefaultParentId_IsNull()
    {
        // Act
        var comment = new Comment();

        // Assert
        Assert.Null(comment.ParentId);
    }

    [Fact]
    public void NewComment_DefaultCommentStatus_IsNull()
    {
        // Act
        var comment = new Comment();

        // Assert
        Assert.Null(comment.CommentStatus);
    }

    #endregion

    #region Property Assignment Tests

    [Fact]
    public void SetId_StoresValue()
    {
        // Arrange
        var comment = new Comment();
        var expectedId = Guid.NewGuid();

        // Act
        comment.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, comment.Id);
    }

    [Fact]
    public void SetContentId_StoresValue()
    {
        // Arrange
        var comment = new Comment();
        int expectedContentId = 12345;

        // Act
        comment.ContentId = expectedContentId;

        // Assert
        Assert.Equal(expectedContentId, comment.ContentId);
    }

    [Fact]
    public void SetContentParentAlias_StoresValue()
    {
        // Arrange
        var comment = new Comment();
        string expectedAlias = "BlogPost";

        // Act
        comment.ContentParentAlias = expectedAlias;

        // Assert
        Assert.Equal(expectedAlias, comment.ContentParentAlias);
    }

    [Fact]
    public void SetCommentText_StoresValue()
    {
        // Arrange
        var comment = new Comment();
        string expectedText = "This is a test comment";

        // Act
        comment.CommentText = expectedText;

        // Assert
        Assert.Equal(expectedText, comment.CommentText);
    }

    [Fact]
    public void SetCommentStatusId_StoresValue()
    {
        // Arrange
        var comment = new Comment();
        int expectedStatusId = 2;

        // Act
        comment.CommentStatusId = expectedStatusId;

        // Assert
        Assert.Equal(expectedStatusId, comment.CommentStatusId);
    }

    [Fact]
    public void SetParentId_StoresValue()
    {
        // Arrange
        var comment = new Comment();
        var expectedParentId = Guid.NewGuid();

        // Act
        comment.ParentId = expectedParentId;

        // Assert
        Assert.Equal(expectedParentId, comment.ParentId);
    }

    [Fact]
    public void SetShownInPortal_True_StoresTrue()
    {
        // Arrange
        var comment = new Comment();

        // Act
        comment.ShownInPortal = true;

        // Assert
        Assert.True(comment.ShownInPortal);
    }

    [Fact]
    public void SetCreatedBy_StoresValue()
    {
        // Arrange
        var comment = new Comment();
        string expectedUser = "john.doe@example.com";

        // Act
        comment.CreatedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, comment.CreatedBy);
    }

    [Fact]
    public void SetModifiedBy_StoresValue()
    {
        // Arrange
        var comment = new Comment();
        string expectedUser = "jane.smith@example.com";

        // Act
        comment.ModifiedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, comment.ModifiedBy);
    }

    [Fact]
    public void SetCreatedOn_StoresValue()
    {
        // Arrange
        var comment = new Comment();
        var expectedDate = new DateTime(2025, 10, 16, 10, 30, 0, DateTimeKind.Utc);

        // Act
        comment.CreatedOn = expectedDate;

        // Assert
        Assert.Equal(expectedDate, comment.CreatedOn);
    }

    [Fact]
    public void SetModifiedOn_StoresValue()
    {
        // Arrange
        var comment = new Comment();
        var expectedDate = new DateTime(2025, 10, 16, 14, 45, 0, DateTimeKind.Utc);

        // Act
        comment.ModifiedOn = expectedDate;

        // Assert
        Assert.Equal(expectedDate, comment.ModifiedOn);
    }

    [Fact]
    public void SetIsDeleted_True_StoresTrue()
    {
        // Arrange
        var comment = new Comment();

        // Act
        comment.IsDeleted = true;

        // Assert
        Assert.True(comment.IsDeleted);
    }

    [Fact]
    public void SetChildren_StoresValue()
    {
        // Arrange
        var comment = new Comment();
        var child1 = new Comment { Id = Guid.NewGuid(), CommentText = "Child 1" };
        var child2 = new Comment { Id = Guid.NewGuid(), CommentText = "Child 2" };
        var expectedChildren = new List<Comment> { child1, child2 };

        // Act
        comment.Children = expectedChildren;

        // Assert
        Assert.Equal(2, comment.Children.Count);
        Assert.Contains(child1, comment.Children);
        Assert.Contains(child2, comment.Children);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void CreateFullComment_AllPropertiesSet_StoresCorrectly()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var createdDate = DateTime.UtcNow.AddDays(-1);
        var modifiedDate = DateTime.UtcNow;

        // Act
        var comment = new Comment
        {
            Id = commentId,
            ContentId = 100,
            ContentParentAlias = "NewsArticle",
            CommentText = "Great article!",
            CommentStatusId = 1,
            ParentId = parentId,
            ShownInPortal = true,
            CreatedBy = "user@example.com",
            CreatedOn = createdDate,
            ModifiedBy = "admin@example.com",
            ModifiedOn = modifiedDate,
            IsDeleted = false
        };

        // Assert
        Assert.Equal(commentId, comment.Id);
        Assert.Equal(100, comment.ContentId);
        Assert.Equal("NewsArticle", comment.ContentParentAlias);
        Assert.Equal("Great article!", comment.CommentText);
        Assert.Equal(1, comment.CommentStatusId);
        Assert.Equal(parentId, comment.ParentId);
        Assert.True(comment.ShownInPortal);
        Assert.Equal("user@example.com", comment.CreatedBy);
        Assert.Equal(createdDate, comment.CreatedOn);
        Assert.Equal("admin@example.com", comment.ModifiedBy);
        Assert.Equal(modifiedDate, comment.ModifiedOn);
        Assert.False(comment.IsDeleted);
    }

    [Fact]
    public void CreateParentWithChildren_HierarchyEstablished()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parent = new Comment
        {
            Id = parentId,
            ContentId = 200,
            CommentText = "Parent comment",
            ParentId = null
        };

        var child1 = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 200,
            CommentText = "Child comment 1",
            ParentId = parentId
        };

        var child2 = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 200,
            CommentText = "Child comment 2",
            ParentId = parentId
        };

        // Act
        parent.Children = new List<Comment> { child1, child2 };

        // Assert
        Assert.Null(parent.ParentId);
        Assert.Equal(2, parent.Children.Count);
        Assert.All(parent.Children, child => Assert.Equal(parentId, child.ParentId));
    }

    [Fact]
    public void CommentText_LongText_StoresCorrectly()
    {
        // Arrange
        var comment = new Comment();
        var longText = new string('A', 1000);

        // Act
        comment.CommentText = longText;

        // Assert
        Assert.Equal(1000, comment.CommentText.Length);
        Assert.Equal(longText, comment.CommentText);
    }

    [Fact]
    public void MultipleComments_DifferentIds_AreUnique()
    {
        // Act
        var comment1 = new Comment { Id = Guid.NewGuid() };
        var comment2 = new Comment { Id = Guid.NewGuid() };
        var comment3 = new Comment { Id = Guid.NewGuid() };

        // Assert
        Assert.NotEqual(comment1.Id, comment2.Id);
        Assert.NotEqual(comment2.Id, comment3.Id);
        Assert.NotEqual(comment1.Id, comment3.Id);
    }

    #endregion
}
