## Prompt Log — 05/07/2026 — Konovalova Sofia — S1
- **Contexte** : Génération de première chambre du jeu
- **Prompt** :
> Act as a professional 2D Metroidvania level designer and Unity greybox designer.
Design the first playable room of a 2D Metroidvania called “Tiny Paws, Big Trouble”.
Game context
The player controls a small domestic cat who has been kidnapped by mice and birds. The cat wakes up inside a prison cell in an unfamiliar underground mouse kingdom.
This room is the very beginning of the game and the player's first interactive experience after the intro cinematic.
The player currently has only the basic abilities:
move left
move right
basic jump
The player does NOT yet have:
Double Jump
Dash
Wall Jump
combat abilities
movement upgrades
These abilities will be unlocked later and must not be required in this room.
Room purpose
The room must:
introduce left and right movement naturally
introduce basic jumping through one or two very simple obstacles
immediately communicate that the cat is imprisoned
establish the underground mouse kingdom atmosphere
guide the player naturally from the spawn point toward the exit
contain no enemies
contain no hazards
be safe and forgiving for a beginner
avoid confusing alternate routes
work as a short tutorial without feeling like a formal tutorial room
be suitable for greyboxing before final art is created
The player should understand where to go mainly through:
level geometry
lighting direction
prop placement
framing
contrast
environmental storytelling
Avoid relying only on tutorial text.
Technical constraints
Design for:
Unity 2D
2D side-view platformer
orthographic camera
tile-based greybox layout
approximately 25–30 tiles wide
approximately 12–16 tiles high
one main playable path
one elevated ledge
one simple jump challenge
one clearly readable exit door
solid floor and walls
no moving platforms
no hazards
no enemies
no spikes
no bottomless pits
no breakable floor
no puzzle that blocks progression
no Double Jump required
no Dash required
no precision platforming
no jump longer than a beginner player can comfortably understand and perform
Assume the basic jump height is approximately 3 tiles.
Keep all required jumps compatible with that limit.
Level-design requirements
The room should have a clear left-to-right flow.
Suggested progression:
The player starts inside the prison area on the left side.
The immediate surroundings make it clear that this is a cell or detention room.
The player has a small safe area where they can test movement.
A simple low obstacle encourages the first jump.
A slightly elevated platform or ledge reinforces jumping without requiring precision.
The level geometry naturally directs the player toward the right side.
The exit door is visible or partially hinted at early enough to become the player's visual goal.
The final approach to the exit should be simple and safe.
The player must never become trapped.
Avoid layouts where the player can fall into an area they cannot leave with only the basic jump.
Environmental storytelling direction
The visual atmosphere should match a cozy fairy-tale adventure with light tension.
The prison should feel mysterious and slightly uncomfortable, but not horror-like.
Possible environmental elements:
oversized mouse-built wooden beams
stone walls with rounded shapes
a crude wooden prison gate
rope
small mouse pawprints
scratched wall marks
a fallen feather suggesting bird involvement
a small food bowl
scattered straw
wooden crates
broken cage pieces
mouse-sized doors or tunnels that are too small for the cat
dim cool ambient light
one warmer light source near the direction of the exit
Do not overcrowd the room.
Props should support navigation and storytelling without making the greybox difficult to read.
Unity design considerations
The result must be easy to recreate using basic Unity GameObjects and Tilemaps.
Prefer:
Grid
Tilemap for walls and platforms
BoxCollider2D
CompositeCollider2D if appropriate
empty GameObjects as spawn points
trigger collider for tutorial messages
trigger collider for exit transition
simple SpriteRenderer placeholders for props
Cinemachine camera bounds if relevant
Do not propose complex systems or scripts unless absolutely necessary.
Required output format
Return the result in exactly this structure:
Room purpose
Explain the gameplay and narrative purpose of the room in one concise paragraph.
Recommended dimensions
Give:
room width in tiles
room height in tiles
approximate playable floor area
suggested spawn position
suggested exit position
Step-by-step player flow
Describe the player's route from spawn to exit in numbered steps.
For each step, explain:
what the player sees
what action they are expected to perform
what mechanic they learn
how the level visually guides them
ASCII layout with legend
Create a clear ASCII layout representing the full room.
Use a readable legend such as:
P = Player spawn
= Solid wall or floor
= Platform
L = Elevated ledge
D = Exit door
T = Tutorial trigger
C = Crate or simple obstacle
R = Rope or prison prop
F = Feather clue
M = Mouse tunnel or mouse-related environmental clue
. = Empty space
The ASCII layout must:
reflect the proposed room dimensions approximately
show a clear left-to-right route
remain physically playable using only basic movement and a 3-tile jump
avoid impossible gaps
avoid dead ends
show the elevated ledge and exit clearly
List of Unity GameObjects needed
List the GameObjects required to greybox the room.
For each GameObject, specify:
suggested name
purpose
important component(s)
Example:
PlayerSpawn — starting position — Empty GameObject
GroundTilemap — walls and floor — Tilemap, TilemapCollider2D
ExitTrigger — load next room — BoxCollider2D set as Trigger
Keep the list practical and minimal.
Suggested tutorial messages
Suggest a small number of short tutorial messages.
Rules:
maximum 3 tutorial messages
keep them short
do not display all messages immediately
mention where each message trigger should be placed
avoid overly formal wording
Environmental storytelling ideas
Suggest 5–8 small environmental details that communicate:
the cat was kidnapped
mice control this area
birds may also be involved
the cat is far from home
the room is a prison
the environment is mysterious but not horror-like
Greybox validation checklist
Add a short checklist to verify:
the room can be completed with only basic jump
no jump exceeds the intended jump height
player cannot become trapped
exit is visually understandable
tutorial messages appear at the correct time
collision boundaries are correct
camera framing does not reveal unwanted areas
the room remains readable without final art
Keep the design simple, readable, beginner-friendly, and realistic to implement in Unity 2D.
Prioritize good player guidance, safe onboarding, and clear progression over visual complexity.

