
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

#if UNITY_WEBGL
using RpgAtsumaruApiForUnity;
#endif


public class AtsumaruRanking : MonoBehaviour
{
#if UNITY_WEBGL

    private void Awake()
    {
        
        // もしプラグインの初期化が終わっていないなら
        if (!RpgAtsumaruApi.Initialized)
        {
            // プラグインの初期化
            RpgAtsumaruApi.Initialize();
        }
    }
    // 指定されたボードIDにスコアデータを送信します
    // 送信できるスコアボードの数は、RPGアツマールのAPI管理画面にて調整する事が出来ます。
    // 既定の数は10個までとなっています。
    public async Task SendScore(int boardId, long score)
    {
        // RPGアツマールにスコアを送信する
        await RpgAtsumaruApi.ScoreboardApi.SendScoreAsync(boardId, score);
    }
    // 指定されたスコアボードIDのスコアボードをRPGアツマール上に表示します
    public async Task ShowScoreboard(int boardId)
    {
        // 非同期の表示呼び出しをする（表示されたかどうかの待機ではなく、処理の結果待機であることに注意して下さい）
        await RpgAtsumaruApi.ScoreboardApi.ShowScoreboardAsync(boardId);
    }
    // RPGアツマールのスコアサーバーからスコアボードのデータを取得します
    public async Task<RpgAtsumaruScoreboardData> GetScoreboardData(int boardId)
    {
        // 非同期の取得呼び出しをする（タプル型で返されるため3つ目の結果だけを受け取る場合は以下の通りに実装すると良いでしょう）
        var (_, _, scoreboardData) = await RpgAtsumaruApi.ScoreboardApi.GetScoreboardAsync(boardId);
        return scoreboardData;
    }

#endif
}
