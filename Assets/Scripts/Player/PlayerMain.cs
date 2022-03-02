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
    [SerializeField] private float groundLinearDrag = 10f;
    private float horizontalInput;
    private bool changingDirection => (rb.velocity.x > 0f && horizontalInput < 0f) || (rb.velocity.x < 0f && horizontalInput > 0f);
    private bool facingRight = true;

    [Header("Jump variables")]
    [SerializeField] private float jumpForce = 12;
    [SerializeField] private float jumpLinearDrag = 2.5f;
    [SerializeField] private float gravity = 1f;
    [SerializeField] private float _fallMultiplier = 8f;
    [SerializeField] private float lowJumpFallMultiplier = 5f;
    [SerializeField] private bool canJump = true;
    Dictionary<string, bool> onGround;
    [SerializeField] private float hangTime = 0.2f;
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
        onGround = playerController.isGrounded(groundCheck1, groundCheck2, groundLayer);

        if (Input.GetButtonDown("Jump") && hangTimeCounter > 0)
            canJump = true;

        //Run animation
        animator.SetBool("isGrounded", (onGround["grounded1"] || onGround["grounded2"]));
        animator.SetFloat("horizontalDirection", Mathf.Abs(horizontalInput));

        if ((horizontalInput > 0 && !facingRight) || horizontalInput < 0 && facingRight)
            flipCharacter();

        if ((!onGround["grounded1"] || !onGround["grounded2"]))
        {
            //If the player is not touching the ground, then the hang time counter begins to decrease,
            //and while the counter is greater than the hang time (0.2) the player can jump.
            hangTimeCounter -= Time.deltaTime;
            return;
        }

        //Hang time counter will have the hang time value (0.2) only if the player is on the ground,
        //and if the hang time counter is greater than 0 (in this case it is because it now has the 0.2 value)
        //then the player can jump.
        //In other words, the player only has 0.2 seconds to jump after leaving the ground.
        hangTimeCounter = hangTime;
    }

    void FixedUpdate()
    {
        playerController.moveCharacter(rb, horizontalInput, movementAcceleration, maxMoveSpeed);
        onGround = playerController.isGrounded(groundCheck1, groundCheck2, groundLayer);

        if (canJump)
        {
            playerController.jump(rb, jumpForce);
            canJump = false;
            hangTimeCounter = 0f;
        }

        if ((onGround["grounded1"] || onGround["grounded2"]))
            playerController.applyGroundLinearDrag(rb, horizontalInput, changingDirection, groundLinearDrag);
        else
        {
            playerController.applyJumpLinearDrag(rb, jumpLinearDrag);
            playerController.fallMultiplier(rb, _fallMultiplier, lowJumpFallMultiplier);
        }
    }

    public void flipCharacter()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }
}