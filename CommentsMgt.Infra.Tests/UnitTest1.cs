using Content_App_POC.CommentsMgt;
using CommentsMgt.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CommentsMgt.Infra.Tests;

public class CommentRepositoryTests : IDisposable
{
    private readonly CommentsMgtContext _context;
    private readonly CommentRepository _repository;

    public CommentRepositoryTests()
    {
        // Create in-memory database with unique name for each test
        var options = new DbContextOptionsBuilder<CommentsMgtContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CommentsMgtContext(options);
        _repository = new CommentRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region UpdateCommentStatusAsync Tests

    [Fact]
    public async Task UpdateCommentStatusAsync_PendingStatus_SetsShownInPortalFalse()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Test comment",
            CommentStatusId = (int)CommentStatusesEnum.Approved,
            ShownInPortal = true
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.UpdateCommentStatusAsync(comment.Id, (int)CommentStatusesEnum.Pending);

        // Assert
        var updated = await _context.Comments.FindAsync(comment.Id);
        Assert.NotNull(updated);
        Assert.Equal((int)CommentStatusesEnum.Pending, updated.CommentStatusId);
        Assert.False(updated.ShownInPortal);
    }

    [Fact]
    public async Task UpdateCommentStatusAsync_RejectedStatus_SetsShownInPortalFalse()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Test comment",
            CommentStatusId = (int)CommentStatusesEnum.Approved,
            ShownInPortal = true
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.UpdateCommentStatusAsync(comment.Id, (int)CommentStatusesEnum.Rejected);

