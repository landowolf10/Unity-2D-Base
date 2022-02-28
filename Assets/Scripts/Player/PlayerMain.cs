using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;

    [Header("Animations")]
    private Animator animator;

    [Header("Layer masks")]
    [SerializeField]
    private LayerMask groundLayer;

    [Header("Movement variables")]
    [SerializeField] private float movementAcceleration = 70;
    [SerializeField] private float maxMoveSpeed = 12;
    [SerializeField] private float linearDrag;
    private float horizontalInput;
    private bool changingDirection => (rb.velocity.x > 0f && horizontalInput < 0f) || (rb.velocity.x < 0f && horizontalInput > 0f);
    private bool facingRight = true;

    [Header("Jump variables")]
    [SerializeField] private float jumpForce = 12;
    [SerializeField] private float gravity = 1f;
    [SerializeField] private float fallMultiplier = 5f;
    [SerializeField] private bool canJump;
    [SerializeField] private float hangTime;
    private float hangTimeCounter;

    [Header("Ground collision variables")]
    [SerializeField] private Transform groundCheck1;
    [SerializeField] private Transform groundCheck2;

    private PlayerController playerController;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerController = new PlayerController();
    }

    void Update()
    {
        horizontalInput = playerController.playerInput().x;
        //Dictionary that stores true or false depending if each transform object is on the ground.
        Dictionary<string, bool> onGround = isGrounded(groundCheck1, groundCheck2, groundLayer);

        if (Input.GetButtonDown("Jump") && (onGround["grounded1"] || onGround["grounded2"]))
            canJump = true;
    }

    void FixedUpdate()
    {
        playerController.moveCharacter(rb, horizontalInput, movementAcceleration, maxMoveSpeed);

        if(canJump)
        {
            jump(rb, jumpForce);
            canJump = false;
        }

        applyLinearDrag();
    }

    private void applyLinearDrag()
    {
        //Begins to deaccelerate if the horizontal input is less than 0.4 or the
        //player is changing the direction of the movement.
        if (Mathf.Abs(horizontalInput) < 0.4f || changingDirection)
            rb.drag = linearDrag;
        else
            rb.drag = 0;
    }

    public void jump(Rigidbody2D rb, float jumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); //Reset vertical velocity
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public Dictionary<string, bool> isGrounded(Transform groundCheck1, Transform groundCheck2,
        LayerMask groundLayer)
    {
        bool grounded1 = Physics2D.OverlapCircle(groundCheck1.position, 0.2f, groundLayer);
        bool grounded2 = Physics2D.OverlapCircle(groundCheck2.position, 0.2f, groundLayer);

        Dictionary<string, bool> onGround = new Dictionary<string, bool>();
        onGround.Add("grounded1", grounded1);
        onGround.Add("grounded2", grounded2);

        return onGround;
    }
}