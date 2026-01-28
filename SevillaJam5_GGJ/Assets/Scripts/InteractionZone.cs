using UnityEngine;

public enum InteractionType { Dance, Drink, Talk }

public class InteractionZone : MonoBehaviour
{
    public InteractionType type;

    private void OnTriggerEnter2D(Collider2D col)
    {
        NPCAnimator anim = col.GetComponent<NPCAnimator>();
        if (anim)
            anim.PlayInteraction(type);
    }
}
