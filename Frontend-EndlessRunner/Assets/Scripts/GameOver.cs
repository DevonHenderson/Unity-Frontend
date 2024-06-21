using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndlessRunner
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverCanvas;
        [SerializeField] public TMP_InputField nameInput;
        [SerializeField] private TextMeshProUGUI scoreDisplay;
        [SerializeField] private GameObject gameplayCanvas; 

        private int score = 0;
        public void StopGame(int score)
        {
            gameOverCanvas.SetActive(true);
            gameplayCanvas.SetActive(false);
            this.score = score;
            scoreDisplay.text = score.ToString();
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void SubmitScore()
        {
            StartCoroutine(UpdateScore());
        }

        private IEnumerator UpdateScore()
        {
            //In api
            //SetName (playerName.text)
            //SetScore (score)
            yield return null;
        }
    }
}
