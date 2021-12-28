using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitMaster : MonoBehaviour
{
    // Tale uses Awake() to initialize, so use Start() here in order to be able to use Tale.
    void Start()
    {
        Tale.Transition("fade", Tale.TransitionType.OUT, 0f);

        Tale.Scene("Scenes/Splash/Tale");
        Tale.Parallel(Tale.Sound.Play("Tale"));
        Tale.Transition("fade", Tale.TransitionType.IN, 0.75f);
        Tale.Wait(1.5f);
        Tale.Transition("fade", Tale.TransitionType.OUT, 0.75f);

        Tale.Scene("Scenes/Splash/FlashingLights");
        Tale.Transition("fade", Tale.TransitionType.IN, 0.75f);
        Tale.Wait(2f);
        Tale.Transition("fade", Tale.TransitionType.OUT, 0.75f);

        Tale.Scene("Scenes/Menu");
    }
}
