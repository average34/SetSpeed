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
using UnityEngine.SceneManagement;

namespace RpgAtsumaruApiForUnity
{
    /// <summary>
    /// RPGアツマールのコメントを制御するクラスになります
    /// </summary>
    public class RpgAtsumaruComment
    {
        // 定数定義
        private const int MaxAllowSceneNameLength = 64;
        private const int MaxAllowContextNameLength = 64;



        /// <summary>
        /// RpgAtsumaruComment のインスタンスを初期化します
        /// </summary>
        /// <param name="receiver">RPGアツマールネイティブAPIコールバックを拾うレシーバ</param>
        internal RpgAtsumaruComment(RpgAtsumaruApi.RpgAtsumaruApiCallbackReceiver receiver)
        {
        }


        /// <summary>
        /// 現在のUnityのアクティブなシーン名を用いて、コメントシーンを切り替えます。
        /// また、コメントシーンの状態をリセットします。
        /// </summary>
        /// <exception cref="ArgumentException">シーン名が null または 空文字列 です</exception>
        /// <exception cref="ArgumentException">シーン名は{MaxAllowSceneNameLength}文字を超えることは出来ません Name={sceneName} Length={sceneName.Length}</exception>
        /// <exception cref="ArgumentException">シーン名にアンダースコア2つから始めることは出来ません Name={sceneName}</exception>
        /// <exception cref="ArgumentException">シーン名に使えない文字が含まれています Name={sceneName} Invalid={chara}</exception>
        public void ChangeScene()
        {
            // 現在のシーン名を渡して状態をリセットする
            ChangeScene(SceneManager.GetActiveScene().name, true);
        }


        /// <summary>
        /// 現在のUnityのアクティブなシーン名を用いて、コメントシーンを切り替えます。
        /// </summary>
        /// <param name="reset">コメントシーンの状態をリセットする場合は true</param>
        /// <exception cref="ArgumentException">シーン名が null または 空文字列 です</exception>
        /// <exception cref="ArgumentException">シーン名は{MaxAllowSceneNameLength}文字を超えることは出来ません Name={sceneName} Length={sceneName.Length}</exception>
        /// <exception cref="ArgumentException">シーン名にアンダースコア2つから始めることは出来ません Name={sceneName}</exception>
        /// <exception cref="ArgumentException">シーン名に使えない文字が含まれています Name={sceneName} Invalid={chara}</exception>
        public void ChangeScene(bool reset)
        {
            // 現在のシーン名を渡して切り替える
            ChangeScene(SceneManager.GetActiveScene().name, reset);
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
        public virtual void ChangeScene(string sceneName, bool reset)
        {
            // 例外判定を入れる
            ThrowIfInvalidSceneName(sceneName);


            // リセットする場合は
            if (reset)
            {
                // リセットしながらシーンを切り替えるネイティブプラグイン関数を叩く
                RpgAtsumaruNativeApi.ResetAndChangeScene(sceneName);
            }
            else
            {
                // そのままシーンを切り替えるネイティブプラグイン関数を叩く
                RpgAtsumaruNativeApi.ChangeScene(sceneName);
            }
        }


        /// <summary>
        /// コメントのシーン内で特定のコンテキストを設定します
        /// </summary>
        /// <param name="context">設定するコンテキストの文字列。ただし、最大64文字のASCII文字列でなければなりません。</param>
        /// <exception cref="ArgumentException">コンテキストが null または 空文字列 です</exception>
        /// <exception cref="ArgumentException">コンテキストは{MaxAllowContextNameLength}文字を超えることは出来ません Name={context} Length={context.Length}</exception>
        /// <exception cref="ArgumentException">コンテキストに使えない文字が含まれています Name={context} Invalid={chara}</exception>
        public virtual void SetContext(string context)
        {
            // 例外判定を入れてからネイティブプラグイン関数を叩く
            ThrowIfInvalidContextName(context);
            RpgAtsumaruNativeApi.SetContext(context);
        }


        /// <summary>
        /// 現在のコンテキストに対して状態を進めます
        /// </summary>
        /// <param name="factor">現在のコンテキストに対して状態の内容を示す文字列</param>
        /// <exception cref="ArgumentException">コンテキストファクタが null または 空文字列 です</exception>
        public virtual void PushContextFactor(string factor)
        {
            // 例外判定を入れてからネイティブプラグイン関数を叩く
            ThrowIfInvalidFactor(factor);
            RpgAtsumaruNativeApi.PushContextFactor(factor);
        }


        /// <summary>
        /// 現在のコンテキストが特定コンテキストファクタの状態におけるマイナーコンテキストを進めます
        /// </summary>
        public virtual void PushMinorContext()
        {
            // ネイティブプラグイン関数を叩く
            RpgAtsumaruNativeApi.PushMinorContext();
        }


        /// <summary>
        /// シーン名がAPIとして使えない場合に例外をスローします
        /// </summary>
        /// <param name="sceneName">確認するシーン名</param>
        /// <exception cref="ArgumentException">シーン名が null または 空文字列 です</exception>
        /// <exception cref="ArgumentException">シーン名は{MaxAllowSceneNameLength}文字を超えることは出来ません Name={sceneName} Length={sceneName.Length}</exception>
        /// <exception cref="ArgumentException">シーン名にアンダースコア2つから始めることは出来ません Name={sceneName}</exception>
        /// <exception cref="ArgumentException">シーン名に使えない文字が含まれています Name={sceneName} Invalid={chara}</exception>
        protected void ThrowIfInvalidSceneName(string sceneName)
        {
            // もし空文字列かnullを渡されたら
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                // 一体どのようなシーンへ切り替えるつもりだったのか
                throw new ArgumentException("シーン名が null または 空文字列 です", nameof(sceneName));
            }


            // 既定文字数を超過しているのなら
            if (sceneName.Length > MaxAllowSceneNameLength)
            {
                // 文字数超過例外を吐く
                throw new ArgumentException($"シーン名は{MaxAllowSceneNameLength}文字を超えることは出来ません Name={sceneName} Length={sceneName.Length}", nameof(sceneName));
            }


            // シーン名が2文字以上でかつアンダースコアが2つ先頭についていた場合は
            if (sceneName.Length >= 2 && sceneName.StartsWith("__"))
            {
                // アンダースコアが文字列の先頭に2つついていることは許されない
                throw new ArgumentException($"シーン名にアンダースコア2つから始めることは出来ません Name={sceneName}", nameof(sceneName));
            }


            // 文字列内の文字を全て回る
            foreach (var chara in sceneName)
            {
                // もしASCIIコードの印字可能な文字の範囲外なら
                if (chara < ' ' || chara > '~')
                {
                    // シーン名に使えない文字を利用しようとした例外を吐く
                    throw new ArgumentException($"シーン名に使えない文字が含まれています Name={sceneName} Invalid={chara}", nameof(sceneName));
                }
            }
        }


