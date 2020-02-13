public class FocusGenereatorDoorContraption : DoorContraption {

    private FocusGenerator focusGenerator;

    public void Awake()
    {
        focusGenerator = GetComponentInChildren<FocusGenerator>();
    }

	public void Deactivate()
    {
        focusGenerator.Deactivate();

        Close();
    }
}
