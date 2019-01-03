using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SetCards.Cards;

public class KasaneDropZone : MonoBehaviour, IDropHandler, IPointerUpHandler
{



    void Update()
    {
        var d = this.GetComponentInChildren<Draggable>();
        if (d != null)
        {
            d.enabled = false;
        }

        if (transform.childCount >= 2)
        {
            GameObject go = transform.GetChild(0).gameObject;
            if (go != null) Debug.Log("台札に2つ以上の子オブジェクトがあります。");
            Destroy(go);
        }
    }

    public void OnDrop(PointerEventData pointerEventData)
    {
        GameObject PointerObject = pointerEventData.pointerDrag;


        Draggable d = pointerEventData.pointerDrag
            .GetComponent<Draggable>();
        if (d != null)
        {

            var c = this.GetComponentInChildren<CardObject>();
            if (c != null)
            {
                Card c1 = c.ThisCard;
                Card c2 = PointerObject.GetComponentInChildren<CardObject>().ThisCard;
                if (c1.Contains(c2) || c2.Contains(c1))
                {
                    Destroy(c.gameObject);
                    d.ReturnToParent = this.transform;
                    AudioManager.Instance.PlaySE("トランプ・配る・出す01", 4);
                }
                else
                {
                    System.Random r = new System.Random();

                    //0以上10未満の乱数を整数で返す
                    switch (r.Next(4))
                    {
                        case 0:
                            AudioManager.Instance.PlaySE("だめだよ", 3);
                            break;
                        case 1:
                            AudioManager.Instance.PlaySE("だめです", 3);
                            break;
                        case 2:
                            AudioManager.Instance.PlaySE("置けないよ", 3);
                            break;
                        case 3:
                            AudioManager.Instance.PlaySE("置けません", 3);
                            break;
                    }
                }

            }
            else
            {

                d.ReturnToParent = this.transform;
                AudioManager.Instance.PlaySE("トランプ・配る・出す01", 4);
            }


        }
        else
        {
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        Draggable d = eventData.pointerDrag
            .GetComponent<Draggable>();
        if (d != null)
        {


        }

    }
}
