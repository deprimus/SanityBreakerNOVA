using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScrollbar : MonoBehaviour
{
    private enum State
    {
        SHOWN,
        HIDING,
        HIDDEN
    }

    public Image handleImage;

    private float DECAY_DELAY = 2f;
    private float DECAY_DURATION = 1f;

    private float clock;
    private bool hidden;

    private State state;

    void Start()
    {
        Hide();
        GetComponent<Scrollbar>().onValueChanged.AddListener((_) => Show());
    }

    void Update()
    {
        switch(state)
        {
            case State.SHOWN:
                clock += Time.unscaledDeltaTime;

                if(clock >= DECAY_DELAY)
                {
                    clock = 0f;
                    state = State.HIDING;
                    return;
                }
                break;
            case State.HIDING:
                clock += Time.unscaledDeltaTime;

                if(clock >= DECAY_DURATION)
                {
                    Hide();
                }
                else
                {
                    SetAlpha(MathSET.Map(clock, 0, DECAY_DURATION, 1f, 0f));
                }
                break;
            default:
                return;
        }
    }

    void SetAlpha(float value)
    {
        handleImage.color = new Color(1f, 1f, 1f, value);
    }

    public void Hide()
    {
        SetAlpha(0f);
        clock = 0f;
        state = State.HIDDEN;
    }

    public void Show()
    {
        SetAlpha(1f);
        clock = 0f;
        state = State.SHOWN;
    }
}
