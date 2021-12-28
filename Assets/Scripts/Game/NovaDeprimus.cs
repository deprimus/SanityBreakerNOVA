using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaDeprimus : MonoBehaviour
{
    public enum Type
    {
        // GEN1
        DEPRIMUS,
        AGILLIUS,
        CRIZATUS,
        DILIUS,
        CONFUSCIUS,
        INGINIUS,
        FOOLUS,
        ORDINARIUS,
        CURIOSUS,
        DISGUSTUS,
        CHANSUS,
        GHINIONUS,
        FERICITUS,
        OBSEDATUS,
        HLIZATUS,
        NERVOSUS,
        // GEN2
        WHEEZUS,
        GENIUS,
        FACADUS,
        CHROMATICUS,
        TRAUMATIZATUS,
        GREEDUS,
        NOSTALGICUS,
        TRISTUS,
        DEPRIMUWUS,
        NEGATUS,
        PLANSUS,
        // OTHER
        OMNIDEPRIMUS
    }

    private enum State
    {
        TRANSITION_IN,
        CONSUMABLE,
        TRANSITION_OUT
    }

    private new SpriteRenderer renderer;
    private Animator animator;
    private State state;
    private bool ballInside;

    public new Transform transform;

    public Type type;
    public Type impersonated;
    public Type actualType;
    private float clock;

    public void Construct(Type type) => Construct(type, type, type);
    public void Construct(Type type, Type impersonated) => Construct(type, impersonated, type);
    public void Construct(Type type, Type impersonated, Type actualType)
    {
        this.type = type;
        this.impersonated = impersonated;
        this.actualType = actualType;
    }

    void Start()
    {
        transform = GetComponent<Transform>();
        renderer = GetComponent<SpriteRenderer>();

        if(type == Type.OMNIDEPRIMUS)
        {
            impersonated = RandomOmnideprimusType();
            actualType = Type.OMNIDEPRIMUS;
            Polymorph(impersonated);
        }
        else if(type == Type.FACADUS)
        {
            // Magic value: if impersonated is set to type, it wasn't initialized properly in Construct(). Otherwise, it was.
            // This is used by NovaSnapshot to avoid reinitialization (after Shot() all Facadus deprimuses should retain their impersonated type).
            if(impersonated == type)
            {
                impersonated = Helper.ProducedOrDefault<Type>(
                    5,
                    NovaGame.deprimusMaster.GenerateTypeUnweighted,
                    (dprType) => dprType != Type.FACADUS && dprType != Type.OMNIDEPRIMUS,
                    Type.DEPRIMUS);
            }

            // This is the actual effect that Facadus will have.
            if(actualType == type)
            {
                actualType = Helper.ProducedOrDefault<Type>(
                    5,
                    NovaGame.deprimusMaster.GenerateType,
                    (dprType) => dprType != Type.FACADUS && dprType != Type.OMNIDEPRIMUS && dprType != impersonated,
                    Type.DEPRIMUS);
            }

            Polymorph(impersonated);
        }
        else
        {
            Polymorph(type);
        }

        animator = GetComponent<Animator>();

        state = State.TRANSITION_IN;
        ballInside = false;
        clock = 0f;
    }

    void Update()
    {
        if(type == Type.OMNIDEPRIMUS)
        {
            clock += Time.deltaTime;

            while (clock >= NovaGame.difficulty.omnideprimusPolymorphDelay)
            {
                impersonated = NextOmnideprimusType();
                Polymorph(impersonated);

                clock -= NovaGame.difficulty.omnideprimusPolymorphDelay;
            }
        }

        switch (state)
        {
            case State.TRANSITION_IN:
            {
                AnimatorStateInfo animatorInfo = animator.GetCurrentAnimatorStateInfo(0);

                // Transitioning in, animation finished
                if(animatorInfo.IsName("NovaDeprimusIn") && animatorInfo.normalizedTime >= 1f)
                {
                    state = State.CONSUMABLE;

                    // Ball entered the deprimus before the state was consumable, so consume now.
                    if(ballInside)
                    {
                        Trigger();
                    }
                }

                break;
            }
            case State.TRANSITION_OUT:
            {
                AnimatorStateInfo animatorInfo = animator.GetCurrentAnimatorStateInfo(0);

                // Transitioning out, animation finished
                if (animatorInfo.IsName("NovaDeprimusOut") && animatorInfo.normalizedTime >= 1f)
                {
                    Destroy(gameObject);
                }

                break;
            }
        }
    }

    public void Polymorph(Type type)
    {
        renderer.sprite = Resources.Load<Sprite>("Sprites/Deprimus/" + type.ToString()[0] + type.ToString().Substring(1).ToLower());
    }

    public bool IsConsumable()
    {
        return state == State.CONSUMABLE;
    }

    public bool IsConsumableOrSpawning()
    {
        return state != State.TRANSITION_OUT;
    }

    public void Trigger()
    {
        if(type == Type.OMNIDEPRIMUS)
        {
            type = impersonated;
        }
        
        // No 'else' here in order to cover the case omnideprimus -> facadus -> other type
        if(type == Type.FACADUS)
        {
            // Change the appearance to FACADUS and change the type to a random deprimus
            Polymorph(type);

            if(actualType == Type.OMNIDEPRIMUS)
            {
                actualType = Helper.ProducedOrDefault<Type>(
                    5,
                    NovaGame.deprimusMaster.GenerateType,
                    (dprType) => dprType != Type.FACADUS && dprType != Type.OMNIDEPRIMUS && dprType != impersonated,
                    Type.DEPRIMUS);
            }

            type = actualType;

            if(NovaGame.deprimusMaster.counters[Type.CHROMATICUS] > 0)
            {
                NovaGame.novaModeMaster.OnTrigger(this);
            }
        }

        NovaSoundMaster.Play((NovaSoundMaster.Clip) Enum.Parse(typeof(NovaSoundMaster.Clip), type.ToString(), false));
        Consume();
    }

    public void Consume()
    {
        Despawn();
        ApplyEffect();
    }

    public void Despawn()
    {
        state = State.TRANSITION_OUT;

        if(animator != null)
        {
            animator.SetTrigger("Despawn");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ApplyEffect()
    {
        switch (type)
        {
            case Type.DEPRIMUS:
                NovaGame.deprimusMaster.Consume(this, 0f, () => {
                    for (byte i = 0; i < 2; ++i)
                        NovaGame.deprimusMaster.Spawn();
                }, null);
                break;
            case Type.AGILLIUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    NovaGame.ball.speed *= 2.0f;
                    NovaGame.ball.rigidBody.velocity *= 2.0f;
                }, () => {
                    NovaGame.ball.speed /= 2.0f;
                    NovaGame.ball.rigidBody.velocity /= 2.0f;
                });
                break;
            case Type.CRIZATUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    ++NovaGame.deprimusMaster.counters[Type.CRIZATUS];

                    if (NovaGame.deprimusMaster.counters[Type.NERVOSUS] > 0)
                    {
                        NovaSoundMaster.Play(NovaSoundMaster.Clip.ULTRA_NERVOSUS);
                    }

                    if (NovaGame.deprimusMaster.counters[Type.CRIZATUS] == 1)
                        NovaGame.crizatusCanvas.SetActive(true);
                }, () => {
                    --NovaGame.deprimusMaster.counters[Type.CRIZATUS];

                    if (NovaGame.deprimusMaster.counters[Type.CRIZATUS] == 0)
                    {
                        NovaGame.crizatusCanvas.SetActive(false);
                        NovaGame.camera.transform.localPosition = NovaGame.camera.basePos;
                    }
                });
                break;
            case Type.DILIUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    NovaGame.difficulty.deprimusSpawnChance *= 3.0f;
                }, () => {
                    NovaGame.difficulty.deprimusSpawnChance /= 3.0f;
                });
                break;
            case Type.CONFUSCIUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    ++NovaGame.deprimusMaster.counters[Type.CONFUSCIUS];
                }, () => {
                    --NovaGame.deprimusMaster.counters[Type.CONFUSCIUS];
                });
                break;
            case Type.INGINIUS:
                NovaGame.deprimusMaster.Consume(this, 0f, () => {
                    for (byte i = 0; i < 2; ++i)
                        NovaGame.deprimusMaster.SpawnOfType(Type.CHANSUS);
                }, null);
                break;
            case Type.FOOLUS:
                NovaGame.deprimusMaster.Consume(this, 0f, () => {
                    for (byte i = 0; i < 2; ++i)
                        NovaGame.deprimusMaster.SpawnOfType(Type.GHINIONUS);
                }, null);
                break;
            case Type.ORDINARIUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    ++NovaGame.deprimusMaster.counters[Type.ORDINARIUS];
                }, () => {
                    --NovaGame.deprimusMaster.counters[Type.ORDINARIUS];

                    if (NovaGame.deprimusMaster.counters[Type.ORDINARIUS] == 0)
                    {
                        NovaGame.ball.transform.eulerAngles = Vector3.zero;
                        NovaGame.ball.ChangeVelocity();
                    }
                });
                break;
            case Type.CURIOSUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    ++NovaGame.deprimusMaster.counters[Type.CURIOSUS];
                }, () => {
                    --NovaGame.deprimusMaster.counters[Type.CURIOSUS];

                    if (NovaGame.deprimusMaster.counters[Type.CURIOSUS] == 0)
                    {
                        NovaGame.ball.transform.eulerAngles = Vector3.zero;
                        NovaGame.ball.ChangeVelocity();
                    }
                });
                break;
            case Type.DISGUSTUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    ++NovaGame.deprimusMaster.counters[Type.DISGUSTUS];

                    if (NovaGame.deprimusMaster.counters[Type.DISGUSTUS] == 1)
                    {
                        NovaGame.player.renderer.material.color = new Color(128 / 255f, 42 / 255f, 77 / 255f);

                        if(NovaGame.chromaticusMaster.colors.Count > 0)
                        {
                            NovaGame.chromaticusMaster.RescaleHotspots();
                        }
                    }
                }, () => {
                    --NovaGame.deprimusMaster.counters[Type.DISGUSTUS];

                    if (NovaGame.deprimusMaster.counters[Type.DISGUSTUS] == 0)
                    {
                        NovaGame.player.renderer.material.color = new Color(1.0f, 1.0f, 1.0f);

                        if (NovaGame.chromaticusMaster.colors.Count > 0)
                        {
                            NovaGame.chromaticusMaster.RescaleHotspots();
                        }
                    }
                });
                break;
            case Type.CHANSUS:
                NovaGame.deprimusMaster.Consume(this, 0f, () => {
                    int count = UnityEngine.Random.Range(1, 4);

                    while ((count--) > 0) {
                        if (NovaGame.brickMaster.bricks.Count > 0)
                        {
                            NovaGame.brickMaster.bricks[UnityEngine.Random.Range(0, NovaGame.brickMaster.bricks.Count)].OnHit();
                        }
                    }
                }, null);
                break;
            case Type.GHINIONUS:
                NovaGame.deprimusMaster.Consume(this, 0f, () => {
                    int count = UnityEngine.Random.Range(1, 4);

                    List<NovaBrick> bricks = new List<NovaBrick>();

                    for (int i = 0; i < NovaGame.brickMaster.bricks.Count; ++i)
                    {
                        if (NovaGame.brickMaster.bricks[i].strength < NovaBrick.MAX_STRENGTH)
                        {
                            bricks.Add(NovaGame.brickMaster.bricks[i]);
                        }
                    }

                    while ((count--) > 0)
                    {
                        // Strengthen an existing brick
                        if (bricks.Count > 0 && (NovaGame.brickMaster.freePositions.Count == 0 || UnityEngine.Random.Range(0, 100) < 50))
                        {
                            NovaBrick brick = bricks[UnityEngine.Random.Range(0, bricks.Count)];
                            ++brick.strength;
                            brick.OnStrengthChange();

                            if (brick.strength == NovaBrick.MAX_STRENGTH)
                            {
                                bricks.Remove(brick);
                            }

                            continue;
                        }

                        // Recreate a destroyed brick
                        NovaGame.brickMaster.RecreateBrick(UnityEngine.Random.Range(0, NovaGame.brickMaster.freePositions.Count));
                    }
                }, null);
                break;
            case Type.FERICITUS:
                NovaGame.deprimusMaster.Consume(this, 0f, () => NovaGame.deprimusMaster.OnReset(), null);
                NovaGame.novaModeMaster.OnReset();
                NovaGame.camera.OnReset();
                NovaGame.snapshot.OnReset();
                break;
            case Type.HLIZATUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    ++NovaGame.deprimusMaster.counters[Type.HLIZATUS];
                }, () => {
                    --NovaGame.deprimusMaster.counters[Type.HLIZATUS];

                    if (NovaGame.deprimusMaster.counters[Type.HLIZATUS] == 0)
                        NovaGame.camera.transform.localPosition = NovaGame.camera.basePos;
                });
                break;
            case Type.OBSEDATUS:
                {
                    // Try getting a random deprimus type.
                    // If the type is OBSEDATUS 5 times in a row, choose the default (DEPRIMUS).
                    Type deprimusType = Helper.ProducedOrDefault<Type>(
                        5,
                        NovaGame.deprimusMaster.GenerateType,
                        (dprType) => dprType != Type.OBSEDATUS,
                        Type.DEPRIMUS);

                    NovaGame.deprimusMaster.Consume(this, 5f, () => {
                        ++NovaGame.deprimusMaster.counters[Type.OBSEDATUS];

                        NovaGame.difficulty.deprimusSpawnChance *= 5f;
                        NovaGame.difficulty.GetDeprimusInfo(deprimusType).chance *= 100f;
                        NovaGame.difficulty.UpdateChanceSum();
                    }, () => {
                        --NovaGame.deprimusMaster.counters[Type.OBSEDATUS];

                        NovaGame.difficulty.deprimusSpawnChance /= 5f;
                        NovaGame.difficulty.GetDeprimusInfo(deprimusType).chance /= 100f;
                        NovaGame.difficulty.UpdateChanceSum();
                    });
                    break;
                }
            case Type.NERVOSUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    ++NovaGame.deprimusMaster.counters[Type.NERVOSUS];

                    if(NovaGame.deprimusMaster.counters[Type.CRIZATUS] > 0)
                    {
                        NovaSoundMaster.Play(NovaSoundMaster.Clip.ULTRA_NERVOSUS);
                    }

                    if (NovaGame.deprimusMaster.counters[Type.NERVOSUS] == 1)
                        NovaGame.nervosusCanvas.SetActive(true);
                }, () => {
                    --NovaGame.deprimusMaster.counters[Type.NERVOSUS];

                    if (NovaGame.deprimusMaster.counters[Type.NERVOSUS] == 0)
                    {
                        NovaGame.nervosusCanvas.SetActive(false);
                        NovaGame.camera.transform.localPosition = NovaGame.camera.basePos;
                    }
                });
                break;
            case Type.WHEEZUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    ++NovaGame.deprimusMaster.counters[Type.WHEEZUS];
                }, () => {
                    --NovaGame.deprimusMaster.counters[Type.WHEEZUS];

                    if (NovaGame.deprimusMaster.counters[Type.WHEEZUS] == 0)
                        NovaGame.camera.transform.localPosition = NovaGame.camera.basePos;
                });
                break;
            case Type.GENIUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    for (byte i = 0; i < 2; ++i)
                        NovaGame.deprimusMaster.SpawnOfType(Type.INGINIUS);

                    NovaGame.difficulty.GetDeprimusInfo(Type.INGINIUS).chance *= 2;
                    NovaGame.difficulty.GetDeprimusInfo(Type.CHANSUS).chance *= 2;
                    NovaGame.difficulty.UpdateChanceSum();
                }, () => {
                    NovaGame.difficulty.GetDeprimusInfo(Type.INGINIUS).chance /= 2;
                    NovaGame.difficulty.GetDeprimusInfo(Type.CHANSUS).chance /= 2;
                    NovaGame.difficulty.UpdateChanceSum();
                });
                break;
            case Type.CHROMATICUS:
            {
                Delegates.ShallowDelegate onConsume = () => { };
                Delegates.ShallowDelegate onReset   = () => { };

                if (NovaGame.chromaticusMaster.colors.Count < NovaGame.difficulty.maxBallColors || NovaGame.difficulty.maxBallColors == 0)
                {
                    onConsume = () =>
                    {
                        ++NovaGame.deprimusMaster.counters[Type.CHROMATICUS];
                        NovaGame.chromaticusMaster.AddColor();
                    };
                    onReset = () =>
                    {
                        --NovaGame.deprimusMaster.counters[Type.CHROMATICUS];
                        NovaGame.chromaticusMaster.RemoveColor();
                    };
                }

                NovaGame.deprimusMaster.Consume(this, 10f, onConsume, onReset);
                break;
            }
            case Type.TRAUMATIZATUS:
                NovaGame.deprimusMaster.Consume(this, 0f, () => {
                    ++NovaGame.deprimusMaster.counters[Type.TRAUMATIZATUS];

                    if(NovaGame.deprimusMaster.counters[Type.TRAUMATIZATUS] == NovaGame.difficulty.traumatizatusThreshold)
                    {
                        NovaGame.OnReset();
                    }
                    else
                    {
                        NovaGame.snapshot.OnReset();
                    }
                }, null);
                break;
            case Type.GREEDUS:
                NovaGame.deprimusMaster.Consume(this, 0f, () => {
                    List<NovaDeprimus> deprimuses = new List<NovaDeprimus>(NovaGame.deprimusMaster.deprimuses);

                    for(int i = 0; i < deprimuses.Count; ++i)
                    {
                        if(deprimuses[i].IsConsumableOrSpawning() && deprimuses[i].type != Type.GREEDUS)
                            deprimuses[i].Trigger();
                    }
                }, null);
                break;
            case Type.NOSTALGICUS:
                NovaGame.deprimusMaster.Consume(this, 0f, () => {
                    if(!NovaGame.snapshot.exists)
                    {
                        NovaGame.snapshot.Snap(this);
                    }
                    else
                    {
                        NovaGame.snapshot.Shot();
                    }
                }, null);
                break;
            case Type.TRISTUS:
                NovaGame.deprimusMaster.Consume(this, 0f, () => {
                    NovaGame.camera.Flip();
                }, null);
                break;
            case Type.DEPRIMUWUS:
                NovaGame.deprimusMaster.Consume(this, 5f, () => {
                    ++NovaGame.deprimusMaster.counters[Type.DEPRIMUWUS];
                }, () => {
                    --NovaGame.deprimusMaster.counters[Type.DEPRIMUWUS];
                });
                break;
            case Type.NEGATUS:
                NovaGame.deprimusMaster.Consume(this, 0f, () => {
                    List<NovaDeprimus> deprimuses = new List<NovaDeprimus>(NovaGame.deprimusMaster.deprimuses);

                    for (int i = 0; i < deprimuses.Count; ++i)
                    {
                        NovaDeprimus deprimus = deprimuses[i];

                        if(!deprimus.IsConsumableOrSpawning() || deprimus.transform == null || deprimus == this || deprimus.type == Type.OMNIDEPRIMUS)
                        {
                            continue;
                        }

                        if(deprimus.type == Type.FACADUS)
                        {
                            // Pass the Facadus type for 'impersonated', such that a new random deprimus is chosen as the impersonated type,
                            // but the actual type (the effect) is the same.
                            NovaGame.deprimusMaster.SpawnOfTypeAt(NovaDeprimus.Type.FACADUS, NovaDeprimus.Type.FACADUS, deprimus.actualType, deprimus.transform.position, false);
                        }
                        else
                        {
                            // Pass the deprimus type to the actual type, such that the Facadus deprimus will look like something else,
                            // but the effect will be the one of the original deprimus.
                            NovaGame.deprimusMaster.SpawnOfTypeAt(NovaDeprimus.Type.FACADUS, NovaDeprimus.Type.FACADUS, deprimus.type, deprimus.transform.position, false);
                        }

                        deprimus.Despawn();
                        NovaGame.deprimusMaster.Remove(deprimus);
                    }
                }, null);
                break;
            case Type.PLANSUS:
                NovaGame.deprimusMaster.Consume(this, 10f, () => {
                    ++NovaGame.deprimusMaster.counters[Type.PLANSUS];
                }, () => {
                    --NovaGame.deprimusMaster.counters[Type.PLANSUS];
                });
                break;
        }
    }

    private Type RandomOmnideprimusType()
    {
        Array pool = Enum.GetValues(typeof(Type));
        return (Type) pool.GetValue(UnityEngine.Random.Range(0, pool.Length - 1)); // Exclude omnideprimus.
    }

    private Type NextOmnideprimusType()
    {
        if((int) impersonated == ((int) Type.OMNIDEPRIMUS) - 1)
        {
            return Type.DEPRIMUS;
        }
        else
        {
            return (Type) (((int) impersonated) + 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Ball"))
        {
            if(IsConsumable())
            {
                Trigger();
            }
            else
            {
                ballInside = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Ball"))
        {
            ballInside = false;
        }
    }
}
