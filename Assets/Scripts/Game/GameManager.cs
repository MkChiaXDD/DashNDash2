using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game End")]
    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private TMP_Text endScoreText;
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private string mainmenuScene;

    [Header("Gameplay")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject player;
    [SerializeField] private float loseThreshold = 50f;

    [Header("Pause")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Image audioIcon;
    [SerializeField] private Sprite muteIcon;
    [SerializeField] private Sprite unmuteIcon;

    [Header("Joystick Toggle")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private Image joystickToggleButtonImage;
    [SerializeField] private TMP_Text joystickToggleButtonText;
    [SerializeField] private Color joystickOnColor = Color.green;
    [SerializeField] private Color joystickOffColor = Color.red;

    [Header("Coins System")]
    [SerializeField] private float distancePerCoin;

    private bool pauseActive;
    private bool muted;
    private bool joystickDynamic;

    private float currDist;
    private float runBestDist;
    private float highscore;
    private bool gameEnded;
    private int coinsEarned;

    private const string HIGHSCORE_KEY = "Highscore";
    private const string MUTE_PREF_KEY = "MasterMuted";
    private const string MIXER_PARAM = "Master";
    private const string COINS_KEY = "Coins";
    private const string JOYSTICK_DYNAMIC_KEY = "JoystickDynamic";

    void Start()
    {
        Application.targetFrameRate = 60;

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        highscore = PlayerPrefs.GetFloat(HIGHSCORE_KEY, 0f);
        runBestDist = player.transform.position.y;

        pauseCanvas.SetActive(false);
        gameEndPanel.SetActive(false);

        LoadAndApplyMute();
        LoadAndApplyJoystickMode();
        UpdateScoreUI();
    }

    void Update()
    {
        if (gameEnded || pauseActive) return;

        currDist = player.transform.position.y;

        if (currDist > runBestDist)
        {
            runBestDist = currDist;

            if (runBestDist > highscore)
            {
                highscore = runBestDist;
                PlayerPrefs.SetFloat(HIGHSCORE_KEY, highscore);
                PlayerPrefs.Save();
            }

            UpdateScoreUI();
        }

        if (runBestDist - currDist > loseThreshold)
            GameEnd();
    }

    public void OnButtonTogglePause()
    {
        pauseActive = !pauseActive;
        pauseCanvas.SetActive(pauseActive);
        Time.timeScale = pauseActive ? 0f : 1f;

        if (pauseActive)
        {
            audioMixer.SetFloat(MIXER_PARAM, -80f);
        }
        else
        {
            LoadAndApplyMute();
        }
    }

    public void OnButtonToggleMute()
    {
        muted = !muted;
        SaveAndApplyMute();
    }

    public void OnButtonToggleJoystickMode()
    {
        joystickDynamic = !joystickDynamic;
        SaveAndApplyJoystickMode();
    }

    public void OnButtonMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainmenuScene);
    }

    private void LoadAndApplyMute()
    {
        muted = PlayerPrefs.GetInt(MUTE_PREF_KEY, 0) == 1;
        ApplyMute();
    }

    private void SaveAndApplyMute()
    {
        PlayerPrefs.SetInt(MUTE_PREF_KEY, muted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMute();
    }

    private void ApplyMute()
    {
        audioMixer.SetFloat(MIXER_PARAM, muted ? -80f : 0f);

        if (audioIcon != null)
            audioIcon.sprite = muted ? muteIcon : unmuteIcon;
    }

    private void LoadAndApplyJoystickMode()
    {
        joystickDynamic = PlayerPrefs.GetInt(JOYSTICK_DYNAMIC_KEY, 1) == 1;
        ApplyJoystickMode();
    }

    private void SaveAndApplyJoystickMode()
    {
        PlayerPrefs.SetInt(JOYSTICK_DYNAMIC_KEY, joystickDynamic ? 1 : 0);
        PlayerPrefs.Save();
        ApplyJoystickMode();
    }

    private void ApplyJoystickMode()
    {
        if (joystick != null)
            joystick.SetJoystickMode(joystickDynamic);

        if (joystickToggleButtonImage != null)
            joystickToggleButtonImage.color = joystickDynamic ? joystickOnColor : joystickOffColor;

        if (joystickToggleButtonText != null)
            joystickToggleButtonText.text = joystickDynamic ? "On" : "Off";
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"{runBestDist:0.0}m";
    }

    private void GameEnd()
    {
        gameEnded = true;
        Time.timeScale = 0f;

        gameEndPanel.SetActive(true);
        endScoreText.text = $"{runBestDist:0.0}m";
        highscoreText.text = $"{highscore:0.0}m";

        coinsEarned = (distancePerCoin > 0f) ? Mathf.FloorToInt(runBestDist / distancePerCoin) : 0;
        int currentCoins = PlayerPrefs.GetInt(COINS_KEY, 0);
        PlayerPrefs.SetInt(COINS_KEY, currentCoins + coinsEarned);
        PlayerPrefs.Save();
    }
}