using System.Collections.Generic;

public class PlayerActionsMap {

    private static Dictionary<string, PlayerAtomicAction[]> actionCommandsMap;

	static PlayerActionsMap()
    {
        InitActionCommandsMap();
    }

    static void InitActionCommandsMap()
    {
        actionCommandsMap = new Dictionary<string, PlayerAtomicAction[]>()
        {
            { PlayerCompositeActions.RUN.DisplayName(), new PlayerAtomicAction[] { PlayerAtomicAction.HORIZONTAL} },
            { PlayerCompositeActions.DRIFT.DisplayName(), new PlayerAtomicAction[] { PlayerAtomicAction.HORIZONTAL } },
            { PlayerCompositeActions.NORMAL_ATTACK.DisplayName(), new PlayerAtomicAction[] { PlayerAtomicAction.ATTACK } },
            { PlayerCompositeActions.DOWN_ATTACK.DisplayName(), new PlayerAtomicAction[] { PlayerAtomicAction.DOWN, PlayerAtomicAction.ATTACK } },
            { PlayerCompositeActions.JUMP.DisplayName(), new PlayerAtomicAction[] { PlayerAtomicAction.JUMP } },
            { PlayerCompositeActions.WALL_JUMP.DisplayName(), new PlayerAtomicAction[] { PlayerAtomicAction.JUMP } },
            { PlayerCompositeActions.JUMP_GAIN.DisplayName(), new PlayerAtomicAction[] { PlayerAtomicAction.JUMP } },
            { PlayerCompositeActions.RECALL.DisplayName(), new PlayerAtomicAction[] { PlayerAtomicAction.RECALL } },
        };
    }

    public static PlayerAtomicAction[] GetAtomicActions(string compositeAction)
    {
        PlayerAtomicAction[] atomicActions;
        if (actionCommandsMap.TryGetValue(compositeAction, out atomicActions))
        {
            return atomicActions;
        }
        else
        {
            return null;
        }
    }
}
