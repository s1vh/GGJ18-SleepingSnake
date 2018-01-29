using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeModule : MonoBehaviour {

    private SnakeHead snakeHead;
    private SnakeModule prevSnakeModule;
    private GameObject head;
    private GameObject prevModule;
    private LineRenderer lineRenderer;
    private Vector3 lastPosition;
    private float tolerance;
    [SerializeField]
    private Color color;
    [SerializeField]
    float margin = 0.1f;
    [SerializeField]
    private bool tpSwitch;

    // Set up references
    void Awake ()
    {
        head = GameObject.Find("SnakeHead");
        snakeHead = head.GetComponent<SnakeHead>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Use this for initialization
    void Start ()
    {
        if (prevModule == null) { SetPrevModule(head); tpSwitch = true; }
    }

    // Update is called once per frame
    void Update()
    {
        /* Problematic cases
        // Borders:
        if (this.transform.position.y > 5.1f) { this.transform.position = new Vector3(this.transform.position.x, 5.0f, this.transform.position.z); ready = false; }
        else if (this.transform.position.y < -5.1f) { this.transform.position = new Vector3(this.transform.position.x, -5.0f, this.transform.position.z); ready = false; }
        else if (this.transform.position.x > 9.0f) { this.transform.position = new Vector3(8.9f, this.transform.position.y, this.transform.position.z); ready = false; }
        else if (this.transform.position.x < -9.0f) { this.transform.position = new Vector3(-8.9f, this.transform.position.y, this.transform.position.z); ready = false; }
        */

        if (tpSwitch == prevSnakeModule.AskForTp())
        {
            Debug.DrawLine(this.transform.position, prevModule.transform.position, Color.green);
        }
        else
        {
            Debug.DrawLine(this.transform.position, prevSnakeModule.GetLastPosition(), Color.green);
        }
    }

    void FixedUpdate ()
    {
        if (this.gameObject != head)
        {
            if (AskForContinuity())
            {
                if (Vector3.Distance(this.transform.position, prevModule.transform.position) > tolerance)
                {
                    this.GetComponent<Rigidbody2D>().velocity = snakeHead.currentVelocity * Vector3.Distance(this.transform.position, prevModule.transform.position)/tolerance * Vector3.Normalize(prevModule.transform.position - this.transform.position);
                }
            }
            else
            {
                if (Vector3.Distance(this.transform.position, prevSnakeModule.GetLastPosition()) > tolerance)
                {
                    this.GetComponent<Rigidbody2D>().velocity = snakeHead.currentVelocity * Vector3.Distance(this.transform.position, prevSnakeModule.GetLastPosition())/tolerance * Vector3.Normalize(prevSnakeModule.GetLastPosition() - this.transform.position);
                }
            }
        }
	}

    private void LateUpdate()
    {
        // Draw a line between this module and the next if they are connected
        lineRenderer.sortingOrder = 1;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        //lineRenderer.material.color = Color.green;
        lineRenderer.material.color = color;
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.SetPosition(0, this.transform.position);
        if (AskForContinuity()) { lineRenderer.SetPosition(1, prevModule.transform.position); }
        else { lineRenderer.SetPosition(1, prevSnakeModule.GetLastPosition()); }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        float m_margin = margin;
        if (col.tag == "XBorder")
        {
            tpSwitch = !tpSwitch;
            lastPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
            if (lastPosition.x < 0) { m_margin = m_margin * -1; }
            this.transform.position = new Vector3(-this.transform.position.x + m_margin, this.transform.position.y, this.transform.position.z);
        }
        else if (col.tag == "YBorder")
        {
            tpSwitch = !tpSwitch;
            lastPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
            if (lastPosition.y < 0) { m_margin = m_margin * -1; }
            this.transform.position = new Vector3(this.transform.position.x, -this.transform.position.y + m_margin, this.transform.position.z);
        }
    }

    public void SetPrevModule(GameObject module)
    {
        prevModule = module;
        prevSnakeModule = prevModule.GetComponent<SnakeModule>();
        tpSwitch = prevSnakeModule.AskForTp();
        tolerance = prevModule.GetComponent<CircleCollider2D>().radius * 2f;
    }

    public Vector3 GetLastPosition()
    {
        return lastPosition;
    }

    public Vector3 GetParsePosition()
    {
        if (tpSwitch == prevSnakeModule.AskForTp())
        {
            return this.transform.position;
        }
        else
        {
            return lastPosition;
        }
    }

    public bool AskForTp()
    {
        return tpSwitch;
    }

    public bool AskForContinuity()
    {
        bool continuity = true;
        if (tpSwitch != prevSnakeModule.AskForTp()) { continuity = false; }
        return continuity;
    }

    public void TurnOff()
    {
        this.gameObject.tag = "Free";
    }
}
