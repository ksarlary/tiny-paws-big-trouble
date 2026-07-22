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

    [Header("Facing")]
    [Tooltip("Enable this if the Old Cat side talking sprites naturally face right in the PNG.")]
    [SerializeField] private bool sideSpritesFaceRightByDefault = true;

    [Header("Interaction")]
    [SerializeField] private GameObject dialogIcon;
    [SerializeField] private DialogueUIController dialogueUI;
    [SerializeField] private PlayerAbilities playerAbilities;

    [Header("Dialogue")]
    [SerializeField] private string speakerName = "Old Cat";

    [TextArea]
    [SerializeField] private string[] firstDialogueLines =
    {
        "Wait, little one...",
        "You carry the smell of the tunnels.",
        "The mice brought you here, did they not?",
        "Do not trust every mouse who smiles at you.",
        "Some are frightened. Some are kind.",
        "But some still serve the Mouse King.",
        "If you want to survive, you must learn to reach higher places.",
        "Feel the ground beneath your paws. Jump once...",
        "Then trust yourself, and jump again.",
        "You learned Double Jump. Press jump again while in the air."
    };

    [TextArea]
    [SerializeField] private string[] repeatDialogueLines =
    {
        "Remember, little one...",
        "The tunnels hide more than cheese and dust.",
        "Use your new jump wisely."
    };

    [Header("Speaking Animation")]
    [SerializeField] private float speakingSpriteSwapSpeed = 0.25f;

    [Header("Anti-Restart")]
    [SerializeField] private float interactionCooldownAfterDialogue = 0.4f;

    private bool playerNearby;
    private bool isTalking;
    private float nextAllowedInteractionTime;

    private Transform playerTransform;
    private Coroutine speakingAnimationCoroutine;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (dialogIcon != null)
        {
            dialogIcon.SetActive(false);
        }

        SetIdleSprite();
    }

    private void Update()
    {
        if (!playerNearby)
        {
            return;
        }

        if (isTalking)
        {
            return;
        }

        if (DialogueUIController.IsDialogueOpen)
        {
            return;
        }

        if (Time.unscaledTime < nextAllowedInteractionTime)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartOldCatDialogue();
        }
    }

    private void StartOldCatDialogue()
    {
        if (dialogueUI == null)
        {
            Debug.LogError(
                "OldCatNPC: DialogueUIController is not assigned.",
                this
            );

            return;
        }

        isTalking = true;

        if (dialogIcon != null)
        {
            dialogIcon.SetActive(false);
        }

        FacePlayer();
        StartSpeakingAnimation();

        string[] linesToUse =
            GameSaveManager.HasDoubleJump()
                ? repeatDialogueLines
                : firstDialogueLines;

        dialogueUI.StartDialogue(
            speakerName,
            linesToUse,
            OnDialogueFinished
        );

        Debug.Log("Old Cat dialogue started.");
    }

    private void OnDialogueFinished()
    {
        StopSpeakingAnimation();
        SetIdleSprite();

        if (!GameSaveManager.HasDoubleJump())
        {
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

        nextAllowedInteractionTime =
            Time.unscaledTime + interactionCooldownAfterDialogue;

        if (playerNearby && dialogIcon != null)
        {
            dialogIcon.SetActive(true);
        }

        Debug.Log("Old Cat dialogue finished.");
    }

    private void FacePlayer()
    {
        if (playerTransform == null || spriteRenderer == null)
        {
            return;
        }

        bool playerIsOnRight =
            playerTransform.position.x > transform.position.x;

        if (sideSpritesFaceRightByDefault)
        {
            // Side sprites naturally face right.
            // Player on right = no flip.
            // Player on left = flip.
            spriteRenderer.flipX = !playerIsOnRight;
        }
        else
        {
            // Side sprites naturally face left.
            // Player on right = flip.
            // Player on left = no flip.
            spriteRenderer.flipX = playerIsOnRight;
        }
    }

    private void SetIdleSprite()
    {
        if (spriteRenderer != null && frontIdleSprite != null)
        {
            spriteRenderer.sprite = frontIdleSprite;
            spriteRenderer.flipX = false;
        }
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

        while (isTalking)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerNearby = true;
        playerTransform = other.transform;

        if (playerAbilities == null)
        {
            playerAbilities = other.GetComponent<PlayerAbilities>();
        }

        if (dialogIcon != null && !isTalking)
        {
            dialogIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerNearby = false;

        if (other.transform == playerTransform)
        {
            playerTransform = null;
        }

        if (dialogIcon != null)
        {
            dialogIcon.SetActive(false);
        }
    }
}