using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPlayBack : MonoBehaviour, IPointerClickHandler
{
    public Transform parentTransform;
    public Transform siblingTransform;

    public void OnPointerClick(PointerEventData eventData)
    {
        Tale.Multiplex(
            Tale.Transform.Position(parentTransform, Screen.width, Tale.Default.FLOAT, 0.5f, Tale.Interpolation.EASE_OUT),
            Tale.Transform.Position(siblingTransform, 0, Tale.Default.FLOAT, 0.5f, Tale.Interpolation.EASE_OUT)
        );

        NovaSoundMaster.Play(NovaSoundMaster.Clip.CLICK1);
    }
}
