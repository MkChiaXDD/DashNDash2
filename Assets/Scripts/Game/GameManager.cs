using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game End")]
    [SerializeField] private GameObject gameEndCanvas;
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

    [Header("BGM")]
    [SerializeField] private Button bgmButton;
    [SerializeField] private TMP_Text bgmButtonText;

    [Header("SFX")]
    [SerializeField] private Button sfxButton;
    [SerializeField] private TMP_Text sfxButtonText;

    [Header("Audio Button Colors")]
    [SerializeField] private Color audioOnColor = Color.green;
    [SerializeField] private Color audioOffColor = Color.red;

    [Header("Coins System")]
    [SerializeField] private float distancePerCoin;

    private bool pauseActive;
    private bool bgmMuted;
    private bool sfxMuted;

    private float currDist;
    private float runBestDist;
    private float highscore;
    private bool gameEnded;
    private int coinsEarned;

    private const string HIGHSCORE_KEY = "Highscore";

    private const string BGM_MUTE_PREF_KEY = "BGMMuted";
    private const string SFX_MUTE_PREF_KEY = "SFXMuted";

    private const string BGM_MIXER_PARAM = "BGM";
    private const string SFX_MIXER_PARAM = "SFX";

    private const string COINS_KEY = "Coins";

    private void Start()
    {
        Application.targetFrameRate = 60;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        highscore = PlayerPrefs.GetFloat(HIGHSCORE_KEY, 0f);

        if (player != null)
        {
            runBestDist = player.transform.position.y;
        }

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }

        if (gameEndCanvas != null)
        {
            gameEndCanvas.SetActive(false);
        }

        LoadAndApplyAudio();
        UpdateScoreUI();
    }

    private void Update()
    {
        if (gameEnded || pauseActive) return;
        if (player == null) return;

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
        {
            GameEnd();
        }
    }

    public void OnButtonTogglePause()
    {
        pauseActive = !pauseActive;

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(pauseActive);
        }

        Time.timeScale = pauseActive ? 0f : 1f;

        ApplyAudio();
    }

    public void OnButtonToggleBGMAudio()
    {
        bgmMuted = !bgmMuted;
        SaveAndApplyAudio();
    }

    public void OnButtonToggleSFXAudio()
    {
        sfxMuted = !sfxMuted;
        SaveAndApplyAudio();
    }

    public void OnButtonMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainmenuScene);
    }

    private void LoadAndApplyAudio()
    {
        bgmMuted = PlayerPrefs.GetInt(BGM_MUTE_PREF_KEY, 0) == 1;
        sfxMuted = PlayerPrefs.GetInt(SFX_MUTE_PREF_KEY, 0) == 1;

        ApplyAudio();
    }

    private void SaveAndApplyAudio()
    {
        PlayerPrefs.SetInt(BGM_MUTE_PREF_KEY, bgmMuted ? 1 : 0);
        PlayerPrefs.SetInt(SFX_MUTE_PREF_KEY, sfxMuted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyAudio();
    }

    private void ApplyAudio()
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(BGM_MIXER_PARAM, bgmMuted ? -80f : 0f);
            audioMixer.SetFloat(SFX_MIXER_PARAM, sfxMuted ? -80f : 0f);
        }

        UpdateAudioButtonVisuals(
            bgmButton,
            bgmButtonText,
            !bgmMuted
        );

        UpdateAudioButtonVisuals(
            sfxButton,
            sfxButtonText,
            !sfxMuted
        );
    }

    private void UpdateAudioButtonVisuals(Button button, TMP_Text text, bool isOn)
    {
        if (button != null)
        {
            Image buttonImage = button.GetComponent<Image>();

            if (buttonImage != null)
            {
                buttonImage.color = isOn ? audioOnColor : audioOffColor;
            }
        }

        if (text != null)
        {
            text.text = isOn ? "ON" : "OFF";
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{runBestDist:0.0}m";
        }
    }

    private void GameEnd()
    {
        gameEnded = true;
        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("GameOver");
        }

        if (gameEndCanvas != null)
        {
            gameEndCanvas.SetActive(true);
        }

        if (endScoreText != null)
        {
            endScoreText.text = $"{runBestDist:0.0}m";
        }

        if (highscoreText != null)
        {
            highscoreText.text = $"{highscore:0.0}m";
        }

        coinsEarned = distancePerCoin > 0f ? Mathf.FloorToInt(runBestDist / distancePerCoin) : 0;

        int currentCoins = PlayerPrefs.GetInt(COINS_KEY, 0);
        PlayerPrefs.SetInt(COINS_KEY, currentCoins + coinsEarned);
        PlayerPrefs.Save();
    }
}