using UnityEngine;

public class NPCAnimator : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetMoving(bool v)
    {
        anim.SetBool("IsMoving", v);
    }

    public void UpdateState(NPCState s)
    {
        anim.SetInteger("State", (int)s);
    }

    public void PlayDead()
    {
        anim.SetTrigger("Die");
    }

    public void PlayInteraction(InteractionType t)
    {
        anim.SetTrigger(t.ToString());
    }
}
