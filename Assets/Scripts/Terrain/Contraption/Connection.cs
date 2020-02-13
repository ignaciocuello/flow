using System;
using UnityEngine;

[ExecuteInEditMode]
public class Connection : MonoBehaviour {

    [SerializeField]
    private Device device;

    /* location in target where point originates from */
    [SerializeField]
    private Vector2 relativeStart;
    /* location in device where point ends */
    [SerializeField]
    private Vector2 relativeEnd;

    /* the transform of the target */
    [SerializeField]
    private Transform targetTransform;

    /* whether packet is advancing or not */
    private bool advancingPacket;

    [SerializeField]
    private float packetTravelTime;
    private float packetAdvanceDist;

    /* scale of time for connection */
    public float TimeScale = 1.0f;

    private LineRenderer lineRenderer;

    [SerializeField]
    private Packet packet;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void AdvancePacket()
    {
        //reset time scale to 1 just in case it has been changed
        TimeScale = 1.0f;

        device.PreHeat(this);

        advancingPacket = true;

        packet.EnableLight();
        packet.StartTrajectory();

        packetAdvanceDist = packet.GetTrajectoryDistance() / packetTravelTime;
    }

    public void StopPacket()
    {
        advancingPacket = false;
        packet.DisableLight();

        device.Deactivate();
    }

    void FixedUpdate()
    {
        if (advancingPacket)
        {
            bool destinationReached = packet.Advance(packetAdvanceDist * Time.fixedDeltaTime);

            if (destinationReached)
            {
                device.Activate();

                advancingPacket = false;
                packet.DisableLight();
            }
        }
    }

    void Update()
    {
        lineRenderer.SetPosition(0, relativeStart);
        Vector2 pos = lineRenderer.GetPosition(1);

        if (relativeStart.x == 0.0f)
        {
            lineRenderer.SetPosition(1, new Vector2(0.0f, pos.y));
        }
        else if (relativeStart.y == 0.0f)
        {
            lineRenderer.SetPosition(1, new Vector2(pos.x, 0.0f));
        }

        if (device != null)
        {
            Vector2 worldRelativeEnd = device.gameObject.transform.position + Vector3.Scale(relativeEnd, device.GetComponent<BoxCollider2D>().size);
            Vector2 localEnd = transform.InverseTransformPoint(worldRelativeEnd);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, localEnd);

            pos = lineRenderer.GetPosition(lineRenderer.positionCount - 2);
            if (relativeEnd.x == 0.0f)
            {
                lineRenderer.SetPosition(lineRenderer.positionCount - 2, new Vector2(localEnd.x, pos.y));
            }
            else if (relativeEnd.y == 0.0f)
            {
                lineRenderer.SetPosition(lineRenderer.positionCount - 2, new Vector2(pos.x, localEnd.y));
            }
        }
    }
}
