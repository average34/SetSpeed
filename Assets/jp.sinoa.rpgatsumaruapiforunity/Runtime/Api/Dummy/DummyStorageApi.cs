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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RpgAtsumaruApiForUnity
{
    /// <summary>
    /// ストレージAPIのダミークラスです。
    /// 内部で簡易なストレージの記録機能を再現した動作を提供します。
    /// </summary>
    internal sealed class DummyRpgAtsumaruStorage : RpgAtsumaruStorage
    {
        // 定数定義
        private const string SaveFileName = "storageapidata.json";

        // メンバ変数定義
        private string saveFilePath;
        private string systemData;
        private Dictionary<int, string> saveDataTable;



        /// <summary>
        /// DummyRpgAtsumaruStorage のインスタンスを初期化します
        /// </summary>
        /// <param name="receiver">RPGアツマールネイティブAPIコールバックを拾うレシーバ</param>
        internal DummyRpgAtsumaruStorage(RpgAtsumaruApi.RpgAtsumaruApiCallbackReceiver receiver) : base(receiver)
        {
            // ファイルパスを生成してセーブデータテーブルも初期化
            saveFilePath = Path.Combine(UnityEngine.Application.persistentDataPath, SaveFileName).Replace("\\", "/");
            saveDataTable = new Dictionary<int, string>();
        }


        /// <summary>
        /// RPGアツマールのサーバーストレージとセーブデータの同期を非同期で行います。
        /// ゲーム側が設定したセーブデータが書き込まれた後、サーバーのセーブデータがロードされます。
        /// また、実際のセーブデータの削除の同期もこのタイミングで行われます。
        /// </summary>
        /// <returns>セーブデータの同期操作を行っているタスクを返します</returns>
        public override Task SyncSaveDataAsync()
        {
            // 非同期でセーブデータの同期を行うタスクを実行する
            return Task.Run(() =>
            {
                // ファイルが存在するなら
                if (File.Exists(saveFilePath))
                {
                    // ファイルの内容を全て読み込んでセーブデータとしてデシリアライズする
                    var jsonData = File.ReadAllText(saveFilePath);
                    var fileSaveData = UnityEngine.JsonUtility.FromJson<SaveData>(jsonData);


                    // 読み込まれたセーブデータのシステムデータを受け取るが、UnityのJsonパーサの都合上
                    // 空文字列もデータが無いと扱うため、nullまたは空文字列の場合は null とする
                    systemData = string.IsNullOrEmpty(fileSaveData.SystemData) ? null : fileSaveData.SystemData;


                    // 全レコード分回る
                    var records = fileSaveData.Records;
                    for (int i = 0; i < fileSaveData.Records.Length; ++i)
                    {
                        // もしセーブデータがnullなら
                        if (records[i].SaveData == null)
                        {
                            // 次へ
                            continue;
                        }


                        // キーと値をそのまま入れていく
                        saveDataTable[records[i].SlotId] = records[i].SaveData;
                    }
                }


                // 書き込むためのセーブデータ構造体のインスタンスを宣言
                var saveData = new SaveData();


                // システムデータを設定
                saveData.SystemData = systemData;


                // レコードの数だけインスタンスを作ってテーブルの内容で回る
                saveData.Records = new SaveDataRecord[saveDataTable.Count];
                int saveDataIndex = 0;
                foreach (var record in saveDataTable)
                {
                    // セーブデータを設定する
                    saveData.Records[saveDataIndex].SlotId = record.Key;
                    saveData.Records[saveDataIndex].SaveData = record.Value;
                    ++saveDataIndex;
                }


                // ファイルにデータを書き込む
                File.WriteAllText(saveFilePath, UnityEngine.JsonUtility.ToJson(saveData));


                // 同期が完了したことをログに出す
                InternalLogger.Log(LogLevel.Log, "StorageAPI : SaveSyncComplete.");
            });
        }


        /// <summary>
        /// 指定されたセーブデータをシステムデータとして設定します。
        /// また null を設定するとセーブデータの削除として扱われます。
        /// </summary>
        /// <param name="systemData">設定するシステムデータ。nullを指定すると削除対象とみなされます。</param>
        public override void SetSystemData(string systemData)
        {
            // そのまま受け取る
            this.systemData = systemData;
        }


        /// <summary>
        /// システムデータを取得します
        /// </summary>
        /// <returns>システムデータを取得した場合はその内容を、システムデータが存在しない場合は null を返します</returns>
        public override string GetSystemData()
        {
            // そのまま返す
            return systemData;
        }


        /// <summary>
        /// システムセーブデータが含まれているかどうかを取得します
        /// </summary>
        /// <returns>システムセーブデータが含まれている場合は true を、含まれていない場合は false を返します</returns>
        public override bool ContainsSystemData()
        {
            // nullではないかどうかの判定をそのまま返す
            return systemData != null;
        }


        /// <summary>
        /// 指定されたスロットIDにセーブデータを設定します。
        /// 約1KB=1ブロックとして扱われます（実際は圧縮されるため少し小さくなる可能性があります）
        /// また null を設定するとセーブデータの削除として扱われます。
        /// </summary>
        /// <param name="slotId">0から始まるスロットID</param>
        /// <param name="saveData">セーブするセーブデータ。nullを指定すると削除対象とみなされます。</param>
        /// <exception cref="ArgumentOutOfRangeException">スロットIDに0未満の値が指定されました</exception>
        public override void SetSaveData(int slotId, string saveData)
        {
            // 例外判定を入れる
            ThrowIfOutOfRangeSlotId(slotId);


            // もし null データを渡されたら
            if (saveData == null)
            {
                // 該当キーのデータを削除して終了
                saveDataTable.Remove(slotId);
                return;
            }


            // 値をそのまま設定する
            saveDataTable[slotId] = saveData;
        }


        /// <summary>
        /// 指定されたスロットIDのセーブデータを取得します
        /// </summary>
        /// <param name="slotId">取得したいセーブデータのスロットID</param>
        /// <returns>セーブデータを取得した場合はその内容を、セーブデータが存在しない場合は null を返します</returns>
        /// <exception cref="ArgumentOutOfRangeException">スロットIDに0未満の値が指定されました</exception>
        public override string GetSaveData(int slotId)
        {
            // 例外判定を入れる
            ThrowIfOutOfRangeSlotId(slotId);


            // TryGetValueの値をそのまま返す
            saveDataTable.TryGetValue(slotId, out var saveData);
            return saveData;
        }


        /// <summary>
        /// 指定されたスロットIDが含まれているか確認をします
        /// </summary>
        /// <param name="slotId">確認したいスロットID</param>
        /// <returns>対象スロットIDのセーブデータが含まれている場合は true を、含まれていない場合は false を返します</returns>
        /// <exception cref="ArgumentOutOfRangeException">スロットIDに0未満の値が指定されました</exception>
        public override bool ContainsSaveData(int slotId)
        {
            // 例外判定を入れる
            ThrowIfOutOfRangeSlotId(slotId);


            // ContainsKeyの結果をそのまま返す
            return saveDataTable.ContainsKey(slotId);
        }


        /// <summary>
        /// システムデータ以外の、全ての有効なセーブデータのスロットIDを配列で取得します
        /// </summary>
        /// <returns>有効なセーブデータを保有するスロットIDを配列で返します。もし1件も無い場合でも長さ0の配列を返します。</returns>
        public override int[] GetAllSaveDataSlotId()
        {
            // Key列挙をそのまま返す
            var keys = new int[saveDataTable.Count];
            saveDataTable.Keys.CopyTo(keys, 0);
            return keys;
        }



        /// <summary>
        /// ファイルとして出力されるセーブデータ全体を表す構造体です
        /// </summary>
        [Serializable]
        private struct SaveData
        {
            /// <summary>
            /// システムデータを保持します
            /// </summary>
            public string SystemData;


            /// <summary>
            /// セーブデータレコード情報を保持します
            /// </summary>
            public SaveDataRecord[] Records;
        }



        /// <summary>
        /// データスロットのレコード単位を表す構造体です
        /// </summary>
        [Serializable]
        private struct SaveDataRecord
        {
            /// <summary>
            /// スロットID
            /// </summary>
            public int SlotId;


            /// <summary>
            /// データスロットのセーブデータ
            /// </summary>
            public string SaveData;
        }
    }
}