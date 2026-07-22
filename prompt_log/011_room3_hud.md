## Prompt Log — 22/07 — Konovalova Sofia — S2
- **Contexte** : Implémentation de room 3 avec l'interface à partir de la description de la room générée
- **Prompt** : + fichier avec la description de la room
> now you tell me in details step by step how to implement this room in our game. i would like to make a change also, we will place the cat basket checkpoint near the start of the room and add 4 hp to the cat (i have assets to display full hp unit and empty one). if the cat falls to 0 hp he respawns at the last visited checkpoint. last visited checkpoint is considered the last basket the cat did cat nap in. resting at checkpoint restores the hp to max and also revives all enemies

- **Modèle & outil** : ChatGPT GPT-5.5
- **Réponse / Output** : 
> Yes. For Room 3, I would implement it as the point where several permanent game systems appear together:

```text
Room 3
├── first real combat
├── Player HP: 4 units
├── HP HUD
├── first cat-basket checkpoint
├── resting / cat nap
├── healing at checkpoint
├── enemy respawn on rest
├── death and respawn
├── encounter-controlled exit
└── transition Room 2 ⇄ Room 3
```

The uploaded Room 3 design is intentionally simple: a mostly flat `36 × 16` room, one mouse enemy at a time, a quiet section between fights, a safe platform, then a reward and locked exit. 

I recommend implementing it in the following order so that each system can be tested independently.

---

# Part 1 — Final gameplay flow

The room will work like this:

```text
Enter from Room 2
        ↓
Cat Basket checkpoint near entrance
        ↓
Press E near basket
        ↓
CAT NAP
├── checkpoint becomes active
├── HP restored to 4/4
├── all room enemies revived
└── game saved
        ↓
First mouse guard
        ↓
quiet section
        ↓
safe platform
        ↓
second mouse guard
        ↓
both guards defeated
        ↓
exit unlocks
        ↓
Room 4
```

Damage loop:

```text
Enemy attack hits cat
        ↓
HP decreases
        ↓
HUD updates

HP > 0
→ continue playing

HP = 0
→ death fade
→ load scene containing last checkpoint
→ spawn near last basket
→ restore HP
→ enemies return alive
```

The phrase **“last visited checkpoint”** should mean:

> the last basket where the player actually performed the Cat Nap interaction,

not merely a basket the player walked past.

---

# Part 2 — Create Room 3

## Step 1 — Duplicate Room 2

In the Project window:

```text
Room02_EscapeCorridor
→ Ctrl + D
→ Room03_FirstCombat
```

Open:

```text
Room03_FirstCombat
```

Immediately save:

```text
Ctrl + S
```

This lets us keep the systems already working:

```text
Player
Cinemachine camera
Pause menu
Settings panel
Scene fade
Memory UI
Save system
Room transition system
Audio routing
```

---

# Part 3 — Clean the duplicated room

Remove Room 2-specific content:

```text
Room 2 terrain
Room 2 memory collectible
future Double Jump route
Room 2 environmental triggers
Room 2 obstacle
Room 2-specific props
Room 2 camera bounds shape
```

Keep:

```text
Systems
Player
Main Camera
Cinemachine Camera
UI
Audio
Grid
GroundTilemap
```

Clear the GroundTilemap and reuse it.

---

# Part 4 — Recommended Room 3 hierarchy

I recommend:

```text
Room03_FirstCombat
│
├── Systems
│   ├── RoomSaveController
│   ├── PauseMenuController
│   ├── SceneEntryController
│   ├── CheckpointManager
│   ├── EnemyRespawnManager
│   └── EncounterManager
│
├── Main Camera
├── CM_PlayerCamera
│
├── Environment
│   ├── Background
│   ├── Grid
│   │   └── GroundTilemap
│   │
│   ├── CatBasketCheckpoint
│   │   └── CheckpointSpawn
│   │
│   ├── SafePlatform
│   ├── ExitDoor
│   ├── ExitBlocker
│   ├── PropsRoot
│   └── Room03CameraBounds
│
├── Gameplay
│   ├── Entry_FromRoom02
│   ├── Player
│   ├── CombatTutorialTrigger
│   ├── RewardItem
│   ├── ExitTrigger_ToRoom04
│   └── ExitTrigger_ToRoom02
│
├── Enemies
│   ├── MouseGuard_01
│   └── MouseGuard_02
│
├── Audio
│   └── RoomMusic
│
└── UI
    ├── GameplayCanvas
    │   └── HealthHUD
    │
    ├── TutorialCanvas
    ├── MemoryCanvas
    ├── PauseCanvas
    └── SceneFadeCanvas
```

