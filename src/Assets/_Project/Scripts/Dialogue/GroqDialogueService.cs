using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GroqDialogueService : MonoBehaviour
{
    [Header("Groq API")]
    [SerializeField] private string apiKey;

    [Tooltip("Example: llama-3.1-8b-instant, llama-3.3-70b-versatile")]
    [SerializeField] private string modelName = "llama-3.1-8b-instant";

    [Header("Generation Settings")]
    [SerializeField] private float temperature = 0.9f;
    [SerializeField] private int maxTokens = 350;

    [Header("Demo / Safety")]
    [SerializeField] private bool useFallbackOnly = false;
    [SerializeField] private bool cacheGeneratedDialogue = true;

    [Header("Prompt")]
    [TextArea(10, 30)]
    [SerializeField]
    private string prompt =
@"You are generating one short NPC dialogue line for a 2D cozy fairy-tale Metroidvania game called “Tiny Paws, Big Trouble”.

Game context:
The player controls a small domestic cat who was kidnapped by mice and birds and wakes up far from home in an underground kingdom. The cat explores prison tunnels, cheese caves, hidden rooms, old shelters, mouse passages, bird nests, and forgotten shortcuts. The cat has learned Double Jump and is slowly finding a way back home.

NPC:
A mysterious bird tarot reader living in a secret room. The bird is calm, mystical, poetic, slightly strange, but not evil. They speak like they are reading cards, feathers, pawprints, and small omens.

Dialogue goal:
Generate ONE atmospheric optional NPC line. The line should feel like a small tarot reading or cryptic hint about the cat’s journey.

Vary the topic. Choose ONE of these themes:
- the cat trying to return home
- hidden paths above the normal road
- the mouse kingdom watching from below
- birds carrying secrets between rooms
- a warning about trusting too quickly
- courage found in small paws
- the meaning of Double Jump as rising above fate
- old doors, lost keys, and forgotten routes
- the comfort of warm light in dangerous places
- the cat being braver than they think

Rules:
- Do NOT reveal story endings.
- Do NOT reveal the final choice.
- Do NOT mention the poison bottle directly.
- Do NOT spoil the rebel mouse’s real intentions.
- Do NOT give exact quest solutions.
- Do NOT use the words: shadow, shadows, whisper, whispers.
- Avoid generic spooky phrases.
- Make it specific to this game world.
- Keep the line short: maximum 1–2 sentences.
- Cozy, mysterious, fairy-tale tone.
- The NPC should sound like a tarot reader.
- Do not mention being an AI.
- Do not break character.

Return exactly 1 dialogue line.
Return only the dialogue line, no explanations.";

    private string[] cachedDialogue;

    private readonly string[] fallbackDialogue =
    {
        "The spirits are quiet right now... return later."
    };

    public IEnumerator GenerateDialogue(Action<string[]> onSuccess)
    {
        if (useFallbackOnly)
        {
            Debug.LogWarning("GroqDialogueService: Using fallback only.");
            onSuccess?.Invoke(fallbackDialogue);
            yield break;
        }

        if (cacheGeneratedDialogue &&
            cachedDialogue != null &&
            cachedDialogue.Length > 0)
        {
            Debug.Log("GroqDialogueService: Using cached dialogue.");
            onSuccess?.Invoke(cachedDialogue);
            yield break;
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Debug.LogWarning("GroqDialogueService: API key is missing.");
            onSuccess?.Invoke(fallbackDialogue);
            yield break;
        }

        modelName = modelName.Trim();

        string url = "https://api.groq.com/openai/v1/chat/completions";

        GroqRequest requestData = new GroqRequest
        {
            model = modelName,
            messages = new GroqMessage[]
            {
                new GroqMessage
                {
                    role = "user",
                    content = prompt
                }
            },
            temperature = temperature,
            max_tokens = maxTokens
        };

        string jsonBody = JsonUtility.ToJson(requestData);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        Debug.Log($"Groq URL: {url}");
        Debug.Log($"Groq Model Name: {modelName}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(
                $"GroqDialogueService: API request failed. " +
                $"{request.responseCode} {request.error}\n" +
                $"{request.downloadHandler.text}"
            );

            onSuccess?.Invoke(fallbackDialogue);
            yield break;
        }

        string responseText = request.downloadHandler.text;
        string generatedText = ExtractGeneratedText(responseText);

        if (string.IsNullOrWhiteSpace(generatedText))
        {
            Debug.LogWarning("GroqDialogueService: Empty generated text.");
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
            GroqResponse response =
                JsonUtility.FromJson<GroqResponse>(json);

            if (response == null ||
                response.choices == null ||
                response.choices.Length == 0)
            {
                return null;
            }

            GroqChoice firstChoice = response.choices[0];

            if (firstChoice.message == null)
            {
                return null;
            }

            return firstChoice.message.content;
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"GroqDialogueService: Failed to parse response. {exception.Message}"
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

            if (cleanedLines.Count >= 1)
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
    private class GroqRequest
    {
        public string model;
        public GroqMessage[] messages;
        public float temperature;
        public int max_tokens;
    }

    [Serializable]
    private class GroqMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    private class GroqResponse
    {
        public GroqChoice[] choices;
    }

    [Serializable]
    private class GroqChoice
    {
        public GroqMessage message;
    }
}