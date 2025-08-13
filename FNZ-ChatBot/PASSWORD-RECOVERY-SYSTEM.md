# ?? Syst�me de R�cup�ration de Mot de Passe - FNZ ChatBot

## ?? Vue d'ensemble

Le syst�me de r�cup�ration de mot de passe permet aux utilisateurs de r�initialiser leur mot de passe en cas d'oubli. Le processus inclut :

1. **V�rification de l'existence de l'email**
2. **G�n�ration d'un code de v�rification � 6 chiffres**
3. **Envoi du code par email**
4. **Validation du code et r�initialisation du mot de passe**

## ?? Architecture Technique

### **Mod�les Cr��s**

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

#### **3. PasswordResetCode (Entit� DB)**
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

### **Services Impl�ment�s**

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

**Fonctionnalit�s :**
- **G�n�ration de codes** : 6 chiffres al�atoires (100000-999999)
- **Invalidation automatique** : Les anciens codes non utilis�s sont marqu�s comme utilis�s
- **Validation temporelle** : Codes valides 15 minutes
- **Nettoyage automatique** : Suppression des codes expir�s

#### **2. Extension EmailService**
```csharp
Task SendPasswordResetCodeAsync(string toEmail, string firstName, string verificationCode);
```

**Template Email :**
- Design moderne avec gradient FNZ
- Code de v�rification en gros caract�res monospace
- Instructions d�taill�es
- Avertissements de s�curit�
- Informations de validit� (15 minutes)

#### **3. CleanupBackgroundService**
```csharp
public class CleanupBackgroundService : BackgroundService
{
    // Nettoyage automatique toutes les heures
    // Supprime les codes expir�s de la base de donn�es
}
```

## ?? Flux Utilisateur

### **1. Acc�s � "Mot de passe oubli�"**

**Page :** `/Account/Login`
- Nouveau bouton "Mot de passe oubli� ?" avec ic�ne cl�
- Style orange warning pour attirer l'attention
- Redirection vers `/Account/ForgotPassword`

### **2. Demande de code de v�rification**

**Page :** `/Account/ForgotPassword`

**Processus :**
1. Utilisateur saisit son email
2. Syst�me v�rifie l'existence de l'email
3. Si trouv� :
   - G�n�ration d'un code 6 chiffres
   - Invalidation des anciens codes
   - Envoi par email
   - Redirection vers reset password
4. Si non trouv� :
   - Message g�n�rique (s�curit�)
   - Pas de r�v�lation d'existence

**Design :**
- Carte moderne avec header orange
- Instructions �tape par �tape
- Liens vers autres actions
- Support et aide

### **3. R�initialisation du mot de passe**

**Page :** `/Account/ResetPassword`

**Processus :**
1. Saisie email + code de v�rification
2. Validation du code (existence, expiration, utilisation)
3. Saisie nouveau mot de passe avec confirmation
4. Indicateur de force en temps r�el
5. R�initialisation et marquage du code comme utilis�

**Fonctionnalit�s UX :**
- Champ code stylis� (monospace, centr�, grande taille)
- Validation temps r�el de la confirmation
- Barre de progression de force du mot de passe
- Conseils de s�curit� visuels
- Auto-formatting du code (chiffres uniquement)

## ?? S�curit� Impl�ment�e

### **1. Codes de V�rification**
- **Dur�e de vie limit�e** : 15 minutes
- **Usage unique** : Marqu�s comme utilis�s apr�s utilisation
- **Invalidation automatique** : Anciens codes invalid�s � chaque nouveau
- **Chiffres uniquement** : 6 chiffres num�riques

### **2. V�rification Email**
- **Existence v�rifi�e** : Seuls les emails existants re�oivent des codes
- **Messages g�n�riques** : Pas de r�v�lation d'existence pour la s�curit�
- **Logging s�curis�** : Journalisation des tentatives

### **3. Protection Contre Abus**
- **Nettoyage automatique** : Suppression des codes expir�s
- **Limitation temporelle** : 15 minutes max par code
- **Validation stricte** : V�rifications multiples

