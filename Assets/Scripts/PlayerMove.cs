using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed = 3.0f;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()   // 1초에 60회
    {
        //Jump
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJump"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJump", true);
        }

        //Stop Speed
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
        //Direction Sprite
        if(Input.GetButtonDown("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        if(Mathf.Abs(rigid.velocity.x) < 0.5)
        {
            anim.SetBool("isWalk", false);
        }
        else
        {
            anim.SetBool("isWalk", true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()  // 1초에 50회
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if(rigid.velocity.x > maxSpeed) // Right
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if(rigid.velocity.x < (-1)*maxSpeed) // Left
        {
            {
                rigid.velocity = new Vector2((-1)*maxSpeed, rigid.velocity.y);
            }
        }

        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1.0f, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    anim.SetBool("isJump", false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if(collision.gameObject.tag == "Enemy")
        {
            OnDamaged(collision.transform.position);
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        // Change Layer (Immortal Active)
        gameObject.layer = 9;

        // View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*7.0f, ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("DoDamaged")
;
        Invoke("OffDamaged", 2.0f);
    }

    void OffDamaged()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);
        gameObject.layer = 8;
    }
}
