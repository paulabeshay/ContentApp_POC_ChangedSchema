// Configuration object to store dynamic values from appsettings.json
let config = {
    adminGroupName: 'CommentsAdmin', // Default fallback
    cmsToggleId: 'cMSDisplay',       // Default fallback
    portalToggleId: 'portalDisplay'  // Default fallback
};

// Function to load configuration from existing CommentsController API
async function loadConfigurationFromAPI() {
    try {
        console.log('Loading configuration from CommentsController API...');
        const response = await fetch('/api/Comments/initial-config', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        });

        if (response.ok) {
            const apiConfig = await response.json();
            console.log('Configuration loaded from CommentsController:', apiConfig);
            
            // Update config with values from appsettings.json
            config.adminGroupName = apiConfig.adminGroupName || config.adminGroupName;
            config.cmsToggleId = apiConfig.cmsToggleId || config.cmsToggleId;
            config.portalToggleId = apiConfig.portalToggleId || config.portalToggleId;
            
            console.log('Updated configuration:', config);
            return config;
        } else {
            console.warn('Failed to load configuration from CommentsController API, using defaults:', response.status);
            return config;
        }
    } catch (error) {
        console.error('Error loading configuration from CommentsController API:', error);
        console.log('Using default configuration:', config);
        return config;
    }
}

// Function to find Umbraco toggles with multiple strategies
function findUmbracoToggle(toggleId) {
    console.log(`Searching for toggle: ${toggleId}`);
    
    // Strategy 1: Direct ID
    let element = document.getElementById(toggleId);
    if (element) {
        console.log(`Found ${toggleId} by ID`);
        return element;
    }
    
    // Strategy 2: Look for data attributes
    element = document.querySelector(`[data-element="${toggleId}"]`);
    if (element) {
        console.log(`Found ${toggleId} by data-element attribute`);
        return element;
    }
    
    // Strategy 3: Look for name attribute
    element = document.querySelector(`[name="${toggleId}"]`);
    if (element) {
        console.log(`Found ${toggleId} by name attribute`);
        return element;
    }
    
    // Strategy 4: Look for alias-based selectors
    element = document.querySelector(`[data-alias="${toggleId}"]`);
    if (element) {
        console.log(`Found ${toggleId} by data-alias attribute`);
        return element;
    }
    
    // Strategy 5: Look within Umbraco property wrappers
    const properties = document.querySelectorAll('.umb-property');
    for (let prop of properties) {
        const label = prop.querySelector('.umb-property-label');
        if (label && label.textContent.toLowerCase().includes(toggleId.toLowerCase())) {
            element = prop.querySelector('input, select, umb-toggle');
            if (element) {
                console.log(`Found ${toggleId} by property label matching`);
                return element;
            }
        }
    }
    
    console.warn(`Toggle ${toggleId} not found with any strategy`);
    return null;
}

// Function to wait for elements to be available
function waitForUmbracoToggles(toggleIds, maxAttempts = 10) {
    return new Promise((resolve) => {
        let attempts = 0;
        
        function checkToggles() {
            console.log(`Attempt ${attempts + 1} to find toggles:`, toggleIds);
            
            const foundToggles = {};
            let allFound = true;
            
            toggleIds.forEach(toggleId => {
                const element = findUmbracoToggle(toggleId);
                if (element) {
                    foundToggles[toggleId] = element;
                } else {
                    allFound = false;
                }
            });
            
            if (allFound || attempts >= maxAttempts) {
                console.log('Toggle search completed. Found:', Object.keys(foundToggles));
                resolve(foundToggles);
            } else {
                attempts++;
                setTimeout(checkToggles, 1000); // Wait 1 second between attempts
            }
        }
        
        checkToggles();
    });
}

