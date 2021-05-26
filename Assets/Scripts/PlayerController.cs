using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 12f;
    public float sprintSpeed = 24f;
    public float mouseSensitivity = 30f;
    public float jumpHeight = 3f;
    public float gravity = 19.62f;
    public Camera attachedCamera;
    public LayerMask groundMask;
    public Transform groundCheck;
    public TextMeshProUGUI counter;
    public TextMeshProUGUI endOfGame;

    private Vector2 movement;
    private Vector2 mouseMovement;
    private CharacterController controller;
    private float xRotation = 0f;
    private bool isSprinting = false;
    private bool isJumping = false;
    private Vector3 verticalVelocity = Vector3.zero;
    private bool isGrounded;
    private float numberOfKeys = 0f;
    private float numberOfCollectedKeys = 0f;

    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        numberOfKeys = GameObject.FindGameObjectsWithTag("key").Length;
        endOfGame.gameObject.SetActive(false);
        PrintKeysCounter();
    }

    void Update()
    {
        float newSpeed = CalculateSpeed();
        Vector3 moveVector = (transform.right * movement.x + transform.forward * movement.y) * newSpeed * Time.deltaTime;
        controller.Move(moveVector);

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, groundMask);

        if (isGrounded)
        {
            verticalVelocity.y = 0f;
            if (isJumping)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
                controller.Move(verticalVelocity * Time.deltaTime);
            }
        }
        else
        {
            verticalVelocity.y -= gravity * Time.deltaTime;
            controller.Move(verticalVelocity * Time.deltaTime);
        }


        Look();
        OnEscape();
    }

    private void Look()
    {
        mouseMovement *= mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, mouseMovement.x);

        xRotation -= mouseMovement.y;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        attachedCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        movement = ctx.ReadValue<Vector2>();
    }

    public void OnMouseMovement(InputAction.CallbackContext ctx)
    {
        mouseMovement = ctx.ReadValue<Vector2>();
    }

    private void OnEscape()
    {
        // itt valamiért nem tudtam megfelelő actiont kiváltani az input systemben
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        isSprinting = !isSprinting;
        Debug.Log(isSprinting);
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        isJumping = ctx.performed;
    }

    private float CalculateSpeed()
    {
        if (isSprinting && isGrounded)
        {
            return sprintSpeed;
        }
        return speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "key")
        {
            numberOfCollectedKeys++;
            PrintKeysCounter();
            other.gameObject.SetActive(false);
            if(numberOfCollectedKeys == numberOfKeys)
            {
                endOfGame.gameObject.SetActive(true);
            }
        }
    }

    private void PrintKeysCounter()
    {
        counter.text = $"Begyűjtött kulcsok: {numberOfCollectedKeys}/{numberOfKeys}";
    }
}
