// ===== SCRIPT DE CORRECTION AUTOMATIQUE DE VISIBILITÉ =====

document.addEventListener('DOMContentLoaded', function() {
    console.log('?? Correction automatique de visibilité activée');
    
    // Function to fix visibility issues
    function fixVisibilityIssues() {
        // Fix all buttons
        fixButtons();
        
        // Fix all text elements
        fixTextElements();
        
        // Fix all form elements
        fixFormElements();
        
        // Fix all cards
        fixCards();
        
        // Fix navigation
        fixNavigation();
        
        // Fix specific problematic elements
        fixProblematicElements();
        
        console.log('? Corrections de visibilité appliquées');
    }
    
    function fixButtons() {
        const buttons = document.querySelectorAll('.btn');
        buttons.forEach(button => {
            // Ensure minimum visibility
            if (window.getComputedStyle(button).opacity < 0.5) {
                button.style.opacity = '1';
            }
            
            // Ensure button has proper background
            if (button.classList.contains('btn-primary')) {
                button.style.background = 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)';
                button.style.color = '#ffffff';
                button.style.border = '2px solid transparent';
            }
            
            if (button.classList.contains('btn-success')) {
                button.style.background = 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)';
                button.style.color = '#ffffff';
                button.style.border = '2px solid transparent';
            }
            
            if (button.classList.contains('btn-outline-primary') || button.classList.contains('btn-outline-secondary')) {
                button.style.background = '#ffffff';
                button.style.color = '#667eea';
                button.style.border = '2px solid #667eea';
            }
            
            // Ensure button text is visible
            if (window.getComputedStyle(button).color === 'rgba(0, 0, 0, 0)' || 
                window.getComputedStyle(button).color === 'transparent') {
                button.style.color = '#ffffff';
            }
        });
    }
    
    function fixTextElements() {
        // Fix all text that might be invisible
        const textElements = document.querySelectorAll('p, span, div, h1, h2, h3, h4, h5, h6, a, label');
        textElements.forEach(element => {
            const computedStyle = window.getComputedStyle(element);
            
            // If text is transparent or nearly invisible
            if (computedStyle.opacity < 0.1 || 
                computedStyle.color === 'transparent' || 
                computedStyle.color === 'rgba(0, 0, 0, 0)') {
                
                // Determine appropriate color based on background
                const bgColor = getEffectiveBackgroundColor(element);
                if (isLightColor(bgColor)) {
                    element.style.color = '#212529'; // Dark text on light background
                } else {
                    element.style.color = '#ffffff'; // Light text on dark background
                }
                element.style.opacity = '1';
            }
        });
    }
    
    function fixFormElements() {
        // Fix form controls
        const formControls = document.querySelectorAll('.form-control, input, textarea, select');
        formControls.forEach(control => {
            control.style.backgroundColor = '#ffffff';
            control.style.color = '#495057';
            control.style.border = '2px solid #ced4da';
        });
        
        // Fix labels
        const labels = document.querySelectorAll('label, .form-label');
        labels.forEach(label => {
            label.style.color = '#495057';
            label.style.fontWeight = '500';
        });
    }
    
    function fixCards() {
        const cards = document.querySelectorAll('.card');
        cards.forEach(card => {
            // Ensure card has white background and dark text
            if (!card.classList.contains('card-gradient')) {
                card.style.backgroundColor = '#ffffff';
                card.style.color = '#212529';
                card.style.border = '1px solid #dee2e6';
            }
        });
    }
    
    function fixNavigation() {
        // Fix navbar links
        const navLinks = document.querySelectorAll('.nav-link');
        navLinks.forEach(link => {
            link.style.color = '#495057';
            link.style.fontWeight = '500';
        });
        
        // Fix navbar brand
        const navbarBrand = document.querySelector('.navbar-brand');
        if (navbarBrand) {
            navbarBrand.style.color = '#667eea';
            navbarBrand.style.fontWeight = '700';
        }
        
        // Fix dropdown items
        const dropdownItems = document.querySelectorAll('.dropdown-item');
        dropdownItems.forEach(item => {
            item.style.color = '#212529';
        });
    }
    
    function fixProblematicElements() {
        // Fix any element that might be problematic
        const allElements = document.querySelectorAll('*');
        allElements.forEach(element => {
            const computedStyle = window.getComputedStyle(element);
            
            // If element is completely transparent
            if (computedStyle.opacity === '0' && !element.classList.contains('animate__animated')) {
                element.style.opacity = '1';
            }
            
            // If element has no visibility
            if (computedStyle.visibility === 'hidden') {
                element.style.visibility = 'visible';
            }
        });
    }
    
    function getEffectiveBackgroundColor(element) {
        let bgColor = 'rgba(0, 0, 0, 0)'; // Default transparent
        let currentElement = element;
        
        while (currentElement && bgColor === 'rgba(0, 0, 0, 0)') {
            const style = window.getComputedStyle(currentElement);
            bgColor = style.backgroundColor;
            currentElement = currentElement.parentElement;
        }
        
        return bgColor;
    }
    
    function isLightColor(color) {
        // Convert color to RGB values
        const rgb = color.match(/\d+/g);
        if (!rgb || rgb.length < 3) return true; // Default to light
        
        const r = parseInt(rgb[0]);
        const g = parseInt(rgb[1]);
        const b = parseInt(rgb[2]);
        
        // Calculate brightness
        const brightness = (r * 299 + g * 587 + b * 114) / 1000;
        return brightness > 128;
    }
    
    // Apply fixes immediately
    fixVisibilityIssues();
    
    // Apply fixes after animations complete
    setTimeout(fixVisibilityIssues, 1000);
    
    // Apply fixes when window resizes
    window.addEventListener('resize', fixVisibilityIssues);
    
    // Apply fixes when new content is loaded
    const observer = new MutationObserver(function(mutations) {
        mutations.forEach(function(mutation) {
            if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                setTimeout(fixVisibilityIssues, 100);
            }
        });
    });
    
    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
    
    // Add manual trigger for debugging
    window.fixVisibility = fixVisibilityIssues;
    
    console.log('?? Tapez "fixVisibility()" dans la console pour forcer une correction');
    
    // Special fix for hero section
    function fixHeroSection() {
        const heroSection = document.querySelector('.hero-section');
        if (heroSection) {
            heroSection.style.background = 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)';
            
            const heroElements = heroSection.querySelectorAll('*');
            heroElements.forEach(element => {
                if (!element.classList.contains('btn')) {
                    element.style.color = '#ffffff';
                }
            });
            
            const heroButtons = heroSection.querySelectorAll('.btn');
            heroButtons.forEach(button => {
                button.style.background = 'rgba(255, 255, 255, 0.2)';
                button.style.border = '2px solid rgba(255, 255, 255, 0.3)';
                button.style.color = '#ffffff';
                button.style.backdropFilter = 'blur(10px)';
            });
        }
    }
    
    // Apply hero section fix
    setTimeout(fixHeroSection, 500);
});

// Add CSS debugging helper
function addDebuggingStyles() {
    const style = document.createElement('style');
    style.textContent = `
        .debug-visibility {
            border: 2px solid red !important;
            background: rgba(255, 255, 0, 0.3) !important;
        }
        
        .debug-invisible {
            border: 2px solid blue !important;
            background: rgba(0, 0, 255, 0.3) !important;
        }
    `;
    document.head.appendChild(style);
}

// Function to debug invisible elements
function debugInvisibleElements() {
    const allElements = document.querySelectorAll('*');
    allElements.forEach(element => {
        const style = window.getComputedStyle(element);
        
        if (style.opacity === '0' || style.visibility === 'hidden' || 
            style.color === 'transparent' || style.color === 'rgba(0, 0, 0, 0)') {
            element.classList.add('debug-invisible');
            console.log('Élément invisible détecté:', element);
        }
    });
}

// Export debugging functions for console use
window.debugInvisibleElements = debugInvisibleElements;
window.addDebuggingStyles = addDebuggingStyles;