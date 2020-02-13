using RenderHeads.Media.AVProMovieCapture;
using UnityEngine;

public class FinalizeCapture : MonoBehaviour {

    private CaptureFromScreen capture;

    private void Awake()
    {
        capture = GetComponent<CaptureFromScreen>();
    }

    private void OnDisable()
    {
        capture.StopCapture();
    }
}
