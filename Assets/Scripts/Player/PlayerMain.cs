using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;

    [Header("Layer masks")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement variables")]
    public PlayerController pc;
    private float horizontalInput;
    private bool facingRight = true;

    [Header("Jump variables")]
    //[SerializeField] private float jumpForce = 20f;
    //[SerializeField] private float jumpLinearDrag = 2.5f;
    [SerializeField] private float _fallMultiplier = 8f;
    [SerializeField] private float lowJumpFallMultiplier = 5f;
    [SerializeField] private bool canJump;
    [SerializeField] private float hangTime;
    private float hangTimeCounter;

    [Header("Ground collision variables")]
    [SerializeField] private Transform groundCheck1;
    [SerializeField] private Transform groundCheck2;
    [SerializeField] private Dictionary<string, bool> _onGround;

    [Header("Animations")]
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        pc = new PlayerController();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = pc.playerInput().x;

        //Dictionary that stores true or false depending if each transform object is on the ground.
        Dictionary<string, bool> onGround = pc.isGrounded(groundCheck1, groundCheck2, groundLayer);

        if (Input.GetButtonDown("Jump") && hangTimeCounter > 0)
            canJump = true;

        if ((pc.playerInput().x > 0 && !facingRight) || (pc.playerInput().x < 0 && facingRight))
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

    private void FixedUpdate()
    {
        Dictionary<string, bool> onGround = pc.isGrounded(groundCheck1, groundCheck2, groundLayer);
        pc.moveCharacter(rb);
        //pc.applyGroundLinearDrag(rb, horizontalInput, groundLinearDrag);

        if (canJump)
        {
            pc.jump(rb);
            canJump = false;
            hangTimeCounter = 0f;
        }

        if (onGround["grounded1"] || onGround["grounded2"])
        {
            pc.applyGroundLinearDrag(rb, horizontalInput);
        }
        else
        {
            pc.applyJumpLinearDrag(rb);
            pc.fallMultiplier(rb, _fallMultiplier, lowJumpFallMultiplier);
        }
    }

    private void LateUpdate()
    {
        Dictionary<string, bool> onGround = pc.isGrounded(groundCheck1, groundCheck2, groundLayer);
        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", false);

        //Run animation
        animator.SetBool("isGrounded", (onGround["grounded1"] || onGround["grounded2"]));
        animator.SetFloat("horizontalDirection", Mathf.Abs(horizontalInput));

        //Jump animation
        if (!onGround["grounded1"] || !onGround["grounded2"])
            animator.SetBool("isJumping", true);

        //Fall animation
        if (rb.velocity.y < 0f)
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }
    }

    public void flipCharacter()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }
}