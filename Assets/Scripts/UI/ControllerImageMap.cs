using System.Collections.Generic;
using UnityEngine;

public class ControllerImageMap : Singleton<ControllerImageMap> {

    [SerializeField]
    private XB1ControllerImageMap xb1Map;
    [SerializeField]
    private KBControllerImageMap kbMap;

    public bool TryGetValue(PlayerAtomicAction atomicAction, out List<Sprite> sprites)
    {
        return InputBuffer.Instance.IsUsingJoystick ? 
            xb1Map.TryGetValue(atomicAction, out sprites) : kbMap.TryGetValue(atomicAction, out sprites);
    }
}
