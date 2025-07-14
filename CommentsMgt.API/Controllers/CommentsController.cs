using Microsoft.AspNetCore.Mvc;
using Content_App_POC.CommentsMgt;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Content_App_POC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IConfiguration _configuration;

        public CommentsController(ICommentService commentService, IConfiguration configuration)
        {
            _commentService = commentService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null) return NotFound();
            return Ok(comment);
        }

        [HttpGet("content/{contentId}/paged")]
        public async Task<IActionResult> GetByContentIdPaged(int contentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _commentService.GetCommentsByContentIdPagedAsync(contentId, page, pageSize);
            return Ok(pagedResult);
        }

        [HttpGet("content/{contentId}")]
        public async Task<IActionResult> GetByContentId(int contentId)
        {
            var comments = await _commentService.GetCommentsByContentIdAsync(contentId);
            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Comment comment)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _commentService.AddCommentAsync(comment);
            return CreatedAtAction(nameof(GetById), new { id = comment.Id }, comment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Comment comment)
        {
            if (id != comment.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _commentService.UpdateCommentAsync(comment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _commentService.DeleteCommentAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] int newStatusId, [FromQuery] bool cascade = false)
        {
            // Update the status of the comment
            await _commentService.UpdateCommentStatusAsync(id, newStatusId);

            // If cascade is true, update all children
            if (cascade)
            {
                await _commentService.CascadeStatusToChildrenAsync(id, newStatusId);
            }

            return NoContent();
        }

        [HttpGet("initial-config")]
        public IActionResult GetInitialConfig()
        {
            int statusId = _configuration.GetValue<int>("CommentsManagement:InitialCommentStatusId", 1);
            bool shownInPortal = _configuration.GetValue<int>("CommentsManagement:InitialVisbilityStatus", 1) == 1;
            return Ok(new { initialCommentStatusId = statusId, initialVisibilityStatus = shownInPortal });
        }

        [HttpGet("user-groups")]
        public IActionResult GetUserGroups()
        {
            var adminGroup = _configuration["CommentsManagement:UserGroups:AdminGroupName"] ?? "CommentsAdmin";
            var viewerGroup = _configuration["CommentsManagement:UserGroups:ViewerGroupName"] ?? "CommentsViewer";
            var cmsToggle = _configuration["CommentsManagement:CommentsMgtToggles:CMS"] ?? "cMSDisplay";
            var portalToggle = _configuration["CommentsManagement:CommentsMgtToggles:Portal"] ?? "portalDisplay";
            return Ok(new {
                adminGroupName = adminGroup,
                viewerGroupName = viewerGroup,
                cmsToggle = cmsToggle,
                portalToggle = portalToggle
            });
        }
    }
} 