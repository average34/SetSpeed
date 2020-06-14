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

using System;
using System.Threading.Tasks;
using IceMilkTea.Core;
using UnityEngine;

namespace RpgAtsumaruApiForUnity
{
    /// <summary>
    /// RPGアツマールのスコアボードを制御するクラスになります
    /// </summary>
    public class RpgAtsumaruScoreboard
    {
        // 定数定義
        private const long MaxLimitScoreValue = 999999999999999L;
        private const long MinLimitScoreValue = -999999999999999L;

        // メンバ変数定義
        private ImtAwaitableManualReset<string> scoreboardShowAwaitable;
        private ImtAwaitableManualReset<string> scoreboardSendAwaitable;
        private ImtAwaitableManualReset<string> scoreboardReceivedAwaitable;



        /// <summary>
        /// RpgAtsumaruScoreboard のインスタンスを初期化します
        /// </summary>
        /// <param name="receiver">RPGアツマールネイティブAPIコールバックを拾うレシーバ</param>
        internal RpgAtsumaruScoreboard(RpgAtsumaruApi.RpgAtsumaruApiCallbackReceiver receiver)
        {
            // レシーバにイベントを登録する
            receiver.ScoreboardShown += OnScoreboardShown;
            receiver.ScoreSendCompleted += OnScoreSendCompleted;
            receiver.ScoreboardReceived += OnScoreboardReceived;


            // マニュアルリセット待機可能オブジェクトをシグナル状態で生成する
            scoreboardShowAwaitable = new ImtAwaitableManualReset<string>(true);
            scoreboardSendAwaitable = new ImtAwaitableManualReset<string>(true);
            scoreboardReceivedAwaitable = new ImtAwaitableManualReset<string>(true);
        }


        /// <summary>
        /// RPGアツマール上にスコアボードの表示の完了イベントを処理します
        /// </summary>
        /// <param name="result">scoreboards.display関数の実行結果を含んだjsonデータ</param>
        private void OnScoreboardShown(string result)
        {
            // 待機オブジェクトに送られてきたjsonデータ付きでシグナルを設定する
            scoreboardShowAwaitable.Set(result);
        }


        /// <summary>
        /// RPGアツマールスコアボードにスコアを送信完了したイベントを処理します
        /// </summary>
        /// <param name="result">scoreboards.setRecord関数の実行結果を含んだjsonデータ</param>
        private void OnScoreSendCompleted(string result)
        {
            // 待機オブジェクトに送られてきたjsonデータ付きでシグナルを設定する
            scoreboardSendAwaitable.Set(result);
        }


        /// <summary>
        /// RPGアツマールスコアボードからスコアデータの受信完了したイベントを処理します
        /// </summary>
        /// <param name="result">scoreboards.getRecords関数の実行結果を含んだjsonデータ</param>
        private void OnScoreboardReceived(string result)
        {
            // 待機オブジェクトに送られてきたjsonデータ付きでシグナルを設定する
            scoreboardReceivedAwaitable.Set(result);
        }


        /// <summary>
        /// RPGアツマール上に指定されたスコアボードを非同期に表示します。
        /// RPGアツマールの仕様上、既定は 1 ～ 10 までです。10個以上の場合は管理ページから上限を指定できます。
        /// </summary>
        /// <param name="boardId">表示したいスコアボードID</param>
        /// <returns>スコアボードを表示する操作タスクを返します</returns>
        public virtual async Task<(bool isError, string message)> ShowScoreboardAsync(int boardId)
        {
            // もし、シグナル状態なら
            if (scoreboardShowAwaitable.IsCompleted)
            {
                // 非シグナル状態にしてネイティブプラグイン関数を叩く
                scoreboardShowAwaitable.Reset();
                RpgAtsumaruNativeApi.ShowScoreBoard(boardId);
            }


            // シグナル状態になるまで待って結果を受け取る
            var jsonData = await scoreboardShowAwaitable;
            var result = JsonUtility.FromJson<RpgAtsumaruBasicResult>(jsonData);


            // 結果を返す
            return (result.ErrorOccured, result.Error.message);
        }


        /// <summary>
        /// RPGアツマールの指定されたスコアボードにスコアを非同期に送信します
        /// </summary>
        /// <param name="boardId">送信する先のスコアボードID</param>
        /// <param name="score">送信するスコア</param>
        /// <returns>スコアを送信する操作タスクを返します</returns>
        /// <exception cref="ArgumentOutOfRangeException">score が {MinLimitScoreValue} - {MaxLimitScoreValue} の範囲外です Value={score}</exception>
        public virtual async Task<(bool isError, string message)> SendScoreAsync(int boardId, long score)
        {
            // 例外判定を処理する
            ThrowIfOutOfRangeScore(score);


            // もし、シグナル状態なら
            if (scoreboardSendAwaitable.IsCompleted)
            {
                // 非シグナル状態にしてネイティブプラグイン関数を叩く
                scoreboardSendAwaitable.Reset();
                RpgAtsumaruNativeApi.SendScoreRecord(boardId, score);
            }


            // シグナル状態になるまで待って結果を受け取る
            var jsonData = await scoreboardSendAwaitable;
            var result = JsonUtility.FromJson<RpgAtsumaruBasicResult>(jsonData);


            // 結果を返す
            return (result.ErrorOccured, result.Error.message);
        }


        /// <summary>
        /// RPGアツマールの指定されたスコアボードからスコアボードデータを非同期に取得します
        /// </summary>
        /// <param name="boardId">取得したいスコアボードID</param>
        /// <returns>スコアボードの取得をする操作タスクを返します</returns>
        public virtual async Task<(bool isError, string message, RpgAtsumaruScoreboardData scoreboard)> GetScoreboardAsync(int boardId)
        {
            // もし、シグナル状態なら
            if (scoreboardReceivedAwaitable.IsCompleted)
            {
                // 非シグナル状態にしてネイティブプラグイン関数を叩く
                scoreboardReceivedAwaitable.Reset();
                RpgAtsumaruNativeApi.GetScoreRecord(boardId);
            }


            // シグナル状態になるまで待って結果を受け取る
            var jsonData = await scoreboardReceivedAwaitable;
            var result = JsonUtility.FromJson<RpgAtsumaruScoreboardResult>(jsonData);


            // 結果を返す
            return (result.ErrorOccured, result.Error.message, result.ScoreboardData);
        }


        /// <summary>
        /// スコアがRPGアツマールで扱えない範囲の場合に例外をスローします
        /// </summary>
        /// <param name="score">これから扱うスコア</param>
        /// <exception cref="ArgumentOutOfRangeException">score が {MinLimitScoreValue} - {MaxLimitScoreValue} の範囲外です Value={score}</exception>
        protected void ThrowIfOutOfRangeScore(long score)
        {
            // スコアが最小と最大の範囲外なら
            if (MinLimitScoreValue > score || score > MaxLimitScoreValue)
            {
                // 範囲外例外を吐く
                throw new ArgumentOutOfRangeException(nameof(score), $"score が {MinLimitScoreValue} - {MaxLimitScoreValue} の範囲外です Value={score}");
            }
        }
    }
}