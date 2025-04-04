using Main;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyDropDown : Dropdown
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        GSC.audioController.PlaySound2D("Click");
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        GSC.audioController.PlaySound2D("Click");
    }

    public override void OnCancel(BaseEventData eventData)
    {
        base.OnCancel(eventData);
        GSC.audioController.PlaySound2D("Click");
    }
}
