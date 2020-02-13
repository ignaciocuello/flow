using UnityEngine;

public class Coin : Entity {

    [SerializeField]
    private GameObject shineParticleSystemPrefab;
    [SerializeField]
    private AudioClip collectClip;
    [SerializeField]
    private float collectClipVolumeScale;

    [SerializeField]
    private RestingCoinState restingState;

    //this is here because Destroy doesn't make the equality check against null true instantly, to prevent double collection of wad
    private bool collected;

    public override void Awake()
    {
        base.Awake();

        FacingRight = false;

        Rest();
    }

    public override void InitStates()
    {
        restingState = Instantiate(restingState);
    }

    public void Rest()
    {
        State = restingState;
    }

    public override void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Collect(collider.GetComponent<Player>());
        }
    }

    public void Collect(Player player)
    {
        if (!collected)
        {
            AddToPlayerInventory(player.Inventory);
            AudioSourcePool.Instance.PlayOneShotClipAt(collectClip, transform.position, collectClipVolumeScale);

            GameObject shineParticleSystem = Instantiate(shineParticleSystemPrefab);
            shineParticleSystem.transform.position = transform.position;

            Destroy(gameObject);

            collected = true;
        }
    }

    public virtual void AddToPlayerInventory(Inventory inventory)
    {
        inventory.NumCoins++;
    }
}