---

# Part 5 — Copyable Room 3 grid schema

The room blueprint uses approximately `36 × 16` tiles. 

Use this as the layout reference:

```text
####################################
#..................................#
#..................................#
#..................................#
#..................................#
#..................................#
#.............SSSSSS...............#
#.............######...............#
#..................................#
#..................................#
#........E1..............E2........#
#..P.B.....................C....DDD#
################################DDD#
################################DDD#
####################################
####################################
```

## Legend

```text
P  = Player entry from Room 2
B  = Cat Basket checkpoint
E1 = First Mouse Guard
E2 = Second Mouse Guard
S  = Top surface of safe retreat platform
#  = Solid terrain
C  = Combat reward
D  = Locked exit toward Room 4
.  = Empty space
```

Only paint these on the Tilemap:

```text
#
```

The following should be GameObjects:

```text
P
B
E1
E2
C
D
```

The safe platform can either be painted into terrain or built as its own environment object.

---

# Part 6 — Build the room geometry first

Before coding HP or combat, test the room shape.

## Step 1 — Main floor

The combat area should be almost entirely flat.

The uploaded design deliberately makes combat—not platforming—the challenge of this room. 

Create approximately:

```text
36 tiles wide
16 tiles high
```

with a flat combat floor.

---

## Step 2 — Entrance safe zone

Leave approximately:

```text
5–6 tiles
```

before the first enemy can attack.

The start should look conceptually like:

```text
ENTRY   BASKET                     ENEMY 1

 P        🧺                           E1

─────────────────────────────────────────
```

The player should have time to:

* enter;
* notice the basket;
* rest;
* observe the enemy.

---

## Step 3 — Safe platform

Near the middle:

```text
        SAFE PLATFORM
       ┌────────────┐
       │            │
───────┘            └────────────────
```

Recommended:

```text
Height: 2 tiles
Width: 5–6 tiles
```

The cat must be able to reach it with basic jump.

The second mouse can remain unable to climb onto it for now.

---

# Part 7 — Add the Player health system

We will now give the cat:

```text
Maximum HP = 4
Starting HP = 4
```

I recommend treating each paw/heart icon as exactly one HP.

So:

```text
■■■■ = 4 HP
■■■□ = 3 HP
■■□□ = 2 HP
■□□□ = 1 HP
□□□□ = 0 HP
```

---

## Step 1 — Create `PlayerHealth.cs`

Create:

```text
Assets/_Project/Scripts/Player/PlayerHealth.cs
```

Use:

```csharp
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 4;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }

    public event Action<int, int> HealthChanged;
    public event Action Died;

    private bool isDead;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(
            0,
            CurrentHealth - damage
        );

        HealthChanged?.Invoke(
            CurrentHealth,
            maxHealth
        );

        if (CurrentHealth == 0)
        {
            Die();
        }
    }

    public void HealFull()
    {
        if (isDead)
        {
            return;
        }

        CurrentHealth = maxHealth;

        HealthChanged?.Invoke(
            CurrentHealth,
            maxHealth
        );
    }

    public void RestoreHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(
            amount,
            0,
            maxHealth
        );

        isDead = false;

        HealthChanged?.Invoke(
            CurrentHealth,
            maxHealth
        );
    }

    private void Die()
    {
        isDead = true;
        Died?.Invoke();
    }
}
```

Attach it to the Player root:

```text
Player
├── PlayerMovement
├── PlayerSaveController
├── PlayerHealth
└── ...
```

Set:

```text
Max Health = 4
```

---

# Part 8 — Create the HP HUD

You said you already have:

```text
full HP unit asset
empty HP unit asset
```

Perfect.

I recommend showing exactly four units at all times.

## Step 1 — Create the hierarchy

Under `GameplayCanvas`:

```text
GameplayCanvas
└── HealthHUD
    ├── HealthUnit_01
    ├── HealthUnit_02
    ├── HealthUnit_03
    └── HealthUnit_04
```

