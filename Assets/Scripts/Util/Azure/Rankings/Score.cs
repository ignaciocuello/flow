using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : IComparable<Score> {

    private const int GREATER_THAN = 1;
    private const int LESS_THAN = -1;
    private const int EQUAL_TO = 0;

    public readonly int NumBloodCoins;
	public readonly int NumWads;
    public readonly int NumCoins;

    public Score(int numBloodCoins, int numWads, int numCoins)
    {
        NumBloodCoins = numBloodCoins;
        NumWads = numWads;
        NumCoins = numCoins;
    }

    public int CompareTo(Score other)
    {
        if (ReferenceEquals(other, null))
        {
            return GREATER_THAN;
        }
        if (NumWads == other.NumWads)
        {
            return NumCoins.CompareTo(other.NumCoins);
        }
        else if (NumWads > other.NumWads)
        {
            return GREATER_THAN;
        }
        else
        {
            return LESS_THAN;
        }
    }

    public override string ToString()
    {
        return string.Format("[NumBloodCoins={0}, NumWads={1}, NumCoins={2}]",
            NumBloodCoins, NumWads, NumCoins);
    }

    public override bool Equals(object obj)
    {
        var score = obj as Score;
        return score != null &&
               NumBloodCoins == score.NumBloodCoins &&
               NumWads == score.NumWads &&
               NumCoins == score.NumCoins;
    }

    public override int GetHashCode()
    {
        var hashCode = 404442194;
        hashCode = hashCode * -1521134295 + NumBloodCoins.GetHashCode();
        hashCode = hashCode * -1521134295 + NumWads.GetHashCode();
        hashCode = hashCode * -1521134295 + NumCoins.GetHashCode();
        return hashCode;
    }

    public static bool operator <(Score s1, Score s2)
    {
        return s1.CompareTo(s2) < 0;
    }

    public static bool operator >(Score s1, Score s2)
    {
        return s1.CompareTo(s2) > 0;
    }

    public static bool operator == (Score s1, Score s2)
    {
        if (ReferenceEquals(s1, s2))
        {
            return true;
        }
        if (ReferenceEquals(s1, null))
        {
            return false;
        }
        if (ReferenceEquals(s2, null))
        {
            return false;
        }

        return s1.CompareTo(s2) == 0;
    }

    public static bool operator != (Score s1, Score s2)
    {
        return s1.CompareTo(s2) != 0;
    }
}
