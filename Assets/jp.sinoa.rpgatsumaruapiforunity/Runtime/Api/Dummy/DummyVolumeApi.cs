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
    /// ボリュームAPIのダミークラスです。
    /// 実際の処理は行わず、何を呼び出されたかのログのみ出力します。
    /// </summary>
    internal sealed class DummyRpgAtsumaruVolume : RpgAtsumaruVolume
    {
        /// <summary>
        /// DummyRpgAtsumaruVolume クラスのインスタンスを初期化します
        /// </summary>
        /// <param name="receiver">RPGアツマールネイティブAPIコールバックを拾うレシーバ</param>
        internal DummyRpgAtsumaruVolume(RpgAtsumaruApi.RpgAtsumaruApiCallbackReceiver receiver) : base(receiver)
        {
        }



        /// <summary>
        /// RPGアツマールの音量調整に対して自動的にUnity側のマスター音量を調整するかどうか
        /// </summary>
        public override bool EnableAutoVolumeSync
        {
            get
            {
                // どのようなパラメータで呼び出されたのかのログを吐く
                InternalLogger.Log(LogLevel.Log, $"return {GetType().Name}.get_{nameof(EnableAutoVolumeSync)}");
                return enableAutoVolumeSync;
            }
            set
            {
                // どのようなパラメータで呼び出されたのかのログを吐く
                InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.set_{nameof(EnableAutoVolumeSync)}({value})");
                enableAutoVolumeSync = value;
            }
        }


        /// <summary>
        /// RPGアツマールの現在のマスター音量を取得します
        /// </summary>
        /// <returns>この関数は常に1.0を返します</returns>
        public override float GetCurrentVolume()
        {
            // TODO : 将来的にエディタウィンドウからエミュレーション出来るようになると良いかも
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(GetCurrentVolume)}()");
            return 1.0f;
        }


        /// <summary>
        /// RPGアツマールの音量調整バーの監視を開始します。
        /// 監視を開始すると自動的に音量調整バーが表示されます。
        /// </summary>
        public override void StartVolumeChangeListen()
        {
            // どのようなパラメータで呼び出されたのかのログを吐く
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(StartVolumeChangeListen)}()");
        }


        /// <summary>
        /// RPGアツマールの音量調整バーの監視を停止します。
        /// 監視を停止しても一度表示された音量調整バーは非表示にはなりません。
        /// </summary>
        public override void StopVolumeChangeListen()
        {
            // どのようなパラメータで呼び出されたのかのログを吐く
            InternalLogger.Log(LogLevel.Log, $"{GetType().Name}.{nameof(StopVolumeChangeListen)}()");
        }
    }
}