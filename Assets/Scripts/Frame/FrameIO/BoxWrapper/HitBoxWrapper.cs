using UnityEngine;

public class HitBoxWrapper : BoxWrapper {

    public HitData HitData;

    public override void Initialize(Box box)
    {
        base.Initialize(box);

        SetSpriteRendererColor();

        HitBox hb = (HitBox)box;
        HitData = hb.HitData;
    }

    public override void OnValidate()
    {
        SetSpriteRendererColor();
    }

    public override Box Convert()
    {
        HitBox hitBox = new HitBox
        {
            HitData = HitData
        };
        SetUpBox(hitBox);

        return hitBox;
    }

    void SetSpriteRendererColor()
    {
        GetComponent<SpriteRenderer>().color = HitBox.DRAW_COLOR;
    }
}
