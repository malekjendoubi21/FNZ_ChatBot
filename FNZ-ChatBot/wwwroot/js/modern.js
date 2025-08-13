// ===== MODERN JAVASCRIPT FOR FNZ CHATBOT =====

// Utilitaires modernes
class ModernUI {
    constructor() {
        this.init();
    }

    init() {
        this.setupAnimations();
        this.setupInteractions();
        this.setupNotifications();
        this.setupNavigation();
        this.setupTheme();
    }

    // Animations au chargement
    setupAnimations() {
        // Observer pour les animations au scroll
        if ('IntersectionObserver' in window) {
            const observerOptions = {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            };

            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('animate__animated', 'animate__fadeInUp');
                        observer.unobserve(entry.target);
                    }
                });
            }, observerOptions);

            // Observer les éléments avec la classe .observe-me
            document.querySelectorAll('.observe-me').forEach(el => {
                observer.observe(el);
            });
        }

        // Animation en cascade pour les cartes
        this.animateCardsInSequence();
    }

    animateCardsInSequence() {
        const cards = document.querySelectorAll('.card');
        cards.forEach((card, index) => {
            if (!card.classList.contains('animate__animated')) {
                card.style.animationDelay = `${index * 0.1}s`;
                card.classList.add('animate__animated', 'animate__fadeInUp');
            }
        });
    }

    // Interactions modernes
    setupInteractions() {
        // Effet de survol avancé pour les boutons
        document.querySelectorAll('.btn').forEach(btn => {
            btn.addEventListener('mouseenter', this.handleButtonHover);
            btn.addEventListener('mouseleave', this.handleButtonLeave);
        });

        // Tooltip automatique pour les icônes
        this.setupTooltips();

        // Gestion des formulaires
        this.setupFormValidation();

        // Copy to clipboard avec feedback visuel
        this.setupCopyToClipboard();
    }

    handleButtonHover(e) {
        e.target.style.transform = 'translateY(-2px)';
        e.target.style.boxShadow = '0 8px 25px rgba(0,0,0,0.15)';
    }

    handleButtonLeave(e) {
        e.target.style.transform = 'translateY(0)';
        e.target.style.boxShadow = '';
    }

    setupTooltips() {
        // Initialiser Bootstrap tooltips si disponible
        if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
            const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
            tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        }
    }

    setupFormValidation() {
        // Validation en temps réel
        document.querySelectorAll('.form-control').forEach(input => {
            input.addEventListener('blur', this.validateField);
            input.addEventListener('input', this.handleFieldInput);
        });
    }

    validateField(e) {
        const field = e.target;
        const isValid = field.checkValidity();
        
        field.classList.remove('is-valid', 'is-invalid');
        if (field.value.trim() !== '') {
            field.classList.add(isValid ? 'is-valid' : 'is-invalid');
        }
    }

    handleFieldInput(e) {
        const field = e.target;
        if (field.classList.contains('is-invalid')) {
            if (field.checkValidity()) {
                field.classList.remove('is-invalid');
                field.classList.add('is-valid');
            }
        }
    }

    setupCopyToClipboard() {
        document.querySelectorAll('[data-copy]').forEach(btn => {
            btn.addEventListener('click', this.copyToClipboard);
        });
    }

    async copyToClipboard(e) {
        const button = e.currentTarget;
        const targetId = button.getAttribute('data-copy');
        const target = document.getElementById(targetId);
        
        if (!target) return;

        try {
            await navigator.clipboard.writeText(target.value || target.textContent);
            
            // Feedback visuel
            const originalHTML = button.innerHTML;
            button.innerHTML = '<i class="fas fa-check text-success"></i>';
            button.classList.add('btn-success');
            
            setTimeout(() => {
                button.innerHTML = originalHTML;
                button.classList.remove('btn-success');
            }, 2000);
            
        } catch (err) {
            console.error('Erreur lors de la copie:', err);
        }
    }

    // Notifications modernes
    setupNotifications() {
        this.showNotifications();
        this.setupNotificationAutoHide();
    }

    showNotifications() {
        const alerts = document.querySelectorAll('.alert');
        alerts.forEach((alert, index) => {
            alert.style.animationDelay = `${index * 0.1}s`;
            alert.classList.add('animate__animated', 'animate__slideInRight');
        });
    }

    setupNotificationAutoHide() {
        // Auto-hide notifications après 5 secondes
        document.querySelectorAll('.alert-dismissible').forEach(alert => {
            setTimeout(() => {
                if (alert.parentNode) {
                    alert.classList.add('animate__animated', 'animate__slideOutRight');
                    setTimeout(() => {
                        if (alert.parentNode) {
                            alert.remove();
                        }
                    }, 500);
                }
            }, 5000);
        });
    }

    // Navigation améliorée
    setupNavigation() {
        this.setupSmoothScroll();
        this.setupNavbarEffects();
        this.setupActiveNavigation();
    }

    setupSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }

    setupNavbarEffects() {
        const navbar = document.querySelector('.navbar');
        if (!navbar) return;

        window.addEventListener('scroll', () => {
            const scrolled = window.scrollY > 50;
            
            if (scrolled) {
                navbar.style.background = 'rgba(255, 255, 255, 0.98)';
                navbar.style.boxShadow = '0 4px 20px rgba(0, 0, 0, 0.1)';
                navbar.style.backdropFilter = 'blur(20px)';
            } else {
                navbar.style.background = 'rgba(255, 255, 255, 0.95)';
                navbar.style.boxShadow = '0 1px 2px rgba(0, 0, 0, 0.05)';
                navbar.style.backdropFilter = 'blur(10px)';
            }
        });
    }

    setupActiveNavigation() {
        const currentPath = window.location.pathname;
        document.querySelectorAll('.nav-link').forEach(link => {
            if (link.getAttribute('href') === currentPath) {
                link.classList.add('active');
            }
        });
    }

    // Gestion du thème
    setupTheme() {
        this.detectColorScheme();
        this.setupThemeToggle();
    }

    detectColorScheme() {
        // Détecter la préférence du système
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            document.body.classList.add('dark-theme');
        }

        // Écouter les changements
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
            if (e.matches) {
                document.body.classList.add('dark-theme');
            } else {
                document.body.classList.remove('dark-theme');
            }
        });
    }

    setupThemeToggle() {
        const themeToggle = document.getElementById('theme-toggle');
        if (themeToggle) {
            themeToggle.addEventListener('click', () => {
                document.body.classList.toggle('dark-theme');
                localStorage.setItem('theme', document.body.classList.contains('dark-theme') ? 'dark' : 'light');
            });
        }

        // Restaurer le thème sauvegardé
        const savedTheme = localStorage.getItem('theme');
        if (savedTheme === 'dark') {
            document.body.classList.add('dark-theme');
        }
    }

    // Utilitaires publics
    static showToast(message, type = 'info') {
        const toastHTML = `
            <div class="toast align-items-center text-white bg-${type} border-0 animate__animated animate__slideInRight" role="alert">
                <div class="d-flex">
                    <div class="toast-body">
                        ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `;

        let toastContainer = document.querySelector('.toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
            document.body.appendChild(toastContainer);
        }

        toastContainer.insertAdjacentHTML('beforeend', toastHTML);
        const toast = toastContainer.lastElementChild;
        
        if (typeof bootstrap !== 'undefined' && bootstrap.Toast) {
            new bootstrap.Toast(toast).show();
        }

        // Auto-remove après animation
        setTimeout(() => {
            toast.remove();
        }, 5000);
    }

    static animateCounter(element, target, duration = 2000) {
        const start = 0;
        const increment = target / (duration / 16);
        let current = start;

        const timer = setInterval(() => {
            current += increment;
            element.textContent = Math.floor(current);

            if (current >= target) {
                element.textContent = target;
                clearInterval(timer);
            }
        }, 16);
    }

    static loadingButton(button, isLoading = true) {
        if (isLoading) {
            button.disabled = true;
            button.dataset.originalText = button.innerHTML;
            button.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Chargement...';
        } else {
            button.disabled = false;
            button.innerHTML = button.dataset.originalText;
        }
    }
}

