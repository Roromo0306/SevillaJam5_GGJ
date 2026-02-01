using UnityEngine;

public class NPCVision : MonoBehaviour
{
    public float radius = 5f;
    public float angle = 90f;

    public LayerMask bodyMask;
    public LayerMask obstacleMask;

    public float heightOffset = 0f; // ← Nuevo: altura del visor

    private NPCController npc;

    void Start()
    {
        npc = GetComponent<NPCController>();
        if (npc == null) Debug.LogError("NPCController no encontrado en NPCVision");
    }

    void Update()
    {
        // Aplicamos el offset de altura
        Vector2 origin = new Vector2(transform.position.x, transform.position.y + heightOffset);

        // Detectar todos los colliders dentro del radio y layer correcta
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, bodyMask);

        foreach (var h in hits)
        {
            Vector2 dir = ((Vector2)h.transform.position - origin).normalized;
            float angleToTarget = Vector2.Angle(npc.MoveDirection, dir);
            if (angleToTarget < angle / 2f)
            {
                float dist = Vector2.Distance(origin, h.transform.position);

                if (!Physics2D.Raycast(origin, dir, dist, obstacleMask))
                {
                    Debug.Log($"NPC '{npc.name}' ve al objeto '{h.name}' a distancia {dist} y ángulo {angleToTarget}");
                    HandleVision(h);
                }
                else
                {
                    Debug.Log($"NPC '{npc.name}' VE al objeto '{h.name}' pero hay un obstáculo");
                }
            }
            else
            {
                Debug.Log($"NPC '{npc.name}' no ve al objeto '{h.name}' porque está fuera del cono (ángulo {angleToTarget})");
            }
        }
    }

    void HandleVision(Collider2D h)
    {
        if (h.CompareTag("DeadBody"))
        {
            Debug.Log($"NPC '{npc.name}' ALERTADO por el cuerpo '{h.name}'");
            npc.SeeBody();
        }
    }

    // Gizmos para ver cono
    void OnDrawGizmosSelected()
    {
        if (npc == null)
            npc = GetComponent<NPCController>();

        Gizmos.color = Color.yellow;

        Vector2 forward = npc.MoveDirection;
        if (forward == Vector2.zero) forward = Vector2.right;

        float halfAngle = angle / 2f;

        // Aplicamos offset de altura al gizmo
        Vector3 gizmoPos = transform.position + new Vector3(0, heightOffset, 0);

        Vector2 leftDir = Quaternion.Euler(0, 0, halfAngle) * forward;
        Vector2 rightDir = Quaternion.Euler(0, 0, -halfAngle) * forward;

        Gizmos.DrawLine(gizmoPos, gizmoPos + (Vector3)(leftDir * radius));
        Gizmos.DrawLine(gizmoPos, gizmoPos + (Vector3)(rightDir * radius));

        int steps = 20;
        Vector3 prevPoint = gizmoPos + (Vector3)(leftDir * radius);
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            float stepAngle = Mathf.Lerp(halfAngle, -halfAngle, t);
            Vector2 dir = Quaternion.Euler(0, 0, stepAngle) * forward;
            Vector3 nextPoint = gizmoPos + (Vector3)(dir * radius);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
