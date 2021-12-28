using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaTimer : MonoBehaviour
{
    public float clock;

    void Start()
    {
        clock = 0;
    }

    void Update()
    {
        clock += Time.deltaTime;
        float delay = NovaGame.difficulty.GetTickDelay();

        while(clock >= delay)
        {
            clock -= delay;

            NovaGame.OnTick();
            NovaGame.difficulty.OnTick();
        }
    }
}
