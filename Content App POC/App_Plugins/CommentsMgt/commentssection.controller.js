var totalCount = 0;

angular.module("umbraco")
    .controller("My.CommentsSectionApp", function ($scope, $http, editorState, userService, contentResource) {

        console.log("CommentsSectionApp controller loaded");

        var vm = this;
        vm.CurrentNodeId = editorState.current.id;
        vm.CurrentNodeAlias = editorState.current.contentTypeAlias;
        vm.CurrentNodeParentAlias = '';
        vm.Comments = [];
        vm.CanAdminComments = false;
        vm.showAddModal = false;
        vm.showEditModal = false;
        vm.showDeleteModal = false;
        vm.showReplyModal = false;
        vm.modalComment = null;
        vm.replyToComment = null;
        vm.newCommentText = '';
        vm.editingCommentText = '';
        vm.replyText = '';
        vm.page = 1;
        vm.pageSize = 10;
        vm.totalCount = 0;
        vm.initialCommentStatusId = 1;
        vm.initialVisibilityStatus = false;
        $http.get('/api/comments/initial-config').then(function(response) {
            if (response.data && response.data.initialCommentStatusId) {
                vm.initialCommentStatusId = response.data.initialCommentStatusId;
            }
            if (response.data && typeof response.data.initialVisibilityStatus !== 'undefined') {
                vm.initialVisibilityStatus = response.data.initialVisibilityStatus;
            }
        });

        // Track collapsed state of replies for each parent comment
        vm.collapsedReplies = {};

        // Fetch parent node alias
        contentResource.getById(editorState.current.parentId).then(function (parentNode) {
            vm.CurrentNodeParentAlias = parentNode.contentTypeAlias;
            console.log("Parent Content Alias:: " + vm.CurrentNodeParentAlias);
        }, function (error) {
            console.error('Failed to fetch parent node alias', error);
        });

        // Fetch comments for the current content id
        vm.loadComments = function () {
            console.log("Loading comments for content ID: " + vm.CurrentNodeId);
            $http.get("/api/comments/content/" + vm.CurrentNodeId + "/paged?page=" + vm.page + "&pageSize=" + vm.pageSize)
                .then(function(response) {
                    // Only show parent comments that are shown in portal
                    vm.Comments = response.data.items;//.map(function(parent) {
                        // Only include replies that are shown in portal
                    //    parent.Children = (parent.Children || []);
                    //    return parent;
                    //});
                    vm.totalCount = response.data.totalCount;
                    vm.page = response.data.page;
                    vm.pageSize = response.data.pageSize;
                    console.log("Comments loaded: ", vm.Comments);
                }, function(error) {
                    console.error("Failed to load comments", error);
                });
        };

        vm.loadComments();

        vm.adminGroupName = 'commentsadmin';
        vm.viewerGroupName = 'commentsviewer';
        $http.get('/api/comments/user-groups').then(function(response) {
            if (response.data && response.data.adminGroupName) {
                vm.adminGroupName = response.data.adminGroupName.toLowerCase();
            }
            if (response.data && response.data.viewerGroupName) {
                vm.viewerGroupName = response.data.viewerGroupName.toLowerCase();
            }
        });

        function checkCommentsAdmin(userGroups) {
            if (!userGroups) return false;
            return userGroups.some(function (g) {
                if (typeof g === 'string') return g.toLowerCase() === vm.adminGroupName;
                if (g && g.name) return g.name.toLowerCase() === vm.adminGroupName;
                if (g && g.alias) return g.alias.toLowerCase() === vm.adminGroupName;
                return false;
            });
        }

        // Modal open/close helpers
        vm.openAddModal = function () {
            vm.newCommentText = '';
            vm.showAddModal = true;
            // Focus management will be handled by $timeout after DOM update
            $scope.$evalAsync(function() {
                var textarea = document.querySelector('.custom-modal-overlay textarea');
                if (textarea) textarea.focus();
            });
        };
        vm.closeAddModal = function() {
            vm.showAddModal = false;
        };
        vm.openEditModal = function (comment) {
            vm.modalComment = angular.copy(comment);
            vm.editingCommentText = comment.commentText;
            vm.showEditModal = true;
            // Focus management will be handled by $timeout after DOM update
            $scope.$evalAsync(function() {
                var textarea = document.querySelector('.custom-modal-overlay textarea');
                if (textarea) textarea.focus();
            });
        };
        vm.closeEditModal = function() {
            vm.showEditModal = false;
            vm.modalComment = null;
        };
        vm.openDeleteModal = function (comment) {
            vm.modalComment = comment;
            vm.showDeleteModal = true;
        };
        vm.closeDeleteModal = function() {
            vm.showDeleteModal = false;
            vm.modalComment = null;
        };

        // Reply modal functions
        vm.openReplyModal = function(comment) {
            vm.replyToComment = comment;
            vm.replyText = '';
            vm.showReplyModal = true;
            // Focus management will be handled by $timeout after DOM update
            $scope.$evalAsync(function() {
                var textarea = document.querySelector('.custom-modal-overlay textarea');
                if (textarea) textarea.focus();
            });
        };
        vm.closeReplyModal = function() {
            vm.showReplyModal = false;
            vm.replyToComment = null;
            vm.replyText = '';
        };

        // Add comment (modal)
        vm.addComment = function() {
            if (!vm.newCommentText) return;
            var comment = {
                contentId: vm.CurrentNodeId,
                commentText: vm.newCommentText,
                contentParentAlias: vm.CurrentNodeParentAlias,
                commentStatusId: vm.initialCommentStatusId,
                shownInPortal: (vm.initialCommentStatusId === 2) ? vm.initialVisibilityStatus : false,
                createdBy: vm.UserName,
                modifiedBy: vm.UserName
            };
            $http.post('/api/comments', comment).then(function() {
                vm.newCommentText = '';
                vm.closeAddModal();
                vm.loadComments();
            });
        };

        // Edit comment (modal)
        vm.saveEdit = function() {
            if (!vm.modalComment) return;
            var updated = angular.copy(vm.modalComment);
            updated.commentText = vm.editingCommentText;
            updated.modifiedBy = vm.UserName;
            $http.put('/api/comments/' + updated.id, updated).then(function() {
                vm.closeEditModal();
                vm.loadComments();
            });
        };

        // Delete comment (modal)
        vm.confirmDelete = function() {
            if (!vm.modalComment) return;
            $http.delete('/api/comments/' + vm.modalComment.id).then(function() {
                vm.closeDeleteModal();
                vm.loadComments();
            });
        };

        // Submit reply
        vm.submitReply = function() {
            if (!vm.replyText || !vm.replyToComment) return;
            var reply = {
                contentId: vm.CurrentNodeId,
                commentText: vm.replyText,
                parentId: vm.replyToComment.id,
                contentParentAlias: vm.CurrentNodeParentAlias,
                commentStatusId: vm.initialCommentStatusId,
                shownInPortal: (vm.initialCommentStatusId === 2) ? vm.initialVisibilityStatus : false,
                createdBy: vm.UserName,
                modifiedBy: vm.UserName
            };
            $http.post('/api/comments', reply).then(function() {
                vm.closeReplyModal();
                vm.loadComments();
            });
        };

        // Get user initials for avatar
        vm.getInitials = function(name) {
            if (!name) return '';
            var names = name.split(' ');
            if (names.length >= 2) {
                return (names[0].charAt(0) + names[1].charAt(0)).toUpperCase();
            }
            return name.charAt(0).toUpperCase();
        };

        // Initial user check
        var user = userService.getCurrentUser().then(function (user) {
            vm.UserName = user.name;
            vm.UserGroups = user.userGroups;
            vm.CanAdminComments = checkCommentsAdmin(user.userGroups);
        });

        // Toggle ShownInPortal for a comment
        vm.toggleShownInPortal = function(comment) {
            var updated = angular.copy(comment);
            updated.modifiedBy = vm.UserName;
            $http.put('/api/comments/' + updated.id, updated).then(function() {
                vm.loadComments();
            }, function(error) {
                // Optionally revert the toggle on error
                comment.shownInPortal = !comment.shownInPortal;
                alert('Failed to update Shown In Portal');
            });
        };

        // Helper to check if a comment's parent is not shown in portal
        vm.isParentNotShownInPortal = function(comment) {
            if (!comment.parentId) return false;
            var parent = vm.Comments.find(function(c) { return c.id === comment.parentId; });
            return parent && !parent.shownInPortal;
        };

        // Helper to check if a comment's parent is approved
        vm.isParentApproved = function(comment) {
            if (!comment.parentId) return true;
            var parent = vm.Comments.find(function(c) { return c.id === comment.parentId; });
            return parent && parent.commentStatusId === 2; // 2 = Approved
        };

        // Status options for dropdown
        vm.statusOptions = [
            { id: 1, label: 'Pending' },
            { id: 2, label: 'Approved' },
            { id: 3, label: 'Rejected' }
        ];

        // Update comment status
        vm.updateCommentStatus = function(comment, cascade) {
            // If status is Pending or Rejected, always set shownInPortal to false and disable toggle
            if (comment.commentStatusId === 1 || comment.commentStatusId === 3) { // 1 = Pending, 3 = Rejected
                comment.shownInPortal = false;
            } else if (comment.commentStatusId === 2) { // 2 = Approved
                comment.shownInPortal = true;
            }
            var url = '/api/comments/' + comment.id + '/status?newStatusId=' + comment.commentStatusId + '&cascade=' + !!cascade;
            $http.put(url).then(function() {
                vm.loadComments();
            }, function(error) {
                alert('Failed to update comment status');
            });
        };

        // Helper to determine if shownInPortal toggle should be disabled
        vm.isShownInPortalDisabled = function(comment) {
            return comment.commentStatusId !== 2; // Only enabled if Approved
        };

        vm.nextPage = function() {
            if (vm.page * vm.pageSize < vm.totalCount) {
                vm.page++;
                vm.loadComments();
            }
        };

        vm.prevPage = function() {
            if (vm.page > 1) {
                vm.page--;
                vm.loadComments();
            }
        };

        // Collapse all replies
        vm.collapseAllReplies = function() {
            vm.Comments.forEach(function(parent) {
                vm.collapsedReplies[parent.id] = true;
            });
        };

        // Expand all replies
        vm.expandAllReplies = function() {
            vm.Comments.forEach(function(parent) {
                vm.collapsedReplies[parent.id] = false;
            });
        };

        // Toggle replies for a specific parent comment
        vm.toggleReplies = function(parentId) {
            vm.collapsedReplies[parentId] = !vm.collapsedReplies[parentId];
        };

    });

angular.module("umbraco")
    .filter('ceil', function() {
        return function(input) {
            return Math.ceil(input);
        };
    });