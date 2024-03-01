using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    enum MovementState { idle, run, jump, fall, wallSlide }

    SpriteRenderer sprite;
    Rigidbody2D rb;
    BoxCollider2D coll;
    Animator anim;

    [SerializeField] float runSpeed = 600f;
    [SerializeField] float jumpSpeed = 1200f;
    [SerializeField] float wallSlideSpeed = 200f;

    float hAxisInput = 0;
    bool jumpButtonPressed = false;

    bool isGrounded = false;
    bool isTouchingWall = false;
    bool isTouchingLeftWall = false;
    bool isTouchingRightWall = false;
    MovementState movementState = MovementState.idle;

    bool toJump = false;

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
        UpdateMovementState();
        UpdateAnimationState();
    }

    void UpdateInput()
    {
        hAxisInput = Input.GetAxisRaw("Horizontal");
        jumpButtonPressed = Input.GetButtonDown("Jump");
    }

    void UpdateCollideState()
    {
        isGrounded = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, 1 << LayerMask.NameToLayer("Ground"));
        isTouchingLeftWall = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, 1 << LayerMask.NameToLayer("Ground"));
        isTouchingRightWall = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, 1 << LayerMask.NameToLayer("Ground"));
        isTouchingWall = isTouchingLeftWall || isTouchingRightWall;
    }

    void UpdateToDo()
    {
        if (jumpButtonPressed && isGrounded)
        {
            toJump = true;
        }
    }

    void UpdateMovementState()
    {
        // judge by input and collide state
        if (isGrounded)
        {
            if (hAxisInput == 0)
            {
                movementState = MovementState.idle;
            }
            else
            {
                movementState = MovementState.run;
            }
        }
        else if (rb.velocity.y < -.1f && ((isTouchingLeftWall && hAxisInput < 0) || (isTouchingRightWall && hAxisInput > 0)))
        {
            movementState = MovementState.wallSlide;
        }
        else
        {
            if (rb.velocity.y > .1f)
            {
                movementState = MovementState.jump;
            }
            else if (rb.velocity.y < -.1f)
            {
                movementState = MovementState.fall;
            }
        }

        // judge by todos
        if (toJump)
        {
            movementState = MovementState.jump;
        }
    }

    void UpdateAnimationState()
    {
        anim.SetInteger("movement state", (int)movementState);

        if (hAxisInput > 0)
        {
            sprite.flipX = false;
        }
        else if (hAxisInput < 0)
        {
            sprite.flipX = true;
        }
    }

    void FixedUpdate()
    {
        // set velocity

        rb.velocity = new Vector2(hAxisInput * runSpeed * Time.deltaTime, rb.velocity.y);

        if (toJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * Time.deltaTime);
            toJump = false;
        }

        if (movementState == MovementState.wallSlide)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed * Time.deltaTime, float.MaxValue));
        }
    }
}
