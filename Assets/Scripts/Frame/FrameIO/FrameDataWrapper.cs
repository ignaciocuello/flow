#if (UNITY_EDITOR)

using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class FrameDataWrapper : MonoBehaviour {

    private static readonly string FRAME_DATA_PATH = "Assets/FrameData";

    /* editor aid variables */
    [SerializeField]
    private bool play;
    [SerializeField]
    private int frameIndex;
    [SerializeField]
    private int numFrames;
    [SerializeField]
    private bool showPreviousFrame;

    [SerializeField]
    private HitData changeHitBoxHitData;
    /* hit boxes with the following id will have their hit data changed by the above, if this is empty
     * all hitboxes will have their hit data changed */
    [SerializeField]
    private string idToChange;

    /* the id that was previously filled with  values. if this is empty then none was filled, this is to prevent
     * us from overwrittinng what the user has typed into the 'changeHitBoxHitData' field */
    private string filledId;

    [SerializeField]
    private FrameData load;

    [Space(10)]

    [SerializeField]
    private GameObject frameWrapperPrefab;

    [Space(10)]

    [SerializeField]
    private Sprite[] spriteList;
    [SerializeField]
    private float playBackSpeed;

    [Space(10)]

    [SerializeField]
    private string relativePath;

    public virtual void Initialize(FrameData fdata, string relativePath)
    {
        foreach (Frame frame in fdata.Frames)
        {
            FrameWrapper fw = Instantiate(frameWrapperPrefab, transform).GetComponent<FrameWrapper>();
            fw.Initialize(frame);
        }

        spriteList = fdata.SpriteList;
        playBackSpeed = fdata.PlayBackSpeed;

        this.relativePath = relativePath;

        OnValidate();
    }

    void OnValidate()
    {
        if (spriteList != null && spriteList.Length > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = spriteList[i % spriteList.Length];
            }
        }
    }

    public void WriteObject()
    {
        FrameData frameData;
        string frameDataPath = Path.Combine(FRAME_DATA_PATH, relativePath + ".asset");
        if (File.Exists(frameDataPath))
        {
            frameData = AssetDatabase.LoadAssetAtPath<FrameData>(frameDataPath);
        }
        else
        {
            frameData = ScriptableObject.CreateInstance<FrameData>();
            AssetDatabase.CreateAsset(frameData, frameDataPath);
        }
        AssetDatabase.SaveAssets();

        frameData.SpriteList = spriteList;
        frameData.Frames = ConvertFrames();
        frameData.PlayBackSpeed = playBackSpeed;

        EditorUtility.SetDirty(frameData);
    }

    private Frame[] ConvertFrames()
    {
        Frame[] frames = new Frame[transform.childCount];

        //reason for using for loop instead of for each and GetComponentsInChildren is that
        //'GetComponentsInChildren' doesn't return when the component is inactive as will be
        //the case for many frame wrappers
        for (int i = 0; i < transform.childCount; i++)
        {
            frames[i] = transform.GetChild(i).GetComponent<FrameWrapper>().Convert();
        }

        return frames;
    }

    public void AppendFrame()
    {
        frameIndex = transform.childCount - 1;
        AppendFrameAtIndex();

        OnValidate();
    }

    public void AppendFrameAtIndex()
    {
        if (transform.childCount != 0)
        {
            FrameWrapper fw = Instantiate(transform.GetChild(frameIndex), transform).GetComponent<FrameWrapper>();
            for (int i = transform.childCount-2; i >= frameIndex + 1; i--)
            {
                transform.GetChild(i).transform.SetSiblingIndex(i + 1);
            }
            fw.transform.SetSiblingIndex(frameIndex + 1);
            frameIndex = frameIndex + 1;
        }
        else
        {
            Instantiate(frameWrapperPrefab, transform);
            frameIndex = 0;
        }

        OnValidate();
    }

    void Update()
    {
        numFrames = transform.childCount;
        if (transform.childCount != 0)
        {
            FrameWrapper prev = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                FrameWrapper fw = transform.GetChild(i).GetComponent<FrameWrapper>();
                if (i == frameIndex)
                {
                    fw.gameObject.SetActive(true);
                    fw.SetAlpha(0.25f);
                    if (showPreviousFrame && i > 0)
                    {
                        prev.gameObject.SetActive(true);
                        prev.SetAlpha(0.1f);
                    }
                }
                else
                {
                    fw.gameObject.SetActive(false);
                }
                prev = fw;
            }
        }

        if (!idToChange.Equals(filledId))
        {
            HitBoxWrapper hbw = null;

            bool filledFields = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                hbw = transform.GetChild(i).GetComponent<FrameWrapper>().GetFirstHitBoxThatMatches(idToChange);
                if (hbw != null)
                {
                    changeHitBoxHitData = hbw.HitData;
                    filledFields = true;
                    break;
                }
            }

            if (filledFields)
            {
                filledId = idToChange;
            }
        }
    }

    void FixedUpdate()
    {
        if (play)
        {
            frameIndex = (frameIndex + 1) % transform.childCount;
        }
    }

    public void ChangeAllFramesHitBoxHitData()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<FrameWrapper>().ChangeAllHitBoxHitData(changeHitBoxHitData, idToChange);
        }
    }

    public void LoadFrameData()
    {
        string fullPath = AssetDatabase.GetAssetPath(load);
        fullPath = fullPath.Replace(".asset", "").Replace(FRAME_DATA_PATH, "").TrimStart('/');

        Initialize(load, fullPath);
    }
}

[CustomEditor(typeof(FrameDataWrapper), true)]
public class FrameDataWrapperEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FrameDataWrapper fdw = (FrameDataWrapper)target;
        if (GUILayout.Button("Write Frame Data"))
        {
            fdw.WriteObject();
        }
        if (GUILayout.Button("Append Frame"))
        {
            fdw.AppendFrame();
        }
        if (GUILayout.Button("Append Frame At Index"))
        {
            fdw.AppendFrameAtIndex();
        }
        if (GUILayout.Button("Change HitBox HitData"))
        {
            fdw.ChangeAllFramesHitBoxHitData();
        }
        if (GUILayout.Button("Load Frame Data"))
        {
            fdw.LoadFrameData();
        }
    }
}

#endif