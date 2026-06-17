## Code Architecture Scheme — Tiny Paws, Big Trouble

```txt
Assets/
└── Scripts/
    ├── Core/
    │   ├── GameManager.cs
    │   ├── SceneLoader.cs
    │   └── AudioManager.cs
    │
    ├── UI/
    │   ├── MainMenuController.cs
    │   ├── SettingsPanelController.cs
    │   ├── UIButtonEffects.cs
    │   └── UIAudioBinder.cs
    │
    ├── Player/
    │   ├── PlayerController.cs
    │   ├── PlayerMovement.cs
    │   ├── PlayerAnimationController.cs
    │   └── PlayerState.cs
    │
    ├── Gameplay/
    │   ├── PowerUnlockManager.cs
    │   ├── CombatController.cs
    │   └── CheckpointManager.cs
    │
    ├── NPC/
    │   ├── NPCInteraction.cs
    │   ├── DialogueController.cs
    │   └── QuestChoiceManager.cs
    │
    └── Enemies/
        ├── EnemyBase.cs
        ├── MouseEnemy.cs
        └── BirdEnemy.cs
```

### Current implemented code focus

```txt
MainMenu Scene
│
├── MainMenuController
│   ├── New Game button
│   ├── Continue button
│   ├── Settings button
│   └── Quit button
│
├── SettingsPanelController
│   ├── Music volume slider
│   ├── Sound volume slider
│   ├── Checkbox options
│   └── Close settings panel
│
├── SceneLoader
│   └── Loads future gameplay scene
│
└── AudioManager
    └── Stores and applies audio settings
```

### Future gameplay code flow

```txt
Player input
   ↓
PlayerController
   ↓
PlayerMovement + PlayerAnimationController
   ↓
Collisions / Combat / Interactions
   ↓
PowerUnlockManager
   ↓
New accessible zones
   ↓
NPC choices + QuestChoiceManager
   ↓
Ending logic
```

### Simplified class responsibility

| Script                         | Responsibility                                   |
| ------------------------------ | ------------------------------------------------ |
| `GameManager.cs`               | Stores global game state.                        |
| `SceneLoader.cs`               | Changes scenes: menu, gameplay, endings.         |
| `AudioManager.cs`              | Manages music and sound settings.                |
| `MainMenuController.cs`        | Handles main menu buttons.                       |
| `SettingsPanelController.cs`   | Handles sliders, checkboxes, and settings panel. |
| `PlayerController.cs`          | Central player script.                           |
| `PlayerMovement.cs`            | Movement, jump, collision behavior.              |
| `PlayerAnimationController.cs` | Connects movement state to animations.           |
| `PowerUnlockManager.cs`        | Stores unlocked powers.                          |
| `DialogueController.cs`        | Displays NPC dialogues.                          |
| `QuestChoiceManager.cs`        | Stores important narrative choices.              |
| `EnemyBase.cs`                 | Common enemy behavior.                           |
| `MouseEnemy.cs`                | Mouse-specific behavior.                         |
| `BirdEnemy.cs`                 | Bird-specific behavior.                          |
