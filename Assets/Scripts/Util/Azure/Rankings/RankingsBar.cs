using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingsBar : MonoBehaviour {

    private const float EPS = 0.01f;

    [SerializeField]
    private GameObject rankingsTextPrefab;
    private RankingsText rankingsText;

    [SerializeField, Tooltip("how long before targetX the van should start breaking")]
    private float breakingDistance;
    [SerializeField, Tooltip("velocity van wheels should target when distance to target is breaking distance lerps to endBreakRotationalSpeed when distance is 0")]
    private float startBreakRotationalSpeed;
    [SerializeField]
    private float endBreakRotationalSpeed;

    [SerializeField]
    private float startRPM;
    [SerializeField]
    private float endRPM;

    [SerializeField]
    private Vector3 targetCameraPos;
    [SerializeField]
    private float targetCameraSize;
    [SerializeField]
    private float horizontalSpeed;
    [SerializeField]
    private float sizeSpeed;

    [SerializeField]
    private GameObject parkingWall;
    [SerializeField]
    private float parkingWallXOffset;

    private float x0;
    private float x1;

    private float targetX;
    public float TargetRanking
    {
        set
        {
            targetX = x0 + RankingToSliderValue(value, ScoreManager.Instance.TotalRankingsCount) * (x1-x0);

            if (!parkingWall.activeInHierarchy)
            {
                parkingWall.SetActive(true);
            }
            parkingWall.transform.position =
                    new Vector3(targetX + parkingWallXOffset, parkingWall.transform.position.y, parkingWall.transform.position.z);

            parkingWall.GetComponent<ParkingWall>().OnVanCollision += (van) =>
            {
                van.TargetSpeed = 0.0f;
                rankingsText.Shing();
            };
        }
    }

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        x0 = transform.position.x + boxCollider.offset.x - boxCollider.size.x / 2.0f;
        x1 = transform.position.x + boxCollider.offset.x + boxCollider.size.x / 2.0f;
    }

    private float RankingToSliderValue(float ranking, float totalRankings)
    {
        return (ranking - totalRankings) / (1.0f - totalRankings);
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Van"))
        {
            if (targetX == 0)
            {
                if (EndSceneSequenceManager.Instance.UseDriveSequence)
                {
                    TransformTracker.Instance.TargetSize = targetCameraSize;
                    TransformTracker.Instance.SizeTrackingSpeed = sizeSpeed;
                }

                TargetRanking = ScoreManager.Instance.PlayerRanking;
            }

            Van van = collider.GetComponentInParent<Van>();
            Vector3 vanPosition = van.GetComponentInChildren<VanBody>().transform.position;

            float ratio = Mathf.Abs((vanPosition.x - x0)/(x1-x0));

            //this makes slider only go up
            slider.value = Mathf.Max(ratio, slider.value);

            if (rankingsText == null)
            {
                rankingsText = UserInterface.Instance.InstantiatePrefab(rankingsTextPrefab, overlay: true).GetComponent<RankingsText>();
                if (!EndSceneSequenceManager.Instance.UseDriveSequence)
                {
                    rankingsText.gameObject.SetActive(false);
                }
            }

            Vector3 vanScreenPos = Camera.main.WorldToScreenPoint(vanPosition);
            RectTransform rectTransform = rankingsText.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = new Vector2(vanScreenPos.x - UserInterface.Instance.OverlayCanvas.anchoredPosition.x, rectTransform.anchoredPosition.y);
            rankingsText.Ranking = Mathf.Lerp(ScoreManager.Instance.TotalRankingsCount, 1.0f, slider.value);

            if (WithtinBreakingDistance(vanPosition.x))
            {
                float distancePastBreakingStartPoint = vanPosition.x - (targetX - breakingDistance);
                float t = distancePastBreakingStartPoint / breakingDistance;

                float rpm = Mathf.Lerp(startRPM, endRPM, t);
                van.ManualSetRPM(rpm, gasPedalDown: Mathf.Abs(rpm - endRPM) > EPS);
                
                float targetRotationSpeed = Mathf.Lerp(startBreakRotationalSpeed, endBreakRotationalSpeed, t);

                van.TargetSpeed = targetRotationSpeed;
            }
        }
    }

    private bool WithtinBreakingDistance(float x)
    {
        float dif = targetX - x;
        return dif > 0 && dif < breakingDistance;
    }
}
