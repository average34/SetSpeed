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
using System.Threading.Tasks;
using IceMilkTea.Core;
using UnityEngine;

namespace RpgAtsumaruApiForUnity
{
    /// <summary>
    /// RPGアツマールのサーバーストレージを使ったセーブデータの管理を行うクラスです
    /// </summary>
    public class RpgAtsumaruStorage
    {
        // 定数定義
        private const string SystemSaveDataKeyName = "system";
        private const string PrefixGameDataKeyName = "data";

        // メンバ変数定義
        private ImtAwaitableManualReset getItemsAwaitable;
        private ImtAwaitableManualReset setItemsAwaitable;
        private ImtAwaitableManualReset syncItemsAwaitable;
        private ImtAwaitableManualReset removeItemAwaitable;
        private Dictionary<string, CachedData> saveDataTable;



        /// <summary>
        /// RpgAtsumaruStorage のインスタンスを初期化します
        /// </summary>
        /// <param name="receiver">RPGアツマールネイティブAPIコールバックを拾うレシーバ</param>
        internal RpgAtsumaruStorage(RpgAtsumaruApi.RpgAtsumaruApiCallbackReceiver receiver)
        {
            // レシーバにイベントを登録する
            receiver.StorageItemsReceived += OnStorageItemsReceived;
            receiver.StorageSetItemsCompleted += OnStorageSetItemCompleted;
            receiver.StorageRemoveItemCompleted += OnStorageRemoveItemCompleted;


            // マニュアルリセット待機可能オブジェクトをシグナル状態で生成する
            getItemsAwaitable = new ImtAwaitableManualReset(true);
            setItemsAwaitable = new ImtAwaitableManualReset(true);
            syncItemsAwaitable = new ImtAwaitableManualReset(true);
            removeItemAwaitable = new ImtAwaitableManualReset(true);


            // セーブデータテーブルのインスタンスを生成
            saveDataTable = new Dictionary<string, CachedData>();
        }


        /// <summary>
        /// RPGアツマールのサーバーストレージからすべてのデータをjsonで受け取ったときのイベントを処理します
        /// </summary>
        /// <param name="jsonData">受け取ったデータのjson文字列データ</param>
        private void OnStorageItemsReceived(string jsonData)
        {
            // Jsonデータからセーブデータへデシリアライズしてセーブデータテーブルをクリア
            var saveData = JsonUtility.FromJson<RpgAtsumaruSaveData>(jsonData);
            saveDataTable.Clear();


            // セーブデータレコードの数分ループ
            var saveDataItems = saveData.SaveDataItems;
            for (int i = 0; i < saveDataItems.Length; ++i)
            {
                // レコードの内容をそのまますべて受け取る
                var key = saveDataItems[i].key;
                var value = saveDataItems[i].value;
                saveDataTable[key] = new CachedData(key, value);
            }


            // データの準備ができたのでシグナルを設定
            getItemsAwaitable.Set();
        }


        /// <summary>
        /// RPGアツマールのサーバーストレージにデータを設定した完了イベントを処理します
        /// </summary>
        private void OnStorageSetItemCompleted()
        {
            // サーバーにデータの設定ができたシグナルを設定
            setItemsAwaitable.Set();
        }


        /// <summary>
        /// RPGアツマールのサーバーストレージからデータを削除した完了イベントを処理します
        /// </summary>
        private void OnStorageRemoveItemCompleted()
        {
            // サーバーからデータの削除ができたシグナルを設定
            removeItemAwaitable.Set();
        }


