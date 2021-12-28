using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuClassic : MonoBehaviour, IPointerClickHandler
{
    public MenuMaster menuMaster;

    public void OnPointerClick(PointerEventData eventData)
    {
        menuMaster.OnSceneExit();

        Tale.Transition("fade", Tale.TransitionType.OUT);
        Tale.Scene("Classic");
        Tale.Transition("fade", Tale.TransitionType.IN);

        NovaSoundMaster.Play(NovaSoundMaster.Clip.CLICK2);
    }
}
