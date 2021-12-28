using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizerMaster : MonoBehaviour
{
    public List<RectTransform> transforms;

    private AudioSource source;

    private Vector2 basePos;

    private float delay = 0.02f;
    private int sampleCount = 205;

    private float clock;

    private float volume;
    private float[] data;

    private float targetScale;
    private float scaleChangeSpeed = 1.25f;
    private float maxVolume = 0.4f;

    private List<Vector2> basePositions;

    void Start()
    {
        source = TaleUtil.Props.audio.music;

        basePositions = new List<Vector2>();

        for(int i = 0; i < transforms.Count; ++i)
        {
            basePositions.Add(transforms[i].anchoredPosition);
            transforms[i].localScale = new Vector2(transforms[i].localScale.x, 0f);
        }

        clock = 0f;
        targetScale = 0f;

        data = new float[sampleCount];
    }

    void Update()
    {
        if (source.clip == null)
            return;

        clock += Time.deltaTime;

        if(clock >= delay)
        {
            clock = 0f;
            source.clip.GetData(data, source.timeSamples);

            volume = 0f;

            foreach(var sample in data)
            {
                volume += Mathf.Abs(sample);
            }

            volume /= sampleCount;

            if (volume > maxVolume)
                volume = maxVolume;

            targetScale = MathSET.Map(volume, 0f, maxVolume, 0f, 1f);
        }

        for(int i = 0; i < transforms.Count; ++i)
        {
            RectTransform transform = transforms[i];
            Vector2 basePos = basePositions[i];

            float diff = (targetScale - transform.localScale.y);
            float delta = (diff < 0 ? -1 : 1) * Mathf.Min(scaleChangeSpeed * Time.deltaTime, Mathf.Abs(diff));

            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + delta);
            transform.anchoredPosition = new Vector2(basePos.x, basePos.y + (transform.rect.height * transform.localScale.y) / 2);
        }
    }
}
