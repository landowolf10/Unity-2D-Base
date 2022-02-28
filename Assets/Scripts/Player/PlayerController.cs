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
    }
}
