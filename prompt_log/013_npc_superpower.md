## Prompt Log — 22/07 — Konovalova Sofia — S2
- **Contexte** : création de room avec un NPC et unlock de nouveau pouvoir
- **Prompt** :
> now we add the room04 with one npc that we can talk to. i have a dialog icon that should appear near the npc. the npc has two side views (for speaking) and 1 front view when we dont interact with him. the npc is the old cat that should warn our hero to not trust the mice and to teach as the double jump

- **Modèle & outil** : ChatGPT GPT-5.5
- **Réponse / Output** : 
> Yes. Room 04 can become a **story + ability unlock room**.

The goal of Room 04:

```text
Room 04 — Old Cat Shelter

Player enters after unlocking the door with the key.
There is no combat.
The Old Cat NPC waits inside.
A dialogue icon appears near him when the player is close.
Press E to talk.
The Old Cat warns the hero not to trust the mice.
At the end of the dialogue, the player learns Double Jump.
```

We’ll build it in this order:

```text
1. Create Room04 scene
2. Add player entry point from Room03
3. Add Old Cat NPC
4. Add dialogue icon
5. Add NPC interaction script
6. Add dialogue messages
7. Unlock Double Jump
8. Save Double Jump permanently
9. Test Room03 → Room04 → talk → ability unlock
```

---

# 1. Create the new scene

In Unity:

```text
File → New Scene
```

Save it as:

```text
Assets/_Project/Scenes/Room04_OldCatShelter.unity
```

Then add it to Build Profiles:

```text
File → Build Profiles
→ Add Open Scenes
```

Your scenes should include:

```text
Room01_Prison
Room02_EscapeCorridor
Room03_FirstCombat
Room04_OldCatShelter
```

---

# 2. Create the Room 04 hierarchy

In `Room04_OldCatShelter`, create this structure:

```text
Room04_OldCatShelter
├── Systems
│   ├── SceneEntryController
│   ├── RoomSaveController
│   └── PauseMenuController
├── Environment
│   ├── Ground
│   ├── Background
│   └── Door_BackToRoom03
├── Gameplay
│   ├── Entry_FromRoom03
│   ├── Player
│   ├── OldCatNPC
│   └── ExitTrigger_BackToRoom03
├── UI
│   ├── TutorialCanvas
│   ├── GameplayCanvas
│   ├── PauseCanvas
│   └── SceneFadeCanvas
```

You can duplicate Room 03 and remove enemies/key/basket if that is faster. Keep the working UI and fade system.

---

# 3. Add the Room 04 entry point

Create an empty object:

```text
Gameplay
└── Entry_FromRoom03
```

Add:

```text
RoomEntryPoint
```

Set:

```text
Entry Point Id = FromRoom03
```

Place it near the entrance where the cat arrives from Room 03.

---

# 4. Update the locked door in Room 03

In Room 03, select the locked door trigger that opens after the key.

Set the `LockedRoomTransitionTrigger` values:

```text
Target Scene Name = Room04_OldCatShelter
Target Entry Point Id = FromRoom03
```

So the transition becomes:

```text
Room03 locked door
↓
Room04_OldCatShelter
↓
Entry_FromRoom03
```

---

# 5. Create the Old Cat NPC object

In Room 04, create:

```text
Gameplay
└── OldCatNPC
```

Add:

```text
SpriteRenderer
BoxCollider2D
OldCatNPC
```

Set the collider:

```text
BoxCollider2D
Is Trigger = ON
```

The trigger area should be larger than the old cat sprite, so the player can interact from nearby.

Suggested size:

```text
Width = 2.5
Height = 2
```

---

# 6. Set up the NPC visual sprites

Your Old Cat has:

```text
1 front view idle sprite
2 side view speaking sprites
```

Use the front view as the default.

On `OldCatNPC → SpriteRenderer`:

```text
Sprite = old_cat_front_idle
Sorting Layer = Characters
Order in Layer = 10
```

The NPC will use:

```text
Front sprite → when idle / not talking
Side speaking sprite 1 → while speaking
Side speaking sprite 2 → while speaking, alternating
```

---

# 7. Add the dialogue icon

Under `OldCatNPC`, create:

```text
OldCatNPC
└── DialogIcon
```

Add:

```text
SpriteRenderer
```

Assign your dialogue icon sprite.

Place it above or beside the NPC’s head:

```text
Local Position X = 0
Local Position Y = 1.3
Local Position Z = 0
```

Set:

```text
Sorting Layer = UI or Characters
Order in Layer = 50
```

Disable it by default:

```text
DialogIcon active = OFF
```

The icon will appear only when the player is close.

