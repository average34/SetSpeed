using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetCards.Cards
{
    public static class CardListExtensions
    {
        //カードを文字列表記
        public static string DeckView(this List<Card> Deck)
        {
            string str = "";
            foreach (Card c in Deck)
            {
                str += c.ToString() + ",";
            }

            return str;
        }

        public static string TopologiesView(this List<List<Card>> Topologies)
        {
            string str = "";
            foreach (List<Card> Deck in Topologies)
            {
                str += Deck.DeckView();
                str += "\r\n";
            }

            return str;
        }

    }
}
