using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static bool IsDialogueBusy { get; private set; }

    [Header("References")]
    [SerializeField] private DialogueUIController dialogueUI;
    [SerializeField] private GroqDialogueService groqDialogueService;

    [Header("Loading Dialogue")]
    [SerializeField] private string loadingSpeakerName = "Bird Tarot Reader";
    [SerializeField] private string loadingLine = "The cards are turning...";

    [Header("Fallback")]
    [SerializeField] private string fallbackLine =
        "The spirits are quiet right now... return later.";

    public void StartGeneratedDialogue(
        string speakerName,
        NPCInteraction npcInteraction)
    {
        if (IsDialogueBusy || DialogueUIController.IsDialogueOpen)
        {
            return;
        }

        StartCoroutine(
            GeneratedDialogueRoutine(
                speakerName,
                npcInteraction
            )
        );
    }

    private IEnumerator GeneratedDialogueRoutine(
        string speakerName,
        NPCInteraction npcInteraction)
    {
        IsDialogueBusy = true;

        if (npcInteraction != null)
        {
            npcInteraction.BeginSpeaking();
        }

        if (dialogueUI != null)
        {
            dialogueUI.StartSingleLineDialogue(
                loadingSpeakerName,
                loadingLine
            );
        }

        while (DialogueUIController.IsDialogueOpen)
        {
            yield return null;
        }

        string[] generatedLines = null;

        if (groqDialogueService != null)
        {
            yield return groqDialogueService.GenerateDialogue(
                lines =>
                {
                    generatedLines = lines;
                }
            );
        }
        else
        {
            Debug.LogWarning(
                "DialogueManager: GroqDialogueService is not assigned."
            );
        }

        string finalLine = GetFirstValidLine(generatedLines);

        if (dialogueUI != null)
        {
            dialogueUI.StartSingleLineDialogue(
                speakerName,
                finalLine,
                () =>
                {
                    FinishDialogue(npcInteraction);
                }
            );
        }
        else
        {
            FinishDialogue(npcInteraction);
        }
    }

    private string GetFirstValidLine(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            return fallbackLine;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(lines[i]))
            {
                return lines[i].Trim();
            }
        }

        return fallbackLine;
    }

    private void FinishDialogue(NPCInteraction npcInteraction)
    {
        if (npcInteraction != null)
        {
            npcInteraction.EndSpeaking();
        }

        IsDialogueBusy = false;

        Debug.Log("Generated NPC dialogue fully finished.");
    }
}