using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static bool CanMove { get; set; } = true;
    private bool ShouldJump => Input.GetKeyDown(jumpKey);
    private bool ShouldLowkick => Input.GetKeyDown(lowkickKey);
    private bool ShouldUpperkick => Input.GetKeyDown(upperkickKey);
    private bool ShouldToogleCamera => Input.GetKeyDown(toogleCameraKey);

    [Header("Functional options")]
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canToogleCamera = true;
    [SerializeField] private bool canLowkicking = true;
    [SerializeField] private bool canUpperkicking = true;

    [Header("Controls")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode toogleCameraKey = KeyCode.F1;
    [SerializeField] private KeyCode lowkickKey = KeyCode.C;
    [SerializeField] private KeyCode upperkickKey = KeyCode.X;

    [Header("Movement parameters")]
    [SerializeField] private float walkingSpeed = 3f;

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
    [SerializeField] private Camera thirdPersonCamera;

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;

    private Interactable currentInteractable;

    private Animator animator;

    private Camera playerCamera;
    private ShotManager shotManager;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0f;

    private bool isFirstPerson = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerCamera = firstPersonCamera;
        shotManager = GetComponent<ShotManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canToogleCamera)
            {
                ToggleCamera();
            }
            if (canJump)
            {
                HandleJump();
            }
            if (canLowkicking)
            {
                HandleLowkick();
                //HandleInteractionCheck();
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
            thirdPersonCamera.gameObject.SetActive(!isFirstPerson);
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2(walkingSpeed * Input.GetAxisRaw("Vertical"), walkingSpeed * Input.GetAxisRaw("Horizontal"));

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
        rotationX -= Input.GetAxisRaw("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, transform.localRotation.y, 0f);
        transform.rotation *= Quaternion.Euler(0f, Input.GetAxisRaw("Mouse X") * lookSpeedX, 0f);
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
            shotManager.Lowkick();
        }
    }

    private void HandleUpperkick()
    {
        if (ShouldUpperkick)
        {
            shotManager.UpperKick();
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

            animator.Play("Lowkick");
        }
    }

    private void ApplyFinalMovements()
    {
        if (moveDirection.y > 0)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        transform.position += moveDirection * Time.deltaTime;
    }
}
