# ?? Système de Récupération de Mot de Passe - FNZ ChatBot

## ?? Vue d'ensemble

Le système de récupération de mot de passe permet aux utilisateurs de réinitialiser leur mot de passe en cas d'oubli. Le processus inclut :

1. **Vérification de l'existence de l'email**
2. **Génération d'un code de vérification à 6 chiffres**
3. **Envoi du code par email**
4. **Validation du code et réinitialisation du mot de passe**

## ?? Architecture Technique

### **Modèles Créés**

#### **1. ForgotPasswordViewModel**
```csharp
public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
```

#### **2. ResetPasswordViewModel**
```csharp
public class ResetPasswordViewModel
{
    [Required] [EmailAddress]
    public string Email { get; set; }
    
    [Required] [StringLength(6, MinimumLength = 6)]
    public string VerificationCode { get; set; }
    
    [Required] [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; }
    
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; }
}
```

#### **3. PasswordResetCode (Entité DB)**
```csharp
public class PasswordResetCode
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Code { get; set; } // 6 chiffres
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; } // +15 minutes
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public virtual ApplicationUser User { get; set; }
    public bool IsValid => !IsUsed && DateTime.UtcNow <= ExpiresAt;
}
```

### **Services Implémentés**

#### **1. IVerificationCodeService**
```csharp
public interface IVerificationCodeService
{
    string GenerateVerificationCode();
    Task<bool> CreatePasswordResetCodeAsync(string userId, string code);
    Task<bool> ValidatePasswordResetCodeAsync(string userId, string code);
    Task<bool> MarkCodeAsUsedAsync(string userId, string code);
    Task CleanupExpiredCodesAsync();
}
```

**Fonctionnalités :**
- **Génération de codes** : 6 chiffres aléatoires (100000-999999)
- **Invalidation automatique** : Les anciens codes non utilisés sont marqués comme utilisés
- **Validation temporelle** : Codes valides 15 minutes
- **Nettoyage automatique** : Suppression des codes expirés

#### **2. Extension EmailService**
```csharp
Task SendPasswordResetCodeAsync(string toEmail, string firstName, string verificationCode);
```

**Template Email :**
- Design moderne avec gradient FNZ
- Code de vérification en gros caractères monospace
- Instructions détaillées
- Avertissements de sécurité
- Informations de validité (15 minutes)

#### **3. CleanupBackgroundService**
```csharp
public class CleanupBackgroundService : BackgroundService
{
    // Nettoyage automatique toutes les heures
    // Supprime les codes expirés de la base de données
}
```

## ?? Flux Utilisateur

### **1. Accès à "Mot de passe oublié"**

**Page :** `/Account/Login`
- Nouveau bouton "Mot de passe oublié ?" avec icône clé
- Style orange warning pour attirer l'attention
- Redirection vers `/Account/ForgotPassword`

### **2. Demande de code de vérification**

**Page :** `/Account/ForgotPassword`

**Processus :**
1. Utilisateur saisit son email
2. Système vérifie l'existence de l'email
3. Si trouvé :
   - Génération d'un code 6 chiffres
   - Invalidation des anciens codes
   - Envoi par email
   - Redirection vers reset password
4. Si non trouvé :
   - Message générique (sécurité)
   - Pas de révélation d'existence

**Design :**
- Carte moderne avec header orange
- Instructions étape par étape
- Liens vers autres actions
- Support et aide

### **3. Réinitialisation du mot de passe**

**Page :** `/Account/ResetPassword`

**Processus :**
1. Saisie email + code de vérification
2. Validation du code (existence, expiration, utilisation)
3. Saisie nouveau mot de passe avec confirmation
4. Indicateur de force en temps réel
5. Réinitialisation et marquage du code comme utilisé

**Fonctionnalités UX :**
- Champ code stylisé (monospace, centré, grande taille)
- Validation temps réel de la confirmation
- Barre de progression de force du mot de passe
- Conseils de sécurité visuels
- Auto-formatting du code (chiffres uniquement)

## ?? Sécurité Implémentée

### **1. Codes de Vérification**
- **Durée de vie limitée** : 15 minutes
- **Usage unique** : Marqués comme utilisés après utilisation
- **Invalidation automatique** : Anciens codes invalidés à chaque nouveau
- **Chiffres uniquement** : 6 chiffres numériques

### **2. Vérification Email**
- **Existence vérifiée** : Seuls les emails existants reçoivent des codes
- **Messages génériques** : Pas de révélation d'existence pour la sécurité
- **Logging sécurisé** : Journalisation des tentatives

### **3. Protection Contre Abus**
- **Nettoyage automatique** : Suppression des codes expirés
- **Limitation temporelle** : 15 minutes max par code
- **Validation stricte** : Vérifications multiples

