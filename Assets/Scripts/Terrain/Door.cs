using System;
using UnityEngine;

public class Door : Device {

    private const float EPS = 1.0e-4f;

    /* 
     * degree of door openness
     * 0 if door is closed, 1 if fully open (if the door is open its lowest point will be 
     * at its highest point when it was closed) 
     */
    [SerializeField, Range(0, 1)]
    private float openness;
    public float Openness
    {
        get { return openness; }
    }

    [SerializeField, Tooltip("Size of Door(collider)")]
    private Vector2 size;
    public Vector2 Size
    {
        get { return size; }
    }

    [SerializeField]
    private SpriteRenderer upperHalf;
    [SerializeField]
    private SpriteRenderer middle;
    [SerializeField]
    private SpriteRenderer lowerHalf;
    [SerializeField]
    private SpriteRenderer valve;

    /* y value when door is considered closed */
    private float closedY;

    [SerializeField]
    private GameObject petDoorPrefab;

    /* true if this door is the final door of the current level */
    [SerializeField]
    private bool finalDoor;
    /* true if the player can go through the lower half of the door from one side, but not the other */
    [SerializeField]
    private bool usesPetDoor;
    [SerializeField]
    private float petDoorSize;

    public Action OnFullyOpen;
    public Action OnFullyClosed;

    private Animator animator;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D collider2d;

    private BoxCollider2D petDoorCollider;

    private Level level;
    private GameObject[] rewards;

    public bool Print;

    private void OnValidate()
    {
        if (size != Vector2.zero)
        {
            GetComponent<BoxCollider2D>().size = size;

            if (upperHalf != null)
            {
                SetSpriteRendererSizes();
            }
            else
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                sr.size = new Vector2(sr.size.x, size.y);
            }
        }
    }

    private void SetSpriteRendererSizes()
    {
        float borderHeight = (size.y - middle.sprite.bounds.size.y) / 2.0f;
        float shift = middle.sprite.bounds.size.y / 2.0f + borderHeight / 2.0f;

        SetBorderHeight(upperHalf, borderHeight, shift);
        SetBorderHeight(lowerHalf, borderHeight, -shift);
    }

    private void SetBorderHeight(SpriteRenderer borderSr, float borderHeight, float shift)
    {
        borderSr.size = new Vector2(borderSr.size.x, borderHeight);
        borderSr.transform.localPosition = shift * Vector3.up;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<BoxCollider2D>();

        level = GetComponentInParent <Level>();

        closedY = transform.localPosition.y;
    }

    private void Start()
    {
        if (usesPetDoor)
        {
            petDoorCollider = Instantiate(petDoorPrefab, transform).GetComponent<BoxCollider2D>();

            //swap components because petDoor is rotated by 90 degrees
            petDoorCollider.size = SwapComponents(collider2d.size);
            petDoorCollider.offset = new Vector2(-collider2d.offset.y, collider2d.offset.x);

            ModifyDoorCollider(petDoorSize);
        }
    }

    public override void PreHeat(Connection connection)
    {
        if (finalDoor)
        {
            LevelFinished();

            SetCollidersTriggers(true);
            ResetDoorCollider();

            valve.GetComponent<Animator>().SetBool("Open", true);

            OnFullyOpen += FullyOpenCallback;
        }
    }

    private void FullyOpenCallback()
    {
        SetCollidersTriggers(false);
        ModifyDoorCollider(petDoorSize);
        OnFullyOpen -= FullyOpenCallback;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (finalDoor && level != null && level.HasBeenFinished && collider.CompareTag("Player"))
        {
            animator.SetTrigger("Flash");
            if (rewards == null)
            {
                GiveRewards();
            }

            AutoCollectRewards();
        }
    }

    private void AutoCollectRewards()
    {
        Player player = EntityFactory.Instance.GetPlayer();

        Wad wad;
        Coin coin;
        foreach (GameObject reward in rewards)
        {
            if (reward != null)
            {
                //if is wad
                if ((wad = reward.GetComponent<Wad>()) != null)
                {
                    wad.Collect(player);
                }
                //else if is coin
                else if ((coin = reward.GetComponent<Coin>()) != null)
                {
                    coin.Collect(player);
                }
            }
        }
    }

    private void SetCollidersTriggers(bool isTrigger)
    {
        foreach (Collider2D collider2d in GetComponentsInChildren<Collider2D>())
        {
            collider2d.isTrigger = isTrigger;
        }
    }

    //modifies the door collider to allow pet door to function
    private void ModifyDoorCollider(float petDoorSize)
    {
        //gets the pet door size local to the gameObject
        //TODO fix this when doors sprite get included
        collider2d.size -= new Vector2(0.0f, petDoorSize);
        collider2d.offset += new Vector2(0.0f, petDoorSize / 2.0f);
    }

    private void ResetDoorCollider()
    {
        collider2d.size = SwapComponents(petDoorCollider.size);
        collider2d.offset = new Vector2(petDoorCollider.offset.y, -petDoorCollider.offset.x);
    }

    private void FixedUpdate()
    {
        //transform.localPosition = 
        //    new Vector3(transform.localPosition.x, closedY + openness * transform.localScale.y, transform.localPosition.z);

        Vector3 targetPos = new Vector3(transform.localPosition.x, closedY + openness * transform.localScale.y * size.y, transform.localPosition.z);

        //Transform Direction to account for rotations
        Vector3 diff = transform.TransformDirection(targetPos - transform.localPosition);

        Vector2 velocity = diff / Time.fixedDeltaTime;

        rigidbody2d.velocity = velocity;

        if (Mathf.Abs(openness) < EPS)
        {
            if (OnFullyClosed != null)
            {
                OnFullyClosed();
            }
        }
        else if (Mathf.Abs(openness - 1.0f) < EPS)
        {
            if (OnFullyOpen != null)
            {
                OnFullyOpen();
            }
        }
    }

    public void Open()
    {
        animator.SetBool("Open", true);

        if (finalDoor)
        {
            GiveRewards();
        }
    }

    private void GiveRewards()
    {
        //null check to allow testing doors that don't belong to a level
        if (rewards == null && level != null)
        {
            rewards = level.GiveRewards(this);
        }
    }

    private void LevelFinished()
    {
        //null check to allow testing doors that don't belong to a level
        if (level != null)
        {
            level.LevelFinished();
        }
    }

    public void Close()
    {
        animator.SetBool("Open", false);
    }

    public override void Activate()
    {
        Open();
    }

    public override void Deactivate()
    {
        Close();
    }

    private Vector2 SwapComponents(Vector2 vector)
    {
        return new Vector2(vector.y, vector.x);
    }
}
