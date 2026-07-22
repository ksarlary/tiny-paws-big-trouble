## Prompt Log — 22/07 — Konovalova Sofia — S2
- **Contexte** : Création de menu de pause
- **Prompt** :
> we need to implement pause menu when pressing esc. In the pause menu there will be  Resume, Settings and Back to Main Menu buttons, the settings panel will be the same as in the main menu

- **Modèle & outil** : ChatGPT GPT-5.5
- **Réponse / Output** : 
> Yes. The clean way is to make the **Settings panel reusable** between the Main Menu and Pause Menu instead of maintaining two separate copies of the settings logic.

For your project, I recommend this structure:

```text
Room01_Prison
├── Systems
│   └── RoomSaveController
│
├── Environment
├── Gameplay
├── Audio
│
└── UI
    ├── TutorialCanvas
    └── PauseCanvas
        ├── PausePanel
        │   ├── ResumeButton
        │   ├── SettingsButton
        │   └── MainMenuButton
        │
        └── SettingsPanel
            ├── MusicSlider
            ├── SFXSlider
            ├── FullscreenToggle
            └── BackButton
```

The flow will be:

```text
ESC
↓
PausePanel

Resume
→ close pause
→ resume game

Settings
→ hide PausePanel
→ show SettingsPanel

Back from Settings
→ hide SettingsPanel
→ show PausePanel

Back to Main Menu
→ save game
→ restore Time.timeScale
→ load MainMenu
```

Let's build it carefully.

---

# 1. First: turn the Main Menu Settings panel into a Prefab

Because you want the exact same settings panel, don't rebuild it manually.

Open your:

```text
MainMenu
```

scene.

In Hierarchy, find your existing:

```text
SettingsPanel
```

Drag it from the Hierarchy into a folder such as:

```text
Assets/_Project/Prefabs/UI/
```

This creates:

```text
SettingsPanel.prefab
```

Then use that prefab in both:

```text
MainMenu
Room01_Prison
```

This is better because future UI changes affect both menus.

Important: the settings panel should contain the visual controls, but ideally its settings script should work independently of the main menu.

---

# 2. Check your Settings controller

Your existing settings script probably already controls:

```text
Music slider
SFX slider
Fullscreen
```

and works with:

```text
MainAudioMixer
MusicVolume
SFXVolume
```

Keep that existing logic.

The reusable settings object should look approximately like:

```text
SettingsPanel
├── SettingsMenuController
├── MusicSlider
├── SFXSlider
├── FullscreenToggle
└── BackButton
```

We need one additional behavior: the Back button should return differently depending on context.

In Main Menu:

```text
Settings → Back → MainMenu panel
```

In Pause Menu:

```text
Settings → Back → PausePanel
```

We'll handle that from `PauseMenuController`, without rewriting your audio logic.

---

# 3. Create the Pause Canvas

Open:

```text
Room01_Prison
```

Under `UI`, create:

```text
PauseCanvas
```

Configure its Canvas:

```text
Render Mode:
Screen Space - Overlay
```

Canvas Scaler:

```text
UI Scale Mode:
Scale With Screen Size

Reference Resolution:
1920 × 1080

Match:
0.5
```

Set Canvas sorting order higher than the tutorial UI, for example:

```text
Sort Order: 100
```

Your hierarchy:

```text
UI
├── TutorialCanvas
└── PauseCanvas
```

---

# 4. Create a dark overlay

Under `PauseCanvas`:

```text
PauseCanvas
└── PauseBackground
```

Create it with:

```text
Right-click PauseCanvas
→ UI
→ Image
```

Stretch it over the entire screen.

Set color approximately:

```text
Black
Alpha: 0.55–0.70
```

This gives:

```text
game visible behind
+
dark translucent overlay
+
pause menu
```

Later the Settings panel can use the same backdrop.

---

# 5. Create PausePanel

Under `PauseCanvas`:

```text
PauseCanvas
├── PauseBackground
└── PausePanel
```

Create an empty UI object or Panel.

Inside:

```text
PausePanel
├── Title
├── ResumeButton
├── SettingsButton
└── MainMenuButton
```

Suggested text:

```text
PAUSED

Resume
Settings
Back to Main Menu
```

For your game's cozy style, you could use:

```text
Paused

Resume
Settings
Back Home
```