- **Modèle & outil** : ChatGPT GPT-5.5
- **Réponse / Output** : 
> # Room purpose

This room acts as a short, invisible onboarding sequence: the player regains control inside a clearly readable prison space, safely experiments with horizontal movement, performs a one-tile obstacle jump, then reinforces the same skill on a broad two-tile-high ledge before reaching the exit. The geometry creates a single left-to-right route, while a distant warm light and visible door establish the immediate objective. Environmental clues introduce the kidnapping and the mouse–bird alliance without interrupting gameplay, supporting the project's broader story and its intended “cozy adventure with light tension” atmosphere.  

# Recommended dimensions

* **Room width:** 28 tiles
* **Room height:** 14 tiles
* **Approximate playable floor area:** about 23 horizontal tiles of continuous ground traversal, plus a 6-tile-wide raised ledge
* **Suggested spawn position:** approximately **(3, 2)**, using the bottom-left corner as the tile-grid origin
* **Suggested exit position:** approximately **(25, 2)**, on the far-right wall

The camera should initially frame roughly the left two-thirds of the room while still allowing the warm lighting around the exit area to be visible at the right edge of the screen. The actual door can become fully centered after the player begins moving.

The room should require only:

* one jump over a **1-tile-high crate**
* one jump onto a **2-tile-high ledge**
* one safe **2-tile drop** from the ledge

All required vertical movement therefore remains below the assumed 3-tile maximum jump height.

# Step-by-step player flow

1. **Wake-up and orientation**

   **What the player sees:**
   The cat begins near the left wall, surrounded by scattered straw, a food bowl, rope, and the remains of a crude wooden prison enclosure. The left side is visually enclosed and darker. On the far right, a warm light source silhouettes the exit door.

   **Expected action:**
   The player experiments with moving left and right.

   **Mechanic learned:**
   Basic horizontal movement.

   **Visual guidance:**
   The player quickly discovers that the left side offers no progression. The larger open space, floor composition, lighting gradient, and visible exit framing all point toward the right.

2. **Safe movement space**

   **What the player sees:**
   A short section of unobstructed floor extends ahead. Mouse pawprints and the direction of a loose rope subtly form a visual line toward the room's right side.

   **Expected action:**
   Walk toward the center of the room.

   **Mechanic learned:**
   Basic control responsiveness, stopping, and changing direction.

   **Visual guidance:**
   Props should be arranged horizontally rather than randomly. The floor remains uncluttered around the player's feet so that the playable surface is immediately readable.

3. **First jump: the crate**

   **What the player sees:**
   A single 1-tile-high crate blocks the direct path.

   **Expected action:**
   Jump over the crate.

   **Mechanic learned:**
   Basic jump timing and horizontal movement while airborne.

   **Visual guidance:**
   The crate should be isolated, with clear empty space before and after it. Its silhouette must visibly communicate that it is an obstacle rather than part of the background.

   The landing zone should be several tiles wide, making failure harmless. A failed jump simply returns the player to the ground in front of the crate.

