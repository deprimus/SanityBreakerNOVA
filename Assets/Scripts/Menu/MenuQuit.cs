using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuQuit : MonoBehaviour, IPointerClickHandler
{
    public MenuMaster menuMaster;

    public void OnPointerClick(PointerEventData eventData)
    {
        menuMaster.OnSceneExit();

        Tale.Transition("fade", Tale.TransitionType.OUT);
        Tale.Exec(() =>
        {
            Application.Quit();
        });

        NovaSoundMaster.Play(NovaSoundMaster.Clip.CLICK2);
    }
}
