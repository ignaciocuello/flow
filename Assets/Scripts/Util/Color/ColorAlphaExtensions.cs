using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorAlphaExtensions {

	public static Color ChangeAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
