using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FogController : MonoBehaviour {

    private GameObject player;
    private Image image;
    private RectTransform transform;
    [SerializeField]
    private float width = 8f;
    [SerializeField]
    private float factor = 8f;
    [SerializeField]
    private float velocity = 4f;
    private float timer = 0f;
    private float x;
    private float y;
    private float targetAlpha;
    private float currentAlpha = 1f;
    [SerializeField]
    private float fillingFactor;

    // Set up references
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        image = GetComponent<Image>();
        transform = GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += velocity * Time.smoothDeltaTime;
        while (timer >= 90) { timer -= 90; }
        x = width + factor * Mathf.Cos(timer);
        y = width + factor * Mathf.Sin(timer);
        targetAlpha = player.GetComponent<SnakeHead>().sleepiness * 0.01f;
        if (currentAlpha < targetAlpha)
        {
            currentAlpha += fillingFactor * Time.smoothDeltaTime;
            if (currentAlpha > targetAlpha) { currentAlpha = targetAlpha; }
        }
        else if (currentAlpha > targetAlpha)
        {
            currentAlpha -= fillingFactor * Time.smoothDeltaTime;
            if (currentAlpha < targetAlpha)
            {
                currentAlpha = targetAlpha;
            }
        }
    }

    // LateUpdate is called once before rendering
    private void LateUpdate()
    {
        transform.localScale = new Vector3(x, y, 1f);
        image.canvasRenderer.SetAlpha(targetAlpha);
    }
}
