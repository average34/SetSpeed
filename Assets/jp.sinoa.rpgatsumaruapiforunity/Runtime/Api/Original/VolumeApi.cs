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
using UnityEngine;

namespace RpgAtsumaruApiForUnity
{
    /// <summary>
    /// RPGアツマールのマスター音量を制御するクラスです
    /// </summary>
    public class RpgAtsumaruVolume
    {
        // メンバ変数定義
        protected bool enableAutoVolumeSync;



        /// <summary>
        /// RPGアツマールの音量調整によって変化されたときのイベントです
        /// </summary>
        public event Action<float> VolumeChanged;



        /// <summary>
        /// RPGアツマールの音量調整に対して自動的にUnity側のマスター音量を調整するかどうか
        /// </summary>
        public virtual bool EnableAutoVolumeSync
        {
            get
            {
                // 現在の設定をそのまま返す
                return enableAutoVolumeSync;
            }
            set
            {
                // 値をそのまま受け取り、自動調整が有効なら
                enableAutoVolumeSync = value;
                if (enableAutoVolumeSync)
                {
                    // 現在のマスター音量をそのまま設定する
                    AudioListener.volume = GetCurrentVolume();
                }
            }
        }



        /// <summary>
        /// RpgAtsumaruVolume のインスタンスを初期化します
        /// </summary>
        /// <param name="receiver">RPGアツマールネイティブAPIコールバックを拾うレシーバ</param>
        internal RpgAtsumaruVolume(RpgAtsumaruApi.RpgAtsumaruApiCallbackReceiver receiver)
        {
            // レシーバにイベントを登録する
            receiver.VolumeChanged += OnVolumeChanged;


            // 既定動作は自動調整をON（プロパティから設定すると音量変更まで直ちにやってしまうので直接初期化）
            enableAutoVolumeSync = true;
        }


        /// <summary>
        /// RPGアツマールのマスター音量を変更したときの通知イベントを処理します
        /// </summary>
        /// <param name="volume">変更された音量（OFF 0.0 ～ 1.0 ON）</param>
        private void OnVolumeChanged(float volume)
        {
            // 自動同期が有効なら
            if (enableAutoVolumeSync)
            {
                // 現在のマスター音量をそのまま設定する
                AudioListener.volume = volume;
            }


            // イベントを呼ぶ
            VolumeChanged?.Invoke(volume);
        }


        /// <summary>
        /// RPGアツマールの現在のマスター音量を取得します
        /// </summary>
        /// <returns>マスター音量を（OFF 0.0 ～ 1.0 ON）の値で返します</returns>
        public virtual float GetCurrentVolume()
        {
            // プラグインから音量の取得をする
            return RpgAtsumaruNativeApi.GetCurrentVolume();
        }


        /// <summary>
        /// RPGアツマールの音量調整バーの監視を開始します。
        /// 監視を開始すると自動的に音量調整バーが表示されます。
        /// </summary>
        public virtual void StartVolumeChangeListen()
        {
            // プラグインの音量監視開始APIを叩く
            RpgAtsumaruNativeApi.StartVolumeListen();
        }


        /// <summary>
        /// RPGアツマールの音量調整バーの監視を停止します。
        /// 監視を停止しても一度表示された音量調整バーは非表示にはなりません。
        /// </summary>
        public virtual void StopVolumeChangeListen()
        {
            // プラグインの音量監視停止APIを叩く
            RpgAtsumaruNativeApi.StopVolumeListen();
        }
    }
}