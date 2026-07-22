## Prompt Log — 22/07 — Konovalova Sofia — S2
- **Contexte** : Script pour faire flotter le collectible
- **Prompt** :
> can we make the collectible float a little?

- **Modèle & outil** : ChatGPT GPT-5.5
- **Réponse / Output** : 
> Yes. For a memory collectible, I’d use a very subtle vertical bob plus optional slow rotation. Keep it gentle so it feels magical rather than like a coin pickup.

Create:

```text
Assets/_Project/Scripts/Collectibles/FloatingCollectible.cs
```

Use:

```csharp
using UnityEngine;

public class FloatingCollectible : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] private bool rotate = true;
    [SerializeField] private float rotationSpeed = 20f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float verticalOffset =
            Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.localPosition = startPosition +
            Vector3.up * verticalOffset;

        if (rotate)
        {
            transform.Rotate(
                0f,
                0f,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
```

Attach it to:

```text
MemoryCollectible_01
```

Recommended values for your game:

```text
Float Amplitude: 0.12
Float Speed: 1.8

Rotate: ON
Rotation Speed: 8
```

I would keep the rotation quite slow. A full constant spin at `40–60` would feel more arcade-like.

For the softest version:

```text
Float Amplitude: 0.10
Float Speed: 1.5
Rotation Speed: 5
```

One small note: because your memory popup sets `Time.timeScale = 0`, the collectible will also stop floating while the memory is open. That is perfectly fine. If you want it to continue moving during paused UI states, use `Time.unscaledTime` and `Time.unscaledDeltaTime` instead.

- **Modifications manuelles** : Aucune
- **Décision d'intégration** : Accepté tel quel

- **Qualité estimée de la réponse** : 5
- **Hallucination détectée** : Aucune