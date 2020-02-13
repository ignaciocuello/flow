using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HitAttemptStats {

    /* the result type of this hit attempt, either early, late, success or other(e.g. spacing error) */
    public ResultType ResultType;

    /* subtract frame delta to the current frame, and at that frame if an attack
     * was initiated it would have hit. negative if early to hit, positive if late to hit, 
     * 0 if actualy hit (Success). Below are all different plausible frame deltas (as we
     * can't gather player intention from this data) */

    /* set of negative frame deltas */
    private HashSet<int> negativeHitFrameDeltaSet;
    /*set of positive frame deltas */
    private HashSet<int> positiveHitFrameDeltaSet;
    /* list of negative frame deltas (for display in editor) */
    public List<int> NegativeFrameDeltas;
    /* list of positive frame deltas (for display in editor) */
    public List<int> PositiveFrameDeltas;

    /* frame deltas corresponding to what we've decided is the canonical result of the hit attempt
     * so containing only early or late deltas */
    private List<int> canonicalFrameDeltas;

    /* minimum absolute value for canonical frame delta, this is the most optimistic case 
     * for earliness/lateness*/
    public int MinHitFrameDelta;
    /* maximum absolute value for canonical frame delta, this is the most pessimistic case */
    public int MaxHitFrameDelta;
    /* average value for canonical frame delta, taken over all 'validInitFrames', i.e. over all 
     * frames where having inititated the attack would have resulted in a hit */
    public float AverageHitFrameDelta;

    public string AttackName;

    public HitAttemptStats(string attackName)
    {
        negativeHitFrameDeltaSet = new HashSet<int>();
        positiveHitFrameDeltaSet = new HashSet<int>();

        NegativeFrameDeltas = new List<int>();
        PositiveFrameDeltas = new List<int>();

        AttackName = attackName;
        ResultType = ResultType.OTHER;
    }

    public void AddToHitFrameDeltaSet(int frameDelta)
    {
        HashSet<int> hitFrameDeltaSet;
        List<int> hitFrameDeltas;
        if (frameDelta < 0)
        {
            hitFrameDeltaSet = negativeHitFrameDeltaSet;
            hitFrameDeltas = NegativeFrameDeltas;
        }
        else
        {
            hitFrameDeltaSet = positiveHitFrameDeltaSet;
            hitFrameDeltas = PositiveFrameDeltas;
        }

        hitFrameDeltaSet.Add(frameDelta);

        hitFrameDeltas.Clear();
        hitFrameDeltas.AddRange(hitFrameDeltaSet);
        hitFrameDeltas.Sort();

        DecideCanonicalResultType();
    }

    private void DecideCanonicalResultType()
    {
        bool late = false;
        if (PositiveFrameDeltas.Count != 0 && NegativeFrameDeltas.Count != 0)
        {
            //smallest pos (list is sorted)
            int posDif = PositiveFrameDeltas[0];
            //largets negative (list is sorted), negate to make positive
            int negDif = -NegativeFrameDeltas[NegativeFrameDeltas.Count - 1];

            late = (posDif <= negDif);
        }
        else if (PositiveFrameDeltas.Count != 0)
        {
            late = true;   
        }
        else if (NegativeFrameDeltas.Count != 0)
        {
            late = false;
        }
        else
        {
            return;
        }

        if (late)
        {
            canonicalFrameDeltas = PositiveFrameDeltas;
            ResultType = ResultType.LATE;
        }
        else
        {
            canonicalFrameDeltas = NegativeFrameDeltas;
            ResultType = ResultType.EARLY;
        }

        CalculateMinMaxAvgStats();
    }

    private void CalculateMinMaxAvgStats()
    {
        int min = int.MaxValue;
        int max = 0;

        int avg = 0;

        foreach (int frameDelta in canonicalFrameDeltas)
        {
            if (Mathf.Abs(frameDelta) < Mathf.Abs(min))
            {
                min = frameDelta;
            }
            if (Mathf.Abs(frameDelta) > Mathf.Abs(max))
            {
                max = frameDelta;
            }

            avg += frameDelta;
        }
        avg /= canonicalFrameDeltas.Count;

        MinHitFrameDelta = min;
        MaxHitFrameDelta = max;
        AverageHitFrameDelta = avg;
    }

    public string ToCSVString()
    {
        return string.Format("{0}, {1}, {2}, {3}, {4}\n",
            ResultType.ToString(), MinHitFrameDelta.ToString(), MaxHitFrameDelta.ToString(),
            AverageHitFrameDelta.ToString(), AttackName);
    }

    public static string GetCSVHeader()
    {
        return "Result, MinAbsDelta, MaxAbsDelta, AvgDelta, Attack\n";
    }
}

public enum ResultType
{
    EARLY, LATE, SUCCESS, OTHER
}
