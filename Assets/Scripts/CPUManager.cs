using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using SetCards.Cards;
using DG.Tweening;

public class CPUManager : SingletonMonoBehaviour<CPUManager>
{


    public enum Player
    {
        None = 0,
        Player1 = 1,
        Player2,
    }

    public enum CPULevel
    {
        None = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Infinity = 99,
    }

    public enum State
    {
        Prepareing = -1,
        None = 0,
        Placing = 1,
        Drawing = 2,
        Isseno = 3,

    }

    public CPULevel _level; //= CPULevel.Infinity;

    public Player _player = Player.None;

    public DateTime _PlaceDT;
    public DateTime _DrawDT;
    private TimeSpan _PlaceTS;
    private TimeSpan _DrawTS;
    public State nowState = State.Prepareing;


    // Use this for initialization
    void Start()
    {
        _level = TitleManager._level;


        _PlaceDT = DateTime.Now;
        _DrawDT = DateTime.Now;

        switch (_level)
        {
            //レベル1
            case CPULevel.Level1:
                _PlaceTS = new TimeSpan(0, 0, 5);
                _DrawTS = new TimeSpan(0, 0, 2);
                break;
            //レベル2
            case CPULevel.Level2:
                _PlaceTS = new TimeSpan(0, 0, 3);
                _DrawTS = new TimeSpan(0, 0, 2);
                break;
            //レベル3
            case CPULevel.Level3:
                _PlaceTS = new TimeSpan(0, 0, 2);
                _DrawTS = new TimeSpan(0, 0, 1);
                break;
            //レベル4
            case CPULevel.Level4:
                _PlaceTS = new TimeSpan(0, 0, 0, 1, 100);
                _DrawTS = new TimeSpan(0, 0, 0, 0, 400);
                break;
            //レベル99
            case CPULevel.Infinity:
                _PlaceTS = new TimeSpan(0, 0, 0, 0, 400);
                _DrawTS = new TimeSpan(0, 0, 0, 0, 300);
                break;
            default:
                _PlaceTS = new TimeSpan(0, 0, 3);
                _DrawTS = new TimeSpan(0, 0, 2);
                //_PlaceTS = new TimeSpan(0, 0, 0, 0, 500);
                //_DrawTS = new TimeSpan(0, 0, 0, 0, 300);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

        //プレイ中でなければ動作しない
        if (GameManager.Instance.nowStatus != GameManager.Status.Playing)
        {
            nowState = State.Prepareing;
            return;
        }
        //台座に何もなければ動作しない
        if (
        null == GameManager.Instance.Daihuda_1P.GetComponentInChildren<CardObject>() ||
        null == GameManager.Instance.Daihuda_2P.GetComponentInChildren<CardObject>())
        { return; }


        if (!GameManager.Instance.Okeru_2P) { return; }

        //1秒ごとドロー判定
        if (DateTime.Now - _DrawDT >= _DrawTS)
        {

            _DrawDT = DateTime.Now;
            if (nowState == State.None)
            {
                GameManager.Instance.DeckObject_2P.GetComponent<DeckObject>().PlaceCardFromDeck();
            }
        }
        //2秒ごとPlace判定
        if (DateTime.Now - _PlaceDT >= _PlaceTS)
        {
            _PlaceDT = DateTime.Now;
            if (nowState == State.None)
            {
                Place();
            }
        }


    }




    void Place()
    {

        CardObject c1;
        CardObject c2;
        CardObject c3;
        CardObject c4;
        CardObject d1;
        CardObject d2;
        bool placed = false;
        bool[] puttableArray = new bool[8];

        c1 = GameManager.Instance.Tehuda1_2P.GetComponentInChildren<CardObject>();
        c2 = GameManager.Instance.Tehuda2_2P.GetComponentInChildren<CardObject>();
        c3 = GameManager.Instance.Tehuda3_2P.GetComponentInChildren<CardObject>();
        c4 = GameManager.Instance.Tehuda4_2P.GetComponentInChildren<CardObject>();
        d1 = GameManager.Instance.Daihuda_1P.GetComponentInChildren<CardObject>();
        d2 = GameManager.Instance.Daihuda_2P.GetComponentInChildren<CardObject>();

        //台札になにもなければ実行しない
        if (d1 == null || d2 == null) return;

        System.Random
            r = new System.Random();

        puttableArray[0] = isPlacable(c1, d1);
        puttableArray[1] = isPlacable(c2, d1);
        puttableArray[2] = isPlacable(c3, d1);
        puttableArray[3] = isPlacable(c4, d1);

        puttableArray[4] = isPlacable(c1, d2);
        puttableArray[5] = isPlacable(c2, d2);
        puttableArray[6] = isPlacable(c3, d2);
        puttableArray[7] = isPlacable(c4, d2);

        //全要素がfalseなら実行しない
        if (puttableArray.All(value => value == false)) { return; }


        do
        {
            switch (r.Next(8))
            {
                case 0:
                    placed = isPlacable(c1, d1);
                    if (placed)
                        PlaceDaihudaFromTehuda(GameManager.Instance.Tehuda1_2P, GameManager.Instance.Daihuda_1P);
                    break;

                case 1:
                    placed = isPlacable(c2, d1);
                    if (placed)
                        PlaceDaihudaFromTehuda(GameManager.Instance.Tehuda2_2P, GameManager.Instance.Daihuda_1P);
                    break;

                case 2:
                    placed = isPlacable(c3, d1);
                    if (placed)
                        PlaceDaihudaFromTehuda(GameManager.Instance.Tehuda3_2P, GameManager.Instance.Daihuda_1P);
                    break;

                case 3:
                    placed = isPlacable(c4, d1);
                    if (placed)
                        PlaceDaihudaFromTehuda(GameManager.Instance.Tehuda4_2P, GameManager.Instance.Daihuda_1P);
                    break;

                case 4:
                    placed = isPlacable(c1, d2);
                    if (placed)
                        PlaceDaihudaFromTehuda(GameManager.Instance.Tehuda1_2P, GameManager.Instance.Daihuda_2P);
                    break;

                case 5:
                    placed = isPlacable(c2, d2);
                    if (placed)
                        PlaceDaihudaFromTehuda(GameManager.Instance.Tehuda2_2P, GameManager.Instance.Daihuda_2P);
                    break;

                case 6:
                    placed = isPlacable(c3, d2);
                    if (placed)
                        PlaceDaihudaFromTehuda(GameManager.Instance.Tehuda3_2P, GameManager.Instance.Daihuda_2P);
                    break;

                case 7:
                    placed = isPlacable(c4, d2);
                    if (placed)
                        PlaceDaihudaFromTehuda(GameManager.Instance.Tehuda4_2P, GameManager.Instance.Daihuda_2P);
                    break;
            }
        }
        while (!placed);


    }


    bool isPlacable(CardObject c, CardObject d)
    {
        if (c == null || d == null) return false;

        //カードが包含関係にあればTrue
        if ((c.ThisCard).Contains(d.ThisCard)) { return true; }
        else if ((d.ThisCard).Contains(c.ThisCard)) { return true; }

        return false;
    }


    /// <summary>
    /// 手札から台札にカードを置くメソッド
    /// </summary>
    /// <param name="Tehuda"></param>
    /// <param name="Daihuda"></param>
    void PlaceDaihudaFromTehuda(GameObject Tehuda, GameObject Daihuda)
    {

        AudioManager.Instance.PlaySE("トランプ・引く02", 4);

        //子オブジェクトを取得
        foreach (Transform child in Tehuda.transform)
        {
            //初期位置を取得
            Vector2 prevPos = child.transform.position;

            //状態を更新
            nowState = State.Placing;

            //台札までカードを動かす
            child.transform.DOMove(Daihuda.transform.position, 0.3f)
                .OnComplete(() =>
                {
                    var c = child.GetComponent<CardObject>();
                    var d = Daihuda.GetComponentInChildren<CardObject>();

                    nowState = State.None;
                    if (!isPlacable(c, d))
                    {

                        child.transform.position = prevPos;
                        child.transform.SetParent(Tehuda.transform);
                        return;
                    }

                    Destroy(d.gameObject);

                    child.transform.SetParent(Daihuda.transform);


                    AudioManager.Instance.PlaySE("トランプ・配る・出す01", 4);

                });
        }


        return;
    }

    /// <summary>
    /// 指定時間中Stateを変更するコルーチン
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="_state"></param>
    /// <returns></returns>
    public IEnumerator WaitForState(float delay, State _state)
    {
        nowState = _state;
        yield return new WaitForSeconds(delay);
        nowState = State.None;
        yield return null;
    }


}
