// ===== CHAT MODERNE - JAVASCRIPT =====

class ModernChat {
    constructor() {
        this.initializeElements();
        this.bindEvents();
        this.currentConversationId = this.getConversationIdFromUrl();
        this.initializeChat();
    }

    initializeElements() {
        this.chatInput = document.getElementById('chatInput');
        this.sendButton = document.getElementById('sendButton');
        this.messagesContainer = document.getElementById('messagesContainer');
        this.conversationsList = document.getElementById('conversationsList');
        this.newChatBtn = document.getElementById('newChatBtn');
        this.chatTitle = document.getElementById('chatTitle');
        this.chatStatus = document.getElementById('chatStatus');
        this.typingIndicator = null;
    }

    bindEvents() {
        // Événements du chat
        if (this.sendButton) {
            this.sendButton.addEventListener('click', () => this.sendMessage());
        }

        if (this.chatInput) {
            this.chatInput.addEventListener('keydown', (e) => {
                if (e.key === 'Enter' && !e.shiftKey) {
                    e.preventDefault();
                    this.sendMessage();
                }
            });

            // Auto-resize du textarea
            this.chatInput.addEventListener('input', () => {
                this.autoResizeTextarea();
                this.updateSendButtonState();
            });

            // Focus effect
            this.chatInput.addEventListener('focus', () => {
                this.chatInput.parentElement.classList.add('focused');
            });

            this.chatInput.addEventListener('blur', () => {
                this.chatInput.parentElement.classList.remove('focused');
            });
        }

        // Nouveau chat
        if (this.newChatBtn) {
            this.newChatBtn.addEventListener('click', () => this.startNewConversation());
        }

        // Suggestions
        this.bindSuggestionChips();

        // Auto-scroll des messages
        this.setupAutoScroll();
    }

    autoResizeTextarea() {
        if (!this.chatInput) return;
        
        this.chatInput.style.height = 'auto';
        const scrollHeight = this.chatInput.scrollHeight;
        const maxHeight = 120; // max-height défini en CSS
        
        if (scrollHeight <= maxHeight) {
            this.chatInput.style.height = scrollHeight + 'px';
        } else {
            this.chatInput.style.height = maxHeight + 'px';
        }
    }

    updateSendButtonState() {
        if (!this.sendButton || !this.chatInput) return;
        
        const hasText = this.chatInput.value.trim().length > 0;
        this.sendButton.disabled = !hasText;
        
        if (hasText) {
            this.sendButton.classList.add('active');
        } else {
            this.sendButton.classList.remove('active');
        }
    }

    bindSuggestionChips() {
        const suggestionChips = document.querySelectorAll('.suggestion-chip');
        suggestionChips.forEach(chip => {
            chip.addEventListener('click', () => {
                const question = chip.textContent.trim();
                if (this.chatInput) {
                    this.chatInput.value = question;
                    this.autoResizeTextarea();
                    this.updateSendButtonState();
                    this.chatInput.focus();
                }
            });
        });
    }

    setupAutoScroll() {
        if (!this.messagesContainer) return;
        
        // Observer pour le scroll automatique
        this.messagesObserver = new MutationObserver(() => {
            this.scrollToBottom();
        });
        
        this.messagesObserver.observe(this.messagesContainer, {
            childList: true,
            subtree: true
        });
    }

    scrollToBottom(smooth = true) {
        if (!this.messagesContainer) return;
        
        this.messagesContainer.scrollTo({
            top: this.messagesContainer.scrollHeight,
            behavior: smooth ? 'smooth' : 'auto'
        });
    }

    async sendMessage() {
        const message = this.chatInput.value.trim();
        if (!message) return;

        // Ajouter le message utilisateur
        this.addMessage(message, 'user');
        
        // Vider l'input
        this.chatInput.value = '';
        this.autoResizeTextarea();
        this.updateSendButtonState();
        
        // Afficher l'indicateur de frappe
        this.showTypingIndicator();
        
        try {
            const response = await this.sendToServer(message);
            this.hideTypingIndicator();
            
            if (response.success) {
                this.addMessage(response.answer, 'bot');
                
                // Mettre à jour l'ID de conversation
                if (response.conversationId && !this.currentConversationId) {
                    this.currentConversationId = response.conversationId;
                    this.updateUrl();
                    this.updateConversationsList();
                }
                
                // Mettre à jour le titre si fourni
                if (response.title) {
                    this.updateChatTitle(response.title);
                }
            } else {
                this.addMessage(response.error || 'Erreur de communication', 'error');
            }
        } catch (error) {
            this.hideTypingIndicator();
            this.addMessage('Erreur de connexion. Veuillez réessayer.', 'error');
            console.error('Erreur chat:', error);
        }
    }

