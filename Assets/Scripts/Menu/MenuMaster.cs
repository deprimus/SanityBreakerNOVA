using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuMaster : MonoBehaviour
{
    public GameObject playModePrefab;
    public RectTransform modesContainerTransform;
    public RectTransform bottomPaddingTransform;
    public Scrollbar menuScrollbar;

    public GameObject difficultyOverviewContainer;
    public TextMeshProUGUI difficultyOverview;
    public TextMeshProUGUI difficultyDescription;
    public TextMeshProUGUI difficultyBrief;

    void Start()
    {
        NovaCustomDifficulty.DeserializeAll();

        foreach(string difficulty in NovaCustomDifficulty.DIFFICULTIES)
        {
            GameObject mode = Instantiate(playModePrefab);

            MenuDifficulty menuDifficulty = mode.GetComponent<MenuDifficulty>();

            menuDifficulty.menuMaster = this;
            menuDifficulty.difficulty = difficulty;

            mode.GetComponent<TextMeshProUGUI>().text = difficulty;
            mode.GetComponent<MenuHoverable>().OnTextChange();

            RectTransform rectTransform = mode.GetComponent<RectTransform>();
            rectTransform.SetParent(modesContainerTransform);
            rectTransform.SetSiblingIndex(bottomPaddingTransform.GetSiblingIndex());
        }

        menuScrollbar.value = 1f;

        Tale.Parallel(Tale.Music.Play("Main", Tale.Music.PlayMode.LOOP, 0.2f));
        Tale.MagicFix();
        Tale.Transition("fade", Tale.TransitionType.IN, 1f);
    }

    public void OnSceneExit()
    {
        Tale.Parallel(Tale.Music.Stop(1f));
    }
}