Each HealthUnit should have:

```text
Image
```

Use a `HorizontalLayoutGroup` on `HealthHUD`.

Suggested:

```text
Child Alignment: Middle Left
Spacing: 8
Control Child Size: OFF
Child Force Expand: OFF
```

Anchor the HUD to:

```text
Top Left
```

Suggested position:

```text
Left: 35
Top: 30
```

---

## Step 2 — Create `HealthHUDController.cs`

```text
Assets/_Project/Scripts/UI/HealthHUDController.cs
```

Use:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class HealthHUDController : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image[] healthUnits;

    [Header("Sprites")]
    [SerializeField] private Sprite fullHealthSprite;
    [SerializeField] private Sprite emptyHealthSprite;

    private void OnEnable()
    {
        if (playerHealth == null)
        {
            return;
        }

        playerHealth.HealthChanged += UpdateHUD;

        UpdateHUD(
            playerHealth.CurrentHealth,
            playerHealth.MaxHealth
        );
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= UpdateHUD;
        }
    }

    private void UpdateHUD(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < healthUnits.Length; i++)
        {
            healthUnits[i].sprite =
                i < currentHealth
                    ? fullHealthSprite
                    : emptyHealthSprite;
        }
    }
}
```

Assign:

```text
Player Health
→ Player

Health Units
Size = 4
0 → HealthUnit_01
1 → HealthUnit_02
2 → HealthUnit_03
3 → HealthUnit_04

Full Health Sprite
→ your full asset

Empty Health Sprite
→ your empty asset
```

---

# Part 9 — Extend the save data for HP and checkpoints

Open:

```text
GameSaveData.cs
```

Add:

```csharp
public int currentHealth = 4;

public bool hasCheckpoint;
public string checkpointSceneName;
public string checkpointEntryPointId;
```

Conceptually:

```text
currentHealth
= health preserved between normal rooms

checkpointSceneName
= scene where last Cat Nap happened

checkpointEntryPointId
= exact respawn point beside the basket
```

---

# Part 10 — Create the basket checkpoint

Place the basket near the start of Room 3:

```text
ENTRY
  P

      🧺 Basket

──────────────────────────────
```

Do not put it immediately inside the room-transition trigger.

---

## Step 1 — Basket hierarchy

Create:

```text
Environment
└── CatBasketCheckpoint
    └── CheckpointSpawn
```

`CatBasketCheckpoint`:

```text
SpriteRenderer
BoxCollider2D
CheckpointBasket
```

Set BoxCollider2D:

```text
Is Trigger = ON
```

`CheckpointSpawn` is an empty GameObject.

Position it next to the basket:

```text
🧺    X
      ↑