---

# 8. Create `PlayerAbilities.cs`

This script stores whether the player has unlocked Double Jump.

Create:

```text
Assets/_Project/Scripts/Player/PlayerAbilities.cs
```

Use:

```csharp
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public bool HasDoubleJump { get; private set; }

    private void Start()
    {
        HasDoubleJump = GameSaveManager.HasDoubleJump();
    }

    public void UnlockDoubleJump()
    {
        if (HasDoubleJump)
        {
            return;
        }

        HasDoubleJump = true;
        GameSaveManager.UnlockDoubleJump();

        Debug.Log("Player unlocked Double Jump.");
    }
}
```

Attach this to:

```text
Player
```

---

# 9. Add Double Jump to save data

Open:

```text
GameSaveData.cs
```

Add:

```csharp
public bool hasDoubleJump;
```

---

# 10. Add Double Jump methods to `GameSaveManager`

Open:

```text
GameSaveManager.cs
```

Add:

```csharp
public static bool HasDoubleJump()
{
    GameSaveData data = LoadOrCreate();
    return data.hasDoubleJump;
}

public static void UnlockDoubleJump()
{
    GameSaveData data = LoadOrCreate();

    if (data.hasDoubleJump)
    {
        return;
    }

    data.hasDoubleJump = true;

    SaveSystem.Save(data);

    Debug.Log("Double Jump saved as unlocked.");
}
```

Now the ability stays unlocked after changing scenes or reloading.

---

# 11. Create `OldCatNPC.cs`

Create:

```text
Assets/_Project/Scripts/NPC/OldCatNPC.cs
```

Use this full script:

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class OldCatNPC : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite frontIdleSprite;
    [SerializeField] private Sprite speakingSideSpriteA;
    [SerializeField] private Sprite speakingSideSpriteB;

    [Header("Interaction")]
    [SerializeField] private GameObject dialogIcon;
    [SerializeField] private TutorialUIController tutorialUI;
    [SerializeField] private PlayerAbilities playerAbilities;

    [Header("Dialogue")]
    [TextArea]
    [SerializeField] private string[] dialogueLines =
    {
        "Wait, little one...",
        "You carry the smell of the tunnels.",
        "Do not trust every mouse who smiles at you.",
        "Some are afraid. Some are kind. But some serve the Mouse King.",
        "If you want to survive, you must learn to move higher.",
        "Listen to your paws. Jump once... then jump again.",
        "You learned Double Jump."
    };

    [SerializeField] private float lineDuration = 3f;
    [SerializeField] private float speakingSpriteSwapSpeed = 0.25f;

    private bool playerNearby;
    private bool isTalking;
    private bool dialogueCompleted;
    private Coroutine speakingAnimationCoroutine;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (dialogIcon != null)
        {
            dialogIcon.SetActive(false);
        }

        if (frontIdleSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = frontIdleSprite;
        }
    }

    private void Update()
    {
        if (!playerNearby || isTalking)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(DialogueRoutine());
        }
    }

    private IEnumerator DialogueRoutine()
    {
        isTalking = true;

        if (dialogIcon != null)
        {
            dialogIcon.SetActive(false);
        }

        StartSpeakingAnimation();

        Debug.Log("Old Cat dialogue started.");

        for (int i = 0; i < dialogueLines.Length; i++)
        {
            if (tutorialUI != null)
            {
                tutorialUI.ShowMessage(
                    dialogueLines[i],
                    lineDuration
                );
            }

            yield return new WaitForSecondsRealtime(lineDuration);
        }

        StopSpeakingAnimation();

        if (frontIdleSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = frontIdleSprite;
        }

        if (!dialogueCompleted)
        {
            dialogueCompleted = true;

            if (playerAbilities != null)
            {
                playerAbilities.UnlockDoubleJump();
            }
            else
            {
                Debug.LogWarning(
                    "OldCatNPC: PlayerAbilities is not assigned.",
                    this
                );
            }
        }

        isTalking = false;

        if (playerNearby && dialogIcon != null)
        {
            dialogIcon.SetActive(true);
        }

        Debug.Log("Old Cat dialogue finished.");
    }

    private void StartSpeakingAnimation()
    {
        if (speakingAnimationCoroutine != null)
        {
            StopCoroutine(speakingAnimationCoroutine);
        }

        speakingAnimationCoroutine =
            StartCoroutine(SpeakingAnimationRoutine());
    }

    private void StopSpeakingAnimation()
    {
        if (speakingAnimationCoroutine != null)
        {
            StopCoroutine(speakingAnimationCoroutine);
            speakingAnimationCoroutine = null;
        }
    }

    private IEnumerator SpeakingAnimationRoutine()
    {
        bool useFirstSprite = true;

        while (true)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = useFirstSprite
                    ? speakingSideSpriteA
                    : speakingSideSpriteB;
            }

            useFirstSprite = !useFirstSprite;

            yield return new WaitForSecondsRealtime(
                speakingSpriteSwapSpeed
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerNearby = true;

        if (dialogIcon != null && !isTalking)
        {
            dialogIcon.SetActive(true);
        }

        if (playerAbilities == null)
        {
            playerAbilities = other.GetComponent<PlayerAbilities>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerNearby = false;

        if (dialogIcon != null)
        {
            dialogIcon.SetActive(false);
        }
    }
}
```

---

# 12. Assign Old Cat NPC references

Select:

```text
OldCatNPC
```

Assign:

```text
Sprite Renderer → OldCatNPC SpriteRenderer

