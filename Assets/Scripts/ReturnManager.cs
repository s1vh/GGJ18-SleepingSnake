using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // Allows to manage scences

public class ReturnManager : MonoBehaviour {

    public Object returnScene;    // Reference to the Game Over scene
    private List<string> scenesInBuild = new List<string>();
    private AsyncOperation m_AsyncLoaderCoroutine;

    // Set up references
    void Awake()
    {
        BuildSceneList();
    }

    // Use this for initialization
    void Start ()
    {
        //BuildSceneList();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown("return"))
        {
            SceneManager.LoadSceneAsync(returnScene.name);
            //StartCoroutine(LoadSceneAsync(returnScene.name));
            print("Reseting...");
        }
    }

    // Build the level list, should be called once!
    void BuildSceneList()
    {
        scenesInBuild = new List<string>();

        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            int lastSlash = scenePath.LastIndexOf("/");
            scenesInBuild.Add(scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1));
            print("Added: " + scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1));
        }
    }

    // Loading coroutine
    IEnumerator LoadSceneAsync(string scene)
    {
        m_AsyncLoaderCoroutine = SceneManager.LoadSceneAsync(scene);
        m_AsyncLoaderCoroutine.allowSceneActivation = true;
        yield return m_AsyncLoaderCoroutine;
    }
}
