using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    /*private GameManager gameManager;
    private GameObject manager;*/
    private GameObject snake;

    [SerializeField]
    private float constantVelocity;
    private Quaternion newRotation;
    [SerializeField]
    private float width = 8f;
    [SerializeField]
    private float spinVelocity = 4f;
    [SerializeField]
    private float alpha = 0.66f;
    private float newAlpha;
    private float timer;
    [SerializeField]
    private float deathDelay;
    [SerializeField]
    private int enemyState; // -1 --> dead  / 0 --> spawning / 1 --> active

    // Set up references
    void Awake()
    {
        /*manager = GameObject.FindGameObjectWithTag("Manager");
        gameManager = manager.GetComponent<GameManager>();*/
        snake = GameObject.FindGameObjectWithTag("Player");
    }

    // Use this for initialization
    void Start ()
    {
        timer = 0f;
        enemyState = 0;
        this.gameObject.tag = "Inactive";
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (enemyState != 1)
        {
            timer += Time.smoothDeltaTime;
            if (timer >= deathDelay)
            {
                timer = 0f;
                if (enemyState == 0)
                {
                    this.gameObject.tag = "Enemy";
                    enemyState = 1;
                }
                else { Object.Destroy(this.gameObject); }
            }
            else
            {
                if (enemyState == -1)
                {
                    newAlpha = alpha * (deathDelay - timer) / deathDelay;
                    if (timer >= deathDelay * 0.5f)
                    {
                        this.gameObject.tag = "Inactive";
                    }
                }
                else if (enemyState == 0)
                {
                    newAlpha = alpha * timer / deathDelay;
                }
            }
        }
        else
        {
            timer += spinVelocity * Time.smoothDeltaTime;
            while (timer >= 360f) { timer -= 360f; }
            newRotation = Quaternion.Euler(0f, 0f, width * Mathf.Sin(timer));
        }
    }

    private void FixedUpdate ()
    {
        if (enemyState == 1)
        {
            this.GetComponent<Rigidbody2D>().velocity = Vector3.Normalize(snake.transform.position - this.transform.position) * constantVelocity;
        }
    }

    // LateUpdate is called once before rendering
    private void LateUpdate()
    {
        if (enemyState != 1)
        {
            Color color = this.GetComponentInChildren<SpriteRenderer>().color;
            color.a = newAlpha;
            this.GetComponentInChildren<SpriteRenderer>().color = color;
        }
        else
        {
            this.transform.rotation = newRotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy" || col.tag == "Inactive")
        {
            enemyState = -1;
            timer = 0f;
            print(this.name+"vanishes!");
        }
        /*else if (col.tag == "Body")
        {
            gameManager.GameOver();
            print("GAME OVER.");
        }*/
    }
}
