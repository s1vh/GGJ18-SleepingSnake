using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // Allows to manage scences

public class GameManager : MonoBehaviour {

    public Object gameOverScene;    // Reference to the Game Over scene
    public GameObject Enemy;
    public GameObject Token;

    [SerializeField]
    private float EnemyTime;
    private float EnemyTimer = 0f;
    [SerializeField]
    private int EnemyCount;
    private GameObject[] enemies;
    [SerializeField]
    private float TokenTime;
    private float TokenTimer = 0f;
    [SerializeField]
    private int TokenCount;
    private GameObject[] tokens;
    [SerializeField]
    private float xRange;
    [SerializeField]
    private float yRange;
    [SerializeField]
    private float radius;
    private Vector2 coordinates;

    private GameObject bgmObj;
    private AudioSource bgm;
    //private List<string> scenesInBuild = new List<string>();
    //private AsyncOperation m_AsyncLoaderCoroutine;

    // Set up references
    void Awake()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        tokens = GameObject.FindGameObjectsWithTag("Target");
        
        // Audio
        bgmObj = GameObject.Find("AudioTheme");
        bgm = bgmObj.GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start ()
    {
        //BuildSceneList();
        bgm.Play();
        bgm.loop = true;
    }

    // Update is called once per frame
    void Update ()
    {
        // Enemies
        if (enemies.Length >= EnemyCount)
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }
        if (enemies.Length < EnemyCount)
        {
            EnemyTimer += Time.smoothDeltaTime;
            while (EnemyTimer >= EnemyTime)
            {
                EnemyTimer -= EnemyTime;
                if (enemies.Length < EnemyCount) { enemies = Spawn(Enemy, "Enemy"); }
                else { break; }
            }
        }

        // Tokens
        if (tokens.Length >= TokenCount)
        {
            tokens = GameObject.FindGameObjectsWithTag("Target");
        }
        if (tokens.Length < TokenCount)
        {
            TokenTimer += Time.smoothDeltaTime;
            while (TokenTimer >= TokenTime)
            {
                TokenTimer -= TokenTime;
                if (tokens.Length < TokenCount) { tokens = Spawn(Token, "Target"); }
                else { break; }
            }
        }
    }

    private GameObject[] Spawn(GameObject go, string tag)
    {
        bool lookingForCoordinates = true;
        while (lookingForCoordinates)
        {
            coordinates = new Vector2(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange));
            Collider2D[] colliders = Physics2D.OverlapCircleAll(coordinates, radius);
            if (colliders.Length == 0) { lookingForCoordinates = false; }
            else
            {
                foreach (Collider2D col in colliders)
                {
                    if (col.tag == "Body" || col.tag == "Free" || col.tag == "Enemy" || col.tag == "Target" || col.tag == "Inactive")
                    {
                        break;
                    }
                    lookingForCoordinates = false;
                }
            }
        }
        GameObject newObject = Instantiate(go, new Vector3(coordinates.x, coordinates.y, this.transform.position.z), Quaternion.identity) as GameObject;
        return GameObject.FindGameObjectsWithTag(tag);
    }

    // Build the level list, should be called once!
    /*void BuildSceneList()
    {
        scenesInBuild = new List<string>();

        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            int lastSlash = scenePath.LastIndexOf("/");
            scenesInBuild.Add(scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1));
            print("Added: " + scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1));
        }
    }*/

    // Loading coroutine
    /*IEnumerator LoadSceneAsync(string scene)
    {
        m_AsyncLoaderCoroutine = SceneManager.LoadSceneAsync(scene);
        m_AsyncLoaderCoroutine.allowSceneActivation = true;
        yield return m_AsyncLoaderCoroutine;
    }*/

    public void GameOver()
    {
        SceneManager.LoadSceneAsync(gameOverScene.name);
    }
}
