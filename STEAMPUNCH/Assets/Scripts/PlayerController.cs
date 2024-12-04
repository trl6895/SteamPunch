using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// The states of the player that determine if they can be controlled or not
/// </summary>
public enum PlayerState { Free, Locked, Surfing, AirPause }

/// <summary>
/// The current fist that the player is punching with
/// </summary>
public enum CurrentFist { Right, Left }

public class PlayerController : MonoBehaviour
{
    // Fields ===========================================================================

    // References -------------------------------------------------------------
    [SerializeField] SceneManager sceneManager;
    [SerializeField] CameraActions cameraActions;

    // Input Handling (New Input System) --------------------------------------
    private NewInputHandler newInputHandler;
    private InputAction move;
    private InputAction look;
    private InputAction aim;
    private bool usingGamepad;

    // Movement ---------------------------------------------------------------
    private float horizontal;
    private float speed = 8f;
    [SerializeField] public float jumpingPower = 16f;
    [SerializeField] public float throwingForceX = 500.0f;
    [SerializeField] public float throwingForceY = 500.0f;
    [SerializeField] public float throwingForce = 1000.0f;
    [SerializeField] public float groundPoundForce = 50.0f;
    private bool isFacingRight = true;

    private bool jumpFlag = false;
    private bool gpFlag = false;
    private bool gpLockout = false;

    private const float airPauseTime = 0.5f;
    private float airPauseTimer = 0.0f;

    [SerializeField]
    float airPunchCounter = 0;

    [SerializeField]
    float punchMoveForce;
    public float currentPunchMoveForce = 0;
    [SerializeField] private float recoilMultiplier = 1.0f;

    // Player State -----------------------------------------------------------
    private PlayerState currentState = PlayerState.Free;

    // Interaction ------------------------------------------------------------
    [SerializeField] private float pickUpRadius = 0.1f;
    private List<Collider2D> nearbyColliders = new List<Collider2D>();
    private ThrowableEnemy nearbyKnockedEnemy;
    public bool isHoldingEnemy = false;
    public bool isSurfingEnemy = false;
    [SerializeField] public float punchCooldown = 0.25f;
    [SerializeField] public float fistResetCooldown = 1.0f;
    [SerializeField] public float punchCooldownTimer = 0.25f;
    [SerializeField] public CapsuleCollider2D punchCollider;
    [SerializeField] public CapsuleCollider2D groundPoundCollider;
    public float throwingAngle = 0.0f;
    [SerializeField] public Vector2 holdingPosition;
    public Vector3 mousePosition;
    public Vector2 rightStickPosition;
    private bool defaultAngle = false;
    private bool isPunching = false;

