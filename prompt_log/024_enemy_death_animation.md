## Prompt Log — 22/07 — Konovalova Sofia — S2
- **Contexte** : Rajout de l'animation pour la défaite des ennemis (en plusieurs prompts)
- **Prompt** :
> so the guard took hits and died (without dying animation though, only disappeared quickly) and there is still no red flashes
now it flashes but there is no death animation
when he died the sprite fell down outside of the map

- **Modèle & outil** : ChatGPT GPT-5.5
- **Réponse / Output** : 
> Yes — that happened because in the death logic we disabled the enemy collider:

```csharp
enemyCollider.enabled = false;
```

but the enemy still has a `Rigidbody2D` affected by gravity. So after death:

```text
Collider disabled
↓
Mouse no longer stands on the floor
↓
Rigidbody2D falls through the map
```

We need to freeze the enemy during death.

## Fix: stop physics during death

In `EnemyHealth.cs`, add these fields:

```csharp
private Rigidbody2D rb;
private float originalGravityScale;
private RigidbodyType2D originalBodyType;
```

In `Awake()`, add:

```csharp
rb = GetComponent<Rigidbody2D>();

if (rb != null)
{
    originalGravityScale = rb.gravityScale;
    originalBodyType = rb.bodyType;
}
```

So your `Awake()` should include:

```csharp
private void Awake()
{
    currentHealth = maxHealth;

    if (spriteRenderer != null)
    {
        originalColor = spriteRenderer.color;
    }

    if (animator == null)
    {
        animator = GetComponentInChildren<Animator>();
    }

    if (enemyCollider == null)
    {
        enemyCollider = GetComponent<Collider2D>();
    }

    if (mouseGuardAI == null)
    {
        mouseGuardAI = GetComponent<MouseGuardAI>();
    }

    rb = GetComponent<Rigidbody2D>();

    if (rb != null)
    {
        originalGravityScale = rb.gravityScale;
        originalBodyType = rb.bodyType;
    }
}
```

---

## Update the death routine

In `DeathRoutine()`, replace this part:

```csharp
Rigidbody2D rb = GetComponent<Rigidbody2D>();

if (rb != null)
{
    rb.linearVelocity = Vector2.zero;
}
```

with this:

```csharp
if (rb != null)
{
    rb.linearVelocity = Vector2.zero;
    rb.angularVelocity = 0f;
    rb.gravityScale = 0f;
    rb.bodyType = RigidbodyType2D.Kinematic;
}
```

Then keep:

```csharp
if (enemyCollider != null)
{
    enemyCollider.enabled = false;
}
```

Now the enemy will stop in place, play the death animation, then disappear.

---

## Update `ResetHealth()`

Because your checkpoint/basket respawns enemies, we also need to restore physics.

In `ResetHealth()`, add this before `gameObject.SetActive(true);`:

```csharp
if (rb != null)
{
    rb.bodyType = originalBodyType;
    rb.gravityScale = originalGravityScale;
    rb.linearVelocity = Vector2.zero;
    rb.angularVelocity = 0f;
}
```

Your reset section should contain:

```csharp
if (enemyCollider != null)
{
    enemyCollider.enabled = true;
}

if (mouseGuardAI != null)
{
    mouseGuardAI.enabled = true;
}

if (rb != null)
{
    rb.bodyType = originalBodyType;
    rb.gravityScale = originalGravityScale;
    rb.linearVelocity = Vector2.zero;
    rb.angularVelocity = 0f;
}

gameObject.SetActive(true);
```

---

## Also check the death animation clip

If the sprite still moves/falls visually after this, then the problem is inside `MouseGuard_Death.anim`.

Open the animation and check if it recorded unwanted Transform properties.

In the Animation window, look for properties like:

```text
Transform.Position
Transform.Rotation
Transform.Scale
```

For a simple sprite death animation, you usually only want:

```text
Sprite Renderer.Sprite
```

or maybe:

```text
Sprite Renderer.Color
```

If you see `Transform.Position Y` going downward, delete that property from the death clip.

To delete it:

```text
Open MouseGuard_Death.anim
↓
Animation window
↓
Find Transform.Position
↓
Right-click property
↓
Remove Property
```

---

## Best setup for now

Use this behavior:

```text
Mouse HP reaches 0
↓
AI disabled
↓
velocity set to 0
↓
gravity disabled
↓
body becomes Kinematic
↓
collider disabled
↓
death animation plays in place
↓
mouse disappears
```

That will stop the guard from falling through the map during death.


- **Modifications manuelles** : reitération pour avoir le résultat voulu
- **Décision d'intégration** : Accepté après modifications

- **Qualité estimée de la réponse** : 3
- **Hallucination détectée** : a oublié plein de détails, l'ennemi est tombé à travers la map