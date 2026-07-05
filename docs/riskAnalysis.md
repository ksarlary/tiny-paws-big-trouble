# Risk Analysis

| Risque | Probabilité | Impact | Mitigation | Responsable |
|---|---|---|---|---|
| Scope trop ambitieux pour 4–6 semaines | Élevée | Élevé | Limiter le jeu à 2–3 zones, 1 boss, 2 fins et 1–2 pouvoirs maximum. Définir un MVP non négociable. | Tech Lead / Game Designer |
| Incohérence visuelle des assets générés par IA | Élevée | Moyen | Utiliser des prompts de référence, conserver une palette fixe, retoucher manuellement dans GIMP ou Krita et refuser les assets hors style. | Art Lead |
| Arrière-plans faussement transparents ou pixels parasites | Élevée | Moyen | Générer sur fond vert si nécessaire, supprimer le fond manuellement, nettoyer les contours et exporter en PNG avec canal alpha. | Art Lead |
| Animations du personnage peu fluides | Moyenne | Moyen | Utiliser peu de frames mais bien calibrées, tester directement dans Unity et ajuster les timings et transitions de l’Animator. | Gameplay Dev |
| Attaque trop courte ou imprécise | Moyenne | Élevé | Séparer le sprite de slash et la hitbox d’attaque, créer une portée supérieure à la largeur du corps du chat et tester contre différents ennemis. | Gameplay Dev |
| Bugs de collision sur les plateformes | Moyenne | Élevé | Utiliser des Collider2D adaptés, tester les interactions dans une scène dédiée et éviter les formes de collision trop complexes. | Gameplay Dev |
| IA générant du code incorrect ou trop complexe | Élevée | Élevé | Demander des scripts courts et ciblés, relire le code, tester chaque fonctionnalité séparément et simplifier avant intégration. Aucun code généré ne doit être intégré sans validation. | Tech Lead |
| Difficulté d’export WebGL | Moyenne | Moyen | Prioriser la version desktop, lancer un premier test WebGL suffisamment tôt et conserver WebGL comme objectif secondaire tant que le MVP n’est pas stabilisé. | Build Manager |
| Perte de temps sur le level design | Moyenne | Moyen | Concevoir une carte courte avec des chemins bloqués simples. Utiliser Tiled ou LDtk uniquement si leur utilisation accélère réellement le développement. | Level Designer |
| Documentation des prompts incomplète | Moyenne | Élevé | Créer le dossier `/prompts_logs` dès le début du projet et ajouter une entrée après chaque échange significatif avec une IA. | Documentation Owner |
| Risque juridique lié aux assets IA ou externes | Moyenne | Élevé | Préférer des assets originaux générés pour le projet, vérifier les licences des ressources externes et maintenir un fichier `ACKNOWLEDGEMENTS.md`. | Project Manager |
| Ressemblance trop forte avec un jeu commercial existant | Faible à moyenne | Élevé | S’inspirer uniquement de mécaniques générales tout en conservant une identité originale fondée sur le chat, les souris, les oiseaux et l’ambiance cozy du projet. | Game Designer |
| Données personnelles envoyées aux LLMs | Faible | Élevé | Ne jamais inclure de données personnelles ou sensibles dans les prompts et anonymiser les contenus si nécessaire. | Toute l’équipe |
| Dialogue ou narration trop complexe | Moyenne | Moyen | Limiter les dialogues aux scènes importantes : vieux chat, souris rebelle, choix final et fins alternatives. | Narrative Designer |
| Manque de tests avant la soutenance | Moyenne | Élevé | Prévoir une phase dédiée au polish et aux tests, organiser plusieurs playtests courts et conserver un journal des bugs et des corrections. | QA / Tech Lead |
| Sauvegarde liée au bouton Continue non prête | Moyenne | Faible à moyen | Implémenter une sauvegarde simple des informations essentielles ou désactiver le bouton Continue lorsqu’aucune sauvegarde valide n’existe. | Gameplay Dev |
| Audio absent, incompatible ou non licencié | Moyenne | Moyen | Utiliser uniquement des sons libres de droits, créés pour le projet ou générés avec des outils autorisés, puis documenter toutes les sources. | Audio / Documentation |
| Mauvaise organisation Git | Moyenne | Moyen | Effectuer des commits fréquents, travailler avec des branches par fonctionnalité, maintenir le README à jour et respecter la structure du dépôt définie dans le projet. | Tech Lead |
| Performance insuffisante à cause d’assets trop lourds | Faible à moyenne | Moyen | Compresser les sprites et fichiers audio, limiter les résolutions inutiles, éviter l’abus de particules et tester régulièrement les builds. | Build Manager |
| Retard lié à l’apprentissage de Unity | Moyenne | Élevé | Prioriser les fonctionnalités essentielles, utiliser des tutoriels ciblés et l’assistance IA pour des problèmes précis, et éviter les systèmes trop avancés pour le périmètre du projet. | Toute l’équipe |