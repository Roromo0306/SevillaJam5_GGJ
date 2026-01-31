using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogController : MonoBehaviour
{
    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    public float lettersPerSecond = 20f;
    public System.Action onDialogClose;
    public static DialogController Instance { get; private set; }

    private Dialog currentDialog;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private NPCController currentNPC;

    private void Awake()
    {
        Instance = this;
        dialogBox.SetActive(false);
    }

    private void Update()
    {
        if (!dialogBox.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            HandleInput();
        }
    }

    public void ShowDialog(Dialog dialog, NPCController npc)
    {
        currentDialog = dialog;
        currentLineIndex = 0;
        currentNPC = npc; 
        dialogBox.SetActive(true);
        StartLine(); ;
    }

    void StartLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeDialog(currentDialog.Lines[currentLineIndex]));
    }

    IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char letter in line)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        isTyping = false;
    }

    void HandleInput()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogText.text = currentDialog.Lines[currentLineIndex];
            isTyping = false;
        }
        else
        {
            currentLineIndex++;

            if (currentLineIndex < currentDialog.Lines.Count)
            {
                StartLine();
            }
            else
            {
                CloseDialog();
            }
        }
    }

    void CloseDialog()
    {
        dialogBox.SetActive(false);
        dialogText.text = "";
        currentDialog = null;

        
        onDialogClose?.Invoke();
        onDialogClose = null;
    }
}
