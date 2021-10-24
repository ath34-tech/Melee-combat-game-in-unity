using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D body;
    private float speed = 7F;
    private Animator anim;
    private bool grounded;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public float attackRange;
    public float maxHealth;
    public float currentHealth;
    public float cooldown;
    public float timeStamp;


    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        grounded = true;
        currentHealth = maxHealth;
        timeStamp = Time.time + cooldown;

    }

    private void Update()
    {
        float ClickInput = Input.GetAxis("Horizontal");

        body.velocity = new Vector2(ClickInput * speed, body.velocity.y);
        if (ClickInput > 0.01f)
            transform.localScale = new Vector3(3, 3, 1);
        else if (ClickInput < -0.01f)
            transform.localScale = new Vector3(-3, 3, 1);
        if (Input.GetKey(KeyCode.Space) && grounded)
            Jump();

        anim.SetBool("run", ClickInput != 0);

       
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Attack("attack1");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Attack("attack2");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Attack("attack3");
        }
            
    }

    void Die()
    {
        anim.SetBool("Dead", true);
        GetComponent<Collider2D>().enabled = false;
        Destroy(body);
        this.enabled = false;
    }
    public void TakeDamage(float damage)
    {
        if (currentHealth != 0)
        {
            currentHealth -= damage;
            anim.SetTrigger("take_damage");

        }
        else
        {
            Die();
        }
    }


    void Attack(string triggerName)
    {
        if (timeStamp <= Time.time)
        {
            anim.SetTrigger(triggerName);
            Collider2D[] Enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
            foreach (Collider2D enemy in Enemies)
            {
                enemy.GetComponent<Enemy>().TakeDamageEnemy(10F);
            }
            timeStamp = Time.time + cooldown;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, speed + 5);
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "grounded")
            grounded = true;
    }
}
