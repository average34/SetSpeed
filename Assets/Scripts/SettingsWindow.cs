using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour {


    //　SoundOptionキャンバスを設定
    [SerializeField]
    private GameObject _settingsCanvas;

    // Use this for initialization
    void Start () {
	}

    // Update is called once per frame
    void Update()
    {

        //Sキーが押されたら開閉
        if (Input.GetKeyDown(KeyCode.S))
        {
            OpenClose();

        }
    }

    public void OpenClose()
    {
        _settingsCanvas.SetActive(!_settingsCanvas.activeSelf);

    }


    public void SetBGM(float volume)
    {
        AudioManager.Instance.ChangeVolume(volume, AudioManager.Instance.AttachSESource_ch1.volume);
    }

    public void SetSE(float volume)
    {
        AudioManager.Instance.ChangeVolume(AudioManager.Instance.AttachBGMSource.volume, volume);
    }

}
