using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    public Color OffColor;
    public Color OnColor;

    private SpriteRenderer spriteRenderer;

    private bool on;
    public bool On
    {
        set
        {
            on = value;
            spriteRenderer.color = on ? OnColor : OffColor;
        }
        get { return on; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

	public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Focus"))
        {
            On = !On;
        }
    }
}
