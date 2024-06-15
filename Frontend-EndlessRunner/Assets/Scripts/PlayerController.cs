using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float initalPlayerSpeed = 4f;
    [SerializeField] private float maximumPlayerSpeed = 30f;
    [SerializeField] private float speedIncreaseRate = .1f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float initialGravity = -9.81f;
    [SerializeField] private LayerMask groundLayer;
    
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

    private void OnEnable()
    {
        turnAction.performed += PlayerTurn;
        jumpAction.performed += PlayerJump;
        slideAction.performed += PlayerTurn;
    }

    private void OnDisable()
    {
        turnAction.performed -= PlayerTurn;
        jumpAction.performed -= PlayerJump;
        slideAction.performed -= PlayerSlide;
    }

    private void Start()
    {
        playerSpeed = initalPlayerSpeed;
        currentGravity = initialGravity;
    }

    //Handle behaviour for turn input
    private void PlayerTurn(InputAction.CallbackContext context)
    {

    }
    //Handle behaviour for jump input
    private void PlayerJump(InputAction.CallbackContext context)
    {

    }
    //Handle behaviour for slide input
    private void PlayerSlide(InputAction.CallbackContext context)
    {

    }

    //Move the player in the current direction
    private void Update()
    {
        characterController.Move(transform.forward * playerSpeed * Time.deltaTime);
    }

    //Perform grounded check in two locations (behind/ahead of player)
    private bool IsGrounded(float length)
    {
        //Set raycast origin point to base of player
        Vector3 firstRaycast = transform.position;
        firstRaycast.y -= characterController.height / 2; //Set to bottom of player
        firstRaycast.y += 0.1f;

        Vector3 secondRaycast = firstRaycast;
        secondRaycast += transform.forward * 0.2f; //Set ahead of player
        firstRaycast -= transform.forward * 0.2f; //Set behind player

        //Check both raycasts for ground collision
        if (Physics.Raycast(firstRaycast, Vector3.down, out RaycastHit hitBehind, length, groundLayer) || 
            Physics.Raycast(secondRaycast, Vector3.down, out RaycastHit hitAhead, length, groundLayer))
        {
            return true;
        }
        return false;
    }
}
