using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Main Navigation")]
    [SerializeField] private string gameScene;
    [SerializeField] private GameObject settingsPanel;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Image audioIcon;
    [SerializeField] private Sprite muteIcon;
    [SerializeField] private Sprite unmuteIcon;
    [SerializeField] private TMP_Text highscoreText;

    private bool muted;

    private const string MUTE_PREF_KEY = "MasterMuted";
    private const string MIXER_PARAM = "Master";

    private void Start()
    {
        settingsPanel.SetActive(false);
        LoadAndApplyMute();
    }

    public void OnButtonPlay()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void OnButtonToggleSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        float highscore = PlayerPrefs.GetFloat("Highscore", 0f);
        highscoreText.text = $"{highscore:0.0}m";
    }

    public void OnButtonToggleMuteAudio()
    {
        muted = !muted;
        SaveAndApplyMute();
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
        audioIcon.sprite = muted ? muteIcon : unmuteIcon;
    }
}
