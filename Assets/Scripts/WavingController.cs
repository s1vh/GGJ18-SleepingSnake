using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavingController : MonoBehaviour {

    Transform transform;
    //private Quaternion baseRotation;
    private Quaternion newRotation;
    [SerializeField]
    private float width = 8f;
    [SerializeField]
    private float velocity = 4f;
    private float timer = 0f;

    // Set up references
    void Awake()
    {
        transform = GetComponent<Transform>();
    }

    // Use this for initialization
    void Start ()
    {
        //baseRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += velocity * Time.smoothDeltaTime;
        while (timer >= 360) { timer -= 360; }
        newRotation = Quaternion.Euler(0f, 0f, width*Mathf.Sin(timer));
    }

    // LateUpdate is called once before rendering
    private void LateUpdate()
    {
        transform.rotation = newRotation;
    }
}
