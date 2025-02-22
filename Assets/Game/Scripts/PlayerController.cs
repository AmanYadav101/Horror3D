using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _controller;
    public Transform cameraTransform;
    public Animator handAnimator;
    
    public float moveSpeed = 4f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 2f;
    public float sprintMultiplier = 1.5f;

    private float xRotation = 0f;
    private Vector3 velocity;

    void Awake()
    {
        _controller = gameObject.GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MovePlayer();
        LookAround();
    }

    void MovePlayer()
    {
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= sprintMultiplier; 

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        _controller.Move(move * (speed * Time.deltaTime));

        velocity.y += gravity * Time.deltaTime;
        _controller.Move(velocity * Time.deltaTime);
        
        float movementMagnitude = new Vector3(x, 0, z).magnitude;
        handAnimator.SetFloat("MoveSpeed", movementMagnitude * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f));
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 60f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}