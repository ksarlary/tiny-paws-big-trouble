## Prompt Log — 22/07 — Konovalova Sofia — S3
- **Contexte** : Générer le dossier de tests avec un rapport de bugs
- **Prompt** :
> write a "dossier de tests" that includes "rapports de bugs" in french based on this playtest experience:
Main menu is functional, all buttons function as intended. New game button deletes  the existing save while Continue button loads the save. Settings button open settings panel with two sound sliders and fullscreen checkbox. Quit button quits the game. In the room 1 players sees the movements buttons tutorial. Camera is bound to the room limits and follow the player. Tutorial messages are only displayed once per playthrough. Player spawn spot is fixed. Collisions are working fine. Scene transitions are working as intended. In the room 2 there is a collectible that can be picked up only once. In the room 3 there is a checkpoint that restores HP and 2 enemies. One of them is supposed to drop a key item that unlocks the door to the Room 4. The bug where the player dropped the key and died without picking it up is fixed. During this bug the player were locked off the room 4 since he had no key. In the room 4 there is an NPC that teaches the player to double jump. There is a bug with the double jump: the second jump can only be used while the character is moving up. In the secret room 5 there is an NPC which responses are provided by AI via API. There is a fallback in case of missing internet connection. However, there is a bug about it: if the connection is lost during the runtime, the NPC wont provide any fallback response. In all scenes there is a bug: the app is not responsive, so changing the window size reveals unused part of the scene. I tested all movements mechanics, all combat mechanics including checkpoint revive mechanic, all NPC interactions and save mechanics. All settings


