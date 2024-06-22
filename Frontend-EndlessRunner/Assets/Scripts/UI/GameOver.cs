using System.Collections;
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

        [SerializeField] private TextMeshProUGUI bestScoreDisplay; // UI element to display best score

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
            bestScoreDisplay.enabled = false;
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
        }

        /// <summary>
        /// Creates/Gets a user from the API
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetUser()
        {
            Debug.Log("Making request SETUSER");
            string json = "{\"username\": \"" + nameInput.text + "\"}";
            string url = "http://localhost:5432/api/user";

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            //Error Checks
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

                    if (request.responseCode == 200 || request.responseCode == 201)
                    {
                        //Save the User ID for other API updates
                        int userID = responseData.id;
                        PlayerPrefs.SetInt("userID", userID);
                        PlayerPrefs.Save(); // Save PlayerPrefs immediately

                        //Debug.Log("User created/updated successfully. UserID: " + userID);

                        // Get the user's best score using the retrieved userID
                        StartCoroutine(GetBestScore(userID));
                        StartCoroutine(GetBestScore(userID)); // Had to call twice otherwise score wouldnt update correctly
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing response: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Gets the current best score for a userID from the API
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private IEnumerator GetBestScore(int userID)
        {
            Debug.Log("Making request BESTSCORE");
            //Make the request
            string url = "http://localhost:5432/api/user/" + userID.ToString();
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            //Check if successful
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to get best score: " + request.error);
            }
            else
            {
                string response = request.downloadHandler.text;
                try
                {
                    // Parse JSON response
                    var responseData = JsonUtility.FromJson<UserDataResponse>(response);
                    int bestScore = responseData.data.unityBestScore;
                    bestScoreDisplay.text = "(" + nameInput.text + ") Personal Best: " + bestScore.ToString();
                    bestScoreDisplay.enabled = true;

                    // Check if the current score is higher than the best score
                    if (score > bestScore)
                    {
                        // Update the best score on the backend
                        StartCoroutine(UpdateScore(userID));
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing response: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Try update to new score if higher than previous best stored on API
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private IEnumerator UpdateScore(int userID)
        {
            Debug.Log("Making request UPDATESCORE");
            // Create JSON object with score
            string json = "{\"unityBestScore\": " + score.ToString() + "}";

            // URL for updating the score
            string url = "http://localhost:5432/api/user/score/" + userID.ToString();

            //Request
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
                StartCoroutine(GetBestScore(userID)); //Make sure the score display is correct
            }
        }

        //Got some help from ChatGPT for creating these classes
        //Allows for successful API calls
        [System.Serializable] private class UserCreationResponse{ public string msg; public int id; }
        [System.Serializable] private class UserDataResponse{ public UserData data; }

        [System.Serializable] private class UserData{ public int unityBestScore; }
    }
}
