using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private UnityEvent apiConnected;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        //bool connected = false;
        //Check connection to api
        //if (!response.success)
        //{
        //    Debug.Log("error");
        //    return;
        //}
        //else
        //{
        //    Debug.Log("Connected successfully to API");
        //    connected = true;
        //}
        yield return null;
        //yield return new WaitUntil(() => connected);
        apiConnected.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
