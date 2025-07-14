How to use Comments Mgt Package?

1- Get and add the following DLLs references:
- CommentsMgt.API.dll
- CommentsMgt.Application.dll
- CommentsMgt.Domain.dll
- CommentsMgt.DTOs.dll
- CommentsMgt.Infra.dll

2- Create Comments Mgt Composition with 2 toggles (CMSDisplay - PortalDisplay) and use this Composition in every needed module.

3- Create 2 new User Groups (CommentsAdmin - CommentsViewer)

4- Create a partial html view to be used in every portal html view where it is needed.

<link rel="stylesheet" href="~/clean-assets/css/comments-section.css"/>
<script>
    window.currentContentId = @(Model?.Id ?? 0);
    window.currentUserName = '@(User?.Identity?.Name ?? "")';
    @{
        var parentAlias = Model?.Parent != null ? Model.Parent.ContentType.Alias : "";
    }
    window.currentParentNodeAlias = '@parentAlias';
</script>
<div id="comments-section-container" class="comments-section-social"></div>
<script src="/clean-assets/js/comments-section.js"></script>  

and use it in every needed html view as

@if (Model.Value("portalDisplay") is bool showPortalDisplay && showPortalDisplay)
{
    @await Html.PartialAsync("~/Views/Partials/CommentsSection.cshtml")
}

5- Add (CommentsMgt - PortalDisplayDependency) folders in App_Plugins folder
6- Add CommentsMgtApp.cs file in New Folder (Comment Mgt App)
7- Add comments-section.js file in wwwroot folder => clean-assets => js
8- Add PortalDisplayDependency.cs