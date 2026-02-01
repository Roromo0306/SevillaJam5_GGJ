using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float killRange = 1f;
    public LayerMask npcLayer;
    public AudioSource audioSource;
    public AudioClip Killclip;
    public AudioClip desenmascararClip;

    [Header("Kill Cooldown")]
    public float killCooldown = 3f;
    private float killTimer = 0f;
    private bool canKill = true;

    [Header("UI Elements")]
    public Image cooldownCircle;
    public TextMeshProUGUI cooldownText;

    [Header("VFX")]
    public VFX vfxController;

    [Header("Desenmascarar")]
    public float unmaskRange = 1.5f;
    public TextMeshProUGUI dialogText; 
    public float dialogDuration = 2f;

    [Header("Animator")]
    public Animator animator;
    private Rigidbody2D rb;

    
    private void Start()
    {
        if (cooldownCircle != null) cooldownCircle.gameObject.SetActive(false);
        if (cooldownText != null) cooldownText.gameObject.SetActive(false);
        if (dialogText != null) dialogText.gameObject.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        Move();
    }
    void Update()
    {
        
        HandleCooldown();

        if (Input.GetKeyDown(KeyCode.E))
            TryKill();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Estoy desenmascarando");
            TryUnmask();
        }
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(h, v).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);

        if (h > 0.01f)
        {
            animator.SetBool("isWalking", true);
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (h < -0.01f)
        {
            animator.SetBool("isWalking", true);
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    void TryKill()
    {
        if (!canKill) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, killRange, npcLayer);
        if (hit)
        {
            NPCController npc = hit.GetComponent<NPCController>();
            if (npc)
            {
                npc.Die();
                audioSource.PlayOneShot(Killclip);
                CameraShake.Instance.Shake(0.12f, 0.08f);
                if (vfxController != null)
                    vfxController.ActivarEfecto();

                

                canKill = false;
                killTimer = killCooldown;
            }
        }
    }

    void HandleCooldown()
    {
        if (!canKill)
        {
            killTimer -= Time.deltaTime;
            if (killTimer < 0f)
                killTimer = 0f;

            if (cooldownCircle != null)
            {
                cooldownCircle.gameObject.SetActive(true);
                cooldownCircle.fillAmount = 1 - (killTimer / killCooldown);
            }
            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = Mathf.Ceil(killTimer).ToString();
            }

            if (killTimer == 0f)
            {
                canKill = true;
                if (cooldownCircle != null) cooldownCircle.gameObject.SetActive(false);
                if (cooldownText != null) cooldownText.gameObject.SetActive(false);
            }
        }
    }

   
    void TryUnmask()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, unmaskRange);

        foreach (Collider2D hit in hits)
        {
            NPCController npc = hit.GetComponent<NPCController>();

            if (npc != null && npc.currentState == NPCState.Dead && !npc.isUnmasked)
            {
                Debug.Log("NPC desenmascarado: " + npc.name);

                audioSource.PlayOneShot(desenmascararClip);

                npc.isUnmasked = true;

                if (npc.isBoss)
                {
                    FadeManagerPersistente.Instance.LoadSceneWithFade("YouWin");
                }
                else
                {
                    StartCoroutine(ShowTemporaryDialog("Ese no es el objetivo...", dialogDuration));
                }

                return; 

            }
        }

        Debug.Log("No hay NPC muerto para desenmascarar");
    }

    IEnumerator ShowTemporaryDialog(string message, float duration)
    {
        Debug.Log("MOSTRANDO TEXTO: " + message);
        if (dialogText == null) yield break;

        dialogText.text = "";
        dialogText.gameObject.SetActive(true);

        float letterDelay = 0.05f;

        foreach (char letter in message)
        {
            dialogText.text += letter;
            yield return new WaitForSecondsRealtime(letterDelay);
        }

        yield return new WaitForSecondsRealtime(duration);

        dialogText.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, unmaskRange);
    }
}
