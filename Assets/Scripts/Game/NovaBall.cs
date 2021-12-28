using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaBall : MonoBehaviour
{
    public float speed = 5.0f;
    public Color color;

    public Rigidbody2D rigidBody;
    public new SpriteRenderer renderer;
    public new Transform transform;
    public TrailRenderer trailRenderer;
    public new ParticleSystem particleSystem;

    public Vector3 target;
    public bool hasTarget;
    public Vector2 baseVelocity;

    void Start()
    {
        rigidBody      = GetComponent<Rigidbody2D>();
        renderer       = GetComponent<SpriteRenderer>();
        transform      = GetComponent<Transform>();
        trailRenderer  = GetComponent<TrailRenderer>();
        particleSystem = GetComponent<ParticleSystem>();

        hasTarget = false;

        OnReset();
    }

    void Update()
    {
        trailRenderer.material.color = NovaGame.backgroundRainbow.lastColor.Invert();

        if(NovaGame.PlayerIsAttractive())
        {
            target = NovaGame.player.transform.position;

            if (!NovaGame.PlayerIsDeadly())
            {
                ChaseTarget();
            }
            else
            {
                FleeFromTarget();
            }
        }
        else if(NovaGame.BallShouldChase())
        {
            if(!NovaGame.BallShouldFlee())
            {
                // If the ball is close enough to the target, find a new one.
                if(!hasTarget || MathSET.WithinThreshold(transform.position, target, renderer.bounds.size.x))
                    FindNearestTarget();

                if(hasTarget)
                {
                    ChaseTarget();
                }
            }
        }
        else if(NovaGame.BallShouldFlee())
        {
            FindNearestTarget();

            if(hasTarget)
            {
                FleeFromTarget();
            }
        }
        else
        {
            hasTarget = false;

            if (NovaGame.BallShouldAdjustVelocity())
            {
                ChangeVelocity();
            }

            transform.rotation = Quaternion.identity;
        }
    }

    public void OnTick()
    {
        if (NovaGame.deprimusMaster.counters[NovaDeprimus.Type.PLANSUS] > 0)
        {
            float chance = NovaGame.difficulty.ballTeleportChance * NovaGame.deprimusMaster.counters[NovaDeprimus.Type.PLANSUS];

            if (chance > UnityEngine.Random.Range(0, 100) / 100f)
            {
                // Teleport the ball.
                transform.position = NovaGame.deprimusMaster.RandomPosInBounds();
                NovaSoundMaster.Play(NovaSoundMaster.Clip.TELEPORT);
            }
        }

        if(!hasTarget && NovaGame.BallShouldChangeVelocity())
        {
            ChangeVelocityDownwards();
        }
    }

    void FleeFromTarget()
    {
        TrackTarget(1);
    }

    void ChaseTarget()
    {
        TrackTarget(-1);
    }

    void TrackTarget(int sign)
    {
        Vector2 diff = target - transform.position;
        diff.Normalize();

        transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg + 90);

        rigidBody.velocity = sign * transform.up * speed;
    }

    void FindTarget()
    {
        hasTarget = false;

        if (NovaGame.deprimusMaster.deprimuses.Count > 0)
        {
            Transform dprTransform = NovaGame.deprimusMaster.deprimuses[Random.Range(0, NovaGame.deprimusMaster.deprimuses.Count)].GetComponent<Transform>();

            if(dprTransform != null)
            {
                target = dprTransform.position;
                hasTarget = true;
            }
        }
    }

    void FindNearestTarget()
    {
        hasTarget = false;

        Vector2 minDiff = Vector2.zero;

        foreach(NovaDeprimus deprimus in NovaGame.deprimusMaster.deprimuses)
        {
            if (deprimus.transform != null && deprimus.IsConsumable())
            {
                Vector2 diff = deprimus.transform.position - transform.position;

                if (minDiff == Vector2.zero || minDiff.magnitude > diff.magnitude)
                {
                    minDiff = diff;
                    target = deprimus.transform.position;
                    hasTarget = true;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if (NovaGame.PlayerIsDeadly())
            {
                NovaGame.OnReset();
                return;
            }

            Vector3 direction = other.contacts[0].normal;
            float angle = Vector3.Angle(direction, Vector3.up);

            // Side collision
            if (Mathf.Approximately(angle, 90))
            {
                if (!NovaGame.difficulty.allowPlayerSideCollisions)
                {
                    NovaGame.OnReset();
                    return;
                }
            }
        }

        NovaSoundMaster.Play(NovaSoundMaster.Clip.HIT);
    }

    public void ChangeVelocity()
    {
        baseVelocity = Random.insideUnitCircle.normalized;

        rigidBody.velocity = baseVelocity * speed;
    }

    public void ChangeVelocityDownwards()
    {
        float angle = UnityEngine.Random.Range(271, 360) * Mathf.Deg2Rad;

        baseVelocity = (new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))).normalized;

        rigidBody.velocity = baseVelocity * speed;
    }

    public void ChangeColor(Color color)
    {
        this.color = color;
        renderer.material.color = new Color(color.r, color.g, color.b, color.a);
    }

    public void ResetColor()
    {
        ChangeColor(new Color32(255, 255, 255, 255));
    }

    public void OnReset()
    {
        ResetColor();

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        baseVelocity       = Random.insideUnitCircle.normalized;
        trailRenderer.Clear();

        hasTarget = false;

        rigidBody.velocity = baseVelocity * speed;
    }
}