CheckpointSpawn
```

Do not spawn the cat directly on top of the basket.

---

# Part 11 — Reuse the entry-point system for checkpoints

Attach:

```text
RoomEntryPoint
```

to:

```text
CheckpointSpawn
```

Set ID:

```text
Checkpoint_Room03_01
```

This means the death system can reuse the scene-entry architecture you already created.

---

# Part 12 — Add checkpoint methods to GameSaveManager

Add something like:

```csharp
public static void ActivateCheckpoint(
    string sceneName,
    string entryPointId,
    int fullHealth)
{
    GameSaveData data = LoadOrCreate();

    data.hasCheckpoint = true;
    data.checkpointSceneName = sceneName;
    data.checkpointEntryPointId = entryPointId;
    data.currentHealth = fullHealth;

    SaveSystem.Save(data);
}
```

Also add:

```csharp
public static GameSaveData GetSaveData()
{
    return LoadOrCreate();
}
```

You may already have an equivalent loading method. Do not duplicate it unnecessarily.

---

# Part 13 — Create `CheckpointBasket.cs`

Create:

```text
Assets/_Project/Scripts/Checkpoints/CheckpointBasket.cs
```

Use:

```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CheckpointBasket : MonoBehaviour
{
    [Header("Checkpoint")]
    [SerializeField] private string checkpointEntryPointId;

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private EnemyRespawnManager enemyRespawnManager;

    private bool playerNearby;

    private void Update()
    {
        if (!playerNearby)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Rest();
        }
    }

    private void Rest()
    {
        playerHealth.HealFull();

        enemyRespawnManager.RespawnAllEnemies();

        GameSaveManager.ActivateCheckpoint(
            SceneManager.GetActiveScene().name,
            checkpointEntryPointId,
            playerHealth.MaxHealth
        );

        Debug.Log(
            $"Checkpoint activated: {checkpointEntryPointId}"
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
```

Configure:

```text
Checkpoint Entry Point Id
→ Checkpoint_Room03_01

Player Health
→ Player

Enemy Respawn Manager
→ Systems
```

---

# Part 14 — Show an interaction hint near the basket

I recommend a simple hint:

```text
Press E to Cat Nap
```

Create:

```text
CatBasketCheckpoint
└── InteractionPrompt
```

Using a small World Space Canvas or your existing tutorial UI.

Behavior:

```text
player enters basket area
→ show "Press E to Cat Nap"

player leaves
→ hide it
```

Later you can animate:

```text
Zzz...
```

for approximately one second before completing the rest.

For the first implementation, instant rest is fine.

---

# Part 15 — Implement enemy health

Create:

```text
Assets/_Project/Scripts/Enemies/EnemyHealth.cs
```

Example:

```csharp
using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<EnemyHealth> Died;

    private void Awake()
    {
        ResetHealth();
    }

    public void TakeDamage(int damage)
    {
        if (IsDead)
        {
            return;
        }

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;
        CurrentHealth = 0;

        Died?.Invoke(this);

        gameObject.SetActive(false);
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        IsDead = false;
    }
}
```

For these first enemies:

```text
Max Health = 3
```

assuming one player claw hit does:

```text
1 damage
```

So:

```text
3 claw hits → enemy defeated
```

That matches the room's beginner-combat purpose. 

---

# Part 16 — Make enemies respawnable

Create:

```text
EnemyRespawnable.cs
```

```csharp
using UnityEngine;

public class EnemyRespawnable : MonoBehaviour
{
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private EnemyHealth enemyHealth;
    private Rigidbody2D rb;

    private void Awake()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        enemyHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Respawn()
    {
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (enemyHealth != null)
        {
            enemyHealth.ResetHealth();
        }
    }
}
```

Attach it to both:

```text
MouseGuard_01
MouseGuard_02
```

---

# Part 17 — Create EnemyRespawnManager

Create:

```text
EnemyRespawnManager.cs
```

Attach it to `Systems`.

```csharp
using UnityEngine;

public class EnemyRespawnManager : MonoBehaviour
{
    [SerializeField]
    private EnemyRespawnable[] enemies;

    public void RespawnAllEnemies()
    {
        foreach (EnemyRespawnable enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.Respawn();
            }
        }
    }
}
```

Inspector:

```text
Enemies Size: 2

0 → MouseGuard_01
1 → MouseGuard_02
```

Now:

```text
Cat Nap
↓
EnemyRespawnManager.RespawnAllEnemies()
↓
both guards return
```

---

# Part 18 — Important: reset the encounter when enemies revive

Otherwise this could happen:

```text
kill both enemies
→ door opens
→ rest
→ enemies revive
→ door remains open ❌
```

We need the encounter to reset too.

The desired behavior is:

```text
rest
↓
enemies revive
↓
encounter resets
↓
exit locks again
```

Modify `EnemyRespawnManager`:

```csharp
using System;
using UnityEngine;

public class EnemyRespawnManager : MonoBehaviour
{
    public event Action EnemiesRespawned;

    [SerializeField]
    private EnemyRespawnable[] enemies;

    public void RespawnAllEnemies()
    {
        foreach (EnemyRespawnable enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.Respawn();
            }
        }

