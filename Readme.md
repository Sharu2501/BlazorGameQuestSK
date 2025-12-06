# BlazorGameQuestSK – LSI1 GRP25  
**Sahkana Sasikumar & Ahmed Kafagy**

## Structure du projet
- **BlazorGame.Client** : Application front-end développée en Blazor WebAssembly.  
- **AuthenticationServices** : API pour gérer l’authentification et les sessions utilisateurs.  
- **BlazorGameAPI** : Micro-service principal responsable de la **logique métier du jeu**.  
  Il gère les interactions liées aux **joueurs, donjons, monstres, objets et historiques de parties**.  
  Ce projet intègre **Entity Framework Core** pour la gestion des données et expose plusieurs **endpoints RESTful** documentés via **Swagger**.
- **SharedModels** : Bibliothèque de classes partagées entre le client et les services.  
- **BlazorGame.Tests** : Projet de tests unitaires utilisant xUnit pour valider les fonctionnalités.

## Choix d'architecture 
Nous avons décidé de réaliser ce projet en micro-services car cela permet d'avoir une **décomposition claire des responsabilités** : on a en effet besoin d'API pour la logique du jeu, une authentication et des modèles pour les différentes entités.

D'autre part, nous avons également choisi cette architecture pour son efficacité en matière d'**évolution** : on peut faire évoluer chaque composant et les mettre à jour puis redéployer indépendamment. 

Par rapport à Keycloack, cette architecture nous parait également plus appropriée car cela permet une meilleure **sécurité** avec une authentication centralisée. Les endpoints REST du jeu n'exposent jamais d'informations de sécurité. De plus, grâce à l’utilisation de Swagger, chaque micro-service expose et documente ses endpoints, ce qui facilite l’intégration, la maintenance et les évolutions.

Enfin, cela nous permet d'écrire des **tests unitaires** et d'intégrations tout en assurant une couverture de code élevée en validant séparément.

Cette architecture nous permettra donc dans le futur un déploiement simple via **Docker** puisque chaque service est dans un contenant.

## Version 1 – V1
Pour cette première version, nous avons mis en place les éléments suivants :  

- Mise en place de la **structure globale du projet** (Solution, micro-services, etc.).  
- Création du **projet Client Blazor WebAssembly**.  
- Identification des pages principales du projet (interface utilisateur essentielle).  
- Configuration du **routing** et intégration des composants de base (CSS, JS, etc.).  
- Développement de la **page d’accueil**, du **menu de navigation** et des composants associés.  
- Création de l’interface **« Nouvelle aventure »**.
- Création de l’interface **« Connexion Admin »** avec l'utilisateur `Admin` et mot de passe `Admin`
- Création de l’interface **« Connexion »** avec l'utilisateur `player` et mot de passe `player`
- Développement d’un **composant Blazor affichant une salle de jeu statique** pour visualiser l’environnement du joueur.

### Tests
- Mise en place du **projet de tests unitaires**.  
- Utilisation de **xUnit** pour l’écriture des tests.  
- Définition et implémentation des **cas de tests pour les fonctionnalités de base**, notamment pour les classes `Player`, `Dungeon`, `Room` et `Monster`.

- Concernant les tests futurs, nous souhaitons réaliser les tests suivants : 
1. Tests sur la progression du joueur

- Vérifier que le joueur valide correctement une salle lorsqu’il remplit les conditions néecssaires (par exemple la résolution d’une énigme ou victoire contre un monstre).

- S’assurer que le système met bien à jour l’état de la salle (ouverte, terminée, verrouillée) après la validation.

- Tester la sauvegarde de la progression du joueur (retour dans la salle correcte après reconnexion).

2. Tests sur les scores et les classements

- Vérifier que le score d’un joueur est correctement calculé en fonction des actions réalisées (victoires, objets collectés, temps écoulé).

- Comparer les scores entre plusieurs joueurs pour tester le bon classement dans le tableau des scores.

