using UnityEngine;

public class NPCVision : MonoBehaviour
{
    public float radius = 5f;
    public float angle = 90f;
    public LayerMask bodyMask;
    public LayerMask obstacleMask;

    NPCController npc;

    void Start()
    {
        npc = GetComponent<NPCController>();
    }

    void Update()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, bodyMask);

        foreach (var h in hits)
        {
            Vector2 dir = (h.transform.position - transform.position).normalized;
            if (Vector2.Angle(transform.right, dir) < angle / 2)
            {
                float dist = Vector2.Distance(transform.position, h.transform.position);
                if (!Physics2D.Raycast(transform.position, dir, dist, obstacleMask))
                {
                    npc.SeeBody();
                }
            }
        }
    }
}
