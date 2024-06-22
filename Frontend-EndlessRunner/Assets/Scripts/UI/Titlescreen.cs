/*
 * File: Titlescreen.cs
 * Purpose: Manages the starting of the game and showing ingame score panel
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Titlescreen : MonoBehaviour
{
    [Header("Canvas Objects")]
    [SerializeField] private GameObject titleCanvas;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject scoreCanvas;
    private Image titlescreenImage;

    //Used to zoom in panel when starting
    private Vector3 initialScale;
    private Vector3 targetScale = new Vector3(1.5f, 1.5f, 1f);

    /// <summary>
    /// Freeze the game at start and get panel image to modify in coroutine
    /// </summary>
    void Start()
    {
        Time.timeScale = 0f;
        titlescreenImage = titlePanel.GetComponent<Image>();
        initialScale = titlePanel.transform.localScale;
    }

    /// <summary>
    /// On button click, fade titlescreen then start game
    /// </summary>
    public void StartGame()
    {
        StartCoroutine(FadeOutTitlescreen());
    }

    /// <summary>
    /// Fades the titlescreen canvas over 1sec before starting the game
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator FadeOutTitlescreen()
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Color initialColor = titlescreenImage.color;

        //Fade the titlescreen canvas over 1 second
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;  // Use unscaledDeltaTime because Time.timeScale is 0
            float alpha = Mathf.Clamp01(1f - (elapsedTime / duration));
            titlescreenImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            //Scale up the panel while fading so it looks like its zooming in
            float scaleProgress = elapsedTime / duration;
            titlePanel.transform.localScale = Vector3.Lerp(initialScale, targetScale, scaleProgress);

            yield return null;
        }

        // Ensure final values are set
        titlescreenImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f); 
        titlePanel.transform.localScale = targetScale;

        titleCanvas.SetActive(false);
        scoreCanvas.SetActive(true);
        Time.timeScale = 1f; //Set the timescale back to 1 so the game can start
    }
}
