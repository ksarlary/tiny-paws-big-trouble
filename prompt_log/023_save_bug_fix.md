## Prompt Log — 22/07 — Konovalova Sofia — S2
- **Contexte** : Fix du bug du système de sauvegarde
- **Prompt** :
> when i started a new game for some reason i spawned at the room 2 instead of room 1

- **Modèle & outil** : ChatGPT GPT-5.5
- **Réponse / Output** : 
> Yes — this is almost certainly a **save / New Game reset issue**.

Your “New Game” probably starts by loading saved data, and your save file still contains:

```text
sceneName = Room02_EscapeCorridor
```

or an old entry point / player position from Room 2.

So when you click New Game, the game thinks:

```text
continue from last saved scene
```

instead of:

```text
start fresh from Room01_Prison
```

## 1. Quick test fix

Delete your save file once.

Use the path from earlier:

```csharp
Debug.Log(Application.persistentDataPath);
```

Open that folder and delete:

```text
save.json
```

Then start New Game again.

If you now spawn in Room 1, the bug is confirmed: New Game was using old save data.

---

# 2. Proper fix: New Game must delete old save

Find your Main Menu script. It may be called something like:

```text
MainMenuController
MenuController
MainMenuButtons
```

Find the method used by the New Game button. It may look like:

```csharp
public void StartNewGame()
{
    SceneManager.LoadScene("Room01_Prison");
}
```

Replace it with this safer version:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartNewGame()
    {
        SaveSystem.DeleteSave();

        SceneTransitionContext.Clear();

        SceneManager.LoadScene("Room01_Prison");
    }

    public void ContinueGame()
    {
        GameSaveData data = GameSaveManager.GetSaveData();

        if (data == null || string.IsNullOrEmpty(data.sceneName))
        {
            SceneManager.LoadScene("Room01_Prison");
            return;
        }

        SceneManager.LoadScene(data.sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
```

If your script already exists, only copy the logic inside `StartNewGame()`:

```csharp
SaveSystem.DeleteSave();
SceneTransitionContext.Clear();
SceneManager.LoadScene("Room01_Prison");
```

That is the important part.

---

# 3. Also reset transition context

This line is important:

```csharp
SceneTransitionContext.Clear();
```

Because if `SceneTransitionContext.EntryPointId` still contains something like:

```text
FromRoom03
FromRoom04
FromRoom05Secret
```

then the scene entry system may try to spawn the player at a wrong entry point.

For a fresh new game, we want:

```text
no saved scene
no saved position
no old entry point
start Room 1 normally
```

---

# 4. Check Room 1 scene name

Make sure the scene name is exactly:

```text
Room01_Prison
```

If your actual Unity scene is named differently, use the exact name from the Project window.

Example:

```csharp
SceneManager.LoadScene("Room01_Prison");
```

must match the `.unity` file name exactly.

---

# 5. Continue button should use save, New Game should not

Correct logic:

```text
New Game
→ delete old save
→ clear transition context
→ load Room01_Prison

Continue
→ load saved sceneName
→ restore saved position / checkpoint
```

Your bug happened because New Game is probably behaving too much like Continue.

---

# 6. Important after testing

After you fix it, test this sequence:

```text
1. Play game until Room 2.
2. Save / transition so save file stores Room 2.
3. Return to Main Menu.
4. Click Continue.
   Expected: Room 2.
5. Return to Main Menu.
6. Click New Game.
   Expected: Room 1.
```

If New Game still loads Room 2 after this, then your New Game button is not connected to the correct method in the Button `OnClick()` event.


- **Modifications manuelles** : reitération pour corriger les bugs produits par cette réponse
- **Décision d'intégration** : Accepté après modifications 

- **Qualité estimée de la réponse** : 2
- **Hallucination détectée** : Le bug initial a été corrigé, mais étant donné du contexte oublié, le nouveau fichier du controller n'avait pas prévu le fonctionnement des boutons du menu