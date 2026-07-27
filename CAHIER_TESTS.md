# Cahier de Recette & Tests Exhaustifs — AfroEvent
> 🧪 **Protocole de test d'excellence pour l'équipe (3 testeurs)**  
> *Suivez ce cahier de test étape par étape pour valider l'intégrité de la logique, des données et du design de l'application avant sa mise en production commerciale.*

---

## Répartition suggérée des rôles pour les 3 collègues :
* **Testeur A (Charle)** : Administrateur de la plateforme.
* **Testeur B (Moi)** : Organisateur d'événements.
* **Testeur C (Diaby)** : Participant (visiteur/acheteur).

---

## 1. Phase Inscription, Rôles & Sécurité

| ID | Testeur | Catégorie | Action à réaliser | Résultat attendu | Statut |
| :--- | :---: | :---: | :--- | :--- | :---: |
| **SEC-01** | **C** | Sécurité | Accéder à l'URL `/Events/Create` sans être connecté. | Redirection automatique vers la page de connexion (`/Identity/Account/Login`). | `[ ] A tester` |
| **SEC-02** | **C** | Sécurité | Accéder à l'URL `/Participant/MesBillets` sans être connecté. | Redirection automatique vers la page de connexion. | `[ ] A tester` |
| **SEC-03** | **C** | Sécurité | S'inscrire comme Participant simple (ne pas cocher l'interrupteur Organisateur). | Création de compte réussie, redirection vers l'accueil. Pas d'accès au menu "Espace Org" ni "Admin". | `[ ] A tester` |
| **ORG-01** | **B** | Inscription Org | S'inscrire en cochant "Je souhaite organiser des événements". Renseigner "Mon Organisation SAS". | Compte créé. Notification de mise en attente affichée. Pas d'accès direct à l'Espace Org à la connexion. | `[ ] A tester` |
| **ADM-01** | **A** | Admin | Se connecter avec le compte Administrateur et ouvrir le panneau **Admin**. | Les statistiques globales s'affichent à 100% avec les données réelles de la base de données. | `[ ] A tester` |
| **ADM-02** | **A** | Admin | Aller sur l'onglet **Organisateurs** dans le dashboard Admin. Trouver la demande de **Testeur B (B)**. | La demande de B s'affiche avec le statut "En attente" et le nom de son organisation. | `[ ] A tester` |
| **ADM-03** | **A** | Admin | Cliquer sur le bouton **Approuver** de la demande de B. | Une boîte de dialogue demande confirmation. Après validation, le compte de B est approuvé et disparaît de la liste des demandes. | `[ ] A tester` |
| **ORG-02** | **B** | Espace Org | Se reconnecter sur le compte de B (Organisateur approuvé). | Le menu dropdown **Espace Org.** apparaît dans la barre de navigation. L'accès à son dashboard est autorisé. | `[ ] A tester` |
| **ADM-04** | **A** | Robustesse | Créer une nouvelle demande d'organisateur fictive, puis cliquer sur **Rejeter**. | Boîte de confirmation affichée. Après clic, le compte est définitivement supprimé en BDD pour libérer l'e-mail. | `[ ] A tester` |

---

## 2. Phase Création & Gestion d'Événements (Rôle Organisateur)

| ID | Testeur | Catégorie | Action à réaliser | Résultat attendu | Statut |
| :--- | :---: | :---: | :--- | :--- | :---: |
| **EVT-01** | **B** | Création | Aller dans Espace Org → **Créer un événement**. Entrer un titre, prix à `5000` FCFA, capacité à `10` et valider. | Événement créé et visible dans le catalogue. Redirection vers son dashboard d'organisateur. | `[ ] A tester` |
| **EVT-02** | **B** | Création | Créer un deuxième événement mais avec un prix à `0` FCFA (Événement Gratuit) et valider. | Événement créé avec le badge vert **Gratuit** bien mis en valeur dans le catalogue. | `[ ] A tester` |
| **EVT-03** | **B** | Statistiques | Ouvrir le dashboard de l'Organisateur B. | Les graphiques (Chart.js) s'adaptent et affichent les 2 événements réels dans les statistiques. Pas de données fictives. | `[ ] A tester` |
| **EVT-04** | **A** | Admin | Se connecter en Admin et aller dans le catalogue ou essayer d'ouvrir `/Events/Create`. | Aucun bouton de création n'apparaît pour l'Admin. L'URL directe renvoie une erreur 403 (Interdit). | `[ ] A tester` |
| **EVT-05** | **B** | Isolation | Se connecter en tant qu'organisateur B et tenter d'éditer un événement créé par un autre organisateur via l'URL. | L'accès est bloqué ou redirigé pour préserver l'isolation des données entre organisateurs. | `[ ] A tester` |

