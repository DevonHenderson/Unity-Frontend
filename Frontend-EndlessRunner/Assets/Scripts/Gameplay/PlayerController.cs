/*
 * File: PlayerController.cs
 * Purpose: Manages the player movement and score during gameplay
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace EndlessRunner {
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Movement")]
        [SerializeField] private float initalPlayerSpeed = 4f;
        [SerializeField] private float maximumPlayerSpeed = 30f;
        [SerializeField] private float speedIncreaseRate = .1f;
        [SerializeField] public float playerSpeed;
        private Vector3 movementDirection = Vector3.forward;

        [Header("Player Jumping")]
        [SerializeField] private float jumpHeight = 1f;
        [SerializeField] private float initialGravity = -9.81f;
        private float currentGravity;
        private Vector3 playerVelocity;

        [Header("Player Input System")] //Store inputs for faster checking of behaviours
        private PlayerInput playerInput;
        private InputAction turnAction;
        private InputAction jumpAction;
        private InputAction slideAction;

        [Header("Player Components")]
        private CharacterController characterController;
        private Animator animator;

        [Header("Layer Masks")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask turnLayer;
        [SerializeField] private LayerMask obstacleLayer;

        [Header("Animator variables")] //For sliding animation
        [SerializeField] AnimationClip slideAnimation;
        private int slidingAnimID;
        private bool isSliding = false;

        [Header("Scoring")]
        public float score = 0; //public so it can update obstacle probabilities
        private float scoreMultiplier = 10f;

        [Header("Events")]
        [SerializeField] private UnityEvent<Vector3> turnEvent;
        [SerializeField] private UnityEvent<int> gameOverEvent;
        [SerializeField] private UnityEvent<int> scoreUpdateEvent;

        [Header("End Game Camera")]
        [SerializeField] private Camera camera;

        /// <summary>
        /// Assign components to objects and store player inputs
        /// </summary>
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            slidingAnimID = Animator.StringToHash("SlidingAnim");
            turnAction = playerInput.actions["Turn"];
            jumpAction = playerInput.actions["Jump"];
            slideAction = playerInput.actions["Slide"];
        }

        /// <summary>
        /// Use input action to run specific functions
        /// </summary>
        private void OnEnable()
        {
            turnAction.performed += PlayerTurn;
            jumpAction.performed += PlayerJump;
            slideAction.performed += PlayerSlide;
        }

        /// <summary>
        /// Stop code running when input action stopped
        /// </summary>
        private void OnDisable()
        {
            turnAction.performed -= PlayerTurn;
            jumpAction.performed -= PlayerJump;
            slideAction.performed -= PlayerSlide;
        }

        /// <summary>
        /// Set the default speed and gravity for the player
        /// </summary>
        private void Start()
        {
            playerSpeed = initalPlayerSpeed;
            currentGravity = initialGravity;
        }

        /// <summary>
        /// Checks the state of the player for movement/jumping/sliding speeds  <br />
        /// Updates the score during gameplay
        /// </summary>
        private void Update()
        {
            //Falling off level ends the game
            if (!IsGrounded(20f))
            {
                GameOver();
                return;
            }

            //Scoring
            score += scoreMultiplier * Time.deltaTime;
            scoreUpdateEvent.Invoke((int)score); //Cast value as int

            //Move the player cosntantly in the current direction
            characterController.Move(transform.forward * playerSpeed * Time.deltaTime); 

            //Make sure player stays on top of ground
            if (IsGrounded() && playerVelocity.y < 0)
            {
                playerVelocity.y = 0;
            }

            playerVelocity.y += currentGravity * Time.deltaTime;
            characterController.Move(playerVelocity * Time.deltaTime);

            //Increase the player speed up to a max value
            if (playerSpeed < maximumPlayerSpeed)
            {
                playerSpeed += Time.deltaTime * speedIncreaseRate;
                currentGravity = initialGravity - playerSpeed; //Make sure gravity is still accurate at higher speeds

                //Make animations player faster if player has sped up
                if (animator.speed < 1.4f)
                {
                    animator.speed += (1 / playerSpeed) * Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// Handle behaviour for turn input
        /// </summary>
        /// <param name="context"></param>
        private void PlayerTurn(InputAction.CallbackContext context)
        {
            Vector3? turnPosition = CheckTurn(context.ReadValue<float>());

            //Player not standing on tile with turn pivot
            if (!turnPosition.HasValue)
            {
                GameOver();
                return;
            }

            Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) * movementDirection;
            turnEvent.Invoke(targetDirection);

            Turn(context.ReadValue<float>(), turnPosition.Value);
        }

        /// <summary>
        /// Returns the type of turn prefab the player is moving on
        /// </summary>
        /// <param name="turnValue"></param>
        /// <returns></returns>
        private Vector3? CheckTurn(float turnValue)
        {
            //Check for pivot point colliders on 'Turn' prefabs
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f, turnLayer);
            if (hitColliders.Length != 0)
            {
                Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
                TileType type = tile.type;

                //Check if player has used correct input for the type of turn
                //eg. A key = -1 value from PlayerTurn context and standing on prefab marked with LEFT pivot point
                if ((type == TileType.LEFT && turnValue == -1) ||
                    (type == TileType.RIGHT && turnValue == 1) ||
                    (type == TileType.SIDEWAYS))
                {
                    return tile.pivot.position;
                }
            }
            return null;
        }

        /// <summary>
        /// Move the player to correct turn position  <br />
        /// Rotate with slight delay so camera movement looks smoother
        /// </summary>
        /// <param name="turnValue">Input value</param>
        /// <param name="turnPosition"></param>
        private void Turn(float turnValue, Vector3 turnPosition)
        {
            // Move the player to the correct position after turning
            Vector3 tempPlayerPosition = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
            characterController.enabled = false;
            transform.position = tempPlayerPosition;
            characterController.enabled = true;

            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);

            // Start the coroutine to rotate player over time
            StartCoroutine(RotateOverTime(targetRotation, 0.05f));

        }

        /// <summary>
        /// Make the rotation at turning pivots appear smoother
        /// </summary>
        /// <param name="targetRotation"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private IEnumerator RotateOverTime(Quaternion targetRotation, float duration)
        {
            Quaternion initialRotation = transform.rotation;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.rotation = targetRotation;

            // Update the stored direction that the player is moving along
            movementDirection = transform.forward.normalized;
        }

        /// <summary>
        /// Handle grounded check for jump input
        /// </summary>
        /// <param name="context"></param>
        private void PlayerJump(InputAction.CallbackContext context)
        {
            //Only jump when grounded
            if (IsGrounded())
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * currentGravity * -3f);
                characterController.Move(playerVelocity * Time.deltaTime);
            }
        }
        /// <summary>
        /// Handle behaviour for slide input
        /// </summary>
        /// <param name="context"></param>
        private void PlayerSlide(InputAction.CallbackContext context)
        {
            if (!isSliding && IsGrounded())
            {
                StartCoroutine(Slide());
            }
        }

        /// <summary>
        /// Perform grounded check in two locations (behind/ahead of player)
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private bool IsGrounded(float length = 0.2f)
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

        /// <summary>
        /// Start the sliding animation  <br />
        /// Can only play once before needing another input
        /// </summary>
        /// <returns></returns>
        private IEnumerator Slide()
        {
            isSliding = true; //Only let player slide once

            //Shrink collider down during animation
            //Moves centre of collider down before shrinking so stays in correct position
            //approx 25% height of player
            Vector3 originalControllerCentre = characterController.center;
            Vector3 newControllerCentre = originalControllerCentre;
            characterController.height /= 2;
            newControllerCentre.y -= characterController.height / 2;
            characterController.center = newControllerCentre;

            //Play the sliding animation
            animator.Play(slidingAnimID);
            yield return new WaitForSeconds(slideAnimation.length / animator.speed); //Wait for animation to play fully

            //Return collider to previous size
            characterController.height *= 2;
            characterController.center = originalControllerCentre;
            
            isSliding = false;
        }

        /// <summary>
        /// Swap camea to endgame camera and show canvas <br />
        /// Update score on final canvas
        /// </summary>
        private void GameOver()
        {
            gameOverEvent.Invoke((int)score);
            gameObject.SetActive(false);
            camera.gameObject.SetActive(true);
        }

        /// <summary>
        /// Check for collision with obstacles/walls
        /// </summary>
        /// <param name="hit"></param>
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0)
            {
                GameOver();
            }
        }
    }
}
