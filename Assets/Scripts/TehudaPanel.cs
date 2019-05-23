using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TehudaPanel : MonoBehaviour {


    public enum Player : int
    {
        None = 0,
        Player1 = 1,
        Player2,
    }

    public Player _player = Player.None;
    
    void Update () {

        //バグで2枚以上のオブジェクトが子に入らないようにする
        while (this.transform.childCount > 1)
        {
            foreach (Transform child in transform)
            {
                if (child.GetSiblingIndex() == this.transform.childCount - 1)
                {
                    GameObject.Destroy(child.gameObject);
                    Debug.Log("手札に2つ以上の子オブジェクトがあります。");
                }
            }
            break;
        }

    }
}
