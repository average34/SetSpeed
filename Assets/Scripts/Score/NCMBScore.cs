using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using System;

public class NCMBScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// データを送信する
    /// </summary>
    public void TimeScoreUpload(TimeSpan timeScore,int id = 0)
    {
        
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(timeScore,id);
    }

}
