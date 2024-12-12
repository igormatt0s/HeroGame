using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public Transform waypointA;
    public Transform waypointB;
    public Transform pcTransform;
    public float vel = 2f;
    private Animator animator;
    private bool isWalking;
    public int attack = 10;
    public int npcHealth = 50;
    public float attackInterval = 1f;

    private Transform alvoAtual;
    private Rigidbody2D rbd;
    private Vector3 escala;
    private Coroutine attackCoroutine;

    public float fadeDuration = 1f;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rbd = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        alvoAtual = waypointA;
        escala = transform.localScale;
        Debug.Log("Life do NPC: " + npcHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (pcTransform.position.x > waypointA.position.x && pcTransform.position.x < waypointB.position.x)
        {
            alvoAtual = pcTransform;
        }
        else
        {
            if (alvoAtual == pcTransform)
            {
                alvoAtual = waypointA;
            }
        }
        MoveTowardsTarget();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ZoneAttack"))
        {
            Debug.Log("Inimigo entrou na zona de ataque!");
        }

        PCController pc = collision.GetComponent<PCController>();

        if(pc == null)
        {
            pc = collision.GetComponentInParent<PCController>();
        }

        if(pc != null)
        {
            if(attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackPC(pc));
            }
            else
            {
                Debug.LogWarning("PC Controller não encontrado no objeto com a tag ZoneAttack!");
            }
        }

        if (collision.CompareTag("AttackZone"))
        {
            Debug.Log("Inimigo está sendo atacado.");
            NPCTakeDamage(10);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ZoneAttack"))
        {
            Debug.Log("Inimigo saiu da zona de ataque!");

            if(attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private IEnumerator AttackPC(PCController pc)
    {
        pc.TakeDamage(attack);
        animator.SetTrigger("Attack");
        Debug.Log("NPC atacando...");
	yield return new WaitForSeconds(attackInterval);
    }

    private void MoveTowardsTarget()
    {
        Vector3 alvoHorizontal = new Vector2(alvoAtual.position.x, transform.position.y);
        Vector2 direcao = (alvoHorizontal - transform.position).normalized;

        transform.position += (Vector3)direcao * vel * Time.deltaTime;

        if (alvoAtual == pcTransform)
        {
            if (pcTransform.position.x < transform.position.x)
            {
                transform.localScale = escala;
            }
            else
            {
                Flip();
            }
        }
        else
        {
            if (Vector2.Distance(alvoHorizontal, transform.position) <= 0.2f)
            {
                SwitchTarget();
            }
        }

        UpdateAnimation();
    }

    private void SwitchTarget()
    {
        if (alvoAtual == waypointA)
        {
            alvoAtual = waypointB;
            Flip();
        } else
        {
            alvoAtual = waypointA;
            transform.localScale = escala;
        }
    }

    private void UpdateAnimation()
    {
        isWalking = (Vector2.Distance(transform.position, alvoAtual.position) > 0.1f);
        animator.SetBool("isWalking", isWalking);
    }

    private void Flip()
    {
        Vector3 escalaInversa = escala;
        escalaInversa.x *= -1;
        transform.localScale = escalaInversa;
    }

    public void NPCTakeDamage(int damage)
    {
        npcHealth -= damage;
        animator.SetBool("inDamage", true);
        Debug.Log("Inimigo tomou " + damage + " de dano. Life restante: " + npcHealth);

        StartCoroutine(ResetDamageAnimation());

        if (npcHealth <= 0)
        {
            Debug.Log("O Inimigo morreu!");
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private IEnumerator ResetDamageAnimation()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("inDamage", false);
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float startAlpha = spriteRenderer.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while(progress < 1.0f)
        {
            Color tmpColor = spriteRenderer.color;
            spriteRenderer.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(startAlpha, 0, progress));
            progress += rate * Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
