using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaHotspot : MonoBehaviour
{
    private new Transform transform;
    private new SpriteRenderer renderer;
    private Color color;

    public void Construct(Color color, int order)
    {
        InitComponents();
        this.color = color;

        renderer.material.color = color;
        renderer.sortingOrder   = order;
    }

    public void Rescale(Vector2 pos, Vector2 parent, float scale)
    {
        transform.position = pos;

        float scaleModX = parent.x / renderer.bounds.size.x;
        float scaleModY = parent.y / renderer.bounds.size.y;

        transform.localScale = new Vector3(transform.localScale.x * scaleModX * scale, transform.localScale.y * scaleModY, transform.localScale.z);
    }

    public void Reposition(Vector2 pos)
    {
        transform.position = pos;
    }

    void InitComponents()
    {
        if (!transform)
        {
            transform = GetComponent<Transform>();
        }

        if (!renderer)
        {
            renderer = GetComponent<SpriteRenderer>();
        }
    }

    void Start()
    {
        InitComponents();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(NovaGame.ball.color != color)
        {
            NovaGame.OnReset();
        }
    }
}
