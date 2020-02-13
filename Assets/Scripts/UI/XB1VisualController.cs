using UnityEngine;
using UnityEngine.UI;

public class XB1VisualController : MonoBehaviour {

    [SerializeField]
    private Image leftStickBg;
    [SerializeField]
    private Image leftStick;
    [SerializeField]
    private Image rightStickBg;
    [SerializeField]
    private Image rightStick;

    [Space(10)]

    [SerializeField]
    private Image bButton;
    [SerializeField]
    private Image xButton;
    [SerializeField]
    private Image yButton;
    [SerializeField]
    private Image aButton;

    [Space(10)]

    [SerializeField]
    private Image leftTrigger;

    private void Awake()
    {
        leftTrigger.material = new Material(leftTrigger.material);
    }

    private void FixedUpdate()
    {
        float leftXPos = InputBuffer.Instance.GetAxis("Horizontal") * 0.5f;
        float leftYPos = InputBuffer.Instance.GetAxis("Vertical") * 0.5f;

        bool leftEnable = (leftXPos != 0.0f || leftYPos != 0.0f);
        leftStickBg.enabled = leftEnable;
        leftStick.enabled = leftEnable;

        leftStick.transform.localPosition = new Vector2(leftXPos, leftYPos);

        bool rightEnable = false;
        rightStickBg.enabled = rightEnable;
        rightStick.enabled = rightEnable;

        bButton.enabled = InputBuffer.Instance.GetButton("Attack");
        xButton.enabled = false;
        yButton.enabled = false;//InputBuffer.Instance.GetButton("Up Attack");
        aButton.enabled = InputBuffer.Instance.GetButton("Jump");

        float leftTriggerValue = InputBuffer.Instance.GetAxis("Perch");

        Color c = leftTrigger.material.color;
        leftTrigger.material.color = new Color(c.r, c.g, c.b, leftTriggerValue);
    }
}
