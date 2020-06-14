// zlib/libpng License
//
// Copyright (c) 2018 Sinoa
//
// This software is provided 'as-is', without any express or implied warranty.
// In no event will the authors be held liable for any damages arising from the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it freely,
// subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software.
//    If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using System.Threading.Tasks;

namespace RpgAtsumaruApiForUnity
{
    /// <summary>
    /// スコアボードAPIのダミークラスです。
    /// 実際の処理は行わず、何を呼び出されたかのログのみ出力します。
    /// </summary>
    internal sealed class DummyRpgAtsumaruScoreboard : RpgAtsumaruScoreboard
    {
        /// <summary>
        /// DummyRpgAtsumaruScoreboard のインスタンスを初期化します
        /// </summary>
        /// <param name="receiver">RPGアツマールネイティブAPIコールバックを拾うレシーバ</param>
        internal DummyRpgAtsumaruScoreboard(RpgAtsumaruApi.RpgAtsumaruApiCallbackReceiver receiver) : base(receiver)
        {
        }


        /// <summary>
        /// RPGアツマール上に指定されたスコアボードを非同期に表示します。
        /// RPGアツマールの仕様上、既定は 1 ～ 10 までです。10個以上の場合は管理ページから上限を指定できます。
        /// </summary>
        /// <param name="boardId">表示したいスコアボードID</param>
        /// <returns>常に成功をする完了タスクを返します</returns>
        public override Task<(bool isError, string message)> ShowScoreboardAsync(int boardId)
        {
            // どのようなパラメータで呼び出されたのかのログを吐く
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(ShowScoreboardAsync)}({boardId})");
            return Task.FromResult((false, string.Empty));
        }


        /// <summary>
        /// RPGアツマールの指定されたスコアボードにスコアを非同期に送信します
        /// </summary>
        /// <param name="boardId">送信する先のスコアボードID</param>
        /// <param name="score">送信するスコア</param>
        /// <returns>常に成功をする完了タスクを返します</returns>
        /// <exception cref="ArgumentOutOfRangeException">score が {MinLimitScoreValue} - {MaxLimitScoreValue} の範囲外です Value={score}</exception>
        public override Task<(bool isError, string message)> SendScoreAsync(int boardId, long score)
        {
            // 例外判定を処理する
            ThrowIfOutOfRangeScore(score);


            // どのようなパラメータで呼び出されたのかのログを吐く
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(SendScoreAsync)}({boardId}, {score})");
            return Task.FromResult((false, string.Empty));
        }


        /// <summary>
        /// RPGアツマールの指定されたスコアボードからスコアボードデータを非同期に取得します
        /// </summary>
        /// <param name="boardId">取得したいスコアボードID</param>
        /// <returns>常に成功をして、空のスコアボード情報を返す完了タスクを返します</returns>
        public override Task<(bool isError, string message, RpgAtsumaruScoreboardData scoreboard)> GetScoreboardAsync(int boardId)
        {
            // どのようなパラメータで呼び出されたのかのログを吐く
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(GetScoreboardAsync)}({boardId})");


            // 空のスコアボードを生成する
            var emptyScoreboard = new RpgAtsumaruScoreboardData()
            {
                // スコアボードID以外無効なデータとして埋める
                boardId = boardId,
                boardName = string.Empty,
                myRecord = new RpgAtsumaruMyRecord() { Available = false },
                myBestRecor = new RpgAtsumaruMyBestRecord() { Available = false },
                ranking = new RpgAtsumaruRanking[0],
            };


            // 空のスコアボードを持った完了タスクを返す
            return Task.FromResult((false, string.Empty, emptyScoreboard));
        }
    }
}