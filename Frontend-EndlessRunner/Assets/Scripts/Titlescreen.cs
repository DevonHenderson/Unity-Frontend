using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Titlescreen : MonoBehaviour
{
    [SerializeField] private GameObject titlescreen;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject scorePanel;
    private Image titlescreenImage;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        titlescreenImage = panel.GetComponent<Image>();
    }

    public void StartGame()
    {
        StartCoroutine(FadeOutTitlescreen());
    }

    private IEnumerator FadeOutTitlescreen()
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Color initialColor = titlescreenImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;  // Use unscaledDeltaTime because Time.timeScale is 0
            float alpha = Mathf.Clamp01(1f - (elapsedTime / duration));
            titlescreenImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        titlescreen.SetActive(false);
        scorePanel.SetActive(true);
        Time.timeScale = 1f;
    }
}
