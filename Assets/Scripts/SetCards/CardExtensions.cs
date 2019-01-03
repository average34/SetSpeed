using System;
using System.Collections.Generic;

namespace SetCards.Cards
{

    /// <summary>
    /// Cardクラスの拡張メソッド。
    /// </summary>
    public static class CardExtensions
    {


        //====================================
        //1カード関数
        //====================================
        /// <summary>
        /// カードから集合の濃度（要素の個数）を返す関数。
        /// </summary>
        public static int Cardinality(this Card c)
        {
            int Cardinality = 0;
            foreach (bool b in c.ExistArray)
            { if (b) { Cardinality += 1; } }

            return Cardinality;
        }

        /// <summary>
        /// カードから補集合のカードを返す関数。スートなし。
        /// </summary>
        public static Card NOT(this Card c)
        {
            Card Not;
            string NotNumber = "";

            for (int i = 0; i < 5; i++)
            {
                if (c.ExistArray[i] == false)
                {
                    NotNumber += (i + 1).ToString();
                }
            }

            //カードを作成
            Not = new Card(0, NotNumber);

            return Not;
        }

        //====================================
        //2カード関数
        //====================================

        //-----------------------------------
        //Card×Card→Card
        //-----------------------------------
        /// <summary>
        /// c1∩c2となるカードを返す関数。スートなし。
        /// </summary>
        /// <param name="c1">第一カード</param>
        /// <param name="c2">第二カード</param>
        /// <returns>c1∩c2</returns>
        public static Card AND(this Card c1, Card c2)
        {
            Card And;
            string Number = "";

            for (int i = 0; i < 5; i++)
            {
                if (c1.ExistArray[i] == true && c2.ExistArray[i] == true)
                {
                    Number += (i + 1).ToString();
                }
            }
            //カードを作成
            And = new Card(0, Number);
            return And;
        }

        /// <summary>
        /// c1∪c2となるカードを返す関数。スートなし。
        /// </summary>
        /// <param name="c1">第一カード</param>
        /// <param name="c2">第二カード</param>
        /// <returns>c1∪c2</returns>
        public static Card OR(this Card c1, Card c2)
        {
            Card Or;
            string Number = "";

            for (int i = 0; i < 5; i++)
            {
                if (c1.ExistArray[i] == true || c2.ExistArray[i] == true)
                {
                    Number += (i + 1).ToString();
                }
            }

            //カードを作成
            Or = new Card(0, Number);
            return Or;
        }


        /// <summary>
        /// c1⊕c2（排他的論理和）となるカードを返す関数。スートなし。
        /// </summary>
        /// <param name="c1">第一カード</param>
        /// <param name="c2">第二カード</param>
        /// <returns>c1⊕c2</returns>
        public static Card XOR(this Card c1, Card c2)
        {
            Card Xor;
            string Number = "";

            for (int i = 0; i < 5; i++)
            {
                if (c1.ExistArray[i] == true ^ c2.ExistArray[i] == true)
                {
                    Number += (i + 1).ToString();
                }
            }

            //カードを作成
            Xor = new Card(0, Number);
            return Xor;
        }


        //-----------------------------------
        //Card×Card→bool
        //-----------------------------------
        /// <summary>
        /// c1⇒c2(c1⊆c2)を判定する関数。スートは問わない。
        /// Containsの逆。
        /// </summary>
        /// <param name="c1">包含されるカード</param>
        /// <param name="c2">包含するカード</param>
        /// <returns>true:c1はc2に包含される</returns>
        public static bool Contained(this Card c1, Card c2)
        {
            bool blContained = true;
            for (int i = 0; i < 5; i++)
            {
                if (c1.ExistArray[i] == true && c2.ExistArray[i] == false)
                {
                    blContained = false;
                    break;
                }
            }
            return blContained;
        }

        /// <summary>
        /// c1←c2(c1⊇c2)を判定する関数。スートは問わない。
        /// Containedの逆。
        /// </summary>
        /// <param name="c1">包含するカード</param>
        /// <param name="c2">包含されるカード</param>
        /// <returns>true:c1はc2に包含される</returns>
        public static bool Contains(this Card c1, Card c2)
        {
            return c2.Contained(c1);
        }

    }
}
