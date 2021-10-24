using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    #region Public variables
    public bool inRange;
    public GameObject target;
    public float attackRange;
    public float distance;
    public float moveSpeed;
    public bool facingRight;
    public Transform hitArea;
    public LayerMask PlayerLayer;
    public float cooldown;
    public float timeStamp;
    public float maxHealthEnemy;
    public float currentHealthEnemy;
    #endregion
    #region Private variables
    private Animator anim;
    private Rigidbody2D bodyEnemy;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        inRange = false;
        facingRight = false;
        anim = GetComponent<Animator>();
        timeStamp = Time.time + cooldown;
        currentHealthEnemy = maxHealthEnemy;
        bodyEnemy = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame  
    void Update()
    {
        if (target != null)
        {
            if (inRange)
            {
                EnemyAIBrain();
            }
            else
            {
                anim.SetBool("walk", false);
            }
        }
        else
        {
            anim.SetBool("walk", false);

        }
    }

    void EnemyAIBrain()
    {

        distance = Vector2.Distance(target.transform.position, transform.position);
        if (target.transform.position.x < transform.position.x && facingRight)
            Flip();
        if (target.transform.position.x > transform.position.x && !facingRight)
            Flip();
        if (distance > attackRange)
        {
            MoveToward();
        }
        else if (attackRange > distance)
        {
            if (timeStamp <= Time.time)
            {
                Attack();
            }
            else
            {
                anim.SetBool("walk", false);
            }
        }
        if (target != null)
        {
            if (transform.position.x == target.transform.position.x)
            {
                if (timeStamp <= Time.time)
                {
                    Attack();
                }
                else
                {
                    anim.SetBool("walk", false);
                }
            }
        }
    }

    void Flip()
    {
        //here your flip funktion, as example
        facingRight = !facingRight;
        Vector3 tmpScale = transform.localScale;
        tmpScale.x *= -1;
        transform.localScale = tmpScale;
    }

    void Attack()
    {
        anim.SetBool("walk", false);
        anim.SetTrigger("enemyattack");
        Collider2D[] Players = Physics2D.OverlapCircleAll(hitArea.position, attackRange, PlayerLayer);
        foreach (Collider2D playeriterator in Players)
        {
            playeriterator.GetComponent<Player>().TakeDamage(10F);
        }
        timeStamp = Time.time + cooldown;

    }
    void OnDrawGizmosSelected()
    {
        if (hitArea == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(hitArea.position, attackRange);
    }
    void MoveToward()
    {
        anim.SetBool("walk", true);
        Vector2 targetPosition = new Vector2(target.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "player")
        {
            target = trig.gameObject;
            inRange = true;
        }

    }

    void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "player")
        {
            target = null;
            inRange = false;
        }
    }



    void EnemyDie()
    {
        anim.SetBool("DeadEnemy", true);
        GetComponent<Collider2D>().enabled = false;
        Destroy(bodyEnemy);
        this.enabled = false;
    }
    public void TakeDamageEnemy(float damage)
    {
        if (currentHealthEnemy != 0)
        {
            currentHealthEnemy -= damage;
            anim.SetTrigger("take_damage_enemy");

        }
        else
        {
            EnemyDie();
        }
    }
}
