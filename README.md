# ?? FNZ ChatBot

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-purple.svg)](https://docs.microsoft.com/en-us/aspnet/core/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![C#](https://img.shields.io/badge/C%23-12.0-orange.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

Un assistant virtuel intelligent basé sur l'intelligence artificielle pour répondre aux questions avec précision et rapidité. Développé avec ASP.NET Core et une interface utilisateur moderne.

## ?? Table des matières

- [Aperçu](#aperçu)
- [Fonctionnalités](#fonctionnalités)
- [Prérequis](#prérequis)
- [Installation](#installation)
- [Configuration](#configuration)
- [Utilisation](#utilisation)
- [Architecture](#architecture)
- [Sécurité](#sécurité)
- [Contribution](#contribution)
- [License](#license)

## ?? Aperçu

FNZ ChatBot est une application web moderne qui offre une expérience de chat interactive alimentée par l'intelligence artificielle. L'application combine une interface utilisateur élégante avec des fonctionnalités avancées de traitement du langage naturel et de recherche sémantique.

### ? Points forts

- ?? **Intelligence Artificielle Avancée** - Powered by des modèles d'IA de pointe
- ?? **Conversations Continues** - Historique complet des conversations
- ?? **Sécurité Enterprise** - Authentification robuste et protection des données
- ?? **Interface Responsive** - Design moderne adapté à tous les appareils
- ? **Performance Optimisée** - Recherche sémantique rapide et efficace

## ?? Fonctionnalités

### Utilisateur Final
- ? Chat en temps réel avec assistant IA
- ? Historique des conversations persistant
- ? Suggestions de questions dynamiques
- ? Interface moderne et intuitive
- ? Support mobile et desktop
- ? Récupération de mot de passe sécurisée

### Administrateur
- ? Gestion des utilisateurs
- ? Base de connaissances configurable
- ? Analytics et statistiques
- ? Enrichissement automatique des données
- ? Recherche sémantique avancée
- ? Interface d'administration complète

### Fonctionnalités Techniques
- ? Authentification ASP.NET Core Identity
- ? Recherche sémantique avec embeddings
- ? Services de nettoyage automatique
- ? Middleware de sécurité personnalisé
- ? Envoi d'emails avec templates HTML
- ? Architecture modulaire et extensible

## ?? Prérequis

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) ou [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)
- Un serveur SMTP pour l'envoi d'emails (optionnel)

## ??? Installation

### 1. Cloner le repository

```bash
git clone https://github.com/votre-username/fnz-chatbot.git
cd fnz-chatbot
```

### 2. Restaurer les packages NuGet

```bash
dotnet restore
```

### 3. Configuration de la base de données

Modifiez la chaîne de connexion dans `appsettings.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FNZChatBotDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 4. Appliquer les migrations

```bash
dotnet ef database update
```

### 5. Lancer l'application

```bash
dotnet run
```

L'application sera accessible sur `https://localhost:7001` ou `http://localhost:5000`.

## ?? Configuration

### Configuration Email (optionnel)

Pour activer la récupération de mot de passe, configurez les paramètres SMTP dans `appsettings.json` :

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "votre-email@example.com",
    "SenderPassword": "votre-mot-de-passe-app",
    "SenderName": "FNZ ChatBot",
    "EnableSsl": true
  }
}
```

### Données initiales

L'application crée automatiquement un compte administrateur par défaut :
- **Email** : `admin@fnz.com`
- **Mot de passe** : `Admin123!`

## ?? Utilisation

### Pour les utilisateurs

1. **Connexion** : Utilisez vos identifiants pour vous connecter
2. **Chat** : Commencez une nouvelle conversation ou reprenez une ancienne
3. **Questions** : Posez vos questions en utilisant les suggestions ou en tapant librement
4. **Historique** : Consultez vos conversations précédentes dans "Mes Conversations"

### Pour les administrateurs

1. **Gestion utilisateurs** : Créez, modifiez et gérez les comptes utilisateur
2. **Base de connaissances** : Ajoutez et organisez le contenu de la base de données
3. **Analytics** : Consultez les statistiques d'utilisation et performance
4. **Recherche sémantique** : Testez et améliorez les réponses de l'IA

## ??? Architecture

### Structure du projet

```
FNZ-ChatBot/
??? Controllers/          # Contrôleurs MVC
?   ??? AccountController.cs
?   ??? ChatController.cs
?   ??? HomeController.cs
?   ??? KnowledgeBaseController.cs
?   ??? SemanticAdminController.cs
?   ??? UserManagementController.cs
??? Data/                 # Contexte de base de données
?   ??? ApplicationDbContext.cs
?   ??? SeedData.cs
??? Middleware/           # Middleware personnalisé
?   ??? ForcePasswordChangeMiddleware.cs
??? Models/              # Modèles de données
??? Services/            # Services métier
?   ??? ChatService.cs
?   ??? EmailService.cs
?   ??? EmbeddingService.cs
?   ??? SemanticChatService.cs
?   ??? VerificationCodeService.cs
??? Views/               # Vues Razor
??? wwwroot/             # Ressources statiques
    ??? css/
    ??? js/
    ??? lib/
```

### Technologies utilisées

- **Backend** : ASP.NET Core 8.0, Entity Framework Core
- **Frontend** : Razor Pages, Bootstrap 5, Font Awesome
- **Base de données** : SQL Server avec Entity Framework
- **IA/ML** : Microsoft.ML, embeddings sémantiques
- **Email** : MailKit pour l'envoi d'emails
- **Sécurité** : ASP.NET Core Identity

## ?? Sécurité

### Mesures de sécurité implémentées

- ? **Authentification forte** avec ASP.NET Core Identity
- ? **Validation des entrées** côté client et serveur
- ? **Protection CSRF** automatique
- ? **Headers de sécurité** configurés
- ? **Récupération de mot de passe sécurisée** avec codes temporaires
- ? **Nettoyage automatique** des données temporaires
- ? **Middleware de sécurité** personnalisé

### Bonnes pratiques

- Les mots de passe sont hachés avec ASP.NET Core Identity
- Les codes de vérification expirent après 15 minutes
- Nettoyage automatique des données sensibles
- Validation des entrées utilisateur
- Protection contre les attaques courantes (XSS, CSRF, SQL Injection)

## ?? Tests

### Lancer les tests

```bash
dotnet test
```

### Test de l'interface

L'application inclut une page de démonstration accessible à `demo-chat-moderne.html` pour tester l'interface utilisateur.

## ?? Monitoring et Analytics

L'application propose des outils d'analytics intégrés :

- ?? Statistiques d'utilisation des conversations
- ?? Performance de la recherche sémantique
- ?? Analytics des utilisateurs
- ?? Logs des actions administratives

## ?? Contribution

Les contributions sont les bienvenues ! Voici comment contribuer :

1. Fork le projet
2. Créez une branche feature (`git checkout -b feature/amazing-feature`)
3. Committez vos changements (`git commit -m 'Add amazing feature'`)
4. Push vers la branche (`git push origin feature/amazing-feature`)
5. Ouvrez une Pull Request

### Standards de code

- Utilisez les conventions de nommage C#
- Documentez les méthodes publiques
- Ajoutez des tests pour les nouvelles fonctionnalités
- Respectez l'architecture existante

## ?? Changelog

### Version 1.0.0
- ? Interface de chat moderne
- ? Système d'authentification complet
- ? Recherche sémantique avancée
- ? Gestion des utilisateurs
- ? Base de connaissances configurable
- ? Récupération de mot de passe
- ? Interface d'administration

## ?? License

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus de détails.

## ?? Support

Pour toute question ou problème :

- ?? Email : admin@fnz.tn
- ?? Issues : [GitHub Issues](https://github.com/votre-username/fnz-chatbot/issues)
- ?? Documentation : [Wiki](https://github.com/votre-username/fnz-chatbot/wiki)

## ?? Remerciements

- L'équipe FNZ pour le support et les retours
- La communauté .NET pour les outils et frameworks
- Tous les contributeurs du projet

---

<div align="center">
  <strong>Fait avec ?? par l'équipe FNZ</strong>
</div>