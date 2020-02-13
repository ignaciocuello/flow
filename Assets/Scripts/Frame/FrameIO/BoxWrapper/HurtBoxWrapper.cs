using UnityEngine;

public class HurtBoxWrapper : BoxWrapper {

    public override void Initialize(Box box)
    {
        base.Initialize(box);

        SetSpriteRendererColor();
    }

    public override void OnValidate()
    {
        SetSpriteRendererColor();
    }

    public override Box Convert()
    {
        HurtBox hurtBox = new HurtBox();
        SetUpBox(hurtBox);

        return hurtBox;
    }

    void SetSpriteRendererColor()
    {
        GetComponent<SpriteRenderer>().color = HurtBox.DRAW_COLOR;
    }
}
