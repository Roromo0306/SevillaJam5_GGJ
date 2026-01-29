using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float killRange = 1f;
    public LayerMask npcLayer;
    public AudioSource audioSource; 
    public AudioClip Killclip;

    [Header("VFX")]
    public VFX vfxController;
    void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.E))
            TryKill();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(h, v).normalized;
        transform.Translate(dir * speed * Time.deltaTime);
    }

    void TryKill()
    {
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
            }
        }
    }
}
