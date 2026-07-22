## Prompt Log — 22/07 — Konovalova Sofia — S2
- **Contexte** : création de prompt pour générer des phrases de dialogue pour le NPC
- **Prompt** :
> write a prompt for gemini api to get some phrases linked to the setting of our game and the story/world. the npc is a bird tarot reader. it shouldnt reveal the story endings

- **Modèle & outil** : ChatGPT GPT-5.5
- **Réponse / Output** : 
> Tiny English polish: **“Write a prompt for the Gemini API to generate some lines connected to our game’s setting, story, and world.”**

Here’s a good **Gemini API prompt** for your bird tarot reader NPC:

```text
You are generating short NPC dialogue lines for a 2D cozy fairy-tale Metroidvania game called “Tiny Paws, Big Trouble”.

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
Return only the dialogue lines, no explanations.
```

Example expected output style:

```text
“The cards tremble when tiny paws walk through borrowed shadows.”
“Not every locked door is closed by wood. Some are closed by fear.”
“The moon sees both the nest and the burrow, little wanderer.”
```

For Unity, I’d also add a stricter format so parsing is easier:

```text
Return the answer as a JSON array of strings only.
Example:
[
  "Line 1",
  "Line 2"
]
```

So the final API-friendly ending can be:

```text
Return exactly 8 dialogue lines as a JSON array of strings only. No markdown, no numbering, no explanations.
```


- **Modifications manuelles** :
- **Décision d'intégration** : Accepté tel quel

- **Qualité estimée de la réponse** : 5
- **Hallucination détectée** :