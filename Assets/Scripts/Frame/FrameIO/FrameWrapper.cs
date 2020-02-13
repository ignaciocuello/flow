#if (UNITY_EDITOR) 

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FrameWrapper : MonoBehaviour {

    private static Dictionary<string, Func<GameObject, BoxWrapper>> boxTypeComponentMap;

    [SerializeField]
    private GameObject boxWrapperPrefab;

    static FrameWrapper()
    {
        InitBoxTypeComponentMap();
    }

    static void InitBoxTypeComponentMap()
    {
        boxTypeComponentMap = new Dictionary<string, Func<GameObject, BoxWrapper>>();
        boxTypeComponentMap["HurtBox"] = obj => obj.AddComponent<HurtBoxWrapper>();
        boxTypeComponentMap["HitBox"] = obj => obj.AddComponent<HitBoxWrapper>();
    }

    public void Initialize(Frame frame)
    {
        foreach (Box box in frame.Boxes)
        {
            GameObject boxObj = Instantiate(boxWrapperPrefab, transform);
            BoxWrapper boxWrapper = boxTypeComponentMap[box.GetType().Name](boxObj);

            boxWrapper.Initialize(box);
        }
    }

    public Frame Convert()
    {
        Frame frame = new Frame();

        List<HurtBox> hurtBoxes = new List<HurtBox>();
        foreach (HurtBoxWrapper hurtWrapper in GetComponentsInChildren<HurtBoxWrapper>())
        {
            hurtBoxes.Add((HurtBox)hurtWrapper.Convert());
        }
        List<HitBox> hitBoxes = new List<HitBox>();
        foreach (HitBoxWrapper hitWrapper in GetComponentsInChildren<HitBoxWrapper>())
        {
            hitBoxes.Add((HitBox)hitWrapper.Convert());
        }

        frame.HurtBoxes = hurtBoxes.ToArray();
        frame.HitBoxes = hitBoxes.ToArray();

        return frame;
    }

    public void AppendBox()
    {
        if (transform.childCount != 0)
        {
            Instantiate(transform.GetChild(transform.childCount - 1), transform);
        }
        else
        {
            Instantiate(boxWrapperPrefab, transform);
        }
    }

    public void SetAlpha(float alpha)
    {
        SetSpriteRendererAlpha(GetComponent<SpriteRenderer>(), alpha);

        for (int i = 0; i < transform.childCount; i++)
        {
            BoxWrapper bw = transform.GetChild(i).GetComponent<BoxWrapper>();
            if (bw != null) {
                SetSpriteRendererAlpha(bw.GetComponent<SpriteRenderer>(), alpha);
            }
        }
    }

    private void SetSpriteRendererAlpha(SpriteRenderer sr, float alpha)
    {
        Color c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, alpha);
    }

    public void ChangeAllHitBoxHitData(HitData hitData, string idToChange)
    {
        HitBoxWrapper hbw;
        for (int i = 0; i < transform.childCount; i++)
        {
            if ((hbw = transform.GetChild(i).GetComponent<HitBoxWrapper>()) != null && 
                (idToChange == "" || idToChange == hbw.Id))
            {
                hbw.HitData = hitData;
            }
        }
    }

    //returns first hit box if id = "", otherwise first hit box that matches id
    public HitBoxWrapper GetFirstHitBoxThatMatches(string id)
    {
        HitBoxWrapper hbw;
        for (int i = 0; i < transform.childCount; i++)
        {
            if ((hbw = transform.GetChild(i).GetComponent<HitBoxWrapper>()) != null && 
                (id == "" || id == hbw.Id))
            {
                return hbw;
            }
        }

        return null;
    } 
}

[CustomEditor(typeof(FrameWrapper))]
public class FrameWrapperEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FrameWrapper fw = (FrameWrapper)target;
        if (GUILayout.Button("Append Box"))
        {
            fw.AppendBox();
        }
    }
}

#endif