    // Collision --------------------------------------------------------------
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] public BoxCollider2D hitbox;

    // Animation --------------------------------------------------------------
    [SerializeField] bool isStanding = false;
    Animator animator;
    CurrentFist currentFist = CurrentFist.Right;
    float punchAnimTimer;
    [SerializeField] SpriteRenderer spriteRenderer;

    // Stats ------------------------------------------------------------------
    [SerializeField] private float health;
    [SerializeField] float invicibilityTimer;
    [SerializeField] private Image healthbar;
    [SerializeField] private Image healthBarHelmet;
    private float healthbarStartingPos;

    // Audio ------------------------------------------------------------------
    [SerializeField] public AudioSource sfx_punchSwing;
    [SerializeField] public AudioSource sfx_punchHit;

    // Child Objects
    [SerializeField] private GameObject aimIndicator;
    [SerializeField] private GameObject crosshair;

    // Game State Change
    private bool paused;

    // Properties =======================================================================

    /// <summary>
    /// Gets whether the player is facing right or not
    /// </summary>
    public bool IsFacingRight
    {
        get
        {
            return isFacingRight;
        }
    }

    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    public PlayerState CurrentState
    {
        get
        {
            return currentState;
        }
    }

    public float InvicibilityTimer
    {
        get
        {
            return invicibilityTimer;
        }
        set
        {
            invicibilityTimer = value;
        }
    }

    // Methods ==========================================================================

    #region Unity Default Methods

    private void Awake()
    {
        newInputHandler = new NewInputHandler();
    }

    // Start is called before the first frame update
    private void Start()
    {
        healthbar.transform.position = new Vector3(healthbar.transform.position.x, healthBarHelmet.transform.position.y, 0);
        healthbarStartingPos = healthbar.transform.position.x;
        // Check for gamepad usage, aiming controls differ
        var devices = InputSystem.devices;
        for (var i = 0; i < devices.Count; ++i)
        {
            var device = devices[i];
            if (device is Joystick || device is Gamepad)
            {
                usingGamepad = true;
            }
        }

        // Stop the player from rotating
        rb.freezeRotation = true;

        // Instantiate the animator
        animator = GetComponent<Animator>();
        punchAnimTimer = .25f;

        // Hide aiming controls
        HideAimControls();
    }

    private void OnEnable()
    {
        // Bind + Enable all Player controls here
        move = newInputHandler.Player.Move;
        move.Enable();

        look = newInputHandler.Player.Look;
        look.Enable();

        aim = newInputHandler.Player.Aim;
        aim.Enable();

        newInputHandler.Player.Jump.performed += JumpAction;
        newInputHandler.Player.Jump.Enable();

        newInputHandler.Player.GroundPound.performed += GroundPound;
        newInputHandler.Player.GroundPound.Enable();

        newInputHandler.Player.Punch.performed += PunchOrThrow;
        newInputHandler.Player.Punch.Enable();

        newInputHandler.Player.Grab.performed += PickUpOrSurf;
        newInputHandler.Player.Grab.Enable();
    }

    private void OnDisable()
    {
        // Disable all Player controls here
        move.Disable();
        look.Disable();
        aim.Disable();
        newInputHandler.Player.Jump.Disable();
        newInputHandler.Player.GroundPound.Disable();
        newInputHandler.Player.Punch.Disable();
        newInputHandler.Player.Grab.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneManager.gameState == GameState.Demo)
        {
            if (paused)
            {
                move.Enable();
                look.Enable();
                aim.Enable();
                newInputHandler.Player.Jump.Enable();
                newInputHandler.Player.GroundPound.Enable();
                newInputHandler.Player.Punch.Enable();
                newInputHandler.Player.Grab.Enable();
                paused = false;
            }


            // Animate the player
            Animate();

            if (currentState != PlayerState.AirPause)
                Walk();

            // Increment the punch cooldown timer
            punchCooldownTimer += Time.deltaTime;

            // Get the position of the mouse
            mousePosition = sceneManager.mainCamera.ScreenToWorldPoint(look.ReadValue<Vector2>());

            // If the player is facing right:
            if (isFacingRight)
            {
                // Set the holding position to the right of the player
                holdingPosition = new Vector2(transform.position.x + 1.0f, transform.position.y + 1.0f);
            }
            // Otherwise:
            else
            {
                // Set the holding position to the left of the player
                holdingPosition = new Vector2(transform.position.x - 1.0f, transform.position.y + 1.0f);
            }

            if (usingGamepad)
            {
                rightStickPosition = aim.ReadValue<Vector2>() * 2;
                if (Mathf.Abs(rightStickPosition.x) <= 0.05f && Mathf.Abs(rightStickPosition.y) <= 0.05f)
                    defaultAngle = true;
                else
                    defaultAngle = false;
            }
            else
                rightStickPosition = ((Vector2)mousePosition - holdingPosition).normalized * 2;

            // Update the throwing angle
            // throwingAngle = Mathf.PI + Mathf.Atan2(rightStickPosition.y, rightStickPosition.x);
            if (!defaultAngle)
            {
                throwingAngle = Mathf.Atan2(rightStickPosition.y, rightStickPosition.x);
            }
            else
            {
                if (isFacingRight)
                {
                    throwingAngle = Mathf.PI / 4.0f;
                }
                else
                {
                    throwingAngle = 3.0f * Mathf.PI / 4.0f;
                }
            }


            // Update the position and rotation of the aim indicator
            aimIndicator.transform.position = new Vector3(holdingPosition.x, holdingPosition.y, -1.0f);
            if (isFacingRight)
            {
                aimIndicator.transform.rotation = Quaternion.Euler(0.0f, 0.0f, throwingAngle * Mathf.Rad2Deg);
            }
            else
            {
                // No idea why I need to do this to get the indicator to face the correct direction, but I do
                aimIndicator.transform.rotation = Quaternion.Euler(0.0f, 0.0f, (Mathf.PI + throwingAngle) * Mathf.Rad2Deg);
            }

            // Update the position of the crosshair
            // crosshair.transform.position = new Vector3(rightStickPosition.x + holdingPosition.x, rightStickPosition.y + holdingPosition.y, -1.0f);

            // If enough time has passed and the non-dominant fist is the current fist:
            if (punchCooldownTimer >= fistResetCooldown && currentFist == CurrentFist.Left)
            {
                // Reset the current fist to the dominant fist
                currentFist = CurrentFist.Right;
            }

            // Temporary death barrier for levels
            if (transform.position.y < -30.0f)
            {
                sceneManager.ResetScene();
            }

            if (isPunching)
            {
                CheckForPunchHit(punchCollider);
                if (airPunchCounter == 0)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0f);
                }
            }
            else if (gpLockout)
            {
                CheckForPunchHit(groundPoundCollider);
            }

            if (currentPunchMoveForce > 0)
            {
                currentPunchMoveForce -= 3 * punchMoveForce * Time.deltaTime;

                if (currentPunchMoveForce < 0)
                {
                    currentPunchMoveForce = 0;

                    // Track that the player is no longer punching
                    isPunching = false;
                }
            }
            if (currentPunchMoveForce < 0)
            {
                currentPunchMoveForce += 3 * punchMoveForce * Time.deltaTime;

                if (currentPunchMoveForce > 0)
                {
                    currentPunchMoveForce = 0;

                    // Track that the player is no longer punching
                    isPunching = false;
                }
            }

            //healthbar stuff
            invicibilityTimer -= Time.deltaTime;
            if (invicibilityTimer < 0)
            {
                invicibilityTimer = 0;
            }
            healthbar.transform.position = new Vector3(healthbarStartingPos + (200 - (Health * 2)), healthBarHelmet.transform.position.y, 0);

            //player will flash briefly after hit
            spriteRenderer.color = new Color(1, 1 - (invicibilityTimer / 2), 1 - (invicibilityTimer / 2), 1 - (invicibilityTimer / 2));

            if (isSurfingEnemy)
            {
                transform.position = new Vector2(nearbyKnockedEnemy.transform.position.x, nearbyKnockedEnemy.transform.position.y + 0.5f);
            }

            if (!isHoldingEnemy && !isSurfingEnemy && NearKnockedEnemy())
                SetKnockedEnemyColor();

            if (health <= 0)
            {
                sceneManager.Death();
            }

            if (currentState == PlayerState.AirPause)
            {
                airPauseTimer += Time.deltaTime;
                if (airPauseTimer > airPauseTime)
                {
                    currentState = PlayerState.Free;
                    airPauseTimer = 0.0f;
                }
            }
        }
        // If the game is paused:
        else if (sceneManager.gameState == GameState.Pause)
        {
            //Nothing here right now :(
            // Disable all Player controls here
            move.Disable();
            look.Disable();
            aim.Disable();
            newInputHandler.Player.Jump.Disable();
            newInputHandler.Player.GroundPound.Disable();
            newInputHandler.Player.Punch.Disable();
            newInputHandler.Player.Grab.Disable();
            paused = true;
        }

        //if player doesn't click punch again, transitions back to movement
        punchAnimTimer -= Time.deltaTime;
        if (punchAnimTimer < 0f)
        {
            animator.SetBool("IsPunching", false);
        }
    }

    // FixedUpdate is called every fixed framerate frame
    private void FixedUpdate()
    {
        if (currentState == PlayerState.AirPause)
        {
            rb.velocity = new Vector2(0.0f, 0.0f);
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
            // Update the player's velocity
            rb.velocity = new Vector2((horizontal * speed) + currentPunchMoveForce, rb.velocity.y);

            // READING MOVE FOR JUMP
            /*
            Vector2 mvXY = move.ReadValue<Vector2>();
            float angle = Mathf.Rad2Deg * Mathf.Atan2(mvXY.y, mvXY.x);

            Debug.Log("ANGLE: " + angle);

            if (angle >= 45 && angle <= 135)
            {
                Jump();
            }
            */

            if (jumpFlag)
            {
                rb.AddForce(new Vector2(0.0f, jumpingPower));
                jumpFlag = false;
                currentState = PlayerState.Free;
            }

            if (currentState == PlayerState.Free)
            {
                if (IsGrounded())
                {
                    rb.AddForce(new Vector2(horizontal * speed, 0.0f), ForceMode2D.Impulse);
                    airPunchCounter = 0;
                    isStanding = true;
                    gpLockout = false;
                }
                else
                {
                    if (!gpFlag)
                        rb.AddForce(new Vector2(horizontal * speed * 8, 0.0f), ForceMode2D.Force);
                    else
                    {
                        rb.AddForce(new Vector2(0.0f, -groundPoundForce), ForceMode2D.Impulse);
                        rb.velocity = new Vector3(0.0f, rb.velocity.y);
                        gpLockout = true;
                        gpFlag = false;
                    }
                }
            }

            if (isPunching)
            {
                if (airPunchCounter == 1)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0f);
                }
            }

            if (currentState == PlayerState.Surfing)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f);
            }
        }
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isStanding = true;
    }

    #endregion

    #region Player Animation/Direction

    /// <summary>
    /// Animates the player
    /// </summary>
    private void Animate()
    {
        // Flip the player in the correct direction
        Flip();

        // Give the player's velocities to the animator
        animator.SetFloat("xvelocity", horizontal);
        animator.SetFloat("yvelocity", rb.velocity.y);

        // If the player is standing on the ground:
        if (isStanding)
        {
            // Tell the animator that the player is no longer jumping
            animator.SetBool("IsJumping", false);
        }
    }

    /// <summary>
    /// Flips the player's sprite to the direction that they're moving in
    /// </summary>
    private void Flip()
    {
        // If the player is moving:
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            // Swap isFacingRight
            isFacingRight = !isFacingRight;

            // Flip the player's transform horizontally
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    /// <summary>
    /// Turns the player to the right
    /// </summary>
    public void TurnRight()
    {
        // Set isFacingRight to true
        isFacingRight = true;

        // Flip the player's transform to the right
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(transform.localScale.x);
        transform.localScale = localScale;

        // Set the holding position to the right of the player
        holdingPosition = new Vector2(transform.position.x + 1.0f, transform.position.y + 1.0f);
    }

    /// <summary>
    /// Turns the player to the left
    /// </summary>
    public void TurnLeft()
    {
        // Set isFacingRight to false
        isFacingRight = false;

        // Flip the player's transform to the left
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(transform.localScale.x) * -1.0f;
        transform.localScale = localScale;

        // Set the holding position to the left of the player
        holdingPosition = new Vector2(transform.position.x - 1.0f, transform.position.y + 1.0f);
    }

    #endregion

    #region Passive Input (Walk)

    /// <summary>
    /// Makes the player walk either left or right, depending on input
    /// </summary>
    public void Walk()
    {
        // Set the player's horizontal direction based on input
        horizontal = move.ReadValue<Vector2>()[0];
    }

    #endregion

    #region Button Input Actions

    /// <summary>
    /// Makes the player jump
    /// </summary>
    private void JumpAction(InputAction.CallbackContext context)
    {
        Jump();
    }

    public void Jump()
    {
        if (IsGrounded() || currentState == PlayerState.Surfing)
        {
            isStanding = false;
            if (isSurfingEnemy == true)
            {
                isSurfingEnemy = false;
                animator.SetBool("IsSurfing", false);
                rb.simulated = true;
            }
            // Tell the animator that the player is jumping
            animator.SetBool("IsJumping", true);

            jumpFlag = true;

            // rb.velocity += new Vector2(rb.velocity.x, jumpingPower) * Time.deltaTime;
        }
    }

    private void GroundPound(InputAction.CallbackContext context)
    {
        if (!IsGrounded() && currentState != PlayerState.Surfing && !gpLockout)
        {
            currentState = PlayerState.AirPause;
            gpFlag = true;
        }
    }

    private void PunchOrThrow(InputAction.CallbackContext context)
    {
        if (!isHoldingEnemy)
        {
            Punch();
        }
        else
        {
            ThrowEnemy();
        }
    }

    /// <summary>
    /// Makes the player peform a punch
    /// </summary>
    public void Punch()
    {
        if (airPunchCounter == 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f);
        }

        if (!IsGrounded())
        {
            airPunchCounter++;
        }

        punchAnimTimer = .25f;
        animator.SetBool("IsPunching", true);

        // Play the punch swing sound effect
        PlayRandomizedSFX(sfx_punchSwing);

        // If the punch cooldown has been completed:
        if (punchCooldownTimer >= punchCooldown)
        {
            // Reset the punch cooldown timer
            punchCooldownTimer = 0.0f;

            // Track that the player is currently punching
            isPunching = true;

            //If the player is facing right:
            if (isFacingRight)
            {
                // Move the player a bit rightward
                //rb.AddForce(new Vector2(750.0f, 0.0f));
                currentPunchMoveForce = punchMoveForce;
            }
            //Otherwise:
            else
            {
                // Move the player a bit leftward
                //rb.AddForce(new Vector2(-750.0f, 0.0f));
                currentPunchMoveForce = -punchMoveForce;
            }
        }

        // If the current fist being used to punch is the right one:
        if (currentFist == CurrentFist.Right)
        {
            // Make the fist for the next punch be the left fist
            currentFist = CurrentFist.Left;
            animator.SetFloat("LeftRight", 0);
        }
        // Otherwise:
        else
        {
            // Make the fist for the next punch be the right fist
            currentFist = CurrentFist.Right;
            animator.SetFloat("LeftRight", 1);
        }
    }

    /// <summary>
    /// Throws the currently held enemy
    /// </summary>
    public void ThrowEnemy()
    {
        isHoldingEnemy = false;
        ValidateThrowDirection();
        nearbyKnockedEnemy.ThrownByPlayer();
        nearbyKnockedEnemy = null;

        // Hide aiming controls
        HideAimControls();
    }

    private void PickUpOrSurf(InputAction.CallbackContext context)
    {
        if (!isHoldingEnemy && NearKnockedEnemy())
        {
            PickUpEnemy();
        }
        else if (isHoldingEnemy)
        {
            SurfEnemy();
        }
    }

    /// <summary>
    /// Picks up the nearby knocked enemy
    /// </summary>
    public void PickUpEnemy()
    {
        if (!isSurfingEnemy && nearbyKnockedEnemy.PickUpCoolDown == 0)
        {
            isHoldingEnemy = true;
            nearbyKnockedEnemy.GrabbedByPlayer(this);

            // Show aiming controls
            ShowAimControls();
        }
    }

    /// <summary>
    /// Makes the player throw and surf on the currently held enemy
    /// </summary>
    public void SurfEnemy()
    {
        animator.SetBool("IsSurfing", true);
        isHoldingEnemy = false;
        ValidateThrowDirection();
        currentState = PlayerState.Surfing; // Use this to lock the player's movement while surfing
        nearbyKnockedEnemy.ThrownByPlayer();
        isSurfingEnemy = true;
        rb.simulated = false;

        // Hide aiming controls
        HideAimControls();
    }

    #endregion

    #region Validation Functions

    /// <summary>
    /// Detects if the player is on the ground
    /// </summary>
    /// <returns>Whether the player is on the ground or not</returns>
    public bool IsGrounded()
    {
        return Physics2D.OverlapArea(new Vector2(groundCheck.position.x - (groundCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            groundCheck.position.y + (groundCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            new Vector2(groundCheck.position.x + (groundCheck.GetComponent<SpriteRenderer>().bounds.size.x / 2),
            groundCheck.position.y - (groundCheck.GetComponent<SpriteRenderer>().bounds.size.y / 2)),
            groundLayer);
    }

    /// <summary>
    /// Turns the player around if they are aiming to throw behind themselves
    /// </summary>
    private void ValidateThrowDirection()
    {
        // If the player is facing right and the mouse is behind them:
        if (isFacingRight && (rightStickPosition.x + transform.position.x) > transform.position.x)
        {
            // Turn them around
            TurnLeft();
        }
        // Otherwise, if the player is facing left and the mouse is behind them:
        else if (!isFacingRight && (rightStickPosition.x + transform.position.x) < transform.position.x)
        {
            // Turn them around
            TurnRight();
        }
    }

    /// <summary>
    /// Checks to see if a punch hit something
    /// </summary>
    private void CheckForPunchHit(Collider2D collider)
    {
        // Initialize a list to hold all colliders that collide with the punch
        List<Collider2D> contacts = new List<Collider2D>();

        // Fill the list with all contacts
        collider.OverlapCollider(new ContactFilter2D().NoFilter(), contacts);

        // Make a boolean to track if anything was punched
        bool successfulHit = false;

        // For each contact point in the punch collider:
        for (int i = 0; i < contacts.Count; i++)
        {
            // Create a temporary enemy object
            Enemy tempEnemy;

            // Create a temporary tilemap collider 2D
            TilemapCollider2D tempTilemapCollider2D;

            // Create a temporary breakable block collider 2D
            BreakableBlock tempBreakableBlock;

            // If the current overlapping collider belongs to an enemy:
            if (contacts[i].gameObject.TryGetComponent<Enemy>(out tempEnemy))
            {
                // If the enemy is alive:
                if (tempEnemy.CurrentState == EnemyStates.Alive)
                {
                    // If the player is facing right:
                    if (isFacingRight)
                    {
                        // Punch the enemy
                        tempEnemy.Punched(new Vector2(300.0f, 200.0f));
                    }
                    // Otherwise:
                    else
                    {
                        // Punch the enemy
                        tempEnemy.Punched(new Vector2(-300.0f, 200.0f));
                    }

                    // Mark that there has been a successful hit
                    successfulHit = true;
                }
            }
            // Otherwise, if the current overlapping collider belongs to a wall:
            else if (contacts[i].gameObject.TryGetComponent<TilemapCollider2D>(out tempTilemapCollider2D))
            {
                // Mark that there has been a successful hit
                successfulHit = true;
            }
            // Otherwise, if the current overlapping collider belongs to a breakable block:
            else if (contacts[i].gameObject.TryGetComponent<BreakableBlock>(out tempBreakableBlock))
            {
                // Destroy the block
                tempBreakableBlock.Break();

                // Mark that there has been a successful hit
                successfulHit = true;
            }
        }

        // If the player missed their punch:
        if (successfulHit)
        {
            // Make the player recoil
            currentPunchMoveForce *= -recoilMultiplier;

            rb.AddForce(new Vector2(0.0f, jumpingPower / 3));

            // Track that the player is no longer punching
            isPunching = false;

            // Shake the camera
            cameraActions.Shake(0.1f, 0.02f);

            PlayRandomizedSFX(sfx_punchHit);
        }
    }

    /// <summary>
    /// Detects whether the player is near a knocked enemy or not
    /// </summary>
    /// <returns>Whether the player is near a knocked enemy or not</returns>
    public bool NearKnockedEnemy()
    {
        // Get the amount of colliders that there are near the player
        int knockedEnemyColliders = Physics2D.OverlapCircle(transform.position, pickUpRadius, new ContactFilter2D().NoFilter(), nearbyColliders);

        // For each collider within picking-up radius:
        for (int i = knockedEnemyColliders - 1; i >= 0; i--)
        {
            // Create a temporary throwable enemy object
            ThrowableEnemy temp;

            // If the current nearby collider belongs to a throwable enemy:
            if (nearbyColliders[i].gameObject.TryGetComponent<ThrowableEnemy>(out temp))
            {
                // If the current enemy is knocked:
                if (temp.BaseEnemy.CurrentState == EnemyStates.Knocked)
                {
                    // Store a reference to the enemy in temp
                    nearbyKnockedEnemy = temp;

                    // Forget about all other nearby colliders
                    nearbyColliders.Clear();


                    return true;
                }
            }
        }

        // Forget about all of the nearby colliders
        nearbyColliders.Clear();

        return false;
    }

    #endregion

    #region Aim Controls Visibility

    /// <summary>
    /// Shows the crosshair and aim indicator
    /// </summary>
    private void ShowAimControls()
    {
        // Enable the crosshair
        //crosshair.SetActive(true);

        // Enable the aim indicator
        aimIndicator.SetActive(true);

    }

    /// <summary>
    /// Hides the crosshair and aim indicator
    /// </summary>
    private void HideAimControls()
    {
        // Disable the crosshair
        crosshair.SetActive(false);

        // Disable the aim indicator
        aimIndicator.SetActive(false);
    }

    /// <summary>
    /// Randomizes the speed and pitch of a sound, and plays it
    /// </summary>
    /// <param name="sound">The AudioSource to be played</param>
    private void PlayRandomizedSFX(AudioSource sound)
    {
        if (sound.pitch != 1f) { sound.pitch = 1f; } // Reset

        sound.pitch += Random.Range(-0.40f, 0.80f);
        sound.Play();
    }
    #endregion

    #region Etc. Helpers
    private void SetKnockedEnemyColor()
    {
        if (nearbyKnockedEnemy != null)
            nearbyKnockedEnemy.SetColor();
    }
    #endregion

    #region Not In Use

    /// <summary>
    /// Prevents the player from falling too fast
    /// 11/13/24 - Not In Use
    /// </summary>
    public void HoldingJump()
    {
        isStanding = false;
        rb.velocity += new Vector2(rb.velocity.x, rb.velocity.y * 0.5f) * Time.deltaTime;
    }

    /// <summary>
    /// Drops the currently held enemy
    /// 11/13/24 - Not in Use
    /// </summary>
    public void DropEnemy()
    {
        isHoldingEnemy = false;
        nearbyKnockedEnemy.DroppedByPlayer();
        nearbyKnockedEnemy = null;

        // Hide aiming controls
        HideAimControls();
    }

    #endregion
}