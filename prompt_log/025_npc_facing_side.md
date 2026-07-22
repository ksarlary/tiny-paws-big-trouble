## Prompt Log — 22/07 — Konovalova Sofia — S2
- **Contexte** : Faire le NPC regarder le joueur quand ils parlent
- **Prompt** :
> can we make the reader face the player side when talking?

- **Modèle & outil** : ChatGPT GPT-5.5
- **Réponse / Output** : 
> Yes. Your `NPCInteraction.cs` already has the logic for facing the player, but we need to make sure the **visual hierarchy and Inspector settings** are correct.

Since your bird has:

```text
front idle sprite = when not talking
side talking sprite A/B = when talking
```

we want:

```text
Idle:
Bird uses front sprite

Talking:
Bird switches to side sprites
Bird flips left/right depending on player position
```

## 1. Recommended Bird hierarchy

Use this structure:

```text
BirdTarotReaderNPC
├── Visual
│   └── BodySprite
├── Glow
└── DialogIcon
```

Where:

```text
BirdTarotReaderNPC = root with NPCInteraction + BoxCollider2D trigger
Visual = object that flips left/right
BodySprite = object with SpriteRenderer
Glow = glow behind bird
DialogIcon = interaction icon
```

If your `SpriteRenderer` is directly on the root, it can still work, but the cleanest setup is with a `Visual` child.

---

## 2. Set `NPCInteraction` references

Select:

```text
BirdTarotReaderNPC
```

In `NPCInteraction`, assign:

```text
NPC Name = Bird Tarot Reader

Sprite Renderer = BodySprite SpriteRenderer
Front Idle Sprite = bird front idle sprite
Speaking Side Sprite A = bird side talk 01
Speaking Side Sprite B = bird side talk 02

Visual Root = Visual
Side Sprites Face Right By Default = true or false

Dialog Icon = DialogIcon
Dialogue Manager = DialogueSystems / DialogueManager
```

Important:

```text
Visual Root should be Visual
not BirdTarotReaderNPC root
```

This lets the bird sprite flip without flipping the trigger, icon, and glow weirdly.

---

## 3. Choose the correct facing setting

Look at your bird side-talking sprite.

If the bird side sprite faces right by default, set:

```text
Side Sprites Face Right By Default = true
```

If the bird side sprite faces left by default, set:

```text
Side Sprites Face Right By Default = false
```

From your earlier bird assets, it probably faces right, so start with:

```text
true
```

Test it. If it turns away from the player, simply switch it to:

```text
false
```

---

## 4. Make sure your `NPCInteraction.cs` has this `FacePlayer()` method

Open `NPCInteraction.cs` and check that this method exists:

```csharp
private void FacePlayer()
{
    if (playerTransform == null || visualRoot == null)
    {
        return;
    }

    bool playerIsOnRight =
        playerTransform.position.x > transform.position.x;

    Vector3 scale = visualRoot.localScale;
    float absX = Mathf.Abs(originalVisualScale.x);

    if (sideSpritesFaceRightByDefault)
    {
        scale.x = playerIsOnRight ? absX : -absX;
    }
    else
    {
        scale.x = playerIsOnRight ? -absX : absX;
    }

    visualRoot.localScale = scale;
}
```

Also make sure `BeginSpeaking()` calls it:

```csharp
public void BeginSpeaking()
{
    isSpeaking = true;

    if (dialogIcon != null)
    {
        dialogIcon.SetActive(false);
    }

    FacePlayer();
    StartSpeakingAnimation();

    Debug.Log($"{npcName} started speaking.");
}
```

And inside the speaking coroutine, make sure it keeps facing the player:

```csharp
private IEnumerator SpeakingAnimationRoutine()
{
    bool useFirstSprite = true;

    while (isSpeaking)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite =
                useFirstSprite
                    ? speakingSideSpriteA
                    : speakingSideSpriteB;
        }

        FacePlayer();

        useFirstSprite = !useFirstSprite;

        yield return new WaitForSecondsRealtime(
            speakingSpriteSwapSpeed
        );
    }
}
```

---

## 5. Make sure idle returns to front view

When the dialogue ends, it should return to the front sprite.

Check this method:

```csharp
private void SetIdleSprite()
{
    if (spriteRenderer != null && frontIdleSprite != null)
    {
        spriteRenderer.sprite = frontIdleSprite;
    }

    if (visualRoot != null)
    {
        visualRoot.localScale = originalVisualScale;
    }
}
```

This means:

```text
not talking = front sprite
talking = side sprite facing player
```

---

## 6. Quick test

Press Play.

Test from both sides:

```text
1. Stand on the right side of the bird.
2. Press E.
3. Bird should turn right toward the player.

4. End dialogue.
5. Bird returns to front view.

6. Stand on the left side of the bird.
7. Press E.
8. Bird should turn left toward the player.
```

If the bird faces the wrong direction while talking, only change this field:

```text
Side Sprites Face Right By Default
```

Switch `true ↔ false`.


- **Modifications manuelles** : Aucune
- **Décision d'intégration** : Accepté tel quel

- **Qualité estimée de la réponse** : 5
- **Hallucination détectée** : Aucune