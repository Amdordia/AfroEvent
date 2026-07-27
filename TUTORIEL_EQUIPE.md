# 📚 Tutoriel Équipe AfroEvent — EF Core, Identity & aspnet-codegenerator

> **Rédigé par : AfroEvent**  
> **Projet :** AfroEvent — Plateforme Événementielle Africaine  
> **Framework :** ASP.NET Core 10.0 MVC | EF Core 10 | SQLite | Bootstrap 5

---

## ⚙️ Prérequis à Vérifier (les deux)

Avant de commencer, vérifiez que vous avez ces outils installés. Ouvrez un terminal et tapez :

```bash
dotnet --version
# Attendu : 10.x.x
```

Installez les outils globaux dotnet :
```bash
dotnet tool install --global dotnet-ef
dotnet tool install --global dotnet-aspnet-codegenerator
```

> **Si la commande dit que l'outil est déjà installé :**
> ```bash
> dotnet tool update --global dotnet-ef
> dotnet tool update --global dotnet-aspnet-codegenerator
> ```

---

## 🔄 Toujours Commencer Par : Récupérer la Dernière Version du Projet

```bash
git checkout main
git pull origin main
```

---

# 🔵 CHARLE LE GOAT — Partie 2 : AppUser, Identity & Gestion des Rôles

### Branche Git : `feature/identity-roles-auth`

---

### Étape 1 — Créer ta branche Git

```bash
git checkout main
git pull origin main
git checkout -b feature/identity-roles-auth
```

---

### Étape 2 — Créer le Modèle AppUser

Crée le fichier `Models/AppUser.cs` avec ce contenu :

```csharp
using System;
using Microsoft.AspNetCore.Identity;

namespace AfroEvent.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    }
}
```

---

### Étape 3 — Mettre à jour le DbContext pour AppUser

Ouvre `Data/AfroEventDbContext.cs` et change la ligne de déclaration de la classe :

```diff
-public class AfroEventDbContext : IdentityDbContext
+public class AfroEventDbContext : IdentityDbContext<AppUser>
```

Ajoute `using AfroEvent.Models;` en haut du fichier si ce n'est pas déjà fait.

---

### Étape 4 — Configurer Identity dans Program.cs

Dans `Program.cs`, ajoute ces `using` tout en haut :

```csharp
using Microsoft.AspNetCore.Identity;
using AfroEvent.Models;
```

Ensuite, **après** la ligne `AddDbContext` et **avant** `var app = builder.Build();`, ajoute :

```csharp
// --- ASP.NET Core Identity ---
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AfroEventDbContext>()
.AddDefaultTokenProviders();
```

Et dans le pipeline de l'application, ajoute `app.UseAuthentication()` **avant** `app.UseAuthorization()` :

```csharp
app.UseSession();
app.UseAuthentication();  // ← Ajouter cette ligne
app.UseAuthorization();
```

---

### Étape 5 — Scaffold des Pages Identity avec aspnet-codegenerator

Dans le terminal, **à la racine du projet**, exécute :

```bash
dotnet aspnet-codegenerator identity --dbContext AfroEvent.Data.AfroEventDbContext --files "Account.Register;Account.Login;Account.Logout"
```

Cette commande génère automatiquement dans `Areas/Identity/Pages/Account/` :
- `Register.cshtml` — Page d'inscription
- `Login.cshtml` — Page de connexion  
- `Logout.cshtml` — Déconnexion

---

### Étape 6 — Créer le Seeder de Rôles

Crée le fichier `Data/DbSeeder.cs` :

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using AfroEvent.Models;

