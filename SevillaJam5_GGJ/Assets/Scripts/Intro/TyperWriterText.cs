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

        foreach (char c in lines[index].ToCharArray())
        {
            textUI.text += c;
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