4. **Jump reinforcement: the raised ledge**

   **What the player sees:**
   A broad raised section approximately 6 tiles wide and 2 tiles above the main floor. The upper surface continues toward the brighter side of the room.

   **Expected action:**
   Jump onto the ledge, cross it, and step off its right side.

   **Mechanic learned:**
   Jumping onto elevated geometry and understanding safe vertical traversal.

   **Visual guidance:**
   The ledge should have a clear top silhouette. A hanging lantern, brighter wall patch, or stronger warm rim light can visually emphasize its right-facing direction.

   Because the ledge is broad, the challenge is understanding the jump rather than executing a precise landing.

5. **Safe descent and narrative clues**

   **What the player sees:**
   After stepping down from the ledge, the player finds a fallen bird feather and a mouse-sized tunnel built into the wall near the route to the exit.

   **Expected action:**
   Continue walking right.

   **Mechanic learned:**
   No new mechanic is introduced here. This section gives the player a brief pause after the movement tutorial.

   **Visual guidance:**
   The floor is flat and uninterrupted. The door now occupies a strong position in the camera frame, while the warm lighting becomes noticeably stronger.

6. **Exit approach**

   **What the player sees:**
   A large cat-sized door or opened prison gate, contrasting with the much smaller mouse tunnel nearby.

   **Expected action:**
   Walk into the exit trigger.

   **Mechanic learned:**
   The player understands that large doors and illuminated passages represent room transitions.

   **Visual guidance:**
   The door receives the strongest local contrast in the room. The space directly before it remains free of clutter, enemies, jumps, or additional interactions.

# ASCII layout with legend

The diagram is approximately **28 tiles wide × 14 tiles high**.

```text
############################
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#..........................#
#........................D.#
#.............**L***.....D.#
#..P.RT..C..T.######.F.M.D.#
############################
############################
```

**Legend:**

```text
P = Player spawn
# = Solid wall or floor
* = Top surface of raised platform
L = Elevated ledge area
D = Exit door
T = Tutorial trigger
C = Crate or simple obstacle
R = Rope or prison prop
F = Fallen feather clue
M = Mouse tunnel or mouse-related environmental clue
. = Empty space
```

The intended traversal sequence is:

```text
Spawn
  ↓
[P] → movement space → [C]
                       jump
                         ↓
                 flat landing area
                         ↓
                 jump onto [L]
                         ↓
                   walk across
                         ↓
                    safe drop
                         ↓
                  [F] → [M] → [D]
```

The raised ledge is supported by solid tiles beneath it, so the player cannot bypass the intended jump by walking underneath it. Its top is only 2 tiles above the normal standing level, remaining comfortably within the 3-tile jump limit.

There are no pits or enclosed lower areas anywhere in the route.

# List of Unity GameObjects needed

| Suggested name             | Purpose                                                                   | Important component(s)                                 |
| -------------------------- | ------------------------------------------------------------------------- | ------------------------------------------------------ |
| `Room01_Prison`            | Root object for the room hierarchy                                        | Transform                                              |
| `Grid`                     | Parent grid for tile-based geometry                                       | Grid                                                   |
| `GroundTilemap`            | Ceiling, walls, floor, crate support geometry, and raised ledge           | Tilemap, TilemapRenderer, TilemapCollider2D            |
| `GroundCollision`          | Efficient combined collision if the tile count becomes significant        | CompositeCollider2D, Static Rigidbody2D                |
| `PlayerSpawn`              | Exact player starting position                                            | Empty GameObject / Transform                           |
| `CrateObstacle`            | First simple jump obstacle                                                | SpriteRenderer placeholder, BoxCollider2D              |
| `TutorialTrigger_Movement` | Displays the first control hint                                           | BoxCollider2D set as Trigger                           |
| `TutorialTrigger_Jump`     | Displays jump guidance before the ledge section                           | BoxCollider2D set as Trigger                           |
| `ExitDoor`                 | Visible navigation goal                                                   | SpriteRenderer placeholder                             |
| `ExitTrigger`              | Starts transition to the next room                                        | BoxCollider2D set as Trigger                           |
| `PropsRoot`                | Parent for rope, bowl, straw, feather, pawprints, and tunnel placeholders | Empty GameObject with child SpriteRenderers            |
| `PrisonGatePlaceholder`    | Visually establishes the starting area as a cell                          | SpriteRenderer placeholder                             |
| `RoomCamera`               | Follows the player                                                        | Cinemachine camera setup                               |
| `CameraBounds`             | Prevents the camera from revealing outside the room                       | Collider shape appropriate for Cinemachine confinement |

