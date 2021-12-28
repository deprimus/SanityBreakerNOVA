using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaGame : MonoBehaviour
{
    public GameObject crizatusCanvasObject;
    public GameObject nervosusCanvasObject;

    public GameObject leftWallObject;
    public GameObject rightWallObject;
    public GameObject topWallObject;
    public GameObject bottomWallObject;

    public NovaBall ballObject;
    public NovaPlayer playerObject;
    public NovaBrickMaster brickMasterObject;
    public NovaDeprimusMaster deprimusMasterObject;
    public NovaChromaticusMaster chromaticusMasterObject;
    public NovaBackgroundRainbow backgroundRainbowObject;
    public NovaCamera cameraObject;
    public GameObject aiCanvasObject;
    public NovaModeMaster novaModeMasterObject;

    public NovaTimer timerObject;

    public static GameObject crizatusCanvas;
    public static GameObject nervosusCanvas;

    public static GameObject leftWall;
    public static GameObject rightWall;
    public static GameObject topWall;
    public static GameObject bottomWall;

    public static NovaBall ball;
    public static NovaPlayer player;
    public static NovaBrickMaster brickMaster;
    public static NovaDeprimusMaster deprimusMaster;
    public static NovaChromaticusMaster chromaticusMaster;
    public static NovaBackgroundRainbow backgroundRainbow;
    public static new NovaCamera camera;
    public static GameObject aiCanvas;
    public static NovaModeMaster novaModeMaster;
    public static NovaSnapshot snapshot;

    public static NovaTimer timer;

    public static NovaDifficulty difficultyType = NovaDifficulty.DIFFICULTIES["Regular"];
    public static NovaDifficulty difficulty = difficultyType.Clone();

    public static bool active = false;

    void Awake()
    {
        active = true;

        crizatusCanvas = crizatusCanvasObject;
        nervosusCanvas = nervosusCanvasObject;

        leftWall   = leftWallObject;
        rightWall  = rightWallObject;
        topWall    = topWallObject;
        bottomWall = bottomWallObject;

        ball              = ballObject;
        player            = playerObject;
        brickMaster       = brickMasterObject;
        deprimusMaster    = deprimusMasterObject;
        chromaticusMaster = chromaticusMasterObject;
        backgroundRainbow = backgroundRainbowObject;
        camera            = cameraObject;
        aiCanvas          = aiCanvasObject;
        novaModeMaster    = novaModeMasterObject;
        snapshot          = new NovaSnapshot();

        timer             = timerObject;

        SetupWalls();

        crizatusCanvas.SetActive(false);
        nervosusCanvas.SetActive(false);
        aiCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0))
        {
            OnReset();
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit();
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                player.isAI = !player.isAI;
                aiCanvas.SetActive(player.isAI);
                NovaSoundMaster.Play(NovaSoundMaster.Clip.SELECTION);
            }
        }
    }

    void SetupWalls()
    {
        Transform transform = leftWall.GetComponent<Transform>();
        SpriteRenderer renderer = leftWall.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(-Camera.main.orthographicSize * Camera.main.aspect - renderer.bounds.size.x / 2, transform.position.y, transform.position.z);

        transform = rightWall.GetComponent<Transform>();
        renderer = rightWall.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(Camera.main.orthographicSize * Camera.main.aspect + renderer.bounds.size.x / 2, transform.position.y, transform.position.z);

        transform = topWall.GetComponent<Transform>();
        renderer = topWall.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(transform.position.x, Camera.main.orthographicSize + renderer.bounds.size.y / 2, transform.position.z);

        transform = bottomWall.GetComponent<Transform>();
        renderer = bottomWall.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(transform.position.x, -Camera.main.orthographicSize - renderer.bounds.size.y / 2, transform.position.z);
    }

    public static void SetDifficulty(NovaDifficulty type)
    {
        difficultyType = type;
        difficulty = difficultyType.Clone();
    }

    public static float GetShakeMagnitude()
    {
        // CRIZATUS, NERVOSUS, HLIZATUS, WHEEZUS
        float[] MAGNITUDES = { 0.05f, 0.25f, 0.5f, 0.8f};

        int[] counts = {
            deprimusMaster.counters[NovaDeprimus.Type.CRIZATUS],
            deprimusMaster.counters[NovaDeprimus.Type.NERVOSUS],
            deprimusMaster.counters[NovaDeprimus.Type.HLIZATUS],
            deprimusMaster.counters[NovaDeprimus.Type.WHEEZUS] };

        if (!difficulty.stackShakes)
        {
            for(int i = 0; i < counts.Length; ++i)
                counts[i] = Mathf.Min(1, counts[i]);
        }

        float mag = 0;

        for (int i = 0; i < counts.Length; ++i)
            mag += MAGNITUDES[i] * counts[i] * difficulty.shakeMultiplier;

        return mag;
    }

    public static bool BricksAreIndestructible()
    {
        return difficulty.indestructibleBricksForChaoticBall &&
            (deprimusMaster.counters[NovaDeprimus.Type.CURIOSUS]   > 0 ||
             deprimusMaster.counters[NovaDeprimus.Type.ORDINARIUS] > 0 ||
             deprimusMaster.counters[NovaDeprimus.Type.DEPRIMUWUS] > 0 ||
             deprimusMaster.counters[NovaDeprimus.Type.PLANSUS]    > 0);
    }

    public static bool BallShouldAdjustVelocity()
    {
        return (Mathf.Abs(ball.rigidBody.velocity.x) < 0.5f || Mathf.Abs(ball.rigidBody.velocity.y) < 0.5f);
    }

    public static bool BallShouldChangeVelocity()
    {
        return (difficulty.ballVelocityChangeChance > UnityEngine.Random.Range(0, 100) / 100f);
    }

    public static bool BallShouldChase()
    {
        return deprimusMaster.counters[NovaDeprimus.Type.CURIOSUS] > 0;
    }

    public static bool BallShouldFlee()
    {
        return deprimusMaster.counters[NovaDeprimus.Type.ORDINARIUS] > 0;
    }

    public static bool PlayerIsDeadly()
    {
        return deprimusMaster.counters[NovaDeprimus.Type.DISGUSTUS] > 0;
    }

    public static bool PlayerIsConfused()
    {
        return deprimusMaster.counters[NovaDeprimus.Type.CONFUSCIUS] > 0;
    }

    public static bool PlayerIsAttractive()
    {
        return deprimusMaster.counters[NovaDeprimus.Type.DEPRIMUWUS] > 0;
    }

    public static void OnTick()
    {
        ball.OnTick();
        deprimusMaster.OnTick();
        chromaticusMaster.OnTick();
    }

    public static void OnWin()
    {
        deprimusMaster.OnReset();
        Tale.Scene("Menu");
    }

    public static void OnReset()
    {
        if (!active)
            return;

        NovaSoundMaster.Play(NovaSoundMaster.Clip.RESET);

        brickMaster.OnReset();
        deprimusMaster.OnReset();
        ball.OnReset();
        camera.OnReset();
        novaModeMaster.OnReset();
        chromaticusMaster.OnReset();
        difficulty = difficultyType.Clone();
        snapshot.OnReset();
    }

    public static void OnExit()
    {
        active = false;

        Time.timeScale = float.Epsilon;

        Tale.Transition("fade", Tale.TransitionType.OUT, 0.5f);
        Tale.Scene("Scenes/Menu");
        Tale.Exec(() => {
            Time.timeScale = 1f;
        });

        NovaSoundMaster.Play(NovaSoundMaster.Clip.RESET);
    }
}
