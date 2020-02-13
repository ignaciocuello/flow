using System.Collections.Generic;
using UnityEngine;

public class Packet : MonoBehaviour {

    [SerializeField]
    private LineRenderer connection;

    private List<Vector3> checkPoints;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        DisableLight();
    }

    public void StartTrajectory()
    {
        Vector3[] positions = new Vector3[connection.positionCount];
        connection.GetPositions(positions);

        checkPoints = new List<Vector3>();
        checkPoints.AddRange(positions);

        transform.localPosition = new Vector3(checkPoints[0].x, checkPoints[0].y, transform.localPosition.z);
    }

    public float GetTrajectoryDistance()
    {
        float distance = 0.0f;
        for (int i = 0; i < checkPoints.Count - 1; i++)
        {
            distance += Vector2.Distance(checkPoints[i], checkPoints[i + 1]);
        }

        return distance;
    }

    /* return true if destination reached */
    public bool Advance(float distance)
    {
        if (checkPoints.Count == 0)
        {
            return true;
        }

        //next check point
        float advancedDistance = 0.0f;

        while (advancedDistance < distance)
        {
            Vector2 cp = checkPoints[0];

            Vector2 oldPos = transform.localPosition;
            Vector2 newPos = Vector2.MoveTowards(transform.localPosition, cp, distance);

            advancedDistance += Vector2.Distance(oldPos, newPos);

            transform.localPosition = new Vector3(newPos.x, newPos.y, transform.localPosition.z);

            if (newPos == cp)
            {
                checkPoints.RemoveAt(0);
                if (checkPoints.Count == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void EnableLight()
    {
        spriteRenderer.enabled = true;
    }

    public void DisableLight()
    {
        spriteRenderer.enabled = false;
    }
}
