using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NovaModeMaster : MonoBehaviour
{
    public Animator indicatorAnimator;
    public TextMeshProUGUI indicatorText;
    public GameObject canvas;
    public bool active;

    public float clock;

    void Awake()
    {
        clock = 0f;
        active = false;
    }

    void Update()
    {
        if(active)
        {
            clock += Time.deltaTime;

            if(clock >= NovaGame.difficulty.novaModeDuration)
            {
                OnReset();
            }
        }


        if (canvas.activeSelf)
        {
            indicatorText.color = NovaGame.backgroundRainbow.lastColor.Invert();

            AnimatorStateInfo info = indicatorAnimator.GetCurrentAnimatorStateInfo(0);

            if (info.IsName("NovaIndicator") && info.normalizedTime >= 1f)
            {
                // Enter the idle state.
                indicatorAnimator.SetTrigger("Indicator");
                canvas.SetActive(false);
            }
        }
    }

    public void OnTrigger(NovaDeprimus emitter)
    {
        if (active)
            return;

        active = true;
        clock = 0f;
        canvas.SetActive(true);

        indicatorAnimator.SetTrigger("Indicator");

        List<Vector2> positions = new List<Vector2>();

        for(int i = 0; i < NovaGame.deprimusMaster.deprimuses.Count; ++i)
        {
            if(NovaGame.deprimusMaster.deprimuses[i] != emitter)
            {
                positions.Add(NovaGame.deprimusMaster.deprimuses[i].GetComponent<Transform>().position);
                NovaGame.deprimusMaster.deprimuses[i].Despawn();
            }
        }

        NovaGame.deprimusMaster.deprimuses.Clear();

        for(int i = 0; i < positions.Count; ++i)
        {
            NovaGame.deprimusMaster.SpawnOfTypeAt(NovaDeprimus.Type.OMNIDEPRIMUS, positions[i], false);
        }

        int instantCount = UnityEngine.Random.Range((int) NovaGame.difficulty.novaModeMinInstantOmnideprimus, (int) NovaGame.difficulty.novaModeMaxInstantOmnideprimus + 1);

        for (int i = 0; i < instantCount; ++i)
        {
            NovaGame.deprimusMaster.SpawnOfType(NovaDeprimus.Type.OMNIDEPRIMUS, false);
        }

        NovaGame.difficulty.deprimusSpawnChance *= 5;

        NovaSoundMaster.Play(NovaSoundMaster.Clip.NOVA);
    }

    public void OnReset()
    {
        if(active)
        {
            NovaGame.difficulty.deprimusSpawnChance /= 5;
        }

        active = false;
        clock = 0f;

        if(canvas.activeSelf)
        {
            AnimatorStateInfo info = indicatorAnimator.GetCurrentAnimatorStateInfo(0);

            if (info.IsName("NovaIndicator"))
            {
                indicatorAnimator.SetTrigger("Indicator");
            }

            canvas.SetActive(false);
        }
    }
}
