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
    [SerializeField] private float linearDrag = 7;
    private float horizontalDirection;
    private bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalDirection = playerInput().x;
    }

    void FixedUpdate()
    {
        moveCharacter();
        applyLinearDrag();
    }

    private Vector2 playerInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void moveCharacter()
    {
        rb.AddForce(new Vector2(horizontalDirection, 0f) * movementAcceleration);

        Debug.Log("Rigid body velocity: " + rb.velocity.x);
        Debug.Log("Rigid body velocity sign: " + Mathf.Sign(rb.velocity.x));

        //Clamps the velocity when player reaches max speed.
        if (Mathf.Abs(rb.velocity.x) > maxMoveSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeed, rb.velocity.y);
    }

    private void applyLinearDrag()
    {
        if (Mathf.Abs(horizontalDirection) < 0.4f)
            rb.drag = linearDrag;
        else
            rb.drag = 0;
    }

    //4:45 p.m.
}