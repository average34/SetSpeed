using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Choosable : MonoBehaviour, IPointerClickHandler
{

    public enum Player : int
    {
        None = 0,
        Player1 = 1,
        Player2,
    }

    public Player _player = Player.None;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {

        AudioManager.Instance.PlaySE("トランプ・配る・出す01", 4);
        if (_player == Player.Player1)
        {
            GameManager.Instance.ChoosedCard_1P = this.gameObject;
        }
        else if (_player == Player.Player2)
        {
            GameManager.Instance.ChoosedCard_2P = this.gameObject;
        }
        else
        {
            GameManager.Instance.ChoosedCard_1P = this.gameObject;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
