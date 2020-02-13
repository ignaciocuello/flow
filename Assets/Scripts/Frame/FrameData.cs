using System.Collections.Generic;
using UnityEngine;

public class FrameData : ScriptableObject
{
    /* sprites to cycle through for frames */
    public Sprite[] SpriteList;

    /* sprites to cycle through for frames */
    public LayeredSprite[] Sprites;

    /* data of hurt/hit boxes */
    public Frame[] Frames;

    /* the speed to play back the frames/sprite animation 1.0 = 60 fps, 0.5 = 30 fps and so on */
    public float PlayBackSpeed;

}
