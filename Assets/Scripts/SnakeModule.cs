using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeModule : MonoBehaviour {

    private SnakeHead snakeHead;
    private GameObject head;
    private GameObject prevModule;
    //private LineRenderer lineRenderer;
    private List<Vector3> savedTpPoints = new List<Vector3>();
    private float tolerance;
    //[SerializeField]
    //private Color color;
    [SerializeField]
    private float margin = 0.1f;
    private Vector3 scale;
    [SerializeField]
    private int tpIndex;
    private bool isLast;

    // Set up references
    void Awake ()
    {
        head = GameObject.Find("SnakeHead");
        snakeHead = head.GetComponent<SnakeHead>();
        //lineRenderer = GetComponent<LineRenderer>();
    }

    // Use this for initialization
    void Start ()
    {
        scale = this.transform.localScale;
        if (this.gameObject == head && prevModule == null) { SetPrevModule(head); tpIndex = 0; }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject != head && (prevModule == null || prevModule == this.gameObject)) { Object.Destroy(this.gameObject); }
        /*This fixes a weird bug where a new module is spawning
         * when its previous module, preventing the new module to
         * get the reference*/

        if (this.gameObject.tag == "Inactive" && this.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
        {
            this.gameObject.tag = "Body";
        }

        if (tpIndex == prevModule.GetComponent<SnakeModule>().AskForTp())
        {
            Debug.DrawLine(this.transform.position, prevModule.transform.position, Color.green);
        }
        else
        {
            Debug.DrawLine(this.transform.position, prevModule.GetComponent<SnakeModule>().ReadLastPosition(tpIndex), Color.green);
        }
    }

    void FixedUpdate ()
    {
        if (this.gameObject != head)
        {
            if (isLast)
            {
                this.transform.position = new Vector3(prevModule.transform.position.x, prevModule.transform.position.y, prevModule.transform.position.z);
                tpIndex = prevModule.GetComponent<SnakeModule>().AskForTp();
            }
            else if (AskForContinuity())
            {
                if (Vector3.Distance(this.transform.position, prevModule.transform.position) > tolerance)
                {
                    this.GetComponent<Rigidbody2D>().velocity = snakeHead.currentVelocity * Vector3.Distance(this.transform.position, prevModule.transform.position) / tolerance * Vector3.Normalize(prevModule.transform.position - this.transform.position);
                }
            }
            else
            {
                if (true)
                {
                    if (Vector3.Distance(this.transform.position, prevModule.GetComponent<SnakeModule>().ReadLastPosition(tpIndex)) >= tolerance)
                    {
                        this.GetComponent<Rigidbody2D>().velocity = snakeHead.currentVelocity * Vector3.Distance(this.transform.position, prevModule.GetComponent<SnakeModule>().ReadLastPosition(tpIndex)) / tolerance * Vector3.Normalize(prevModule.GetComponent<SnakeModule>().ReadLastPosition(tpIndex) - this.transform.position);
                    }
                }
            }
        }
	}

    /*private void LateUpdate()
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
        else { lineRenderer.SetPosition(1, prevModule.GetComponent<SnakeModule>().GetLastPosition()); }
    }*/

    private void OnTriggerEnter2D(Collider2D col)   // when entering the same trigger too quickly, it still gets buggy!
    {
        float x_margin = margin;
        float y_margin = margin;
        if (col.tag == "Corner")
        {
            SaveCurrentPosition();
            tpIndex++;
            if (this.transform.position.x < 0) { x_margin = x_margin * -1; }
            if (this.transform.position.y < 0) { y_margin = y_margin * -1; }
            this.transform.position = new Vector3(-this.transform.position.x + x_margin, -this.transform.position.y + y_margin, this.transform.position.z);
        }
        else if (col.tag == "XBorder")
        {
            SaveCurrentPosition();
            tpIndex++;
            if (this.transform.position.x < 0) { x_margin = x_margin * -1; }
            this.transform.position = new Vector3(-this.transform.position.x + x_margin, this.transform.position.y, this.transform.position.z);
        }
        else if (col.tag == "YBorder")
        {
            SaveCurrentPosition();
            tpIndex++;
            if (this.transform.position.y < 0) { y_margin = y_margin * -1; }
            this.transform.position = new Vector3(this.transform.position.x, -this.transform.position.y + y_margin, this.transform.position.z);
        }
    }

    private void SaveCurrentPosition()
    {
        Vector3 position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        savedTpPoints.Add(position);
    }

    public Vector3 ReadLastPosition(int index)
    {
        if (index > savedTpPoints.Count - 1 && savedTpPoints.Count > 0) { index = savedTpPoints.Count - 1;  }
        Vector3 position = new Vector3(savedTpPoints[index].x, savedTpPoints[index].y, savedTpPoints[index].z);
        return position;
    }

    public void SetPrevModule(GameObject module)
    {
        prevModule = module;
        tpIndex = prevModule.GetComponent<SnakeModule>().AskForTp();
        tolerance = prevModule.GetComponent<CircleCollider2D>().radius * 2f;
    }

    public int AskForTp()
    {
        return tpIndex;
    }

    public bool AskForContinuity()
    {
        bool continuity = true;
        if (tpIndex != prevModule.GetComponent<SnakeModule>().AskForTp()) { continuity = false; }
        return continuity;
    }

    public void SetTpStatus(int status)
    {
        tpIndex = status;
    }

    public void SetLastStatus(bool last)
    {
        isLast = last;
    }

    /*public void NormalizeModulesStatus()    // Recursive check on the snake modules!
    {
        if (this.gameObject != head && Vector3.Distance(this.transform.position, prevModule.transform.position) <= tolerance)
        {
            prevModule.GetComponent<SnakeModule>().SetTpStatus(tpIndex);
            prevModule.GetComponent<SnakeModule>().NormalizeModulesStatus();
        }
    }*/

    public void TurnOff()
    {
        this.gameObject.tag = "Free";
    }

    public void ResizeModule(float factor)
    {
        if (factor < 0.1f) { factor = 0.1f; }
        this.transform.localScale = new Vector3(scale.x, scale.y, scale.z) * factor;
        tolerance = prevModule.GetComponent<CircleCollider2D>().radius * 2f * factor;
    }
}
