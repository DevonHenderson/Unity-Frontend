using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float initalPlayerSpeed = 4f;
    [SerializeField] private float maximumPlayerSpeed = 30f;
    [SerializeField] private float speedIncreaseRate = .1f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float initialGravity = -9.81f;
    
    private float currentGravity;
    private float playerSpeed;
    private Vector3 movementDirection = Vector3.forward;
    private PlayerInput playerInput;
    private InputAction turnAction;
    private InputAction jumpAction;
    private InputAction slideAction;
    private CharacterController characterController;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        turnAction = playerInput.actions["Turn"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];
    }

    private void Start()
    {
        playerSpeed = initalPlayerSpeed;
        currentGravity = initialGravity;
    }

    
}