For the greybox, the environment can be separated into three visual layers:

```text
Background       → stone wall silhouettes and distant tunnel shapes
Gameplay layer   → floor, walls, crate, raised ledge, door
Foreground       → optional bars, rope ends, or beam silhouettes
```

Only the gameplay layer needs collision.

The project already uses Unity 2D and C# as its main development stack, making this Tilemap-and-trigger-based structure consistent with the existing technical direction. 

# Suggested tutorial messages

1. **“A / D or ← / → to move”**
   Place the trigger approximately 2–3 tiles to the right of the spawn point. It should appear only after the player has gained control rather than immediately during the cinematic transition.

2. **“Space to jump”**
   Place the trigger shortly before the crate or between the crate and the raised ledge. It should disappear after the player successfully jumps or after a short duration.

3. **“That door... maybe a way out.”**
   Place the trigger after the player safely steps down from the raised ledge. This is primarily a narrative thought rather than a mechanical instruction.

The room should remain understandable even when all three messages are disabled during testing.

# Environmental storytelling ideas

1. **Broken prison gate pieces near the spawn**
   A crude mouse-built wooden gate or cage frame immediately communicates imprisonment. One section can be bent or poorly repaired, explaining why the cat can leave the immediate cell area without requiring a puzzle.

2. **A cat-sized food bowl with mouse-sized portions**
   The bowl suggests that the cat has been held here for some time, while the absurdly small amount of food adds a slightly playful tone rather than making the scene disturbing.

3. **Mouse pawprints crossing the room**
   A short trail can begin near the prison area and disappear into the small mouse tunnel. This identifies who controls the underground structure and simultaneously provides subtle directional composition.

4. **A single fallen bird feather near the exit route**
   Its placement suggests that the mice are not acting alone. The feather should be noticeable but not presented as a collectible or required interaction.

5. **Scratched wall marks near the cell**
   A few rounded, stylized scratch marks suggest previous attempts to escape. Keep them cute and exaggerated rather than violent or horror-like.

6. **Oversized wooden beams held together with mouse-scale construction**
   Thick wooden supports can contrast with tiny knots, ropes, pegs, and miniature access doors. This establishes that the kingdom has been physically adapted by much smaller inhabitants.

7. **A tiny mouse door beside the cat-sized exit**
   The miniature tunnel reinforces the scale difference between the cat and its captors. It also suggests a larger civilization beyond the room without creating an actual alternate route.

8. **Lighting transition from cool lavender shadow to warm yellow glow**
   The prison side can use subdued lavender and brown-grey values, while the exit direction receives soft Butter Yellow and Soft Vanilla highlights. This follows the project's established palette without turning the underground prison into a horror environment. 

# Greybox validation checklist

* [ ] Complete the entire room using only left movement, right movement, and basic jump.
* [ ] Verify that the crate requires no more than a 1-tile vertical jump.
* [ ] Verify that the raised ledge is no more than 2 tiles above the normal standing level.
* [ ] Test the ledge jump from a standing start as well as with normal running movement.
* [ ] Confirm that missing a jump never causes damage, death, or a soft lock.
* [ ] Confirm that the player cannot enter any area that requires Double Jump, Dash, or Wall Jump to escape.
* [ ] Confirm that the raised ledge cannot accidentally be bypassed through collision gaps.
* [ ] Verify that the player can safely step down from the right side of the ledge.
* [ ] Confirm that the exit door is noticeable from the initial or early camera framing.
* [ ] Verify that the first tutorial trigger does not activate before the player gains control.
* [ ] Verify that the jump message appears before the player needs the corresponding action.
* [ ] Check all TilemapCollider2D edges for unwanted seams or snag points.
* [ ] Check the crate collider for clean takeoff and landing behavior.
* [ ] Verify that the exit trigger cannot activate accidentally through a wall or platform collider.
* [ ] Confirm that camera confinement prevents empty space outside the room from becoming visible.
* [ ] Disable all tutorial text and verify that geometry, lighting, framing, and prop direction still communicate the route.
* [ ] Replace all props with simple monochrome blocks and verify that the main path remains readable.
* [ ] Test the room with a first-time player and confirm that they reach the exit without being told where to go.


- **Modifications manuelles** : Non
- **Décision d'intégration** : Accepté tel quel

- **Qualité estimée de la réponse** : 5
- **Hallucination détectée** : Non