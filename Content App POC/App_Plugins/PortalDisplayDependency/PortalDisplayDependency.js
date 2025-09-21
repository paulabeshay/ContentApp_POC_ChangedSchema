(function () {
    'use strict';

    // Configuration object to hold all settings
    const config = {
        adminGroupName: 'commentsadmin',
        viewerGroupName: 'commentsviewer',
        cmsToggle: 'cMSDisplay',
        portalToggle: 'portalDisplay',
        initialized: false,
        configLoaded: false
    };

    // State management
    const state = {
        isCommentsAdmin: false,
        eventListenersAttached: false
    };

    // Fetch configuration from API with proper error handling
    async function loadConfiguration() {
        try {
            const response = await fetch('/api/comments/user-groups');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const data = await response.json();
            
            if (data) {
                if (data.adminGroupName) config.adminGroupName = data.adminGroupName.toLowerCase();
                if (data.viewerGroupName) config.viewerGroupName = data.viewerGroupName.toLowerCase();
                if (data.cmsToggle) config.cmsToggle = data.cmsToggle;
                if (data.portalToggle) config.portalToggle = data.portalToggle;
            }
            config.configLoaded = true;
            console.log('PortalDisplayDependency: Configuration loaded successfully', config);
        } catch (error) {
            console.warn('PortalDisplayDependency: Failed to load configuration from API, using defaults', error);
            config.configLoaded = true; // Continue with defaults
        }
    }

    // Utility functions for DOM manipulation
    function findElement(selector) {
        // Try multiple selection strategies
        return document.querySelector(`[data-element="${selector}"]`) || 
               document.getElementById(selector) ||
               document.querySelector(`[name="${selector}"]`);
    }

    function setElementState(element, disabled, reason = '') {
        if (!element) return;
        
        element.disabled = disabled;
        element.style.opacity = disabled ? '0.4' : '1';
        element.style.pointerEvents = disabled ? 'none' : 'auto';
        
        // Add accessibility attributes
        element.setAttribute('aria-disabled', disabled.toString());
        if (reason) {
            element.setAttribute('aria-describedby', `${element.id || 'element'}-disabled-reason`);
            element.setAttribute('title', reason);
        }
    }

    function applyVisualState(container, disabled) {
        if (!container) return;
        
        if (disabled) {
            container.classList.add('umb-property--disabled');
            container.style.opacity = '0.6';
        } else {
            container.classList.remove('umb-property--disabled');
            container.style.opacity = '1';
        }
    }

    // Enhanced function to handle the dependency between cMSDisplay and portalDisplay
    function handlePortalDisplayDependency() {
        if (state.eventListenersAttached) {
            console.log('PortalDisplayDependency: Event listeners already attached, skipping');
            return;
        }

        // Find elements using enhanced selector
        const cmsDisplayField = findElement(config.cmsToggle);
        const portalDisplayField = findElement(config.portalToggle);

        if (!cmsDisplayField || !portalDisplayField) {
            console.warn(`PortalDisplayDependency: Elements not found - CMS: ${!!cmsDisplayField}, Portal: ${!!portalDisplayField}`);
            return;
        }

        // Get the actual input elements
        const cmsDisplayInput = cmsDisplayField.querySelector ? 
            cmsDisplayField.querySelector('input[type="checkbox"], select') : cmsDisplayField;
        const portalDisplayInput = portalDisplayField.querySelector ? 
            portalDisplayField.querySelector('input[type="checkbox"]') : portalDisplayField;

        if (!cmsDisplayInput || !portalDisplayInput) {
            console.warn('PortalDisplayDependency: Input elements not found within containers');
            return;
        }

        // Apply initial permissions based on user role
        if (!state.isCommentsAdmin) {
            setElementState(cmsDisplayInput, true, 'Requires CommentsAdmin role');
            setElementState(portalDisplayInput, true, 'Requires CommentsAdmin role');
        }

        // Function to update portalDisplay state based on cMSDisplay and user group
        function updatePortalDisplayState() {
            const cmsDisplayValue = cmsDisplayInput.type === 'checkbox' ? 
                cmsDisplayInput.checked : cmsDisplayInput.value;
            const isCMSDisplayEnabled = cmsDisplayInput.type === 'checkbox' ? 
                cmsDisplayValue : cmsDisplayValue === '1';

            // If cMSDisplay is disabled, uncheck portalDisplay
            if (!isCMSDisplayEnabled) {
                portalDisplayInput.checked = false;
                // Trigger change event to update the model
                const event = new Event('change', { bubbles: true });
                portalDisplayInput.dispatchEvent(event);
            }

            // Apply visual states to containers
            const portalDisplayContainer = portalDisplayField.closest('.umb-property');
            const cmsDisplayContainer = cmsDisplayField.closest('.umb-property');
            
            if (portalDisplayContainer) {
                applyVisualState(portalDisplayContainer, !isCMSDisplayEnabled || !state.isCommentsAdmin);
            }
            
            if (cmsDisplayContainer) {
                applyVisualState(cmsDisplayContainer, !state.isCommentsAdmin);
            }

            // Disable portal display if CMS display is off or user lacks permissions
            const shouldDisablePortal = !isCMSDisplayEnabled || !state.isCommentsAdmin;
            setElementState(portalDisplayInput, shouldDisablePortal, 
                !state.isCommentsAdmin ? 'Requires CommentsAdmin role' : 
                !isCMSDisplayEnabled ? 'CMS Display must be enabled first' : '');
        }

        // Add event listener to cMSDisplay (only once)
        cmsDisplayInput.addEventListener('change', updatePortalDisplayState);
        state.eventListenersAttached = true;

        // Initial state update
        updatePortalDisplayState();
        
        console.log('PortalDisplayDependency: Successfully initialized for elements', {
            cmsToggle: config.cmsToggle,
            portalToggle: config.portalToggle,
            isCommentsAdmin: state.isCommentsAdmin
        });
    }

    // Helper to check if user is in CommentsAdmin group
    function isUserCommentsAdmin(user) {
        if (!user || !user.userGroups) return false;
        return user.userGroups.some(function (g) {
            if (typeof g === 'string') return g.toLowerCase() === config.adminGroupName;
            if (g && g.name) return g.name.toLowerCase() === config.adminGroupName;
            if (g && g.alias) return g.alias.toLowerCase() === config.adminGroupName;
            return false;
        });
    }

    // Enhanced initialization with proper async handling
    async function initialize() {
        if (config.initialized) {
            console.log('PortalDisplayDependency: Already initialized, skipping');
            return;
        }

        try {
            // Wait for configuration to load
            if (!config.configLoaded) {
                await loadConfiguration();
            }

            // Wait for Angular and user service to be available
            const user = await getUserFromAngular();
            if (user) {
                state.isCommentsAdmin = isUserCommentsAdmin(user);
                console.log('PortalDisplayDependency: User loaded', { 
                    isCommentsAdmin: state.isCommentsAdmin,
                    userGroups: user.userGroups?.map(g => g.name || g.alias || g) 
                });
            }

            // Wait for DOM elements to be available
            await waitForElements();
            
            // Initialize the dependency handling
            handlePortalDisplayDependency();
            config.initialized = true;
            
        } catch (error) {
            console.error('PortalDisplayDependency: Initialization failed', error);
            // Retry after a delay
            setTimeout(() => {
                config.initialized = false;
                initialize();
            }, 2000);
        }
    }

    // Promise-based Angular user service access
    function getUserFromAngular() {
        return new Promise((resolve, reject) => {
            let attempts = 0;
            const maxAttempts = 20; // 10 seconds max wait
            
            function tryGetUser() {
                attempts++;
                
                if (window.angular && angular.module) {
                    try {
                        const injector = angular.element(document.body).injector();
                        if (injector) {
                            const userService = injector.get('userService');
                            userService.getCurrentUser().then(resolve).catch(reject);
                            return;
                        }
                    } catch (error) {
                        console.warn('PortalDisplayDependency: Angular injector not ready, attempt', attempts);
                    }
                }
                
                if (attempts < maxAttempts) {
                    setTimeout(tryGetUser, 500);
                } else {
                    reject(new Error('Angular user service not available after maximum attempts'));
                }
            }
            
            tryGetUser();
        });
    }

    // Wait for DOM elements to be available
    function waitForElements() {
        return new Promise((resolve) => {
            let attempts = 0;
            const maxAttempts = 20;
            
            function checkElements() {
                attempts++;
                const cmsElement = findElement(config.cmsToggle);
                const portalElement = findElement(config.portalToggle);
                
                if (cmsElement && portalElement) {
                    resolve();
                } else if (attempts < maxAttempts) {
                    setTimeout(checkElements, 500);
                } else {
                    console.warn('PortalDisplayDependency: Elements not found after maximum attempts');
                    resolve(); // Continue anyway
                }
            }
            
            checkElements();
        });
    }

    // Debounced initialization to prevent multiple calls
    let initTimeout;
    function debouncedInitialize() {
        clearTimeout(initTimeout);
        initTimeout = setTimeout(initialize, 100);
    }

    // Multiple initialization triggers for different scenarios
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', debouncedInitialize);
    } else {
        debouncedInitialize();
    }

    // Umbraco-specific initialization
    if (typeof angular !== 'undefined' && angular.module) {
        try {
            angular.module('umbraco').run(['$rootScope', function ($rootScope) {
                $rootScope.$on('contentLoaded', debouncedInitialize);
                $rootScope.$on('formSubmitting', function() {
                    // Reset state on form submission to handle page transitions
                    config.initialized = false;
                    state.eventListenersAttached = false;
                });
            }]);
        } catch (error) {
            console.warn('PortalDisplayDependency: Could not register Umbraco event handlers', error);
        }
    }

    // Expose configuration for debugging
    window.PortalDisplayDependencyDebug = {
        config,
        state,
        reinitialize: () => {
            config.initialized = false;
            state.eventListenersAttached = false;
            initialize();
        }
    };
})(); 