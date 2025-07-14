(function () {
    'use strict';

    // Function to handle the dependency between cMSDisplay and portalDisplay
    function handlePortalDisplayDependency(isCommentsAdmin) {
        // Find the cMSDisplay field
        var cmsDisplayField = document.querySelector('[data-element="cMSDisplay"]');
        var portalDisplayField = document.querySelector('[data-element="portalDisplay"]');

        // Disable/enable portalDisplay based on cMSDisplay and user group
        if (document.getElementById('cMSDisplay') && document.getElementById('portalDisplay'))
        {
            if (!isCommentsAdmin) {
                document.getElementById('cMSDisplay').disabled = '"disabled"';
                document.getElementById('cMSDisplay').style.opacity = '0.4';
                document.getElementById('cMSDisplay').style.pointerEvents = 'none';

                document.getElementById('portalDisplay').disabled = '"disabled"';
                document.getElementById('portalDisplay').style.opacity = '0.4';
                document.getElementById('portalDisplay').style.pointerEvents = 'none';
            }
        }
        else
        {
            console.error("Element with data-element='cMSDisplay' and 'portalDisplay' not found.");
        }

        if (!cmsDisplayField || !portalDisplayField) {
            return;
        }

        // Get the cMSDisplay input
        var cmsDisplayInput = cmsDisplayField.querySelector('input[type="checkbox"], select');
        var portalDisplayInput = portalDisplayField.querySelector('input[type="checkbox"]');

        if (!cmsDisplayInput || !portalDisplayInput) {
            return;
        }

        // Function to update portalDisplay state based on cMSDisplay and user group
        function updatePortalDisplayState() {
            var cmsDisplayValue = cmsDisplayInput.type === 'checkbox' ? cmsDisplayInput.checked : cmsDisplayInput.value;
            var isCMSDisplayEnabled = cmsDisplayInput.type === 'checkbox' ? cmsDisplayValue : cmsDisplayValue === '1';

            // If cMSDisplay is disabled, uncheck portalDisplay
            if (!isCMSDisplayEnabled) {
                portalDisplayInput.checked = false;
                // Trigger change event to update the model
                var event = new Event('change', { bubbles: true });
                portalDisplayInput.dispatchEvent(event);
            }

            // Add visual indication
            //var portalDisplayContainer = portalDisplayField.closest('.umb-property');
            //var cmsDisplayContainer = cmsDisplayField.closest('.umb-property');
            //if (portalDisplayContainer) {
            //    if (!isCMSDisplayEnabled || !isCommentsAdmin) {
            //        portalDisplayContainer.classList.add('umb-property--disabled');
            //        portalDisplayContainer.style.opacity = '0.6';
            //    } else {
            //        portalDisplayContainer.classList.remove('umb-property--disabled');
            //        portalDisplayContainer.style.opacity = '1';
            //    }
            //}
            //if (cmsDisplayContainer) {
            //    if (!isCommentsAdmin) {
            //        cmsDisplayContainer.classList.add('umb-property--disabled');
            //        cmsDisplayContainer.style.opacity = '0.6';
            //    } else {
            //        cmsDisplayContainer.classList.remove('umb-property--disabled');
            //        cmsDisplayContainer.style.opacity = '1';
            //    }
            //}
        }

        // Add event listener to cMSDisplay
        cmsDisplayInput.addEventListener('change', updatePortalDisplayState);

        // Initial state update
        updatePortalDisplayState();
    }

    var adminGroupName = 'commentsadmin';
    var viewerGroupName = 'commentsviewer';
    // Fetch group names from API
    fetch('/api/comments/user-groups').then(function(response) {
        return response.json();
    }).then(function(data) {
        if (data && data.adminGroupName) adminGroupName = data.adminGroupName.toLowerCase();
        if (data && data.viewerGroupName) viewerGroupName = data.viewerGroupName.toLowerCase();
    });

    // Helper to check if user is in CommentsAdmin group
    function isUserCommentsAdmin(user) {
        if (!user || !user.userGroups) return false;
        return user.userGroups.some(function (g) {
            if (typeof g === 'string') return g.toLowerCase() === adminGroupName;
            if (g && g.name) return g.name.toLowerCase() === adminGroupName;
            if (g && g.alias) return g.alias.toLowerCase() === adminGroupName;
            return false;
        });
    }

    // Initialize when DOM is ready
    function initialize() {
        // Use Angular's userService to get current user
        if (window.angular && angular.module) {
            var injector = angular.element(document.body).injector();
            if (injector) {
                var userService = injector.get('userService');
                userService.getCurrentUser().then(function(user) {
                    var isCommentsAdmin = isUserCommentsAdmin(user);
                    setTimeout(function () {
                        handlePortalDisplayDependency(isCommentsAdmin);
                    }, 1000);
                });
            } else {
                // fallback if injector not ready
                setTimeout(initialize, 500);
            }
        } else {
            // fallback if angular not ready
            setTimeout(initialize, 500);
        }

        // Also listen for dynamic content loading
        document.addEventListener('DOMContentLoaded', function () {
            setTimeout(initialize, 500);
        });
    }

    // Run initialization
    initialize();

    // Also run when Umbraco content is loaded (for dynamic content)
    if (typeof angular !== 'undefined' && angular.module) {
        angular.module('umbraco').run(['$rootScope', function ($rootScope) {
            $rootScope.$on('contentLoaded', function () {
                setTimeout(initialize, 500);
            });
        }]);
    }
})(); 