        /// <summary>
        /// RPGアツマールのサーバーストレージとセーブデータの同期を非同期で行います。
        /// ゲーム側が設定したセーブデータが書き込まれた後、サーバーのセーブデータがロードされます。
        /// また、実際のセーブデータの削除の同期もこのタイミングで行われます。
        /// </summary>
        /// <returns>セーブデータの同期操作を行っているタスクを返します</returns>
        public virtual async Task SyncSaveDataAsync()
        {
            // もし同期待機オブジェクトが完了状態なら
            if (syncItemsAwaitable.IsCompleted)
            {
                // 同期待機オブジェクトをリセットする
                syncItemsAwaitable.Reset();


                // ダーティデータのリストと削除するべきリストの用意
                List<CachedData> dirtyDataList = new List<CachedData>();
                List<CachedData> removeDataList = new List<CachedData>();


                // セーブデータの数分ループ
                foreach (var saveData in saveDataTable.Values)
                {
                    // セーブデータがダーティなら
                    if (saveData.Dirty)
                    {
                        // セーブデータがnullなら
                        if (saveData.SaveData == null)
                        {
                            // nullの場合は削除対象
                            removeDataList.Add(saveData);
                        }
                        else
                        {
                            // 汚れリストに追加
                            dirtyDataList.Add(saveData);
                        }
                    }
                }


                // 削除データの件数が1件以上あるなら
                if (removeDataList.Count >= 1)
                {
                    // 削除データの件数分ループ
                    foreach (var removeSaveData in removeDataList)
                    {
                        // 削除待機オブジェクトをリセットしてRPGアツマールのサーバーストレージから削除をする
                        removeItemAwaitable.Reset();
                        RpgAtsumaruNativeApi.RemoveStorageItem(removeSaveData.Key);
                        await removeItemAwaitable;
                    }
                }


                // ダーティなデータの件数が1件以上あるなら
                if (dirtyDataList.Count >= 1)
                {
                    // サーバーに同期するためのデータを生成する
                    var syncSaveData = new RpgAtsumaruSaveData();
                    syncSaveData.SaveDataItems = new RpgAtsumaruDataRecord[dirtyDataList.Count];


                    // 同期用データにダーティなデータを渡す
                    for (int i = 0; i < dirtyDataList.Count; ++i)
                    {
                        // ダーティなデータを受け取ってダーティをリセット
                        syncSaveData.SaveDataItems[i] = new RpgAtsumaruDataRecord() { key = dirtyDataList[i].Key, value = dirtyDataList[i].SaveData };
                        dirtyDataList[i].ClearDirty();
                    }


                    // 同期セーブデータをJSONシリアライズしてRPGアツマールサーバーへ送って待機する
                    setItemsAwaitable.Reset();
                    var saveDataJson = JsonUtility.ToJson(syncSaveData);
                    RpgAtsumaruNativeApi.SetStorageItems(saveDataJson);
                    await setItemsAwaitable;
                }


                // こんどはRPGアツマールサーバーから最新のデータを受け取る
                getItemsAwaitable.Reset();
                RpgAtsumaruNativeApi.GetStorageItems();
                await getItemsAwaitable;


                // 同期が完了したことをシグナルで設定
                syncItemsAwaitable.Set();
            }


            // 同期待機オブジェクトを待機する
            await syncItemsAwaitable;
        }


        /// <summary>
        /// 指定されたセーブデータをシステムデータとして設定します。
        /// また null を設定するとセーブデータの削除として扱われます。
        /// </summary>
        /// <param name="systemData">設定するシステムデータ。nullを指定すると削除対象とみなされます。</param>
        public virtual void SetSystemData(string systemData)
        {
            // もしセーブデータテーブルに既存のデータが存在するなら
            if (saveDataTable.TryGetValue(SystemSaveDataKeyName, out var saveData))
            {
                // 更新して終了
                saveData.Update(systemData);
                return;
            }


            // システムファイルのキーで新しく作って更新をマーク
            saveDataTable[SystemSaveDataKeyName] = new CachedData(SystemSaveDataKeyName, systemData);
            saveDataTable[SystemSaveDataKeyName].Update(systemData);
        }


        /// <summary>
        /// システムデータを取得します
        /// </summary>
        /// <returns>システムデータを取得した場合はその内容を、システムデータが存在しない場合は null を返します</returns>
        public virtual string GetSystemData()
        {
            // もしセーブデータテーブルに既存のデータが存在するなら
            if (saveDataTable.TryGetValue(SystemSaveDataKeyName, out var saveData))
            {
                // 内容を返す
                return saveData.SaveData;
            }


            // データが無いのならnullを返す
            return null;
        }


        /// <summary>
        /// システムセーブデータが含まれているかどうかを取得します
        /// </summary>
        /// <returns>システムセーブデータが含まれている場合は true を、含まれていない場合は false を返します</returns>
        public virtual bool ContainsSystemData()
        {
            // システムセーブデータの取得結果がnull以外が存在するとする
            return GetSystemData() != null;
        }


        /// <summary>
        /// 指定されたスロットIDにセーブデータを設定します。
        /// 約1KB=1ブロックとして扱われます（実際は圧縮されるため少し小さくなる可能性があります）
        /// また null を設定するとセーブデータの削除として扱われます。
        /// </summary>
        /// <param name="slotId">0から始まるスロットID</param>
        /// <param name="saveData">セーブするセーブデータ。nullを指定すると削除対象とみなされます。</param>
        /// <exception cref="ArgumentOutOfRangeException">スロットIDに0未満の値が指定されました</exception>
        public virtual void SetSaveData(int slotId, string saveData)
        {
            // 例外判定を入れる
            ThrowIfOutOfRangeSlotId(slotId);


            // キー名を生成してセーブデータテーブルに既存のデータが存在するなら
            var key = PrefixGameDataKeyName + slotId.ToString();
            if (saveDataTable.TryGetValue(key, out var data))
            {
                // 更新して終了
                data.Update(saveData);
                return;
            }


            // システムファイルのキーで新しく作って更新をマーク
            saveDataTable[key] = new CachedData(key, saveData);
            saveDataTable[key].Update(saveData);
        }


