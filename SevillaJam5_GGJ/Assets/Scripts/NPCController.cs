using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    [Header("Stats")]
    public NPCState currentState;
    public float alertTime = 3f;

    [Header("Movement")]
    public Transform[] wanderPoints;
    public Transform alertPoint;

    [Header("Wander Settings")]
    public float wanderRadius = 5f;
    public float minWaitTime = 1.5f;
    public float maxWaitTime = 4f;

    [Header("Movement Settings")]
    public MovementType movementType = MovementType.Free;
    private int currentWaypointIndex = 0;

    [Header("Animator")]
    public Animator animator;

    [Header("Interaction")]
    public float interactRadius = 1.5f;
    private bool playerInRange = false;

    [Header("Dead")]
    public Sprite deadSprite;

    [Header("Alert Icon")]
    public GameObject alertIcon;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip screamClip;

    [Header("Boss Settings")]
    public bool isBoss = false;


    [Header("Efectos al Terminar")]
    public AudioClip clip1;        
    public AudioClip clip2;         
    [Range(0f, 1f)] public float volumen = 1f; 

    [Header("Dialog")]
    public Dialog dialog;

    public Image pantalla;          
    [Range(0f, 1f)] public float alpha = 0.5f; 
    public float duracionAlerta = 5f;     
    public float velocidadParpadeo = 0.2f; 

    private NavMeshAgent agent;
    private SpriteRenderer sr;
    private float alertTimer = 0f;
    private bool hasSeenBody = false;
    private bool isWaiting = false;
    private bool dialogActive = false;
    public TimerController timerController;
    private bool alertaFinalIniciada = false;
    [HideInInspector]
    public bool isUnmasked = false;

    
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
        if (currentState == NPCState.Dead) return; 
        if (currentState == NPCState.Interacting)return;

        // Actualizar dirección de movimiento
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            MoveDirection = agent.velocity.normalized;

            if (sr != null)
            {
                if (MoveDirection.x > 0.1f)
                {
                    animator.SetBool("isWalking", true);
                    sr.flipX = false;
                }  
                else if (MoveDirection.x < -0.1f)
                {
                    animator.SetBool("isWalking", true);
                    sr.flipX = true;
                }
                else if(MoveDirection.x == 0)
                {
                    animator.SetBool("isWalking", false);
                }
                    
            }
        }

        switch (currentState)
        {
            case NPCState.Walking:
                if (!agent.hasPath && !isWaiting)
                    StartCoroutine(WanderRoutine());
                break;

            case NPCState.Alerted:
                HandleAlert();
                break;
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
    }

    // Movimiento libre + pausas
    IEnumerator WanderRoutine()
    {
        isWaiting = true;

        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        if (currentState == NPCState.Dead || agent == null || !agent.enabled)
        {
            isWaiting = false;
            yield break;
        }

        MoveRandom();

        isWaiting = false;
    }

    void MoveRandom()
    {
        if (currentState == NPCState.Dead || agent == null || !agent.enabled) return;

        switch (movementType)
        {
            case MovementType.Free:
                if (wanderPoints.Length == 0 || Random.value < 0.7f)
                {
                    Vector3 target = RandomNavMeshLocation(wanderRadius);
                    agent.SetDestination(target);
                }
                else
                {
                    Transform target = wanderPoints[Random.Range(0, wanderPoints.Length)];
                    agent.SetDestination(target.position);
                }
                break;

            case MovementType.Waypoints:
                if (wanderPoints.Length == 0) return;
                agent.SetDestination(wanderPoints[currentWaypointIndex].position);
                currentWaypointIndex = (currentWaypointIndex + 1) % wanderPoints.Length;
                break;
        }
    }

    // Obtener punto aleatorio en el NavMesh
    Vector3 RandomNavMeshLocation(float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 randomDirection = new Vector3(randomCircle.x, randomCircle.y, 0) + transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);
        return hit.position;
    }

    void HandleAlert()
    {
        if (agent == null || alertPoint == null) return; // <-- evita crash

        if (!agent.hasPath)
        {
            agent.SetDestination(alertPoint.position);
        }

        float distance = Vector2.Distance(transform.position, alertPoint.position);

        if (distance < 0.3f)
        {
            alertTimer += Time.deltaTime;

            if (alertTimer >= alertTime && !alertaFinalIniciada)
            {
                alertaFinalIniciada = true;
                StartCoroutine(AlertaFinal());
            }
        }
    }

    public void SeeBody()
    {
        if (currentState == NPCState.Dead) return;
        if (hasSeenBody) return;

        hasSeenBody = true;

        
        ChangeState(NPCState.Alerted);

        
        if (alertIcon)
            alertIcon.SetActive(true);

        // Detener movimiento actual
        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.isStopped = true; 
        }

        
        StartCoroutine(ReactToBody());
    }

    IEnumerator ReactToBody()
    {
      
        if (audioSource != null && screamClip != null)
        {
            audioSource.pitch = Random.Range(0.3f, 1f); // Pitch aleatorio
            audioSource.PlayOneShot(screamClip);
        }

        // Esperar 3 segundos
        yield return new WaitForSeconds(3f);

        // Continuar hacia el punto de alerta
        if (agent != null && agent.enabled && alertPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(alertPoint.position);
        }
    }

    public void Die()
    {
        currentState = NPCState.Dead;

        if (agent != null) agent.enabled = false;

        if (sr != null && deadSprite != null)
            sr.sprite = deadSprite;

        gameObject.tag = "DeadBody";
        gameObject.layer = LayerMask.NameToLayer("DeadBody");
    }

    public void ChangeState(NPCState newState)
    {
        currentState = newState;
        alertTimer = 0f;

        if (newState != NPCState.Alerted && alertIcon)
            alertIcon.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    public void Interact()
    {
        if (currentState == NPCState.Interacting) return;

        ChangeState(NPCState.Interacting);

        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

      
        DialogController.Instance.onDialogClose = FinishInteraction;

        DialogController.Instance.ShowDialog(dialog,this);
    }

    public void FinishInteraction()
    {
        if (currentState == NPCState.Interacting)
        {
            ChangeState(NPCState.Walking);

            if (agent != null && agent.enabled)
            {
                agent.isStopped = false;
            }
        }
    }

    public IEnumerator AlertaFinal()
    {
       
        if (pantalla != null)
            pantalla.gameObject.SetActive(true);

        
        if (audioSource != null)
        {
            if (clip1 != null) audioSource.PlayOneShot(clip1, volumen);
            if (clip2 != null) audioSource.PlayOneShot(clip2, volumen);
        }

        float tiempoTranscurrido = 0f;
        bool colorRojo = true;

        while (tiempoTranscurrido < duracionAlerta)
        {
            if (pantalla != null)
            {
                Color colorActual = colorRojo ? Color.red : Color.blue;
                colorActual.a = alpha; // Ajusta la opacidad
                pantalla.color = colorActual;
            }

            colorRojo = !colorRojo;

            yield return new WaitForSeconds(velocidadParpadeo);
            tiempoTranscurrido += velocidadParpadeo;
        }

        // Dejar la pantalla transparente o desactivada al final
        if (pantalla != null)
            pantalla.gameObject.SetActive(false);

        // Llamar a GameOver seguro
        GameOver();
    }

    void GameOver()
    {
        if (FadeManagerPersistente.Instance != null)
        {
            FadeManagerPersistente.Instance.LoadSceneWithFade("MainMenu");
        }
        else
        {
            Debug.LogWarning("FadeManagerPersistente.Instance no está inicializado. Cargando MainMenu sin fade.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        Debug.Log("GAME OVER");
    }
}
