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

namespace RpgAtsumaruApiForUnity
{
    /// <summary>
    /// コメントAPIのダミークラスです。
    /// 実際の処理は行わず、何を呼び出されたかのログのみ出力します。
    /// </summary>
    internal sealed class DummyRpgAtsumaruComment : RpgAtsumaruComment
    {
        /// <summary>
        /// DummyRpgAtsumaruComment クラスのインスタンスを初期化します
        /// </summary>
        /// <param name="receiver">RPGアツマールネイティブAPIコールバックを拾うレシーバ</param>
        internal DummyRpgAtsumaruComment(RpgAtsumaruApi.RpgAtsumaruApiCallbackReceiver receiver) : base(receiver)
        {
        }


        /// <summary>
        /// 指定されたシーン名を用いて、コメントシーンを切り替えます。
        /// </summary>
        /// <param name="sceneName">切り替えるコメントシーン名。ただし、最大64文字のASCII文字列でなければなりません。さらに文字列の先頭にはアンダースコア2つをつけることは許されていません。</param>
        /// <param name="reset">コメントシーンの状態をリセットする場合は true</param>
        /// <exception cref="ArgumentException">シーン名が null または 空文字列 です</exception>
        /// <exception cref="ArgumentException">シーン名は{MaxAllowSceneNameLength}文字を超えることは出来ません Name={sceneName} Length={sceneName.Length}</exception>
        /// <exception cref="ArgumentException">シーン名にアンダースコア2つから始めることは出来ません Name={sceneName}</exception>
        /// <exception cref="ArgumentException">シーン名に使えない文字が含まれています Name={sceneName} Invalid={chara}</exception>
        public override void ChangeScene(string sceneName, bool reset)
        {
            // どのようなパラメータで呼び出されたのかのログを吐く
            ThrowIfInvalidSceneName(sceneName);
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(ChangeScene)}({sceneName}, {reset})");
        }


        /// <summary>
        /// コメントのシーン内で特定のコンテキストを設定します
        /// </summary>
        /// <param name="context">設定するコンテキストの文字列。ただし、最大64文字のASCII文字列でなければなりません。</param>
        /// <exception cref="ArgumentException">コンテキストが null または 空文字列 です</exception>
        /// <exception cref="ArgumentException">コンテキストは{MaxAllowContextNameLength}文字を超えることは出来ません Name={context} Length={context.Length}</exception>
        /// <exception cref="ArgumentException">コンテキストに使えない文字が含まれています Name={context} Invalid={chara}</exception>
        public override void SetContext(string context)
        {
            // どのようなパラメータで呼び出されたのかのログを吐く
            ThrowIfInvalidContextName(context);
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(SetContext)}({context})");
        }


        /// <summary>
        /// 現在のコンテキストに対して状態を進めます
        /// </summary>
        /// <param name="factor">現在のコンテキストに対して状態の内容を示す文字列</param>
        /// <exception cref="ArgumentException">コンテキストファクタが null または 空文字列 です</exception>
        public override void PushContextFactor(string factor)
        {
            // どのようなパラメータで呼び出されたのかのログを吐く
            ThrowIfInvalidFactor(factor);
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(PushContextFactor)}({factor})");
        }


        /// <summary>
        /// 現在のコンテキストが特定コンテキストファクタの状態におけるマイナーコンテキストを進めます
        /// </summary>
        public override void PushMinorContext()
        {
            // どのようなパラメータで呼び出されたのかのログを吐く
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(PushMinorContext)}()");
        }
    }
}