/*
 * File: GameOver.cs
 * Purpose: Shows the endscreen panel and has fields to POST to API
 */

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndlessRunner
{
    public class GameOver : MonoBehaviour
    {
        [Header("UI objects")]
        [SerializeField] private GameObject gameOverCanvas;
        [SerializeField] public TMP_InputField nameInput;
        [SerializeField] private TextMeshProUGUI scoreDisplay;
        [SerializeField] private GameObject gameplayCanvas; 
        private int score = 0;

        /// <summary>
        /// Shows the end game canvas with accurate score
        /// </summary>
        /// <param name="score"></param>
        public void StopGame(int score)
        {
            gameOverCanvas.SetActive(true);
            gameplayCanvas.SetActive(false);
            this.score = score;
            scoreDisplay.text = score.ToString();
        }

        /// <summary>
        /// Reloads the scene to allow for another playthrough
        /// </summary>
        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Sends the current score and username to update the API
        /// </summary>
        public void SubmitScore()
        {
            StartCoroutine(UpdateScore());
        }

        /// <summary>
        /// Updates the score on the API on submit button OnClick event
        /// </summary>
        /// <returns>null</returns>
        // Not currently completed
        private IEnumerator UpdateScore()
        {
            //In api
            //SetName (playerName.text)
            //SetScore (score)
            yield return null;
        }
    }
}
