using UnityEngine;

public class FallingPlatformMovement : MonoBehaviour
{
    [SerializeField] private bool shaking=true;
    [SerializeField] private float shake_vol=0.3f;
    [SerializeField] private float range=0.2f;
    private Rigidbody2D rb;
    private float shake_ubound,shake_lbound;

    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        shake_ubound=transform.position.y+range;
        shake_lbound=transform.position.y-range;
        if(shaking)
        {
            rb.velocity=new Vector2(0,shake_vol);
        }
    }

    void Shake()
    {
        if(transform.position.y>shake_ubound)
        {
            rb.velocity=new Vector2(0,-shake_vol);
        }
        if(transform.position.y<shake_lbound)
        {
            rb.velocity=new Vector2(0,shake_vol);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(shaking)
        {
            Shake();
        }
        else
        {
            rb.velocity=new Vector2(0,0);
        }
    }
}
