using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteraction : MonoBehaviour
{
    [Header("NPC Identity")]
    [SerializeField] private string npcName = "Bird Tarot Reader";

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite frontIdleSprite;
    [SerializeField] private Sprite speakingSideSpriteA;
    [SerializeField] private Sprite speakingSideSpriteB;

    [Header("Facing")]
    [Tooltip("Enable this if the side talking sprites naturally face right in the PNG.")]
    [SerializeField] private bool sideSpritesFaceRightByDefault = true;

    [Header("Interaction")]
    [SerializeField] private GameObject dialogIcon;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Speaking Animation")]
    [SerializeField] private float speakingSpriteSwapSpeed = 0.25f;

    [Header("Anti-Restart")]
    [SerializeField] private float interactionCooldownAfterDialogue = 0.4f;

    private bool playerNearby;
    private bool isSpeaking;
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

        if (isSpeaking)
        {
            return;
        }

        if (DialogueManager.IsDialogueBusy ||
            DialogueUIController.IsDialogueOpen)
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
            Interact();
        }
    }

    private void Interact()
    {
        if (dialogueManager == null)
        {
            Debug.LogError(
                "NPCInteraction: DialogueManager is not assigned.",
                this
            );

            return;
        }

        if (dialogIcon != null)
        {
            dialogIcon.SetActive(false);
        }

        FacePlayer();

        dialogueManager.StartGeneratedDialogue(
            npcName,
            this
        );
    }

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

    public void EndSpeaking()
    {
        StopSpeakingAnimation();
        SetIdleSprite();

        isSpeaking = false;

        nextAllowedInteractionTime =
            Time.unscaledTime + interactionCooldownAfterDialogue;

        if (playerNearby && dialogIcon != null)
        {
            dialogIcon.SetActive(true);
        }

        Debug.Log($"{npcName} stopped speaking.");
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
            // Sprite naturally faces right.
            // Player on right = no flip.
            // Player on left = flip.
            spriteRenderer.flipX = !playerIsOnRight;
        }
        else
        {
            // Sprite naturally faces left.
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerNearby = true;
        playerTransform = other.transform;

        if (dialogIcon != null && !isSpeaking)
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