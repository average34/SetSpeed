using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Slider))]
public class BGMSlider : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Slider>().value = AudioManager.Instance.AttachBGMSource.volume;


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
