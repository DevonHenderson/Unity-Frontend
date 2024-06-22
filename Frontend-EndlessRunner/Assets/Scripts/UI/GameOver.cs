/*
 * File: GameOver.cs
 * Purpose: Shows the endscreen panel and has fields to POST to API
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
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

        [SerializeField] private TextMeshProUGUI bestScore;

        /// <summary>
        /// Shows the end game canvas with accurate score
        /// </summary>
        /// <param name="score"></param>
        public void StopGame(int score)
        {
            gameOverCanvas.SetActive(true);
            gameplayCanvas.SetActive(false);
            this.score = score;
            scoreDisplay.text = "Final Score: " + score.ToString();
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
            StartCoroutine(SetUser());
            StartCoroutine(UpdateScore());
        }

        private IEnumerator SetUser()
        {
            string json = "{\"username\": \"" + nameInput.text + "\"}";
            string url = "http://localhost:5432/api/user"; // Replace <PORT> with your actual port

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to create user: " + request.error);
            }
            else
            {
                string response = request.downloadHandler.text;
                try
                {
                    // Parse JSON response
                    var responseData = JsonUtility.FromJson<UserCreationResponse>(response);

                    if (responseData != null)
                    {
                        if (request.responseCode == 200 || request.responseCode == 201) // Handle both success and user exists scenarios
                        {
                            int userID = responseData.id;
                            PlayerPrefs.SetInt("userID", userID);
                            PlayerPrefs.Save(); // Save PlayerPrefs immediately

                            Debug.Log("User created/updated successfully. UserID: " + userID);

                            // Now update the score using the retrieved userID
                            StartCoroutine(UpdateScore());
                        }
                        else
                        {
                            Debug.LogError("Failed to create/update user. Unexpected response code: " + request.responseCode);
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to parse response data.");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing response: " + e.Message);
                }
            }
        }

        private IEnumerator UpdateScore()
        {
            int userID = PlayerPrefs.GetInt("userID");

            // Construct the JSON object with score
            string json = "{\"unityBestScore\": " + score.ToString() + "}";

            // Construct the URL for updating the score
            string url = "http://localhost:5432/api/user/score/" + userID.ToString(); 

            UnityWebRequest request = new UnityWebRequest(url, "PUT");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to update score: " + request.error);
            }
            else
            {
                Debug.Log("Score updated successfully!");

            }
        }

        [System.Serializable]
        private class UserCreationResponse
        {
            public string msg;
            public int id; // Assuming the response includes the user ID
        }
    }
}
