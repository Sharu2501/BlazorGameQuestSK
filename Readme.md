# BlazorGameQuestSK – LSI1 GRP25  
**Sahkana Sasikumar & Ahmed Kafagy**

## Structure du projet
- **BlazorGame.Client** : Application front-end développée en Blazor WebAssembly.  
- **AuthenticationServices** : API pour gérer l’authentification et les sessions utilisateurs.  
- **SharedModels** : Bibliothèque de classes partagées entre le client et les services.  
- **BlazorGame.Tests** : Projet de tests unitaires utilisant xUnit pour valider les fonctionnalités.

## Version 1 – V1
Pour cette première version, nous avons mis en place les éléments suivants :  

- Mise en place de la **structure globale du projet** (Solution, micro-services, etc.).  
- Création du **projet Client Blazor WebAssembly**.  
- Identification des pages principales du projet (interface utilisateur essentielle).  
- Configuration du **routing** et intégration des composants de base (CSS, JS, etc.).  
- Développement de la **page d’accueil**, du **menu de navigation** et des composants associés.  
- Création de l’interface **« Nouvelle aventure »**.
- Développement d’un **composant Blazor affichant une salle de jeu statique** pour visualiser l’environnement du joueur.

### Tests
- Mise en place du **projet de tests unitaires**.  
- Utilisation de **xUnit** pour l’écriture des tests.  
- Définition et implémentation des **cas de tests pour les fonctionnalités de base**, notamment pour les classes `Player`, `Dungeon`, `Room` et `Monster`.