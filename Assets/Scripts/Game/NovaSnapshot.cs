using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// There is no end though there is a start in space -- Infinity.
public class NovaSnapshot
{
    struct BrickSnapshotInfo
    {
        public Vector2 pos;
        public uint strength;
    }

    struct DeprimusSnapshotInfo
    {
        public Vector2 pos;
        public NovaDeprimus.Type type;
        public NovaDeprimus.Type impersonated;
        public NovaDeprimus.Type actualType;
    }

    bool crizatusCanvasActive;
    bool nervosusCanvasActive;

    Vector2 ballPos;
    float ballSpeed;
    Color ballColor;
    Vector3 ballTarget;
    bool ballHasTarget;
    Vector2 ballBaseVelocity;

    Vector2 playerPos;
    Color playerColor;

    List<BrickSnapshotInfo> brickMasterBricks;
    List<Vector2> brickMasterFreePositions;

    List<NovaDeprimusMaster.EffectInfo> deprimusMasterEffects;
    List<DeprimusSnapshotInfo> deprimusMasterDeprimuses;
    Dictionary<NovaDeprimus.Type, int> deprimusMasterCounters;

    List<Color> chromaticusMasterColors;

    float backgroundRainbowClock;

    Vector3 cameraPosition;
    float cameraZRotation;

    bool novaModeMasterActive;
    float novaModeMasterClock;

    float timerClock;

    NovaDifficulty difficulty;

    public bool exists = false;

    public void Snap(NovaDeprimus emitter)
    {
        deprimusMasterEffects = new List<NovaDeprimusMaster.EffectInfo>();
        for (int i = 0; i < NovaGame.deprimusMaster.effects.Count; ++i)
        {
            deprimusMasterEffects.Add(NovaGame.deprimusMaster.effects[i].Clone());
        }

        deprimusMasterDeprimuses = new List<DeprimusSnapshotInfo>();
        for (int i = 0; i < NovaGame.deprimusMaster.deprimuses.Count; ++i)
        {
            NovaDeprimus deprimus = NovaGame.deprimusMaster.deprimuses[i];

            // Don't respawn the original Nostalgicus when the snapshot is restored.
            if (deprimus == emitter || deprimus.transform == null)
                continue;

            DeprimusSnapshotInfo info;
            info.pos = deprimus.transform.position;
            info.type = deprimus.type;
            info.impersonated = deprimus.impersonated;
            info.actualType = deprimus.actualType;

            deprimusMasterDeprimuses.Add(info);
        }
        deprimusMasterCounters = new Dictionary<NovaDeprimus.Type, int>(NovaGame.deprimusMaster.counters);

        crizatusCanvasActive = NovaGame.crizatusCanvas.activeSelf;
        nervosusCanvasActive = NovaGame.nervosusCanvas.activeSelf;

        ballPos = NovaGame.ball.transform.position;
        ballSpeed = NovaGame.ball.speed;
        ballColor = NovaGame.ball.color;
        ballTarget = NovaGame.ball.target;
        ballHasTarget = NovaGame.ball.hasTarget;
        ballBaseVelocity = NovaGame.ball.baseVelocity;

        playerPos = NovaGame.player.transform.position;
        playerColor = NovaGame.player.renderer.material.color;

        brickMasterBricks = new List<BrickSnapshotInfo>();
        for (int i = 0; i < NovaGame.brickMaster.bricks.Count; ++i)
        {
            NovaBrick brick = NovaGame.brickMaster.bricks[i];

            BrickSnapshotInfo info;
            info.pos = brick.transform.position;
            info.strength = brick.strength;

            brickMasterBricks.Add(info);
        }
        brickMasterFreePositions = new List<Vector2>(NovaGame.brickMaster.freePositions);

        chromaticusMasterColors = new List<Color>(NovaGame.chromaticusMaster.colors);

        backgroundRainbowClock = NovaGame.backgroundRainbow.clock;

        cameraPosition = NovaGame.camera.transform.localPosition;
        cameraZRotation = NovaGame.camera.zRotation;

        novaModeMasterActive = NovaGame.novaModeMaster.active;
        novaModeMasterClock = NovaGame.novaModeMaster.clock;

        timerClock = NovaGame.timer.clock;

        difficulty = NovaGame.difficulty.Clone();

        exists = true;
    }

    public void Shot()
    {
        NovaGame.deprimusMaster.OnReset();

        NovaGame.deprimusMaster.effects = new List<NovaDeprimusMaster.EffectInfo>();
        for (int i = 0; i < deprimusMasterEffects.Count; ++i)
        {
            NovaGame.deprimusMaster.effects.Add(deprimusMasterEffects[i].Clone());
        }

        for (int i = 0; i < deprimusMasterDeprimuses.Count; ++i)
        {
            DeprimusSnapshotInfo info = deprimusMasterDeprimuses[i];
            NovaGame.deprimusMaster.SpawnOfTypeAt(info.type, info.impersonated, info.actualType, info.pos, false);
        }

        NovaGame.deprimusMaster.counters = new Dictionary<NovaDeprimus.Type, int>(deprimusMasterCounters);

        NovaGame.crizatusCanvas.SetActive(crizatusCanvasActive);
        NovaGame.nervosusCanvas.SetActive(nervosusCanvasActive);

        NovaGame.ball.transform.position = ballPos;
        NovaGame.ball.speed = ballSpeed;
        NovaGame.ball.color = ballColor;
        NovaGame.ball.target = ballTarget;
        NovaGame.ball.hasTarget = ballHasTarget;
        NovaGame.ball.baseVelocity = ballBaseVelocity;

        NovaGame.player.transform.position = playerPos;
        NovaGame.player.renderer.material.color = playerColor;

        NovaGame.brickMaster.Clear();

        for (int i = 0; i < brickMasterBricks.Count; ++i)
        {
            BrickSnapshotInfo info = brickMasterBricks[i];
            NovaGame.brickMaster.CreateBrick(info.pos, info.strength);
        }
        NovaGame.brickMaster.freePositions = new List<Vector2>(brickMasterFreePositions);

        NovaGame.chromaticusMaster.colors.Clear();
        NovaGame.chromaticusMaster.ClearHotspots();

        for(int i = 0; i < chromaticusMasterColors.Count; ++i)
        {
            NovaGame.chromaticusMaster.AddColor(chromaticusMasterColors[i]);
        }

        NovaGame.backgroundRainbow.clock = backgroundRainbowClock;

        NovaGame.camera.transform.localPosition = cameraPosition;
        NovaGame.camera.zRotation = cameraZRotation;

        NovaGame.novaModeMaster.OnReset();
        NovaGame.novaModeMaster.active = novaModeMasterActive;
        NovaGame.novaModeMaster.clock = novaModeMasterClock;

        NovaGame.timer.clock = timerClock;

        NovaGame.difficulty = difficulty.Clone();
    }

    public void OnReset()
    {
        if(exists)
        {
            brickMasterBricks.Clear();
            brickMasterFreePositions.Clear();

            deprimusMasterEffects.Clear();
            deprimusMasterDeprimuses.Clear();
            deprimusMasterCounters.Clear();

            chromaticusMasterColors.Clear();
        }

        exists = false;
    }
}
