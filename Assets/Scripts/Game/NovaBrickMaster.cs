using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NovaBrickMaster : MonoBehaviour
{
    private new Transform transform;

    public GameObject prefab;
    private Renderer prefabRenderer;

    public uint rowCount = 6;
    public uint columnCount = 14;
    public float padding = 0.05f;

    private Vector2 basePos;

    public List<NovaBrick> bricks;
    public List<Vector2> freePositions;

    void Awake()
    {
        bricks = new List<NovaBrick>();
        freePositions = new List<Vector2>();

        transform = GetComponent<Transform>();

        float prefabWidth = (Camera.main.orthographicSize * Camera.main.aspect * 2f - (columnCount + 1) * padding) / columnCount;

        Transform prefabTransform = prefab.GetComponent<Transform>();
        prefabRenderer = prefab.GetComponent<SpriteRenderer>();

        prefabTransform.localScale = new Vector3(prefabTransform.localScale.x * (prefabWidth / prefabRenderer.bounds.size.x), prefabTransform.localScale.y, prefabTransform.localScale.z);

        transform.position = new Vector3(-Camera.main.orthographicSize * Camera.main.aspect + prefabWidth / 2f + padding, Camera.main.orthographicSize - prefabRenderer.bounds.size.y / 2f - padding, transform.position.z);

        basePos = transform.position;

        OnReset();
    }

    public void OnReset()
    {
        Clear();

        for (uint column = 0; column < columnCount; ++column)
        {
            for (uint row = 0; row < rowCount; ++row)
            {
                Vector2 pos = (Vector2)basePos + new Vector2(column * (prefabRenderer.bounds.size.x + padding), -row * (prefabRenderer.bounds.size.y + padding));
                CreateBrick(pos, NovaGame.difficulty.GetBrickStrength(row, column));
            }
        }
    }

    public void Clear()
    {
        foreach (NovaBrick brick in bricks)
            Destroy(brick.gameObject);
        bricks.Clear();

        freePositions.Clear();
    }

    public void CreateBrick(Vector2 pos, uint strength)
    {
        NovaBrick brick = Instantiate(prefab, pos, Quaternion.identity).GetComponent<NovaBrick>();
        brick.Construct(strength);
        bricks.Add(brick);
    }

    public void DestroyBrick(NovaBrick brick)
    {
        bricks.Remove(brick);
        Destroy(brick.gameObject);

        if (bricks.Count == 0)
        {
            NovaGame.OnWin();
        }
        else
        {
            freePositions.Add(brick.gameObject.transform.position);
        }
    }

    public void RecreateBrick(int index)
    {
        if (freePositions.Count == 0)
            return;

        CreateBrick(freePositions[index], 1);
        freePositions.RemoveAt(index);
    }

    public float GetTopLimit()
    {
        return ((Vector2)basePos).y - rowCount * (prefabRenderer.bounds.size.y + padding);
    }

    public float GetMaxBrickCount()
    {
        return rowCount * columnCount;
    }
}