        // Assert
        var updated = await _context.Comments.FindAsync(comment.Id);
        Assert.NotNull(updated);
        Assert.Equal((int)CommentStatusesEnum.Rejected, updated.CommentStatusId);
        Assert.False(updated.ShownInPortal);
    }

    [Fact]
    public async Task UpdateCommentStatusAsync_ApprovedStatus_SetsShownInPortalTrue()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Test comment",
            CommentStatusId = (int)CommentStatusesEnum.Pending,
            ShownInPortal = false
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.UpdateCommentStatusAsync(comment.Id, (int)CommentStatusesEnum.Approved);

        // Assert
        var updated = await _context.Comments.FindAsync(comment.Id);
        Assert.NotNull(updated);
        Assert.Equal((int)CommentStatusesEnum.Approved, updated.CommentStatusId);
        Assert.True(updated.ShownInPortal);
    }

    [Fact]
    public async Task UpdateCommentStatusAsync_AnyStatus_UpdatesModifiedOn()
    {
        // Arrange
        var originalDate = DateTime.UtcNow.AddDays(-1);
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Test comment",
            CommentStatusId = (int)CommentStatusesEnum.Pending,
            ModifiedOn = originalDate
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        var beforeUpdate = DateTime.UtcNow;

        // Act
        await _repository.UpdateCommentStatusAsync(comment.Id, (int)CommentStatusesEnum.Approved);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        var updated = await _context.Comments.FindAsync(comment.Id);
        Assert.NotNull(updated);
        Assert.InRange(updated.ModifiedOn, beforeUpdate, afterUpdate);
        Assert.NotEqual(originalDate, updated.ModifiedOn);
    }

    [Fact]
    public async Task UpdateCommentStatusAsync_NonExistentId_DoesNotThrow()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert - Should not throw
        await _repository.UpdateCommentStatusAsync(nonExistentId, (int)CommentStatusesEnum.Approved);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_PendingStatus_ForcesShownInPortalFalse()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Test comment",
            CommentStatusId = (int)CommentStatusesEnum.Pending,
            ShownInPortal = true // Try to set it true
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        // Modify and try to keep ShownInPortal true
        comment.CommentText = "Updated comment";
        comment.ShownInPortal = true;

        // Act
        await _repository.UpdateAsync(comment);

        // Assert
        var updated = await _context.Comments.FindAsync(comment.Id);
        Assert.NotNull(updated);
        Assert.False(updated.ShownInPortal); // Should be forced to false
    }

    [Fact]
    public async Task UpdateAsync_RejectedStatus_ForcesShownInPortalFalse()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Test comment",
            CommentStatusId = (int)CommentStatusesEnum.Rejected,
            ShownInPortal = true // Try to set it true
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        comment.CommentText = "Updated comment";
        comment.ShownInPortal = true;

        // Act
        await _repository.UpdateAsync(comment);

        // Assert
        var updated = await _context.Comments.FindAsync(comment.Id);
        Assert.NotNull(updated);
        Assert.False(updated.ShownInPortal); // Should be forced to false
    }

    [Fact]
    public async Task UpdateAsync_ApprovedStatus_AllowsShownInPortalTrue()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Test comment",
            CommentStatusId = (int)CommentStatusesEnum.Approved,
            ShownInPortal = true
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        comment.CommentText = "Updated comment";

        // Act
        await _repository.UpdateAsync(comment);

        // Assert
        var updated = await _context.Comments.FindAsync(comment.Id);
        Assert.NotNull(updated);
        Assert.True(updated.ShownInPortal); // Should remain true
    }

    [Fact]
    public async Task UpdateAsync_ApprovedStatus_AllowsShownInPortalFalse()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Test comment",
            CommentStatusId = (int)CommentStatusesEnum.Approved,
            ShownInPortal = false
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        comment.CommentText = "Updated comment";

        // Act
        await _repository.UpdateAsync(comment);

        // Assert
        var updated = await _context.Comments.FindAsync(comment.Id);
        Assert.NotNull(updated);
        Assert.False(updated.ShownInPortal); // Should remain false
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ValidId_SetsIsDeletedTrue()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Test comment",
            IsDeleted = false
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(comment.Id);

        // Assert
        var deleted = await _context.Comments.FindAsync(comment.Id);
        Assert.NotNull(deleted); // Still exists in database
        Assert.True(deleted.IsDeleted); // But marked as deleted
    }

    [Fact]
    public async Task DeleteAsync_ValidId_DoesNotRemoveFromDatabase()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Test comment"
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        var countBefore = await _context.Comments.CountAsync();

        // Act
        await _repository.DeleteAsync(comment.Id);

        // Assert
        var countAfter = await _context.Comments.CountAsync();
        Assert.Equal(countBefore, countAfter); // Count should be same (soft delete)
    }

    [Fact]
    public async Task DeleteAsync_InvalidId_DoesNotThrow()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert - Should not throw
        await _repository.DeleteAsync(nonExistentId);
    }

    #endregion

    #region CascadeStatusToChildrenAsync Tests

    [Fact]
    public async Task CascadeStatusToChildrenAsync_HasDirectChildren_UpdatesAll()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parent = new Comment
        {
            Id = parentId,
            ContentId = 100,
            CommentText = "Parent",
            CommentStatusId = (int)CommentStatusesEnum.Approved,
            ShownInPortal = true
        };

        var child1 = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Child 1",
            ParentId = parentId,
            CommentStatusId = (int)CommentStatusesEnum.Pending,
            ShownInPortal = false
        };

        var child2 = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Child 2",
            ParentId = parentId,
            CommentStatusId = (int)CommentStatusesEnum.Pending,
            ShownInPortal = false
        };

        await _context.Comments.AddRangeAsync(parent, child1, child2);
        await _context.SaveChangesAsync();

        // Act
        await _repository.CascadeStatusToChildrenAsync(parentId, (int)CommentStatusesEnum.Approved);

        // Assert
        var updatedChild1 = await _context.Comments.FindAsync(child1.Id);
        var updatedChild2 = await _context.Comments.FindAsync(child2.Id);

        Assert.Equal((int)CommentStatusesEnum.Approved, updatedChild1.CommentStatusId);
        Assert.Equal((int)CommentStatusesEnum.Approved, updatedChild2.CommentStatusId);
        Assert.True(updatedChild1.ShownInPortal);
        Assert.True(updatedChild2.ShownInPortal);
    }

    [Fact]
    public async Task CascadeStatusToChildrenAsync_HasGrandchildren_UpdatesRecursively()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var parent = new Comment
        {
            Id = parentId,
            ContentId = 100,
            CommentText = "Parent",
            CommentStatusId = (int)CommentStatusesEnum.Approved
        };

        var child = new Comment
        {
            Id = childId,
            ContentId = 100,
            CommentText = "Child",
            ParentId = parentId,
            CommentStatusId = (int)CommentStatusesEnum.Pending
        };

        var grandchild = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Grandchild",
            ParentId = childId,
            CommentStatusId = (int)CommentStatusesEnum.Pending
        };

        await _context.Comments.AddRangeAsync(parent, child, grandchild);
        await _context.SaveChangesAsync();

        // Act
        await _repository.CascadeStatusToChildrenAsync(parentId, (int)CommentStatusesEnum.Rejected);

        // Assert
        var updatedChild = await _context.Comments.FindAsync(child.Id);
        var updatedGrandchild = await _context.Comments.FindAsync(grandchild.Id);

        Assert.Equal((int)CommentStatusesEnum.Rejected, updatedChild.CommentStatusId);
        Assert.Equal((int)CommentStatusesEnum.Rejected, updatedGrandchild.CommentStatusId);
        Assert.False(updatedChild.ShownInPortal);
        Assert.False(updatedGrandchild.ShownInPortal);
    }

    [Fact]
    public async Task CascadeStatusToChildrenAsync_RejectedStatus_SetsAllChildrenShownInPortalFalse()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parent = new Comment { Id = parentId, ContentId = 100, CommentText = "Parent" };
        var child1 = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Child 1",
            ParentId = parentId,
            ShownInPortal = true
        };
        var child2 = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Child 2",
            ParentId = parentId,
            ShownInPortal = true
        };

        await _context.Comments.AddRangeAsync(parent, child1, child2);
        await _context.SaveChangesAsync();

        // Act
        await _repository.CascadeStatusToChildrenAsync(parentId, (int)CommentStatusesEnum.Rejected);

        // Assert
        var updatedChild1 = await _context.Comments.FindAsync(child1.Id);
        var updatedChild2 = await _context.Comments.FindAsync(child2.Id);

        Assert.False(updatedChild1.ShownInPortal);
        Assert.False(updatedChild2.ShownInPortal);
    }

    [Fact]
    public async Task CascadeStatusToChildrenAsync_ApprovedStatus_SetsAllChildrenShownInPortalTrue()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parent = new Comment { Id = parentId, ContentId = 100, CommentText = "Parent" };
        var child1 = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Child 1",
            ParentId = parentId,
            ShownInPortal = false
        };
        var child2 = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Child 2",
            ParentId = parentId,
            ShownInPortal = false
        };

        await _context.Comments.AddRangeAsync(parent, child1, child2);
        await _context.SaveChangesAsync();

        // Act
        await _repository.CascadeStatusToChildrenAsync(parentId, (int)CommentStatusesEnum.Approved);

        // Assert
        var updatedChild1 = await _context.Comments.FindAsync(child1.Id);
        var updatedChild2 = await _context.Comments.FindAsync(child2.Id);

        Assert.True(updatedChild1.ShownInPortal);
        Assert.True(updatedChild2.ShownInPortal);
    }

    [Fact]
    public async Task CascadeStatusToChildrenAsync_NoChildren_DoesNotThrow()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parent = new Comment { Id = parentId, ContentId = 100, CommentText = "Parent" };
        await _context.Comments.AddAsync(parent);
        await _context.SaveChangesAsync();

        // Act & Assert - Should not throw
        await _repository.CascadeStatusToChildrenAsync(parentId, (int)CommentStatusesEnum.Approved);
    }

    [Fact]
    public async Task CascadeStatusToChildrenAsync_UpdatesModifiedOn()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var originalDate = DateTime.UtcNow.AddDays(-1);

        var parent = new Comment { Id = parentId, ContentId = 100, CommentText = "Parent" };
        var child = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Child",
            ParentId = parentId,
            ModifiedOn = originalDate
        };

        await _context.Comments.AddRangeAsync(parent, child);
        await _context.SaveChangesAsync();

        var beforeUpdate = DateTime.UtcNow;

        // Act
        await _repository.CascadeStatusToChildrenAsync(parentId, (int)CommentStatusesEnum.Approved);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        var updatedChild = await _context.Comments.FindAsync(child.Id);
        Assert.InRange(updatedChild.ModifiedOn, beforeUpdate, afterUpdate);
        Assert.NotEqual(originalDate, updatedChild.ModifiedOn);
    }

    [Fact]
    public async Task CascadeStatusToChildrenAsync_IgnoresDeletedChildren()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var parent = new Comment { Id = parentId, ContentId = 100, CommentText = "Parent" };
        var activeChild = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Active Child",
            ParentId = parentId,
            IsDeleted = false,
            CommentStatusId = (int)CommentStatusesEnum.Pending
        };
        var deletedChild = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Deleted Child",
            ParentId = parentId,
            IsDeleted = true,
            CommentStatusId = (int)CommentStatusesEnum.Pending
        };

        await _context.Comments.AddRangeAsync(parent, activeChild, deletedChild);
        await _context.SaveChangesAsync();

        // Act
        await _repository.CascadeStatusToChildrenAsync(parentId, (int)CommentStatusesEnum.Approved);

        // Assert
        var updatedActiveChild = await _context.Comments.FindAsync(activeChild.Id);
        var updatedDeletedChild = await _context.Comments.FindAsync(deletedChild.Id);

        Assert.Equal((int)CommentStatusesEnum.Approved, updatedActiveChild.CommentStatusId);
        Assert.Equal((int)CommentStatusesEnum.Pending, updatedDeletedChild.CommentStatusId); // Should not change
    }

    #endregion
}