        EnemiesRespawned?.Invoke();
    }
}
```

---

# Part 19 — Create EncounterManager

Create:

```text
Assets/_Project/Scripts/Combat/EncounterManager.cs
```

Attach to `Systems`.

The logic:

```text
2 required enemies
↓
Enemy 1 dies → remaining 1
Enemy 2 dies → remaining 0
↓
unlock exit
```

Example:

```csharp
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private EnemyHealth[] requiredEnemies;

    [Header("Exit")]
    [SerializeField] private GameObject exitBlocker;
    [SerializeField] private Animator exitDoorAnimator;

    [Header("Respawn")]
    [SerializeField] private EnemyRespawnManager respawnManager;

    private int remainingEnemies;

    private void Start()
    {
        foreach (EnemyHealth enemy in requiredEnemies)
        {
            enemy.Died += OnEnemyDied;
        }

        if (respawnManager != null)
        {
            respawnManager.EnemiesRespawned += ResetEncounter;
        }

        ResetEncounter();
    }

    private void OnEnemyDied(EnemyHealth enemy)
    {
        remainingEnemies--;

        if (remainingEnemies <= 0)
        {
            UnlockExit();
        }
    }

    private void ResetEncounter()
    {
        remainingEnemies = requiredEnemies.Length;

        if (exitBlocker != null)
        {
            exitBlocker.SetActive(true);
        }

        if (exitDoorAnimator != null)
        {
            exitDoorAnimator.SetBool("Open", false);
        }
    }

    private void UnlockExit()
    {
        if (exitBlocker != null)
        {
            exitBlocker.SetActive(false);
        }

        if (exitDoorAnimator != null)
        {
            exitDoorAnimator.SetBool("Open", true);
        }
    }
}
```

Assign:

```text
Required Enemies
Size 2
0 → MouseGuard_01
1 → MouseGuard_02

Exit Blocker
→ ExitBlocker

Exit Door Animator
→ ExitDoor

Respawn Manager
→ Systems
```

---

# Part 20 — Player damage

Each Mouse Guard attack should deal:

```text
1 HP
```

So the player can survive:

```text
4 total hits from full HP
```

A simple enemy attack hitbox can call:

```csharp
playerHealth.TakeDamage(1);
```

Do not make ordinary enemy body contact constantly drain HP every physics frame.

Damage should happen through a controlled attack event:

```text
mouse attack wind-up
↓
attack animation
↓
hitbox briefly active
↓
1 damage
↓
hitbox disabled
```

That is much easier to balance.

---

# Part 21 — Add invulnerability frames

Without this, a single attack collider can damage the cat several times.

Add to `PlayerHealth`:

```csharp
[SerializeField]
private float invulnerabilityDuration = 0.8f;

private bool isInvulnerable;
```

At the beginning of `TakeDamage`:

```csharp
if (isDead || isInvulnerable || damage <= 0)
{
    return;
}
```

After applying damage:

```csharp
StartCoroutine(InvulnerabilityRoutine());
```

Add:

```csharp
private IEnumerator InvulnerabilityRoutine()
{
    isInvulnerable = true;

    yield return new WaitForSeconds(
        invulnerabilityDuration
    );

    isInvulnerable = false;
}
```

And add:

```csharp
using System.Collections;
```

For the cat:

```text
Invulnerability Duration: 0.8
```

is a good starting point.

---

# Part 22 — Player death and checkpoint respawn

Create:

```text
DeathRespawnController.cs
```

Attach to `Systems`.

Its responsibility:

```text
listen to PlayerHealth.Died
↓
get saved checkpoint
↓
restore HP value
↓
set transition entry point
↓
load checkpoint scene
```

Example:

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathRespawnController : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    private void OnEnable()
    {
        playerHealth.Died += HandleDeath;
    }

    private void OnDisable()
    {
        playerHealth.Died -= HandleDeath;
    }

    private void HandleDeath()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // Optional:
        // yield return SceneFadeController.Instance.FadeOut();

        GameSaveData data =
            GameSaveManager.GetSaveData();

        if (data.hasCheckpoint)
        {
            data.currentHealth =
                playerHealth.MaxHealth;

            SaveSystem.Save(data);

            SceneTransitionContext.EntryPointId =
                data.checkpointEntryPointId;

            Time.timeScale = 1f;

            SceneManager.LoadScene(
                data.checkpointSceneName
            );
        }
        else
        {
            // fallback if no checkpoint has ever been activated
            Time.timeScale = 1f;

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name
            );
        }

        yield break;
    }
}
```

---

# Part 23 — What happens after death?

I recommend this behavior:

```text
HP reaches 0
↓
fade to black
↓
checkpoint scene loads
↓
cat spawns beside basket
↓
HP = 4
↓
all enemies alive
↓
fade in
```

Why revive enemies on death as well?

Because reloading the scene naturally creates fresh enemy instances. Therefore:

```text
Death
→ scene reload
→ enemies return alive automatically
```

The explicit `RespawnAllEnemies()` system is primarily needed for:

