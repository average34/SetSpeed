using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// BGMとSEの管理をするマネージャ。シングルトン。
/// </summary>
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    //ボリューム保存用のkeyとデフォルト値
    private const string BGM_VOLUME_KEY = "BGM_VOLUME_KEY";
    private const string SE_VOLUME_KEY = "SE_VOLUME_KEY";
    private const float BGM_VOLUME_DEFULT = 0.5f;
    private const float SE_VOLUME_DEFULT = 1.0f;

    //BGMがフェードするのにかかる時間
    public const float BGM_FADE_SPEED_RATE_HIGH = 0.9f;
    public const float BGM_FADE_SPEED_RATE_LOW = 0.3f;
    private float _bgmFadeSpeedRate = BGM_FADE_SPEED_RATE_HIGH;

    //次流すBGM名、SE名
    private string _nextBGMName;
    private string _nextSEName;

    //BGMをフェードアウト中か
    private bool _isFadeOut = false;

    //BGM用、SE用に分けてオーディオソースを持つ
    public AudioSource AttachBGMSource;
    public AudioSource AttachSESource_ch1;
    public AudioSource AttachSESource_ch2;
    public AudioSource AttachSESource_ch3;
    public AudioSource AttachSESource_ch4;
    AudioSource AttachSESource;

    //全Audioを保持
    private Dictionary<string, AudioClip> _bgmDic, _seDic;

    //=================================================================================
    //初期化
    //=================================================================================

    protected override void Awake()
    {
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        //リソースフォルダから全SE&BGMのファイルを読み込みセット
        _bgmDic = new Dictionary<string, AudioClip>();
        _seDic = new Dictionary<string, AudioClip>();

        object[] bgmList = Resources.LoadAll("Audio/BGM");
        object[] seList = Resources.LoadAll("Audio/SE");

        foreach (AudioClip bgm in bgmList)
        {
            _bgmDic[bgm.name] = bgm;
        }
        foreach (AudioClip se in seList)
        {
            _seDic[se.name] = se;
        }
    }

    private void Start()
    {
        AttachBGMSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, BGM_VOLUME_DEFULT);
        AttachSESource_ch1.volume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, SE_VOLUME_DEFULT);
        AttachSESource_ch2.volume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, SE_VOLUME_DEFULT);
        AttachSESource_ch3.volume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, SE_VOLUME_DEFULT);
        AttachSESource_ch4.volume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, SE_VOLUME_DEFULT);
    }

    //=================================================================================
    //SE
    //=================================================================================

    /// <summary>
    /// 指定したファイル名のSEを流す。第二引数のdelayに指定した時間だけ再生までの間隔を空ける
    /// </summary>
    public void PlaySE(string seName, int channel = 1 ,float delay = 0.0f)
    {
        if (!_seDic.ContainsKey(seName))
        { 
            Debug.Log(seName + "という名前のSEがありません");
            return;
        }

        //_nextSEName = seName;
        //int ch = channel;


        //delay秒後に実行する
        StartCoroutine(DelayMethod(delay, (int ch,string _nextSEName) =>
        {
            if (ch == 1) AttachSESource_ch1.PlayOneShot(_seDic[_nextSEName] as AudioClip);
            else if (ch == 2) AttachSESource_ch2.PlayOneShot(_seDic[_nextSEName] as AudioClip);
            else if (ch == 3) AttachSESource_ch3.PlayOneShot(_seDic[_nextSEName] as AudioClip);
            else if (ch == 4) AttachSESource_ch4.PlayOneShot(_seDic[_nextSEName] as AudioClip);
            else
            {
                Debug.Log("無効な効果音のチャンネルが指定されました");
            }
        },channel,seName));

        //Invoke("DelayPlaySE", delay);
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="waitTime">遅延時間[ミリ秒]</param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod<T,t>(float waitTime, Action<T,t> action, T t1, t t2)
    {
        yield return new WaitForSeconds(waitTime);
        action(t1,t2);
    }
    

    //=================================================================================
    //BGM
    //=================================================================================

    /// <summary>
    /// 指定したファイル名のBGMを流す。ただし既に流れている場合は前の曲をフェードアウトさせてから。
    /// 第二引数のfadeSpeedRateに指定した割合でフェードアウトするスピードが変わる
    /// </summary>
    public void PlayBGM(string bgmName, float fadeSpeedRate = BGM_FADE_SPEED_RATE_HIGH)
    {
        if (!_bgmDic.ContainsKey(bgmName))
        {
            Debug.Log(bgmName + "という名前のBGMがありません");
            return;
        }

        //現在BGMが流れていない時はそのまま流す
        if (!AttachBGMSource.isPlaying)
        {
            _nextBGMName = "";
            AttachBGMSource.clip = _bgmDic[bgmName] as AudioClip;
            AttachBGMSource.Play();
        }
        //違うBGMが流れている時は、流れているBGMをフェードアウトさせてから次を流す。同じBGMが流れている時はスルー
        else if (AttachBGMSource.clip.name != bgmName)
        {
            _nextBGMName = bgmName;
            FadeOutBGM(fadeSpeedRate);
        }

    }

    /// <summary>
    /// 現在流れている曲をフェードアウトさせる
    /// fadeSpeedRateに指定した割合でフェードアウトするスピードが変わる
    /// </summary>
    public void FadeOutBGM(float fadeSpeedRate = BGM_FADE_SPEED_RATE_LOW)
    {
        _bgmFadeSpeedRate = fadeSpeedRate;
        _isFadeOut = true;
    }

    private void Update()
    {
        if (!_isFadeOut)
        {
            return;
        }

        //徐々にボリュームを下げていき、ボリュームが0になったらボリュームを戻し次の曲を流す
        AttachBGMSource.volume -= Time.deltaTime * _bgmFadeSpeedRate;
        if (AttachBGMSource.volume <= 0)
        {
            AttachBGMSource.Stop();
            AttachBGMSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, BGM_VOLUME_DEFULT);
            _isFadeOut = false;

            if (!string.IsNullOrEmpty(_nextBGMName))
            {
                PlayBGM(_nextBGMName);
            }
        }

    }

    //=================================================================================
    //音量変更
    //=================================================================================

    /// <summary>
    /// BGMとSEのボリュームを別々に変更&保存
    /// </summary>
    public void ChangeVolume(float BGMVolume, float SEVolume)
    {
        AttachBGMSource.volume = BGMVolume;
        AttachSESource_ch1.volume = SEVolume;
        AttachSESource_ch2.volume = SEVolume;
        AttachSESource_ch3.volume = SEVolume;
        AttachSESource_ch4.volume = SEVolume;

        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, BGMVolume);
        PlayerPrefs.SetFloat(SE_VOLUME_KEY, SEVolume);
    }
}