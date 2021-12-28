using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaDeprimusMaster : MonoBehaviour
{
    public GameObject prefab;
    private Renderer prefabRenderer;

    private class Bounds
    {
        public float left;
        public float right;
        public float top;
        public float bottom;
    }

    private Bounds spawnBounds;

    public class EffectInfo
    {
        public float remaining;
        public Delegates.ShallowDelegate untrigger;

        public EffectInfo(float remaining, Delegates.ShallowDelegate untrigger) { this.remaining = remaining; this.untrigger = untrigger; }

        public EffectInfo Clone() => new EffectInfo(remaining, untrigger);
    }

    public List<EffectInfo> effects;
    public List<NovaDeprimus> deprimuses;

    public Dictionary<NovaDeprimus.Type, int> counters;

    void Start()
    {
        effects = new List<EffectInfo>();
        deprimuses = new List<NovaDeprimus>();
        counters = new Dictionary<NovaDeprimus.Type, int>();

        ResetCounters();

        prefabRenderer = prefab.GetComponent<Renderer>();

        spawnBounds = new Bounds();
        spawnBounds.left = -Camera.main.orthographicSize * Camera.main.aspect + prefabRenderer.bounds.size.x / 2f;
        spawnBounds.right = Camera.main.orthographicSize * Camera.main.aspect - prefabRenderer.bounds.size.x / 2f;
        spawnBounds.top = NovaGame.brickMaster.GetTopLimit() - prefabRenderer.bounds.size.y / 2f;
        spawnBounds.bottom = NovaGame.player.transform.position.y + NovaGame.player.GetComponent<Renderer>().bounds.size.y / 2f + prefabRenderer.bounds.size.y / 2f;
    }

    void Update()
    {
        TickEffects();
    }

    void TickEffects()
    {
        for (int i = 0; i < effects.Count;)
        {
            effects[i].remaining -= Time.deltaTime;

            if (effects[i].remaining <= 0f)
            {
                effects[i].untrigger();
                effects.RemoveAt(i);
            }
            else ++i;
        }
    }

    public void OnTick()
    {
        if (NovaGame.difficulty.deprimusSpawnChance > UnityEngine.Random.Range(0, 100) / 100f)
            Spawn();
    }

    public NovaDeprimus.Type GenerateType()
    {
        if(NovaGame.novaModeMaster.active)
        {
            return NovaDeprimus.Type.OMNIDEPRIMUS;
        }

        float value = MathSET.Map(UnityEngine.Random.Range(0, 1000000), 0, 1000000, 0, NovaGame.difficulty.deprimusPoolChanceSum);
        NovaDeprimusSpawnInfo info = null;

        for (int i = 0; i < NovaGame.difficulty.deprimusPool.Count; ++i)
        {
            NovaDeprimusSpawnInfo currentInfo = NovaGame.difficulty.deprimusPool[i];

            if(currentInfo.chance == 0f)
            {
                continue;
            }

            info = currentInfo;

            if (value - info.chance < 0)
                break;

            value -= info.chance;
        }

        return info.type;
    }

    public NovaDeprimus.Type GenerateTypeUnweighted()
    {
        Array pool = Enum.GetValues(typeof(NovaDeprimus.Type));
        return (NovaDeprimus.Type) pool.GetValue(UnityEngine.Random.Range(0, pool.Length - 1)); // Exclude omnideprimus
    }

    public Vector2 RandomPosInBounds()
    {
        return new Vector2(UnityEngine.Random.Range(spawnBounds.left, spawnBounds.right), UnityEngine.Random.Range(spawnBounds.bottom, spawnBounds.top));
    }

    public void Spawn()
    {
        if (NovaGame.difficulty.deprimusPool.Count == 0)
            return;

        SpawnOfType(GenerateType());
    }

    public void SpawnOfType(NovaDeprimus.Type type, bool sound = true)
    {
        Vector2 pos = RandomPosInBounds();

        NovaDeprimus deprimus = Instantiate(prefab, pos, Quaternion.identity).GetComponent<NovaDeprimus>();
        deprimus.Construct(type);

        deprimuses.Add(deprimus);

        if(sound)
            NovaSoundMaster.Play(NovaSoundMaster.Clip.SPAWN);
    }

    public void SpawnOfTypeAt(NovaDeprimus.Type type, Vector2 pos, bool sound = true)
    {
        NovaDeprimus deprimus = Instantiate(prefab, pos, Quaternion.identity).GetComponent<NovaDeprimus>();
        deprimus.Construct(type);

        deprimuses.Add(deprimus);

        if(sound)
            NovaSoundMaster.Play(NovaSoundMaster.Clip.SPAWN);
    }

    public void SpawnOfTypeAt(NovaDeprimus.Type type, NovaDeprimus.Type impersonated, NovaDeprimus.Type actualType, Vector2 pos, bool sound = true)
    {
        NovaDeprimus deprimus = Instantiate(prefab, pos, Quaternion.identity).GetComponent<NovaDeprimus>();
        deprimus.Construct(type, impersonated, actualType);

        deprimuses.Add(deprimus);

        if (sound)
            NovaSoundMaster.Play(NovaSoundMaster.Clip.SPAWN);
    }

    public void OnReset()
    {
        foreach(EffectInfo effect in effects)
            effect.untrigger();
        effects.Clear();

        while(deprimuses.Count > 0)
        {
            deprimuses[0].Despawn();
            deprimuses.RemoveAt(0);
        }
    }

    public void Consume(NovaDeprimus deprimus, float time, Delegates.ShallowDelegate onTrigger, Delegates.ShallowDelegate onUntrigger)
    {
        onTrigger();

        if(time != 0f)
            effects.Add(new EffectInfo(time, onUntrigger));

        Remove(deprimus);
    }

    public void Remove(NovaDeprimus deprimus)
    {
        deprimuses.Remove(deprimus);
    }

    void ResetCounters()
    {
        counters[NovaDeprimus.Type.CRIZATUS] = 0;
        counters[NovaDeprimus.Type.CONFUSCIUS] = 0;
        counters[NovaDeprimus.Type.CURIOSUS] = 0;
        counters[NovaDeprimus.Type.ORDINARIUS] = 0;
        counters[NovaDeprimus.Type.DISGUSTUS] = 0;
        counters[NovaDeprimus.Type.OBSEDATUS] = 0;
        counters[NovaDeprimus.Type.HLIZATUS] = 0;
        counters[NovaDeprimus.Type.NERVOSUS] = 0;
        counters[NovaDeprimus.Type.WHEEZUS] = 0;
        counters[NovaDeprimus.Type.CHROMATICUS] = 0;
        counters[NovaDeprimus.Type.TRAUMATIZATUS] = 0;
        counters[NovaDeprimus.Type.DEPRIMUWUS] = 0;
        counters[NovaDeprimus.Type.PLANSUS] = 0;
    }
}