```text
Cat Nap without changing scenes
```

---

# Part 24 — Save HP across normal room transitions

Your normal door transition should preserve the current HP.

Before changing scene, update save data with the player's current health.

I suggest adding to `RoomSaveController.SaveGame()`:

```csharp
data.currentHealth = playerHealth.CurrentHealth;
```

Then when entering a room normally:

```text
Room 3 at 2 HP
→ go to Room 4
→ arrive with 2 HP
```

But after Cat Nap:

```text
rest
→ health saved as 4
```

After death:

```text
respawn
→ health restored to 4
```

This gives understandable behavior.

---

# Part 25 — Restore HP on scene load

In the system responsible for restoring Player state:

```csharp
playerHealth.RestoreHealth(
    data.currentHealth
);
```

However, remember your existing entry priority:

```text
Door transition
→ entry point wins over saved position

Continue
→ saved exact position

Death
→ checkpoint entry point wins
```

So the complete priority should be:

```text
1. SceneTransitionContext entry exists?
   → spawn at that entry point

2. Otherwise loading Continue?
   → restore exact saved position

3. Otherwise
   → use scene default position
```

Health can be restored independently.

---

# Part 26 — Place the first enemy

Create:

```text
Enemies
└── MouseGuard_01
```

Place it approximately:

```text
7–10 tiles from entrance
```

The player must be able to see it before it detects the player.

Suggested initial tuning:

```text
Health: 3
Damage: 1
Patrol distance: 3–4 tiles
Detection distance: 4–5 tiles
Attack range: short
Attack cooldown: around 1.2–1.6 sec
```

Behavior:

```text
Patrol
↓
notice player
↓
short reaction
↓
approach
↓
attack wind-up
↓
attack
↓
recovery pause
↓
repeat
```

This is the exact teaching purpose of the first encounter in the room design. 

---

# Part 27 — Combat tutorial trigger

Before the first enemy's detection range:

```text
Gameplay
└── CombatTutorialTrigger
```

Reuse your tutorial trigger system.

Message:

```text
Claws out! Press [Attack] to strike.
```

It should appear once.

Do not pause combat.

The trigger should be before enemy aggro:

```text
P → tutorial → short space → E1
```

not:

```text
P → E1 attacks → tutorial finally appears
```

---

# Part 28 — Quiet section

After E1, leave:

```text
5–6 tiles
```

with no immediate enemy aggro.

Add visual props:

```text
guard stool
mouse lantern
tiny weapon rack
scratch marks
small mouse door
```

The room design specifically calls for this tension rhythm:

```text
fight
→ relief
→ anticipation
→ second fight
```

rather than continuous pressure. 

---

# Part 29 — Second enemy

Place:

```text
MouseGuard_02
```

in the final third.

Use the same enemy type.

Do not make it much stronger.

Recommended differences:

```text
same HP
same damage
slightly longer patrol
slightly larger detection range
```

This encounter should make the player think:

> I know how to fight this now.

---

# Part 30 — Reward after combat

The original room design recommends a small healing item. 

However, with your new checkpoint system, I would slightly change the reward.

Since the basket already restores full HP near the room entrance, the reward after both battles could be:

```text
small Cat Memory fragment
```

or:

```text
small single-HP food pickup
```

I prefer **1 HP food** for this room because it reinforces the new health system.

Behavior:

```text
player has 2/4 HP
↓
collect food
↓
3/4 HP
```

Do not let it exceed max HP.

You can add:

```csharp
public void Heal(int amount)
{
    if (isDead || amount <= 0)
    {
        return;
    }

    CurrentHealth = Mathf.Min(
        CurrentHealth + amount,
        maxHealth
    );

    HealthChanged?.Invoke(
        CurrentHealth,
        maxHealth
    );
}
```

Then the pickup calls:

```csharp
playerHealth.Heal(1);
```

---

# Part 31 — Cat Basket interaction polish

After the basic logic works, make Cat Nap feel special.

Suggested sequence:

```text
Press E
↓
player input locked
↓
cat animation: curl up / sleeping
↓
small Zzz particles
↓
0.8–1.2 sec
↓
HP icons refill
↓
enemies revive
↓
checkpoint saved
↓
player stands again
```

For the first implementation, the interaction can be immediate.

