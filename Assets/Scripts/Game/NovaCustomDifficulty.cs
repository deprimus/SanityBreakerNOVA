using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;

public class NovaCustomDifficultyTickDelay
{
    public bool? dynamic;
    public float? min;
    public float? max;

    public NovaCustomDifficultyTickDelay() { }

    public NovaCustomDifficultyTickDelay(bool dynamic, float min, float max)
    {
        this.dynamic = dynamic;
        this.min = min;
        this.max = max;
    }
}

public class NovaCustomDifficulty
{
    public string name;
    public string description;
    public List<string> brief;
    public float? deprimus_spawn_chance;
    public Dictionary<string, uint> deprimus_chances;
    public float? ball_color_change_chance;
    public uint? ball_max_colors;
    public float? ball_teleport_chance;
    public float? ball_velocity_change_chance;
    public float? shake_multiplier;
    public bool? shake_stack;
    public bool? indestructible_bricks_for_chaotic_ball;
    public bool? allow_double_brick_destruction;
    public float? brick_max_strength_break_chance;
    public bool? allow_player_side_collisions;
    public float? nova_mode_duration;
    public uint? nova_mode_min_instant_omnideprimus;
    public uint? nova_mode_max_instant_omnideprimus;
    public float? omnideprimus_polymorph_delay;
    public uint? traumatizatus_threshold;
    public NovaCustomDifficultyTickDelay tick_delay;
    public List<uint> brick_strengths;

    public NovaDifficulty ToNovaDifficulty()
    {
        NovaDifficulty difficulty = new NovaDifficulty();

        difficulty.description                        = description;
        difficulty.brief                              = new List<string>(brief);
        difficulty.deprimusSpawnChance                = deprimus_spawn_chance.Value;
        difficulty.ballColorChangeChance              = ball_color_change_chance.Value;
        difficulty.maxBallColors                      = ball_max_colors.Value;
        difficulty.ballTeleportChance                 = ball_teleport_chance.Value;
        difficulty.ballVelocityChangeChance           = ball_velocity_change_chance.Value;
        difficulty.shakeMultiplier                    = shake_multiplier.Value;
        difficulty.stackShakes                        = shake_stack.Value;
        difficulty.indestructibleBricksForChaoticBall = indestructible_bricks_for_chaotic_ball.Value;
        difficulty.allowDoubleBrickDestruction        = allow_double_brick_destruction.Value;
        difficulty.brickMaxStrengthBreakChance        = brick_max_strength_break_chance.Value;
        difficulty.allowPlayerSideCollisions          = allow_player_side_collisions.Value;
        difficulty.novaModeDuration                   = nova_mode_duration.Value;
        difficulty.novaModeMinInstantOmnideprimus     = nova_mode_min_instant_omnideprimus.Value;
        difficulty.novaModeMaxInstantOmnideprimus     = nova_mode_max_instant_omnideprimus.Value;
        difficulty.omnideprimusPolymorphDelay         = omnideprimus_polymorph_delay.Value;
        difficulty.traumatizatusThreshold             = traumatizatus_threshold.Value;
        difficulty.brickStrengths                     = new List<uint>(brick_strengths);
        difficulty.dynamicTickDelay                   = tick_delay.dynamic.Value;
        difficulty.tickDelayMin                       = tick_delay.min.Value;
        difficulty.tickDelayMax                       = tick_delay.max.Value;
        difficulty.OnTick                             = () => { };

        foreach(KeyValuePair<string, uint> entry in deprimus_chances)
        {
            NovaDeprimus.Type deprimus;

            if(!Enum.TryParse<NovaDeprimus.Type>(entry.Key.ToUpperInvariant(), out deprimus))
            {
                continue;
            }

            difficulty.GetDeprimusInfo(deprimus).chance = entry.Value;
        }

        difficulty.UpdateChanceSum();

        return difficulty;
    }

    public void Register()
    {
        NovaDifficulty.DIFFICULTIES[name] = ToNovaDifficulty();
        DIFFICULTIES.Add(name);
    }

    public static List<string> DIFFICULTIES = new List<string>();

    private static fsSerializer serializer = new fsSerializer();
    public static string CUSTOM_DIFFICULTIES_PATH = Path.Combine(Application.dataPath, "Difficulties");

    private static bool deserialized = false;

    private static void Default<T>(ref Nullable<T> field, T defaultValue) where T : struct
    {
        if(!field.HasValue)
        {
            field = defaultValue;
        }
    }

    private static void Default<T>(ref T field, T defaultValue)
    {
        if (field == null)
        {
            field = defaultValue;
        }
    }

    public static void DeserializeAll()
    {
        if(deserialized)
        {
            return;
        }

        deserialized = true;

        try
        {
            foreach (string file in Directory.EnumerateFiles(CUSTOM_DIFFICULTIES_PATH, "*.json", SearchOption.AllDirectories))
            {
                try
                {
                    DeserializeFile(file);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public static void DeserializeFile(string path)
    {
        string src = File.ReadAllText(path);

        Deserialize(src);
    }

    public static void Deserialize(string src)
    {
        fsData data = fsJsonParser.Parse(src);

        object obj = null;
        serializer.TryDeserialize(data, typeof(NovaCustomDifficulty), ref obj).AssertSuccessWithoutWarnings();

        NovaCustomDifficulty deserialized = (NovaCustomDifficulty) obj;

        if(deserialized.name == null)
        {
            return;
        }

        Default(ref deserialized.description, "No description.");
        Default(ref deserialized.brief, null);
        Default(ref deserialized.deprimus_spawn_chance, 0.3f);
        Default(ref deserialized.deprimus_chances, new Dictionary<string, uint>());
        Default(ref deserialized.ball_color_change_chance, 0.1f);
        Default<uint>(ref deserialized.ball_max_colors, 2);
        Default(ref deserialized.ball_teleport_chance, 0.1f);
        Default(ref deserialized.ball_velocity_change_chance, 0f);
        Default(ref deserialized.shake_multiplier, 1f);
        Default(ref deserialized.shake_stack, false);
        Default(ref deserialized.indestructible_bricks_for_chaotic_ball, false);
        Default(ref deserialized.allow_double_brick_destruction, true);
        Default(ref deserialized.brick_max_strength_break_chance, 1f);
        Default(ref deserialized.allow_player_side_collisions, true);
        Default(ref deserialized.nova_mode_duration, 5f);
        Default<uint>(ref deserialized.nova_mode_min_instant_omnideprimus, 2);
        Default<uint>(ref deserialized.nova_mode_max_instant_omnideprimus, 6);
        Default(ref deserialized.omnideprimus_polymorph_delay, 0.05f);
        Default<uint>(ref deserialized.traumatizatus_threshold, 10);
        Default(ref deserialized.tick_delay, new NovaCustomDifficultyTickDelay(true, 0.1f, 0.5f));
        Default(ref deserialized.brick_strengths, new List<uint>() { 1, 1, 1, 1, 1, 1 });

        Default(ref deserialized.tick_delay.dynamic, true);
        Default(ref deserialized.tick_delay.min, 0.1f);
        Default(ref deserialized.tick_delay.max, 0.5f);

        for (int i = deserialized.brick_strengths.Count; i < 6; ++i)
        {
            deserialized.brick_strengths.Add(1);
        }

        deserialized.Register();
    }
}