### **4. Base de Donn�es**
- **Indexes optimis�s** : Pour UserId, Code, ExpiresAt
- **Cascade Delete** : Suppression auto si utilisateur supprim�
- **Contraintes** : Code exactement 6 caract�res

## ?? Interface Utilisateur

### **Design System Coh�rent**
- **Couleurs** : Orange pour forgot password, bleu pour reset
- **Animations** : Transitions fluides avec delays progressifs
- **Responsive** : Adapt� mobile et desktop
- **Ic�nes** : Font Awesome coh�rent
- **Typographie** : Inter pour modernit�

### **Pages Cr��es**

#### **1. /Account/ForgotPassword**
```razor
- Header orange avec ic�ne cl�
- Instructions �tape par �tape
- Champ email avec validation
- Liens vers connexion et reset
- Aide et support
```

#### **2. /Account/ResetPassword**
```razor
- Header bleu avec ic�ne lock-open
- Instructions d�taill�es
- Champ email pr�-rempli
- Champ code stylis� (monospace)
- Champs mot de passe avec force indicator
- Conseils de s�curit� visuels
- Aide et liens utiles
```

### **Styles CSS Ajout�s**
```css
.forgot-password-page { /* Styles pour forgot password */ }
.reset-password-page { /* Styles pour reset password */ }
.verification-code-input { /* Style sp�cial pour code */ }
.password-strength-bar { /* Indicateur de force */ }
```

## ?? Templates Email

### **Template de Code de V�rification**
```html
- Header avec gradient FNZ
- Code en gros caract�res monospace
- Instructions d�taill�es
- Avertissements de s�curit�
- Informations de validit�
- Contact support
- Footer professionnel
```

**Informations incluses :**
- Code de v�rification (6 chiffres)
- Nom de l'utilisateur
- Email de destination
- Dur�e de validit� (15 minutes)
- Instructions �tape par �tape
- Contact support admin@fnz.tn

## ?? Configuration et D�ploiement

### **Services Enregistr�s (Program.cs)**
```csharp
builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();
builder.Services.AddHostedService<CleanupBackgroundService>();
```

### **Migration Base de Donn�es**
```bash
dotnet ef migrations add AddPasswordResetCode
dotnet ef database update
```

### **Table Cr��e**
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

## ?? Contr�leur Actions

### **AccountController �tendu**

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

**Fonctionnalit�s :**
- Validation des mod�les
- V�rification d'existence email
- G�n�ration et validation codes
- Envoi d'emails
- R�initialisation mot de passe
- Gestion d'erreurs compl�te
- Logging s�curis�

## ? R�sultat Final

### **Fonctionnalit�s Impl�ment�es**
- ? **V�rification email** : Existence v�rifi�e avant envoi
- ? **Code par email** : 6 chiffres avec template moderne
- ? **Validation s�curis�e** : Expiration, usage unique
- ? **Interface moderne** : Design coh�rent et responsive
- ? **UX optimis�e** : Validation temps r�el, indicateurs visuels
- ? **S�curit� renforc�e** : Logging, nettoyage, protection abus
- ? **Nettoyage automatique** : Service background
- ? **Documentation** : Code comment� et guide complet

### **Workflow Complet**
1. **Utilisateur** ? "Mot de passe oubli�" sur login
2. **Syst�me** ? V�rifie email et g�n�re code
3. **Email** ? Code envoy� avec template moderne
4. **Utilisateur** ? Saisit code et nouveau mot de passe
5. **Syst�me** ? Valide code et r�initialise mot de passe
6. **Utilisateur** ? Peut se connecter avec nouveau mot de passe

### **Technologies Utilis�es**
- **ASP.NET Core 8** pour l'architecture
- **Entity Framework Core** pour la persistance
- **Identity** pour la gestion utilisateurs
- **MailKit** pour l'envoi d'emails
- **Bootstrap 5** pour le responsive
- **Font Awesome** pour les ic�nes
- **CSS Grid/Flexbox** pour le layout

Le syst�me est maintenant pr�t pour la production avec toutes les fonctionnalit�s de s�curit� et UX modernes ! ??