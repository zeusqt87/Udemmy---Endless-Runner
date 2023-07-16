using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool runBegun;
    public float moveSpeed;
    public float jumpForce;

    public Rigidbody2D rb;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (runBegun)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

        CheckInput();

    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            runBegun = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        }
    }
}
