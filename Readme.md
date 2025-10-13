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
- Création de l’interface **« Connexion Admin »** avec l'utilisateur `Admin` et mot de passe `Admin`
- Création de l’interface **« Connexion »** avec l'utilisateur `Test` et mot de passe `Test`
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