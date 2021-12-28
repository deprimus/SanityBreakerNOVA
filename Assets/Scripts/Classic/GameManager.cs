using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Ball ball;
    public static Player player;
    private static new Camera camera;

    public static GameObject crizatusCanvas;
    public static GameObject nervosusCanvas;

    void Start()
    {
        ball = FindObjectOfType<Ball>();
        player = FindObjectOfType<Player>();
        camera = FindObjectOfType<Camera>();

        SetupWalls();

        crizatusCanvas = GameObject.FindGameObjectsWithTag("CrizatusCanvas")[0];
        nervosusCanvas = GameObject.FindGameObjectsWithTag("NervosusCanvas")[0];

        crizatusCanvas.SetActive(false);
        nervosusCanvas.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0))
            Reset();
        else if(Input.GetKeyDown(KeyCode.Escape))
            OnExit();
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                player.isAI = !player.isAI;
                SoundManager.Play(SoundManager.Clip.SELECTION);
            }
        }
    }

    void SetupWalls()
    {
        GameObject leftWall = GameObject.FindGameObjectsWithTag("WallLeft")[0];
        GameObject rightWall = GameObject.FindGameObjectsWithTag("WallRight")[0];
        GameObject topWall = GameObject.FindGameObjectsWithTag("WallTop")[0];
        GameObject bottomWall = GameObject.FindGameObjectsWithTag("WallBottom")[0];

        Transform transform = leftWall.GetComponent<Transform>();
        SpriteRenderer renderer = leftWall.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(-camera.orthographicSize * camera.aspect - renderer.bounds.size.x / 2, transform.position.y, transform.position.z);

        transform = rightWall.GetComponent<Transform>();
        renderer = rightWall.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(camera.orthographicSize * camera.aspect + renderer.bounds.size.x / 2, transform.position.y, transform.position.z);

        transform = topWall.GetComponent<Transform>();
        renderer = topWall.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(transform.position.x, camera.orthographicSize + renderer.bounds.size.y / 2, transform.position.z);

        transform = bottomWall.GetComponent<Transform>();
        renderer = bottomWall.GetComponent<SpriteRenderer>();

        transform.position = new Vector3(transform.position.x, -camera.orthographicSize - renderer.bounds.size.y / 2, transform.position.z);
    }

    public static void Reset()
    {
        SoundManager.Play(SoundManager.Clip.RESET);

        BrickManager.Reset();
        DeprimusManager.Reset();

        ball.Reset();
        player.Reset();

        crizatusCanvas.SetActive(false);
        nervosusCanvas.SetActive(false);
    }

    public static void OnExit()
    {
        Time.timeScale = float.Epsilon;

        Tale.Transition("fade", Tale.TransitionType.OUT, 0.5f);
        Tale.Scene("Scenes/Menu");
        Tale.Exec(() => {
            Time.timeScale = 1f;
        });

        SoundManager.Play(SoundManager.Clip.RESET);
    }
}
