using UnityEngine;

public abstract class BoxWrapper : MonoBehaviour {

    public string Id;

    public virtual void Initialize(Box box)
    {
        transform.localPosition = box.BoundsData.Center;
        transform.localScale = box.BoundsData.Size;

        Id = box.Id;
    }

    public virtual void OnValidate()
    {
    }

    public void SetUpBox(Box box)
    {
        SetBounds(box);
        SetId(box);
    }

    public void SetBounds(Box box)
    {
        box.BoundsData = new BoundsData(center: transform.localPosition, size: transform.localScale);
    }

    public void SetId(Box box)
    {
        box.Id = Id;
    }

    public abstract Box Convert();

}
