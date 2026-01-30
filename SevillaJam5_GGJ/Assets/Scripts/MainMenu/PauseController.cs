using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject panelPausa;

    // Start is called before the first frame update
    void Start()
    {
        panelPausa.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AbrirMenuPausa()
    {
        Time.timeScale = 0;
        panelPausa.SetActive(true);

    }

    public void Reanudar()
    {
        Time.timeScale = 1;
        panelPausa.SetActive(false);

    }

    public void Reiniciar()
    {
        SceneManager.LoadScene("Game");
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
        
    }
}
