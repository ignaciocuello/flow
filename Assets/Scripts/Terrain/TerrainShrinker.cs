using UnityEngine;

public class TerrainShrinker : MonoBehaviour {

    [SerializeField]
    private Vector2 shrinkAmount;
    //duration of shrink in seconds
    [SerializeField]
    private float shrinkDuration;

    //the number of times this terrain has been shrunk
    private int timesShrinked;

    private AnimationFunction animFuncX;
    private AnimationFunction animFuncY;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Focus"))
        {
            AlterScale(shrinkAmount);
            timesShrinked++;
        }
    }

    public void OnFocusTrigger()
    {
        //reverse a single shrink
        if (timesShrinked > 0)
        {
            AlterScale(-shrinkAmount);
            timesShrinked--;
        }
    }

    private void AlterScale(Vector2 amount)
    {
        if (animFuncX != null && animFuncY != null)
        {
            UpdateLocalScale(animFuncX.EndValue, animFuncY.EndValue);
        }

        animFuncX = new AnimationFunction(Time.time, transform.localScale.x, shrinkDuration, transform.localScale.x - amount.x);
        animFuncY = new AnimationFunction(Time.time, transform.localScale.y, shrinkDuration, transform.localScale.y - amount.y);
    }

    private void Update()
    {
        if (animFuncX != null && animFuncY != null)
        {
            if (animFuncX.Finished(Time.time) && animFuncY.Finished(Time.time))
            {
                UpdateLocalScale(animFuncX.EndValue, animFuncY.EndValue);

                animFuncX = null;
                animFuncY = null;
            }
            else
            {
                UpdateLocalScale(animFuncX.GetValue(Time.time), animFuncY.GetValue(Time.time));
            }
        }
    }

    private void UpdateLocalScale(float newX, float newY)
    {
        newX = (newX < 0) ? 0 : newX;
        newY = (newY < 0) ? 0 : newY;

        transform.localScale = new Vector3(newX, newY, transform.localScale.z);
    }
}
