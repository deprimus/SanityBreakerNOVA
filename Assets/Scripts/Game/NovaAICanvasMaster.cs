using UnityEngine;

public class NovaAICanvasMaster : MonoBehaviour
{
    public GameObject aiCanvas;
    public float delay = 1f;

    private float clock;

    void Start()
    {
        clock = 0f;
    }

    void Update()
    {
        clock += Time.deltaTime;

        while (clock >= delay)
        {
            clock -= delay;

            if(NovaGame.player.isAI)
            {
                aiCanvas.SetActive(!aiCanvas.activeSelf);
            }
        }
    }
}
