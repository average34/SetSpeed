using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaSwitch : SingletonMonoBehaviour<CharaSwitch>
{


    public enum Character
    {
        None = -1,
        Yukari = 0,
        Itako = 1,
        Zunko = 2,
        Kiritan = 3,
        Akane = 4,
    }

    public enum TachieState
    {
        Lose = -2,
        Bad = -1,
        Normal = 0,
        Good = 1,
        Win = 2,
        Draw = 99,
    }


    public Character _chara_1P = Character.Yukari;
    public Character _chara_2P = Character.Itako;

    [SerializeField] private GameObject _tachie_1P;
    [SerializeField] private GameObject _tachie_2P;

    // Use this for initialization
    void Start()
    {
        _chara_1P = TitleManager._chara_1P;
        _chara_2P = TitleManager._chara_2P;

    }

    // Update is called once per frame
    void Update()
    {

    }


    Character Player2Chara(int player)
    {
        Character chara;
        if (player == 1)
        {
            chara = _chara_1P;
        }
        else if (player == 2)
        {
            chara = _chara_2P;
        }
        else
        {
            throw new System.Exception("入力されたplayer値が無効です");
        }
        return chara;

    }

    Image Player2Image(int player)
    {
        Image obj;
        if (player == 1)
        {
            obj = _tachie_1P.GetComponent<Image>();
        }
        else if (player == 2)
        {
            obj = _tachie_2P.GetComponent<Image>();
        }
        else
        {
            throw new System.Exception("入力されたplayer値が無効です");
        }
        return obj;
    }

    /// <summary>
    /// 「いっせーの」のボイスをかけるメソッド。
    /// </summary>
    /// <param name="player">プレイヤー。1:プレイヤー1 , 2:プレイヤー2</param>
    public void IssenoVoice(int player)
    {
        Character chara = Player2Chara(player);

        switch (chara)
        {
            case Character.Yukari:
                AudioManager.Instance.PlaySE("いっせぇぇの_yu", player);
                break;
            case Character.Itako:
                AudioManager.Instance.PlaySE("いっせぇぇの_ita", player);
                break;
            case Character.Zunko:
                AudioManager.Instance.PlaySE("いっせぇぇの_zunko", player);
                break;
            case Character.Kiritan:
                AudioManager.Instance.PlaySE("いっせぇぇの_kiri", player);
                break;
            case Character.Akane:
                AudioManager.Instance.PlaySE("いっせぇぇの_aka", player);
                break;
        }



    }


    public void YouWinVoice(int player)
    {
        Character chara = Player2Chara(player);
        System.Random r = new System.Random();


        switch (chara)
        {
            case Character.Yukari:
                switch (r.Next(3))
                {
                    case 0:
                        AudioManager.Instance.PlaySE("やったあ！", player);
                        break;
                    case 1:
                        AudioManager.Instance.PlaySE("まあゆかりさんは天才ですから？こんなの楽勝ですよ", player);
                        break;
                    case 2:
                        AudioManager.Instance.PlaySE("ゆかりさん、だいしょうりー！", player);
                        break;
                }
                break;
            case Character.Itako:
                AudioManager.Instance.PlaySE("いっせぇぇの_ita", player);
                break;
            case Character.Zunko:
                AudioManager.Instance.PlaySE("いっせぇぇの_zunko", player);
                break;
            case Character.Kiritan:
                AudioManager.Instance.PlaySE("いっせぇぇの_kiri", player);
                break;
            case Character.Akane:
                AudioManager.Instance.PlaySE("いっせぇぇの_aka", player);
                break;
        }

    }


    public void YouLoseVoice(int player)
    {

        Character chara = Player2Chara(player);
        System.Random r = new System.Random();
        switch (chara)
        {
            case Character.Yukari:
                switch (r.Next(2))
                {
                    case 0:
                        AudioManager.Instance.PlaySE("むずすぎやだあああああ", player);
                        break;
                    case 1:
                        AudioManager.Instance.PlaySE("うわーん負けちゃいましたーー", player);
                        break;
                }
                break;
            case Character.Itako:
                AudioManager.Instance.PlaySE("いっせぇぇの_ita", player);
                break;
            case Character.Zunko:
                AudioManager.Instance.PlaySE("いっせぇぇの_zunko", player);
                break;
            case Character.Kiritan:
                AudioManager.Instance.PlaySE("いっせぇぇの_kiri", player);
                break;
            case Character.Akane:
                AudioManager.Instance.PlaySE("いっせぇぇの_aka", player);
                break;
        }
    }

    public void TachieDisable(int player)
    {
        //Character chara = Player2Chara(player);
        Image img = Player2Image(player);
        img.enabled = false;

    }
    

    public void SetTachie(int player, TachieState state)
    {
        Image img = Player2Image(player);
        SetTachieToImg(player, state, img);

    }


    public void SetTachieToImg(int player, TachieState state,Image img)
    {

        img.enabled = true;
        Character chara = Player2Chara(player);
        switch (chara)
        {
            case Character.Yukari:
                switch (state)
                {
                    case TachieState.Normal:
                        img.sprite = Resources.Load<Sprite>("Yuzuki/ゲーム内/通常微笑");
                        break;
                    case TachieState.Win:
                        img.sprite = Resources.Load<Sprite>("Yuzuki/ゲーム内/赤面笑う");
                        break;
                    case TachieState.Lose:
                        img.sprite = Resources.Load<Sprite>("Yuzuki/ゲーム内/赤面泣く");
                        break;
                    case TachieState.Good:
                        img.sprite = Resources.Load<Sprite>("Yuzuki/ゲーム内/赤面どや");
                        break;
                    case TachieState.Bad:
                        img.sprite = Resources.Load<Sprite>("Yuzuki/ゲーム内/通常困る");
                        break;
                    case TachieState.Draw:
                        img.sprite = Resources.Load<Sprite>("Yuzuki/ゲーム内/通常慄く");
                        break;
                    default:
                        img.sprite = Resources.Load<Sprite>("Yuzuki/ゲーム内/通常微笑");
                        break;
                }
                break;
            case Character.Itako:
                switch (state)
                {
                    case TachieState.Normal:
                        img.sprite = Resources.Load<Sprite>("Itako/ゲーム内/微笑");
                        break;
                    case TachieState.Win:
                        img.sprite = Resources.Load<Sprite>("Itako/ゲーム内/笑い");
                        break;
                    case TachieState.Lose:
                        img.sprite = Resources.Load<Sprite>("Itako/ゲーム内/ぐるぐる");
                        break;
                    case TachieState.Good:
                        img.sprite = Resources.Load<Sprite>("Itako/ゲーム内/余裕");
                        break;
                    case TachieState.Bad:
                        img.sprite = Resources.Load<Sprite>("Itako/ゲーム内/ひきつり笑い");
                        break;
                    case TachieState.Draw:
                        img.sprite = Resources.Load<Sprite>("Itako/ゲーム内/驚く");
                        break;
                    default:
                        img.sprite = Resources.Load<Sprite>("Itako/ゲーム内/微笑");
                        break;
                }
                break;
            case Character.Zunko:
                switch (state)
                {
                    case TachieState.Normal:
                        img.sprite = Resources.Load<Sprite>("Zunko/ゲーム内/普通");
                        break;
                    case TachieState.Win:
                        img.sprite = Resources.Load<Sprite>("Zunko/ゲーム内/ウインク笑い");
                        break;
                    case TachieState.Lose:
                        img.sprite = Resources.Load<Sprite>("Zunko/ゲーム内/ギャグやられ");
                        break;
                    case TachieState.Good:
                        img.sprite = Resources.Load<Sprite>("Zunko/ゲーム内/ウインク笑い");
                        break;
                    case TachieState.Bad:
                        img.sprite = Resources.Load<Sprite>("Zunko/ゲーム内/不満");
                        break;
                    case TachieState.Draw:
                        img.sprite = Resources.Load<Sprite>("Zunko/ゲーム内/驚き");
                        break;
                    default:
                        img.sprite = Resources.Load<Sprite>("Zunko/ゲーム内/普通");
                        break;
                }
                break;
            case Character.Kiritan:

                switch (state)
                {
                    case TachieState.Normal:
                        img.sprite = Resources.Load<Sprite>("Kiritan/ゲーム内/微笑");
                        break;
                    case TachieState.Win:
                        img.sprite = Resources.Load<Sprite>("Kiritan/ゲーム内/笑い");
                        break;
                    case TachieState.Lose:
                        img.sprite = Resources.Load<Sprite>("Kiritan/ゲーム内/やられ");
                        break;
                    case TachieState.Good:
                        img.sprite = Resources.Load<Sprite>("Kiritan/ゲーム内/ジト目");
                        break;
                    case TachieState.Bad:
                        img.sprite = Resources.Load<Sprite>("Kiritan/ゲーム内/怒り");
                        break;
                    case TachieState.Draw:
                        img.sprite = Resources.Load<Sprite>("Kiritan/ゲーム内/驚き");
                        break;
                    default:
                        img.sprite = Resources.Load<Sprite>("Kiritan/ゲーム内/微笑");
                        break;
                }
                break;
            case Character.Akane:

                switch (state)
                {
                    case TachieState.Normal:
                        img.sprite = Resources.Load<Sprite>("Akane/ゲーム内/微笑");
                        break;
                    case TachieState.Win:
                        img.sprite = Resources.Load<Sprite>("Akane/ゲーム内/笑い");
                        break;
                    case TachieState.Lose:
                        img.sprite = Resources.Load<Sprite>("Akane/ゲーム内/ギャグやられ");
                        break;
                    case TachieState.Good:
                        img.sprite = Resources.Load<Sprite>("Akane/ゲーム内/ギャグネコ");
                        break;
                    case TachieState.Bad:
                        img.sprite = Resources.Load<Sprite>("Akane/ゲーム内/ギャグ不満");
                        break;
                    case TachieState.Draw:
                        img.sprite = Resources.Load<Sprite>("Akane/ゲーム内/驚き");
                        break;
                    default:
                        img.sprite = Resources.Load<Sprite>("Akane/ゲーム内/微笑");
                        break;
                }
                img.sprite = Resources.Load<Sprite>("Akane/ゲーム内/微笑");
                break;
        }
    }





    }
