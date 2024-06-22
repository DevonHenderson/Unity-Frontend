/*
 * File: SetScore.cs
 * Purpose: Updates the ingame score during gameplay
 */

using UnityEngine;
using TMPro;

public class SetScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    /// <summary>
    /// Gets the score from the player controller and displays on canvas during gameplay
    /// </summary>
    /// <param name="score">Player score that increases over time</param>
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
}
