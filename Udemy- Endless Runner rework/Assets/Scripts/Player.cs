using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private bool isDead;

    [Header("Knock Back Info")]
    [SerializeField] private Vector2 knockbackDirection;
    private bool isKnocked;
    private bool canBeKnocked = true;
    private bool playerUnlocked;

    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedMultiplier;
    [Space]
    private float defaultSpeed;
    [SerializeField] private float milestoneIncreaser;
    private float defaultMilestoneIncrease;
    private float speedMilestone;

    [Header("Jump Info")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;

    [Header("Collision Info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float ceillingCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;

    private bool wallDetected;
    private bool isGrounded;
    private bool ceillingDetected;

    [Header("Slide Info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCooldown;
    private float slideCooldownCounter;
    private float slideTimeCounter;
    private bool isSliding;
    [HideInInspector] public bool ledgeDetected;


    [Header("Ledge Info")]
    [SerializeField] private Vector2 offset1;//before climb
    [SerializeField] private Vector2 offset2;//after climb

    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGrabLedge = true;
    private bool canClimb;



    // Start is called before the first frame update
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        speedMilestone = milestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncrease = milestoneIncreaser;

    }

    // Update is called once per frame
    void Update()
    {
        AnimatorControllers();


        if (Input.GetKeyDown(KeyCode.K))
        {
            Knockback();
        }
        if (Input.GetKeyDown(KeyCode.O)&&!isDead)
        {
         StartCoroutine(Die());
        }

        slideTimeCounter -= Time.deltaTime;
        slideCooldownCounter -= Time.deltaTime;

        if (isDead)
            return;

        if (isKnocked)
            return;

        if (playerUnlocked)
        {
            SetupMovement();
        }

        if (isGrounded)
        {
            canDoubleJump = true;
        }
      

        SpeedController();
        CheckForLedge();
        CheckInput();
        CheckCollision();
        CheckForSlideCancel();
       
    }

    public void Damage()
    {
        if (moveSpeed >= maxSpeed)
            Knockback();
        else
           StartCoroutine(Die());

    }

    private IEnumerator Die()
    {
        canBeKnocked = false;
        isDead = true;
        rb.velocity = knockbackDirection;
        anim.SetBool("isDead", true);

        yield return new WaitForSeconds(1f);
        rb.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(1f);
        GameManager.instance.RestartLevel();

    }

    private IEnumerator Invincibility()
    {
        Color originalColor = sr.color;
        Color darkenColor = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
        canBeKnocked = false;

        sr.color = darkenColor;
        yield return new WaitForSeconds(.1f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.1f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.1f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.15f);
        sr.color = darkenColor;
        yield return new WaitForSeconds(.15f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.25f);
        sr.color = darkenColor;
        yield return new WaitForSeconds(.25f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.35f);
        sr.color = darkenColor;
        yield return new WaitForSeconds(.35f);
        sr.color = originalColor;
        yield return new WaitForSeconds(.40f);






        canBeKnocked = true;
    }

    private void Knockback()
    {
        if (!canBeKnocked)
            return;
        StartCoroutine(Invincibility());
        isKnocked = true;
        rb.velocity = knockbackDirection;
    }

    private void CancelKnockback() => isKnocked = false;
   
    #region Speedcontroll
    private void SpeedReset()
    {
        moveSpeed = defaultSpeed;
        milestoneIncreaser = defaultMilestoneIncrease;
    }

    private void SpeedController()
    {
        if (moveSpeed == maxSpeed)
            return;

        if (transform.position.x > speedMilestone)
        {
            speedMilestone = speedMilestone + milestoneIncreaser;

            moveSpeed *= speedMultiplier;
            milestoneIncreaser *= speedMultiplier;
            if (moveSpeed > maxSpeed)
                moveSpeed = maxSpeed;
        }
    }
    #endregion

    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge) { 
            canGrabLedge = false;
            rb.gravityScale = 0;

            Vector2 ledgePosition = GetComponentInChildren<ledgeDetection>().transform.position;
            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = ledgePosition + offset2;
            canClimb = true;

        }
        if (canClimb)
            transform.position = climbBegunPosition;

    }
    private void LedgeClimbOver()
    {
        canClimb = false;
        rb.gravityScale = 5;
        transform.position = climbOverPosition;
        Invoke("AllowLedgeGrab", 0.1f);
    }

    private void AllowLedgeGrab() => canGrabLedge = true;
    

    private void CheckForSlideCancel()
    {
        if (slideTimeCounter < 0 && !ceillingDetected)
            isSliding = false;
    }

    private void SetupMovement()
    {
        if (wallDetected)
        {
            SpeedReset();
            return;
        }
        if (isSliding)
        {
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y);
        }
        else
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    #region Inputs

    private void SlideButton()
    {
        if (rb.velocity.x != 0&& slideCooldownCounter<0)
        {
        isSliding = true;
        slideTimeCounter = slideTime;
        slideCooldownCounter = slideCooldown;

        }
    }

    private void JumpButton()
    {
        if (isSliding)
            return;
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);    
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            playerUnlocked = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();

        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            SlideButton();
        }
    }
    #endregion

    #region Animations
    private void AnimatorControllers()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);

        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canClimb", canClimb);

        if (rb.velocity.y < -20)
            anim.SetBool("canRoll", true);
        anim.SetBool("isKnocked", isKnocked);
    }

    private void AnimFinished() => anim.SetBool("canRoll", false);

    #endregion

    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        ceillingDetected = Physics2D.Raycast(transform.position, Vector2.up, ceillingCheckDistance, whatIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceillingCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
