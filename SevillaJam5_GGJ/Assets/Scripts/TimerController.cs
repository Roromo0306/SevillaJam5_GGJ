using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [Header("Configuración del Timer")]
    public float tiempoEnSegundos = 60f; 
    public TextMeshProUGUI timerText;     

    [Header("Efectos al Terminar")]
    public AudioSource audioSource; 
    public AudioClip clip1;         
    public AudioClip clip2;         
    [Range(0f, 1f)] public float volumen = 1f; 

    public Image pantalla;         
    [Range(0f, 1f)] public float alpha = 0.5f; 
    public float duracionAlerta = 5f;     
    public float velocidadParpadeo = 0.2f; 

    private bool timerActivo = true;

    void Start()
    {
        
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
                colorActual.a = alpha; 
                pantalla.color = colorActual;
            }

            colorRojo = !colorRojo;

            yield return new WaitForSeconds(velocidadParpadeo);
            tiempoTranscurrido += velocidadParpadeo;
        }

        
        if (pantalla != null)
            pantalla.gameObject.SetActive(false);

        
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
