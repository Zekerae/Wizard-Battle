#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using TMPro;

namespace WizardGame.Editor
{
    public static class WizardSceneGenerator
    {
        private const string ScenesDir = "Assets/Scenes";
        private const string WizardAssetsDir = "Assets/wizard-assets";

        [MenuItem("Tools/Wizard Game/Generate All Scenes & Setup Build Order", priority = 100)]
        public static void GenerateAllScenes()
        {
            if (!AssetDatabase.IsValidFolder(ScenesDir))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            // Ensure RenderTextures exist for the wizards
            RenderTexture redRT = GetOrCreateRenderTexture("RedWizardRT", 512, 512);
            RenderTexture blueRT = GetOrCreateRenderTexture("BlueWizardRT", 512, 512);

            // Load Video Clips
            VideoClip redStance = AssetDatabase.LoadAssetAtPath<VideoClip>($"{WizardAssetsDir}/red_wizard_stance.mp4");
            VideoClip redAttack = AssetDatabase.LoadAssetAtPath<VideoClip>($"{WizardAssetsDir}/red_wizard_attack.mp4");
            VideoClip blueStance = AssetDatabase.LoadAssetAtPath<VideoClip>($"{WizardAssetsDir}/blue_wizard_stance.mp4");
            VideoClip blueAttack = AssetDatabase.LoadAssetAtPath<VideoClip>($"{WizardAssetsDir}/blue_wizard_attack.mp4");

            // Load Background Sprite/Texture
            Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{WizardAssetsDir}/wizard_background.png");
            Texture2D bgTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{WizardAssetsDir}/wizard_background.png");

            // 1. Build Scene1 (Title Screen)
            CreateScene1();

            // 2. Build Scene2 (Player Name Entry)
            CreateScene2();

            // 3. Build Scene3 (Battle Arena)
            CreateScene3(redRT, blueRT, redStance, redAttack, blueStance, blueAttack, bgSprite, bgTexture);

            // 4. Build Scene4 (Game Over)
            CreateScene4();

            // 5. Configure Build Profiles / EditorBuildSettings scene order
            SetupBuildSettings();

            // Open Scene1 so the user can immediately test
            EditorSceneManager.OpenScene($"{ScenesDir}/Scene1.unity");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Wizard Game Setup Complete",
                "Successfully generated all 4 scenes with wired UI, components, and configured Build Settings (indices 0 to 3)!\n\nScene1 is now open and ready to Play.",
                "Awesome!"
            );
            Debug.Log("<color=green>[WizardGame] All 4 scenes generated and Build Settings successfully configured!</color>");
        }

        private static void CreateScene1()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            Canvas canvas = CreateCanvas("Canvas");
            CreateEventSystem();

            // Title: "A Sample Game"
            GameObject titleGO = CreateTextObject(canvas.transform, "TitleText", "A Sample Game", 72, FontStyles.Bold, TextAlignmentOptions.Center);
            RectTransform titleRT = titleGO.GetComponent<RectTransform>();
            titleRT.anchoredPosition = new Vector2(0, 150);
            titleRT.sizeDelta = new Vector2(900, 140);

            // Yellow Start Button
            GameObject btnGO = CreateButton(canvas.transform, "StartButton", "Start", new Color(1f, 0.88f, 0.1f), Color.black, 32);
            RectTransform btnRT = btnGO.GetComponent<RectTransform>();
            btnRT.anchoredPosition = new Vector2(0, -60);
            btnRT.sizeDelta = new Vector2(240, 70);

            // ScriptGO with SceneOne
            GameObject scriptGO = new GameObject("ScriptGO");
            SceneOne sceneOne = scriptGO.AddComponent<SceneOne>();

            // Persistent OnClick listener
            Button button = btnGO.GetComponent<Button>();
            UnityEventTools.AddPersistentListener(button.onClick, sceneOne.NextScene);

