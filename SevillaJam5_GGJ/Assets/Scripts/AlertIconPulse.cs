using UnityEngine;

public class AlertIconPulse : MonoBehaviour
{
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.15f;

    Vector3 baseScale;

    void OnEnable()
    {
        baseScale = Vector3.one;
        transform.localScale = baseScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * scale;
    }
}
