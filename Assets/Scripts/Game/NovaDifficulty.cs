using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaDeprimusSpawnInfo
{
    public NovaDeprimus.Type type;
    public float chance;

    public NovaDeprimusSpawnInfo(NovaDeprimus.Type type, float chance)
    {
        this.type = type;
        this.chance = chance;
    }
}

public class NovaDifficulty
{
    public string description;
    public List<string> brief;

    public List<NovaDeprimusSpawnInfo> deprimusPool;
    public float deprimusPoolChanceSum;

    public float deprimusSpawnChance;
    public float ballColorChangeChance;
    public uint maxBallColors;
    public float ballTeleportChance;
    public float ballVelocityChangeChance;
    public float shakeMultiplier;
    public bool stackShakes;
    public bool indestructibleBricksForChaoticBall;
    public bool allowDoubleBrickDestruction;
    public float brickMaxStrengthBreakChance;
    public bool allowPlayerSideCollisions;
    public float novaModeDuration;
    public uint novaModeMinInstantOmnideprimus;
    public uint novaModeMaxInstantOmnideprimus;
    public float omnideprimusPolymorphDelay;
    public uint traumatizatusThreshold;
    public bool dynamicTickDelay;
    public float tickDelayMin;
    public float tickDelayMax;
    public List<uint> brickStrengths;

    public Delegates.ShallowDelegate OnTick;

    private string cachedBriefText;

    public uint GetBrickStrength(uint row, uint col)
    {
        return brickStrengths[(int) row];
    }

    public float GetTickDelay()
    {
        if (dynamicTickDelay)
        {
            return MathSET.Map(NovaGame.brickMaster.bricks.Count, NovaGame.brickMaster.GetMaxBrickCount(), 1, tickDelayMax, tickDelayMin);
        }
        else
        {
            return tickDelayMax;
        }
    }

    public NovaDeprimusSpawnInfo GetDeprimusInfo(NovaDeprimus.Type type)
    {
        for(int i = 0; i < deprimusPool.Count; ++i)
        {
            if (deprimusPool[i].type == type)
            {
                return deprimusPool[i];
            }
        }

        return null;
    }

    public NovaDifficulty Clone()
    {
        NovaDifficulty difficulty = new NovaDifficulty();

        difficulty.description                        = description;
        difficulty.brief                              = new List<string>(brief);
        difficulty.deprimusPool                       = new List<NovaDeprimusSpawnInfo>(deprimusPool);
        difficulty.deprimusPoolChanceSum              = deprimusPoolChanceSum;
        difficulty.deprimusSpawnChance                = deprimusSpawnChance;
        difficulty.ballColorChangeChance              = ballColorChangeChance;
        difficulty.maxBallColors                      = maxBallColors;
        difficulty.ballTeleportChance                 = ballTeleportChance;
        difficulty.ballVelocityChangeChance           = ballVelocityChangeChance;
        difficulty.shakeMultiplier                    = shakeMultiplier;
        difficulty.stackShakes                        = stackShakes;
        difficulty.indestructibleBricksForChaoticBall = indestructibleBricksForChaoticBall;
        difficulty.allowDoubleBrickDestruction        = allowDoubleBrickDestruction;
        difficulty.brickMaxStrengthBreakChance        = brickMaxStrengthBreakChance;
        difficulty.allowPlayerSideCollisions          = allowPlayerSideCollisions;
        difficulty.novaModeDuration                   = novaModeDuration;
        difficulty.novaModeMinInstantOmnideprimus     = novaModeMinInstantOmnideprimus;
        difficulty.novaModeMaxInstantOmnideprimus     = novaModeMaxInstantOmnideprimus;
        difficulty.omnideprimusPolymorphDelay         = omnideprimusPolymorphDelay;
        difficulty.traumatizatusThreshold             = traumatizatusThreshold;
        difficulty.brickStrengths                     = new List<uint>(brickStrengths);
        difficulty.dynamicTickDelay                   = dynamicTickDelay;
        difficulty.tickDelayMin                       = tickDelayMin;
        difficulty.tickDelayMax                       = tickDelayMax;
        difficulty.OnTick                             = OnTick;

        return difficulty;
    }

    public NovaDifficulty()
    {
        deprimusPool = new List<NovaDeprimusSpawnInfo>();

        Array pool = Enum.GetValues(typeof(NovaDeprimus.Type));

        for(uint i = 0; i < pool.Length; ++i)
        {
            float value = 1f;

            if ((NovaDeprimus.Type) pool.GetValue(i) == NovaDeprimus.Type.OMNIDEPRIMUS)
            {
                value = 0f;
            }

            deprimusPool.Add(new NovaDeprimusSpawnInfo((NovaDeprimus.Type) pool.GetValue(i), value));
        }

        cachedBriefText = null;
    }

