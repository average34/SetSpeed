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
    /// コントローラAPIのダミークラスです。
    /// 実際の処理は行わず、何を呼び出されたかのログのみ出力します。
    /// </summary>
    internal sealed class DummyRpgAtsumaruController : RpgAtsumaruController
    {
        /// <summary>
        /// DummyRpgAtsumaruController クラスのインスタンスを初期化します
        /// </summary>
        /// <param name="receiver">RPGアツマールネイティブAPIコールバックを拾うレシーバ</param>
        internal DummyRpgAtsumaruController(RpgAtsumaruApi.RpgAtsumaruApiCallbackReceiver receiver) : base(receiver)
        {
        }


        /// <summary>
        /// RPGアツマールのコントローラ入力通知のリスンを開始します。
        /// コントローラAPIの操作をする前に必ず一度だけ呼ぶようにしてください。
        /// また StopControllerListen() 関数によって停止した場合は、もう一度呼び出してください。
        /// </summary>
        public override void StartControllerListen()
        {
            // どのようなパラメータで呼び出されたのかのログを吐く
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(StartControllerListen)}()");
        }


        /// <summary>
        /// RPGアツマールのコントローラ入力通知のリスンを停止します。
        /// 入力制御を完全に停止する場合に使いますが、通常はリスンしたままにする事が推奨されます。
        /// </summary>
        public override void StopControllerListen()
        {
            // どのようなパラメータで呼び出されたのかのログを吐く
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(StopControllerListen)}()");
        }


        /// <summary>
        /// RPGアツマールのコントローラ入力状態を元にUnity側の入力状態を更新します。
        /// 通常は、毎フレーム1度だけ呼び出してください。呼び出さない場合は、状態がロックされます。
        /// </summary>
        public override void Update()
        {
            // 何もしない
        }


        /// <summary>
        /// 指定されたキーが押されているかどうかを取得します
        /// </summary>
        /// <param name="key">確認したいキー</param>
        /// <returns>この関数は常にfalseを返します</returns>
        public override bool GetButton(RpgAtsumaruInputKey key)
        {
            // TODO : 将来的にエディタウィンドウからエミュレーション出来るようになると良いかも
            return false;
        }


        /// <summary>
        /// 指定されたキーが現在のフレームで押された瞬間かどうかを取得します
        /// </summary>
        /// <param name="key">確認したいキー</param>
        /// <returns>この関数は常にfalseを返します</returns>
        public override bool GetButtonDown(RpgAtsumaruInputKey key)
        {
            // TODO : 将来的にエディタウィンドウからエミュレーション出来るようになると良いかも
            return false;
        }


        /// <summary>
        /// 指定されたキーが現在のフレームで離された瞬間かどうかを取得します
        /// </summary>
        /// <param name="key">確認したいキー</param>
        /// <returns>この関数は常にfalseを返します</returns>
        public override bool GetButtonUp(RpgAtsumaruInputKey key)
        {
            // TODO : 将来的にエディタウィンドウからエミュレーション出来るようになると良いかも
            return false;
        }
    }
}