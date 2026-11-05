using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Menu item: Skill Check > Build Menus
// Generates MainMenu.unity, GameOver.unity and Resources/PauseMenu.prefab from the team's
// sprites and puts MainMenu first in the build order. Safe to run again; it overwrites
// those three assets, so tweak layouts here (or tweak by hand and stop re-running it).
public static class MenuSceneBuilder
{
    private const string ButtonsSheet = "Assets/Sprites/Buttons.png";
    private const string WindowsSheet = "Assets/Sprites/Windows for menu.png";
    private const string MainMenuArt = "Assets/Sprites/MAINMENUPAGE.png";
    private const string GameOverArt = "Assets/Sprites/GameOver.png";

    private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
    private const string GameOverScenePath = "Assets/Scenes/GameOver.unity";
    private const string ResourcesFolder = "Assets/Resources";
    private const string PausePrefabPath = "Assets/Resources/PauseMenu.prefab";

    private static readonly Vector2 ReferenceResolution = new Vector2(1920, 1080);
    private static readonly Color InkPurple = new Color32(58, 26, 94, 255);

    // Pink = normal, cyan = pressed/hovered (from Buttons.png).
    private struct ButtonArt
    {
        public Sprite normal;
        public Sprite pressed;
    }

    private static ButtonArt menuArt, exitArt, startArt, playArt, pauseArt;
    private static Sprite pauseWindow;

