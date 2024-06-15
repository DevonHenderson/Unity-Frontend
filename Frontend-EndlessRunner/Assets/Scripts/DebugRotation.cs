using UnityEngine;

public class DebugRotation : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Left Arrow Key Pressed");
            RotatePlayer(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right Arrow Key Pressed");
            RotatePlayer(1);
        }
    }

    private void RotatePlayer(float turnValue)
    {
        Debug.Log("RotatePlayer called with value: " + turnValue);
        Quaternion targetRotation = Quaternion.Euler(0, 90 * turnValue, 0);
        Debug.Log("Target Rotation: " + targetRotation.eulerAngles);
        transform.rotation = targetRotation;
        Debug.Log("New Rotation: " + transform.rotation.eulerAngles);

        // Directly set the forward direction as a simpler example
        transform.forward = new Vector3(turnValue, 0, 0);
        Debug.Log("transform.forward after: " + transform.forward);
    }

    private void LateUpdate()
    {
        Debug.Log("LateUpdate - transform.rotation: " + transform.rotation.eulerAngles);
    }
}