using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator
{
    private Image image;
    private List<Sprite> frames;
    private float framesPerSecond;

    private int currentFrame;
    private float startTime;

    public float FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }
    public List<Sprite> Frames => frames;

    public SpriteAnimator(Image image, List<Sprite> frames, float framesPerSecond = 8f)
    {
        this.image = image;
        this.frames = frames;
        this.framesPerSecond = framesPerSecond;
    }

    public void Start()
    {
        currentFrame = 0;
        startTime = Time.time;
        if (frames != null && frames.Count > 0)
        {
            image.sprite = frames[0];
        }
    }

    public void HandleUpdate()
    {
        if (frames == null || frames.Count <= 1) return;
        if (framesPerSecond <= 0f) return;

        int frame = (int)((Time.time - startTime) * framesPerSecond) % frames.Count;
        if (frame != currentFrame)
        {
            currentFrame = frame;
            image.sprite = frames[currentFrame];
        }
    }
}
