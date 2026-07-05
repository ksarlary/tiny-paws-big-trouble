## Prompt Log — 05/07/2026 — Konovalova Sofia — S1
- **Contexte** : Implémenter un mechanisme de sauvegarde (améliorer l'existant pour sauvegarder la position de joueur et les tutorial triggers)
- **Prompt** :
> Act like an experienced game developer, write a step by step detailed guide on how to upgrade the previously implemented save feature to include as saved data the player's position and the state (already triggered or not) of the tutorial triggers

- **Modèle & outil** : ChatGPT GPT-5.5
- **Réponse / Output** : 
> Yes. For your project, I would now replace the lightweight `PlayerPrefs`-style save approach with a real JSON save file.

For the current state of *Tiny Paws, Big Trouble*, the save should contain at least:

```text
Current scene
Player position
Player velocity
Facing direction
Whether intro was seen
Tutorial trigger states
```

Later, the same system can grow to save:

```text
Unlocked abilities
Collected items
Opened doors
Defeated bosses
Moved crates
NPC states
Checkpoints
```

For now, let’s build the base properly.

---

# Final save flow

We want:

```text
New Game
↓
delete old save
↓
play IntroCutscene
↓
load Room01_Prison
↓
create initial save
```

Then during gameplay:

```text
Save
↓
store Room01_Prison
store exact player position
store velocity
store facing direction
store tutorial state
```

And Continue:

```text
Continue
↓
read save.json
↓
load saved scene
↓
scene finishes loading
↓
move Player to saved position
↓
restore state
```

---

# 1. Create save folders

Create:

```text
Assets/_Project/Scripts/Save/
```

We'll add:

```text
GameSaveData.cs
SaveSystem.cs
PlayerSaveController.cs
```

---

# 2. Create `GameSaveData.cs`

Create:

```text
GameSaveData.cs
```

Use:

```csharp
using System;

[Serializable]
public class GameSaveData
{
    public string sceneName;

    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    public float playerVelocityX;
    public float playerVelocityY;

    public bool playerFacingRight;

    public bool introSeen;

    public bool movementTutorialTriggered;
    public bool jumpTutorialTriggered;
    public bool exitThoughtTriggered;
}
```

Why separate floats instead of directly saving `Vector3`?

Because this keeps the JSON simple and explicit:

```json
{
  "sceneName": "Room01_Prison",
  "playerPositionX": 6.43,
  "playerPositionY": 2.11,
  "playerPositionZ": 0.0
}
```

---

# 3. Create `SaveSystem.cs`

Create:

```text
SaveSystem.cs
```

Use:

```csharp
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string SaveFileName = "save.json";

    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, SaveFileName);

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void Save(GameSaveData data)
    {
        if (data == null)
        {
            Debug.LogError("SaveSystem: Tried to save null data.");
            return;
        }

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(SavePath, json);

        Debug.Log($"Game saved to: {SavePath}");
    }

    public static GameSaveData Load()
    {
        if (!HasSave())
        {
            Debug.LogWarning("SaveSystem: No save file found.");
            return null;
        }

        string json = File.ReadAllText(SavePath);

        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static void DeleteSave()
    {
        if (!HasSave())
        {
            return;
        }

        File.Delete(SavePath);

        Debug.Log("Save file deleted.");
    }
}
```

This writes the save file into:

```text
Application.persistentDataPath
```

which is the correct kind of location for persistent game saves.

---

# 4. Create `PlayerSaveController.cs`

Create:

```text
PlayerSaveController.cs
```

Attach this script to the root:

```text
Player
```

Use:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSaveController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visual;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public GameSaveData CreateSaveData()
    {
        Vector3 position = transform.position;
        Vector2 velocity = rb.linearVelocity;

        bool facingRight = true;

        if (visual != null)
        {
            facingRight = visual.localScale.x > 0f;
        }

        GameSaveData data = new GameSaveData
        {
            sceneName = SceneManager.GetActiveScene().name,

            playerPositionX = position.x,
            playerPositionY = position.y,
            playerPositionZ = position.z,

            playerVelocityX = velocity.x,
            playerVelocityY = velocity.y,

            playerFacingRight = facingRight,

            introSeen = true
        };

        return data;
    }

    public void ApplySaveData(GameSaveData data)
    {
        if (data == null)
        {
            return;
        }

        transform.position = new Vector3(
            data.playerPositionX,
            data.playerPositionY,
            data.playerPositionZ
        );

        rb.linearVelocity = new Vector2(
            data.playerVelocityX,
            data.playerVelocityY
        );

        ApplyFacingDirection(data.playerFacingRight);
    }

    private void ApplyFacingDirection(bool facingRight)
    {
        if (visual == null)
        {
            return;
        }

        Vector3 scale = visual.localScale;

        scale.x = facingRight
            ? Mathf.Abs(scale.x)
            : -Mathf.Abs(scale.x);

        visual.localScale = scale;
    }
}
```

---

# 5. Assign the Visual reference

Select:

```text
Player
```

In:

```text
Player Save Controller
```

assign:

```text
Visual → Player/Visual
```

So:

```text
Player
├── PlayerMovement
├── PlayerSaveController
├── Rigidbody2D
├── CapsuleCollider2D
│
├── Visual
├── GroundCheck
└── SFXAudio
```

---

# 6. Create a gameplay save manager

Create:

```text
GameSaveManager.cs
```

under:

```text
Assets/_Project/Scripts/Save/
```

Use:

```csharp
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }

    private PlayerSaveController playerSaveController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        FindPlayer();
        TryLoadPlayerState();
    }

    private void FindPlayer()
    {
        playerSaveController =
            FindFirstObjectByType<PlayerSaveController>();

        if (playerSaveController == null)
        {
            Debug.LogWarning(
                "GameSaveManager: PlayerSaveController not found."
            );
        }
    }

    public void SaveGame()
    {
        if (playerSaveController == null)
        {
            FindPlayer();
        }

        if (playerSaveController == null)
        {
            Debug.LogError(
                "GameSaveManager: Cannot save because player was not found."
            );

            return;
        }

        GameSaveData data =
            playerSaveController.CreateSaveData();

        SaveSystem.Save(data);
    }

    private void TryLoadPlayerState()
    {
        if (!SaveSystem.HasSave())
        {
            return;
        }

        GameSaveData data = SaveSystem.Load();

        if (data == null)
        {
            return;
        }

        playerSaveController?.ApplySaveData(data);
    }
}
```

---

# 7. Add the GameSaveManager object to Room 1

In the Hierarchy:

```text
Room01_Prison
├── Systems
│   └── GameSaveManager
├── Main Camera
├── CM_PlayerCamera
├── Environment
├── Gameplay
├── Audio
└── UI
```

Create:

```text
Systems
```

then child:

```text
GameSaveManager
```

Attach:

```text
GameSaveManager.cs
```

For now, don't use `DontDestroyOnLoad`. Each gameplay scene can have its own manager initialize the current scene state.

---

# 8. We need to distinguish New Game from Continue

There's one important problem with the current manager.

Suppose:

```text
saved position = X 20
```

Then you choose:

```text
New Game
```

If the old save was not deleted, Room 1 will immediately restore:

```text
X 20
```

instead of starting at spawn.

So `New Game` must delete the old save.

In your main-menu New Game logic, add:

```csharp
SaveSystem.DeleteSave();
```

before loading the intro.

For example:

```csharp
public void StartNewGame()
{
    SaveSystem.DeleteSave();

    SceneManager.LoadScene("IntroCutscene");
}
```

In your actual project, the scene load is probably handled by `MainMenuTransitionController`, so put `SaveSystem.DeleteSave()` at the beginning of the New Game flow rather than duplicating scene-loading logic.

---

# 9. Create the initial save when entering Room 1

There is another problem.

After:

```text
New Game
→ Intro
→ Room01_Prison
```

there is no save yet.

That's okay initially, but Continue will remain unavailable until the first save.

We need Room 1 to create an initial save at spawn.

Modify `GameSaveManager.Start()`:

```csharp
private void Start()
{
    FindPlayer();

    if (SaveSystem.HasSave())
    {
        TryLoadPlayerState();
    }
    else
    {
        SaveGame();
    }
}
```

Now:

### New Game

```text
no save
↓
Room 1 loads
↓
Player at spawn
↓
initial save created
```

### Continue

```text
save exists
↓
Room 1 loads
↓
saved position restored
```

---

# 10. Add a temporary save key for testing

Before building pause menu saving, let's make sure the save system actually works.

Add to `GameSaveManager`:

```csharp
private void Update()
{
    if (UnityEngine.InputSystem.Keyboard.current == null)
    {
        return;
    }

    if (UnityEngine.InputSystem.Keyboard.current.f5Key.wasPressedThisFrame)
    {
        SaveGame();
    }
}
```

Now test:

```text
Start Room 1
↓
Walk to X = 15
↓
Press F5
↓
Stop game
↓
Play again
```

Expected:

```text
cat appears at X = 15
```

not the original spawn.

Once confirmed, we can remove the temporary F5 input.

---

# 11. Save tutorial trigger states

Right now, if you:

```text
trigger movement tutorial
↓
save
↓
continue
```

the tutorial trigger would appear again.

So we should make tutorial triggers saveable too.

Modify `TutorialTrigger.cs`.

Add:

```csharp
public bool HasTriggered => hasTriggered;
```

and:

```csharp
public void SetTriggeredState(bool triggered)
{
    hasTriggered = triggered;
}
```

So part of the script becomes:

```csharp
private bool hasTriggered;

