using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static bool CanMove { get; set; } = true;
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldLowkick => Input.GetKeyDown(lowkickKey) && characterController.isGrounded;
    private bool ShouldToogleCamera => Input.GetKeyDown(toogleCameraKey);

    [Header("Functional options")]
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canToogleCamera = true;
    [SerializeField] private bool canLowkicking = true;

    [Header("Controls")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode toogleCameraKey = KeyCode.F1;
    [SerializeField] private KeyCode lowkickKey = KeyCode.L;

    [Header("Movement parameters")]
    [SerializeField] private float walkingSpeed = 3f;

    [Header("Look parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 40f;

    [Header("Jump parameters")]
    [SerializeField] private float jumpForce = 8;
    [SerializeField] private float gravity = 30;

    [Header("Camera settings")]
    [SerializeField] private Transform firstPersonCamera;
    [SerializeField] private Transform thirdPersonCamera;

    private Animator animator;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0f;

    private bool isFirstPerson = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
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
            animator.SetBool("Lowkick", false);
            Debug.Log("Is moving");
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        Debug.Log($"First is pressed {moveDirection}");
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxisRaw("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
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
            animator.SetBool("Lowkick", true);
            Debug.Log($"When C is pressed {moveDirection}");
        }
    }

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        Debug.Log($"Final is pressed {moveDirection}");

    }
}