// Function to disable Umbraco backoffice toggles
async function disableUmbracoToggles(toggleIds) {
    console.log('Starting to disable Umbraco toggles:', toggleIds);
    
    // Wait for toggles to be available
    const foundToggles = await waitForUmbracoToggles(toggleIds);
    
    Object.entries(foundToggles).forEach(([toggleId, element]) => {
        console.log(`Disabling Umbraco toggle: ${toggleId}`);
        
        // Method 1: Direct properties
        element.disabled = true;
        element.readOnly = true;
        
        // Method 2: Attributes for maximum compatibility
        element.setAttribute('disabled', 'disabled');
        element.setAttribute('readonly', 'readonly');
        element.setAttribute('aria-disabled', 'true');
        
        // Method 3: Find and disable all inputs within the toggle
        const inputs = element.querySelectorAll('input, select, textarea');
        inputs.forEach(input => {
            input.disabled = true;
            input.readOnly = true;
            input.setAttribute('readonly', 'readonly');
            input.setAttribute('disabled', 'disabled');
        });
        
        // Method 4: If element itself is an input, disable it directly
        if (element.tagName === 'INPUT' || element.tagName === 'SELECT') {
            element.disabled = true;
            element.readOnly = true;
        }
        
        // Method 5: Umbraco-specific styling and classes
        element.classList.add('umb-toggle--disabled', 'disabled');
        element.style.cssText += `
            opacity: 0.6 !important;
            pointer-events: none !important;
            cursor: not-allowed !important;
        `;
        
        // Method 6: Disable the property container
        const propertyContainer = element.closest('.umb-property, .umb-editor, .umb-control-group');
        if (propertyContainer) {
            propertyContainer.classList.add('umb-property--disabled');
            propertyContainer.style.opacity = '0.6';
            console.log(`Property container for ${toggleId} also disabled`);
        }
        
        // Method 7: Add tooltip for user feedback
        element.title = 'Requires CommentsAdmin role';
        
        console.log(`Umbraco toggle ${toggleId} fully disabled and set to readonly`);
    });
    
    // Log any missing toggles
    const missingToggles = toggleIds.filter(id => !foundToggles[id]);
    if (missingToggles.length > 0) {
        console.warn('Could not find these toggles:', missingToggles);
        
        // Debug: List all elements with IDs for troubleshooting
        console.log('Available elements with IDs:');
        document.querySelectorAll('[id]').forEach(el => {
            if (el.id.toLowerCase().includes('display') || el.id.toLowerCase().includes('cms') || el.id.toLowerCase().includes('portal')) {
                console.log(`- ID: ${el.id}, Tag: ${el.tagName}, Classes: ${el.className}`);
            }
        });
    }
}

// Using window load event
window.addEventListener('load', function () {
    console.log('Page is fully loaded including images, stylesheets, etc.');
    initializePortalDisplayDependency();
});

// Main initialization function
async function initializePortalDisplayDependency() {
    console.log('Initializing Portal Display Dependency...');
    
    // Step 1: Load configuration from appsettings.json
    await loadConfigurationFromAPI();
    
    // Step 2: Get current user using Umbraco's Angular userService
    if (window.angular && angular.module) {
        try {
            const injector = angular.element(document.body).injector();
            if (injector) {
                const userService = injector.get('userService');
                if (userService) {
                    userService.getCurrentUser().then(function (user) {
                        console.log('Current user retrieved:', user);
                        console.log('User name:', user.name);
                        console.log('User email:', user.email);
                        console.log('User ID:', user.id);
                        console.log('User groups:', user.userGroups);

                        // Check if user is CommentsAdmin using dynamic configuration
                        const isCommentsAdmin = user.userGroups && user.userGroups.some(function (group) {
                            const groupName = group.name || group.alias || group;
                            return groupName.toLowerCase() === config.adminGroupName.toLowerCase();
                        });

                        console.log('Is CommentsAdmin:', isCommentsAdmin);

                        // Disable toggles if user is not CommentsAdmin using dynamic configuration
                        if (!isCommentsAdmin) {
                            disableUmbracoToggles([config.cmsToggleId, config.portalToggleId]);
                        }

                        // You can now use the user data for your portal display logic
                        // handlePortalDisplayLogic(user, isCommentsAdmin);

                    }).catch(function (error) {
                        console.error('Error getting current user:', error);
                    });
                } else {
                    console.warn('userService not available');
                }
            } else {
                console.warn('Angular injector not available');
            }
        } catch (error) {
            console.error('Error accessing Angular services:', error);
        }
    } else {
        console.warn('Angular not available or not loaded yet');
    }
}

