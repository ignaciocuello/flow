using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerTriggerEvent : UnityEvent<Player>
{
}

[Serializable]
public class BossTriggerEvent : UnityEvent<Boss>
{
}

[Serializable]
public class FocusTriggerEvent : UnityEvent<Focus>
{
}

public class Trigger : MonoBehaviour {

    private bool playerHasEntered;
    private bool playerHasExited;

    [SerializeField]
    private PlayerTriggerEvent onPlayerEntered;
    [SerializeField]
    private PlayerTriggerEvent onPlayerEnteredOnce;

    [SerializeField]
    private PlayerTriggerEvent onPlayerExited;
    [SerializeField]
    private PlayerTriggerEvent onPlayerExitedOnce;

    [SerializeField]
    private BossTriggerEvent onBossEntered;
    [SerializeField]
    private BossTriggerEvent onBossExited;

    [SerializeField]
    private FocusTriggerEvent onFocusEntered;
    [SerializeField]
    private FocusTriggerEvent onFocusExited;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Player player = collider.GetComponent<Player>();
            onPlayerEntered.Invoke(player);

            if (!playerHasEntered)
            {
                onPlayerEnteredOnce.Invoke(player);

                playerHasEntered = true;
            }
        }
        else if (collider.CompareTag("Focus"))
        {
            onFocusEntered.Invoke(collider.GetComponent<Focus>());
        }
        else if (collider.CompareTag("Boss"))
        {
            onBossEntered.Invoke(collider.GetComponent<Boss>());
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Player player = collider.GetComponent<Player>();
            onPlayerExited.Invoke(player);

            if (!playerHasExited)
            {
                onPlayerExitedOnce.Invoke(player);

                playerHasExited = true;
            }
        }
        else if (collider.CompareTag("Focus"))
        {
            onFocusExited.Invoke(collider.GetComponent<Focus>());
        }
        else if (collider.CompareTag("Boss"))
        {
            onBossExited.Invoke(collider.GetComponent<Boss>());
        }
    }
}
