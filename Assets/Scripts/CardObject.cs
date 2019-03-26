using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SetCards.Cards;

public class CardObject : MonoBehaviour
{
    [SerializeField] string _SpriteName;
    public Card ThisCard;


    private void Update()
    {
        if (ThisCard == null || _SpriteName == null)
        {
            //画像名からカード名を取得
            foreach (Transform tf in gameObject.transform)
            {

                if (tf.name != "Omote") { continue; }
                var img = tf.GetComponents<Image>();
                foreach (var sp in img)
                {
                    ThisCard = null;
                    ThisCard = new Card(sp.sprite.name);
                    _SpriteName = ThisCard.ToString();
                }
            }
        }
        else if(ThisCard.ToString() != _SpriteName)
        {
            CardInstantiate(_SpriteName);
        }

    }

    /// <summary>
    /// カード名からオブジェクト生成・画像変更・
    /// </summary>
    /// <param name="inputStr"></param>
    public void CardInstantiate(string inputStr)
    {
        foreach (Transform tf in gameObject.transform)
        {

            if (tf.name != "Omote") { continue; }
            var img = tf.GetComponents<Image>();
            foreach (var sp in img)
            {

                ThisCard = null;
                ThisCard = new Card(inputStr);
                _SpriteName = ThisCard.ToString();

                string SpriteName2 = _SpriteName;
                if (SpriteName2 == "B" || SpriteName2 == "R") { SpriteName2 += "0"; }
                sp.sprite = Resources.Load<Sprite>(SpriteName2);
            }
        }

    }

    //カードのGameObjectにアタッチしたScriptに記述
    //右回転用
    public IEnumerator CardOpen()
    {
        //カードを予め-180度回転させ裏面用の画像を表示する
        //裏面表示はコルーチン外で行っても良い
        //CanvasGroupでなくspriteのalpha値を操作しても良い
        transform.eulerAngles = new Vector3(0, 180, 0);

        //ReversedFace_CanvasGroup.alpha = 1f;

        float angle = -180f;
        float Speed = 300f;

        //-90度を超えるまで回転
        while (angle < -90f)
        {
            angle += Speed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0, angle, 0);
            yield return null;
        }

        //裏面用の画像を非表示(表面が表示される)
        //ReversedFace_CanvasGroup.alpha = 0f;

        //0度まで回転
        while (angle < 0f)
        {
            angle += Speed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0, angle, 0);
            yield return null;
        }

        //綺麗に0度にならないことがあるため、補正
        transform.eulerAngles = new Vector3(0, 0, 0);
    }
    
}