namespace AfroEvent.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // Création des 3 rôles AfroEvent
            string[] roles = { "Admin", "Organisateur", "Participant" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Création du compte Admin par défaut
            var adminEmail = "admin@afroevent.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "AfroEvent",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
```

---

### Étape 7 — Appeler le Seeder au Démarrage

Dans `Program.cs`, **après** `var app = builder.Build();`, ajoute :

```csharp
// Seeding des rôles et du compte Admin
using (var scope = app.Services.CreateScope())
{
    await AfroEvent.Data.DbSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
}
```

---

### Étape 8 — Sécuriser les Contrôleurs

Ouvre `Controllers/AdminController.cs` et ajoute au-dessus de la classe :

```csharp
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")]
public class AdminController : Controller { ... }
```

Ouvre `Controllers/OrganizerController.cs` et ajoute :

```csharp
[Authorize(Roles = "Organisateur,Admin")]
public class OrganizerController : Controller { ... }
```

---

### Étape 9 — Ajouter le Partial de Login dans la Navbar

Dans `Views/Shared/_Layout.cshtml`, à l'intérieur de la navbar (après les liens nav), ajoute :

```html
<partial name="_LoginPartial" />
```

---

### Étape 10 — Commit & Push

```bash
git add .
git commit -m "feat(identity): AppUser, roles Admin/Organisateur/Participant, scaffold Identity et DbSeeder"
git push origin feature/identity-roles-auth
```

Puis crée une **Pull Request** vers `main` sur GitHub pour que je puisse valider.

---
---

# 🟡 COLLÈGUE 3 — Partie 3 : Ticket Entity, Migrations & BDD SQLite

### Branche Git : `feature/participant-tickets-persistence`

> **Important :** Attends que Charle le GOAT ait mergé sa branche (Partie 2) sur `main` avant de commencer !

---

### Étape 1 — Créer ta branche

```bash
git checkout main
git pull origin main
git checkout -b feature/participant-tickets-persistence
```

---

### Étape 2 — Créer l'Entité Ticket

Crée le fichier `Models/TicketEntity.cs` :

```csharp
using System;

namespace AfroEvent.Models
{
    public class TicketEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string QrCodeHash { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public bool IsPresent { get; set; }
        public DateTime? ScanDate { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        // Clé étrangère vers l'Événement
        public Guid EventId { get; set; }
        public EventEntity? Event { get; set; }

        // Clé étrangère vers le Participant (AppUser)
        public string ParticipantId { get; set; } = string.Empty;
    }
}
```

---

### Étape 3 — Ajouter Ticket dans le DbContext

Ouvre `Data/AfroEventDbContext.cs` et ajoute cette ligne avec les autres `DbSet` :

```csharp
public DbSet<TicketEntity> Tickets { get; set; } = null!;
```

---

### Étape 4 — Créer la Migration EF Core

```bash
dotnet ef migrations add InitialCreate
```

Cette commande crée automatiquement un dossier `Migrations/` avec les fichiers décrivant la structure complète de la base de données.

---

### Étape 5 — Appliquer la Migration (Créer le Fichier .db)

```bash
dotnet ef database update
```

Un fichier `AfroEvent.db` apparaît à la racine du projet. **C'est ta base de données SQLite entière dans un seul fichier !** Aucun serveur à installer.

---

### Étape 6 — Visualiser la BDD

Télécharge **DB Browser for SQLite** ou install l'extension **SQLite Viewer** dans VS Code (gratuit) :  
👉 https://sqlitebrowser.org/dl/

Ouvre `AfroEvent.db` avec DB Browser for SQLite ou l'extension SQLite Viewer dans VS Code et explore les tables générées :
- `AspNetUsers` — Utilisateurs
- `AspNetRoles` — Rôles
- `Categories` — Catégories d'événements
- `Events` — Événements
- `Tickets` — Billets

---

### Étape 7 — Scaffold Automatique d'un Contrôleur CRUD (avec aspnet-codegenerator)

Pour générer automatiquement toutes les pages CRUD pour les Tickets, exécute :

```bash
dotnet aspnet-codegenerator controller \
  -name TicketsController \
  -m AfroEvent.Models.TicketEntity \
  -dc AfroEvent.Data.AfroEventDbContext \
  --relativeFolderPath Controllers \
  --useDefaultLayout \
  --referenceScriptLibraries
```

Cette commande génère automatiquement :
- `Controllers/TicketsController.cs` avec toutes les actions CRUD
- `Views/Tickets/Index.cshtml`, `Create.cshtml`, `Edit.cshtml`, `Details.cshtml`, `Delete.cshtml`

---

### Étape 8 — Commit & Push

```bash
git add .
git commit -m "feat(tickets): Entité TicketEntity, migration InitialCreate, BDD SQLite AfroEvent.db générée"
git push origin feature/participant-tickets-persistence
```

Crée une **Pull Request** vers `main` sur GitHub.

---

## 📋 Aide-Mémoire Commandes EF Core

| Commande | Description |
|---|---|
| `dotnet ef migrations add MonNom` | Crée une nouvelle migration |
| `dotnet ef database update` | Applique les migrations (crée/met à jour la BDD) |
| `dotnet ef migrations list` | Liste toutes les migrations |
| `dotnet ef migrations remove` | Supprime la dernière migration non appliquée |
| `dotnet ef database drop` | Supprime la BDD ⚠️ (dev uniquement !) |

## 📋 Aide-Mémoire Commandes aspnet-codegenerator

| Commande | Description |
|---|---|
| `dotnet aspnet-codegenerator controller -name <Nom> -m <Modèle> -dc <Context> --useDefaultLayout` | Scaffold d'un contrôleur CRUD complet |
| `dotnet aspnet-codegenerator identity -dc <Context> --files "<Pages>"` | Scaffold des pages Identity (Login, Register...) |

---

> 📌 **Questions ou problèmes ?** Contactez **Moi**
