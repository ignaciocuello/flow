using System.Collections.Generic;
using UnityEngine;

//TODO take KB map as template for doing this
public class XB1ControllerImageMap : ScriptableObject {

    public bool TryGetValue(PlayerAtomicAction atomicAction, out List<Sprite> sprites)
    {
        sprites = null;
        return false;
    }
}