- **Modèle & outil** : ChatGPT GPT-5.5
- **Réponse / Output** : 
> ```md
# Dossier de tests — Tiny Paws, Big Trouble

## 1. Objectif du dossier

Ce dossier présente les tests réalisés sur le prototype jouable de **Tiny Paws, Big Trouble**, un jeu de plateforme 2D de type Metroidvania léger développé sous Unity.

L’objectif des tests est de vérifier que les fonctionnalités principales du jeu sont opérationnelles, que la progression du joueur fonctionne correctement, que les mécaniques essentielles sont stables, et d’identifier les bugs restants avant livraison.

Les tests couvrent principalement :

- le menu principal ;
- les paramètres du jeu ;
- le système de sauvegarde ;
- les transitions entre scènes ;
- les contrôles du joueur ;
- les tutoriels ;
- les collisions ;
- les collectibles ;
- les checkpoints ;
- les ennemis ;
- les objets-clés ;
- les PNJ ;
- la mécanique de double saut ;
- le PNJ IA de la salle secrète ;
- l’affichage des scènes selon la taille de la fenêtre.

---

## 2. Périmètre des tests

Les tests ont été réalisés sur les scènes actuellement disponibles du prototype :

| Scène | Fonctionnalités testées |
|---|---|
| Main Menu | Boutons, paramètres, sauvegarde, lancement de partie |
| Room 1 | Déplacement, tutoriel, caméra, collisions, transition |
| Room 2 | Collectible récupérable une seule fois |
| Room 3 | Checkpoint, restauration des PV, ennemis, drop d’objet-clé, porte verrouillée |
| Room 4 | PNJ donnant la capacité de double saut |
| Room 5 Secret Room | PNJ avec réponses générées par IA via API, fallback hors connexion |
| Toutes les scènes | Redimensionnement de fenêtre, responsive, limites visibles |

---

## 3. Environnement de test

| Élément | Description |
|---|---|
| Moteur | Unity 2D |
| Plateforme testée | Build local / éditeur Unity |
| Type de jeu | Platformer 2D / Metroidvania léger |
| Contrôles | Clavier |
| Sauvegarde | Sauvegarde locale |
| Connexion internet | Testée avec connexion active et avec perte de connexion |
| Version testée | Prototype actuel |

---

## 4. Résumé global des résultats

Le prototype est globalement fonctionnel. Le menu principal, les déplacements, les collisions, les transitions de scène, les checkpoints, les interactions PNJ, le système de sauvegarde et les mécaniques de combat principales fonctionnent comme prévu.

Plusieurs bugs critiques ont été corrigés pendant la phase de test, notamment le problème lié à l’objet-clé de la Room 3. Ce bug pouvait bloquer définitivement la progression si le joueur mourait après le drop de la clé mais avant de la récupérer.

Trois bugs restent identifiés :

1. le double saut ne peut être utilisé que lorsque le personnage est encore en montée ;
2. le PNJ IA de la salle secrète ne déclenche pas de réponse fallback si la connexion est perdue pendant l’exécution du jeu ;
3. le jeu n’est pas responsive : changer la taille de la fenêtre révèle des zones inutilisées de la scène.

---

# 5. Plan de tests fonctionnels

## 5.1 Tests du menu principal

| ID | Fonctionnalité | Étapes de test | Résultat attendu | Résultat obtenu | Statut |
|---|---|---|---|---|---|
| T-MENU-01 | Bouton New Game | Cliquer sur `New Game` depuis le menu principal | Une nouvelle partie démarre et l’ancienne sauvegarde est supprimée | Le bouton lance bien une nouvelle partie et supprime la sauvegarde existante | Validé |
| T-MENU-02 | Bouton Continue | Créer une sauvegarde, revenir au menu, cliquer sur `Continue` | La partie reprend depuis la sauvegarde existante | La sauvegarde est correctement chargée | Validé |
| T-MENU-03 | Bouton Settings | Cliquer sur `Settings` | Le panneau de paramètres s’ouvre | Le panneau s’ouvre correctement | Validé |
| T-MENU-04 | Sliders audio | Modifier les deux sliders audio | Le volume correspondant change | Les sliders sont présents et fonctionnels | Validé |
| T-MENU-05 | Checkbox fullscreen | Activer/désactiver l’option fullscreen | Le mode plein écran change selon l’état de la checkbox | L’option est disponible et fonctionne | Validé |
| T-MENU-06 | Bouton Quit | Cliquer sur `Quit` | L’application se ferme | Le bouton quitte correctement le jeu | Validé |

---

## 5.2 Tests de la Room 1

| ID | Fonctionnalité | Étapes de test | Résultat attendu | Résultat obtenu | Statut |
|---|---|---|---|---|---|
| T-R1-01 | Spawn du joueur | Lancer une nouvelle partie | Le joueur apparaît à l’emplacement prévu | Le point de spawn est correct et stable | Validé |
| T-R1-02 | Tutoriel de mouvement | Entrer dans la Room 1 | Les messages de tutoriel des contrôles apparaissent | Les messages apparaissent correctement | Validé |
| T-R1-03 | Tutoriel affiché une seule fois | Lire le tutoriel, quitter/revenir ou continuer la partie | Les messages ne doivent pas réapparaître dans le même playthrough | Les messages sont affichés une seule fois | Validé |
| T-R1-04 | Caméra | Se déplacer dans la Room 1 | La caméra suit le joueur sans dépasser les limites de la salle | La caméra suit correctement le joueur et reste bornée | Validé |
| T-R1-05 | Collisions | Marcher, sauter et toucher les plateformes/murs | Le joueur ne traverse pas les éléments solides | Les collisions fonctionnent correctement | Validé |
| T-R1-06 | Transition de scène | Atteindre la sortie vers la salle suivante | La scène suivante se charge correctement | La transition fonctionne comme prévu | Validé |

---

## 5.3 Tests de la Room 2

| ID | Fonctionnalité | Étapes de test | Résultat attendu | Résultat obtenu | Statut |
|---|---|---|---|---|---|
| T-R2-01 | Collectible | Entrer dans la Room 2 et récupérer le collectible | Le collectible est ajouté/récupéré | Le collectible peut être ramassé correctement | Validé |
| T-R2-02 | Collectible unique | Récupérer le collectible, quitter la salle puis revenir | Le collectible ne doit pas réapparaître | Le collectible est bien récupérable une seule fois | Validé |
| T-R2-03 | Transition de scène | Passer vers la salle suivante | La transition s’effectue correctement | La transition fonctionne | Validé |

---

## 5.4 Tests de la Room 3

| ID | Fonctionnalité | Étapes de test | Résultat attendu | Résultat obtenu | Statut |
|---|---|---|---|---|---|
| T-R3-01 | Checkpoint | Atteindre le checkpoint | Le checkpoint est activé | Le checkpoint fonctionne | Validé |
| T-R3-02 | Restauration des PV | Interagir avec le checkpoint ou mourir après activation | Les PV sont restaurés | Les PV sont correctement restaurés | Validé |
| T-R3-03 | Ennemis | Combattre les deux ennemis présents dans la salle | Les ennemis réagissent au combat et peuvent être vaincus | Les mécaniques de combat fonctionnent | Validé |
| T-R3-04 | Drop de l’objet-clé | Vaincre l’ennemi censé laisser tomber la clé | L’objet-clé apparaît après la mort de l’ennemi | L’objet-clé apparaît correctement | Validé |
| T-R3-05 | Ouverture de la porte vers Room 4 | Récupérer la clé puis atteindre la porte | La porte vers la Room 4 se débloque | La porte se débloque correctement avec la clé | Validé |
| T-R3-06 | Mort après drop de clé non récupérée | Vaincre l’ennemi, faire apparaître la clé, mourir avant de la ramasser | Le joueur ne doit pas être bloqué définitivement | Bug corrigé : le joueur n’est plus softlock | Corrigé |

---

## 5.5 Tests de la Room 4

| ID | Fonctionnalité | Étapes de test | Résultat attendu | Résultat obtenu | Statut |
|---|---|---|---|---|---|
| T-R4-01 | Interaction PNJ | Entrer dans la Room 4 et interagir avec le PNJ | Le dialogue du PNJ se déclenche | L’interaction fonctionne | Validé |
| T-R4-02 | Déblocage du double saut | Terminer le dialogue ou l’interaction prévue | Le joueur obtient la capacité de double saut | Le double saut est bien débloqué | Validé |
| T-R4-03 | Utilisation du double saut | Sauter puis appuyer une deuxième fois sur la touche de saut | Le joueur doit pouvoir effectuer un second saut en l’air | Le double saut fonctionne uniquement quand le personnage est encore en montée | Bug ouvert |

---

## 5.6 Tests de la Room 5 — Secret Room

| ID | Fonctionnalité | Étapes de test | Résultat attendu | Résultat obtenu | Statut |
|---|---|---|---|---|---|
| T-R5-01 | Interaction avec le PNJ secret | Entrer dans la salle secrète et interagir avec le PNJ | Le PNJ répond au joueur | L’interaction fonctionne | Validé |
| T-R5-02 | Réponses IA via API | Interagir avec le PNJ avec une connexion active | Le PNJ fournit une réponse générée par IA | Les réponses IA fonctionnent via API | Validé |
| T-R5-03 | Fallback sans connexion au lancement | Lancer le jeu sans connexion et interagir avec le PNJ | Le PNJ fournit une réponse fallback | Le fallback est prévu et fonctionne dans le cas d’une absence de connexion initiale | Validé |
| T-R5-04 | Perte de connexion pendant le runtime | Lancer le jeu avec connexion, couper la connexion pendant l’exécution, interagir avec le PNJ | Le PNJ doit fournir une réponse fallback | Le PNJ ne fournit pas de réponse fallback si la connexion est perdue pendant le runtime | Bug ouvert |

---

## 5.7 Tests globaux toutes scènes

| ID | Fonctionnalité | Étapes de test | Résultat attendu | Résultat obtenu | Statut |
|---|---|---|---|---|---|
| T-GLOB-01 | Déplacement | Tester déplacement gauche/droite dans toutes les scènes jouables | Le joueur se déplace correctement | Le déplacement fonctionne | Validé |
| T-GLOB-02 | Saut | Tester le saut dans toutes les zones de gameplay | Le saut répond correctement | Le saut fonctionne | Validé |
| T-GLOB-03 | Combat | Attaquer les ennemis avec la mécanique prévue | Les ennemis reçoivent les dégâts | Le combat fonctionne | Validé |
| T-GLOB-04 | Mort et réapparition | Mourir après avoir activé un checkpoint | Le joueur réapparaît au checkpoint avec les PV restaurés | La mécanique de revive fonctionne | Validé |
| T-GLOB-05 | Interactions PNJ | Interagir avec tous les PNJ disponibles | Les dialogues ou réponses se déclenchent | Les interactions fonctionnent | Validé |
| T-GLOB-06 | Sauvegarde | Sauvegarder, quitter, utiliser Continue | Le jeu reprend depuis la sauvegarde | Le système de sauvegarde fonctionne | Validé |
| T-GLOB-07 | Paramètres | Modifier les réglages audio et fullscreen | Les paramètres sont appliqués correctement | Les paramètres fonctionnent | Validé |
| T-GLOB-08 | Redimensionnement de fenêtre | Changer la taille de la fenêtre pendant le jeu | L’affichage doit rester propre et ne pas révéler de zones inutilisées | Le changement de taille révèle des parties inutilisées de la scène | Bug ouvert |

---

# 6. Rapports de bugs

## BUG-001 — Softlock après mort sans récupération de l’objet-clé

| Champ | Description |
|---|---|
| ID | BUG-001 |
| Titre | Le joueur pouvait être bloqué s’il mourait après le drop de la clé sans la récupérer |
| Scène concernée | Room 3 |
| Sévérité | Critique |
| Priorité | Haute |
| Statut | Corrigé |
| Description | Dans la Room 3, un ennemi laisse tomber un objet-clé permettant d’ouvrir la porte vers la Room 4. Avant correction, si le joueur tuait cet ennemi, faisait apparaître la clé, puis mourait sans la ramasser, il pouvait revenir sans posséder la clé et sans possibilité de la récupérer à nouveau. |
| Étapes de reproduction | 1. Entrer dans la Room 3. 2. Vaincre l’ennemi qui drop la clé. 3. Ne pas récupérer la clé. 4. Mourir. 5. Réapparaître au checkpoint. 6. Tenter d’accéder à la Room 4. |
| Résultat attendu | Le joueur doit pouvoir récupérer la clé après sa mort ou ne jamais perdre définitivement l’accès à l’objet-clé. |
| Résultat obtenu avant correction | Le joueur n’avait pas la clé et ne pouvait plus ouvrir la porte vers la Room 4. |
| Impact | Blocage complet de la progression principale. Le joueur devait recommencer la partie ou charger une sauvegarde antérieure. |
| Correction appliquée | Le comportement de l’objet-clé a été corrigé afin d’éviter le softlock. La progression vers la Room 4 n’est plus définitivement bloquée si le joueur meurt avant de ramasser la clé. |
| Test de non-régression | Répéter le scénario de mort après drop de la clé et vérifier que le joueur peut toujours obtenir l’objet-clé ou accéder correctement à la suite. |
| Responsable | Gameplay Dev |

---

## BUG-002 — Double saut utilisable uniquement pendant la montée

| Champ | Description |
|---|---|
| ID | BUG-002 |
| Titre | Le deuxième saut ne fonctionne que lorsque le personnage est encore en phase ascendante |
| Scène concernée | Room 4 et scènes suivantes |
| Sévérité | Majeure |
| Priorité | Haute |
| Statut | Ouvert |
| Description | Après l’obtention du double saut dans la Room 4, le joueur devrait pouvoir utiliser le second saut tant qu’il est en l’air et qu’il n’a pas encore consommé cette capacité. Actuellement, le deuxième saut ne fonctionne que lorsque le personnage est encore en train de monter après le premier saut. Si le joueur commence à tomber, le double saut ne se déclenche plus. |
| Étapes de reproduction | 1. Entrer dans la Room 4. 2. Interagir avec le PNJ pour obtenir le double saut. 3. Effectuer un premier saut. 4. Attendre que le personnage commence à descendre. 5. Appuyer une deuxième fois sur la touche de saut. |
| Résultat attendu | Le joueur doit pouvoir effectuer un double saut même pendant la phase descendante, tant qu’il est en l’air et que le double saut n’a pas encore été utilisé. |
| Résultat obtenu | Le double saut ne se déclenche pas lorsque le personnage est en chute. |
| Impact | La capacité est moins intuitive et peut empêcher certains passages prévus pour être franchis avec le double saut. |
| Cause probable | La condition de déclenchement du double saut dépend probablement de la vélocité verticale positive ou d’un état de saut trop restrictif. |
| Recommandation de correction | Modifier la logique pour autoriser le double saut lorsque le joueur n’est pas au sol, possède la capacité et n’a pas encore consommé son second saut, indépendamment de la direction verticale actuelle. |
| Test de validation après correction | Tester le double saut pendant la montée, au sommet du saut et pendant la chute. Le second saut doit fonctionner dans les trois cas si la capacité est disponible. |
| Responsable | Gameplay Dev |

---

## BUG-003 — Absence de fallback si la connexion est perdue pendant le runtime

| Champ | Description |
|---|---|
| ID | BUG-003 |
| Titre | Le PNJ IA ne fournit pas de réponse fallback si la connexion est coupée pendant l’exécution |
| Scène concernée | Room 5 — Secret Room |
| Sévérité | Majeure |
| Priorité | Moyenne à haute |
| Statut | Ouvert |
| Description | Le PNJ de la salle secrète utilise une API IA pour générer ses réponses. Un fallback existe en cas d’absence de connexion internet. Cependant, si le jeu est lancé avec une connexion active puis que la connexion est perdue pendant le runtime, le PNJ ne fournit pas de réponse fallback. |
| Étapes de reproduction | 1. Lancer le jeu avec une connexion internet active. 2. Entrer dans la Room 5. 3. Couper la connexion internet pendant que le jeu est lancé. 4. Interagir avec le PNJ IA. |
| Résultat attendu | Le PNJ doit détecter l’échec de l’appel API et afficher une réponse fallback locale. |
| Résultat obtenu | Le PNJ ne fournit pas de réponse fallback après perte de connexion pendant le runtime. |
| Impact | L’interaction avec le PNJ peut sembler cassée ou vide. Cela nuit à l’expérience utilisateur et à la robustesse du prototype. |
| Cause probable | Le fallback semble seulement prévu au lancement ou avant l’appel API, mais pas dans le bloc de gestion d’erreur de la requête runtime. |
| Recommandation de correction | Ajouter une gestion d’exception ou de timeout autour de chaque appel API. En cas d’échec, d’absence de réponse, de timeout ou d’erreur réseau, afficher immédiatement une réponse fallback locale. |
| Test de validation après correction | Couper la connexion pendant le jeu, interagir avec le PNJ et vérifier qu’un dialogue fallback apparaît sans bloquer l’interface. |
| Responsable | Gameplay Dev / AI Integration Dev |

---

## BUG-004 — Affichage non responsive lors du redimensionnement de la fenêtre

| Champ | Description |
|---|---|
| ID | BUG-004 |
| Titre | Le redimensionnement de la fenêtre révèle des zones inutilisées de la scène |
| Scène concernée | Toutes les scènes |
| Sévérité | Moyenne |
| Priorité | Moyenne |
| Statut | Ouvert |
| Description | Lorsque la taille de la fenêtre est modifiée, le jeu ne s’adapte pas correctement. Des parties inutilisées ou non décorées de la scène deviennent visibles. |
| Étapes de reproduction | 1. Lancer le jeu. 2. Entrer dans n’importe quelle scène. 3. Modifier la taille de la fenêtre. 4. Observer les bords de l’écran et les zones hors décor. |
| Résultat attendu | Le jeu doit conserver un cadrage propre, sans révéler de zones inutilisées, quelle que soit la taille de la fenêtre. |
| Résultat obtenu | Le changement de taille de fenêtre révèle des zones vides ou non prévues dans la scène. |
| Impact | Problème visuel important, surtout pour un build desktop ou WebGL. Peut donner une impression de manque de finition. |
| Cause probable | La caméra ou le Canvas ne gère pas correctement les ratios d’écran variables. Les limites de caméra sont adaptées à une résolution fixe mais pas aux changements de ratio. |
| Recommandation de correction | Fixer un ratio cible, ajouter des bandes noires si nécessaire, ajuster les limites de caméra selon l’orthographic size, et vérifier le Canvas Scaler pour l’UI. |
| Test de validation après correction | Tester plusieurs formats de fenêtre : 16:9, 16:10, 4:3, fenêtre réduite, plein écran. Aucune zone inutilisée ne doit être visible. |
| Responsable | UI/UX Dev / Build Manager |

---

# 7. Tests de non-régression recommandés

Après correction des bugs ouverts, les tests suivants doivent être rejoués :

| ID | Bug lié | Test à rejouer | Critère de validation |
|---|---|---|---|
| NR-001 | BUG-002 | Tester le double saut pendant la montée, au sommet du saut et pendant la chute | Le second saut fonctionne dans toutes les phases aériennes |
| NR-002 | BUG-003 | Couper la connexion pendant le runtime puis interagir avec le PNJ IA | Une réponse fallback locale est affichée |
| NR-003 | BUG-004 | Redimensionner la fenêtre dans toutes les scènes | Aucune zone inutilisée de la scène n’est visible |
| NR-004 | BUG-001 | Mourir après le drop de la clé sans la ramasser | Le joueur n’est pas bloqué et peut toujours progresser |

---

# 8. Conclusion des tests

Les tests réalisés montrent que le prototype de **Tiny Paws, Big Trouble** est fonctionnel sur ses mécaniques principales. Le menu principal, les paramètres, la sauvegarde, les déplacements, les collisions, les transitions de scène, les collectibles, les checkpoints, les combats, les interactions PNJ et la progression par objet-clé sont opérationnels.

Le bug critique lié à l’objet-clé de la Room 3 a été corrigé, ce qui sécurise la progression principale vers la Room 4.

Trois problèmes restent à traiter avant une version de livraison plus stable :

1. corriger la logique du double saut ;
2. renforcer le fallback du PNJ IA en cas de perte de connexion pendant l’exécution ;
3. améliorer le comportement responsive de la caméra et de l’affichage.

Une fois ces corrections appliquées, le prototype devrait répondre correctement aux exigences principales d’un mini-jeu Metroidvania étudiant : personnage contrôlable, pouvoir débloquable, PNJ, chemin de progression, sauvegarde, interactions et boucle de gameplay testable.
```


- **Modifications manuelles** : Aucune
- **Décision d'intégration** : Accepté tel quel

- **Qualité estimée de la réponse** : 5
- **Hallucination détectée** :