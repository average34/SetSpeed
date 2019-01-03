using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableRemover : SingletonMonoBehaviour<DraggableRemover>
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }


    public void EnableAll()
    {
        //全オブジェクトのDraggableを無効にする
        foreach (GameObject obj in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            var Draggable = obj.GetComponent<Draggable>();
            if (Draggable != null) Draggable.enabled = true;

        }
    }


    public void RemoveAll()
    {
        //全オブジェクトのDraggableを無効にする
        foreach (GameObject obj in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            var Draggable = obj.GetComponent<Draggable>();
            if(Draggable != null) Draggable.enabled = false;

        }
    }


    public void Remove_2P()
    {
        //2PカードのDraggableを無効にする
        foreach (GameObject obj in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            if (obj.name == "Tehuda1_2P" ||
                obj.name == "Tehuda2_2P" ||
                obj.name == "Tehuda3_2P" ||
                obj.name == "Tehuda4_2P"
                )
            {
                var Draggable = obj.GetComponentInChildren<Draggable>();
                if (Draggable != null) Draggable.enabled = false;
            }
        }
    }



    public void Remove_2P_Drop()
    {
        //2PカードのDraggableを無効にする
        foreach (GameObject obj in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            if (obj.name == "Tehuda1_2P" ||
                obj.name == "Tehuda2_2P" ||
                obj.name == "Tehuda3_2P" ||
                obj.name == "Tehuda4_2P"
                )
            {
                var Draggable = obj.GetComponentInChildren<Draggable>();
                if (Draggable != null) Draggable.enabled = false;
            }
        }
    }

}
