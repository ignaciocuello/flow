using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerPrompt : MonoBehaviour {

    public const float DEFAULT_HEIGHT = 100.0f;

    [SerializeField, Range(0.0f,1.0f)]
    private float alpha;

    private PlayerAtomicAction[] atomicActions;
    public PlayerAtomicAction[] AtomicActions
    {
        get { return atomicActions; }
        set
        {
            atomicActions = value;
            updateLayoutOnNextFrame = true;
        }
    }

    [SerializeField]
    private string compositeActionText;
    public string CompositeActionText
    {
        get { return compositeActionText; }
        set
        {
            compositeActionText = value;
            updateLayoutOnNextFrame = true;
        }
    }

    /* sprite of a 'plus' sign */
    [SerializeField]
    private Sprite plusSprite;
    /* size of 'plus' sign */
    [SerializeField]
    private int plusSize;

    /* text of child components */
    private Text childText;

    private ObjectPool imagePool;

    private bool awoken;
    private bool updateLayoutOnNextFrame;

    void Awake()
    {
        childText = GetComponentInChildren<Text>();
        childText.material = new Material(childText.material);

        Image image = GetComponent<Image>();
        image.material = new Material(image.material);

        imagePool = GetComponent<ObjectPool>();
        UpdateAlpha();

        awoken = true;
    }

    void OnValidate()
    {
        if (awoken)
        {
            UpdateLayout();
        }
    }

    /*set correct transparency on self and children */
    void UpdateAlpha()
    {
        //original color of images/text
        Color c;
        Image[] images = GetComponentsInChildren<Image>();
        if (images != null)
        {
            foreach (Image image in images)
            {
                c = image.material.color;
                image.material.color = new Color(c.r, c.g, c.b, alpha);
            }
        }

        c = childText.material.color;
        childText.material.color = new Color(c.r, c.g, c.b, alpha);
    }
    public void UpdateLayout()
    {
        UpdateCompositeActionText();
        UpdateAtomicActionsImages();
        UpdateAlpha();
    }

    void UpdateCompositeActionText()
    {
        childText.text = compositeActionText.ToUpper();
    }

    void UpdateAtomicActionsImages()
    {
        imagePool.DestroyAllObjects();

        //if atomicActions.size > 1 add + 

        int i = 0;
        foreach (PlayerAtomicAction atomicAction in atomicActions)
        {
            if (i > 0)
            {
                Image plus = AddImage(plusSprite);
                plus.rectTransform.sizeDelta = Vector2.one * plusSize;
            }

            List<Sprite> sprites;
            ControllerImageMap.Instance.TryGetValue(atomicAction, out sprites);
            if (sprites != null)
            {
                foreach (Sprite sprite in sprites)
                {
                    AddImage(sprite);
                }
            }

            i++;
        }
    }

    private Image AddImage(Sprite sprite)
    {
        Image img = imagePool.NewObject().GetComponent<Image>();
        img.sprite = sprite;
        img.material = new Material(img.material);

        return img;
    }

    void Update()
    {
        if (updateLayoutOnNextFrame)
        {
            UpdateLayout();
            updateLayoutOnNextFrame = false;
        }
    }
}
