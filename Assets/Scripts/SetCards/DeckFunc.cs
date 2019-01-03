using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetCards.Cards
{


    public static class DeckFunc
    {

        //====================================
        //引数なし
        //====================================

            /// <summary>
            /// デッキを作る関数。
            /// </summary>
            /// <returns></returns>
        public static List<Card> MakeDeck()
        {
            List<Card> Deck = new List<Card>();
            Deck.AddRange(MakeHalfDeck(1));
            Deck.AddRange(MakeHalfDeck(2));
            return Deck;

        }

        /// <summary>
        /// 色の統一された32枚のデッキを作る関数。
        /// </summary>
        /// <param name="Suit">スート。0:色なし 1:黒 2:赤</param>
        /// <returns></returns>
        public static List<Card> MakeHalfDeck(int Suit)
        {
            List<Card> Deck = new List<Card>();
            string Number;
                for (int Exist1 = 0; Exist1 <= 1; Exist1++)
                {

                    for (int Exist2 = 0; Exist2 <= 1; Exist2++)
                    {

                        for (int Exist3 = 0; Exist3 <= 1; Exist3++)
                        {

                            for (int Exist4 = 0; Exist4 <= 1; Exist4++)
                            {

                                for (int Exist5 = 0; Exist5 <= 1; Exist5++)
                                {
                                    if (Suit == 1) { Number = "B"; }
                                    else if (Suit == 2) { Number = "R"; }
                                    else { Number = "N"; }

                                    if (Exist1 == 1) { Number += "1"; }
                                    if (Exist2 == 1) { Number += "2"; }
                                    if (Exist3 == 1) { Number += "3"; }
                                    if (Exist4 == 1) { Number += "4"; }
                                    if (Exist5 == 1) { Number += "5"; }
                                    Deck.Add(new Card(Suit, Number));
                                    Number = "";
                                }
                            }
                        }
                    }
                }
            Deck.Sort(DeckFunc.CompareBy);
            return Deck;
        }


        //========================================
        //順序
        //========================================
        /// <summary>
        /// カード同士を辞書順で比較する
        /// </summary>
        /// <param name="x">カード1</param>
        /// <param name="y">カード2</param>
        /// <returns></returns>
        public static int CompareBy(Card x, Card y)
        {
            //nullが最も小さいとする
            if (x == null && y == null)
            {
                return 0;
            }
            else if (x == null)
            {
                return -1;
            }
            else if (y == null)
            {
                return 1;
            }
            if(x.Suit != y.Suit)
            {
                return x.Suit - y.Suit;
            }

            //文字列を比較する
            return string.Compare(x.ToString(),y.ToString());
        }

        /// <summary>
        /// カード同士を包含関係で比較する
        /// </summary>
        /// <param name="x">カード1</param>
        /// <param name="y">カード2</param>
        /// <returns>0:同じ 1:カード1がカード2を含む -1:カード2がカード1を含まれる 65535:比較不可</returns>
        public static int CompareByContain(Card x, Card y)
        {
            //nullが最も小さいとする
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            if (x.Contained(y) && x.Contains(y)) { return 0; }
            else if (x.Contained(y)) { return -1; }
            else if (x.Contains(y)) { return 1; }
            else
            {

                //スートで比較
                if (x.Suit != y.Suit)
                {
                    return x.Suit - y.Suit;
                }

                //比較できない場合
                return 65535;
            }
        }

        /// <summary>
        /// 濃度順
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int CompareByCardinality(Card x, Card y)
        {
            //nullが最も小さいとする
            if (x == null && y == null)
            {
                return 0;
            }
            else if (x == null)
            {
                return -1;
            }
            else if (y == null)
            {
                return 1;
            }

            if(x.Cardinality() != y.Cardinality())
            {
                return x.Cardinality() - y.Cardinality();
            }

            if (x.Suit != y.Suit)
            {
                return x.Suit - y.Suit;
            }

            //文字列を比較する
            return string.Compare(x.ToString(), y.ToString());
        }

    }
}