Front Idle Sprite → old_cat_front
Speaking Side Sprite A → old_cat_speaking_01
Speaking Side Sprite B → old_cat_speaking_02

Dialog Icon → DialogIcon
Tutorial UI → TutorialCanvas
Player Abilities → Player
```

If `Player Abilities` is empty, it can still auto-find it when the player enters the trigger, but assigning it manually is cleaner.

---

# 13. Add Double Jump to Player movement

Now we need the actual movement ability.

Since I don’t have your exact current `PlayerMovement.cs`, I’ll describe the logic you need to add.

Your movement script probably already has:

```csharp
private bool isGrounded;
private bool jumpRequested;
```

You need to add:

```csharp
[SerializeField] private PlayerAbilities playerAbilities;

private bool hasUsedDoubleJump;
```

When the player touches ground:

```csharp
if (isGrounded)
{
    hasUsedDoubleJump = false;
}
```

When jump is pressed:

```csharp
if (isGrounded)
{
    Jump();
}
else if (
    playerAbilities != null &&
    playerAbilities.HasDoubleJump &&
    !hasUsedDoubleJump)
{
    Jump();
    hasUsedDoubleJump = true;
}
```

Conceptually:

```text
On ground:
    jump normally
    reset double jump

In air:
    if Double Jump unlocked and not used yet:
        jump again
        mark double jump as used
```

If you send me your current `PlayerMovement.cs`, I can rewrite the exact full file without breaking your existing movement/landing/pause logic.

---

# 14. Add a small Double Jump unlock message

At the end of the dialogue, the last line already says:

```text
You learned Double Jump.
```

But if you want it to feel more like a tutorial, use:

```text
You learned Double Jump.
Press jump again while in the air.
```

So the final dialogue array could be:

```text
"Wait, little one..."
"You carry the smell of the tunnels."
"Do not trust every mouse who smiles at you."
"Some are afraid. Some are kind. But some serve the Mouse King."
"If you want to survive, you must learn to move higher."
"Listen to your paws. Jump once... then jump again."
"You learned Double Jump. Press jump again while in the air."
```

---

# 15. Recommended Room 04 layout

Room 04 should feel safe and meaningful.

Simple layout:

```text
################################
#                              #
#                              #
#          Old Cat             #
#            ?                 #
#          [NPC]               #
#                              #
#  Player                      #
#   X                          #
################################
```

Suggested gameplay flow:

```text
Enter from Room 03
↓
Walk forward
↓
See Old Cat
↓
Dialogue icon appears
↓
Talk
↓
Unlock Double Jump
↓
Exit later toward vertical room / Cheese Tunnels
```

Since this is an ability room, do not add enemies here.

---

# 16. Test Room 04

Test this exact sequence:

```text
1. Enter Room04 from Room03
2. Player appears at Entry_FromRoom03
3. Walk near OldCatNPC
4. DialogIcon appears
5. Press E
6. Old Cat switches from front sprite to speaking side sprites
7. Dialogue lines appear
8. At the end, Console says:
   Player unlocked Double Jump.
   Double Jump saved as unlocked.
9. Press jump in air
10. Cat can jump twice
```

---

# 17. Important save testing

If you already talked to the Old Cat once, `hasDoubleJump` may be saved.

For testing from scratch, delete your save file or start a new game.

Otherwise the player may already have Double Jump before talking.

---

For now, implement the NPC and ability unlock first. Then send me your current `PlayerMovement.cs`, and I’ll give you the exact edited version with Double Jump integrated into your existing movement script.


- **Modifications manuelles** : reitération pour  corriger le regard du NPC (vers le joueur)
- **Décision d'intégration** : Accepté après modifications

- **Qualité estimée de la réponse** : 4
- **Hallucination détectée** :