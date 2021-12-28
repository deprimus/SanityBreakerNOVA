using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaBackgroundRainbow : MonoBehaviour
{
    public float speed = 0.1f;
    public float minHue = 0.5f;
    public float maxHue = 0.7f;
    public float minBrightness = 0.35f;
    public float maxBrightness = 0.5f;

    public Color lastColor;

    public float clock;
    private new SpriteRenderer renderer;

    void Awake()
    {
        clock = 0f;

        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float hue = 0f;
        float brightness = 0f;

        float actualMinBrightness = minBrightness;
        float actualMaxBrightness = maxBrightness;
        float actualMinHue = minHue;
        float actualMaxHue = maxHue;

        float brickRatio = ((float) NovaGame.brickMaster.bricks.Count) / NovaGame.brickMaster.GetMaxBrickCount();

        if (brickRatio <= 0.5f)
        {
            actualMinBrightness = MathSET.Map(brickRatio, 0.5f, 0f, minBrightness, 1f);
            actualMaxBrightness = MathSET.Map(brickRatio, 0.5f, 0f, maxBrightness, 1f);
            actualMinHue = MathSET.Map(brickRatio, 0.5f, 0f, minHue, 0f);
            actualMaxHue = MathSET.Map(brickRatio, 0.5f, 0f, maxHue, 1f);
        }

        float clockMod1 = clock % 1f;

        if (clockMod1 <= 0.5f)
        {
            brightness = MathSET.Map(clockMod1, 0f, 0.5f, actualMinBrightness, actualMaxBrightness);
        }
        else if (clockMod1 < 1f)
        {
            brightness = MathSET.Map(clockMod1, 0.5f, 1f, actualMaxBrightness, actualMinBrightness);
        }

        if (clock <= 1f)
        {
            hue = MathSET.Map(clock, 0f, 1f, actualMinHue, actualMaxHue);
        }
        else if (clock > 1f)
        {
            hue = MathSET.Map(clock, 1f, 2f, actualMaxHue, actualMinHue);
        }

        lastColor = Color.HSVToRGB(hue, 1f, brightness);

        renderer.color = lastColor;
        clock = (clock + speed * Time.deltaTime) % 2f;
    }
}
