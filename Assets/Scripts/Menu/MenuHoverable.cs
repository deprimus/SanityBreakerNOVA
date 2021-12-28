using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuHoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;
    private string content;
    private MenuDifficulty menuDifficulty;

    void Awake()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        OnTextChange();

        menuDifficulty = GetComponent<MenuDifficulty>();
    }

    void Update()
    {

    }

    public void OnTextChange()
    {
        content = text.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.text = "> " + content + " <";
        NovaSoundMaster.Play(NovaSoundMaster.Clip.HOVER);

        if(menuDifficulty)
        {
            menuDifficulty.OnHover();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.text = content;

        if (menuDifficulty)
        {
            menuDifficulty.OnUnhover();
        }
    }
}
