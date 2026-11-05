using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Lives on the PauseMenu prefab in Assets/Resources. It spawns itself into every
// minigame scene listed in GameStatManager, so new minigames get a pause menu for free.
public class PauseMenu : MonoBehaviour
{
    private const string PrefabResourcePath = "PauseMenu";

    public static bool IsPaused { get; private set; }

    private static bool pauseBlocked;
    private static PauseMenu instance;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private TMP_Text statsText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        IsPaused = false;
        pauseBlocked = false;
        instance = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // The first scene finished loading before this ran, so handle it by hand.
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsPaused = false;
        pauseBlocked = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (!GameStatManager.IsMinigameScene(scene.name))
            return;

        // Someone already dropped the prefab into this scene by hand.
        if (instance != null || FindFirstObjectByType<PauseMenu>() != null)
            return;

        GameObject prefab = Resources.Load<GameObject>(PrefabResourcePath);
        if (prefab == null)
        {
            Debug.LogWarning("PauseMenu prefab not found in Resources. Run Skill Check > Build Menus.");
            return;
        }

        Instantiate(prefab).name = prefab.name;
    }

    // Called when a minigame starts its win/lose transition.
    public static void BlockPausing()
    {
        pauseBlocked = true;
        if (instance != null)
        {
            if (IsPaused)
                instance.Resume();
            instance.pauseButton.SetActive(false);
        }
    }

    void Awake()
    {
        instance = this;
        EnsureEventSystem();
        pausePanel.SetActive(false);
        pauseButton.SetActive(true);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            if (IsPaused)
            {
                IsPaused = false;
                Time.timeScale = 1f;
                AudioListener.pause = false;
            }
        }
    }

    void Update()
    {
        if (pauseBlocked)
            return;

        if (PausePressedThisFrame())
            TogglePause();
    }

    private static bool PausePressedThisFrame()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && (keyboard.escapeKey.wasPressedThisFrame || keyboard.pKey.wasPressedThisFrame))
            return true;

        Gamepad gamepad = Gamepad.current;
        return gamepad != null && gamepad.startButton.wasPressedThisFrame;
    }

    public void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (IsPaused || pauseBlocked)
            return;

        IsPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (statsText != null)
            statsText.text = $"Lives: {GameStatManager.health}     Score: {GameStatManager.score}";

        pausePanel.SetActive(true);
        pauseButton.SetActive(false);

        if (EventSystem.current != null && firstSelected != null)
            EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void Resume()
    {
        if (!IsPaused)
            return;

        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        pausePanel.SetActive(false);
        pauseButton.SetActive(!pauseBlocked);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void QuitToMenu()
    {
        GameStatManager.ReturnToMainMenu();
    }

    public void QuitGame()
    {
        GameStatManager.QuitGame();
    }

    // Some minigame scenes (BulletHell) have no EventSystem, and UI buttons need one.
    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
        InputSystemUIInputModule module = eventSystem.AddComponent<InputSystemUIInputModule>();
        module.AssignDefaultActions();
    }
}
