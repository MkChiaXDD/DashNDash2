using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clickSound;

    private void Start()
    {
        RegisterAllButtons();
    }

    public void RegisterAllButtons()
    {
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button button in buttons)
        {
            if (!button.gameObject.scene.IsValid())
                continue;

            button.onClick.RemoveListener(PlayClickSound);
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        sfxSource.PlayOneShot(clickSound);
    }
}