using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {

    public List<GameObject> StartButtons;
    public List<GameObject> LevelButtons;

    public void StartButton()
    {
        foreach(GameObject button in StartButtons)
        {
            button.SetActive(false);
        }
        foreach (GameObject button in LevelButtons)
        {
            button.SetActive(true);
        }
        //曲再生
        //midstream jam - musmus
        AudioManager.Instance.PlayBGM("tw061");
        

    }


    public void Exit()
    {
        Application.Quit();
    }

        public void NextScene(int level)
    {
        TitleManager._CPULevel = (TitleManager.Level)level;


        SceneManager.LoadScene("SetSpeed_v02");


        switch (TitleManager._CPULevel)
        {
            case TitleManager.Level.level1:
                TitleManager._chara_1P = CharaSwitch.Character.Yukari;
                TitleManager._chara_2P = CharaSwitch.Character.Itako;
                TitleManager._level = CPUManager.CPULevel.Level1;
                break;
            case TitleManager.Level.level2:
                TitleManager._chara_1P = CharaSwitch.Character.Yukari;
                TitleManager._chara_2P = CharaSwitch.Character.Zunko;
                TitleManager._level = CPUManager.CPULevel.Level2;
                break;
            case TitleManager.Level.level3:
                TitleManager._chara_1P = CharaSwitch.Character.Yukari;
                TitleManager._chara_2P = CharaSwitch.Character.Kiritan;
                TitleManager._level = CPUManager.CPULevel.Level3;
                break;
            case TitleManager.Level.level4:
                TitleManager._chara_1P = CharaSwitch.Character.Yukari;
                TitleManager._chara_2P = CharaSwitch.Character.Akane;
                TitleManager._level = CPUManager.CPULevel.Level4;
                break;

        }


    }

    public void PlayMainBGM()
    {

        //曲再生
        //midstream jam - musmus
        AudioManager.Instance.PlayBGM("tw061");
    }

}
