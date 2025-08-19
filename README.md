# ?? FNZ ChatBot

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-purple.svg)](https://docs.microsoft.com/en-us/aspnet/core/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![C#](https://img.shields.io/badge/C%23-12.0-orange.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

Un assistant virtuel intelligent bas� sur l'intelligence artificielle pour r�pondre aux questions avec pr�cision et rapidit�. D�velopp� avec ASP.NET Core et une interface utilisateur moderne.

## ?? Table des mati�res

- [Aper�u](#aper�u)
- [Fonctionnalit�s](#fonctionnalit�s)
- [Pr�requis](#pr�requis)
- [Installation](#installation)
- [Configuration](#configuration)
- [Utilisation](#utilisation)
- [Architecture](#architecture)
- [S�curit�](#s�curit�)
- [Contribution](#contribution)
- [License](#license)

## ?? Aper�u

FNZ ChatBot est une application web moderne qui offre une exp�rience de chat interactive aliment�e par l'intelligence artificielle. L'application combine une interface utilisateur �l�gante avec des fonctionnalit�s avanc�es de traitement du langage naturel et de recherche s�mantique.

### ? Points forts

- ?? **Intelligence Artificielle Avanc�e** - Powered by des mod�les d'IA de pointe
- ?? **Conversations Continues** - Historique complet des conversations
- ?? **S�curit� Enterprise** - Authentification robuste et protection des donn�es
- ?? **Interface Responsive** - Design moderne adapt� � tous les appareils
- ? **Performance Optimis�e** - Recherche s�mantique rapide et efficace

## ?? Fonctionnalit�s

### Utilisateur Final
- ? Chat en temps r�el avec assistant IA
- ? Historique des conversations persistant
- ? Suggestions de questions dynamiques
- ? Interface moderne et intuitive
- ? Support mobile et desktop
- ? R�cup�ration de mot de passe s�curis�e

### Administrateur
- ? Gestion des utilisateurs
- ? Base de connaissances configurable
- ? Analytics et statistiques
- ? Enrichissement automatique des donn�es
- ? Recherche s�mantique avanc�e
- ? Interface d'administration compl�te

### Fonctionnalit�s Techniques
- ? Authentification ASP.NET Core Identity
- ? Recherche s�mantique avec embeddings
- ? Services de nettoyage automatique
- ? Middleware de s�curit� personnalis�
- ? Envoi d'emails avec templates HTML
- ? Architecture modulaire et extensible

## ?? Pr�requis

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

### 3. Configuration de la base de donn�es

Modifiez la cha�ne de connexion dans `appsettings.json` :

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

Pour activer la r�cup�ration de mot de passe, configurez les param�tres SMTP dans `appsettings.json` :

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

### Donn�es initiales

L'application cr�e automatiquement un compte administrateur par d�faut :
- **Email** : `admin@fnz.com`
- **Mot de passe** : `Admin123!`

## ?? Utilisation

### Pour les utilisateurs

1. **Connexion** : Utilisez vos identifiants pour vous connecter
2. **Chat** : Commencez une nouvelle conversation ou reprenez une ancienne
3. **Questions** : Posez vos questions en utilisant les suggestions ou en tapant librement
4. **Historique** : Consultez vos conversations pr�c�dentes dans "Mes Conversations"

### Pour les administrateurs

1. **Gestion utilisateurs** : Cr�ez, modifiez et g�rez les comptes utilisateur
2. **Base de connaissances** : Ajoutez et organisez le contenu de la base de donn�es
3. **Analytics** : Consultez les statistiques d'utilisation et performance
4. **Recherche s�mantique** : Testez et am�liorez les r�ponses de l'IA

## ??? Architecture

### Structure du projet

```
FNZ-ChatBot/
??? Controllers/          # Contr�leurs MVC
?   ??? AccountController.cs
?   ??? ChatController.cs
?   ??? HomeController.cs
?   ??? KnowledgeBaseController.cs
?   ??? SemanticAdminController.cs
?   ??? UserManagementController.cs
??? Data/                 # Contexte de base de donn�es
?   ??? ApplicationDbContext.cs
?   ??? SeedData.cs
??? Middleware/           # Middleware personnalis�
?   ??? ForcePasswordChangeMiddleware.cs
??? Models/              # Mod�les de donn�es
??? Services/            # Services m�tier
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

### Technologies utilis�es

- **Backend** : ASP.NET Core 8.0, Entity Framework Core
- **Frontend** : Razor Pages, Bootstrap 5, Font Awesome
- **Base de donn�es** : SQL Server avec Entity Framework
- **IA/ML** : Microsoft.ML, embeddings s�mantiques
- **Email** : MailKit pour l'envoi d'emails
- **S�curit�** : ASP.NET Core Identity

## ?? S�curit�

### Mesures de s�curit� impl�ment�es

- ? **Authentification forte** avec ASP.NET Core Identity
- ? **Validation des entr�es** c�t� client et serveur
- ? **Protection CSRF** automatique
- ? **Headers de s�curit�** configur�s
- ? **R�cup�ration de mot de passe s�curis�e** avec codes temporaires
- ? **Nettoyage automatique** des donn�es temporaires
- ? **Middleware de s�curit�** personnalis�

### Bonnes pratiques

- Les mots de passe sont hach�s avec ASP.NET Core Identity
- Les codes de v�rification expirent apr�s 15 minutes
- Nettoyage automatique des donn�es sensibles
- Validation des entr�es utilisateur
- Protection contre les attaques courantes (XSS, CSRF, SQL Injection)

## ?? Tests

### Lancer les tests

```bash
dotnet test
```

### Test de l'interface

L'application inclut une page de d�monstration accessible � `demo-chat-moderne.html` pour tester l'interface utilisateur.

## ?? Monitoring et Analytics

L'application propose des outils d'analytics int�gr�s :

- ?? Statistiques d'utilisation des conversations
- ?? Performance de la recherche s�mantique
- ?? Analytics des utilisateurs
- ?? Logs des actions administratives

## ?? Contribution

Les contributions sont les bienvenues ! Voici comment contribuer :

1. Fork le projet
2. Cr�ez une branche feature (`git checkout -b feature/amazing-feature`)
3. Committez vos changements (`git commit -m 'Add amazing feature'`)
4. Push vers la branche (`git push origin feature/amazing-feature`)
5. Ouvrez une Pull Request

### Standards de code

- Utilisez les conventions de nommage C#
- Documentez les m�thodes publiques
- Ajoutez des tests pour les nouvelles fonctionnalit�s
- Respectez l'architecture existante

## ?? Changelog

### Version 1.0.0
- ? Interface de chat moderne
- ? Syst�me d'authentification complet
- ? Recherche s�mantique avanc�e
- ? Gestion des utilisateurs
- ? Base de connaissances configurable
- ? R�cup�ration de mot de passe
- ? Interface d'administration

## ?? License

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus de d�tails.

## ?? Support

Pour toute question ou probl�me :

- ?? Email : admin@fnz.tn
- ?? Issues : [GitHub Issues](https://github.com/votre-username/fnz-chatbot/issues)
- ?? Documentation : [Wiki](https://github.com/votre-username/fnz-chatbot/wiki)

## ?? Remerciements

- L'�quipe FNZ pour le support et les retours
- La communaut� .NET pour les outils et frameworks
- Tous les contributeurs du projet

---

<div align="center">
  <strong>Fait avec ?? par l'�quipe FNZ</strong>
</div>