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
    /// 汎用APIのダミークラスです。
    /// 実際の処理は行わず、何を呼び出されたかのログのみ出力します。
    /// </summary>
    internal sealed class DummyRpgAtsumaruGeneral : RpgAtsumaruGeneral
    {
        /// <summary>
        /// DummyRpgAtsumaruGeneral のインスタンスを初期化します
        /// </summary>
        /// <param name="receiver">RPGアツマールネイティブAPIコールバックを拾うレシーバ</param>
        internal DummyRpgAtsumaruGeneral(RpgAtsumaruApi.RpgAtsumaruApiCallbackReceiver receiver) : base(receiver)
        {
        }


        /// <summary>
        /// ゲームURLのクエリに設定された値を取得します（RPGアツマールの仕様上クエリの変数名は param1～param9 になります）
        /// </summary>
        /// <param name="name">取得したいクエリ名</param>
        /// <returns>常に空文字列を返します</returns>
        public override string GetQuery(string name)
        {
            // 空文字列を返す
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(GetQuery)}({name})");
            return string.Empty;
        }


        /// <summary>
        /// RPGアツマールのURLを開くポップアップを非同期で表示します
        /// </summary>
        /// <param name="url">URLを開くポップアップに表示するURL</param>
        /// <returns>常に成功を返す完了タスクを返します</returns>
        public override Task<(bool isError, string message)> OpenLinkAsync(string url)
        {
            // 指定されたURLを開いて成功を返す
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(OpenLinkAsync)}({url})");
            UnityEngine.Application.OpenURL(url);
            return Task.FromResult((false, string.Empty));
        }


        /// <summary>
        /// 指定されたニコニコユーザーIDの作者情報ダイアログを非同期で表示します
        /// </summary>
        /// <param name="niconicoUserId">表示するニコニコユーザーID</param>
        /// <returns>常に成功を返す完了タスクを返します</returns>
        public override Task<(bool isError, string message)> ShowCreatorInformationAsync(int niconicoUserId)
        {
            // 成功を返す
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(ShowCreatorInformationAsync)}({niconicoUserId})");
            return Task.FromResult((false, string.Empty));
        }


        /// <summary>
        /// スクリーンショットを撮った後にTwitter投稿ダイアログを非同期で操作します
        /// </summary>
        /// <returns>常に成功を返す完了タスクを返します</returns>
        public override Task<(bool isError, string message)> ScreenshotAsync()
        {
            // 成功を返す
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(ScreenshotAsync)}()");
            return Task.FromResult((false, string.Empty));
        }
    }
}