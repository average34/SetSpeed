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
using System.Collections;
using UnityEngine;

namespace RpgAtsumaruApiForUnity
{
    /// <summary>
    /// RPGアツマールのAPIの中心となるクラスです。
    /// あらゆるAPIはこのクラスからアクセスする事になります。
    /// </summary>
    public static class RpgAtsumaruApi
    {
        // 定数定義
        internal const string CallbackReceiverGameObjectName = "__RPGATSUMARU_CALLBACK_RECEIVER__";

        // クラス変数宣言
        private static RpgAtsumaruGeneral generalApi;
        private static RpgAtsumaruStorage storageApi;
        private static RpgAtsumaruVolume volumeApi;
        private static RpgAtsumaruComment commentApi;
        private static RpgAtsumaruController controllerApi;
        private static RpgAtsumaruScoreboard scoreboardApi;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private static bool initialized;
#endif



        #region プロパティ
        /// <summary>
        /// RpgAtsumaruApiForUnity プラグインが初期化済みかどうか
        /// </summary>
        public static bool Initialized =>
#if !UNITY_EDITOR && UNITY_WEBGL && !DEVELOPMENT_BUILD
            RpgAtsumaruNativeApi.IsInitialized();
#else
            initialized;
#endif


        /// <summary>
        /// RPGアツマールの汎用的なAPIを取得します
        /// </summary>
        public static RpgAtsumaruGeneral GeneralApi
        {
            get
            {
                // 例外判定を入れてからAPIのインスタンスを返す
                ThrowIfNotInitialized();
                return generalApi;
            }
        }


        /// <summary>
        /// RPGアツマールのサーバーストレージを操作するAPIを取得します
        /// </summary>
        /// <exception cref="InvalidOperationException">プラグインが初期化されていません。Initialize関数を呼び出して初期化を完了してください</exception>
        public static RpgAtsumaruStorage StorageApi
        {
            get
            {
                // 例外判定を入れてからAPIのインスタンスを返す
                ThrowIfNotInitialized();
                return storageApi;
            }
        }


        /// <summary>
        /// RPGアツマールのマスター音量を制御するAPIを取得します
        /// </summary>
        /// <exception cref="InvalidOperationException">プラグインが初期化されていません。Initialize関数を呼び出して初期化を完了してください</exception>
        public static RpgAtsumaruVolume VolumeApi
        {
            get
            {
                // 例外判定を入れてからAPIのインスタンスを返す
                ThrowIfNotInitialized();
                return volumeApi;
            }
        }


        /// <summary>
        /// RPGアツマールのコメントを制御するAPIを取得します
        /// </summary>
        /// <exception cref="InvalidOperationException">プラグインが初期化されていません。Initialize関数を呼び出して初期化を完了してください</exception>
        public static RpgAtsumaruComment CommentApi
        {
            get
            {
                // 例外判定を入れてからAPIのインスタンスを返す
                ThrowIfNotInitialized();
                return commentApi;
            }
        }


        /// <summary>
        /// RPGアツマールのコントローラを制御するAPIを取得します
        /// </summary>
        /// <exception cref="InvalidOperationException">プラグインが初期化されていません。Initialize関数を呼び出して初期化を完了してください</exception>
        public static RpgAtsumaruController ControllerApi
        {
            get
            {
                // 例外判定を入れてからAPIのインスタンスを返す
                ThrowIfNotInitialized();
                return controllerApi;
            }
        }


        /// <summary>
        /// RPGアツマールのスコアボードを制御するAPIを取得します
        /// </summary>
        /// <exception cref="InvalidOperationException">プラグインが初期化されていません。Initialize関数を呼び出して初期化を完了してください</exception>
        public static RpgAtsumaruScoreboard ScoreboardApi
        {
            get
            {
                // 例外判定を入れてからAPIのインスタンスを返す
                ThrowIfNotInitialized();
                return scoreboardApi;
            }
        }
        #endregion



