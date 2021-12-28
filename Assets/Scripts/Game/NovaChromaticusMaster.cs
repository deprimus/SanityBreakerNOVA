using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaChromaticusMaster : MonoBehaviour
{
    public GameObject hotspotPrefab;
    public List<Color> colors;
    public List<NovaHotspot> hotspots;

    void Start()
    {
        colors = new List<Color>();
        hotspots = new List<NovaHotspot>();
    }

    void LateUpdate()
    {
        if(hotspots.Count > 0)
        {
            RepositionHotspots();
        }
    }

    public void OnTick()
    {
        if (colors.Count == 0)
            return;

        if (NovaGame.difficulty.ballColorChangeChance > UnityEngine.Random.Range(0, 100) / 100f)
            ChangeBallColor();
    }

    public void OnReset()
    {
        colors.Clear();
        ClearHotspots();
    }

    public void AddColor() => AddColor(new Color32((byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), 255));

    public void AddColor(Color color)
    {
        colors.Add(color);

        NovaHotspot hotspot = Instantiate(hotspotPrefab, Vector2.zero, Quaternion.identity).GetComponent<NovaHotspot>();
        hotspot.Construct(color, hotspots.Count + 1);

        hotspots.Add(hotspot);

        if (colors.Count == 1)
        {
            AddColor();
        }
        else
        {
            RescaleHotspots();
        }

        ChangeBallColor();
    }

    public void RemoveColor()
    {
        Debug.Assert(colors.Count > 1, "Color list has count 0 or 1");

        colors.RemoveAt(colors.Count - 1);
        RemoveHotspot();

        if (colors.Count == 1)
        {
            colors.Clear();
            ClearHotspots();
            NovaGame.ball.ResetColor();
        }
        else
        {
            ChangeBallColor();
            RescaleHotspots();
        }
    }

    void RemoveHotspot()
    {
        Destroy(hotspots[hotspots.Count - 1].gameObject);
        hotspots.RemoveAt(hotspots.Count - 1);
    }

    public void ClearHotspots()
    {
        for(int i = hotspots.Count - 1; i >= 0; --i)
        {
            Destroy(hotspots[i].gameObject);
            hotspots.RemoveAt(i);
        }
    }

    void ChangeBallColor()
    {
        NovaGame.ball.ChangeColor(colors[UnityEngine.Random.Range(0, colors.Count)]);
    }

    public void RescaleHotspots()
    {
        Vector2 parent;
        Vector2 pos;

        if(NovaGame.PlayerIsDeadly())
        {
            parent = new Vector2(Camera.main.orthographicSize * Camera.main.aspect * 2f, NovaGame.player.renderer.bounds.size.y / 2f);
            pos    = new Vector2(0, -Camera.main.orthographicSize + NovaGame.player.renderer.bounds.size.y / 4f);
        }
        else
        {
            parent = NovaGame.player.renderer.bounds.size;
            pos    = NovaGame.player.transform.position;
        }

        float scale       = 1f / colors.Count;

        float xIncrement = parent.x / colors.Count;
        float xOffset    = pos.x - parent.x / 2f + xIncrement / 2f;

        parent = new Vector2(parent.x, parent.y * 1.1f);

        for(int i = 0; i < colors.Count; ++i, xOffset += xIncrement)
        {
            hotspots[i].Rescale(new Vector2(xOffset, pos.y), parent, scale);
        }
    }

    void RepositionHotspots()
    {
        if (NovaGame.PlayerIsDeadly())
            return;

        Vector2 parent = NovaGame.player.renderer.bounds.size;
        Vector2 pos = NovaGame.player.transform.position;

        float xIncrement = parent.x / colors.Count;
        float xOffset = pos.x - parent.x / 2f + xIncrement / 2f;

        for (int i = 0; i < colors.Count; ++i, xOffset += xIncrement)
        {
            hotspots[i].Reposition(new Vector2(xOffset, pos.y));
        }
    }
}
