using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum Scenes { None, HeartsTable, Menu}

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;
    
    [SerializeField] private GameObject sceneSwitchCover;
    [SerializeField] private AnimationCurve sceneFade;

    public delegate void SceneLoadedCallback(Scenes scene);
    public event SceneLoadedCallback OnSceneLoaded;

    public IEnumerator FadeScreen;
    private float fadeTimer = 0;
    private float speed = 1;


    private void Awake()
    {

    #region  Singleton
        if (instance != null)
        {
            Debug.LogWarning("GameSceneManager already exists");
            Destroy(gameObject);
            return;
        }
        instance = this;
    #endregion


    }
    public void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }
    
    void OnEnable()
    {
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    public void LoadScene(Scenes to)
    {
        SceneManager.LoadScene(to.ToString());
    }

     public void LoadScene(string to)
    {
        Debug.Log("LoadeSceneActivated");
        sceneSwitchCover.SetActive(true);
        StartCoroutine(PrepareSceneSwitch(to));
    }

    /// <summary>
    /// This gets called automatically whenever unity loads a new scene.
    /// In this instance, it gets called after the screen fades to black
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (OnSceneLoaded != null)
            OnSceneLoaded(GetCurrentScene());

        StartCoroutine(EndSceneSwitch());

    }

    /// <summary>
    /// Called when quitting to menu
    /// </summary>
    /// <param name="DestinationScene"></param>
    public void QuitToMenu(Scenes DestinationScene)
    {

    }


    private IEnumerator EndSceneSwitch()
    {
        fadeTimer = 1;
        while (fadeTimer > 0)
        {
            fadeTimer -= speed * Time.deltaTime; 
            var tempColor = sceneSwitchCover.GetComponent<Image>().color;
            tempColor.a = sceneFade.Evaluate(fadeTimer);
            sceneSwitchCover.GetComponent<Image>().color = tempColor;
            yield return null;

        }
        sceneSwitchCover.SetActive(false);
    }

    private IEnumerator PrepareSceneSwitch(string to)
    {
        fadeTimer = 0;
        while (fadeTimer < 1)
        {
            fadeTimer += speed * Time.deltaTime;
            var tempColor = sceneSwitchCover.GetComponent<Image>().color;
            tempColor.a = sceneFade.Evaluate(fadeTimer);
            sceneSwitchCover.GetComponent<Image>().color = tempColor;
            yield return null;
        
        }
        SceneManager.LoadScene(to);
    }

    /// <summary>
    /// Returns the enum equivelant of the current scene
    /// </summary>
    /// <returns></returns>
    public Scenes GetCurrentScene()
    {
        return ((Scenes)System.Enum.Parse(typeof(Scenes), SceneManager.GetActiveScene().name));
    }


}