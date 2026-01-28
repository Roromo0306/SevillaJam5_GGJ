using UnityEngine;

public class Door : MonoBehaviour
{
    public Collider2D col;
    public Animator anim;
    bool open;

    public void Toggle()
    {
        open = !open;
        col.enabled = !open;
        anim.SetBool("Open", open);
    }
}
