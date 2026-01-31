using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManagerPersistente : MonoBehaviour
{
    public static FadeManagerPersistente Instance; 
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Awake()
    {
       
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    private void Start()
    {
        
        StartCoroutine(FadeIn());
    }

    
    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            float alpha = t / fadeDuration;
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0);
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(1f);
        
        SceneManager.LoadScene(sceneName);
        yield return null;
        StartCoroutine(FadeIn());
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, alpha);
    }
}