        #region 初期化＆汎用ロジック
        /// <summary>
        /// RPGアツマールAPIの初期化を行います。
        /// あらゆるAPIを呼び出す前に必ず一度だけ呼び出してください。
        /// </summary>
        public static void Initialize()
        {
            // 既に初期化済みなら
            if (Initialized)
            {
                // 直ちに終了
                return;
            }


            // コールバックを受け取るためのゲームオブジェクトを生成して、トランスフォームの取得と必要コンポーネントのアタッチをする
            var gameObject = new GameObject(CallbackReceiverGameObjectName);
            var transform = gameObject.GetComponent<Transform>();
            var receiver = gameObject.AddComponent<RpgAtsumaruApiCallbackReceiver>();


            // ゲームオブジェクトをヒエラルキから姿を消してシーン遷移による削除を受けないようにする
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            UnityEngine.Object.DontDestroyOnLoad(gameObject);


            // トランスフォームを初期化する
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.eulerAngles = Vector3.zero;


            // ネイティブAPIの初期化パラメータを生成して初期化する
            var nativeApiInitializeParam = new RpgAtsumaruNativeApiInitializeParameter()
            {
                // オブジェクト名や、コールバック名を設定していく
                UnityObjectName = CallbackReceiverGameObjectName,
                GetItemsCallback = nameof(RpgAtsumaruApiCallbackReceiver.OnStorageItemsReceived),
                SetItemsCallback = nameof(RpgAtsumaruApiCallbackReceiver.OnStorageSetItemsCompleted),
                RemoveItemCallback = nameof(RpgAtsumaruApiCallbackReceiver.OnStorageRemoveItemCompleted),
                VolumeChangedCallback = nameof(RpgAtsumaruApiCallbackReceiver.OnVolumeChanged),
                OpenLinkCallback = nameof(RpgAtsumaruApiCallbackReceiver.OnOpenLinkCompleted),
                CreatorInfoShownCallback = nameof(RpgAtsumaruApiCallbackReceiver.OnCreatorInfoShown),
                ScreenshotCallback = nameof(RpgAtsumaruApiCallbackReceiver.OnScreenshotCompleted),
                TakeScreenshotCallback = nameof(RpgAtsumaruApiCallbackReceiver.OnRequestScreenShot),
                ScoreboardShownCallback = nameof(RpgAtsumaruApiCallbackReceiver.OnScoreboardShown),
                SetScoreCallback = nameof(RpgAtsumaruApiCallbackReceiver.OnScoreSendCompleted),
                GetScoreCallback = nameof(RpgAtsumaruApiCallbackReceiver.OnScoreboardReceived),
            };


#if !UNITY_EDITOR && UNITY_WEBGL && !DEVELOPMENT_BUILD
            // 初期化パラメータのJSONデータ化してネイティブAPIの初期化をする
            var jsonData = JsonUtility.ToJson(nativeApiInitializeParam);
            RpgAtsumaruNativeApi.Initialize(jsonData);


            // 各APIを処理するクラスのインスタンスを生成
            generalApi = new RpgAtsumaruGeneral(receiver);
            storageApi = new RpgAtsumaruStorage(receiver);
            volumeApi = new RpgAtsumaruVolume(receiver);
            commentApi = new RpgAtsumaruComment(receiver);
            controllerApi = new RpgAtsumaruController(receiver);
            scoreboardApi = new RpgAtsumaruScoreboard(receiver);
#else
            // 各APIを処理するダミークラスのインスタンスを生成
            generalApi = new DummyRpgAtsumaruGeneral(receiver);
            storageApi = new DummyRpgAtsumaruStorage(receiver);
            volumeApi = new DummyRpgAtsumaruVolume(receiver);
            commentApi = new DummyRpgAtsumaruComment(receiver);
            controllerApi = new DummyRpgAtsumaruController(receiver);
            scoreboardApi = new DummyRpgAtsumaruScoreboard(receiver);


            // 初期化済みであることをマークする
            initialized = true;
            InternalLogger.Log(LogLevel.Log, "RpgAtsumaruAPI Initialized");
#endif
        }


        /// <summary>
        /// プラグインが未初期化の場合に例外をスローします
        /// </summary>
        /// <exception cref="InvalidOperationException">プラグインが初期化されていません。Initialize関数を呼び出して初期化を完了してください</exception>
        private static void ThrowIfNotInitialized()
        {
            // 未初期化なら
            if (!Initialized)
            {
                // 未初期化例外を吐く
                throw new InvalidOperationException("プラグインが初期化されていません。Initialize関数を呼び出して初期化を完了してください");
            }
        }
        #endregion



        #region レシーバコンポーネントクラスの実装
        /// <summary>
        /// RPGアツマールネイティブAPIからのコールバックを受け付けるレシーバコンポーネントクラスです
        /// </summary>
        internal sealed class RpgAtsumaruApiCallbackReceiver : MonoBehaviour
        {
            /// <summary>
            /// RPGアツマールのサーバーストレージからデータを受信したときのイベントです
            /// </summary>
            public event Action<string> StorageItemsReceived;

            /// <summary>
            /// RPGアツマールのサーバーストレージにデータを設定した完了イベントです
            /// </summary>
            public event Action StorageSetItemsCompleted;

            /// <summary>
            /// RPGアツマールのサーバーストレージからデータを削除した完了イベントです
            /// </summary>
            public event Action StorageRemoveItemCompleted;

            /// <summary>
            /// RPGアツマールのマスター音量を変更したときの通知イベントです
            /// </summary>
            public event Action<float> VolumeChanged;

            /// <summary>
            /// RPGアツマールのURLを開くポップアップの表示を完了したイベントです
            /// </summary>
            public event Action<string> OpenLinkCompleted;

            /// <summary>
            /// RPGアツマールの作者情報ダイアログの表示を完了したイベントです
            /// </summary>
            public event Action<string> CreatorInfoShown;

            /// <summary>
            /// RPGアツマールのスクリーンショットとそのダイアログ表示を完了したイベントです
            /// </summary>
            public event Action<string> ScreenshotCompleted;

            /// <summary>
            /// RPGアツマールからスクリーンショットの要求を受けた時のイベントです
            /// </summary>
            public event Action RequestScreenShot;

            /// <summary>
            /// RPGアツマール上にスコアボードの表示を完了したイベントです
            /// </summary>
            public event Action<string> ScoreboardShown;

