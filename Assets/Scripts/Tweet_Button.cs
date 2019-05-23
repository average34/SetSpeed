using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class Tweet_Button : MonoBehaviour, IPointerClickHandler
{
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        string message =
        this.transform.parent.GetComponent<ResultObject>().TweetText();
        
        Application.OpenURL(url: "http://twitter.com/intent/tweet?text=" + UnityWebRequest.EscapeURL(message));

    }
}