    async sendToServer(message) {
        const response = await fetch('/Chat/GetResponse', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                userInput: message,
                conversationId: this.currentConversationId
            })
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        const data = await response.json();
        return {
            success: true,
            answer: data.answer,
            conversationId: data.conversationId,
            title: data.title
        };
    }

    addMessage(content, type, animate = true) {
        if (!this.messagesContainer) return;

        // Cacher l'état vide si présent
        this.hideEmptyState();

        const messageWrapper = document.createElement('div');
        messageWrapper.className = `message-wrapper ${type}`;
        
        if (animate) {
            messageWrapper.style.opacity = '0';
            messageWrapper.style.transform = 'translateY(20px)';
        }

        const messageTime = new Date().toLocaleTimeString('fr-FR', { 
            hour: '2-digit', 
            minute: '2-digit' 
        });

        let avatarIcon = '';
        switch (type) {
            case 'user':
                avatarIcon = '<i class="fas fa-user"></i>';
                break;
            case 'bot':
                avatarIcon = '<i class="fas fa-robot"></i>';
                break;
            case 'error':
                avatarIcon = '<i class="fas fa-exclamation-triangle"></i>';
                break;
        }

        messageWrapper.innerHTML = `
            <div class="message-avatar ${type}">
                ${avatarIcon}
            </div>
            <div class="message-content">
                <div class="message-bubble ${type}">
                    ${this.formatMessage(content)}
                </div>
                <div class="message-time">${messageTime}</div>
                ${type === 'user' ? '<div class="message-status"><i class="fas fa-check status-icon status-sent"></i></div>' : ''}
            </div>
        `;

        this.messagesContainer.appendChild(messageWrapper);

        if (animate) {
            // Animation d'entrée
            requestAnimationFrame(() => {
                messageWrapper.style.transition = 'all 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55)';
                messageWrapper.style.opacity = '1';
                messageWrapper.style.transform = 'translateY(0)';
            });
        }

        this.scrollToBottom();
    }

    formatMessage(content) {
        // Conversion des retours à la ligne en <br>
        let formatted = content.replace(/\n/g, '<br>');
        
        // Gestion des liens
        formatted = formatted.replace(
            /(https?:\/\/[^\s]+)/g, 
            '<a href="$1" target="_blank" rel="noopener noreferrer">$1</a>'
        );
        
        // Gestion du code inline
        formatted = formatted.replace(/`([^`]+)`/g, '<code>$1</code>');
        
        return formatted;
    }

    showTypingIndicator() {
        if (this.typingIndicator) return;

        this.typingIndicator = document.createElement('div');
        this.typingIndicator.className = 'typing-indicator';
        this.typingIndicator.innerHTML = `
            <div class="typing-avatar">
                <i class="fas fa-robot"></i>
            </div>
            <div class="typing-bubble">
                <div class="typing-dots">
                    <div class="typing-dot"></div>
                    <div class="typing-dot"></div>
                    <div class="typing-dot"></div>
                </div>
            </div>
        `;

        this.messagesContainer.appendChild(this.typingIndicator);
        this.scrollToBottom();
    }

    hideTypingIndicator() {
        if (this.typingIndicator) {
            this.typingIndicator.remove();
            this.typingIndicator = null;
        }
    }

    hideEmptyState() {
        const emptyState = document.querySelector('.chat-empty');
        if (emptyState) {
            emptyState.style.display = 'none';
        }
    }

    startNewConversation() {
        window.location.href = '/Chat/Index';
    }

    loadConversation(conversationId) {
        window.location.href = `/Chat/Index?conversationId=${conversationId}`;
    }

    getConversationIdFromUrl() {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get('conversationId');
    }

    updateUrl() {
        if (this.currentConversationId) {
            const url = new URL(window.location);
            url.searchParams.set('conversationId', this.currentConversationId);
            window.history.replaceState({}, '', url);
        }
    }

    updateChatTitle(title) {
        if (this.chatTitle) {
            this.chatTitle.textContent = title;
        }
    }

    updateConversationsList() {
        // Recharger la liste des conversations
        // Ici on pourrait faire un appel AJAX pour mettre à jour la sidebar
        setTimeout(() => {
            location.reload();
        }, 1000);
    }

    initializeChat() {
        // Focus initial sur l'input
        if (this.chatInput) {
            this.chatInput.focus();
        }
        
        // Scroll initial vers le bas
        this.scrollToBottom(false);
        
        // État initial du bouton
        this.updateSendButtonState();
        
        // Charger les messages existants avec animation
        this.animateExistingMessages();
    }

    animateExistingMessages() {
        const existingMessages = document.querySelectorAll('.message-wrapper');
        existingMessages.forEach((message, index) => {
            message.style.opacity = '0';
            message.style.transform = 'translateY(20px)';
            
            setTimeout(() => {
                message.style.transition = 'all 0.4s ease';
                message.style.opacity = '1';
                message.style.transform = 'translateY(0)';
            }, index * 100);
        });
    }
}

// Utilitaires pour les conversations
class ConversationManager {
    static setupConversationItems() {
        const conversationItems = document.querySelectorAll('.conversation-item');
        conversationItems.forEach(item => {
            item.addEventListener('click', () => {
                const conversationId = item.dataset.conversationId;
                if (conversationId) {
                    window.location.href = `/Chat/Index?conversationId=${conversationId}`;
                }
            });
        });
    }

    static setupDeleteButtons() {
        const deleteButtons = document.querySelectorAll('.delete-conversation');
        deleteButtons.forEach(button => {
            button.addEventListener('click', async (e) => {
                e.stopPropagation();
                
                const conversationId = button.dataset.conversationId;
                const conversationTitle = button.dataset.title;
                
                if (confirm(`Êtes-vous sûr de vouloir supprimer la conversation "${conversationTitle}" ?`)) {
                    try {
                        const response = await fetch(`/Chat/DeleteConversation/${conversationId}`, {
                            method: 'DELETE'
                        });
                        
                        if (response.ok) {
                            // Supprimer l'élément de la liste
                            const conversationItem = button.closest('.conversation-item');
                            conversationItem.style.transition = 'all 0.3s ease';
                            conversationItem.style.opacity = '0';
                            conversationItem.style.transform = 'translateX(-100%)';
                            
                            setTimeout(() => {
                                conversationItem.remove();
                            }, 300);
                        }
                    } catch (error) {
                        console.error('Erreur lors de la suppression:', error);
                        alert('Erreur lors de la suppression de la conversation');
                    }
                }
            });
        });
    }
}

// Animations et effets visuels
class ChatAnimations {
    static addRippleEffect(element) {
        element.addEventListener('click', function(e) {
            const ripple = document.createElement('div');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;
            
            ripple.style.cssText = `
                position: absolute;
                width: ${size}px;
                height: ${size}px;
                left: ${x}px;
                top: ${y}px;
                background: rgba(255, 255, 255, 0.3);
                border-radius: 50%;
                transform: scale(0);
                pointer-events: none;
                animation: ripple 0.6s ease-out;
            `;
            
            this.style.position = 'relative';
            this.style.overflow = 'hidden';
            this.appendChild(ripple);
            
            setTimeout(() => ripple.remove(), 600);
        });
    }

    static setupScrollIndicator() {
        const messagesContainer = document.getElementById('messagesContainer');
        if (!messagesContainer) return;

        const indicator = document.createElement('div');
        indicator.className = 'scroll-indicator';
        indicator.innerHTML = '<i class="fas fa-chevron-down"></i>';
        indicator.style.cssText = `
            position: absolute;
            bottom: 80px;
            right: 20px;
            width: 40px;
            height: 40px;
            background: var(--chat-primary);
            color: white;
            border-radius: 50%;
            display: none;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            z-index: 10;
        `;

        messagesContainer.parentElement.appendChild(indicator);

        messagesContainer.addEventListener('scroll', () => {
            const { scrollTop, scrollHeight, clientHeight } = messagesContainer;
            const isNearBottom = scrollHeight - scrollTop - clientHeight < 100;
            
            indicator.style.display = isNearBottom ? 'none' : 'flex';
        });

        indicator.addEventListener('click', () => {
            messagesContainer.scrollTo({
                top: messagesContainer.scrollHeight,
                behavior: 'smooth'
            });
        });
    }
}

// Initialisation du CSS pour les animations
const addAnimationStyles = () => {
    const style = document.createElement('style');
    style.textContent = `
        @keyframes ripple {
            to { transform: scale(2); opacity: 0; }
        }
        
        .message-wrapper {
            transition: all 0.3s ease;
        }
        
        .message-wrapper:hover {
            transform: translateY(-1px);
        }
        
        .conversation-item {
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        }
        
        .typing-indicator {
            animation: slideInUp 0.3s ease;
        }
        
        @keyframes slideInUp {
            from {
                opacity: 0;
                transform: translateY(20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
    `;
    document.head.appendChild(style);
};

// Fonction d'initialisation globale
function initializeModernChat() {
    // Ajouter les styles d'animation
    addAnimationStyles();
    
    // Initialiser le chat principal
    window.modernChat = new ModernChat();
    
    // Configurer les conversations
    ConversationManager.setupConversationItems();
    ConversationManager.setupDeleteButtons();
    
    // Ajouter les effets visuels
    const buttons = document.querySelectorAll('.send-button, .new-chat-btn, .action-btn');
    buttons.forEach(button => ChatAnimations.addRippleEffect(button));
    
    // Indicateur de scroll
    ChatAnimations.setupScrollIndicator();
    
    console.log('Chat moderne initialisé');
}

// Démarrage automatique
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeModernChat);
} else {
    initializeModernChat();
}

// Export pour utilisation externe
window.ModernChat = ModernChat;
window.ConversationManager = ConversationManager;
window.ChatAnimations = ChatAnimations;