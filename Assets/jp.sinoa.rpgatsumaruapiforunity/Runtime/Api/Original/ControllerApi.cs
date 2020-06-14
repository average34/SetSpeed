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
    /// RPGアツマールのコントローラを制御するクラスになります
    /// </summary>
    public class RpgAtsumaruController
    {
        // メンバ変数定義
        private uint inputPress;
        private uint inputDown;
        private uint inputUp;



        /// <summary>
        /// RpgAtsumaruController のインスタンスを初期化します
        /// </summary>
        /// <param name="receiver">RPGアツマールネイティブAPIコールバックを拾うレシーバ</param>
        internal RpgAtsumaruController(RpgAtsumaruApi.RpgAtsumaruApiCallbackReceiver receiver)
        {
        }


        /// <summary>
        /// RPGアツマールのコントローラ入力通知のリスンを開始します。
        /// コントローラAPIの操作をする前に必ず一度だけ呼ぶようにしてください。
        /// また StopControllerListen() 関数によって停止した場合は、もう一度呼び出してください。
        /// </summary>
        public virtual void StartControllerListen()
        {
            // 入力状態を初期化する
            inputPress = 0;
            inputDown = 0;
            inputUp = 0;


            // ネイティブAPI側の関数を叩く
            RpgAtsumaruNativeApi.StartControllerListen();
        }


        /// <summary>
        /// RPGアツマールのコントローラ入力通知のリスンを停止します。
        /// 入力制御を完全に停止する場合に使いますが、通常はリスンしたままにする事が推奨されます。
        /// </summary>
        public virtual void StopControllerListen()
        {
            // 入力状態を初期化する
            inputPress = 0;
            inputDown = 0;
            inputUp = 0;


            // ネイティブAPI側の関数を叩く
            RpgAtsumaruNativeApi.StopControllerListen();
        }


        /// <summary>
        /// RPGアツマールのコントローラ入力状態を元にUnity側の入力状態を更新します。
        /// 通常は、毎フレーム1度だけ呼び出してください。呼び出さない場合は、状態がロックされます。
        /// </summary>
        public virtual void Update()
        {
            // RPGアツマールの入力状態を取得して、Press、Down、Upの情報を更新する
            var currentState = RpgAtsumaruNativeApi.GetInputState();
            var diffState = inputPress ^ currentState;
            inputPress = currentState;
            inputDown = diffState & currentState;
            inputUp = diffState & ~currentState;
        }


        /// <summary>
        /// 指定されたキーが押されているかどうかを取得します
        /// </summary>
        /// <param name="key">確認したいキー</param>
        /// <returns>指定されたキーが押されている場合は true を、押されていない場合は false を返します</returns>
        public virtual bool GetButton(RpgAtsumaruInputKey key)
        {
            // Pressの状態をマスクして返す
            return (inputPress & (uint)key) != 0;
        }


        /// <summary>
        /// 指定されたキーが現在のフレームで押された瞬間かどうかを取得します
        /// </summary>
        /// <param name="key">確認したいキー</param>
        /// <returns>指定されたキーが押された瞬間の場合は true を、そうでない場合は false を返します</returns>
        public virtual bool GetButtonDown(RpgAtsumaruInputKey key)
        {
            // Downの状態をマスクして返す
            return (inputDown & (uint)key) != 0;
        }


        /// <summary>
        /// 指定されたキーが現在のフレームで離された瞬間かどうかを取得します
        /// </summary>
        /// <param name="key">確認したいキー</param>
        /// <returns>指定されたキーが離された瞬間の場合は true を、そうでない場合は false を返します</returns>
        public virtual bool GetButtonUp(RpgAtsumaruInputKey key)
        {
            // Upの状態をマスクして返す
            return (inputUp & (uint)key) != 0;
        }
    }
}