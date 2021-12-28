using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaPlayer : MonoBehaviour
{
    public new SpriteRenderer renderer;
    public new Transform transform;

    public bool isAI = false;

    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        transform = GetComponent<Transform>();
    }

    void Update()
    {
        UpdatePos();   
    }

    void UpdatePos()
    {
        if (!isAI)
        {
            transform.position = new Vector2((NovaGame.PlayerIsConfused() ? -1 : 1) * Mathf.Max(-Camera.main.orthographicSize * Camera.main.aspect, Mathf.Min(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.ScreenToWorldPoint(Input.mousePosition).x)), transform.position.y);
        }
        else
        {
            Vector3 desiredPos = new Vector3(NovaGame.ball.transform.position.x, transform.position.y, transform.position.z);

            if(NovaGame.PlayerIsDeadly())
                desiredPos = new Vector3((NovaGame.ball.transform.position.x < 0 ? 1 : -1)* Camera.main.orthographicSize * Camera.main.aspect, desiredPos.y, desiredPos.z);

            transform.position = desiredPos;
        }
    }
}
