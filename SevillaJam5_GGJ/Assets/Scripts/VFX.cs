using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VFX : MonoBehaviour
{
    [Header("Configuración del Viñeteado")]
    [Range(0, 1)] public float intensidadMaxVineteo = 0.5f;
    [Range(0, 1)] public float smoothnessMaxVineteo = 0.5f;
    [Min(0)] public float duracionAumento = 0.1f;
    [Min(0)] public float duracionDisminucion = 2f;

    private Vignette vignette;

    void Start()
    {
        BuscarVignette();
    }

    // Método público para activar el efecto desde otro script
    public void ActivarEfecto()
    {
        if (vignette != null)
        {
            StopAllCoroutines(); // Para evitar que varias corrutinas se acumulen
            StartCoroutine(VignetteLerp());
        }
    }

    private IEnumerator VignetteLerp()
    {
        float cronometro = 0f;

        // Aumentar intensidad
        while (cronometro / duracionAumento < 1f)
        {
            cronometro += Time.deltaTime;
            float t = Mathf.Clamp01(cronometro / duracionAumento);
            vignette.intensity.value = Mathf.Lerp(0, intensidadMaxVineteo, t);
            vignette.smoothness.value = Mathf.Lerp(0, smoothnessMaxVineteo, t);
            yield return null;
        }

        // Disminuir intensidad
        cronometro = 0f;
        while (cronometro < 1f)
        {
            cronometro += Time.deltaTime / duracionDisminucion;
            float t = Mathf.Clamp01(cronometro);
            vignette.intensity.value = Mathf.Lerp(intensidadMaxVineteo, 0, t);
            vignette.smoothness.value = Mathf.Lerp(smoothnessMaxVineteo, 0, t);
            yield return null;
        }
    }

    void BuscarVignette()
    {
        GameObject globalVolume = GameObject.Find("Global Volume");
        if (globalVolume != null)
        {
            Volume volume = globalVolume.GetComponent<Volume>();
            if (volume != null)
            {
                if (!volume.profile.TryGet(out vignette))
                {
                    Debug.LogError("No se encontró Vignette en el Volume Profile");
                }
                else
                {
                    vignette.intensity.overrideState = true;
                    vignette.smoothness.overrideState = true;
                }
            }
            else Debug.LogError("No se encontró la componente Volume");
        }
        else Debug.LogError("No se encuentra el objeto Global Volume");
    }
}
