using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    
    
    public void OnDrop(PointerEventData pointerEventData)
    {
        Draggable d = pointerEventData.pointerDrag
            .GetComponent<Draggable>();
        if (d != null)
        {
            d.ReturnToParent = this.transform;
        }
    }
}
