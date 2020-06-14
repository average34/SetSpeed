using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
#if UNITY_WEBGL
using RpgAtsumaruApiForUnity;
#endif
using System.Threading.Tasks;

public class Tweet_Button : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Twitterに投稿
    /// </summary>
    /// <param name="eventData"></param>
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        string message =
        this.transform.parent.GetComponent<ResultObject>().TweetText();

#if UNITY_WEBGL
        // もしプラグインの初期化が終わっていないなら
        if (!RpgAtsumaruApi.Initialized)
        {
            // プラグインの初期化
            RpgAtsumaruApi.Initialize();
        }
        //初期化されたならアツマール
        if (RpgAtsumaruApi.Initialized)
        {
            //URLを開く
            Task<(bool isError, string message)> urlTask =
                RpgAtsumaruApi.GeneralApi.OpenLinkAsync("http://twitter.com/intent/tweet?text=" + UnityWebRequest.EscapeURL(message));
        }
#endif

#if !UNITY_WEBGL
        
        Application.OpenURL(url: "http://twitter.com/intent/tweet?text=" + UnityWebRequest.EscapeURL(message));
#endif

    }
}
