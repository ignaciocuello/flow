using Rewired;
using System.Collections.Generic;

public class PlayerAtomicAction
{
    public static readonly PlayerAtomicAction HORIZONTAL = 
        new PlayerAtomicAction("Horizontal", new List<string>() { "Backwards", "Forwards" });
    public static readonly PlayerAtomicAction VERTICAL = 
        new PlayerAtomicAction("Vertical", new List<string>() { "Up", "Down" });
    public static readonly PlayerAtomicAction DOWN = new PlayerAtomicAction("Vertical", new List<string>() { "Down" });

    public static readonly PlayerAtomicAction ATTACK = new PlayerAtomicAction("Attack");
    public static readonly PlayerAtomicAction JUMP = new PlayerAtomicAction("Jump");

    public static readonly PlayerAtomicAction RECALL = new PlayerAtomicAction("Recall");

    /* name used to find input action e.g. Horizontal */
    public string Name;
    /* more specific names, e.g. Left, Right */
    private List<string> descriptiveNames;

    public PlayerAtomicAction(string name) : this(name, new List<string>() { name })
    {
    }

    public PlayerAtomicAction(string name, List<string> descriptiveNames)
    {
        Name = name;
        this.descriptiveNames = descriptiveNames;
    }

    public bool Matches(ActionElementMap actionMap)
    {
        return descriptiveNames.Contains(actionMap.actionDescriptiveName);   
    }
    
}