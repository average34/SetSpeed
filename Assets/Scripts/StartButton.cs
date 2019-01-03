using System;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour {



    [SerializeField]
    private GameObject DeckObject_1P;
    [SerializeField]
    private GameObject DeckObject_2P;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }


    public void OnGameStart()
    {
        //状態をゲームプレイ時に
        GameManager.Instance.nowStatus = GameManager.Status.Playing;
        GameManager.Instance._startTime = DateTime.Now;
        CPUManager.Instance.nowState = CPUManager.State.None;

        //ドラッグ可
        DraggableRemover.Instance.EnableAll();

        //デッキ出現とともに開始
        DeckObject_1P.SetActive(true);
        DeckObject_2P.SetActive(true);

        //このオブジェクトは非表示に
        this.gameObject.SetActive(false);
    }
}
