using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // reference
    SpriteRenderer sprite;
    Rigidbody2D rb;
    BoxCollider2D coll;
    Animator anim;

    // speed
    [Header("Speed")]
    [SerializeField] float runSpeed = 12f;
    [SerializeField] float jumpSpeed = 24f;
    [SerializeField] float wallSlideSpeed = 4f;

    // input
    float hAxisInput = 0;
    [Header("Input Buffer Time")]
    [SerializeField] float jumpButtonDownBufferTime = .1f;
    float jumpButtonDownBufferTimer = 0;

    // collide state
    bool isGrounded = false;
    bool isTouchingLeftWall = false;
    bool isTouchingRightWall = false;

    // to-do
    bool toJump = false;

    // grace time
    [Header("Grace Time")]
    [SerializeField] float jumpGraceTime = .1f;
    float jumpGraceTimer = 0;

    // animation state
    enum AnimationState { idle, run, rise, fall, wallSlide }
    AnimationState animState = AnimationState.idle;

    // actual move
    bool isActMovingH = false; // is actively moving horizontally
    bool isActMovingL = false; // is actively moving left
    bool isActMovingR = false; // is actively moving right
    bool isWallSliding = false;

    void Start()
    {
        // get reference
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateInput();
        UpdateCollideState();
        UpdateToDo();
        UpdateAnimation();
    }

    void UpdateInput()
    {
        hAxisInput = Input.GetAxisRaw("Horizontal");

        // jump button down input buffer
        if (Input.GetButtonDown("Jump"))
            jumpButtonDownBufferTimer = jumpButtonDownBufferTime;
        else if (jumpButtonDownBufferTimer > 0)
            jumpButtonDownBufferTimer -= Time.deltaTime;
    }

    void UpdateCollideState()
    {
        isGrounded = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, 1 << LayerMask.NameToLayer("Ground"));
        isTouchingLeftWall = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, 1 << LayerMask.NameToLayer("Ground"));
        isTouchingRightWall = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, 1 << LayerMask.NameToLayer("Ground"));
    }

    void UpdateToDo()
    {
        if (!toJump)
        {
            // jump grace
            if (isGrounded)
                jumpGraceTimer = jumpGraceTime;
            else if (jumpGraceTimer > 0)
                jumpGraceTimer -= Time.deltaTime;

            if (jumpButtonDownBufferTimer > 0 && jumpGraceTimer > 0)
            {
                toJump = true;
                jumpButtonDownBufferTimer = 0;
                jumpGraceTimer = 0;
            }
        }
    }

    void UpdateAnimation()
    {
        if (isGrounded)
        {
            if (isActMovingH) // & grounded => running
                animState = AnimationState.run;
            else // grounded & not actively moving horizontally => idling
                animState = AnimationState.idle;
        }
        else if (isWallSliding) // => wall sliding
        {
            animState = AnimationState.wallSlide;
        }
        else // not grounded & not wall sliding => in the air
        {
            if (rb.velocity.y > .1f) // in the air & moving upwards => rising
                animState = AnimationState.rise;
            else if (rb.velocity.y < -.1f) // in the air & moving downwards => falling
                animState = AnimationState.fall;
        }

        anim.SetInteger("state", (int)animState);

        if (isActMovingL)
            sprite.flipX = true;
        else if (isActMovingR)
            sprite.flipX = false;
    }

    void FixedUpdate()
    {
        // move horizontally
        if (hAxisInput != 0)
        {
            rb.velocity = new Vector2(hAxisInput * runSpeed, rb.velocity.y);
            isActMovingH = true;
            isActMovingL = hAxisInput < 0;
            isActMovingR = hAxisInput > 0;
        }
        else
        {
            rb.velocity = new Vector2(hAxisInput * runSpeed, rb.velocity.y);
            isActMovingH = isActMovingL = isActMovingR = false;
        }

        // wall slide
        if (!isGrounded && ((isTouchingLeftWall && hAxisInput < 0) || (isTouchingRightWall && hAxisInput > 0)) && rb.velocity.y < -.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        // jump
        if (toJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            toJump = false;
        }
    }
}
