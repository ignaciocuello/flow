using UnityEngine;

public class DoorContraption : MonoBehaviour {

    private static readonly Vector2 DEFAULT_REWARD_RELATIVE_SPAWN_POINT = new Vector2(0.0f, -0.25f);

    [SerializeField]
    protected Door door;
    [SerializeField]
    protected Packet packet;

    [SerializeField]
    /* amount to scale time when goal completed, usually < 1 */
    private float slowDownFactor;
    [SerializeField]
    /* amount to keep time slowed down in seconds */
    private float slowDownDuration;
    /* how sharply the time should normalize */
    [SerializeField]
    private float exponentialGrowthFactor;

    /* true if door has already slowed down time, only want to slow down once */
    private bool slowedDownOnce;

    private bool advancingPacket;

    [SerializeField]
    private float packetTravelTime;
    private float packetAdvanceDist;

    /* open door and advance level */
    public void OpenAndSlowDown()
    {
        Open();

        if (!slowedDownOnce)
        {
            Level currentLevel = GetComponentInParent<Level>();
            StatsTracker.Instance.TimeTracker.UpdateMeasurementIfExists(currentLevel.ToString());
            //currentLevel.LevelFinished(
            ///    door.transform.TransformPoint(DEFAULT_REWARD_RELATIVE_SPAWN_POINT));

            //SHOULD LOG HERE THAT PLAYER HAS BEATEN THE LEVEL
            TimeManager.Instance.SetSlowDownFactor(slowDownFactor, slowDownDuration, exponentialGrowthFactor);

            slowedDownOnce = true;
        }
    }

    private void AdvancePacket()
    {
        advancingPacket = true;

        packet.EnableLight();
        packet.StartTrajectory();

        packetAdvanceDist = packet.GetTrajectoryDistance() / packetTravelTime;
    }

    void FixedUpdate()
    {
        if (advancingPacket)
        {
            if (packet.Advance(packetAdvanceDist * Time.fixedDeltaTime))
            {
                door.Open();
                advancingPacket = false;
                packet.DisableLight();
            }
        }
    }

    public void Open()
    {
        AdvancePacket();
    }

    public void Close()
    {
        advancingPacket = false;
        packet.DisableLight();

        door.Close();
    }
}