- Valider la mise à jour du classement après une nouvelle partie.

3. Tests sur le système de quêtes et d’aventures

- S’assurer qu’une nouvelle aventure peut être créée correctement et qu’elle est bien enregistrée.

- Tester le comportement du jeu lorsqu’un joueur interrompt une partie en cours (sauvegarde et reprise).

- Vérifier que les récompenses (points, objets, expérience) sont bien attribuées à la fin d’une aventure.

## Version 2 – V2
Pour cette deuxième version, nous avons mis en place les éléments suivants :

- Redéfinition des **principaux modèles** et de leurs propriétés : `User`, `Admin`, `GameHistory`, `HighScore`, `Monster`, `Artifact`, etc.

- Mise en place de **Entity Framework Core (EF Core)** avec prise en charge d’une **base InMemory** pour les tests.  

- Création des **migrations initiales** et configuration complète du **`ApplicationDbContext`**.  

- Développement des **micro-services et endpoints** correspondants dans l’API **BlazorGameAPI** pour gérer les modèles du jeu.  

- Ajout et configuration de **Swagger** pour la documentation et le test des endpoints.  

- **Tests via l’interface Swagger** pour valider les requêtes et les réponses des services.  

- **Mise à jour des tests unitaires** existants pour prendre en compte les nouvelles fonctionnalités du modèle et des services.  

- **Vérification de la couverture de code** avec Coverlet et ajustements des tests sur les classes principales.

## Version 3 – V3

Dans cette troisième version, nous avons mis en cohérence notre backend et frontend. Nous avons notamment :
- une **génération aléatoire** d'une salle en fonction du level du joueur connecté.
- un **choix de difficulté** dans l'interface graphique.
- un affichage des **stats** du joueur en fonction de ses actions.
- un affichage du **score final** en fin de partie.
- une sauvegarde du **score** et de **l'historique**.
- une réalisation des **jeux de tests** plus poussées.
- une vérification de la **couverture de code** avec Coverlet.

Les données liées à une partie, joueurs et monstres sont stockés dans la base de données **SupaBase**. Pour l'instant, sans keycloak, nous réalisons une vérification pour l'authentication rapide.

Le jeu se déroule sur le port 5000 et les requêtes se font en démarrant le projet BlazorGameAPI.

## Version 4 – V4
Dans cette version, nous avons rapporté les modifications suivantes : 
- La page historique personnel et classement général du joueur dans **Scores**.
- Le **tableau de bord** admin contenant la liste des joueurs (avec action désactivation), la liste des scores, le classement général, la liste des parties et l'export des joueurs accessible avec la **connexion** Admin et mdp Admin.
- On peut visualiser dans **Swagger** la liste des salles, des évènements, la liste des
joueurs, la liste des monstres, les scores, l'historique des parties et les donjons.

Enfin, les interfaces blazor pour cette partie **admin** ont été réalisées.

Nous en avons également profité pour mettre à jour les **tests automatiques** et vérifier la **couverture de code**.

## Version 5 – V5
Dans cette dernière version, nous avons réalisé l'intégration de Keycloak avec notamment :
- une authentification OpenID Connect.
- l'ttribution des rôles joueur et admin.
- la sécurisation des API.
- la modification de la page de connexion dans le projet Blazor pour faire en sorte que seul les utilisateurs dans Keycloak puissent se connecter.
- Et l'accès à toutes les pages du projet web à un utilisateur authentifié uniquement.

D'autre part, nous avons eu l'occasion de réaliser d'autres modifications : 
- l'enrichissement de la documentation Swagger.
- un déploiement sous Docker.
- une configuration de la Gateway comme seul point d’entrée d’appel vers les APIs.
- Un test pour vérifier le bon déroulement d’une partie.

Et enfin la mise à jour des **tests automatiques** et la vérification de la **couverture de code**.

