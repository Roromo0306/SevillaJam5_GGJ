using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public Image cooldownCircle; // Imagen tipo "Filled" para el círculo
    public TextMeshProUGUI cooldownText;

    [Header("VFX")]
    public VFX vfxController;

    private void Start()
    {
        if (cooldownCircle != null) cooldownCircle.gameObject.SetActive(false);
        if (cooldownText != null) cooldownText.gameObject.SetActive(false);
    }
    void Update()
    {
        Move();
        HandleCooldown();

        if (Input.GetKeyDown(KeyCode.E))
            TryKill();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(h, v).normalized;
        transform.Translate(dir * speed * Time.deltaTime);

        
        if (h > 0.01f) 
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x); 
            transform.localScale = scale;
        }
        else if (h < -0.01f) 
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x); 
            transform.localScale = scale;
        }
    }

    void TryKill()
    {
        if (!canKill) return; // todavía en cooldown

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

                // Iniciar cooldown
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

            // Activar UI durante cooldown
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

                // Ocultar UI cuando termina el cooldown
                if (cooldownCircle != null) cooldownCircle.gameObject.SetActive(false);
                if (cooldownText != null) cooldownText.gameObject.SetActive(false);
            }
        }
    }

}
