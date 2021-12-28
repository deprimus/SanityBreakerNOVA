using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaBrick : MonoBehaviour
{
    private new Transform transform;
    private new SpriteRenderer renderer;

    public uint strength;

    public const uint MAX_STRENGTH = 4;

    public void Construct(uint strength)
    {
        this.strength = strength;
    }

    void Start() {
        transform = GetComponent<Transform>();
        renderer = GetComponent<SpriteRenderer>();
        OnStrengthChange();
    }

    public void OnHit()
    {
        --strength;

        if (strength == 0)
            NovaGame.brickMaster.DestroyBrick(this);
        else OnStrengthChange();
    }

    public void OnRegen()
    {
        if (strength == MAX_STRENGTH)
            return;

        ++strength;

        OnStrengthChange();
    }

    public void OnStrengthChange()
    {
        SetColorIntensity(MathSET.Map(strength, 1f, MAX_STRENGTH, 1f, 0f));
    }

    private void SetColorIntensity(float intensity)
    {
        if (renderer == null || renderer.material == null)
            return;

        Color color = renderer.material.color;
        color.r = intensity;
        color.g = intensity;
        color.b = intensity;
        renderer.material.color = color;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!NovaGame.BricksAreIndestructible())
        {
            if(!NovaGame.difficulty.allowDoubleBrickDestruction)
            {
                if (TaleUtil.Triggers.GetImmediate("nova_double_brick_destruction"))
                    return;
                TaleUtil.Triggers.Set("nova_double_brick_destruction");
            }

            if(strength == MAX_STRENGTH && (UnityEngine.Random.Range(0, 100) / 100f) > NovaGame.difficulty.brickMaxStrengthBreakChance)
            {
                return;
            }

            OnHit();
            NovaGame.ball.particleSystem.Play();
        }
    }
}
