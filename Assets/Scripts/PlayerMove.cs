using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed = 3.0f;
    public float jumpPower;
    Rigidbody2D rigid;
    CapsuleCollider2D capsuleCollider;
    SpriteRenderer spriteRenderer;
    Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
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
        if(Input.GetButton("Horizontal"))
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
        // Attack
        if(collision.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y && collision.gameObject.name.Contains("Enemy"))
            {
                OnAttack(collision.transform);
            }
            else // Damaged
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            //Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
            {
                gameManager.stagePoint += 50;
            }
            else if(isSilver)
            {
                gameManager.stagePoint += 100;
            }
            else if(isGold)
            {
                gameManager.stagePoint += 300;
            }

            //Deactive Item
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "Finish")
        {
            //Next Stage
            gameManager.NextStage();
        }
    }

    void OnAttack(Transform enemy)
    {
        // Point
        gameManager.stagePoint += 100;
        // Reaction Force
        rigid.AddForce(Vector2.up * 10.0f, ForceMode2D.Impulse);
        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        gameManager.HealthDown();
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

    public void OnDie()
    {
        //Sprite Alpha;
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        capsuleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5.0f, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
