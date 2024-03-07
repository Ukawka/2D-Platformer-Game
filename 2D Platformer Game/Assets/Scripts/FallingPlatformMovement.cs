using UnityEngine;

public class FallingPlatformMovement : MonoBehaviour
{
    [SerializeField] private bool shaking = true;
    [SerializeField] private float shakeVelocity = 0.3f;
    [SerializeField] private float range = 0.2f;
    [SerializeField] private float fallDelay = 0.5f;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private float shakeUbound, shakeLbound, destroyDelay = 2f;
    private bool falling = false, stomped = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        shakeUbound = transform.position.y + range;
        shakeLbound = transform.position.y - range;
        if (shaking)
        {
            rb.velocity = new Vector2(0, shakeVelocity);
        }
    }

    void Shake()
    {
        if (transform.position.y > shakeUbound)
        {
            rb.velocity = new Vector2(0, -shakeVelocity);
        }
        if (transform.position.y < shakeLbound)
        {
            rb.velocity = new Vector2(0, shakeVelocity);
        }
    }

    bool IsStomped()
    {
        if (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, .1f, 1 << LayerMask.NameToLayer("Player")))
        {
            stomped = true;
        }
        return stomped;
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }

    void StartFalling()
    {
        falling=true;
        coll.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        Invoke(nameof(SelfDestroy),destroyDelay);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsStomped())
        {
            Invoke(nameof(StartFalling),fallDelay);
        }
    }

    void FixedUpdate()
    {
        if(shaking && !falling)
        {
            Shake();
        }
        else if(!shaking)
        {
            rb.velocity = Vector2.zero;
        }
    }
}
