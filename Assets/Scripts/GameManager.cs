using SetCards.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{

    public enum Status
    {
        None = -1,
        Preparing = 0,
        Playing = 1,
        Finished = 2,
        Result = 3,
    }

    public Status nowStatus = Status.None;


    public bool Okeru_1P = false;
    public bool Okeru_2P = false;
    private bool Ready_1P = false;
    private bool Ready_2P = false;

    public GameObject Tehuda1_1P;
    public GameObject Tehuda2_1P;
    public GameObject Tehuda3_1P;
    public GameObject Tehuda4_1P;


    public GameObject Tehuda1_2P;
    public GameObject Tehuda2_2P;
    public GameObject Tehuda3_2P;
    public GameObject Tehuda4_2P;

    public GameObject Daihuda_1P;
    public GameObject Daihuda_2P;

    
    public GameObject DeckObject_1P;
    public GameObject DeckObject_2P;

    

    [SerializeField]
    private GameObject ChoiceNotice;
    [SerializeField]
    private GameObject CannotPutNotice;
    [SerializeField]
    private GameObject ResultObject;
    [SerializeField]
    private GameObject Sokomade;



    private GameObject Tehuda1;
    private GameObject Tehuda2;
    private GameObject Tehuda3;
    private GameObject Tehuda4;
    private GameObject DeckObject;

    private DateTime dt;
    private DateTime dt2;
    public DateTime _startTime;
    public DateTime _finishTime;

    public bool _isseno { get; private set; }
    public bool inReady { get; private set; }

    //山札切れ・手詰まりのときに選んだカード
    public GameObject ChoosedCard_1P { private get; set; }
    public GameObject ChoosedCard_2P { private get; set; }

    //ドラッグ中のイベントを取得
    public static PointerEventData _pointerEventData;


    // Use this for initialization
    void Start()
    {

        //準備中
        nowStatus = Status.Preparing;
        //曲再生
        //midstream jam - musmus
        AudioManager.Instance.PlayBGM("tw061");

    }

    // Update is called once per frame
    void Update()
    {
        //リザルト表示中はキー受付しない
        if (nowStatus != Status.Result)
        {
            //R でリトライ
            if (Input.GetKey(KeyCode.R))
                Retry();
            //T でタイトルへ
            if (Input.GetKey(KeyCode.T))
                ToTitle();
            //Escape でもタイトルへ
            if (Input.GetKey(KeyCode.Escape))
                ToTitle();
        }


        //現在のゲーム状態に応じて条件分岐
        switch (nowStatus)
        {


            //準備時
            case Status.Preparing:
                CharaSwitch.Instance.SetTachie(1, CharaSwitch.TachieState.Normal);
                CharaSwitch.Instance.SetTachie(2, CharaSwitch.TachieState.Normal);
                break;

            //ゲームプレイ時
            case Status.Playing:
                DraggableRemover.Instance.Remove_2P();
                DraggableRemover.Instance.Remove_2P_Raycast();

                //ゲーム中の前提条件
                if (Daihuda_1P.transform.childCount != 0 && Daihuda_2P.transform.childCount != 0)
                {

                    int Number1P = DeckObject_1P.GetComponent<DeckObject>().NumberOfCards;
                    int Number2P = DeckObject_1P.GetComponent<DeckObject>().NumberOfCards;

                    //カードの枚数に合わせて立ち絵を切り替え
                    if(Number1P-Number2P >= 8)
                    {
                        CharaSwitch.Instance.SetTachie(1, CharaSwitch.TachieState.Good);
                        CharaSwitch.Instance.SetTachie(2, CharaSwitch.TachieState.Bad);

                    }
                    else if (Number2P - Number1P >= 8)
                    {
                        CharaSwitch.Instance.SetTachie(1, CharaSwitch.TachieState.Bad);
                        CharaSwitch.Instance.SetTachie(2, CharaSwitch.TachieState.Good);

                    }
                    else
                    {
                        CharaSwitch.Instance.SetTachie(1, CharaSwitch.TachieState.Normal);
                        CharaSwitch.Instance.SetTachie(2, CharaSwitch.TachieState.Normal);

                    }


                    //置けるかどうかの判定を行う
                    Okeru_1P = TsumariJudge(1);
                    Okeru_2P = TsumariJudge(2);
                    

                    //1Pが置けないまま3秒が経過した場合、CannotPutNotice表示をアクティブに
                    if (!Okeru_1P)
                    {

                        if (DateTime.Now - dt2 >= new TimeSpan(0, 0, 3))
                        {
                            if (!inReady && !_isseno)
                            {
                                //Notice表示をアクティブに
                                CannotPutNotice.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        //Notice表示を非アクティブに
                        CannotPutNotice.SetActive(false);

                        dt2 = DateTime.Now;
                    }


                    //両プレイヤーとも置けなくなった場合
                    if (!Okeru_1P && !Okeru_2P)
                    {
                        //置けないまま5秒が経過した場合、準備状態に突入
                        if (DateTime.Now - dt >= new TimeSpan(0, 0, 5) && !_isseno)
                        {
                            //ChoiceNotice表示をアクティブに
                            ChoiceNotice.SetActive(true);
                            //CannotPutNotice表示を非アクティブに
                            CannotPutNotice.SetActive(false);

                            //デッキがなければ手札を選択可能に
                            if (DeckObject_1P.transform.childCount == 0)
                            {

                                foreach (Transform child in Tehuda1_1P.transform)
                                {
                                    var choosable = child.gameObject.AddComponent<Choosable>();
                                    choosable._player = Choosable.Player.Player1;
                                }
                                foreach (Transform child in Tehuda2_1P.transform)
                                {
                                    var choosable = child.gameObject.AddComponent<Choosable>();
                                    choosable._player = Choosable.Player.Player1;
                                }
                                foreach (Transform child in Tehuda3_1P.transform)
                                {
                                    var choosable = child.gameObject.AddComponent<Choosable>();
                                    choosable._player = Choosable.Player.Player1;
                                }
                                foreach (Transform child in Tehuda4_1P.transform)
                                {
                                    var choosable = child.gameObject.AddComponent<Choosable>();
                                    choosable._player = Choosable.Player.Player1;
                                }
                            }
                            if (DeckObject_2P.transform.childCount == 0)
                            {
                                foreach (Transform child in Tehuda1_2P.transform)
                                {
                                    var choosable = child.gameObject.AddComponent<Choosable>();
                                    choosable._player = Choosable.Player.Player2;
                                }
                                foreach (Transform child in Tehuda2_2P.transform)
                                {
                                    var choosable = child.gameObject.AddComponent<Choosable>();
                                    choosable._player = Choosable.Player.Player2;
                                }
                                foreach (Transform child in Tehuda3_2P.transform)
                                {
                                    var choosable = child.gameObject.AddComponent<Choosable>();
                                    choosable._player = Choosable.Player.Player2;
                                }
                                foreach (Transform child in Tehuda4_2P.transform)
                                {
                                    var choosable = child.gameObject.AddComponent<Choosable>();
                                    choosable._player = Choosable.Player.Player2;
                                }
                            }
                            inReady = true;
                            _isseno = true;
                        }
                    }
                    else
                    {
                        dt = DateTime.Now;
                        _isseno = false;
                        inReady = false;
                    }

                    //いっせーの準備状態ならば、
                    if (inReady)
                    {

                        //ChoiceNotice表示をアクティブに
                        ChoiceNotice.SetActive(true);
                        //カードが山札にある・カードが選ばれているならばプレイヤーの準備完了
                        if (DeckObject_1P.transform.childCount > 0) { Ready_1P = true; }
                        else if (ChoosedCard_1P != null) { Ready_1P = true; }
                        else { Ready_1P = false; }

                        //CPUにカードを選ばせる
                        //これは本来ならCPUManagerに入れたい
                        if (DeckObject_2P.transform.childCount == 0 && ChoosedCard_2P == null)
                        {
                            System.Random r = new System.Random();
                            switch (r.Next(4))
                            {
                                case 0:
                                    foreach (Transform child in Tehuda1_2P.transform)
                                    {
                                        ChoosedCard_2P = child.gameObject;
                                    }
                                    break;
                                case 1:
                                    foreach (Transform child in Tehuda2_2P.transform)
                                    {
                                        ChoosedCard_2P = child.gameObject;
                                    }
                                    break;
                                case 2:
                                    foreach (Transform child in Tehuda3_2P.transform)
                                    {
                                        ChoosedCard_2P = child.gameObject;
                                    }
                                    break;
                                case 3:
                                    foreach (Transform child in Tehuda4_2P.transform)
                                    {
                                        ChoosedCard_2P = child.gameObject;
                                    }
                                    break;

                            }


                        }


                        if (DeckObject_2P.transform.childCount > 0) { Ready_2P = true; }
                        else if (ChoosedCard_2P != null) { Ready_2P = true; }
                        else { Ready_2P = false; }




                        //両プレイヤーとも準備完了ならば、いっせーのでカードを出す
                        if (Ready_1P && Ready_2P)
                        {
                            //カードを出す
                            DeckObject_1P.GetComponent<DeckObject>().Isseno(ChoosedCard_1P);
                            DeckObject_2P.GetComponent<DeckObject>().Isseno(ChoosedCard_2P);
                            //オブジェクトは初期化
                            ChoosedCard_1P = null;
                            ChoosedCard_2P = null;

                            //「いっせーの」ボイス
                            //SE再生。AUDIO.SE_BUTTONがSEのファイル名
                            CharaSwitch.Instance.IssenoVoice(1);
                            CharaSwitch.Instance.IssenoVoice(2);


                            //Noticeは非アクティブに
                            ChoiceNotice.SetActive(false);
                            CannotPutNotice.SetActive(false);

                            //全カードを選択不可能に
                            foreach (Transform child in Tehuda1_1P.transform) Destroy(child.GetComponent<Choosable>());
                            foreach (Transform child in Tehuda2_1P.transform) Destroy(child.GetComponent<Choosable>());
                            foreach (Transform child in Tehuda3_1P.transform) Destroy(child.GetComponent<Choosable>());
                            foreach (Transform child in Tehuda4_1P.transform) Destroy(child.GetComponent<Choosable>());
                            foreach (Transform child in Daihuda_1P.transform) Destroy(child.GetComponent<Choosable>());
                            foreach (Transform child in Tehuda1_2P.transform) Destroy(child.GetComponent<Choosable>());
                            foreach (Transform child in Tehuda2_2P.transform) Destroy(child.GetComponent<Choosable>());
                            foreach (Transform child in Tehuda3_2P.transform) Destroy(child.GetComponent<Choosable>());
                            foreach (Transform child in Tehuda4_2P.transform) Destroy(child.GetComponent<Choosable>());
                            foreach (Transform child in Daihuda_2P.transform) Destroy(child.GetComponent<Choosable>());

                            //初期化
                            dt = DateTime.Now;
                            _isseno = false;
                            inReady = false;
                        }


                    }

                    //ゲーム終了判定も行う
                    if (GameOverJudge(1) && GameOverJudge(2))
                    {
                        Finished();

                        Debug.Log("引き分け");
                    }
                    else if (GameOverJudge(1))
                    {
                        Finished();
                        Debug.Log("1Pの勝ち");
                    }
                    else if (GameOverJudge(2))
                    {
                        Finished();
                        Debug.Log("2Pの勝ち");
                    }

                }
                break;

            case Status.Finished:
                DraggableRemover.Instance.RemoveAll();

                break;
            case Status.Result:

                CharaSwitch.Instance.TachieDisable(1);
                CharaSwitch.Instance.TachieDisable(2);
                break;
        }
    }




    bool TsumariJudge(int player)
    {

        CardObject c1;
        CardObject c2;
        CardObject c3;
        CardObject c4;
        CardObject d1;
        CardObject d2;

        //プレイヤーごとに判定（初期化）
        if (player == 1)
        {
            Tehuda1 = Tehuda1_1P;
            Tehuda2 = Tehuda2_1P;
            Tehuda3 = Tehuda3_1P;
            Tehuda4 = Tehuda4_1P;
            DeckObject = DeckObject_1P;
        }
        else if (player == 2)
        {

            Tehuda1 = Tehuda1_2P;
            Tehuda2 = Tehuda2_2P;
            Tehuda3 = Tehuda3_2P;
            Tehuda4 = Tehuda4_2P;
            DeckObject = DeckObject_2P;
        }
        else
        {
            return false;
        }

        if (Daihuda_1P.transform.childCount == 0 || Daihuda_2P.transform.childCount == 0)
        {
            throw new System.Exception("台札にカードがない状態で詰まり判定を起こそうとしました。");
        }



        c1 = Tehuda1.GetComponentInChildren<CardObject>();
        c2 = Tehuda2.GetComponentInChildren<CardObject>();
        c3 = Tehuda3.GetComponentInChildren<CardObject>();
        c4 = Tehuda4.GetComponentInChildren<CardObject>();
        d1 = Daihuda_1P.GetComponentInChildren<CardObject>();
        d2 = Daihuda_2P.GetComponentInChildren<CardObject>();

        int _tehudaChildCount = Tehuda1.transform.childCount + Tehuda2.transform.childCount +
            Tehuda3.transform.childCount + Tehuda4.transform.childCount;

        //山札あり判定モード
        if (DeckObject.transform.childCount > 0)
        {
            //手札に抜けがあればまだ詰まっていない
            if (_tehudaChildCount <= 2)
            {
                return true;
            }
            else if (_tehudaChildCount == 3)
            {

                if (_pointerEventData == null)
                {
                    return true;
                }
                //ポインタになカードを持っていた状態で3枠埋まっていたらそれも含めて判定
                else if (_pointerEventData.pointerDrag != null)
                {
                    CardObject c = _pointerEventData.pointerDrag
                        .GetComponent<CardObject>();
                    if (c == null)
                    {
                        return true;
                    }
                    else
                    {
                        //ここにカード4種が場札に置けないことを判定
                        return PuttableJudge(new CardObject[] { c, c1, c2, c3, c4 }, new CardObject[] { d1, d2 });

                    }
                }



            }
            else
            {

                //ここにカード4種が場札に置けないことを判定
                return PuttableJudge(new CardObject[] { c1, c2, c3, c4 }, new CardObject[] { d1, d2 });

            }
            return true;
        }
        //山札なし判定モード
        else
        {


            if (_pointerEventData == null)
            {

                //ここにカード4種が場札に置けないことを判定
                return PuttableJudge(new CardObject[] { c1, c2, c3, c4 }, new CardObject[] { d1, d2 });

            }
            //ポインタにカードを持っていた状態ならそれも含めて判定
            else if (_pointerEventData.pointerDrag != null)
            {
                CardObject c = _pointerEventData.pointerDrag
                    .GetComponent<CardObject>();
                if (c == null)
                {
                    //ここにカード4種が場札に置けないことを判定
                    return PuttableJudge(new CardObject[] { c1, c2, c3, c4 }, new CardObject[] { d1, d2 });
                }
                else
                {
                    //ここにカード4種が場札に置けないことを判定
                    return PuttableJudge(new CardObject[] { c, c1, c2, c3, c4 }, new CardObject[] { d1, d2 });

                }
            }
            else
            {

                //ここにカード4種が場札に置けないことを判定
                return PuttableJudge(new CardObject[] { c1, c2, c3, c4 }, new CardObject[] { d1, d2 });
            }

        }
    }


    /// <summary>
    /// 手札と台札のカードオブジェクトを与えて、手札が台札におけるかどうかを判定
    /// </summary>
    /// <param name="Cs">手札のカードオブジェクト</param>
    /// <param name="Ds">台札のカードオブジェクト</param>
    /// <returns></returns>
    bool PuttableJudge(IEnumerable<CardObject> Cs, IEnumerable<CardObject> Ds)
    {
        foreach (CardObject c in Cs)
        {
            if (c == null) { continue; }
            foreach (CardObject d in Ds)
            {
                //カードが包含関係にあればTrue
                if ((c.ThisCard).Contains(d.ThisCard)) { return true; }
                else if ((d.ThisCard).Contains(c.ThisCard)) { return true; }
            }
        }
        return false;
    }

    /// <summary>
    /// ゲーム終了条件を満たすかどうかの判定
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    bool GameOverJudge(int player)
    {


        //プレイヤーごとに判定（初期化）
        if (player == 1)
        {
            Tehuda1 = Tehuda1_1P;
            Tehuda2 = Tehuda2_1P;
            Tehuda3 = Tehuda3_1P;
            Tehuda4 = Tehuda4_1P;
            DeckObject = DeckObject_1P;
        }
        else if (player == 2)
        {

            Tehuda1 = Tehuda1_2P;
            Tehuda2 = Tehuda2_2P;
            Tehuda3 = Tehuda3_2P;
            Tehuda4 = Tehuda4_2P;
            DeckObject = DeckObject_2P;
        }
        else
        {
            return false;
        }

        if (Daihuda_1P.transform.childCount == 0 || Daihuda_2P.transform.childCount == 0)
        {
            throw new System.Exception("台札にカードがない状態でゲーム終了判定を起こそうとしました。");
        }


        //デッキにまだカードがあれば終了しない
        if (DeckObject.transform.childCount >= 1)
        {
            return false;
        }
        //デッキでカード移動中であれば終了しない
        else if (DeckObject.GetComponent<DeckObject>().inDoTween)
        {
            return false;
        }

        if (_pointerEventData != null)
        {
            //ポインタになにか持っていた状態なら終了しない
            if (_pointerEventData.pointerDrag != null)
            {
                CardObject c = _pointerEventData.pointerDrag
                    .GetComponent<CardObject>();
                if (c != null)
                {
                    return false;
                }
            }
        }

        int _tehudaChildCount = Tehuda1.transform.childCount + Tehuda2.transform.childCount +
            Tehuda3.transform.childCount + Tehuda4.transform.childCount;
        //手札があればまだ終わっていない
        if (_tehudaChildCount >= 1)
        {
            return false;
        }



        return true;
    }


    /// <summary>
    /// ゲーム終了時に行う処理
    /// </summary>
    void Finished()
    {

        nowStatus = Status.Finished;
        _finishTime = DateTime.Now;


        AudioManager.Instance.PlaySE("そこまで", 3);
        AudioManager.Instance.PlaySE("ホイッスル・連続", 4);

        //そこまで！の表示
        Sokomade.SetActive(true);


        StartCoroutine(ToResult());
    }


    IEnumerator ToResult()
    {

        yield return new WaitForSeconds(3.0f);

        nowStatus = Status.Result;

        //そこまで！の非表示
        Sokomade.SetActive(false);

        //リザルトの表示
        ResultObject.SetActive(true);

        //立ち絵非表示
        CharaSwitch.Instance.TachieDisable(1);
        CharaSwitch.Instance.TachieDisable(2);

        if (GameOverJudge(1) && GameOverJudge(2)) { }
        else if (GameOverJudge(1))
            CharaSwitch.Instance.YouWinVoice(1);
        else if (GameOverJudge(2))
            CharaSwitch.Instance.YouLoseVoice(1);


    }

    /// <summary>
    /// ゲームを再起動するメソッド
    /// </summary>
    public void Retry()
    {

        SceneManager.LoadScene("SetSpeed_v02");
    }

    /// <summary>
    /// タイトルに行くメソッド
    /// </summary>
    public void ToTitle()
    {

        SceneManager.LoadScene("Title");
    }

}
