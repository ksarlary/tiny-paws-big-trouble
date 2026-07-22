using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueUIController : MonoBehaviour
{
    public static bool IsDialogueOpen { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialogueRoot;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text continueHintText;

    [Header("Input")]
    [SerializeField] private float inputCooldown = 0.2f;

    private string[] currentLines;
    private int currentIndex;
    private Action onDialogueFinished;
    private float nextAllowedInputTime;

    private bool forceCloseOnNextInput;

    private void Awake()
    {
        CloseImmediate();
    }

    private void Update()
    {
        if (!IsDialogueOpen)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Time.unscaledTime < nextAllowedInputTime)
        {
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame ||
            Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            nextAllowedInputTime = Time.unscaledTime + inputCooldown;
            ShowNextLineOrClose();
        }
    }

    public void StartDialogue(
        string speakerName,
        string[] lines,
        Action onFinished = null)
    {
        if (IsDialogueOpen)
        {
            return;
        }

        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("DialogueUIController: Tried to start empty dialogue.");
            return;
        }

        forceCloseOnNextInput = false;

        currentLines = lines;
        currentIndex = 0;
        onDialogueFinished = onFinished;

        IsDialogueOpen = true;
        Time.timeScale = 0f;

        nextAllowedInputTime = Time.unscaledTime + inputCooldown;

        if (dialogueRoot != null)
        {
            dialogueRoot.SetActive(true);
        }

        if (speakerNameText != null)
        {
            speakerNameText.text = speakerName;
        }

        if (continueHintText != null)
        {
            continueHintText.text = lines.Length <= 1
                ? "Press E to close"
                : "Press E to continue";
        }

        ShowCurrentLine();
    }

    public void StartSingleLineDialogue(
        string speakerName,
        string line,
        Action onFinished = null)
    {
        if (IsDialogueOpen)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(line))
        {
            line = "...";
        }

        forceCloseOnNextInput = true;

        currentLines = new[] { line };
        currentIndex = 0;
        onDialogueFinished = onFinished;

        IsDialogueOpen = true;
        Time.timeScale = 0f;

        nextAllowedInputTime = Time.unscaledTime + inputCooldown;

        if (dialogueRoot != null)
        {
            dialogueRoot.SetActive(true);
        }

        if (speakerNameText != null)
        {
            speakerNameText.text = speakerName;
        }

        if (continueHintText != null)
        {
            continueHintText.text = "Press E to close";
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (dialogueText == null)
        {
            return;
        }

        if (currentLines == null)
        {
            return;
        }

        if (currentIndex < 0 || currentIndex >= currentLines.Length)
        {
            return;
        }

        dialogueText.text = currentLines[currentIndex];
    }

    private void ShowNextLineOrClose()
    {
        if (forceCloseOnNextInput)
        {
            EndDialogue();
            return;
        }

        currentIndex++;

        if (currentIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        Action finishedCallback = onDialogueFinished;

        CloseImmediate();

        onDialogueFinished = null;

        finishedCallback?.Invoke();
    }

    private void CloseImmediate()
    {
        IsDialogueOpen = false;
        Time.timeScale = 1f;

        forceCloseOnNextInput = false;

        if (dialogueRoot != null)
        {
            dialogueRoot.SetActive(false);
        }

        currentLines = null;
        currentIndex = 0;
    }
}