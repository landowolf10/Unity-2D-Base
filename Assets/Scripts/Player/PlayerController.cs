using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{
    public Vector2 playerInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public void moveCharacter(Rigidbody2D rb, float horizontalInput, float movementAcceleration, float maxMoveSpeed)
    {
        rb.AddForce(new Vector2(horizontalInput, 0f) * movementAcceleration);

        //Clamps the velocity when player reaches max speed.
        if (Mathf.Abs(rb.velocity.x) > maxMoveSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeed, rb.velocity.y);

        Debug.Log("Rigid body velocity: " + rb.velocity);
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

    public void applyGroundLinearDrag(Rigidbody2D rb, float horizontalInput, bool changingDirection, float groundLinearDrag)
    {
        //Begins to deaccelerate if the horizontal input is less than 0.4 or the
        //player is changing the direction of the movement.
        if (Mathf.Abs(horizontalInput) < 0.4f || changingDirection)
            rb.drag = groundLinearDrag;
        else
            rb.drag = 0;
    }

    public void applyJumpLinearDrag(Rigidbody2D rb, float jumpLinearDrag)
    {
        rb.drag = jumpLinearDrag;
    }

    public void fallMultiplier(Rigidbody2D rb, float _fallMultiplier, float lowJumpFallMultiplier)
    {
        if (rb.velocity.y < 0)
            rb.gravityScale = _fallMultiplier;
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.gravityScale = lowJumpFallMultiplier;
        else
            rb.gravityScale = 1f;
    }
}
