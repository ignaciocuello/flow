using UnityEngine;

public class TransformTracker : Singleton<TransformTracker> {

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private Transform target;
    public Transform Target
    {
        get { return target; }
        set
        {
            target = value;
        }
    }

    public Vector3? FixedTarget;

    [SerializeField]
    private float targetSize;
    public float TargetSize
    {
        get { return targetSize; }
        set
        {
            targetSize = value;
        }
    }

    public Vector3 Offset;

    [SerializeField]
    private float horizontalTrackingSpeed;
    public float HorizontalTrackingSpeed
    {
        get { return horizontalTrackingSpeed; }
        set { horizontalTrackingSpeed = value; }
    }

    [SerializeField]
    private float verticalTrackingSpeed;
    public float VerticalTrackingSpeed
    {
        get { return verticalTrackingSpeed; }
        set { verticalTrackingSpeed = value; }
    }

    [SerializeField]
    private float sizeTrackingSpeed;
    public float SizeTrackingSpeed
    {
        get { return sizeTrackingSpeed; }
        set { sizeTrackingSpeed = value; }
    }

    /* default values for everything */
    public Transform DefaultTarget;

    public float DefaultTargetSize;

    public float DefaultHorizontalTrackingSpeed;
    public float DefaultVerticalTrackingSpeed;
    public float DefaultSizeTrackingSpeed;

    /* size gain due to transform momentum */
    private float sizeGain;

    /* speed should be above component values to trigger gains */
    [SerializeField]
    private Vector2 gainsThresholds;
    [SerializeField]
    private float sizeGainMultiplier;
    /* how much distance the camera is allowed to lag behind the transform, in each component */
    [SerializeField]
    private float targetTime;
    [SerializeField]
    private Vector2 maxLag;

    private Vector3 startingPosition;
    private float startingSize;

    protected override void Initialize()
    {
        DefaultTarget = target;
        DefaultTargetSize = targetSize;

        DefaultHorizontalTrackingSpeed = horizontalTrackingSpeed;
        DefaultVerticalTrackingSpeed = verticalTrackingSpeed;
        DefaultSizeTrackingSpeed = sizeTrackingSpeed;

        startingPosition = transform.position;
        startingSize = Camera.main.orthographicSize;

        DontDestroyOnLoad(gameObject);
    }

    public void PrepareForReset()
    {
        DefaultTarget = null;
        target = null;
        FixedTarget = null;
    }

    public void TrackPlayer()
    {
        //reset our position/size to default
        transform.position = startingPosition;
        Camera.main.orthographicSize = startingSize;

        //must do this in Start, otherwise Player is not initialized yet
        if (target == null)
        {
            //track the player if no target is given initially
            Player player = EntityFactory.Instance.GetPlayer();
            Target = player.transform;
            DefaultTarget = player.transform;
        }    
    }

	void FixedUpdate () {
        if (FixedTarget != null)
        {
            ResetGainsDueToMomentum();

            Track((Vector3)FixedTarget, Vector2.positiveInfinity);
        }
        else if (target != null)
        {
            /*float height = 2.0f * Camera.main.orthographicSize;
            float width = Camera.main.aspect * height;

            float velX = target.GetComponent<Rigidbody2D>().velocity.x;
            if (velX > gainsThresholds.x)
            {
                float targetD = Mathf.Abs(velX) / targetTime;
                float trgtSize = targetD / Camera.main.aspect;

                sizeGain = trgtSize - targetSize;
                Debug.Log("velX = " + velX);
                Debug.Log("targetD = " + targetD);
                Debug.Log("sizeGain = " + sizeGain);
            }
            else
            {
                sizeGain = 0.0f;
            }*/
            CalculateGainsDueToMomentum();

            Track(target.position, maxLag);
        }

        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize + sizeGain, Time.deltaTime * sizeTrackingSpeed);
	}

    private void ResetGainsDueToMomentum()
    {
        sizeGain = 0.0f;
    }

    private void CalculateGainsDueToMomentum()
    {
        Rigidbody2D rb;
        //TODO make this lerp
        if (target != null && (rb = target.GetComponent<Rigidbody2D>()) != null)
        {
            Vector2 speedDif = new Vector2(Mathf.Abs(rb.velocity.x), Mathf.Abs(rb.velocity.y));
            Vector2 spGain = speedDif - gainsThresholds;

            //set speedGain components to >= 0
            //speedGain = Vector2.Max(spGain, Vector2.zero);
            //set components in proper directions
            //speedGain = new Vector2(
            //    (rb.velocity.x < 0.0f ? -1.0f : 1.0f) * speedGain.x, (rb.velocity.y < 0.0f ? -1.0f : 1.0f) * speedGain.y);
            

            sizeGain = sizeGainMultiplier * Vector2.Max(spGain, Vector2.zero).magnitude;
        }
        else
        {
            ResetGainsDueToMomentum();
        }     
    }

    private void Track(Vector3 trgt, Vector2 maxLag)
    {
        trgt += Offset;

        float x = Mathf.Lerp(transform.position.x, trgt.x, Time.deltaTime * horizontalTrackingSpeed);
        float y = Mathf.Lerp(transform.position.y, trgt.y, Time.deltaTime * verticalTrackingSpeed);

        if (sizeGain > 0.0f)
        {
            if (x - trgt.x > maxLag.x)
            {
                x = maxLag.x + trgt.x;
            }
            else if (trgt.x - x > maxLag.x)
            {
                x = -maxLag.x + trgt.x;
            }

            if (y - trgt.y > maxLag.y)
            {
                y = maxLag.y + trgt.y;
            }
            else if (trgt.y - y > maxLag.y)
            {
                y = -maxLag.y + trgt.y;
            }
        }

        transform.position = new Vector3(x, y, transform.position.z);
    }

    public void ResetToDefault()
    {
        Target = DefaultTarget;
        TargetSize = DefaultTargetSize;
        FixedTarget = null;
        Offset = Vector3.zero;

        HorizontalTrackingSpeed = DefaultHorizontalTrackingSpeed;
        VerticalTrackingSpeed = DefaultVerticalTrackingSpeed;
        SizeTrackingSpeed = DefaultSizeTrackingSpeed;
    }
}
