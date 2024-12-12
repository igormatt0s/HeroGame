using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PCController : MonoBehaviour
{
    public float vel = 10f;
    public float salto = 15f;
    private Rigidbody2D rbd;
    private float entrada;
    private Animator animator;
    private bool isRunning = false;
    private bool isJumping = false;

    public int pcHealth = 100;

    public Slider lifeSlider;
    // Start is called before the first frame update
    void Start()
    {
        rbd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("isRunning", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("inDamage", false);
        Debug.Log("Life do PC: " + pcHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Attack");
        }

        entrada = Input.GetAxisRaw("Horizontal");

        // animação para correr/ficar parado
        if (entrada != 0)
        {
            isRunning = true;
            animator.SetBool("isJumping", false);
        } else
        {
            isRunning = false;
        }
        animator.SetBool("isRunning", isRunning);

        if (Input.GetKeyDown(KeyCode.UpArrow) && !isJumping)
        {
            Jump();
        }

        if (transform.position.y <= -25f)
        {
            Debug.Log("Game Over!");
            SceneManager.LoadScene(2);
        }

        lifeSlider.value = pcHealth * 0.01f;

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GoldenTreasure"))
        {
            pcHealth += 70;
            Debug.Log("Golden Treasure coletado!");
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("SilverTreasure"))
        {
            pcHealth += 50;
            Debug.Log("Silver Treasure coletado!");
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("BronzeTreasure"))
        {
            pcHealth += 30;
            Debug.Log("Bronze Treasure coletado!");
            Destroy(collision.gameObject);
        }

        // Limita a saúde máxima para evitar que exceda o valor definido 100
        if (pcHealth > 100)
        {
            pcHealth = 100;
        }
        Debug.Log("Saúde aumentada para: " + pcHealth);
    }


    void FixedUpdate()
    {
        rbd.velocity = new Vector2(entrada * vel, rbd.velocity.y);

        // Movimento do PC
        if(entrada > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else if (entrada < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if(Mathf.Abs(rbd.velocity.y) > 0.02f)
        {
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }
        animator.SetBool("isJumping", isJumping);
    }

    void Jump()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        if(hit.collider != null)
        {
            rbd.velocity = new Vector2(rbd.velocity.x, salto);
            animator.SetBool("isJumping", true);
        }
    }

    public void TakeDamage(int damage)
    {
        pcHealth -= damage;
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("inDamage", true);
            StartCoroutine(ResetDamageAnimation());
        }
        Debug.Log("PC tomou " + damage + " de dano. Life restante: " + pcHealth);

        if(pcHealth <= 0)
        {
            Debug.Log("Game Over!");
            SceneManager.LoadScene(2);
        }
    }

    private IEnumerator ResetDamageAnimation()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("inDamage", false);
    }
}
