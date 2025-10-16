using Content_App_POC.CommentsMgt;
using Content_App_POC.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CommentsMgt.API.Tests;

public class CommentsControllerTests
{
    private readonly Mock<ICommentService> _mockService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly CommentsController _controller;

    public CommentsControllerTests()
    {
        _mockService = new Mock<ICommentService>();
        _mockConfiguration = new Mock<IConfiguration>();
        _controller = new CommentsController(_mockService.Object, _mockConfiguration.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithComments()
    {
        // Arrange
        var comments = new List<Comment>
        {
            new Comment { Id = Guid.NewGuid(), CommentText = "Comment 1" },
            new Comment { Id = Guid.NewGuid(), CommentText = "Comment 2" }
        };
        _mockService.Setup(s => s.GetAllCommentsAsync())
            .ReturnsAsync(comments);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedComments = Assert.IsAssignableFrom<IEnumerable<Comment>>(okResult.Value);
        Assert.Equal(2, returnedComments.Count());
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = commentId, CommentText = "Test comment" };
        _mockService.Setup(s => s.GetCommentByIdAsync(commentId))
            .ReturnsAsync(comment);

        // Act
        var result = await _controller.GetById(commentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedComment = Assert.IsType<Comment>(okResult.Value);
        Assert.Equal(commentId, returnedComment.Id);
    }

    [Fact]
    public async Task GetById_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        _mockService.Setup(s => s.GetCommentByIdAsync(commentId))
            .ReturnsAsync((Comment)null);

        // Act
        var result = await _controller.GetById(commentId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region GetByContentIdPaged Tests

    [Fact]
    public async Task GetByContentIdPaged_ValidParams_ReturnsOkResult()
    {
        // Arrange
        int contentId = 100;
        int page = 1;
        int pageSize = 10;
        var comments = new List<Comment> { new Comment { Id = Guid.NewGuid() } };
        var pagedResult = new PaginatedResult<Comment>(comments, 1, page, pageSize);

        _mockService.Setup(s => s.GetCommentsByContentIdPagedAsync(contentId, page, pageSize))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetByContentIdPaged(contentId, page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<PaginatedResult<Comment>>(okResult.Value);
        Assert.Equal(1, returnedResult.TotalCount);
        Assert.Equal(page, returnedResult.Page);
        Assert.Equal(pageSize, returnedResult.PageSize);
    }

    [Fact]
    public async Task GetByContentIdPaged_DefaultParams_UsesDefaults()
    {
        // Arrange
        int contentId = 100;
        var pagedResult = new PaginatedResult<Comment>(new List<Comment>(), 0, 1, 10);

        _mockService.Setup(s => s.GetCommentsByContentIdPagedAsync(contentId, 1, 10))
            .ReturnsAsync(pagedResult);

        // Act - Not providing page and pageSize, should use defaults
        var result = await _controller.GetByContentIdPaged(contentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        _mockService.Verify(s => s.GetCommentsByContentIdPagedAsync(contentId, 1, 10), Times.Once);
    }

    #endregion

    #region GetByContentId Tests

    [Fact]
    public async Task GetByContentId_ValidId_ReturnsOkResult()
    {
        // Arrange
        int contentId = 100;
        var comments = new List<Comment>
        {
            new Comment { Id = Guid.NewGuid(), ContentId = contentId }
        };
        _mockService.Setup(s => s.GetCommentsByContentIdAsync(contentId))
            .ReturnsAsync(comments);

        // Act
        var result = await _controller.GetByContentId(contentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedComments = Assert.IsAssignableFrom<IEnumerable<Comment>>(okResult.Value);
        Assert.Single(returnedComments);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_ValidComment_ReturnsCreatedAtAction()
    {
        // Arrange
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentId = 100,
            CommentText = "New comment"
        };
        _mockService.Setup(s => s.AddCommentAsync(It.IsAny<Comment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(comment);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
        Assert.Equal(comment.Id, ((Comment)createdResult.Value).Id);
        _mockService.Verify(s => s.AddCommentAsync(comment), Times.Once);
    }

    [Fact]
    public async Task Create_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var comment = new Comment { Id = Guid.NewGuid() };
        _controller.ModelState.AddModelError("CommentText", "Required");

        // Act
        var result = await _controller.Create(comment);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_MismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = Guid.NewGuid() }; // Different ID

        // Act
        var result = await _controller.Update(commentId, comment);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = commentId };
        _controller.ModelState.AddModelError("CommentText", "Required");

        // Act
        var result = await _controller.Update(commentId, comment);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    [Fact]
    public async Task Update_ValidComment_ReturnsNoContent()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new Comment { Id = commentId, CommentText = "Updated" };
        _mockService.Setup(s => s.UpdateCommentAsync(It.IsAny<Comment>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(commentId, comment);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.UpdateCommentAsync(comment), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ValidId_ReturnsNoContent()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteCommentAsync(commentId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(commentId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.DeleteCommentAsync(commentId), Times.Once);
    }

    #endregion

    #region UpdateStatus Tests

    [Fact]
    public async Task UpdateStatus_CascadeFalse_OnlyUpdatesComment()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        int newStatusId = 2;
        _mockService.Setup(s => s.UpdateCommentStatusAsync(commentId, newStatusId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateStatus(commentId, newStatusId, cascade: false);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.UpdateCommentStatusAsync(commentId, newStatusId), Times.Once);
        _mockService.Verify(s => s.CascadeStatusToChildrenAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStatus_CascadeTrue_UpdatesCommentAndChildren()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        int newStatusId = 2;
        _mockService.Setup(s => s.UpdateCommentStatusAsync(commentId, newStatusId))
            .Returns(Task.CompletedTask);
        _mockService.Setup(s => s.CascadeStatusToChildrenAsync(commentId, newStatusId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateStatus(commentId, newStatusId, cascade: true);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.UpdateCommentStatusAsync(commentId, newStatusId), Times.Once);
        _mockService.Verify(s => s.CascadeStatusToChildrenAsync(commentId, newStatusId), Times.Once);
    }

    #endregion

    #region GetUserGroups Tests

    [Fact]
    public void GetUserGroups_ReturnsOkResult()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["CommentsManagement:UserGroups:AdminGroupName"])
            .Returns("CommentsAdmin");
        _mockConfiguration.Setup(c => c["CommentsManagement:UserGroups:ViewerGroupName"])
            .Returns("CommentsViewer");
        _mockConfiguration.Setup(c => c["CommentsManagement:CommentsMgtToggles:CMS"])
            .Returns("cMSDisplay");
        _mockConfiguration.Setup(c => c["CommentsManagement:CommentsMgtToggles:Portal"])
            .Returns("portalDisplay");

        // Act
        var result = _controller.GetUserGroups();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    #endregion
}
