using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public NPCState currentState;

    [Header("Movement")]
    public Transform[] wanderPoints;
    public Transform alertPoint;

    [Header("Alert")]
    public float alertTime = 3f;
    private float alertTimer;

    private NavMeshAgent agent;
    private NPCAnimator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<NPCAnimator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        ChangeState(NPCState.Walking);
    }

    void Update()
    {
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
        Transform t = wanderPoints[Random.Range(0, wanderPoints.Length)];
        agent.SetDestination(t.position);
        anim.SetMoving(true);
    }

    void HandleAlert()
    {
        agent.SetDestination(alertPoint.position);

        if (Vector2.Distance(transform.position, alertPoint.position) < 0.3f)
        {
            alertTimer += Time.deltaTime;
            if (alertTimer >= alertTime)
                GameManager.Instance.GameOver();
        }
    }

    public void SeeBody()
    {
        if (currentState == NPCState.Dead) return;
        ChangeState(NPCState.Alerted);
    }

    public void Die()
    {
        currentState = NPCState.Dead;
        agent.enabled = false;
        anim.PlayDead();
        gameObject.tag = "DeadBody";
    }

    void ChangeState(NPCState newState)
    {
        currentState = newState;
        anim.UpdateState(newState);
    }
}
