using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    enum MovementState { idle, run, jump, fall }

    SpriteRenderer sprite;
    Rigidbody2D rb;
    BoxCollider2D coll;
    Animator anim;

    [SerializeField] float runSpeed = 600f;
    [SerializeField] float jumpSpeed = 1200f;
    [SerializeField] float slideSpeed = 200f;

    float horizontal = 0;
    bool jump = false;

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
        UpdateAnimationState();
    }

    void UpdateInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jump = true;
        }
    }

    void UpdateAnimationState()
    {

        MovementState state = MovementState.idle;

        if (horizontal > 0)
        {
            state = MovementState.run;
            sprite.flipX = false;
        }
        else if (horizontal < 0)
        {
            state = MovementState.run;
            sprite.flipX = true;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jump;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.fall;
        }

        anim.SetInteger("state", (int)state);
    }

    void FixedUpdate()
    {
        // move player according to input by setting velocity

        rb.velocity = new Vector2(horizontal * runSpeed * Time.deltaTime, rb.velocity.y);
        
        if (jump)
        {
            jump = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * Time.deltaTime);
        }

        if (IsOnWall())
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -slideSpeed * Time.deltaTime, float.MaxValue));
        }
    }

    bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, 1 << LayerMask.NameToLayer("Ground"));
    }

    bool IsOnWall()
    {
        return IsOnLeftWall() || IsOnRightWall();
    }

    bool IsOnLeftWall()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.left, .1f, 1 << LayerMask.NameToLayer("Ground")) && !IsGrounded();
    }

    bool IsOnRightWall()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.right, .1f, 1 << LayerMask.NameToLayer("Ground")) && !IsGrounded();
    }
}