        /// <summary>
        /// コンテキスト名がAPIとして使えない場合に例外をスローします
        /// </summary>
        /// <param name="context">確認するコンテキスト</param>
        /// <exception cref="ArgumentException">コンテキストが null または 空文字列 です</exception>
        /// <exception cref="ArgumentException">コンテキストは{MaxAllowContextNameLength}文字を超えることは出来ません Name={context} Length={context.Length}</exception>
        /// <exception cref="ArgumentException">コンテキストに使えない文字が含まれています Name={context} Invalid={chara}</exception>
        protected void ThrowIfInvalidContextName(string context)
        {
            // もし空文字列かnullを渡されたら
            if (string.IsNullOrWhiteSpace(context))
            {
                // 一体どのようなコンテキストを設定するつもりだったのか
                throw new ArgumentException("コンテキストが null または 空文字列 です", nameof(context));
            }


            // 既定文字数を超過しているのなら
            if (context.Length > MaxAllowContextNameLength)
            {
                // 文字数超過例外を吐く
                throw new ArgumentException($"コンテキストは{MaxAllowContextNameLength}文字を超えることは出来ません Name={context} Length={context.Length}", nameof(context));
            }


            // 文字列内の文字を全て回る
            foreach (var chara in context)
            {
                // もしASCIIコードの印字可能な文字の範囲外なら
                if (chara < ' ' || chara > '~')
                {
                    // コンテキスト使えない文字を利用しようとした例外を吐く
                    throw new ArgumentException($"コンテキストに使えない文字が含まれています Name={context} Invalid={chara}", nameof(context));
                }
            }
        }


        /// <summary>
        /// ファクタがAPIとして使えない場合に例外をスローします
        /// </summary>
        /// <param name="factor">確認するファクタ</param>
        /// <exception cref="ArgumentException">コンテキストファクタが null または 空文字列 です</exception>
        protected void ThrowIfInvalidFactor(string factor)
        {
            // もし空文字列かnullを渡されたら
            if (string.IsNullOrWhiteSpace(factor))
            {
                // 一体どのようなコンテキストファクタをプッシュするつもりだったのか
                throw new ArgumentException("コンテキストファクタが null または 空文字列 です", nameof(factor));
            }
        }
    }
}