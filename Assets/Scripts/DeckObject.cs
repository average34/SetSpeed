using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SetCards.Cards;
using DG.Tweening;

public class DeckObject : MonoBehaviour, IPointerClickHandler
{

    enum Player
    {
        None = 0,
        Player1 = 1,
        Player2,
    }
    enum Color : int
    {
        None = 0,
        Black = 1,
        Red = 2,
    }


    [SerializeField]
    private Player _player = Player.None;


    [SerializeField] private Color _color = Color.None;
    [SerializeField]
    private GameObject _cardObjectPrefab;
    [SerializeField]
    private GameObject _uraObjectPrefab;

    public List<Card> _cardDeckList = new List<Card>();

    //山札+手札+持っているカードの合計枚数
    public int NumberOfCards
    {
        get
        {
            int holdCard = 0;


            //ポインタがカードを持っている状態か

            if (GameManager._pointerEventData != null)
            {
                if (GameManager._pointerEventData.pointerDrag != null)
                {
                    CardObject c = GameManager._pointerEventData.pointerDrag
                        .GetComponent<CardObject>();
                    if (c != null)
                    {
                        holdCard = 1;
                    }
                }
            }

            return _cardDeckList.Count + Tehuda1.transform.childCount +
                Tehuda2.transform.childCount +
                Tehuda3.transform.childCount +
                Tehuda4.transform.childCount +
                holdCard;
        }
    }
    [SerializeField]
    private GameObject Tehuda1;
    [SerializeField]
    private GameObject Tehuda2;
    [SerializeField]
    private GameObject Tehuda3;
    [SerializeField]
    private GameObject Tehuda4;
    [SerializeField]
    private GameObject Daihuda;

    //カードを動かしている最中かどうか
    public bool inDoTween { get; private set; }




    void Awake()
    {
        //ハーフデッキを生成してランダマイズ
        _cardDeckList = DeckFunc.MakeHalfDeck((int)_color);
        _cardDeckList = _cardDeckList.OrderBy(a => Guid.NewGuid()).ToList();
        inDoTween = false;
    }

    void Start()
    {
        StartCoroutine(StartUp());
    }

    // Update is called once per frame
    void Update()
    {
        while (this.transform.childCount < _cardDeckList.Count)
        {
            MakeUraCard(this.transform.childCount + 1);
        }
        while (this.transform.childCount > _cardDeckList.Count)
        {
            foreach (Transform child in transform)
            {
                if (child.GetSiblingIndex() == this.transform.childCount - 1)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            break;
        }

    }


    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {


        //ポインタになにか持っていた状態ならドローしない
        if (eventData.pointerDrag != null)
        {
            Draggable d = eventData.pointerDrag
                .GetComponent<Draggable>();
            if (d != null)
            {
                return;
            }
        }


        PlaceCardFromDeck();


    }

    IEnumerator StartUp()
    {

        int retInt;
        for (int index = 0; index < 4; index++)
        {

            CPUManager.Instance.nowState = CPUManager.State.Drawing;
            do
            {
                retInt = PlaceCardFromDeck();
                yield return null;
            }
            while (retInt != 0);
        }

        yield return null;

        CPUManager.Instance.nowState = CPUManager.State.Placing;

        if (_player == Player.Player1)
        {
            CharaSwitch.Instance.IssenoVoice(1);
        }
        else if (_player == Player.Player2)
        {
            CharaSwitch.Instance.IssenoVoice(2);
        }
        Isseno();
        CPUManager.Instance.nowState = CPUManager.State.None;

        yield return null;
    }


    void MakeUraCard(int CardNumber)
    {

        Vector2 pos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f * (CardNumber - 1));
        // プレハブからインスタンスを生成
        GameObject obj = (GameObject)Instantiate(_uraObjectPrefab, pos, gameObject.transform.rotation);
        // 作成したオブジェクトを子として登録
        obj.transform.SetParent(this.transform);
        obj.transform.localScale = Vector3.one;
    }


    /// <summary>
    /// カードを引いて置くまでの一連のメソッド
    /// </summary>
    public int PlaceCardFromDeck()
    {
        //移動中はドローできない
        if (inDoTween) { return -1; }
        //埋まってたらドローしない
        if (Tehuda1.transform.childCount == 1 &&
            Tehuda2.transform.childCount == 1 &&
            Tehuda3.transform.childCount == 1 &&
            Tehuda4.transform.childCount == 1
            ) { return -1; }


        //カードをドロー
        GameObject Draw = DrawCard();
        if (Draw == null) { return -1; }

        if (Tehuda1.transform.childCount == 0)
        {

            SetParentFromDeck(Draw, Tehuda1); 
        }
        else if (Tehuda2.transform.childCount == 0)
        {
            SetParentFromDeck(Draw, Tehuda2);
        }
        else if (Tehuda3.transform.childCount == 0)
        {
            SetParentFromDeck(Draw, Tehuda3);
        }
        else if (Tehuda4.transform.childCount == 0)
        {
            SetParentFromDeck(Draw, Tehuda4);
        }
        else
        {
            throw new Exception("ドローしたあとに置く場所がありません");
        }
        //成功
        return 0;
    }


