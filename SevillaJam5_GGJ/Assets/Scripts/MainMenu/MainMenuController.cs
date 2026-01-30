using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject pressAnyKey;
    public GameObject mainMenu;
    public GameObject creditsPanel;
    public GameObject tutorialPanel;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(false);
        pressAnyKey.SetActive(true);
        creditsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            pressAnyKey.SetActive(false);
            mainMenu.SetActive(true);
        }   
    }

    public void Jugar()
    {
        FadeManagerPersistente.Instance.LoadSceneWithFade("Intro");
    }

    public void CreditosAbrir()
    {
        mainMenu.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void TutorialAbrir()
    {
        mainMenu.SetActive(false);
        tutorialPanel.SetActive(true);
    }

    public void Cerrar()
    {
        mainMenu.SetActive(true);
        creditsPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