    [MenuItem("Skill Check/Build Menus")]
    public static void BuildAll()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        if (!LoadArt())
            return;

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        BuildPausePrefab();
        BuildMainMenuScene();
        BuildGameOverScene();
        UpdateBuildSettings();

        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene(MainMenuScenePath);
        Debug.Log("Skill Check menus built: MainMenu, GameOver and Resources/PauseMenu. MainMenu is now first in the build order.");
    }

    // ---------- Art ----------

    private static bool LoadArt()
    {
        bool ok = true;
        menuArt = new ButtonArt { normal = Slice(ButtonsSheet, "Buttons_0", ref ok), pressed = Slice(ButtonsSheet, "Buttons_2", ref ok) };
        exitArt = new ButtonArt { normal = Slice(ButtonsSheet, "Buttons_1", ref ok), pressed = Slice(ButtonsSheet, "Buttons_3", ref ok) };
        startArt = new ButtonArt { normal = Slice(ButtonsSheet, "Buttons_4", ref ok), pressed = Slice(ButtonsSheet, "Buttons_6", ref ok) };
        playArt = new ButtonArt { normal = Slice(ButtonsSheet, "Buttons_8", ref ok), pressed = Slice(ButtonsSheet, "Buttons_10", ref ok) };
        pauseArt = new ButtonArt { normal = Slice(ButtonsSheet, "Buttons_9", ref ok), pressed = Slice(ButtonsSheet, "Buttons_11", ref ok) };
        pauseWindow = Slice(WindowsSheet, "Windows for menu_0", ref ok);

        if (AssetDatabase.LoadAssetAtPath<Texture2D>(MainMenuArt) == null || AssetDatabase.LoadAssetAtPath<Texture2D>(GameOverArt) == null)
        {
            Debug.LogError("Menu builder: MAINMENUPAGE.png or GameOver.png is missing from Assets/Sprites.");
            ok = false;
        }
        return ok;
    }

    private static Sprite Slice(string sheetPath, string spriteName, ref bool ok)
    {
        Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(sheetPath)
            .OfType<Sprite>()
            .FirstOrDefault(s => s.name == spriteName);
        if (sprite == null)
        {
            Debug.LogError($"Menu builder: sprite '{spriteName}' not found in {sheetPath}. Was the sheet re-sliced?");
            ok = false;
        }
        return sprite;
    }

    // ---------- Pause menu prefab ----------

    private static void BuildPausePrefab()
    {
        if (!AssetDatabase.IsValidFolder(ResourcesFolder))
            AssetDatabase.CreateFolder("Assets", "Resources");

        Canvas canvas = CreateCanvas("PauseMenu", 100);
        GameObject root = canvas.gameObject;
        PauseMenu pauseMenu = root.AddComponent<PauseMenu>();

        // Corner pause button, visible during play.
        Button pauseButton = CreateSpriteButton("PauseButton", root.transform, pauseArt,
            new Vector2(1, 1), new Vector2(-80, -80), new Vector2(110, 110));
        UnityEventTools.AddPersistentListener(pauseButton.onClick, new UnityAction(pauseMenu.Pause));

        // Overlay shown while paused.
        RectTransform panel = CreateRect("PausePanel", root.transform);
        Stretch(panel);

        RectTransform dim = CreateRect("Dim", panel);
        Stretch(dim);
        dim.gameObject.AddComponent<Image>().color = new Color(0.08f, 0.02f, 0.15f, 0.7f);

        RectTransform window = CreateRect("Window", panel);
        Image windowImage = window.gameObject.AddComponent<Image>();
        windowImage.sprite = pauseWindow;
        windowImage.SetNativeSize();
        // The window art is ~968x633 with a drop shadow on the right/bottom.

        TMP_Text title = CreateText("Title", window, "PAUSED", 64, Color.white, TextAlignmentOptions.MidlineLeft);
        Place(title.rectTransform, new Vector2(0, 1), new Vector2(330, -56), new Vector2(560, 100));
        title.fontStyle = FontStyles.Bold;

        TMP_Text stats = CreateText("Stats", window, "Lives: 3     Score: 0", 46, InkPurple, TextAlignmentOptions.Center);
        Place(stats.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(-10, 70), new Vector2(860, 80));

        Button resume = CreateSpriteButton("ResumeButton", window, playArt,
            new Vector2(0.5f, 0.5f), new Vector2(-320, -90), new Vector2(140, 140));
        Button menu = CreateSpriteButton("MenuButton", window, menuArt,
            new Vector2(0.5f, 0.5f), new Vector2(-80, -90), new Vector2(280, 105));
        Button exit = CreateSpriteButton("ExitButton", window, exitArt,
            new Vector2(0.5f, 0.5f), new Vector2(230, -90), new Vector2(280, 105));

        UnityEventTools.AddPersistentListener(resume.onClick, new UnityAction(pauseMenu.Resume));
        UnityEventTools.AddPersistentListener(menu.onClick, new UnityAction(pauseMenu.QuitToMenu));
        UnityEventTools.AddPersistentListener(exit.onClick, new UnityAction(pauseMenu.QuitGame));

        TMP_Text hint = CreateText("Hint", window, "ESC / P to resume", 28, InkPurple, TextAlignmentOptions.Center);
        Place(hint.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(-10, -215), new Vector2(600, 40));

        SerializedObject so = new SerializedObject(pauseMenu);
        so.FindProperty("pausePanel").objectReferenceValue = panel.gameObject;
        so.FindProperty("pauseButton").objectReferenceValue = pauseButton.gameObject;
        so.FindProperty("firstSelected").objectReferenceValue = resume.gameObject;
        so.FindProperty("statsText").objectReferenceValue = stats;
        so.ApplyModifiedPropertiesWithoutUndo();

        panel.gameObject.SetActive(false);

        PrefabUtility.SaveAsPrefabAsset(root, PausePrefabPath);
        Object.DestroyImmediate(root);
    }

    // ---------- Main menu ----------

    private static void BuildMainMenuScene()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera();

        GameObject controllerObject = new GameObject("MainMenu");
        MainMenu controller = controllerObject.AddComponent<MainMenu>();

        Canvas canvas = CreateCanvas("Canvas", 0);
        CreateBackground(canvas.transform, MainMenuArt);

        // Sits between the Skill Check logo and the skyline.
        Button start = CreateSpriteButton("StartButton", canvas.transform, startArt,
            new Vector2(0.5f, 0.5f), new Vector2(-170, -215), new Vector2(300, 112));
        Button exit = CreateSpriteButton("ExitButton", canvas.transform, exitArt,
            new Vector2(0.5f, 0.5f), new Vector2(170, -215), new Vector2(300, 112));

        UnityEventTools.AddPersistentListener(start.onClick, new UnityAction(controller.StartGame));
        UnityEventTools.AddPersistentListener(exit.onClick, new UnityAction(controller.QuitGame));

        CreateEventSystem(start.gameObject);
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), MainMenuScenePath);
    }

    // ---------- Game over ----------

    private static void BuildGameOverScene()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera();

        GameObject controllerObject = new GameObject("GameOverScreen");
        GameOverScreen controller = controllerObject.AddComponent<GameOverScreen>();

        Canvas canvas = CreateCanvas("Canvas", 0);
        CreateBackground(canvas.transform, GameOverArt);

        // Everything below sits inside the empty lower half of the GAME OVER window in the art.
        TMP_Text score = CreateText("ScoreText", canvas.transform, "Final Score: 0", 52, InkPurple, TextAlignmentOptions.Center);
        Place(score.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(15, -45), new Vector2(800, 70));
        score.fontStyle = FontStyles.Bold;

        Button retry = CreateSpriteButton("RetryButton", canvas.transform, startArt,
            new Vector2(0.5f, 0.5f), new Vector2(-250, -150), new Vector2(240, 90));
        Button menu = CreateSpriteButton("MenuButton", canvas.transform, menuArt,
            new Vector2(0.5f, 0.5f), new Vector2(15, -150), new Vector2(240, 90));
        Button exit = CreateSpriteButton("ExitButton", canvas.transform, exitArt,
            new Vector2(0.5f, 0.5f), new Vector2(280, -150), new Vector2(240, 90));

        UnityEventTools.AddPersistentListener(retry.onClick, new UnityAction(controller.Retry));
        UnityEventTools.AddPersistentListener(menu.onClick, new UnityAction(controller.ToMainMenu));
        UnityEventTools.AddPersistentListener(exit.onClick, new UnityAction(controller.QuitGame));

        SerializedObject so = new SerializedObject(controller);
        so.FindProperty("scoreText").objectReferenceValue = score;
        so.ApplyModifiedPropertiesWithoutUndo();

        CreateEventSystem(retry.gameObject);
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), GameOverScenePath);
    }

    // ---------- Build settings ----------

    private static void UpdateBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes
            .Where(s => s.path != MainMenuScenePath && s.path != GameOverScenePath)
            .ToList();

        scenes.Insert(0, new EditorBuildSettingsScene(MainMenuScenePath, true));
        scenes.Add(new EditorBuildSettingsScene(GameOverScenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    // ---------- Helpers ----------

    private static void CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0, 0, -10);
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;
    }

    private static void CreateEventSystem(GameObject firstSelected)
    {
        GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
        InputSystemUIInputModule module = eventSystemObject.AddComponent<InputSystemUIInputModule>();
        module.AssignDefaultActions();
        eventSystemObject.GetComponent<EventSystem>().firstSelectedGameObject = firstSelected;
    }

    private static Canvas CreateCanvas(string name, int sortingOrder)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        go.layer = LayerMask.NameToLayer("UI");

        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = ReferenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    // The menu art sheets are sliced into many sprites, so the full image is drawn as a texture.
    private static void CreateBackground(Transform parent, string texturePath)
    {
        RectTransform rect = CreateRect("Background", parent);
        Stretch(rect);
        RawImage image = rect.gameObject.AddComponent<RawImage>();
        image.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        image.raycastTarget = false;

        AspectRatioFitter fitter = rect.gameObject.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = ReferenceResolution.x / ReferenceResolution.y;
    }

    private static Button CreateSpriteButton(string name, Transform parent, ButtonArt art, Vector2 anchor, Vector2 position, Vector2 size)
    {
        RectTransform rect = CreateRect(name, parent);
        Place(rect, anchor, position, size);

        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = art.normal;
        image.preserveAspect = true;

        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.SpriteSwap;
        button.spriteState = new SpriteState
        {
            highlightedSprite = art.pressed,
            pressedSprite = art.pressed,
            selectedSprite = art.normal,
            disabledSprite = art.normal
        };
        return button;
    }

    private static TMP_Text CreateText(string name, Transform parent, string text, float size, Color color, TextAlignmentOptions alignment)
    {
        RectTransform rect = CreateRect(name, parent);
        TextMeshProUGUI tmp = rect.gameObject.AddComponent<TextMeshProUGUI>();
        if (tmp.font == null)
            tmp.font = TMP_Settings.defaultFontAsset;
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.raycastTarget = false;
        return tmp;
    }

    private static RectTransform CreateRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void Place(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }
}
