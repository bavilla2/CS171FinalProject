﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    [SerializeField] public float speed = 7;
    private Rigidbody2D m_rigidbody;
    [SerializeField] public float jumpForce = 0f;//= 7.5f;
    [SerializeField] public float health = 50.00f;
    private bool isGrounded = false;
    private bool alive = true;
    private Collider2D m_collider;
    [SerializeField] public bool isThrown = false;
    public Animator animator; 
    private RaycastHit2D jumpInfo;
    public static Player instance;
    private float delay = 2f;
    private float nextUse = 0f;
    public string level;
    public float ballSpeed;

    public GameObject snowball;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = this.GetComponent<Rigidbody2D>(); 
        m_collider = this.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 feetPosition = new Vector2(this.transform.position.x, m_collider.bounds.min.y);
        jumpInfo = Physics2D.Raycast(feetPosition, Vector2.down, 1f);
        Debug.DrawRay(feetPosition, Vector2.down * 1f, Color.green);

        life();
        move();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpForce += 15f;
            Jump();
            jumpForce -= 2.0f;
        }

        if(!isGrounded)
        {
            if(jumpForce >= 0.01f)
            {
                jumpForce -= 2.0f;
            }
            else {
                jumpForce = 0.0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && Time.time > nextUse)
        {
            StartCoroutine(Throw_Snowball_Animation());
            nextUse = Time.time + delay;
        }

    }

    IEnumerator Throw_Snowball_Animation()
    {
        animator.SetBool("Is_thrown", true);
        yield return new WaitForSeconds(0.4f);
        throw_snowball();
        animator.SetBool("Is_thrown", false);
    }

    void move()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void Jump()
    {
        m_rigidbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (jumpInfo && jumpInfo.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        else if (collision.collider.CompareTag("Lava"))
        {
            StartCoroutine(IsDead());
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            if (jumpInfo && jumpInfo.collider.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Snowman_Jr"))
        {
            speed = 0f;
            SceneManager.LoadScene(level);
        }
        else if(collider.CompareTag("Cactus"))
        {
            health -= 5;
        }
        else if (collider.CompareTag("Fire"))
        {
            health -= 15;
            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("Flamethrower"))
        {
            health -= 25;
            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("Grill"))
        {
            health -= 10;
            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("IceCube"))
        {
            health += 5;
            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("AC"))
        {
            health += 30;
            Destroy(collider.gameObject);
        }
        else if (collider.CompareTag("Popsicle"))
        {
            health += 10;
            Destroy(collider.gameObject);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void life()
    {
        health -= GetDeltaTime();

        if (health < 0)
        {
            StartCoroutine(IsDead());
        }
    }

    IEnumerator IsDead()
    {
        alive = false;
        speed = 0f;
        animator.SetBool("Is_dead", true);
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene("GameOver");
    }

    private static float GetDeltaTime()
    {
        return Time.deltaTime;
    }

    void restart_game()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void throw_snowball()
    {
        Vector3 oneUnitRightOfMe = this.transform.position + Vector3.right;
        GameObject Snowball = Instantiate(snowball, oneUnitRightOfMe, Quaternion.identity);
        Snowball.GetComponent<Rigidbody2D>().AddForce(new Vector2(ballSpeed, 0f), ForceMode2D.Impulse);
        health -= 5;
        Destroy(Snowball, 2f);
    }

}
