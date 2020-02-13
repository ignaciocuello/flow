using UnityEngine;
using UnityEngine.PostProcessing;

public class TimeManager : Singleton<TimeManager> {

    public const float DEFAULT_FIXED_DELTA_TIME = 1.0f / 60.0f;

    /* slow down factor at which the contrast is set to 2 */
    [SerializeField]
    private float minimumSlowDownContrast;
    /* minimum contrast value */
    [SerializeField]
    private float minimumContrast;

    private float slowDownFactor;
    private float slowDownDuration;
    /* how great is the rate of change from slow down to normal */
    private float exponentialGrowthFactor;

    private float elapsedUnscaled;

    private PostProcessingProfile postProcessing;

    protected override void Initialize()
    {
        slowDownDuration = 0.0f;
        slowDownFactor = 0.0f;
        elapsedUnscaled = 0.0f;
        exponentialGrowthFactor = 0.0f;
        
        postProcessing = Camera.main.GetComponent<PostProcessingBehaviour>().profile;

        SetTimeScale(1.0f, modifyFixedDelta: true);
    }

    void Update()
    {
        if (slowDownFactor != 0.0f && slowDownDuration != 0.0f)
        {
            elapsedUnscaled += Time.unscaledDeltaTime;
            if (elapsedUnscaled >= slowDownDuration)
            {
                Initialize();
            }
            else
            {
                SetTimeScale(Mathf.Clamp(CalculateTimeScale(), 0.0f, 1.0f), modifyFixedDelta: true);
                Time.fixedDeltaTime = Mathf.Clamp(
                    DEFAULT_FIXED_DELTA_TIME * Time.timeScale, 0.0f, DEFAULT_FIXED_DELTA_TIME);
            }
        }
    }

    public void SetSlowDownFactor(float slowDownFactor, float slowDownDuration, float exponentialGrowthFactor)
    {
        this.slowDownDuration = slowDownDuration;
        this.slowDownFactor = slowDownFactor;
        this.exponentialGrowthFactor = exponentialGrowthFactor;

        SetTimeScale(slowDownFactor, modifyFixedDelta: true);

        elapsedUnscaled = 0.0f;
    }

    public void FreezeTime()
    {
        slowDownFactor = 0.0f;
        SetTimeScale(slowDownFactor, modifyFixedDelta: false);
    }

    public void UnfreezeTime(float unfreezeDuration, float exponentialGrowthFactor)
    {
        SetSlowDownFactor(0.1f, unfreezeDuration, exponentialGrowthFactor);
    }

    private float CalculateTimeScale()
    {
        return slowDownFactor + (1.0f - slowDownFactor) * NormalizedExponentialFraction(exponentialGrowthFactor);
    }

    /* elapsedFraction = elapsedTime / slowDownFactor , returns 0 if elapsedTime is 0, 1 if elapsedTime = slowDownFactor,
     increases from 0 to 1 exponentially. */
    private float NormalizedExponentialFraction(float b)
    {
        return (Mathf.Pow(b, elapsedUnscaled/slowDownDuration) - 1.0f) / (b - 1.0f);
    }

    public void ResetTimeScale()
    {
        SetTimeScale(1.0f, modifyFixedDelta: true);
    }

    private void SetTimeScale(float timeScale, bool modifyFixedDelta)
    {
        Time.timeScale = timeScale;
        if (modifyFixedDelta)
        {
            Time.fixedDeltaTime = DEFAULT_FIXED_DELTA_TIME * timeScale;
        }

        ColorGradingModel.Settings settings = postProcessing.colorGrading.settings;
        settings.basic.contrast = GetContrast();

        postProcessing.colorGrading.settings = settings;
    }

    private float GetContrast()
    {
        //add proper range
        return Mathf.Lerp(minimumContrast, 1.0f, (Time.timeScale-minimumSlowDownContrast)/(1.0f-minimumSlowDownContrast));
    }
}
