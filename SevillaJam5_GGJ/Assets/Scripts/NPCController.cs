using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    [Header("Stats")]
    public NPCState currentState;
    public float alertTime = 3f;

    [Header("Movement")]
    public Transform[] wanderPoints;
    public Transform alertPoint;

    [Header("Dead")]
    public Sprite deadSprite;

    private NavMeshAgent agent;
    private SpriteRenderer sr;
    private float alertTimer = 0f;
    private bool hasSeenBody = false;

    // Dirección de movimiento para visión y sprite
    public Vector2 MoveDirection { get; private set; } = Vector2.right;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        sr = GetComponent<SpriteRenderer>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        ChangeState(NPCState.Walking);
    }

    void Update()
    {
        // Actualizar dirección de movimiento
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            MoveDirection = agent.velocity.normalized;

            // Girar sprite hacia la dirección
            float angle = Mathf.Atan2(MoveDirection.y, MoveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        switch (currentState)
        {
            case NPCState.Walking:
                if (!agent.hasPath)
                    MoveRandom();
                break;

            case NPCState.Alerted:
                HandleAlert();
                break;
        }
    }

    void MoveRandom()
    {
        if (wanderPoints.Length == 0) return;

        Transform target = wanderPoints[Random.Range(0, wanderPoints.Length)];
        agent.SetDestination(target.position);
    }

    void HandleAlert()
    {
        if (!agent.hasPath)
        {
            agent.SetDestination(alertPoint.position);
        }

        float distance = Vector2.Distance(transform.position, alertPoint.position);

        if (distance < 0.3f)
        {
            alertTimer += Time.deltaTime;

            if (alertTimer >= alertTime)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    public void SeeBody()
    {
        if (currentState == NPCState.Dead) return;
        if (hasSeenBody) return;

        hasSeenBody = true;

        ChangeState(NPCState.Alerted);

        if (agent.enabled)
        {
            agent.ResetPath();
            agent.SetDestination(alertPoint.position);
        }
    }

    public void Die()
    {
        currentState = NPCState.Dead;

        if (agent != null) agent.enabled = false;

        // Cambiar sprite
        if (sr != null && deadSprite != null)
            sr.sprite = deadSprite;

        // Cambiar tag para que otros NPCs lo vean
        gameObject.tag = "DeadBody";

        gameObject.layer = LayerMask.NameToLayer("DeadBody");
    }

    void ChangeState(NPCState newState)
    {
        currentState = newState;
        alertTimer = 0f;
    }
}