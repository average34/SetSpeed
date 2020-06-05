using System;
using UnityEngine;
using UnityEngine.UI;

public class ResultObject : MonoBehaviour
{

    public enum Rank
    {
        None = 0,
        Chuusotsu,
        Kousotsu,
        Rouninsei,
        Daisotsu,
        Master,
        Docter,
        Jokyou,
        Junkyouju,
        Kyouju,
        FieldsMedal, //フィールズ賞

    }

    public enum Shouhai
    {
        Lose = -1,
        Draw,
        Win,
    }


    int Maisuu_1P;
    int Maisuu_2P;
    TimeSpan ts;
    [SerializeField] int _rest = default;
    Shouhai _shouhai;
    Rank _rank;

    // Use this for initialization
    void Start()
    {

        Maisuu_1P = GameManager.Instance.DeckObject_1P.GetComponent<DeckObject>().NumberOfCards;
        Maisuu_2P = GameManager.Instance.DeckObject_2P.GetComponent<DeckObject>().NumberOfCards;


        ts = (GameManager.Instance._finishTime - GameManager.Instance._startTime);
        _rest = Math.Max(Maisuu_1P, Maisuu_2P);

        //勝敗判定
        if (Maisuu_1P == Maisuu_2P)
            _shouhai = Shouhai.Draw;
        else if (Maisuu_1P < Maisuu_2P)
            _shouhai = Shouhai.Win;
        else if (Maisuu_1P > Maisuu_2P)
            _shouhai = Shouhai.Lose;
        else
            _shouhai = Shouhai.Draw;



        //子オブジェクトのそれぞれについて記入
        foreach (Transform child in gameObject.transform)
        {
            switch (child.name)
            {

                case "立ち絵":
                    var _tachie = child.GetComponent<Image>();


                    if (_shouhai == Shouhai.Draw)
                        CharaSwitch.Instance.SetTachieToImg(1, CharaSwitch.TachieState.Draw, _tachie);
                    else if (_shouhai == Shouhai.Win)
                        CharaSwitch.Instance.SetTachieToImg(1, CharaSwitch.TachieState.Win, _tachie);
                    else if (_shouhai == Shouhai.Lose)
                        CharaSwitch.Instance.SetTachieToImg(1, CharaSwitch.TachieState.Lose, _tachie);
                    else
                        CharaSwitch.Instance.SetTachieToImg(1, CharaSwitch.TachieState.Draw, _tachie);


                    break;

                case "勝敗":
                    var _shouhaiText = child.GetComponent<Text>();

                    if (_shouhai == Shouhai.Draw)
                        _shouhaiText.text = "引き分け";
                    else if (_shouhai == Shouhai.Win)
                        _shouhaiText.text = "勝ち";
                    else if (_shouhai == Shouhai.Lose)
                        _shouhaiText.text = "負け";

                    else
                        _shouhaiText.text = "ノーコンテスト";

                    break;

                case "Level":

                    var _levelText = child.GetComponent<Text>();
                    _levelText.text = LevelToText();

                    break;
                case "ResultTime":
                    var _timeText = child.GetComponent<Text>();
                    _timeText.text = TimeSpanToText();
                    break;

                case "NumberOfRest":
                    
                    var _restText = child.GetComponent<Text>();
                    _restText.text = _rest.ToString() + " 枚";

                    break;


                case "Rank":

                    var _rankText = child.GetComponent<Text>();
                    _rankText.text = RankToText();

                    break;
                case "RankingButton":

                    //勝った場合でなければ非表示
                    if (_shouhai != Shouhai.Win) child.gameObject.SetActive(false);
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    string TimeSpanToText()
    {
        return (Math.Round(ts.TotalSeconds, 2, MidpointRounding.AwayFromZero)).ToString() + " 秒";
    }

    string LevelToText()
    {
        switch (CPUManager.Instance._level)
        {
            //レベル1
            case CPUManager.CPULevel.Level1:
                return "かんたん";
            //レベル2
            case CPUManager.CPULevel.Level2:
                return "ふつう";
            //レベル3
            case CPUManager.CPULevel.Level3:
                return "むずい";
            //レベル4
            case CPUManager.CPULevel.Level4:
                return "最強";
            //レベル99
            case CPUManager.CPULevel.Infinity:
                return "∞";
            default:
                return "テスト";
        }
    }

    string RankToText()
    {
        _rank = Ranking();
        switch(_rank)
        {
            case Rank.Chuusotsu:
                return "中卒級";
            case Rank.Kousotsu:
                return "高卒級";
            case Rank.Rouninsei:
                return "浪人生級";
            case Rank.Daisotsu:
                return "大卒級";
            case Rank.Master:
                return "修士級";
            case Rank.Docter:
                return "博士級";
            case Rank.Jokyou:
                return "助教級";
            case Rank.Junkyouju:
                return "准教授級";
            case Rank.Kyouju:
                return "教授級";
            case Rank.FieldsMedal:
                return "フィールズ賞";
            default:
                return "テスト級";

        }

    }

    Rank Ranking()
    {
        //・中卒級
        //イタコに16枚差以上で負ける

        //・高卒級
        //イタコに負ける
        //ずん子に8枚差以上で負ける

        //・浪人生級
        //イタコに引き分ける〜4枚差で勝つ
        //ずん子・きりたん・茜に負ける
        //ずん子に引き分ける

        //・大卒級
        //イタコに5枚差以上で勝つ
        //ずん子に〜60秒で勝つ
        //きりたんに〜70秒で勝つ
        //きりたん・茜に引き分ける

        //・修士級
        //ずん子に60秒〜40秒で勝つ
        //きりたんに70秒〜60秒で勝つ
        //茜に〜70秒で勝つ

        //・博士級
        //ずん子に40秒以内で勝つ
        //きりたんに60秒〜55秒で勝つ
        //茜に70秒〜60秒で勝つ

        //・助教級
        //きりたんに55秒〜50秒で勝つ
        //茜に60秒〜55秒で勝つ

        //・准教授級
        //きりたんに50秒〜45秒で勝つ
        //茜に55秒〜50秒で勝つ

        //・教授級
        //きりたんに45秒以内で勝つ
        //茜に50秒〜30秒で勝つ

        //・フィールズ賞
        //茜に30秒以内で勝つ


        switch (CPUManager.Instance._level)
        {
            case CPUManager.CPULevel.Level1:
                switch (_shouhai)
                {
                    case Shouhai.Lose:
                        if (_rest >= 16) return Rank.Chuusotsu;
                        else return Rank.Kousotsu;
                    case Shouhai.Draw:
                        return Rank.Rouninsei;
                    case Shouhai.Win:
                        if (_rest >= 5)
                            return Rank.Daisotsu;
                        else return Rank.Rouninsei;
                    default:
                        return Rank.None;
                }
            case CPUManager.CPULevel.Level2:
                switch (_shouhai)
                {
                    case Shouhai.Lose:
                        if (_rest >= 8) return Rank.Kousotsu;
                        else return Rank.Rouninsei;
                    case Shouhai.Draw:
                        return Rank.Rouninsei;
                    case Shouhai.Win:
                        if (ts >= new TimeSpan(0,0,60))
                            return Rank.Daisotsu;
                        else if (ts >= new TimeSpan(0, 0, 40))
                            return Rank.Master;
                        else return Rank.Docter;
                    default:
                        return Rank.None;
                }
            case CPUManager.CPULevel.Level3:
                switch (_shouhai)
                {
                    case Shouhai.Lose:
                        return Rank.Rouninsei;
                    case Shouhai.Draw:
                        return Rank.Daisotsu;
                    case Shouhai.Win:
                        if (ts >= new TimeSpan(0, 0, 70))
                            return Rank.Daisotsu;
                        else if (ts >= new TimeSpan(0, 0, 60))
                            return Rank.Master;
                        else if (ts >= new TimeSpan(0, 0, 55))
                            return Rank.Docter;
                        else if (ts >= new TimeSpan(0, 0, 50))
                            return Rank.Jokyou;
                        else if (ts >= new TimeSpan(0, 0, 45))
                            return Rank.Junkyouju;
                        else
                            return Rank.Kyouju;
                    default:
                        return Rank.None;
                }
            case CPUManager.CPULevel.Level4:
                switch (_shouhai)
                {
                    case Shouhai.Lose:
                        return Rank.Rouninsei;
                    case Shouhai.Draw:
                        return Rank.Daisotsu;
                    case Shouhai.Win:
                        if (ts >= new TimeSpan(0, 0, 70))
                            return Rank.Master;
                        else if (ts >= new TimeSpan(0, 0, 60))
                            return Rank.Docter;
                        else if (ts >= new TimeSpan(0, 0, 55))
                            return Rank.Jokyou;
                        else if (ts >= new TimeSpan(0, 0, 50))
                            return Rank.Junkyouju;
                        else if (ts >= new TimeSpan(0, 0, 30))
                            return Rank.Kyouju;
                        else
                            return Rank.FieldsMedal;
                    default:
                        return Rank.None;
                }


                //break;
        }


        return Rank.Daisotsu;
    }


    public string TweetText()
    {
        string _tweetText;
        _tweetText = "【集合スピード】";

        switch (CPUManager.Instance._level)
        {
            //レベル1
            case CPUManager.CPULevel.Level1:
                _tweetText += "東北イタコ(かんたん)";
                break;
            //レベル2
            case CPUManager.CPULevel.Level2:
                _tweetText += "東北ずん子(ふつう)";
                break;
            //レベル3
            case CPUManager.CPULevel.Level3:
                _tweetText += "東北きりたん(むずい)";
                break;
            //レベル4
            case CPUManager.CPULevel.Level4:
                _tweetText += "琴葉茜(最強)";
                break;
            //レベル99
            case CPUManager.CPULevel.Infinity:
                _tweetText += "デバッグ(おかしい)";
                break;
            default:
                return "テスト";
        }
        _tweetText += "に";

        if (_shouhai == Shouhai.Draw)
            _tweetText += "引き分けました。";
        else if (_shouhai == Shouhai.Win)
            _tweetText += "勝ちました！";
        else if (_shouhai == Shouhai.Lose)
            _tweetText += "負けました……。";

        _tweetText += "あなたは" + RankToText() + "です。";


        _tweetText += "記録:" + TimeSpanToText() + "・" + _rest.ToString() + "枚差";
        _tweetText += " #集合スピード ";
        _tweetText += "https://average34.itch.io/setspeed";






        return _tweetText;



    }

    /// <summary>
    /// ランキング登録
    /// </summary>
    public void RankingRegister()
    {
        //
        if (null == ts) return;



        var id = (int)CPUManager.Instance._level - 1;
        if (id < 0 || id >= 4) return;

        //データランキング送信
        if (_shouhai == Shouhai.Win)
        {
            GetComponent<NCMBScore>().TimeScoreUpload(ts, id);
        }
    }

}