public bool HasTriggered => hasTriggered;

public void SetTriggeredState(bool triggered)
{
    hasTriggered = triggered;
}
```

---

## Give triggers explicit references in GameSaveManager

Add:

```csharp
[Header("Tutorial Triggers")]
[SerializeField] private TutorialTrigger movementTutorial;
[SerializeField] private TutorialTrigger jumpTutorial;
[SerializeField] private TutorialTrigger exitThoughtTutorial;
```

Then modify `SaveGame()` after creating the data:

```csharp
data.movementTutorialTriggered =
    movementTutorial != null &&
    movementTutorial.HasTriggered;

data.jumpTutorialTriggered =
    jumpTutorial != null &&
    jumpTutorial.HasTriggered;

data.exitThoughtTriggered =
    exitThoughtTutorial != null &&
    exitThoughtTutorial.HasTriggered;
```

Then in load:

```csharp
movementTutorial?.SetTriggeredState(
    data.movementTutorialTriggered
);

jumpTutorial?.SetTriggeredState(
    data.jumpTutorialTriggered
);

exitThoughtTutorial?.SetTriggeredState(
    data.exitThoughtTriggered
);
```

Your `TryLoadPlayerState()` becomes roughly:

```csharp
private void TryLoadPlayerState()
{
    GameSaveData data = SaveSystem.Load();

    if (data == null)
    {
        return;
    }

    playerSaveController?.ApplySaveData(data);

    movementTutorial?.SetTriggeredState(
        data.movementTutorialTriggered
    );

    jumpTutorial?.SetTriggeredState(
        data.jumpTutorialTriggered
    );

    exitThoughtTutorial?.SetTriggeredState(
        data.exitThoughtTriggered
    );
}
```

Then assign the three triggers in the Inspector.

---

# 12. Continue button scene loading

The Continue button should not directly assume:

```text
Room01_Prison
```

It should read:

```text
data.sceneName
```

Conceptually:

```csharp
public void ContinueGame()
{
    GameSaveData data = SaveSystem.Load();

    if (data == null)
    {
        return;
    }

    SceneManager.LoadScene(data.sceneName);
}
```

Then Room 1's `GameSaveManager` restores the player's exact state when the scene loads.

The flow is:

```text
MainMenu
↓
SaveSystem.Load()
↓
sceneName = Room01_Prison
↓
SceneManager.LoadScene("Room01_Prison")
↓
Room's GameSaveManager starts
↓
ApplySaveData()
↓
player position restored
```

---

# 13. One thing I recommend: don't restore vertical velocity yet

Technically, you asked for exact state, so the code above stores:

```text
velocity X
velocity Y
```

However, saving in the middle of a jump can produce this:

```text
save while falling
↓
Continue
↓
cat appears in air
↓
immediately continues falling
```

That is technically more exact.

For a cozy Metroidvania, I would personally restore:

```csharp
rb.linearVelocity = Vector2.zero;
```

instead of the saved velocity.

So in `ApplySaveData()`:

```csharp
rb.linearVelocity = Vector2.zero;
```

This preserves exact position but avoids strange scene-entry momentum.

I recommend that approach for your game.

---

# 14. Current save data versus future save data

For Room 1:

```text
scene
player position
facing direction
tutorial state
intro state
```

Soon, when you add abilities:

```text
HasDoubleJump
HasDash
HasWallJump
```

Later:

```text
collectedItemIds
openedDoorIds
defeatedBossIds
activatedCheckpointId
```

For movable objects like crates, don't hard-code:

```text
crate1X
crate1Y
crate2X
crate2Y
```

We'll eventually create a generic system where every persistent object has a unique ID, for example:

```text
Room01_Crate_001
Room03_Door_002
Forest_Collectible_014
```

and save their states in collections.

---

## What I recommend implementing now

Implement this part first:

```text
1. GameSaveData
2. SaveSystem
3. PlayerSaveController
4. GameSaveManager
5. New Game deletes old save
6. F5 temporary save
7. Continue loads saved scene
8. Test exact Player position restoration
```

Then after position save/load is confirmed, add tutorial trigger persistence. That way, if something breaks, we know whether the problem is the core save system or the extra room-state saving logic.


- **Modifications manuelles** : Re-itération pour corriger la sauvegarde de la position + debugging
- **Décision d'intégration** : Accepté après modifications

- **Qualité estimée de la réponse** : 4
- **Hallucination détectée** : A oublié le context et a cassé en partie la save feature d'avant