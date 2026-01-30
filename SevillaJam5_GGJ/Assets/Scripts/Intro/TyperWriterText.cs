using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TypewriterText : MonoBehaviour
{
    [Header("UI y Texto")]
    public TextMeshProUGUI textUI;
    public string[] lines;
    public float typingSpeed = 0.05f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip lineSound;

    [Header("Escena siguiente")]
    public string nextSceneName; // Nombre de la escena a cargar al terminar

    private int index;
    private bool isTyping;

    void Start()
    {
        textUI.text = "";
        if (lines.Length > 0)
            StartCoroutine(TypeLine());
        else
            LoadNextScene();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                textUI.text = lines[index];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        textUI.text = "";
        string line = lines[index];
        bool insideTag = false;

        foreach (char c in line)
        {
            if (c == '<')
                insideTag = true;

            textUI.text += c;

            if (!insideTag && lineSound != null)
            {
                // Reproducir el sonido por letra sin superponer
                if (!audioSource.isPlaying)
                    audioSource.PlayOneShot(lineSound);
            }

            if (c == '>')
                insideTag = false;

            // Solo esperamos si NO estamos dentro de una etiqueta
            if (!insideTag)
                yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void NextLine()
    {
        index++;

        if (index < lines.Length)
        {
            textUI.text = "";
            StartCoroutine(TypeLine());
        }
        else
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            FadeManagerPersistente.Instance.LoadSceneWithFade(nextSceneName);
    }
}
