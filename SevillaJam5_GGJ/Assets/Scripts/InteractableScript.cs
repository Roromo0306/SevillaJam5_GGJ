using UnityEngine;
using UnityEngine.UI;

public class InteractableImage : MonoBehaviour
{
    [Header("Imagen que mostrará este punto")]
    public Sprite imageToShow;

    [Header("UI")]
    public GameObject panelUI;
    public Image panelImage;

    private bool playerInside;

    private void Awake()
    {
        panelUI.SetActive(false);    
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.F))
        {
            TogglePanel();
        }
    }

    void TogglePanel()
    {
        bool isActive = panelUI.activeSelf;
        panelUI.SetActive(!isActive);

        if (!isActive)
        {
            panelImage.sprite = imageToShow;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Player entered trigger");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            panelUI.SetActive(false);
            Debug.Log("Player exited trigger");
        }
    }
    public void ClosePanel()
    {
        panelUI.SetActive(false);
    }
}
