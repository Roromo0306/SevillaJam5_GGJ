using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // <-- para cambiar escenas
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float killRange = 1f;
    public LayerMask npcLayer;
    public AudioSource audioSource;
    public AudioClip Killclip;

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
    public TextMeshProUGUI dialogText; // para mostrar "ese no era"
    public float dialogDuration = 2f;

    [Header("Animator")]
    public Animator animator;

    private void Start()
    {
        if (cooldownCircle != null) cooldownCircle.gameObject.SetActive(false);
        if (cooldownText != null) cooldownText.gameObject.SetActive(false);
        if (dialogText != null) dialogText.gameObject.SetActive(false);
    }

    void Update()
    {
        Move();
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
        transform.Translate(dir * speed * Time.deltaTime);

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
        else if(h == 0)
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

    // ---------- NUEVO: desenmascarar NPC ----------
    void TryUnmask()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, unmaskRange);
        foreach (Collider2D hit in hits)
        {
            NPCController npc = hit.GetComponent<NPCController>();
            if (npc != null && npc.currentState == NPCState.Dead && !npc.isUnmasked)
            {
                npc.isUnmasked = true; // marcamos que ya se desenmascaró

                if (npc.isBoss)
                {
                    FadeManagerPersistente.Instance.LoadSceneWithFade("YouWin");
                }
                else
                {
                    StartCoroutine(ShowTemporaryDialog("ese no era", dialogDuration));
                }

                break; // solo desenmascarar un NPC a la vez
            }
        }
    }

    IEnumerator ShowTemporaryDialog(string message, float duration)
    {
        if (dialogText == null) yield break;

        dialogText.text = "";
        dialogText.gameObject.SetActive(true);

        // Efecto de escritura
        float letterDelay = 0.05f; // 50ms entre letras, puedes ajustar
        foreach (char letter in message)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }

        // Esperar un tiempo después de escribir todo el texto
        yield return new WaitForSeconds(duration);

        dialogText.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, unmaskRange);
    }
}