// Fonctions spécifiques aux pages
class PageSpecificJS {
    static userManagement() {
        // Animation des statistiques
        document.querySelectorAll('.stat-icon + div h3').forEach(stat => {
            const target = parseInt(stat.textContent);
            ModernUI.animateCounter(stat, target);
        });

        // Recherche en temps réel
        const searchInput = document.getElementById('user-search');
        if (searchInput) {
            searchInput.addEventListener('input', this.filterUsers);
        }
    }

    static filterUsers(e) {
        const searchTerm = e.target.value.toLowerCase();
        const userRows = document.querySelectorAll('.user-row');

        userRows.forEach(row => {
            const userName = row.querySelector('.fw-semibold').textContent.toLowerCase();
            const userEmail = row.querySelector('.text-muted').textContent.toLowerCase();
            
            if (userName.includes(searchTerm) || userEmail.includes(searchTerm)) {
                row.style.display = '';
                row.classList.add('animate__animated', 'animate__fadeIn');
            } else {
                row.style.display = 'none';
            }
        });
    }

    static addUser() {
        // Validation avancée du formulaire
        const form = document.querySelector('.needs-validation');
        if (form) {
            form.addEventListener('submit', this.handleFormSubmit);
        }

        // Génération d'aperçu du mot de passe
        this.showPasswordPreview();
    }

