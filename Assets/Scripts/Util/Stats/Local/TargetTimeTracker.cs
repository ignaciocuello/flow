using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TargetTimeTracker : MonoBehaviour {

    private const string FILE_NAME = "TargetTimes";

    /* the targets that are being tracked */
    [SerializeField]
    private List<Target> targets;

    //do in start to make sure everything has been initialized
    private void Start()
    {
        //start measuring time for all targets (as they can be done in none deterministic order)
        foreach (Target target in targets)
        {
            StatsTracker.Instance.TimeTracker.StartMeasuring(target.ToString());

            target.TargetEntered.AddListener(OnTargetEntered);
        }
    }

	public void OnTargetEntered(Target target)
    {
        //if target in list (this check is semi-redundant, but better to be safe)
        if (targets.Contains(target))
        {
            //update time to see how long it took user to complete that target
            StatsTracker.Instance.TimeTracker.UpdateMeasurement(target.ToString());

            //remove listener and remove target from list, we only care about it until its
            //been entered once, subsequent triggers of the event are irrelevant
            target.TargetEntered.RemoveListener(OnTargetEntered);
            targets.Remove(target);

            //reset all other targets timers
            ResetTimers();
        }
    }

    private void ResetTimers()
    {
        foreach (Target target in targets)
        {
            StatsTracker.Instance.TimeTracker.StartMeasuring(target.ToString());
        }
    }

    public void Write()
    {
        StatsTracker.Instance.TimeTracker.Write(
            UniqueFileNameFinder.Find(StatsTracker.Instance.TimeTracker.GetPathFromName, FILE_NAME, ".csv"));
    }

    public void OnDestroy()
    {
        Write();
    }
}