// Enhanced Umbraco event listener setup with multiple approaches
function setupUmbracoEventListeners() {
    console.log('Setting up Umbraco event listeners...');
    
    // Approach 1: Try Angular events if available
    if (typeof angular !== 'undefined') {
        try {
            console.log('✅ Angular is available - registering Angular events');
            
            // Get the Umbraco app module
            const umbracoApp = angular.module('umbraco');
            
            umbracoApp.run(['$rootScope', '$location', function ($rootScope, $location) {
                console.log('Umbraco Angular app is running - registering events');
                
                // Listen for route changes (most reliable for navigation)
                $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
                    console.log('🔄 Route changed - reinitializing Portal Display Dependency');
                    setTimeout(initializePortalDisplayDependency, 1000);
                });
                
                // Listen for location changes
                $rootScope.$on('$locationChangeSuccess', function (event, newUrl, oldUrl) {
                    console.log('📍 Location changed - reinitializing Portal Display Dependency');
                    setTimeout(initializePortalDisplayDependency, 1000);
                });
                
                // Listen for content loaded events
                $rootScope.$on('contentLoaded', function () {
                    console.log('📄 Content loaded - reinitializing Portal Display Dependency');
                    setTimeout(initializePortalDisplayDependency, 500);
                });
                
                // Listen for form events
                $rootScope.$on('formSubmitted', function () {
                    console.log('📝 Form submitted - checking Portal Display Dependency');
                    setTimeout(initializePortalDisplayDependency, 500);
                });
                
                // Listen for app ready
                $rootScope.$on('appReady', function () {
                    console.log('🚀 App ready - initializing Portal Display Dependency');
                    setTimeout(initializePortalDisplayDependency, 500);
                });
            }]);
            
        } catch (e) {
            console.warn('Could not register Angular event listeners:', e);
        }
    }
    
    // Approach 2: URL change detection (fallback)
    let currentUrl = window.location.href;
    setInterval(function() {
        if (window.location.href !== currentUrl) {
            console.log('🔗 URL change detected - reinitializing Portal Display Dependency');
            currentUrl = window.location.href;
            setTimeout(initializePortalDisplayDependency, 1000);
        }
    }, 1000); // Check every second
    
    // Approach 3: MutationObserver for DOM changes (additional fallback)
    if (typeof MutationObserver !== 'undefined') {
        const observer = new MutationObserver(function(mutations) {
            let shouldReinitialize = false;
            
            mutations.forEach(function(mutation) {
                // Check if new nodes contain our target elements
                if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                    for (let node of mutation.addedNodes) {
                        if (node.nodeType === 1) { // Element node
                            // Check if it contains our toggle elements or Umbraco content
                            if (node.querySelector && (
                                node.querySelector('[id*="Display"]') || 
                                node.querySelector('.umb-property') ||
                                node.classList?.contains('umb-editor')
                            )) {
                                shouldReinitialize = true;
                                break;
                            }
                        }
                    }
                }
            });
            
            if (shouldReinitialize) {
                console.log('🔍 DOM changes detected - reinitializing Portal Display Dependency');
                setTimeout(initializePortalDisplayDependency, 500);
            }
        });
        
        // Start observing
        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
        
        console.log('👁️ MutationObserver setup for DOM changes');
    }
}

// Debug Angular availability and setup listeners
console.log('=== Angular Debug Info ===');
console.log('typeof angular:', typeof angular);
console.log('window.angular:', window.angular);
console.log('Angular available:', typeof angular !== 'undefined');

// Setup event listeners immediately if Angular is available
if (typeof angular !== 'undefined') {
    setupUmbracoEventListeners();
} else {
    console.log('❌ Angular not available immediately, setting up watcher...');
    
    // Wait for Angular to become available
    let angularCheckAttempts = 0;
    const maxAngularChecks = 20;
    
    function checkForAngular() {
        angularCheckAttempts++;
        console.log(`Checking for Angular... attempt ${angularCheckAttempts}/${maxAngularChecks}`);
        
        if (typeof angular !== 'undefined') {
            console.log('✅ Angular found after waiting!');
            setupUmbracoEventListeners();
        } else if (angularCheckAttempts < maxAngularChecks) {
            setTimeout(checkForAngular, 500);
        } else {
            console.warn('❌ Angular never became available, using fallback methods only');
            // Still setup the non-Angular fallbacks
            setupUmbracoEventListeners();
        }
    }
    
    setTimeout(checkForAngular, 500);
}

// Or using onload (fallback)
window.onload = function () {
    console.log('Page fully loaded');
};