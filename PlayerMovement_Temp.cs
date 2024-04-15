using UnityEngine;

public class PlayerMovement_Temp : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float movementMultiplier = 10f;
    public KeyCode JumpKey = KeyCode.Space;
    public float jumpforce = 5f;
    public float airMultiplier = 0.5f;

    [Header("Drag")]
    public float groundDrag = 6f;
    public float airDrag = 1f;
    public float groundDistance = 0.4f;
    public float extraGravity = 1f;

    [SerializeField] private Transform orientation;
    [SerializeField] LayerMask groundMask;

    float playerHeight;

    float horizontalMovement;
    float verticalMovement;

    bool isGrounded;

    Vector3 moveDirection;

    public Rigidbody rb;
    public RaycastHit slopeHit;
    public Vector3 slopeMoveDirection;

    private void Start()
    {
        orientation = GameObject.Find("Player").transform.Find("Orientation").GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        isGrounded = true;
        playerHeight = transform.localScale.y * 2;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0,1,0), groundDistance, groundMask);
        MyInput();
        ControlDrag();
        if(Input.GetKeyDown(JumpKey) && isGrounded)
        {
            Jump();
        }
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal); 
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if(slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }
    
    private void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
            rb.AddForce(-transform.up * extraGravity, ForceMode.Acceleration);
        }
    }

    private void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection * moveSpeed * airMultiplier * movementMultiplier, ForceMode.Acceleration);
        }
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpforce, ForceMode.Impulse);
    }
}