but `Back to Main Menu` is clearer for now.

---

# 6. Add the SettingsPanel prefab

Drag:

```text
SettingsPanel.prefab
```

under `PauseCanvas`:

```text
PauseCanvas
├── PauseBackground
├── PausePanel
└── SettingsPanel
```

Initially set:

```text
PauseCanvas: inactive
PausePanel: active
SettingsPanel: inactive
```

The complete UI:

```text
PauseCanvas
├── PauseBackground
├── PausePanel
│   ├── ResumeButton
│   ├── SettingsButton
│   └── MainMenuButton
│
└── SettingsPanel
    ├── MusicSlider
    ├── SFXSlider
    └── BackButton
```

---

# 7. Create `PauseMenuController.cs`

Create:

```text
Assets/_Project/Scripts/UI/PauseMenuController.cs
```

Use:

```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Save")]
    [SerializeField] private RoomSaveController roomSaveController;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;
    private bool settingsOpen;

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        HandleEscape();
    }

    private void HandleEscape()
    {
        if (!isPaused)
        {
            PauseGame();
            return;
        }

        if (settingsOpen)
        {
            CloseSettings();
            return;
        }

        ResumeGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        settingsOpen = false;

        pauseCanvas.SetActive(true);
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        settingsOpen = false;

        Time.timeScale = 1f;

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }
    }

    public void OpenSettings()
    {
        settingsOpen = true;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsOpen = false;

        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        if (roomSaveController != null)
        {
            roomSaveController.SaveGame();
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
```

---

# 8. Where to attach `PauseMenuController`

I recommend attaching it to:

```text
PauseCanvas
```

or alternatively:

```text
Systems
```

For clarity, I would use:

```text
PauseCanvas
├── Canvas
├── CanvasScaler
├── GraphicRaycaster
└── PauseMenuController
```

Then assign its fields.

---

# 9. Assign the references

Select:

```text
PauseCanvas
```

In `Pause Menu Controller`, assign:

```text
Pause Canvas
→ PauseCanvas

Pause Panel
→ PausePanel

Settings Panel
→ SettingsPanel

Room Save Controller
→ Systems
```

Unity will take the `RoomSaveController` component from `Systems`.

Set:

```text
Main Menu Scene Name:
MainMenu
```

Make sure your actual scene is exactly called:

```text
MainMenu
```

---

# 10. Connect Resume button

Select:

```text
ResumeButton
```

In Button:

```text
On Click()
```

Click `+`.

Drag:

```text
PauseCanvas
```

into the object field.

Choose:

```text
PauseMenuController
→ ResumeGame()
```

---

# 11. Connect Settings button

Select:

```text
SettingsButton
```

In `On Click()`:

```text
PauseCanvas
→ PauseMenuController
→ OpenSettings()
```

---

# 12. Connect Back to Main Menu button

Select:

```text
MainMenuButton
```

Connect:

```text
PauseCanvas
→ PauseMenuController
→ BackToMainMenu()
```

This is important because it performs:

```text
Save current player position
↓
Save tutorial state
↓
Time.timeScale = 1
↓
Load MainMenu
```

So Continue should return the player to the saved state.

---

# 13. Connect Settings Back button

Inside the pause menu's Settings panel, select:

```text
BackButton
```

Connect:

```text
PauseCanvas
→ PauseMenuController
→ CloseSettings()
```

This should produce:

```text
Settings
→ Back
→ PausePanel
```

not:

```text
Settings
→ MainMenu
```

---

# 14. Important problem with your Settings prefab

Your existing Settings panel may have its Back button already wired to a `MainMenuController` method.

For example:

```text
BackButton
→ MainMenuController.CloseSettings()
```

If you instantiate the same prefab in Room 1, that reference may be invalid or unsuitable.

The cleanest approach is:

* prefab contains visuals + sliders + settings logic;
* each scene connects its Back button appropriately.

So:

### Main Menu instance

```text
BackButton
→ MainMenuController.CloseSettings()
```

### Pause Menu instance

```text
BackButton
→ PauseMenuController.CloseSettings()
```

That lets the panel be reused without duplicating the sliders.

---

# 15. Important: settings sliders while game is paused

Because:

```csharp
Time.timeScale = 0f;
```

standard Unity UI input still works.

