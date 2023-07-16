using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Move Info")]
    [SerializeField]private float moveSpeed;
    [SerializeField]private float jumpForce;


    [Header("Collision Info")]
   [SerializeField] private float groundCheckDistance;
   [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    private bool isRunning;
    private bool runBegun;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        AnimatorControllers();

        if (runBegun)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);

        CheckInput();

    }

    private void AnimatorControllers()
    {
        isRunning = rb.velocity.x != 0;
        anim.SetBool("isRunning", isRunning);
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            runBegun = true;
        }

        if (Input.GetKeyDown(KeyCode.Space)&& isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
    }
}
