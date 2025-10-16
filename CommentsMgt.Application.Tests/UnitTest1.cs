using Content_App_POC.CommentsMgt;
using Moq;
using Xunit;

namespace CommentsMgt.Application.Tests;

public class CommentServiceTests
{
    private readonly Mock<ICommentRepository> _mockRepository;
    private readonly CommentService _service;

    public CommentServiceTests()
    {
        _mockRepository = new Mock<ICommentRepository>();
        _service = new CommentService(_mockRepository.Object);
    }

    #region GetCommentByIdAsync Tests

    [Fact]
    public async Task GetCommentByIdAsync_ValidId_ReturnsComment()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var expectedComment = new Comment { Id = commentId, CommentText = "Test comment" };
        _mockRepository.Setup(r => r.GetByIdAsync(commentId))
            .ReturnsAsync(expectedComment);

        // Act
        var result = await _service.GetCommentByIdAsync(commentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
        Assert.Equal("Test comment", result.CommentText);
        _mockRepository.Verify(r => r.GetByIdAsync(commentId), Times.Once);
    }

    [Fact]
    public async Task GetCommentByIdAsync_NonExistentId_ReturnsNull()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(commentId))
            .ReturnsAsync((Comment)null);

        // Act
        var result = await _service.GetCommentByIdAsync(commentId);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(commentId), Times.Once);
    }

    [Fact]
    public async Task GetCommentByIdAsync_CallsRepository_Once()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Comment)null);

        // Act
        await _service.GetCommentByIdAsync(commentId);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(commentId), Times.Once);
    }

    #endregion

    #region GetCommentsByContentIdAsync Tests

    [Fact]
    public async Task GetCommentsByContentIdAsync_ValidContentId_ReturnsComments()
    {
        // Arrange
        int contentId = 100;
        var expectedComments = new List<Comment>
        {
            new Comment { Id = Guid.NewGuid(), ContentId = contentId, CommentText = "Comment 1" },
            new Comment { Id = Guid.NewGuid(), ContentId = contentId, CommentText = "Comment 2" }
        };
        _mockRepository.Setup(r => r.GetByContentIdAsync(contentId))
            .ReturnsAsync(expectedComments);

        // Act
        var result = await _service.GetCommentsByContentIdAsync(contentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetByContentIdAsync(contentId), Times.Once);
    }

    [Fact]
    public async Task GetCommentsByContentIdAsync_NoComments_ReturnsEmptyList()
    {
        // Arrange
        int contentId = 999;
        _mockRepository.Setup(r => r.GetByContentIdAsync(contentId))
            .ReturnsAsync(new List<Comment>());

        // Act
        var result = await _service.GetCommentsByContentIdAsync(contentId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetByContentIdAsync(contentId), Times.Once);
    }

    #endregion

    #region GetAllCommentsAsync Tests

    [Fact]
    public async Task GetAllCommentsAsync_HasComments_ReturnsAll()
    {
        // Arrange
        var expectedComments = new List<Comment>
        {
            new Comment { Id = Guid.NewGuid(), CommentText = "Comment 1" },
            new Comment { Id = Guid.NewGuid(), CommentText = "Comment 2" },
            new Comment { Id = Guid.NewGuid(), CommentText = "Comment 3" }
        };
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedComments);

        // Act
        var result = await _service.GetAllCommentsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllCommentsAsync_NoComments_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Comment>());

        // Act
        var result = await _service.GetAllCommentsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    #endregion

    #region AddCommentAsync Tests

    [Fact]
    public async Task AddCommentAsync_ValidComment_CallsRepositoryAdd()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "New comment"
        };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Comment>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AddCommentAsync(comment);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(comment), Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_NullComment_StillCallsRepository()
    {
        // Arrange
        Comment comment = null;
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Comment>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.AddCommentAsync(comment);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(null), Times.Once);
    }

    #endregion

    #region UpdateCommentAsync Tests

    [Fact]
    public async Task UpdateCommentAsync_ValidComment_CallsRepositoryUpdate()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "Updated comment"
        };
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Comment>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateCommentAsync(comment);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(comment), Times.Once);
    }

    #endregion

    #region DeleteCommentAsync Tests

    [Fact]
    public async Task DeleteCommentAsync_ValidId_CallsRepositoryDelete()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteCommentAsync(commentId);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(commentId), Times.Once);
    }

    [Fact]
    public async Task DeleteCommentAsync_EmptyGuid_CallsRepository()
    {
        // Arrange
        var commentId = Guid.Empty;
        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteCommentAsync(commentId);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(Guid.Empty), Times.Once);
    }

    #endregion

    #region UpdateCommentStatusAsync Tests

    [Fact]
    public async Task UpdateCommentStatusAsync_ValidData_CallsRepository()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        int newStatusId = 2;
        _mockRepository.Setup(r => r.UpdateCommentStatusAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateCommentStatusAsync(commentId, newStatusId);

        // Assert
        _mockRepository.Verify(r => r.UpdateCommentStatusAsync(commentId, newStatusId), Times.Once);
    }

    [Fact]
    public async Task UpdateCommentStatusAsync_DifferentStatusIds_CallsRepositoryWithCorrectValues()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        int statusId = 3;
        _mockRepository.Setup(r => r.UpdateCommentStatusAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateCommentStatusAsync(commentId, statusId);

        // Assert
        _mockRepository.Verify(r => r.UpdateCommentStatusAsync(commentId, statusId), Times.Once);
    }

    #endregion

    #region CascadeStatusToChildrenAsync Tests

    [Fact]
    public async Task CascadeStatusToChildrenAsync_ValidData_CallsRepository()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        int newStatusId = 2;
        _mockRepository.Setup(r => r.CascadeStatusToChildrenAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.CascadeStatusToChildrenAsync(parentId, newStatusId);

        // Assert
        _mockRepository.Verify(r => r.CascadeStatusToChildrenAsync(parentId, newStatusId), Times.Once);
    }

    #endregion

    #region GetCommentsByContentIdPagedAsync Tests

    [Fact]
    public async Task GetCommentsByContentIdPagedAsync_ValidParams_ReturnsPaginatedResult()
    {
        // Arrange
        int contentId = 100;
        int page = 1;
        int pageSize = 10;
        var comments = new List<Comment>
        {
            new Comment { Id = Guid.NewGuid(), ContentId = contentId }
        };
        var expectedResult = new PaginatedResult<Comment>(comments, 1, page, pageSize);
        
        _mockRepository.Setup(r => r.GetByContentIdPagedAsync(contentId, page, pageSize))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.GetCommentsByContentIdPagedAsync(contentId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
        _mockRepository.Verify(r => r.GetByContentIdPagedAsync(contentId, page, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetCommentsByContentIdPagedAsync_DifferentPageSizes_CallsRepositoryCorrectly()
    {
        // Arrange
        int contentId = 200;
        int page = 2;
        int pageSize = 25;
        var emptyResult = new PaginatedResult<Comment>(new List<Comment>(), 0, page, pageSize);
        
        _mockRepository.Setup(r => r.GetByContentIdPagedAsync(contentId, page, pageSize))
            .ReturnsAsync(emptyResult);

        // Act
        var result = await _service.GetCommentsByContentIdPagedAsync(contentId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(25, result.PageSize);
        _mockRepository.Verify(r => r.GetByContentIdPagedAsync(contentId, page, pageSize), Times.Once);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidRepository_CreatesInstance()
    {
        // Arrange
        var mockRepo = new Mock<ICommentRepository>();

        // Act
        var service = new CommentService(mockRepo.Object);

        // Assert
        Assert.NotNull(service);
    }

    #endregion
}
