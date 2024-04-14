using Mirror;
using Mirror.Examples.NetworkRoom;
using Mirror.Examples.TankTheftAuto;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransformReliable))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    public static bool CanMove { get; set; } = true;
    private bool ShouldJump => Input.GetKeyDown(jumpKey);
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldLowkick => Input.GetKeyDown(lowkickKey);
    private bool ShouldUpperkick => Input.GetKeyDown(upperkickKey);
    private bool ShouldToogleCamera => Input.GetKeyDown(toogleCameraKey);

    [Header("Functional options")]
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canToogleCamera = true;
    [SerializeField] private bool canLowkicking = true;
    [SerializeField] private bool canUpperkicking = true;

    [Header("Controls")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode toogleCameraKey = KeyCode.F1;
    [SerializeField] private KeyCode lowkickKey = KeyCode.C;
    [SerializeField] private KeyCode upperkickKey = KeyCode.X;

    [Header("Movement parameters")]
    [SerializeField] private float walkingSpeed = 100f;
    [SerializeField] private float sprintSpeed = 8f;


    [Header("Look parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 40f;

    [Header("Jump parameters")]
    [SerializeField] private float jumpForce = 18;
    [SerializeField] private float gravity = 30;

    [Header("Camera settings")]
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] public Vector3 playerCameraPosition;

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;


    private Interactable currentInteractable;

    [SerializeField] private CharacterController characterController;

    public Camera playerCamera;
    public ShotManager shotManager;
    private Animator animator;
    private PlayerScore playerScore;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0f;

    private bool isFirstPerson = true;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        GetComponent<Rigidbody>().isKinematic = true;

        characterController.enabled = false;
        characterController.skinWidth = 0.02f;
        characterController.minMoveDistance = 0f;

        //GetComponent<PlayerScore>().enabled = false;
        GetComponent<ShotManager>().enabled = false;
        this.enabled = false;
    }

    public override void OnStartAuthority()
    {
        shotManager = GetComponent<ShotManager>();
        shotManager.ball = GameObject.FindWithTag("Ball");
        playerScore = GetComponent<PlayerScore>();  

        Debug.Log("OnStartAuthority");
        shotManager.lowkickHelper = GameObject.FindWithTag("LowkickHelper");
        shotManager.ballRigidbody = shotManager.ball.GetComponent<Rigidbody>();

        //NetworkIdentity ballNetIdentity = shotManager.ball.GetComponent<NetworkIdentity>();
        //ballNetIdentity.AssignClientAuthority(connectionToClient);

        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerScore.enabled = true;
        shotManager.enabled = true;
        characterController.enabled = true;
        this.enabled = true;
    }

    public override void OnStopAuthority()
    {
        this.enabled = false;
        characterController.enabled = false;
    }

    void Update()
    {
        if (!characterController.enabled)
            return;

        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump)
            {
                HandleJump();
            }
            if (canLowkicking)
            {
                HandleLowkick();
                HandleInteractionCheck();
                //HandleInteractionInput();
            }
            if (canUpperkicking)
            {
                HandleUpperkick();
            }

            ApplyFinalMovements();
        }
    }

    private void ToggleCamera()
    {
        if (ShouldToogleCamera)
        {
            isFirstPerson = !isFirstPerson;

            firstPersonCamera.gameObject.SetActive(isFirstPerson);
            //thirdPersonCamera.gameObject.SetActive(!isFirstPerson);
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((IsSprinting ? sprintSpeed : walkingSpeed) * Input.GetAxisRaw("Vertical"), (IsSprinting ? sprintSpeed : walkingSpeed) * Input.GetAxisRaw("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;

        if (currentInput != Vector2.zero)
        {
            animator.SetBool("IsMoving", true);
            //animator.SetBool("Lowkick", false);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private void HandleMouseLook()
    {
        if (playerCamera != null)
        {
            rotationX -= Input.GetAxisRaw("Mouse Y") * lookSpeedY;
            rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, transform.localRotation.y, 0f);
            transform.rotation *= Quaternion.Euler(0f, Input.GetAxisRaw("Mouse X") * lookSpeedX, 0f);
        }
    }

    private void HandleJump()
    {
        if (ShouldJump)
        {
            moveDirection.y = jumpForce;
        }
    }

    private void HandleLowkick()
    {
        if (ShouldLowkick)
        {
            CmdAssignAuthority(shotManager.ball.GetComponent<NetworkIdentity>());

            shotManager.Lowkick(this.netId);
        }
    }

    private void HandleUpperkick()
    {
        if (ShouldUpperkick)
        {
            CmdAssignAuthority(shotManager.ball.GetComponent<NetworkIdentity>());

            shotManager.UpperKick(this.netId);
        }
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject.layer == 7 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(lowkickKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();

            //animator.Play("Lowkick");
        }
    }

    private void ApplyFinalMovements()
    {
        //if (moveDirection.y > 0)
        //{
        //    moveDirection.y -= gravity * Time.deltaTime;
        //}

        //transform.position += moveDirection * Time.deltaTime;


        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    [Command]
    public void CmdAssignAuthority(NetworkIdentity _networkIdentity)
    {
        // Debug.Log("Mirror Object owner set to: " + this.netIdentity);

        //shotManager.ball = _networkIdentity.GetComponent<TankController>();

        // so we dont assign it to same person again
        _networkIdentity.RemoveClientAuthority();
        _networkIdentity.AssignClientAuthority(connectionToClient);
        //if (tankController.objectOwner != this.netIdentity)
        //{
        //    // commands are a good place to do additional validation/cheat checks, but these are left out for simplicity here

        //    tankController.objectOwner = this.netIdentity;
        //}
    }
}
