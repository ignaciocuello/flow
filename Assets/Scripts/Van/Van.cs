using System.Collections;
using UnityEngine;

public class Van : MonoBehaviour {

    /* wheel properties when driving */
    [SerializeField]
    private float drivingWheelDampingRatio;
    [SerializeField]
    private float drivingWheelFrequency;

    [SerializeField, Tooltip("acceleration in degrees per second^2")]
    private float acceleration;

    private WheelJoint2D[] wheels;

    private RealisticEngineSound res;
    private CarSimulator carSimulator;

    private int gasPresses;
    private bool useSimulator = true;

    public float TargetSpeed;

    public float ArtificalMaxRPM = Mathf.Infinity;

    private void Awake()
    {
        res = GetComponent<RealisticEngineSound>();
        carSimulator = GetComponent<CarSimulator>();

        wheels = GetComponentsInChildren<WheelJoint2D>();

        if (EndSceneSequenceManager.Instance != null)
        {
            EndSceneSequenceManager.Instance.Van = this;
        }
    }

	public void ReadyWheelsForDrive()
    {
        foreach (WheelJoint2D wheel in wheels)
        {
            JointSuspension2D suspension = wheel.suspension;
            suspension.dampingRatio = drivingWheelDampingRatio;
            suspension.frequency = drivingWheelFrequency;

            wheel.suspension = suspension;
        }
    }

    public void Update()
    {
        if (useSimulator)
        {
            res.engineCurrentRPM = Mathf.Min(carSimulator.rpm, ArtificalMaxRPM);
            res.gasPedalPressing = carSimulator.gasPedalPressing;
        }
    }


    public void Gas(float duration)
    {
        StartCoroutine(GasCoroutine(duration));
    }

    IEnumerator GasCoroutine(float duration)
    {
        carSimulator.gasPedalPressing = true;
        gasPresses++;
        yield return new WaitForSeconds(duration);
        gasPresses--;
        if (gasPresses == 0)
        {
            carSimulator.gasPedalPressing = false;
        }
    }

    public void ManualSetRPM(float rpm, bool gasPedalDown)
    {
        useSimulator = false;
        res.gasPedalPressing = gasPedalDown;
        res.engineCurrentRPM = rpm;
    }

    public void GasUntil(float targetRPM)
    {
        StartCoroutine(GasUntilCoroutine(targetRPM));
    }

    IEnumerator GasUntilCoroutine(float targetRPM)
    {
        if (res.maxRPMLimit < targetRPM)
        {
            res.maxRPMLimit = targetRPM + 1;
            carSimulator.maxRPM = res.maxRPMLimit;
        }

        carSimulator.gasPedalPressing = true;
        gasPresses++;
        while (res.engineCurrentRPM < targetRPM)
        {
            yield return null;
        }

        res.engineCurrentRPM = targetRPM;
        res.gasPedalPressing = true;
        gasPresses--;
        if (gasPresses == 0)
        {
            carSimulator.gasPedalPressing = false;
        }

        useSimulator = false;
    }
    

    private void FixedUpdate()
    {
        JointMotor2D motor = wheels[0].motor;

        //if speed is greater than target, by a margin large enough that decelerating won't  overshoot to a velocity too low
        if (motor.motorSpeed >= TargetSpeed + acceleration * Time.fixedDeltaTime)
        {
            motor.motorSpeed -= acceleration * Time.fixedDeltaTime;
        }
        //if speed is lower than target, by a margin large enough that accelerating won't  overshoot to a velocity too high
        else if (motor.motorSpeed <= TargetSpeed - acceleration * Time.fixedDeltaTime)
        {
            motor.motorSpeed += acceleration * Time.fixedDeltaTime;
        }
        //if speed is close enough to target, just set it to target speed
        else
        {
            motor.motorSpeed = TargetSpeed;
        }

        foreach (WheelJoint2D wheel in wheels)
        {
            wheel.motor = motor;
        }
    }
}
