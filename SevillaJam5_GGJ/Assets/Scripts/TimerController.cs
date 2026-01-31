using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [Header("Configuración del Timer")]
    public float tiempoEnSegundos = 60f; // Tiempo inicial en segundos
    public TextMeshProUGUI timerText;     // TextMeshPro para mostrar el timer

    [Header("Efectos al Terminar")]
    public AudioSource audioSource; // Un solo AudioSource
    public AudioClip clip1;         // Primer sonido
    public AudioClip clip2;         // Segundo sonido
    [Range(0f, 1f)] public float volumen = 1f; // Volumen de ambos clips

    public Image pantalla;          // Panel de policía (sirena)
    [Range(0f, 1f)] public float alpha = 0.5f; // Opacidad de la imagen durante la alerta
    public float duracionAlerta = 5f;      // Duración del parpadeo
    public float velocidadParpadeo = 0.2f; // Tiempo entre cambios de color

    private bool timerActivo = true;

    void Start()
    {
        // Validaciones
        if (timerText == null)
            Debug.LogError("Asignar un TextMeshProUGUI para mostrar el timer.");
        if (pantalla == null)
            Debug.LogError("Asignar una Image UI que cubra la pantalla.");
        if (audioSource == null)
            Debug.LogError("Asignar un AudioSource.");

        // Desactivar panel al inicio
        if (pantalla != null)
            pantalla.gameObject.SetActive(false);

        StartCoroutine(Contador());
    }

    IEnumerator Contador()
    {
        float tiempoRestante = tiempoEnSegundos;

        while (tiempoRestante > 0 && timerActivo)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);

            if (timerText != null)
                timerText.text = string.Format("{0:00}:{1:00}", minutos, segundos);

            yield return new WaitForSeconds(1f);
            tiempoRestante -= 1f;
        }

        if (timerText != null)
            timerText.text = "00:00";

        timerActivo = false;
        StartCoroutine(AlertaFinal());
    }

    public IEnumerator AlertaFinal()
    {
        // Activar panel al empezar la alerta
        if (pantalla != null)
            pantalla.gameObject.SetActive(true);

        // Reproducir ambos clips simultáneamente
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