    static handleFormSubmit(e) {
        const form = e.target;
        const submitButton = form.querySelector('button[type="submit"]');

        if (form.checkValidity()) {
            ModernUI.loadingButton(submitButton, true);
        } else {
            e.preventDefault();
            e.stopPropagation();
        }

        form.classList.add('was-validated');
    }

    static showPasswordPreview() {
        const passwordExample = document.getElementById('password-example');
        if (passwordExample) {
            // Simuler un aperçu de mot de passe sécurisé
            const example = 'Xy9$mK4#nL8@';
            passwordExample.textContent = example;
            
            // Animation de caractères
            let i = 0;
            const interval = setInterval(() => {
                if (i < example.length) {
                    passwordExample.textContent = example.substring(0, i + 1);
                    i++;
                } else {
                    clearInterval(interval);
                }
            }, 100);
        }
    }

    static home() {
        // Animation du robot
        this.animateRobot();
        
        // Statistiques en temps réel
        this.updateStats();
    }

    static animateRobot() {
        const robot = document.querySelector('.hero-image i');
        if (robot) {
            setInterval(() => {
                robot.style.transform = 'scale(1.1)';
                setTimeout(() => {
                    robot.style.transform = 'scale(1)';
                }, 200);
            }, 3000);
        }
    }

    static updateStats() {
        // Simuler des stats en temps réel
        const statElements = document.querySelectorAll('.stat-number');
        statElements.forEach(stat => {
            const originalValue = parseInt(stat.textContent);
            setInterval(() => {
                const variation = Math.floor(Math.random() * 3) - 1; // -1, 0, ou 1
                const newValue = Math.max(0, originalValue + variation);
                stat.textContent = newValue;
            }, 30000); // Mise à jour toutes les 30 secondes
        });
    }
}

// Initialisation
document.addEventListener('DOMContentLoaded', function() {
    // Initialiser l'UI moderne
    new ModernUI();

    // Initialiser les fonctionnalités spécifiques à la page
    const page = document.body.getAttribute('data-page');
    switch(page) {
        case 'user-management':
            PageSpecificJS.userManagement();
            break;
        case 'add-user':
            PageSpecificJS.addUser();
            break;
        case 'home':
            PageSpecificJS.home();
            break;
    }

    // Effet de chargement global
    document.body.classList.add('loaded');
});

// Export pour utilisation globale
window.ModernUI = ModernUI;
window.PageSpecificJS = PageSpecificJS;