### **4. Base de Données**
- **Indexes optimisés** : Pour UserId, Code, ExpiresAt
- **Cascade Delete** : Suppression auto si utilisateur supprimé
- **Contraintes** : Code exactement 6 caractères

## ?? Interface Utilisateur

### **Design System Cohérent**
- **Couleurs** : Orange pour forgot password, bleu pour reset
- **Animations** : Transitions fluides avec delays progressifs
- **Responsive** : Adapté mobile et desktop
- **Icônes** : Font Awesome cohérent
- **Typographie** : Inter pour modernité

### **Pages Créées**

#### **1. /Account/ForgotPassword**
```razor
- Header orange avec icône clé
- Instructions étape par étape
- Champ email avec validation
- Liens vers connexion et reset
- Aide et support
```

#### **2. /Account/ResetPassword**
```razor
- Header bleu avec icône lock-open
- Instructions détaillées
- Champ email pré-rempli
- Champ code stylisé (monospace)
- Champs mot de passe avec force indicator
- Conseils de sécurité visuels
- Aide et liens utiles
```

### **Styles CSS Ajoutés**
```css
.forgot-password-page { /* Styles pour forgot password */ }
.reset-password-page { /* Styles pour reset password */ }
.verification-code-input { /* Style spécial pour code */ }
.password-strength-bar { /* Indicateur de force */ }
```

## ?? Templates Email

### **Template de Code de Vérification**
```html
- Header avec gradient FNZ
- Code en gros caractères monospace
- Instructions détaillées
- Avertissements de sécurité
- Informations de validité
- Contact support
- Footer professionnel
```

**Informations incluses :**
- Code de vérification (6 chiffres)
- Nom de l'utilisateur
- Email de destination
- Durée de validité (15 minutes)
- Instructions étape par étape
- Contact support admin@fnz.tn

## ?? Configuration et Déploiement

### **Services Enregistrés (Program.cs)**
```csharp
builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();
builder.Services.AddHostedService<CleanupBackgroundService>();
```

### **Migration Base de Données**
```bash
dotnet ef migrations add AddPasswordResetCode
dotnet ef database update
```

### **Table Créée**
```sql
CREATE TABLE [PasswordResetCodes] (
    [Id] int IDENTITY PRIMARY KEY,
    [UserId] nvarchar(450) NOT NULL,
    [Code] nvarchar(6) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [UsedAt] datetime2 NULL,
    FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);

-- Index pour performances
CREATE INDEX [IX_PasswordResetCodes_UserId] ON [PasswordResetCodes] ([UserId]);
CREATE INDEX [IX_PasswordResetCodes_Code] ON [PasswordResetCodes] ([Code]);
CREATE INDEX [IX_PasswordResetCodes_ExpiresAt] ON [PasswordResetCodes] ([ExpiresAt]);
```

## ?? Contrôleur Actions

### **AccountController Étendu**

#### **1. ForgotPassword [GET/POST]**
```csharp
[HttpGet] public IActionResult ForgotPassword()
[HttpPost] public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
```

#### **2. ResetPassword [GET/POST]**
```csharp
[HttpGet] public IActionResult ResetPassword()
[HttpPost] public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
```

**Fonctionnalités :**
- Validation des modèles
- Vérification d'existence email
- Génération et validation codes
- Envoi d'emails
- Réinitialisation mot de passe
- Gestion d'erreurs complète
- Logging sécurisé

## ? Résultat Final

### **Fonctionnalités Implémentées**
- ? **Vérification email** : Existence vérifiée avant envoi
- ? **Code par email** : 6 chiffres avec template moderne
- ? **Validation sécurisée** : Expiration, usage unique
- ? **Interface moderne** : Design cohérent et responsive
- ? **UX optimisée** : Validation temps réel, indicateurs visuels
- ? **Sécurité renforcée** : Logging, nettoyage, protection abus
- ? **Nettoyage automatique** : Service background
- ? **Documentation** : Code commenté et guide complet

### **Workflow Complet**
1. **Utilisateur** ? "Mot de passe oublié" sur login
2. **Système** ? Vérifie email et génère code
3. **Email** ? Code envoyé avec template moderne
4. **Utilisateur** ? Saisit code et nouveau mot de passe
5. **Système** ? Valide code et réinitialise mot de passe
6. **Utilisateur** ? Peut se connecter avec nouveau mot de passe

### **Technologies Utilisées**
- **ASP.NET Core 8** pour l'architecture
- **Entity Framework Core** pour la persistance
- **Identity** pour la gestion utilisateurs
- **MailKit** pour l'envoi d'emails
- **Bootstrap 5** pour le responsive
- **Font Awesome** pour les icônes
- **CSS Grid/Flexbox** pour le layout

Le système est maintenant prêt pour la production avec toutes les fonctionnalités de sécurité et UX modernes ! ??