Your sliders should continue to work because UI interaction doesn't require gameplay time progression.

However, any custom UI animation implemented using:

```csharp
Time.deltaTime
```

will freeze while paused.

For UI fades while paused, use:

```csharp
Time.unscaledDeltaTime
```

instead.

For example:

```csharp
timer += Time.unscaledDeltaTime;
```

This matters only if your Settings panel has animated fades.

---

# 16. ESC behavior

With the provided controller:

### During gameplay

```text
ESC
→ open Pause menu
```

### While Pause menu is open

```text
ESC
→ Resume
```

### While Settings is open

```text
ESC
→ return to Pause menu
```

That is a clean navigation flow:

```text
Gameplay
  ESC ↓ ↑ ESC
Pause
  Settings ↓ ↑ ESC/Back
Settings
```

---

# 17. Prevent gameplay input while paused

Since:

```csharp
Time.timeScale = 0f;
```

your Rigidbody physics stops.

But your `PlayerMovement.Update()` may still process keyboard input because `Update()` continues executing.

Usually physics movement won't actually advance because fixed physics simulation is stopped, but animator parameters or queued jump inputs could behave strangely.

I recommend adding a guard near the beginning of `PlayerMovement.Update()`:

```csharp
private void Update()
{
    if (Time.timeScale == 0f)
    {
        return;
    }

    ReadMovementInput();
    ReadJumpInput();

    UpdateFacingDirection();
    UpdateAnimator();
}
```

That prevents:

```text
press movement while paused
close pause
cat suddenly reacts unexpectedly
```

---

# 18. Prevent landing SFX or animation oddities on pause

The same pause guard helps because your animator update won't keep processing stale conditions while paused.

Alternatively, on Pause:

```text
Animator.speed = 0
```

but for your current setup, `Time.timeScale = 0` already stops normal Animator playback if it's using Normal update mode.

No additional animator code is necessary right now.

---

# 19. Save before leaving to Main Menu

Your `BackToMainMenu()` already calls:

```csharp
roomSaveController.SaveGame();
```

This is better than depending only on:

```csharp
OnApplicationQuit()
```

because returning to Main Menu does not close the application.

The intended flow becomes:

```text
Player at X=22
Tutorials completed
↓
ESC
↓
Back to Main Menu
↓
SaveGame()
↓
MainMenu
↓
Continue
↓
Room01_Prison
↓
restore X=22
↓
tutorials stay completed
```

---

# 20. Important potential problem with your autosave coroutine

Your `RoomSaveController` currently uses:

```csharp
yield return new WaitForSeconds(autosaveInterval);
```

When the game is paused:

```text
Time.timeScale = 0
```

that wait also pauses.

That's actually okay. You don't need autosaving every 2 seconds while the game is paused.

When returning to Main Menu, the explicit:

```csharp
roomSaveController.SaveGame();
```

saves immediately.

So no changes are needed there.

---

# 21. Recommended hierarchy

For your current room:

```text
Room01_Prison
├── Systems
│   └── RoomSaveController component
│
├── Main Camera
├── CM_PlayerCamera
│
├── Environment
├── Gameplay
├── Audio
│
└── UI
    ├── TutorialCanvas
    │   └── TutorialMessage
    │
    └── PauseCanvas
        ├── PauseBackground
        │
        ├── PausePanel
        │   ├── Title
        │   ├── ResumeButton
        │   ├── SettingsButton
        │   └── MainMenuButton
        │
        └── SettingsPanel
            ├── MusicSlider
            ├── SFXSlider
            └── BackButton
```

---

## I would implement it in this order

First create only:

```text
PauseCanvas
PauseBackground
PausePanel
ResumeButton
SettingsButton
MainMenuButton
```

Then implement `PauseMenuController` and verify:

```text
ESC → pause
Resume → unpause
ESC → pause
ESC → unpause
```

After that, bring in the existing `SettingsPanel` prefab and connect its Back button to `CloseSettings()`.

The most important architectural point is: **reuse the existing settings UI/controller and AudioMixer logic; only the panel navigation differs between Main Menu and Pause Menu.**


- **Modifications manuelles** : Aucune
- **Décision d'intégration** : Accepté tel quel

- **Qualité estimée de la réponse** : 5
- **Hallucination détectée** : Aucune