Later, add a nap animation.

---

# Part 32 — Make the active basket visually clear

Before activation:

```text
basket normal
```

After resting:

```text
basket has soft warm glow
tiny floating Z particles
small sparkle
```

That tells the player:

> this is my active checkpoint.

When a later basket becomes active, the previous basket does not necessarily need to lose all visual decoration, but it should no longer show the special **active checkpoint** effect.

Later, we can implement that through checkpoint IDs.

---

# Part 33 — Room transitions

Room 2 exit should become:

```text
Target Scene:
Room03_FirstCombat

Target Entry Point:
FromRoom02
```

Room 3 needs:

```text
Gameplay
└── Entry_FromRoom02
```

with:

```text
RoomEntryPoint
ID = FromRoom02
```

For backtracking, Room 3 also needs a left-side return trigger:

```text
ExitTrigger_ToRoom02
```

configured:

```text
Target Scene:
Room02_EscapeCorridor

Target Entry Point:
FromRoom03
```

And Room 2 needs:

```text
Entry_FromRoom03
```

with matching ID.

---

# Part 34 — Camera bounds

Create:

```text
Room03CameraBounds
```

Use the same working Cinemachine Confiner setup as Rooms 1 and 2.

The room is approximately:

```text
36 × 16 tiles
```

The camera should show:

* Enemy 1 before detection;
* the safe platform during Encounter 2;
* the exit-opening event.

The uploaded room plan specifically notes that the camera should not undermine combat pacing by showing every encounter too early. 

---

# Part 35 — Recommended implementation order

Do not implement everything simultaneously.

## Pass A — Room

```text
1. Duplicate Room 2
2. Paint Room 3 layout
3. Add safe platform
4. Add camera bounds
5. Test movement
```

## Pass B — Health

```text
6. Add PlayerHealth
7. Add 4-unit HUD
8. Test TakeDamage manually
9. Test HealFull
```

For testing, temporarily call:

```csharp
playerHealth.TakeDamage(1);
```

from a debug key or Inspector test—not from enemies yet.

## Pass C — Checkpoint

```text
10. Add basket
11. Add CheckpointSpawn
12. Implement E interaction
13. Heal to 4
14. Save checkpoint
```

Test:

```text
damage cat to 2 HP
→ rest
→ HUD becomes 4/4
```

## Pass D — Enemies

```text
15. Add EnemyHealth
16. Add MouseGuard_01
17. Add MouseGuard_02
18. Add EnemyRespawnable
19. Add EnemyRespawnManager
```

Test:

```text
kill both
→ rest at basket
→ both return alive
```

## Pass E — Progression

```text
20. Add EncounterManager
21. Add ExitBlocker
22. Kill E1 → door remains locked
23. Kill E2 → door opens
24. Rest → enemies return and door locks again
```

## Pass F — Death

```text
25. Add DeathRespawnController
26. Activate basket
27. Take 4 damage
28. Verify respawn near basket
29. Verify 4 HP
30. Verify enemies alive
```

Only after those work should you add final room decoration and polish.

---

# Final expected test scenario

This is the complete test I would perform:

```text
Start Room 2
↓
enter Room 3
↓
spawn at Entry_FromRoom02
↓
walk to basket
↓
Press E
↓
checkpoint activated
↓
4/4 HP
↓
fight MouseGuard_01
↓
lose 1 HP
↓
HUD shows 3 full + 1 empty
↓
kill E1
↓
fight E2
↓
lose remaining HP
↓
death
↓
fade out
↓
Room 3 reloads
↓
spawn beside basket
↓
HP 4/4
↓
E1 alive
↓
E2 alive
↓
fight again
↓
kill both
↓
door opens
↓
rest again
↓
both enemies revive
↓
door locks again
```

That test validates the entire new gameplay loop.

One design decision I would make now: **the first basket should be close to the entrance, but not automatically activated**. The player must deliberately Cat Nap in it. That makes the checkpoint mechanic understandable: finding a basket is not enough; resting is the action that establishes the respawn point.


- **Modifications manuelles** : reintération pour corriger le comportement d'ennemis
- **Décision d'intégration** : Accepté après modifications

- **Qualité estimée de la réponse** : 4
- **Hallucination détectée** : le comportement de l'ennemi ne correspondait à celui décrit par IA