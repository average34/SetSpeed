using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, 
    IDragHandler, IBeginDragHandler, IEndDragHandler

{

    private GameObject draggingObject;

    public Transform ReturnToParent = null;

    // ドラッグ前の位置
    private Vector2 prevPos;


    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        CreateDragObject();

        AudioManager.Instance.PlaySE("トランプ・配る・出す02", 4);
        // ドラッグ前の位置を記憶しておく
        prevPos = transform.position;

        ReturnToParent = transform.parent;
        transform.SetParent(transform.parent.parent);




        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ドラッグ中は位置を更新する
        transform.position = eventData.position;
        GameManager._pointerEventData = eventData;


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(OnEndDragCoroutine());


    }



    // ドラッグオブジェクト作成
    private void CreateDragObject()
    {
        draggingObject = new GameObject("Dragging Object");
        draggingObject.transform.SetParent(this.transform.parent);
        draggingObject.transform.SetAsLastSibling();
        draggingObject.transform.localScale = Vector3.one;

        // レイキャストがブロックされないように
        CanvasGroup canvasGroup = draggingObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        
    }

    IEnumerator OnEndDragCoroutine()
    {
        //ドラッグ元に置いたオブジェクトを削除
        Destroy(draggingObject);
        // 1フレーム待機
        yield return null;
        // ドラッグ前の位置に戻す
        transform.position = prevPos;
        transform.SetParent(ReturnToParent);

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

}