---

## 3. Phase Inscriptions & Billetterie (Rôle Participant)

| ID | Testeur | Catégorie | Action à réaliser | Résultat attendu | Statut |
| :--- | :---: | :---: | :--- | :--- | :---: |
| **BIL-01** | **C** | Flux Payant | Se connecter en Participant (C), aller sur l'événement payant (5000 FCFA) et s'inscrire. | Le formulaire s'affiche pré-rempli avec les données du compte de C. Choix du pass. | `[ ] A tester` |
| **BIL-02** | **C** | Flux Payant | Valider le formulaire d'inscription. | Redirection vers la page de confirmation de paiement (avec récapitulatif détaillé de l'événement et du prix). | `[ ] A tester` |
| **BIL-03** | **C** | Paiement | Cliquer sur "Confirmer et Payer". | Simulation de paiement réussie. Affichage de la page du e-billet avec le QR Code SVG dessiné en noir et blanc. | `[ ] A tester` |
| **BIL-04** | **C** | Flux Gratuit | S'inscrire à l'événement gratuit (0 FCFA) créé par B. | **Flux court** : Court-circuite la page de confirmation de paiement. Le e-billet est généré et affiché directement. | `[ ] A tester` |
| **BIL-05** | **C** | Espace Billets | Cliquer sur **Mes billets** dans le menu de navigation. | Affiche la liste des billets achetés par C sous forme de cartes contrastées. Aucun texte blanc sur fond clair. | `[ ] A tester` |
| **BIL-06** | **C** | Consultation | Sur l'une des cartes de billet, cliquer sur **Voir QR Code**. | Le billet s'ouvre à nouveau en grand avec toutes ses données et son QR Code valide. | `[ ] A tester` |
| **BIL-07** | **C** | Téléchargement | Cliquer sur l'icône de l'imprimante pour télécharger le billet. | Télécharge un fichier HTML premium. En l'ouvrant, le billet est propre, stylisé et propose d'imprimer en PDF d'un clic. | `[ ] A tester` |

---

## 4. Phase Scan & Présence (Temps Réel SignalR)

| ID | Testeur | Catégorie | Action à réaliser | Résultat attendu | Statut |
| :--- | :---: | :---: | :--- | :--- | :---: |
| **RT-01** | **B & C** | Temps Réel | Garder le dashboard de l'organisateur B ouvert dans un onglet. Dans un autre onglet, inscrire le participant C. | À la validation de l'inscription de C, une notification SignalR s'affiche instantanément sur le dashboard de B. | `[ ] A tester` |
| **RT-02** | **B** | Présence | Aller sur la liste des participants de l'événement de B. Trouver C. | C est marqué comme "Non présent" (ou Absent). | `[ ] A tester` |
| **RT-03** | **B** | Validation | Cliquer sur le bouton **Check-in** (Valider la présence) de C. | Le statut de C passe instantanément à "Présent". La date et l'heure du scan sont enregistrées en base. | `[ ] A tester` |
| **RT-04** | **B** | Statistiques | Retourner sur le dashboard de l'organisateur B. | Le taux de présence global et le compteur de présences validées ont augmenté en direct. | `[ ] A tester` |

---

## 5. Tests aux Limites & Injections Logiques (Robustesse)

| ID | Testeur | Catégorie | Action à réaliser | Résultat attendu | Statut |
| :--- | :---: | :---: | :--- | :--- | :---: |
| **LIM-01** | **C** | Limite | Inscrire successivement 10 participants (ou simuler) sur l'événement créé avec une capacité de 10 places. | L'événement est affiché comme complet. Le bouton "S'inscrire" est désactivé ou masqué. | `[ ] A tester` |
| **LIM-02** | **C** | Robustesse | Tenter de forcer l'accès à l'inscription à l'événement complet via l'URL d'inscription directe. | Le système bloque l'inscription, affiche un message d'erreur "Capacité maximale atteinte" et redirige. | `[ ] A tester` |
| **LIM-03** | **C** | Robuste URL | Saisir un ID d'événement inexistant dans l'URL (ex: `/Participant/SInscrire?eventId=00000000-0000-0000-0000-000000000000`). | Redirection propre vers la liste des événements avec un message d'erreur convivial "Événement introuvable". | `[ ] A tester` |
| **LIM-04** | **B** | Dates | Créer un événement dont la date de fin est antérieure à la date de début. | La validation de formulaire échoue avec un message d'erreur clair sur la cohérence des dates. | `[ ] A tester` |
| **LIM-05** | **B** | Prix | Créer un événement avec un prix négatif (ex: `-1500` FCFA). | La validation échoue, le prix doit être supérieur ou égal à 0. | `[ ] A tester` |
