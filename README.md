# Wizard Battle

A 2D turn-based wizard fighting game built in Unity (Universal 2D template) across 4 scenes.

## Overview & Architecture

* **Scene 1 (Title Screen)**: Camera with solid black background, TextMeshPro title (`A Sample Game`), and yellow `Start` button.
* **Scene 2 (Player Name Entry)**: Player name entry with Red and Blue `TMP_InputField` components and green `Play` button.
* **Scene 3 (Battle Arena)**: Arena displaying player names, HP counters (starting at 50), animated wizard characters rendered via `VideoPlayer` -> `RenderTexture` -> `RawImage`, and Red/Blue `Attack` buttons.
* **Scene 4 (Game Over)**: Centered TextMeshPro `Game Over` screen.

## Project Scripts (`Assets/Scripts/`)

* **`SceneOne.cs`**: Handles transition from Scene 1 to Scene 2.
* **`SceneTwo.cs`**: Persists player names across scenes and handles transition to Scene 3.
* **`SceneThree.cs`**: Manages battle arena health logic, dynamic attack video swapping, `loopPointReached` damage hooks, and transition to Game Over.
* **`Editor/WizardSceneGenerator.cs`**: One-click Unity Editor menu tool (**Tools > Wizard Game > Generate All Scenes & Setup Build Order**) that builds all 4 scenes, creates RenderTextures, and wires all button events and Inspector references.

## Portability & Import Options

### Method A: Full Unity Project
Open the repository root folder in **Unity 6 (6000.x)** via Unity Hub (**Add > Add project from disk**).

### Method B: Unity Package (`Wizard.unitypackage`)
Import the standalone package into any Unity project:
1. Open your Unity project.
2. Drag `Wizard.unitypackage` into the Project window (or choose **Assets > Import Package > Custom Package...**).
3. Click **Import All**.
