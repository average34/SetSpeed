using UnityEngine;
using UnityEngine.EventSystems;

public class Tweet_Button : MonoBehaviour, IPointerClickHandler
{
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        string message =
        this.transform.parent.GetComponent<ResultObject>().TweetText();
        
        Application.OpenURL("http://twitter.com/intent/tweet?text=" + WWW.EscapeURL(message));

    }
}
