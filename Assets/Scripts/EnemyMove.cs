using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove: MonoBehaviour
{
    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    public int nextMove;


    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        Invoke("Think", 5.0f);
    } 

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platfrom Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1.0f, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2);

        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);
        //Flip Sprite
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }

        //Recursive
        float nextThinkTime = Random.Range(2.0f, 5.0f);

        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 2.0f);
    }

    public void OnDamaged()
    {
        //Sprite Alpha;
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        boxCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5.0f, ForceMode2D.Impulse);
        //Destroy
        Invoke("DeActive", 5.0f);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