    public void UpdateChanceSum()
    {
        deprimusPoolChanceSum = 0;

        for(int i = 0; i < deprimusPool.Count; ++i)
        {
            deprimusPoolChanceSum += deprimusPool[i].chance;
        }
    }

    public string GetBriefText()
    {
        if (cachedBriefText == null)
        {
            CacheBriefText();
        }

        return cachedBriefText;
    }

    private void CacheBriefText()
    {
        cachedBriefText = "";

        const string POSITIVE_COLOR = "#00ff00";
        const string NEUTRAL_COLOR = "#ffff00";
        const string NEGATIVE_COLOR = "#ff0000";

        foreach(string line in brief)
        {
            if(line.Length > 0)
            {
                char c = line[0];

                switch(c)
                {
                    case '+':
                        cachedBriefText += string.Format("<color={0}>{1}</color>", POSITIVE_COLOR, line.Substring(1)).Trim();
                        break;
                    case '~':
                        cachedBriefText += string.Format("<color={0}>{1}</color>", NEUTRAL_COLOR, line.Substring(1)).Trim();
                        break;
                    case '-':
                        cachedBriefText += string.Format("<color={0}>{1}</color>", NEGATIVE_COLOR, line.Substring(1)).Trim();
                        break;
                    default:
                        cachedBriefText += line.Trim();
                        break;
                }
            }

            cachedBriefText += '\n';
        }

        cachedBriefText.TrimEnd('\n');
    }

    public static Dictionary<string, NovaDifficulty> DIFFICULTIES = new Dictionary<string, NovaDifficulty>()
    {
        { "Fair",          CreateFair() },
        { "Regular",       CreateRegular() },
        { "Lunatic",       CreateLunatic() }
    };

    private static NovaDifficulty CreateFair()
    {
        NovaDifficulty difficulty = new NovaDifficulty();

        difficulty.description                        = "The easiest difficulty. Almost unbeatable by the AI.";
        difficulty.brief                              = new List<string>()
        {
            "+Every deprimus has the same spawn chance",
            "+Less Omnideprimuses for Nova mode",
            "+Decreased max colors",
            "+Weak bricks",
            "+Lower ball teleportation chance",
            "+Destructible bricks for chaotic ball",
            "+Double brick destruction is allowed",
            "+Side collisions are allowed",
            "+Large fixed tick delay",
            "+Shakes don't stack"
        };

        difficulty.deprimusSpawnChance                = 0.3f;
        difficulty.ballColorChangeChance              = 0.1f;
        difficulty.maxBallColors                      = 2;
        difficulty.ballTeleportChance                 = 0.1f;
        difficulty.ballVelocityChangeChance           = 0f;
        difficulty.shakeMultiplier                    = 1f;
        difficulty.stackShakes                        = false;
        difficulty.indestructibleBricksForChaoticBall = false;
        difficulty.allowDoubleBrickDestruction        = true;
        difficulty.brickMaxStrengthBreakChance        = 1f;
        difficulty.allowPlayerSideCollisions          = true;
        difficulty.novaModeDuration                   = 5f;
        difficulty.novaModeMinInstantOmnideprimus     = 1;
        difficulty.novaModeMaxInstantOmnideprimus     = 3;
        difficulty.omnideprimusPolymorphDelay         = 0.05f;
        difficulty.traumatizatusThreshold             = 10;
        difficulty.brickStrengths                     = new List<uint>() { 1, 1, 1, 1, 1, 1 };
        difficulty.dynamicTickDelay                   = false;
        difficulty.tickDelayMax                       = 1.0f;
        difficulty.OnTick                             = () => { };

        difficulty.UpdateChanceSum();

        return difficulty;
    }

