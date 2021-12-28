using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuDifficulty : MonoBehaviour, IPointerClickHandler
{
    public MenuMaster menuMaster;
    public string difficulty;

    public void OnPointerClick(PointerEventData eventData)
    {
        NovaGame.SetDifficulty(NovaDifficulty.DIFFICULTIES[difficulty]);

        menuMaster.OnSceneExit();

        Tale.Transition("fade", Tale.TransitionType.OUT, 1f);
        Tale.Scene("Game");
        Tale.Transition("fade", Tale.TransitionType.IN, 1f);

        NovaSoundMaster.Play(NovaSoundMaster.Clip.CLICK2);
    }

    public void OnHover()
    {
        menuMaster.difficultyDescription.gameObject.SetActive(true);
        menuMaster.difficultyOverviewContainer.SetActive(true);

        NovaDifficulty diff = NovaDifficulty.DIFFICULTIES[difficulty];

        menuMaster.difficultyOverview.text = difficulty;
        menuMaster.difficultyDescription.text = diff.description;
        menuMaster.difficultyBrief.text = diff.GetBriefText();
    }

    public void OnUnhover()
    {
        menuMaster.difficultyDescription.gameObject.gameObject.SetActive(false);
        menuMaster.difficultyOverviewContainer.SetActive(false);

        menuMaster.difficultyOverview.text = "";
        menuMaster.difficultyDescription.text = "";
        menuMaster.difficultyBrief.text = "";
    }
}
