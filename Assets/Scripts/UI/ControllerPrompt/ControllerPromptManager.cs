using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerPromptManager : Singleton<ControllerPromptManager> {

    [SerializeField]
    private GameObject controllerPromptPrefab;

    private ControllerPrompt[] activePrompts;
    //the list of actions known up to that point
    public HashSet<PlayerCompositeActions> KnownActions
    {
        get; private set;
    }

    [SerializeField]
    private float mainPromptScaleMultiplier;
    [SerializeField]
    private float sidePromptScaleMultiplier;

    [SerializeField]
    private float sideSpacing;

    /* prompt displayed at the center of the screen, if null no prompt displayed */
    private ControllerPrompt mainPrompt;
    public ControllerPrompt MainPrompt
    {
        get { return mainPrompt; }
    }

    private ObjectPool controllerPromptPool;

    private void Start()
    {
        //must do this in Start, otherwise Player is not initialized yet
        SubscribeToPlayer();
    }

    protected override void Initialize()
    {
        controllerPromptPool = GetComponent<ObjectPool>();

        KnownActions = new HashSet<PlayerCompositeActions>();
        //TODO: this is hacky, should change
        VerticalLayoutGroup layoutGroup = UserInterface.Instance.Create(UIElemType.CONTROLLER_PROMPT_PANEL).GetComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = sideSpacing - ControllerPrompt.DEFAULT_HEIGHT * (1.0f - sidePromptScaleMultiplier);
    }

    public void OnEnterEvent(EntityState playerState)
    {
        string[] availableActions = FilterKnownAvailableActions(
            playerState.GetAvailableActions(), KnownActions);
        
        //TODO add crosses to previous actions if length == 0
        if (availableActions.Length > 0)
        {
            controllerPromptPool.DestroyAllObjects();

            activePrompts = new ControllerPrompt[availableActions.Length];
            for (int i = 0; i < availableActions.Length; i++)
            {
                activePrompts[i] = controllerPromptPool.NewObject().GetComponent<ControllerPrompt>();

                activePrompts[i].GetComponent<RectTransform>().localScale = sidePromptScaleMultiplier * Vector3.one;

                activePrompts[i].transform.SetParent(UserInterface.Instance.Get(UIElemType.CONTROLLER_PROMPT_PANEL).transform);

                activePrompts[i].AtomicActions = PlayerActionsMap.GetAtomicActions(availableActions[i]);
                activePrompts[i].CompositeActionText = availableActions[i];
            }
        }
    }

    string[] FilterKnownAvailableActions(string[] actions, HashSet<PlayerCompositeActions> known)
    {
        HashSet<string> actionSet = new HashSet<string>(actions);
        HashSet<string> knownSet = new HashSet<string>();

        foreach (PlayerCompositeActions action in known)
        {
            knownSet.Add(action.DisplayName());
        }

        actionSet.IntersectWith(knownSet);

        return new List<string>(actionSet).ToArray();
    }

    public void AssignActionToMainControllerPrompt(string action)
    {
        AssignActionToMainControllerPrompt(action, action);
    }

    public void AssignActionToMainControllerPrompt(string action, string commandText)
    {
        if (mainPrompt != null)
        {
            DisableMainControllerPrompt();
        }

        mainPrompt = UserInterface.Instance.InstantiatePrefab(controllerPromptPrefab, overlay: true).GetComponent<ControllerPrompt>();
        mainPrompt.AtomicActions = PlayerActionsMap.GetAtomicActions(action);
        mainPrompt.CompositeActionText = commandText;

        PositionMainControllerPrompt();
    }

    void PositionMainControllerPrompt()
    {
        RectTransform rt = mainPrompt.GetComponent<RectTransform>();
        rt.anchorMax = rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.localScale = mainPromptScaleMultiplier * Vector2.one;
        rt.anchoredPosition = new Vector2(0.0f, 100.0f);
    }

    public void DisableMainControllerPrompt()
    {
        if (mainPrompt != null)
        {
            Destroy(mainPrompt.gameObject);
        }
    }

    public void SubscribeToPlayer()
    {
        Player player = EntityFactory.Instance.GetPlayer();
        player.OnEnterStateEvent.AddListener(OnEnterEvent);
    }

    public void UnsubscribeFromPlayer()
    {
        Player player = EntityFactory.Instance.GetPlayer();
        if (player != null)
        {
            player.OnEnterStateEvent.RemoveListener(OnEnterEvent);
        }
    }
}