    /// <summary>
    /// 上からカードを引く操作
    /// </summary>
    /// <returns>引いたカードのGameObject</returns>
    GameObject DrawCard()
    {
        GameObject obj;
        if (_cardDeckList.Count == 0) return null;


        Vector2 pos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        // プレハブからインスタンスを生成
        obj = (GameObject)Instantiate(_cardObjectPrefab, pos, gameObject.transform.rotation);

        // 作成したオブジェクトをCanvasの子として登録
        obj.transform.SetParent(this.transform.parent);
        obj.transform.localScale = Vector3.one;

        //2PならばDraggableを無効化
        if (_player == Player.Player2) {

            var Draggable = obj.GetComponentInChildren<Draggable>();
            if (Draggable != null) Draggable.enabled = false;
        }


        //カードを取得
        obj.GetComponent<CardObject>().CardInstantiate(_cardDeckList[0].ToString());
        //カードをデッキから削除
        _cardDeckList.Remove(_cardDeckList[0]);

        return obj;
    }

    /// <summary>
    /// デッキから引いたカードを手札に置くメソッド
    /// </summary>
    /// <param name="Draw">引くカード</param>
    /// <param name="Tehuda">カードを置くPanel</param>
    void SetParentFromDeck(GameObject Draw, GameObject Tehuda)
    {
        inDoTween = true;

        AudioManager.Instance.PlaySE("トランプ・引く02", 4);

        if(_player == Player.Player2) { CPUManager.Instance.nowState = CPUManager.State.Drawing; }

        Draw.transform.DOMove(Tehuda.transform.position, 0.3f)
            .OnComplete(() =>
            {
                if (_player == Player.Player2) { CPUManager.Instance.nowState = CPUManager.State.None; }
                Draw.transform.SetParent(Tehuda.transform);
                inDoTween = false;
            });
        return;
    }

    /// <summary>
    /// デッキから台札にカードを置くメソッド
    /// </summary>
    /// <param name="Draw"></param>
    public void SetBahudaFromDeck(GameObject Draw)
    {

        inDoTween = true;
        AudioManager.Instance.PlaySE("トランプ・引く01", 4);

        //カードの裏のみ表示
        foreach (Transform child in Draw.transform)
        {
            if (child.name == "Omote")
            {
                child.GetComponent<Image>().enabled = false;
            }
        }

        if (_player == Player.Player2) { CPUManager.Instance.nowState = CPUManager.State.Isseno; }
        //台札までカードを動かす
        Draw.transform.DOMove(Daihuda.transform.position, 1.0f)
            .OnComplete(() => StartCoroutine(SetBahudaCoroutine(Draw)));
        return;
    }

    IEnumerator SetBahudaCoroutine(GameObject Draw)
    {
        foreach (Transform child in Daihuda.transform)
        {
            Destroy(child.gameObject);
        }
        yield return null;

        Draw.transform.SetParent(Daihuda.transform);

        if (_player == Player.Player2) { CPUManager.Instance.nowState = CPUManager.State.None; }

        AudioManager.Instance.PlaySE("トランプ・配る・出す01", 4);
        inDoTween = false;

        foreach (Transform child in Draw.transform)
        {
            if (child.name == "Omote")
            {
                child.GetComponent<Image>().enabled = true;
            }
        }

    }


    /// <summary>
    /// ゲーム停止時に「いっせーの」でカードを出してゲームを再開させる関数。
    /// </summary>
    /// <param name="ChoosedCard">山札切れの場合に、台札に出すカード</param>
    public void Isseno(GameObject ChoosedCard = null)
    {

        //カードをドロー
        GameObject Draw = DrawCard();

        if (Draw != null)
        {
            //台札にカードを置く
            SetBahudaFromDeck(Draw);
        }
        else
        {
            if (ChoosedCard == null)
            {
                throw new NullReferenceException("山札切れの状態にもかかわらず台札に出すカードが選択されていません。");
            }

            //台札にカードを置く
            SetBahudaFromDeck(ChoosedCard);
        }

        //CPUの時間をリセット
        CPUManager.Instance._PlaceDT = DateTime.Now;
        CPUManager.Instance._DrawDT = DateTime.Now;

    }


}
