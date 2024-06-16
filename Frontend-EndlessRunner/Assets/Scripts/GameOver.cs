using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EndlessRunner
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverCanvas;
        [SerializeField] private TMP_InputField nameInput;

        private int score = 0;
        public void StopGame(int score)
        {
            gameOverCanvas.SetActive(true);
            this.score = score;
            SubmitScore();
        }

        public void RestartLevel()
        {

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
        }
    }
}
