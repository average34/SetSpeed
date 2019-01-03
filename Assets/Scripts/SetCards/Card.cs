namespace SetCards.Cards
{
    /// <summary>
    /// 1枚の集合トランプのカードそのものを表すクラス。
    /// 標準的な環境では、{1,2,3,4,5}の部分集合32種×2色、という構成になる。
    /// </summary>
    public class Card
    {

        //=====================================================
        //プロパティ
        //=====================================================
        //自動プロパティにしても良い
        private int _suit;
        /// <summary>
        /// カードのマーク(スート)を保持
        /// 0:スートなし扱い(N) 1:黒(B) 2:赤(R)
        /// </summary>
        public int Suit
        {
            get { return _suit; }
            set { this._suit = value; }
        }

        /// <summary>
        /// カードの数字
        /// null:エラー, "":空集合, "12345":全体集合
        /// </summary>
        private string _number;
        // カードの数字が含まれているかどうか
        private bool _ExistOne;
        private bool _ExistTwo;
        private bool _ExistThree;
        private bool _ExistFour;
        private bool _ExistFive;

        //カードの数字を代入する処理
        public string Number
        {
            get { return _number; }
            set
            {
                if (value == null)
                {
                    this._ExistOne = false;
                    this._ExistTwo = false;
                    this._ExistThree = false;
                    this._ExistFour = false;
                    this._ExistFive = false;
                    this._number = null;
                }
                else
                {

                    this._ExistOne = value.Contains("1");
                    this._ExistTwo = value.Contains("2");
                    this._ExistThree = value.Contains("3");
                    this._ExistFour = value.Contains("4");
                    this._ExistFive = value.Contains("5");
                    this._number = null;
                    
                    if (_ExistOne) { this._number += "1"; }
                    if (_ExistTwo) { this._number += "2"; }
                    if (_ExistThree) { this._number += "3"; }
                    if (_ExistFour) { this._number += "4"; }
                    if (_ExistFive) { this._number += "5"; }

                    //スートの代入
                    if (value.Contains("B")) { this._suit = 1; }
                    else if (value.Contains("R")) { this._suit = 2; }
                    else if (value.Contains("N")) { this._suit = 0; }
                }
            }
        }

        /// <summary>
        ///カードの数字が含まれているかどうかの変数を配列に入れたもの
        /// </summary>
        public bool[] ExistArray
        {
            get
            {
                bool[] Array = new bool[5];
                Array[0] = _ExistOne;
                Array[1] = _ExistTwo;
                Array[2] = _ExistThree;
                Array[3] = _ExistFour;
                Array[4] = _ExistFive;
                return Array;
            }
        }

        //=====================================================
        //コンストラクタ（構築子）
        //=====================================================
        //カード作成(null)
        public Card()
        {
            this.Suit = 0;
            this.Number = null;
        }


        //カード作成
        public Card(int inputSuit, string inputNumber)
        {
            this.Suit = inputSuit;
            this.Number = inputNumber;
        }

        //カード作成2
        public Card(string inputString)
        {

            //スートの代入
            if (inputString.Contains("B")) { this._suit = 1; }
            else if (inputString.Contains("R")) { this._suit = 2; }
            else { this._suit = 0; }
            this.Number = inputString;
        }


        //カード作成3(スートに文字列を入れた場合)
        public Card(string inputStrSuit, string inputNumber)
        {
            if (inputStrSuit == "N") { this.Suit = 0; }
            else if (inputStrSuit == "B") { this.Suit = 1; }
            else if (inputStrSuit == "R") { this.Suit = 2; }
            else { this.Suit = 0; }
            this.Number = inputNumber;
        }


        //=====================================================
        //メソッド
        //=====================================================

        //カードを文字列表記
        public override string ToString()
        {
            string str;
            if (Suit == 0) { str = "N"; }
            else if (Suit == 1) { str = "B"; }
            else if (Suit == 2) { str = "R"; }
            else { return null; }

            str += this.Number;

            return str;
        }
    }
}
