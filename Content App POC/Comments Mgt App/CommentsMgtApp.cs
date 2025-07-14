using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models;

namespace Content_App_POC.Comments_Mgt
{
    public class CommentsMgtAppComponent : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Add our word counter content app into the composition aka into the DI
            builder.ContentApps().Append<CommentsMgtApp>();
        }
    }

    public class CommentsMgtApp : IContentAppFactory
    {
        private readonly string _adminGroupName;
        private readonly string _viewerGroupName;
        private readonly string _cmsToggle;
        private readonly string _portalToggle;

        public CommentsMgtApp()
        {
            // Fallback for DI-less instantiation (should not be used in production)
            _adminGroupName = "commentsadmin";
            _viewerGroupName = "commentsviewer";
            _cmsToggle = "cMSDisplay";
            _portalToggle = "portalDisplay";
        }

        public CommentsMgtApp(Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _adminGroupName = (config["CommentsManagement:UserGroups:AdminGroupName"] ?? "commentsadmin").ToLowerInvariant();
            _viewerGroupName = (config["CommentsManagement:UserGroups:ViewerGroupName"] ?? "commentsviewer").ToLowerInvariant();
            _cmsToggle = config["CommentsManagement:CommentsMgtToggles:CMS"] ?? "cMSDisplay";
            _portalToggle = config["CommentsManagement:CommentsMgtToggles:Portal"] ?? "portalDisplay";
        }

        public ContentApp? GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            if (userGroups.All(x => x.Alias.ToLowerInvariant() != _adminGroupName && x.Alias.ToLowerInvariant() != _viewerGroupName))
                return null;

            // Only show app on content items
            if (source is not IContent content)
                return null;

            // Only show app on content items with template
            //if (content.TemplateId is null)
            //    return null;

            // Only show app on content with certain content type alias
            var property = content.Properties.FirstOrDefault(p => p.Alias == _cmsToggle);
            if (property?.GetValue()?.ToString() != "1")
                return null;

            return new ContentApp
            {
                Alias = "commentsSection",
                Name = "Comments Section",
                Icon = "icon-reply-arrow",
                View = "/App_Plugins/CommentsMgt/commentssection.html",
                Weight = 0
            };
        }
    }
}