            /// <summary>
            /// RPGアツマールスコアボードにスコアの送信が完了したイベントです
            /// </summary>
            public event Action<string> ScoreSendCompleted;

            /// <summary>
            /// RPGアツマールスコアボードのスコアデータを受信したイベントです
            /// </summary>
            public event Action<string> ScoreboardReceived;


            /// <summary>
            /// Unityの描画ループの最後に呼び出すフレーム終了イベントです
            /// </summary>
            public event Action EndOfFrameTriggered;



            /// <summary>
            /// コンポーネントの初期化を行います
            /// </summary>
            private void Awake()
            {
                // フレーム終了ループを開始する
                StartCoroutine(DoEndOfFrameLoop(new WaitForEndOfFrame()));
            }


            /// <summary>
            /// RPGアツマールのサーバーストレージからすべてのデータをjsonで受け取ったときのイベントを処理します
            /// </summary>
            /// <param name="jsonData">受け取ったデータのjson文字列データ</param>
            public void OnStorageItemsReceived(string jsonData)
            {
                // イベントにそのまま横流し
                StorageItemsReceived?.Invoke(jsonData);
            }


            /// <summary>
            /// RPGアツマールのサーバーストレージにデータを設定した完了イベントを処理します
            /// </summary>
            public void OnStorageSetItemsCompleted()
            {
                // イベントにそのまま横流し
                StorageSetItemsCompleted?.Invoke();
            }


            /// <summary>
            /// RPGアツマールのサーバーストレージからデータを削除した完了イベントを処理します
            /// </summary>
            public void OnStorageRemoveItemCompleted()
            {
                // イベントにそのまま横流し
                StorageRemoveItemCompleted?.Invoke();
            }


            /// <summary>
            /// RPGアツマールのマスター音量を変更したときの通知イベントを処理します
            /// </summary>
            /// <param name="volume">変更された音量（OFF 0.0 ～ 1.0 ON）</param>
            public void OnVolumeChanged(float volume)
            {
                // イベントにそのまま横流し
                VolumeChanged?.Invoke(volume);
            }


            /// <summary>
            /// RPGアツマールのURLポップアップ表示をした完了イベントを処理します
            /// </summary>
            /// <param name="result">openLink関数の実行結果を含んだjsonデータ</param>
            public void OnOpenLinkCompleted(string result)
            {
                // イベントにそのまま横流し
                OpenLinkCompleted?.Invoke(result);
            }


            /// <summary>
            /// RPGアツマールの作者情報ダイアログを表示した完了イベントを処理します
            /// </summary>
            /// <param name="result">displayCreatorInformationModal関数の実行結果を含んだjsonデータ</param>
            public void OnCreatorInfoShown(string result)
            {
                // イベントにそのまま横流し
                CreatorInfoShown?.Invoke(result);
            }


            /// <summary>
            /// RPGアツマールのスクリーンショットとそのダイアログ表示の完了イベントを処理します
            /// </summary>
            /// <param name="result">screenshot.displayModal関数の実行結果を含んだjsonデータ</param>
            public void OnScreenshotCompleted(string result)
            {
                // イベントにそのまま横流し
                ScreenshotCompleted?.Invoke(result);
            }


            /// <summary>
            /// RPGアツマールのスクリーンショットデータ要求イベントを処理します
            /// </summary>
            public void OnRequestScreenShot()
            {
                // イベントにそのまま横流し
                RequestScreenShot?.Invoke();
            }


            /// <summary>
            /// RPGアツマール上にスコアボードの表示の完了イベントを処理します
            /// </summary>
            /// <param name="result">scoreboards.display関数の実行結果を含んだjsonデータ</param>
            public void OnScoreboardShown(string result)
            {
                // イベントにそのまま横流し
                ScoreboardShown?.Invoke(result);
            }


            /// <summary>
            /// RPGアツマールスコアボードにスコアを送信完了したイベントを処理します
            /// </summary>
            /// <param name="result">scoreboards.setRecord関数の実行結果を含んだjsonデータ</param>
            public void OnScoreSendCompleted(string result)
            {
                // イベントにそのまま横流し
                ScoreSendCompleted?.Invoke(result);
            }


            /// <summary>
            /// RPGアツマールスコアボードからスコアデータの受信完了したイベントを処理します
            /// </summary>
            /// <param name="result">scoreboards.getRecords関数の実行結果を含んだjsonデータ</param>
            public void OnScoreboardReceived(string result)
            {
                // イベントにそのまま横流し
                ScoreboardReceived?.Invoke(result);
            }


            /// <summary>
            /// Unityのフレーム終了イベントループを実行します
            /// </summary>
            /// <returns>Unityのフレーム終了待機オブジェクトを返します</returns>
            private IEnumerator DoEndOfFrameLoop(WaitForEndOfFrame waitForEndOfFrame)
            {
                // 無限ループ
                while (true)
                {
                    // フレーム終了まで待機して、戻ってきたらイベントを起こす
                    yield return waitForEndOfFrame;
                    EndOfFrameTriggered?.Invoke();
                }
            }
        }
        #endregion
    }
}