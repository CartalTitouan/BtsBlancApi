# BtsBlancApi

Hello Sofiane, voici mon API que tu nous as demandez pour le BTS blanc.
Bien évidement j'ai limité à fond l'utilisation de l'IA comme tu nous l'a demandez ;) .
Tu pourra donc retrouvé une API de type gestion d'évenement. Enjoy

## Stack technique

### Back-end
ASP.NET Core 6 (Web API), Entity Framework Core 6 (Code First, migrations), SQL Server LocalDB (localdb)\MSSQLLocalDB, Authentification JWT (Bearer Token), BCrypt pour le hashage des mots de passe

### Front-end
HTML / CSS / JS classique (servi via wwwroot), Fetch API, localStorage, décodage JWT avec atob()

## Fonctionnalités
# Authentification
- Inscription et connexion utilisateur
- Token JWT retourné à la connexion, stocké en localStorage
- Système de rôles : User (défaut) et Admin
- Promotion d'un utilisateur en admin via l'endpoint /api/admin/promouvoir/{userId}

### Événements
- Création réservée aux admins
- Chaque événement possède un titre, un contenu, une date, un lieu et un type parmi : Salon professionnel, Conférence, Journées thématiques
- Consultation et suppression disponibles

### Inscriptions
- Un utilisateur connecté peut s'inscrire / se désinscrire d'un événement
- Inscription unique par utilisateur et par événement (contrainte BDD + vérification applicative)

### Endpoints principaux

| Méthode | Route | Accès |
|---------|--------|--------|
| POST | /api/auth/register | Public |
| POST | /api/auth/login | Public |
| GET | /api/evenement | Authentifié |
| POST | /api/evenement | Admin |
| DELETE | /api/evenement/{id} | Admin |
| POST | /api/inscription/{evenementId} | Authentifié |
| DELETE | /api/inscription/{evenementId} | Authentifié |
| GET | /api/inscription/evenement/{evenementId} | Authentifié |
| POST | /api/admin/promouvoir/{userId} | Admin |
| GET | /api/admin/users | Admin |

- Front accessible sur https://localhost:7056/index.html
- Documentation Swagger sur https://localhost:7056/swagger
