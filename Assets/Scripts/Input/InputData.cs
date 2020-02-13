public class InputData {

    public float AxisValue;

    /* true if the input is being held */
    public bool Held;
    /* true if the input was pressed during this frame */
    public bool Down;
    /* ture if this is the last frame the input was held */
    public bool Up;

    public InputData(float axisValue, bool held, bool down, bool up)
    {
        AxisValue = axisValue;

        Held = held;
        Down = down;
        Up = up;
    }
}
