using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GeminiDialogueService : MonoBehaviour
{
    [Header("Gemini API")]
    [SerializeField] private string apiKey;
    [SerializeField] private string modelName = "gemini-2.0-flash";

    [Header("Demo / Safety")]
    [SerializeField] private bool useFallbackOnly = false;
    [SerializeField] private bool cacheGeneratedDialogue = true;

    [Header("Prompt")]
    [TextArea(10, 30)]
    [SerializeField] private string prompt =
@"You are generating short NPC dialogue lines for a 2D cozy fairy-tale Metroidvania game called “Tiny Paws, Big Trouble”.

Game context:
The player controls a domestic cat who was kidnapped by mice and birds and wakes up in an unknown kingdom. The cat explores connected zones, unlocks powers such as Double Jump and Dash, fights mice and birds, meets NPCs, and searches for a way back home.

NPC:
A mysterious bird tarot reader living in a secret room. The bird is calm, mystical, poetic, slightly strange, but not evil. They speak in short symbolic phrases, like someone reading cards and omens.

Dialogue goal:
Generate atmospheric dialogue lines linked to the game world, the cat’s journey, the mouse kingdom, the bird faction, hidden paths, choices, danger, and the feeling of trying to return home.

Rules:
- Do NOT reveal the story endings.
- Do NOT reveal the final choice.
- Do NOT mention the poison bottle directly.
- Do NOT spoil the rebel mouse’s real intentions.
- Do NOT give exact solutions to quests.
- Keep the dialogue mysterious and spoiler-free.
- Keep each line short: maximum 1–2 sentences.
- Use a cozy but slightly eerie fairy-tale tone.
- The NPC should sound like a tarot reader.
- The dialogue should be suitable for an optional NPC.
- Do not mention being an AI.
- Do not break character.

Return exactly 8 dialogue lines.
Return only the dialogue lines, no explanations.";

    private string[] cachedDialogue;

    private readonly string[] fallbackDialogue =
    {
        "The spirits are quiet right now... return later."
    };

    public IEnumerator GenerateDialogue(Action<string[]> onSuccess)
    {
        if (useFallbackOnly)
        {
            Debug.LogWarning("GeminiDialogueService: Using fallback only.");
            onSuccess?.Invoke(fallbackDialogue);
            yield break;
        }

        if (cacheGeneratedDialogue &&
            cachedDialogue != null &&
            cachedDialogue.Length > 0)
        {
            Debug.Log("GeminiDialogueService: Using cached dialogue.");
            onSuccess?.Invoke(cachedDialogue);
            yield break;
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Debug.LogWarning("GeminiDialogueService: API key is missing.");
            onSuccess?.Invoke(fallbackDialogue);
            yield break;
        }

        modelName = modelName.Trim();

        string url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent";

        Debug.Log($"Gemini URL: {url}");
        Debug.Log($"Gemini Model Name: {modelName}");

        GeminiRequest requestData = new GeminiRequest
        {
            contents = new GeminiContent[]
            {
                new GeminiContent
                {
                    parts = new GeminiPart[]
                    {
                        new GeminiPart
                        {
                            text = prompt
                        }
                    }
                }
            }
        };

        string jsonBody = JsonUtility.ToJson(requestData);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-goog-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(
                $"GeminiDialogueService: API request failed. " +
                $"{request.responseCode} {request.error}\n" +
                $"{request.downloadHandler.text}"
            );

            if (request.responseCode == 429)
            {
                Debug.LogWarning(
                    "GeminiDialogueService: Rate limit or quota exceeded. Using fallback dialogue."
                );
            }

            onSuccess?.Invoke(fallbackDialogue);
            yield break;
        }

        string responseText = request.downloadHandler.text;
        string generatedText = ExtractGeneratedText(responseText);

        if (string.IsNullOrWhiteSpace(generatedText))
        {
            Debug.LogWarning("GeminiDialogueService: Empty generated text.");
            onSuccess?.Invoke(fallbackDialogue);
            yield break;
        }

        string[] lines = CleanDialogueLines(generatedText);

        if (lines == null || lines.Length == 0)
        {
            onSuccess?.Invoke(fallbackDialogue);
            yield break;
        }

        if (cacheGeneratedDialogue)
        {
            cachedDialogue = lines;
        }

        onSuccess?.Invoke(lines);
    }

    private string ExtractGeneratedText(string json)
    {
        try
        {
            GeminiResponse response =
                JsonUtility.FromJson<GeminiResponse>(json);

            if (response == null ||
                response.candidates == null ||
                response.candidates.Length == 0)
            {
                return null;
            }

            GeminiCandidate firstCandidate = response.candidates[0];

            if (firstCandidate.content == null ||
                firstCandidate.content.parts == null ||
                firstCandidate.content.parts.Length == 0)
            {
                return null;
            }

            return firstCandidate.content.parts[0].text;
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"GeminiDialogueService: Failed to parse response. {exception.Message}"
            );

            return null;
        }
    }

    private string[] CleanDialogueLines(string rawText)
    {
        string[] splitLines = rawText.Split(
            new[] { '\n', '\r' },
            StringSplitOptions.RemoveEmptyEntries
        );

        System.Collections.Generic.List<string> cleanedLines =
            new System.Collections.Generic.List<string>();

        foreach (string rawLine in splitLines)
        {
            string line = rawLine.Trim();

            line = line.TrimStart('-', '*', '•', ' ');
            line = RemoveNumberPrefix(line);
            line = line.Trim();
            line = line.Trim('"');

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            cleanedLines.Add(line);

            if (cleanedLines.Count >= 8)
            {
                break;
            }
        }

        if (cleanedLines.Count == 0)
        {
            return fallbackDialogue;
        }

        return cleanedLines.ToArray();
    }

    private string RemoveNumberPrefix(string line)
    {
        int dotIndex = line.IndexOf('.');

        if (dotIndex <= 0 || dotIndex > 3)
        {
            return line;
        }

        string possibleNumber = line.Substring(0, dotIndex);

        if (int.TryParse(possibleNumber, out _))
        {
            return line.Substring(dotIndex + 1).Trim();
        }

        return line;
    }

    [Serializable]
    private class GeminiRequest
    {
        public GeminiContent[] contents;
    }

    [Serializable]
    private class GeminiContent
    {
        public GeminiPart[] parts;
    }

    [Serializable]
    private class GeminiPart
    {
        public string text;
    }

    [Serializable]
    private class GeminiResponse
    {
        public GeminiCandidate[] candidates;
    }

    [Serializable]
    private class GeminiCandidate
    {
        public GeminiContent content;
    }
}