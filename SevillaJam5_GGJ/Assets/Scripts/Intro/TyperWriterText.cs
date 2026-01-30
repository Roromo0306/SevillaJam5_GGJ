using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterText : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public string[] lines;
    public float typingSpeed = 0.05f;

    public AudioSource audioSource;
    public AudioClip lineSound;

    private int index;
    private bool isTyping;

    void Start()
    {
        textUI.text = "";
        StartCoroutine(TypeLine());
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
        audioSource.PlayOneShot(lineSound);

        textUI.text = "";

        string line = lines[index];
        bool insideTag = false;

        foreach (char c in line)
        {
            if (c == '<')
                insideTag = true;

            textUI.text += c;

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
        if (index < lines.Length - 1)
        {
            index++;
            textUI.text = "";
            StartCoroutine(TypeLine());
        }
    }
}
