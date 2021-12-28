using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NovaRainbow : MonoBehaviour
{
    public float speed = 0.2f;

    private float clock;
    private TextMeshProUGUI text;

    void Awake()
    {
        clock = 0f;

        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {

        text.color = Color.HSVToRGB(clock, 1f, 1f);
        clock = (clock + speed * Time.deltaTime) % 1f;
    }
}