    private static NovaDifficulty CreateRegular()
    {
        NovaDifficulty difficulty = new NovaDifficulty();

        difficulty.description = "The way it's meant to be played. Significantly harder.";
        difficulty.brief = new List<string>()
        {
            "~Reduced spawn chance for good deprimuses",
            "~50% breaking chance for max strength bricks",
            "~Lower Traumatizatus threshold",
            "~Larger shake multiplier"
        };

        difficulty.deprimusSpawnChance                = 0.2f;
        difficulty.ballColorChangeChance              = 0.1f;
        difficulty.maxBallColors                      = 3;
        difficulty.ballTeleportChance                 = 0.15f;
        difficulty.ballVelocityChangeChance           = 0f;
        difficulty.shakeMultiplier                    = 1.5f;
        difficulty.stackShakes                        = true;
        difficulty.indestructibleBricksForChaoticBall = true;
        difficulty.allowDoubleBrickDestruction        = false;
        difficulty.brickMaxStrengthBreakChance        = 0.5f;
        difficulty.allowPlayerSideCollisions          = false;
        difficulty.novaModeDuration                   = 5f;
        difficulty.novaModeMinInstantOmnideprimus     = 2;
        difficulty.novaModeMaxInstantOmnideprimus     = 6;
        difficulty.omnideprimusPolymorphDelay         = 0.05f;
        difficulty.traumatizatusThreshold             = 5;
        difficulty.brickStrengths                     = new List<uint>() { 3, 2, 2, 1, 1, 1 };
        difficulty.dynamicTickDelay                   = true;
        difficulty.tickDelayMin                       = 0.1f;
        difficulty.tickDelayMax                       = 0.5f;
        difficulty.OnTick                             = () => { };

        difficulty.GetDeprimusInfo(NovaDeprimus.Type.INGINIUS).chance = 0.5f;
        difficulty.GetDeprimusInfo(NovaDeprimus.Type.CHANSUS).chance = 0.5f;
        difficulty.GetDeprimusInfo(NovaDeprimus.Type.FERICITUS).chance = 0.5f;
        difficulty.GetDeprimusInfo(NovaDeprimus.Type.GENIUS).chance = 0.5f;

        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.CHROMATICUS).chance = 50;

        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.DEPRIMUWUS).chance = 15;
        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.DISGUSTUS).chance = 15;
        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.ORDINARIUS).chance = 15;
        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.CURIOSUS).chance = 15;

        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.CURIOSUS).chance = 8;
        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.GREEDUS).chance = 0;

        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.TRAUMATIZATUS).chance = 50;
        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.GREEDUS).chance = 50;
        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.FERICITUS).chance = 50;

        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.CHROMATICUS).chance = 50;
        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.FACADUS).chance = 20;
        //difficulty.GetDeprimusInfo(NovaDeprimus.Type.DISGUSTUS).chance = 20;

        difficulty.UpdateChanceSum();

        return difficulty;
    }

    private static NovaDifficulty CreateLunatic()
    {
        NovaDifficulty difficulty = new NovaDifficulty();

        difficulty.description = "Good luck.";
        difficulty.brief = new List<string>()
        {
            "~+ Chromaticus, Facadus",
            "~Higher deprimus spawn rate",
            "~Higher chance of changing ball colors",
            "~More Omnideprimuses for Nova mode",
            "~Nova mode lasts longer",
            "-Positive deprimuses can't spawn naturally",
            "-Insanely strong bricks",
            "-Higher ball teleportation chance",
            "-Small ball velocity change chance",
            "-Unlimited max colors",
            "-10% breaking chance for max strength bricks",
            "-INSTAKILL Traumatizatus threshold",
            "-Much larger shake multiplier"
        };

        difficulty.deprimusSpawnChance                = 0.25f;
        difficulty.ballColorChangeChance              = 0.2f;
        difficulty.maxBallColors                      = 0;
        difficulty.ballTeleportChance                 = 0.2f;
        difficulty.ballVelocityChangeChance           = 0.05f;
        difficulty.shakeMultiplier                    = 2f;
        difficulty.stackShakes                        = true;
        difficulty.indestructibleBricksForChaoticBall = true;
        difficulty.allowDoubleBrickDestruction        = false;
        difficulty.brickMaxStrengthBreakChance        = 0.1f;
        difficulty.allowPlayerSideCollisions          = false;
        difficulty.novaModeDuration                   = 7.5f;
        difficulty.novaModeMinInstantOmnideprimus     = 3;
        difficulty.novaModeMaxInstantOmnideprimus     = 8;
        difficulty.omnideprimusPolymorphDelay         = 0.05f;
        difficulty.traumatizatusThreshold             = 1;
        difficulty.OnTick                             = () => { };
        difficulty.brickStrengths                     = new List<uint>() { 4, 4, 4, 4, 4, 4 };
        difficulty.dynamicTickDelay                   = true;
        difficulty.tickDelayMin                       = 0.1f;
        difficulty.tickDelayMax                       = 0.5f;
        difficulty.OnTick                             = () => { };

        difficulty.GetDeprimusInfo(NovaDeprimus.Type.INGINIUS).chance = 0;
        difficulty.GetDeprimusInfo(NovaDeprimus.Type.CHANSUS).chance = 0;
        difficulty.GetDeprimusInfo(NovaDeprimus.Type.FERICITUS).chance = 0;
        difficulty.GetDeprimusInfo(NovaDeprimus.Type.GENIUS).chance = 0;

        difficulty.GetDeprimusInfo(NovaDeprimus.Type.CHROMATICUS).chance = 3;
        difficulty.GetDeprimusInfo(NovaDeprimus.Type.FACADUS).chance = 5;

        difficulty.UpdateChanceSum();

        return difficulty;
    }
}