            EditorUtility.SetDirty(scriptGO);
            EditorUtility.SetDirty(btnGO);
            EditorSceneManager.SaveScene(scene, $"{ScenesDir}/Scene1.unity");
        }

        private static void CreateScene2()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            Canvas canvas = CreateCanvas("Canvas");
            CreateEventSystem();

            // Screen Header
            GameObject headerGO = CreateTextObject(canvas.transform, "Header", "Enter Player Names", 56, FontStyles.Bold, TextAlignmentOptions.Center);
            RectTransform headerRT = headerGO.GetComponent<RectTransform>();
            headerRT.anchoredPosition = new Vector2(0, 250);
            headerRT.sizeDelta = new Vector2(800, 100);

            // Red Player Label & InputField
            GameObject redLabel = CreateTextObject(canvas.transform, "RedLabel", "Red Player Name:", 28, FontStyles.Bold, TextAlignmentOptions.Center);
            redLabel.GetComponent<TMP_Text>().color = new Color(1f, 0.4f, 0.4f);
            RectTransform redLabelRT = redLabel.GetComponent<RectTransform>();
            redLabelRT.anchoredPosition = new Vector2(-280, 120);
            redLabelRT.sizeDelta = new Vector2(350, 40);

            TMP_InputField redInput = CreateInputField(canvas.transform, "RedInputField", "Enter Red Name...", "Red Wizard");
            RectTransform redInputRT = redInput.GetComponent<RectTransform>();
            redInputRT.anchoredPosition = new Vector2(-280, 50);
            redInputRT.sizeDelta = new Vector2(350, 60);

            // Blue Player Label & InputField
            GameObject blueLabel = CreateTextObject(canvas.transform, "BlueLabel", "Blue Player Name:", 28, FontStyles.Bold, TextAlignmentOptions.Center);
            blueLabel.GetComponent<TMP_Text>().color = new Color(0.4f, 0.7f, 1f);
            RectTransform blueLabelRT = blueLabel.GetComponent<RectTransform>();
            blueLabelRT.anchoredPosition = new Vector2(280, 120);
            blueLabelRT.sizeDelta = new Vector2(350, 40);

            TMP_InputField blueInput = CreateInputField(canvas.transform, "BlueInputField", "Enter Blue Name...", "Blue Wizard");
            RectTransform blueInputRT = blueInput.GetComponent<RectTransform>();
            blueInputRT.anchoredPosition = new Vector2(280, 50);
            blueInputRT.sizeDelta = new Vector2(350, 60);

            // Green Play Button
            GameObject playBtnGO = CreateButton(canvas.transform, "PlayButton", "Play", new Color(0.18f, 0.8f, 0.44f), Color.white, 32);
            RectTransform playBtnRT = playBtnGO.GetComponent<RectTransform>();
            playBtnRT.anchoredPosition = new Vector2(0, -140);
            playBtnRT.sizeDelta = new Vector2(240, 70);

            // ScriptGO with SceneTwo
            GameObject scriptGO = new GameObject("ScriptGO");
            SceneTwo sceneTwo = scriptGO.AddComponent<SceneTwo>();
            sceneTwo.redInputName = redInput;
            sceneTwo.blueInputName = blueInput;

            // Persistent OnClick listener
            Button playBtn = playBtnGO.GetComponent<Button>();
            UnityEventTools.AddPersistentListener(playBtn.onClick, sceneTwo.NextScene);

            EditorUtility.SetDirty(scriptGO);
            EditorUtility.SetDirty(playBtnGO);
            EditorSceneManager.SaveScene(scene, $"{ScenesDir}/Scene2.unity");
        }

        private static void CreateScene3(
            RenderTexture redRT,
            RenderTexture blueRT,
            VideoClip redStance,
            VideoClip redAttack,
            VideoClip blueStance,
            VideoClip blueAttack,
            Sprite bgSprite,
            Texture2D bgTexture)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            Canvas canvas = CreateCanvas("Canvas");
            CreateEventSystem();

            // Background display
            if (bgSprite != null || bgTexture != null)
            {
                GameObject bgGO = new GameObject("Background");
                bgGO.transform.SetParent(canvas.transform, false);
                if (bgSprite != null)
                {
                    Image bgImg = bgGO.AddComponent<Image>();
                    bgImg.sprite = bgSprite;
                }
                else
                {
                    RawImage bgRaw = bgGO.AddComponent<RawImage>();
                    bgRaw.texture = bgTexture;
                }
                RectTransform bgRT = bgGO.GetComponent<RectTransform>();
                bgRT.anchorMin = Vector2.zero;
                bgRT.anchorMax = Vector2.one;
                bgRT.sizeDelta = Vector2.zero;
            }

            // Video Players (Scene GameObjects)
            GameObject redVidGO = new GameObject("RedVideoPlayer");
            VideoPlayer redVP = redVidGO.AddComponent<VideoPlayer>();
            redVP.renderMode = VideoRenderMode.RenderTexture;
            redVP.targetTexture = redRT;
            redVP.clip = redStance;
            redVP.isLooping = true;
            redVP.playOnAwake = true;

            GameObject blueVidGO = new GameObject("BlueVideoPlayer");
            VideoPlayer blueVP = blueVidGO.AddComponent<VideoPlayer>();
            blueVP.renderMode = VideoRenderMode.RenderTexture;
            blueVP.targetTexture = blueRT;
            blueVP.clip = blueStance;
            blueVP.isLooping = true;
            blueVP.playOnAwake = true;

            // RawImages to display VideoPlayers in Canvas
            GameObject redRawGO = new GameObject("RedWizardRawImage");
            redRawGO.transform.SetParent(canvas.transform, false);
            RawImage redRaw = redRawGO.AddComponent<RawImage>();
            redRaw.texture = redRT;
            RectTransform redRawRT = redRawGO.GetComponent<RectTransform>();
            redRawRT.anchoredPosition = new Vector2(-420, -30);
            redRawRT.sizeDelta = new Vector2(480, 480);

            GameObject blueRawGO = new GameObject("BlueWizardRawImage");
            blueRawGO.transform.SetParent(canvas.transform, false);
            RawImage blueRaw = blueRawGO.AddComponent<RawImage>();
            blueRaw.texture = blueRT;
            RectTransform blueRawRT = blueRawGO.GetComponent<RectTransform>();
            blueRawRT.anchoredPosition = new Vector2(420, -30);
            blueRawRT.sizeDelta = new Vector2(480, 480);

            // Red UI: Name & HP
            GameObject redNameGO = CreateTextObject(canvas.transform, "RedPlayerNameText", "Red", 36, FontStyles.Bold, TextAlignmentOptions.Center);
            redNameGO.GetComponent<TMP_Text>().color = new Color(1f, 0.4f, 0.4f);
            RectTransform redNameRT = redNameGO.GetComponent<RectTransform>();
            redNameRT.anchoredPosition = new Vector2(-420, 270);
            redNameRT.sizeDelta = new Vector2(350, 50);

            GameObject redHPGO = CreateTextObject(canvas.transform, "RedHPText", "50", 42, FontStyles.Bold, TextAlignmentOptions.Center);
            redHPGO.GetComponent<TMP_Text>().color = new Color(1f, 0.85f, 0.2f);
            RectTransform redHPRT = redHPGO.GetComponent<RectTransform>();
            redHPRT.anchoredPosition = new Vector2(-420, 220);
            redHPRT.sizeDelta = new Vector2(200, 50);

            // Blue UI: Name & HP
            GameObject blueNameGO = CreateTextObject(canvas.transform, "BluePlayerNameText", "Blue", 36, FontStyles.Bold, TextAlignmentOptions.Center);
            blueNameGO.GetComponent<TMP_Text>().color = new Color(0.4f, 0.7f, 1f);
            RectTransform blueNameRT = blueNameGO.GetComponent<RectTransform>();
            blueNameRT.anchoredPosition = new Vector2(420, 270);
            blueNameRT.sizeDelta = new Vector2(350, 50);

            GameObject blueHPGO = CreateTextObject(canvas.transform, "BlueHPText", "50", 42, FontStyles.Bold, TextAlignmentOptions.Center);
            blueHPGO.GetComponent<TMP_Text>().color = new Color(1f, 0.85f, 0.2f);
            RectTransform blueHPRT = blueHPGO.GetComponent<RectTransform>();
            blueHPRT.anchoredPosition = new Vector2(420, 220);
            blueHPRT.sizeDelta = new Vector2(200, 50);

            // Red Attack Button
            GameObject redAtkBtnGO = CreateButton(canvas.transform, "RedAttackButton", "Red Attack", new Color(0.85f, 0.2f, 0.2f), Color.white, 24);
            RectTransform redAtkBtnRT = redAtkBtnGO.GetComponent<RectTransform>();
            redAtkBtnRT.anchoredPosition = new Vector2(-420, -320);
            redAtkBtnRT.sizeDelta = new Vector2(220, 60);

            // Blue Attack Button
            GameObject blueAtkBtnGO = CreateButton(canvas.transform, "BlueAttackButton", "Blue Attack", new Color(0.2f, 0.45f, 0.85f), Color.white, 24);
            RectTransform blueAtkBtnRT = blueAtkBtnGO.GetComponent<RectTransform>();
            blueAtkBtnRT.anchoredPosition = new Vector2(420, -320);
            blueAtkBtnRT.sizeDelta = new Vector2(220, 60);

            // ScriptGO with SceneThree
            GameObject scriptGO = new GameObject("ScriptGO");
            SceneThree sceneThree = scriptGO.AddComponent<SceneThree>();
            sceneThree.redGameName = redNameGO.GetComponent<TMP_Text>();
            sceneThree.blueGameName = blueNameGO.GetComponent<TMP_Text>();
            sceneThree.redHPText = redHPGO.GetComponent<TMP_Text>();
            sceneThree.blueHPText = blueHPGO.GetComponent<TMP_Text>();

            sceneThree.redStance = redStance;
            sceneThree.redAttack = redAttack;
            sceneThree.blueStance = blueStance;
            sceneThree.blueAttack = blueAttack;

            sceneThree.redVidPlayer = redVP;
            sceneThree.blueVidPlayer = blueVP;

            sceneThree.redHP = 50;
            sceneThree.blueHP = 50;

            // Persistent Button Listeners
            Button redBtn = redAtkBtnGO.GetComponent<Button>();
            Button blueBtn = blueAtkBtnGO.GetComponent<Button>();
            UnityEventTools.AddPersistentListener(redBtn.onClick, sceneThree.AttackRed);
            UnityEventTools.AddPersistentListener(blueBtn.onClick, sceneThree.AttackBlue);

            EditorUtility.SetDirty(scriptGO);
            EditorUtility.SetDirty(redAtkBtnGO);
            EditorUtility.SetDirty(blueAtkBtnGO);
            EditorSceneManager.SaveScene(scene, $"{ScenesDir}/Scene3.unity");
        }

        private static void CreateScene4()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            Canvas canvas = CreateCanvas("Canvas");
            CreateEventSystem();

            // Centered Game Over Text
            GameObject goTextGO = CreateTextObject(canvas.transform, "GameOverText", "Game Over", 88, FontStyles.Bold, TextAlignmentOptions.Center);
            goTextGO.GetComponent<TMP_Text>().color = new Color(0.95f, 0.25f, 0.25f);
            RectTransform goTextRT = goTextGO.GetComponent<RectTransform>();
            goTextRT.anchoredPosition = Vector2.zero;
            goTextRT.sizeDelta = new Vector2(800, 160);

            EditorUtility.SetDirty(goTextGO);
            EditorSceneManager.SaveScene(scene, $"{ScenesDir}/Scene4.unity");
        }

        private static void SetupBuildSettings()
        {
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene($"{ScenesDir}/Scene1.unity", true),
                new EditorBuildSettingsScene($"{ScenesDir}/Scene2.unity", true),
                new EditorBuildSettingsScene($"{ScenesDir}/Scene3.unity", true),
                new EditorBuildSettingsScene($"{ScenesDir}/Scene4.unity", true)
            };
            EditorBuildSettings.scenes = scenes;
        }

        private static Camera CreateCamera()
        {
            GameObject camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            Camera cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            camGO.transform.position = new Vector3(0, 0, -10);
            return cam;
        }

        private static Canvas CreateCanvas(string name)
        {
            GameObject canvasGO = new GameObject(name);
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static void CreateEventSystem()
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();

            var inputSystemType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemType != null)
            {
                esGO.AddComponent(inputSystemType);
            }
            else
            {
                esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }

        private static GameObject CreateTextObject(Transform parent, string name, string text, float fontSize, FontStyles style, TextAlignmentOptions alignment)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.alignment = alignment;
            tmp.color = Color.white;
            return go;
        }

        private static GameObject CreateButton(Transform parent, string name, string labelText, Color btnColor, Color textColor, float fontSize)
        {
            GameObject btnGO = new GameObject(name);
            btnGO.transform.SetParent(parent, false);
            Image img = btnGO.AddComponent<Image>();
            img.color = btnColor;
            btnGO.AddComponent<Button>();

            GameObject textGO = new GameObject("Text (TMP)");
            textGO.transform.SetParent(btnGO.transform, false);
            TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = labelText;
            tmp.fontSize = fontSize;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = textColor;

            RectTransform textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;

            return btnGO;
        }

        private static TMP_InputField CreateInputField(Transform parent, string name, string placeholderText, string defaultText)
        {
            GameObject inputGO = new GameObject(name);
            inputGO.transform.SetParent(parent, false);

            Image bg = inputGO.AddComponent<Image>();
            bg.color = new Color(0.18f, 0.18f, 0.22f, 1f);

            // Text Viewport Area
            GameObject textAreaGO = new GameObject("Text Area");
            textAreaGO.transform.SetParent(inputGO.transform, false);
            RectTransform textAreaRT = textAreaGO.AddComponent<RectTransform>();
            textAreaRT.anchorMin = Vector2.zero;
            textAreaRT.anchorMax = Vector2.one;
            textAreaRT.sizeDelta = new Vector2(-20, -10);
            textAreaGO.AddComponent<RectMask2D>();

            // Placeholder
            GameObject placeholderGO = new GameObject("Placeholder");
            placeholderGO.transform.SetParent(textAreaGO.transform, false);
            TextMeshProUGUI placeholderTMP = placeholderGO.AddComponent<TextMeshProUGUI>();
            placeholderTMP.text = placeholderText;
            placeholderTMP.fontSize = 22;
            placeholderTMP.fontStyle = FontStyles.Italic;
            placeholderTMP.color = new Color(0.6f, 0.6f, 0.6f, 0.65f);
            placeholderTMP.alignment = TextAlignmentOptions.MidlineLeft;
            RectTransform placeholderRT = placeholderGO.GetComponent<RectTransform>();
            placeholderRT.anchorMin = Vector2.zero;
            placeholderRT.anchorMax = Vector2.one;
            placeholderRT.sizeDelta = Vector2.zero;

            // Input Text
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(textAreaGO.transform, false);
            TextMeshProUGUI textTMP = textGO.AddComponent<TextMeshProUGUI>();
            textTMP.text = defaultText;
            textTMP.fontSize = 22;
            textTMP.color = Color.white;
            textTMP.alignment = TextAlignmentOptions.MidlineLeft;
            RectTransform textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;

            // TMP_InputField Setup
            TMP_InputField inputField = inputGO.AddComponent<TMP_InputField>();
            inputField.textViewport = textAreaRT;
            inputField.textComponent = textTMP;
            inputField.placeholder = placeholderTMP;
            inputField.text = defaultText;

            return inputField;
        }

        private static RenderTexture GetOrCreateRenderTexture(string name, int width, int height)
        {
            string path = $"{WizardAssetsDir}/{name}.renderTexture";
            RenderTexture rt = AssetDatabase.LoadAssetAtPath<RenderTexture>(path);
            if (rt == null)
            {
                rt = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
                AssetDatabase.CreateAsset(rt, path);
            }
            return rt;
        }
    }
}
#endif
