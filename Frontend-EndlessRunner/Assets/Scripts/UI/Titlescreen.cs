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

    /// <summary>
    /// Freeze the game at start and get panel image to modify in coroutine
    /// </summary>
    void Start()
    {
        Time.timeScale = 0f;
        titlescreenImage = titlePanel.GetComponent<Image>();
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
            yield return null;
        }

        titleCanvas.SetActive(false);
        scoreCanvas.SetActive(true);
        Time.timeScale = 1f; //Set the timescale back to 1 so the game can start
    }
}
