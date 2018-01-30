using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // Allows to manage scences

public class SnakeHead : MonoBehaviour {

    public static SnakeHead instance = null;

    public GameObject body;
    public Material snakeMaterial;
    public float sleepiness;
    public float currentVelocity;

    private GameManager gameManager;
    private GameObject manager;

    [SerializeField]
    private int overcharge = 10;
    [SerializeField]
    private int neckCount = 1;
    [SerializeField]
    private float sleepGain = 1f;
    [SerializeField]
    private float caffeineGain = 20f;
    [SerializeField]
    private float fullVelocity = 8f;
    [SerializeField]
    private float angleWidth = 4f;
    [SerializeField]
    private float tickTime = 2f;
    [SerializeField]
    private float resizeFactor = 1f;

    private float tickTimer;
    private GameObject thisModule;
    private GameObject lastModule;
    private List<GameObject> snake = new List<GameObject>();
    //private LineRenderer lineRenderer;
    //private List<string> scenesInBuild = new List<string>();
    //private AsyncOperation m_AsyncLoaderCoroutine;

    private GameObject bgmObj;
    private AudioSource bgm;

    // Awake is always called before any Start functions
    void Awake ()
    {
        // Check if instance already exists and instances it if it doesn't
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        // Set up references
        thisModule = this.gameObject;
        lastModule = thisModule;
        manager = GameObject.FindGameObjectWithTag("Manager");
        gameManager = manager.GetComponent<GameManager>();
        //lineRenderer = GetComponent<LineRenderer>();
    }

    // Use this for initialization
    void Start ()
    {
        sleepiness = 50f;
        tickTimer = 0;
        //if (neckCount > overcharge) { neckCount = overcharge; }
        snake.Add(this.gameObject);
        while (overcharge > 0)
        {
            if (neckCount > 0)
            {
                Grow(false);
                neckCount--;
            }
            else
            {
                Grow(true);
            }
            overcharge--;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Resize the snake
        ResizeSnake(snake, resizeFactor);

        // Handle timers
        tickTimer += Time.smoothDeltaTime;
        if (tickTimer >= tickTime)
        {
            while (tickTimer >= tickTime)   // normalize
            {
                tickTimer -= tickTime;
            }

            if (sleepiness < 100) { sleepiness += sleepGain; }
            else { sleepiness = 100; }
        }

        // Read input
        float inputX = Input.GetAxis("Horizontal");
        this.transform.Rotate(new Vector3(0f, 0f, inputX * angleWidth * -1f));

        // Calculate and apply current velocity
        currentVelocity = (100 - sleepiness) * 0.01f * fullVelocity;
        this.GetComponent<Rigidbody2D>().velocity = this.transform.up * currentVelocity;
    }

    private void LateUpdate ()
    {
        // Draw a line between the snake modules
        /*lineRenderer.sortingOrder = 1;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = Color.green;
        lineRenderer.startWidth = 0.4f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.positionCount = snake.Count;
        int i = 0;
        while (i < snake.Count)
        {
            lineRenderer.SetPosition(i, snake[i].gameObject.GetComponent<SnakeModule>().GetParsePosition());
            i++;
        }*/
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Target")
        {
            Object.Destroy(col.gameObject);
            sleepiness -= caffeineGain;
            if (sleepiness < 0) { sleepiness = 0; }
            Grow(true);
            print("Snake is caffeined and growing!");
        }
        else if (col.tag == "Enemy" || col.tag == "Body")
        {
            gameManager.GameOver();
            print("GAME OVER.");
        }
    }

    private void Grow(bool active)
    {
        GameObject newModule = Instantiate(body, lastModule.transform.position, Quaternion.identity) as GameObject;
        newModule.GetComponent<SnakeModule>().SetPrevModule(lastModule);
        lastModule.GetComponent<SnakeModule>().SetLastStatus(false);
        if (!active) { newModule.GetComponent<SnakeModule>().TurnOff(); }
        newModule.GetComponent<SnakeModule>().SetLastStatus(true);
        lastModule = newModule;
        snake.Add(lastModule);
    }

    private void ResizeSnake(List<GameObject> list, float factor)
    {
        int i = 0;
        while (i < list.Count - 1)
        {
            i++;
            list[list.Count - i].GetComponent<SnakeModule>().ResizeModule(Mathf.Log10(factor * i));
        }
    }
}
