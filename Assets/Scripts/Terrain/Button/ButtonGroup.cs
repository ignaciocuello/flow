using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGroup : MonoBehaviour {

    [SerializeField]
    private GameObject controlled;

    private SpriteRenderer[] controlledSpriteRenderers;
    private LineRenderer[] controlledLineRenderers;
    private Collider2D[] controlledColliders;

    private Button[] childButtons;

    [SerializeField]
    private Color disabledColor;

    

    private Dictionary<SpriteRenderer, Color> originalControlledSpriteColors;
    private Dictionary<LineRenderer, Color> originalControlledLineColors;

    //use start since must wait for awake to be called on button
	private void Start()
    {
        childButtons = GetComponentsInChildren<Button>();

        Color offColor = new Color(disabledColor.r, disabledColor.g, disabledColor.b, 1.0f);
        foreach (Button child in childButtons)
        {
            child.OffColor = offColor;
            child.On = false;
        }

        controlledSpriteRenderers = controlled.GetComponentsInChildren<SpriteRenderer>();
        controlledLineRenderers = controlled.GetComponentsInChildren<LineRenderer>();
        controlledColliders = controlled.GetComponentsInChildren<Collider2D>();

        CacheOriginalColors();
    }

    private void CacheOriginalColors()
    {
        originalControlledSpriteColors = new Dictionary<SpriteRenderer, Color>();
        foreach (SpriteRenderer sr in controlledSpriteRenderers)
        {
            originalControlledSpriteColors[sr] = sr.color;
        }

        originalControlledLineColors = new Dictionary<LineRenderer, Color>();
        foreach (LineRenderer lr in controlledLineRenderers)
        {
            originalControlledLineColors[lr] = lr.startColor;
        }
    }

    private void FixedUpdate()
    {
        if (AllChildButtonsOn())
        {
            foreach (SpriteRenderer sr in controlledSpriteRenderers)
            {
                sr.color = originalControlledSpriteColors[sr];
            }
            foreach (LineRenderer lr in controlledLineRenderers)
            {
                lr.startColor = lr.endColor = originalControlledLineColors[lr];
            }
            foreach (Collider2D collider in controlledColliders)
            {
                collider.enabled = true;
            }
        }
        else
        {
            foreach (SpriteRenderer sr in controlledSpriteRenderers)
            {
                sr.color = disabledColor;
            }
            foreach (LineRenderer lr in controlledLineRenderers)
            {
                lr.startColor = disabledColor;
                lr.endColor = disabledColor;
            }
            foreach (Collider2D collider in controlledColliders)
            {
                collider.enabled = false;
            }
        }
    }

    private bool AllChildButtonsOn()
    {
        foreach (Button child in childButtons)
        {
            if (!child.On)
            {
                return false;
            }
        }

        return true;
    }
}
