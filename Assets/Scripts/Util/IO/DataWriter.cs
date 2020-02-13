using RenderHeads.Media.AVProMovieCapture;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataWriter : MonoBehaviour {

    /* whether data should be written to file or not */
    [SerializeField]
    private bool writeData;

    public void OnValidate()
    {
        if (Camera.main != null)
        {
            Camera.main.GetComponent<CaptureFromScreen>().enabled = writeData;
        }
    }
	
	public bool WriteAllText(string path, string text)
    {
        if (writeData)
        {
            File.WriteAllText(path, text);
            return true;
        }

        return false;
    }
}
