using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Main Navigation")]
    [SerializeField] private string gameScene;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("BGM")]
    [SerializeField] private Button bgmButton;
    [SerializeField] private TMP_Text bgmButtonText;

    [Header("SFX")]
    [SerializeField] private Button sfxButton;
    [SerializeField] private TMP_Text sfxButtonText;

    [Header("Colors")]
    [SerializeField] private Color onColor = Color.green;
    [SerializeField] private Color offColor = Color.red;

    [Header("UI")]
    [SerializeField] private TMP_Text highscoreText;

    private bool bgmMuted;
    private bool sfxMuted;

    private const string BGM_MUTE_PREF_KEY = "BGMMuted";
    private const string SFX_MUTE_PREF_KEY = "SFXMuted";

    private const string BGM_MIXER_PARAM = "BGM";
    private const string SFX_MIXER_PARAM = "SFX";

    private void Start()
    {
        LoadAndApplyAudio();
        OnButtonGetHighScore();
    }

    public void OnButtonPlay()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void OnButtonGetHighScore()
    {
        float highscore = PlayerPrefs.GetFloat("Highscore", 0f);
        highscoreText.text = $"Highscore: {highscore:0.0}m";
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
        audioMixer.SetFloat(BGM_MIXER_PARAM, bgmMuted ? -80f : 0f);
        audioMixer.SetFloat(SFX_MIXER_PARAM, sfxMuted ? -80f : 0f);

        UpdateButtonVisuals(
            bgmButton,
            bgmButtonText,
            !bgmMuted
        );

        UpdateButtonVisuals(
            sfxButton,
            sfxButtonText,
            !sfxMuted
        );
    }

    private void UpdateButtonVisuals(Button button, TMP_Text text, bool isOn)
    {
        Image buttonImage = button.GetComponent<Image>();

        if (isOn)
        {
            buttonImage.color = onColor;
            text.text = "ON";
        }
        else
        {
            buttonImage.color = offColor;
            text.text = "OFF";
        }
    }
}