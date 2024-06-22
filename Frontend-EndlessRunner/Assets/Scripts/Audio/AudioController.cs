/*
 * File: AudioController.cs
 * Purpose: Plays/mutes background music during gameplay
 */

using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool isMuted = false;
    [SerializeField] private Sprite unmutedSprite;
    [SerializeField] private Sprite mutedSprite;
    [SerializeField] private Button muteButton;
    [SerializeField] private float startingVolume = 0.25f;

    /// <summary>
    /// Start audio clip with correct settings  <br />
    /// Check if game was previously muted and use to show correct button/volume
    /// </summary>
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = startingVolume; // Set starting volume
        audioSource.Play();

        muteButton = GameObject.FindGameObjectWithTag("VolumeButton").GetComponent<Button>();
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        // Set button sprite based on muted state
        SetVolume();
        UpdateButtonSprite();
    }

    /// <summary>
    /// Toggles the isMuted value and saves to PlayerPref  <br />
    /// Update volume and sprite accordingly
    /// </summary>
    public void ToggleMute()
    {
        // Save new mute state to PlayerPrefs
        isMuted = !isMuted;
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        //Update current volume and button sprite
        SetVolume();
        UpdateButtonSprite();
    }

    /// <summary>
    /// Set the correct volume based on playerpref isMuted state
    /// </summary>
    void SetVolume(){ audioSource.volume = isMuted ? 0 : startingVolume; }

    /// <summary>
    /// Change button sprite based on mute state
    /// </summary>
    private void UpdateButtonSprite(){ muteButton.image.sprite = isMuted ? mutedSprite : unmutedSprite; }
}