        /// <summary>
        /// 指定されたスロットIDのセーブデータを取得します
        /// </summary>
        /// <param name="slotId">取得したいセーブデータのスロットID</param>
        /// <returns>セーブデータを取得した場合はその内容を、セーブデータが存在しない場合は null を返します</returns>
        /// <exception cref="ArgumentOutOfRangeException">スロットIDに0未満の値が指定されました</exception>
        public virtual string GetSaveData(int slotId)
        {
            // 例外判定を入れる
            ThrowIfOutOfRangeSlotId(slotId);


            // キー名を生成してセーブデータテーブルに既存のデータが存在するなら
            var key = PrefixGameDataKeyName + slotId.ToString();
            if (saveDataTable.TryGetValue(key, out var data))
            {
                // 内容を返す
                return data.SaveData;
            }


            // データが無いのならnullを返す
            return null;
        }


        /// <summary>
        /// 指定されたスロットIDが含まれているか確認をします
        /// </summary>
        /// <param name="slotId">確認したいスロットID</param>
        /// <returns>対象スロットIDのセーブデータが含まれている場合は true を、含まれていない場合は false を返します</returns>
        /// <exception cref="ArgumentOutOfRangeException">スロットIDに0未満の値が指定されました</exception>
        public virtual bool ContainsSaveData(int slotId)
        {
            // 値を取得してnull以外がデータが含まれていると判定する
            return GetSaveData(slotId) != null;
        }


        /// <summary>
        /// システムデータ以外の、全ての有効なセーブデータのスロットIDを配列で取得します
        /// </summary>
        /// <returns>有効なセーブデータを保有するスロットIDを配列で返します。もし1件も無い場合でも長さ0の配列を返します。</returns>
        public virtual int[] GetAllSaveDataSlotId()
        {
            // テーブルがそもそも空 または 1件だがシステムデータ なら
            if (saveDataTable.Count == 0 || (saveDataTable.Count == 1 && ContainsSystemData()))
            {
                // 長さ0の配列を返す
                return Array.Empty<int>();
            }


            // 返却用配列を生成する
            var result = new int[ContainsSystemData() ? saveDataTable.Count - 1 : saveDataTable.Count];


            // テーブルのレコード分回る
            int insertIndex = 0;
            foreach (var saveData in saveDataTable.Values)
            {
                // もしシステムデータ または データがnull なら
                if (saveData.Key == SystemSaveDataKeyName || saveData.SaveData == null)
                {
                    // そのまま次へ
                    continue;
                }


                // セーブデータのプレフィックスを削除した文字列をintにパースして配列に詰める
                result[insertIndex++] = int.Parse(saveData.Key.Replace(PrefixGameDataKeyName, ""));
            }


            // 完成品を返す
            return result;
        }


        /// <summary>
        /// スロットIDが、もし0未満の値を示した場合は例外をスローします
        /// </summary>
        /// <param name="slotId">確認するスロットID</param>
        /// <exception cref="ArgumentOutOfRangeException">スロットIDに0未満の値が指定されました</exception>
        protected void ThrowIfOutOfRangeSlotId(int slotId)
        {
            // スロットIDに負の値が指定されていたのなら
            if (slotId < 0)
            {
                // 流石に負の値のスロット番号は無理
                throw new ArgumentOutOfRangeException(nameof(slotId), "スロットIDに0未満の値が指定されました");
            }
        }



        /// <summary>
        /// RPGアツマール側ではなくメモリ上にセーブデータをキャッシュしたデータを保持するクラスです
        /// </summary>
        private class CachedData
        {
            /// <summary>
            /// キャッシュされたデータのキー名
            /// </summary>
            public string Key { get; private set; }


            /// <summary>
            /// キャッシュされたセーブデータ
            /// </summary>
            public string SaveData { get; private set; }


            /// <summary>
            /// キャッシュセーブデータがダーティ状態かどうか（同期するべきかどうか）
            /// </summary>
            public bool Dirty { get; private set; }



            /// <summary>
            /// CachedData のインスタンスを初期化します
            /// </summary>
            /// <param name="key">セーブデータのキー名</param>
            /// <param name="saveData">初期セーブデータの内容</param>
            public CachedData(string key, string saveData)
            {
                // 初期化をする
                Key = key;
                SaveData = saveData;
                Dirty = false;
            }


            /// <summary>
            /// 指定されたセーブデータで内容を更新します。
            /// また、ダーティフラグがセットされます。
            /// </summary>
            /// <param name="saveData">更新するセーブデータ内容</param>
            public void Update(string saveData)
            {
                // セーブデータ内容を受け取ってダーティフラグを設定
                SaveData = saveData;
                Dirty = true;
            }


            /// <summary>
            /// ダーティフラグをリセットします
            /// </summary>
            public void ClearDirty()
            {
                // ダーティフラグを折る
                Dirty = false;
            }
        }
    }
}