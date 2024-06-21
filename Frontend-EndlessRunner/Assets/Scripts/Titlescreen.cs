using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Titlescreen : MonoBehaviour
{
    [SerializeField] private GameObject titlescreen;
    [SerializeField] private GameObject scorePanel;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
    }
    
    public void StartGame()
    {
        titlescreen.SetActive(false);
        scorePanel.SetActive(true);
        Time.timeScale = 1f;
            
    }
}
