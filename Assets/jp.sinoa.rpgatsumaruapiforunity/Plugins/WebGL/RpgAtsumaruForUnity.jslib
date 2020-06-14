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

// (確認UnityVersion 2018.3.0b7)
// Unity用RPGアツマールオブジェクトを宣言
var RpgAtsumaruApiForUnity =
{
    // プラグインの状態を保持するコンテキストテーブルです
    $Context:
    {
        initialized: false,           // プラグインが初期化されたかどうか
        unityObjectName: "",          // 非同期操作時にUnityへ通知するためのゲームオブジェクト名
        volumeSubscription: null,     // 音量APIのサブスクリプション参照
        controllerSubscription: null, // コントローラAPIのサブスクリプション参照
        inputPress: 0,                // コントローラの押し込み状態を保持する
        screenShotCallback: null,     // スクリーンショットコールバック関数（第一引数にDataUrl形式の文字列を渡す必要あり）
        unityMethodNames:             // 非同期的にUnityへ通知するための各種メソッド名
        {
            // Storage API
            getItems: "",   // getItems APIの通知名
            setItems: "",   // setItems APIの通知名
            removeItem: "", // removeItem APIの通知名

            // Volume API
            volumeChanged: "", // changed APIの通知名

            // OpenLink API
            openLink: "", // openLink APIの通知名

            // displayCreatorInformationModal API
            creatorInfoShown: "", // displayCreatorInformationModal APIの通知名

            // Screenshot API
            screenshot: "",     // screenshot.displayModal APIの通知名
            takeScreenShot: "", // playerFeatures.takeScreenShot 関数の通知名

            // Scoreboard API
            scoreDisplay: "", // scoreboards.display APIの通知名
            setScore: "",     // scoreboards.setRecord APIの通知名
            getScore: "",     // scoreboards.getRecords APIの通知名
        },
    },


    // プラグインが初期化済みかどうか
    // 戻り値 : 初期化済みなら true を、未初期化なら false を返します
    IsInitialized: function()
    {
        // 初期化されているかどうかの情報をそのまま返す
        return Context.initialized;
    },


    // プラグインの初期化を行います
    // initializeParameterJson : 初期化するためのパラメータをJSON化した値
    Initialize: function(initializeParameterJson)
    {
        // 既に初期化済みなら
        if (Context.initialized)
        {
            // 直ちに終了
            return;
        }


        // 受け取ったjsonデータをパースする
        var jsonData = Pointer_stringify(initializeParameterJson);
        var initParam = JSON.parse(jsonData);


        // コンテキストを初期化していく
        Context.unityObjectName = initParam.UnityObjectName;
        Context.unityMethodNames.getItems = initParam.GetItemsCallback;
        Context.unityMethodNames.setItems = initParam.SetItemsCallback;
        Context.unityMethodNames.removeItem = initParam.RemoveItemCallback;
        Context.unityMethodNames.volumeChanged = initParam.VolumeChangedCallback;
        Context.unityMethodNames.openLink = initParam.OpenLinkCallback;
        Context.unityMethodNames.creatorInfoShown = initParam.CreatorInfoShownCallback;
        Context.unityMethodNames.screenshot = initParam.ScreenshotCallback;
        Context.unityMethodNames.takeScreenShot = initParam.TakeScreenshotCallback;
        Context.unityMethodNames.scoreDisplay = initParam.ScoreboardShownCallback;
        Context.unityMethodNames.setScore = initParam.SetScoreCallback;
        Context.unityMethodNames.getScore = initParam.GetScoreCallback;


        // ウィンドウサイズが変化した時のコールバックを登録
        window.onresize = function()
        {
            // Unityの描画用キャンバスを取得して、リサイズ後の自身がいるウィンドウのサイズを適用する
            var canvas = document.getElementById("#canvas");
            canvas.width = window.innerWidth;
            canvas.height = window.innerHeight;
        }


        // RPGアツマールがスクリーンショットを撮影するときに呼び出すハンドラを設定する
        RPGAtsumaru.experimental.screenshot.setScreenshotHandler(function()
        {
            // スクリーンショットデータを取り出す為の非同期制御オブジェクトを生成して返す
            return new Promise(function(resolve, reject)
            {
                // 成功を通知するためのリゾルバ関数をコンテキストに入れて、スクリーンショットデータの要求が来た事をUnityへ通知する
                Context.screenShotCallback = resolve;
                SendMessage(Context.unityObjectName, Context.unityMethodNames.takeScreenShot);
            })
        });


        // 初期化済みをマーク
        Context.initialized = true;
    },


    // RPGアツマールサーバーストレージからデータを取得します
    GetStorageItems: function()
    {
        // 未初期化なら
        if (!Context.initialized)
        {
            // 直ちに終了する（応答するにも応答先を知らない）
            return;
        }


        // RPGアツマールサーバーストレージからデータを拾ってくる
        window.RPGAtsumaru.storage.getItems()
            .then(function(saveData)
            {
                // 無名の配列はUnityのJsonUtilityで処理できないのでSaveDataItemsという名前を付加してJSONデータを生成
                var finalSaveData = {SaveDataItems:saveData};
                var jsonData = JSON.stringify(finalSaveData);


                // Unityに生成したJsonデータを送る
                SendMessage(Context.unityObjectName, Context.unityMethodNames.getItems, jsonData);
            });
    },


    // RPGアツマールサーバーストレージにデータを設定します。
    // 設定するデータのキーは、仕様上 "system", "data{N}" といった予約キー名を使うことを推奨します。
    // saveDataJson : {"SaveDataItems":[{"key":"key","value":"value"},,,,]}の構造を持ったJSONデータ
    SetStorageItems: function(saveDataJson)
    {
        // 未初期化なら
        if (!Context.initialized)
        {
            // 直ちに終了する（応答するにも応答先を知らない）
            return;
        }


        // C#文字列のポインタからJS文字列として取得してJSONパースをする
        var saveData = JSON.parse(Pointer_stringify(saveDataJson));


        // セーブデータをRPGアツマールサーバーへ設定する
        window.RPGAtsumaru.storage.setItems(saveData.SaveDataItems)
            .then(function()
            {
                // 完了したことを通知する
                SendMessage(Context.unityObjectName, Context.unityMethodNames.setItems);
            });
    },


    // RPGアツマールサーバーストレージから指定されたキーのデータを削除します
    // key : 削除するデータのキー（仕様上 "system", "data{N}" といった予約キー名を使うことを推奨します）
    RemoveStorageItem: function(key)
    {
        // 未初期化なら
        if (!Context.initialized)
        {
            // 直ちに終了する（応答するにも応答先を知らない）
            return;
        }


        // C#文字列のポインタからJS文字列として取得
        var jsKey = Pointer_stringify(key);


        // RPGアツマールのストレージアイテム削除APIを叩く
        window.RPGAtsumaru.storage.removeItem(jsKey)
            .then(function()
            {
                // 完了したことを通知する
                SendMessage(Context.unityObjectName, Context.unityMethodNames.removeItem);
            });
    },


    // RPGアツマール側で設定されている、現在のマスターボリュームを0.0～1.0の正規化された値を取得します
    // 戻り値 : 正規化された現在のマスターボリュームを返します
    GetCurrentVolume: function()
    {
        // この関数は素直にバイパスするだけで、現在のボリューム値をそのまま返す
        return window.RPGAtsumaru.volume.getCurrentValue();
    },


    // RPGアツマールの音量変化通知を受け取るリスンを開始します
    StartVolumeListen: function()
    {
        // 未初期化 または 既に購読している なら
        if (!Context.initialized || Context.volumeSubscription != null)
        {
            // 直ちに終了する（応答するにも応答先を知らない）
            return;
        }


        // 音量変化を購読する
        Context.volumeSubscription = window.RPGAtsumaru.volume.changed.subscribe({
            next: function(volume)
            {
                // Unityに変化した音量を通知する
                SendMessage(Context.unityObjectName, Context.unityMethodNames.volumeChanged, volume);
            },
            error: function(err) {},
            complete: function() {},
        });
    },


    // RPGアツマールの音量変化通知を受け取るリスンを停止します
    StopVolumeListen: function()
    {
        // 未初期化 または 購読していない なら
        if (!Context.initialized || Context.volumeSubscription == null)
        {
            // 直ちに終了する（応答するにも応答先を知らない）
            return;
        }


        // 購読を停止する
        Context.volumeSubscription.unsubscribe();
        Context.volumeSubscription = null;
    },


    // RPGアツマール側で外部URLを開くポップアップを表示します
    // url : 開きたい外部URL
    OpenLink: function(url)
    {
        // openLink APIを叩く
        window.RPGAtsumaru.popups.openLink(Pointer_stringify(url))
            .then(function()
            {
                // エラーは発生しなかったJSONデータを作って結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:false});
                SendMessage(Context.unityObjectName, Context.unityMethodNames.openLink, jsonData);
            })
            .catch(function(error)
            {
                // 発生したエラーを包んでJSONデータを作り結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:true,Error:error})
                SendMessage(Context.unityObjectName, Context.unityMethodNames.openLink, jsonData);
            });
    },


    // URLのクエリに設定された値を取得します（RPGアツマールの仕様上クエリの変数名は param1～param9 になります）
    // name : 取得したいクエリ名
    // 戻り値 : 取得された値を返しますが、取得できない場合は空文字列を返します
    GetQuery: function(name)
    {
        // C#文字列ポインタからJS文字列へ変換しクエリを取得するが、正しく取り出せていないなら
        var value = window.RPGAtsumaru.experimental.query[Pointer_stringify(name)];
        if (value == null)
        {
            // 空文字列を設定しておく
            value = "";
        }


        // JS文字列からC#で扱えるようにUTF8エンコードをしてポインタを返す
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    },


    // 指定されたニコニコユーザーIDの作者情報ダイアログを表示します
    // niconicoUserId : 表示するニコニコユーザーID
    ShowCreatorInformation: function(niconicoUserId)
    {
        // 作者情報ダイアログを表示する
        window.RPGAtsumaru.experimental.popups.displayCreatorInformationModal(niconicoUserId)
            .then(function()
            {
                // エラーは発生しなかったJSONデータを作って結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:false});
                SendMessage(Context.unityObjectName, Context.unityMethodNames.creatorInfoShown, jsonData);
            })
            .catch(function(error)
            {
                // 発生したエラーを包んでJSONデータを作り結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:true,Error:error})
                SendMessage(Context.unityObjectName, Context.unityMethodNames.openLink, jsonData);
            });
    },


    // スクリーンショットをとってTwitterに投稿するダイアログを表示します
    Screenshot: function()
    {
        // スクリーンショットをとってダイアログを表示する
        window.RPGAtsumaru.experimental.screenshot.displayModal()
            .then(function()
            {
                // エラーは発生しなかったJSONデータを作って結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:false});
                SendMessage(Context.unityObjectName, Context.unityMethodNames.screenshot, jsonData);
            })
            .catch(function(error)
            {
                // 発生したエラーを包んでJSONデータを作り結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:true,Error:error})
                SendMessage(Context.unityObjectName, Context.unityMethodNames.screenshot, jsonData);
            });
    },


    // Unity側で生成したスクリーンショットのデータを設定します
    // imageDataUrl : DataUrls形式のイメージデータ
    SetScreenShotData: function(imageDataUrl)
    {
        // C#文字列ポインタからJS文字列へ変換してスクリーンショットコールバックを呼ぶ
        var imageDataUrl = Pointer_stringify(imageDataUrl);
        Context.screenShotCallback(imageDataUrl);
    },


    // RPGアツマールのコントローラ入力通知のリスンを開始します
    // リスンを開始すると入力状態の制御が自動的に行われ GetInputState() 関数から得られる押し込み状態が更新されます
    StartControllerListen: function()
    {
        // 未初期化 または 既に購読している なら
        if (!Context.initialized || Context.controllerSubscription != null)
        {
            // 直ちに終了する（応答するにも応答先を知らない）
            return;
        }


        // コントローラの入力状態を初期化する
        Context.inputPress = 0;


        // コントローラ入力を購読する
        Context.controllerSubscription = window.RPGAtsumaru.controllers.defaultController.subscribe({
            next: function(input)
            {
                // 入力状態の受付と入力操作用ビットマスク変数の宣言（0xEnter,Esc,上,下,左,右 :LSB:を想定）
                var inputKey = input["key"];
                var inputStatus = input["type"];
                var inputBitMask = 0;


                // 上方向入力なら
                if (inputKey == "up")
                {
                    // 上方向入力ビットマスクを設定
                    inputBitMask = 0x08;
                }
                else if (inputKey == "down")
                {
                    // 下方向入力ビットマスクを設定
                    inputBitMask = 0x04;
                }
                else if (inputKey == "left")
                {
                    // 左方向入力ビットマスクを設定
                    inputBitMask = 0x02;
                }
                else if (inputKey == "right")
                {
                    // 右方向入力ビットマスクを設定
                    inputBitMask = 0x01;
                }
                else if (inputKey == "ok")
                {
                    // 決定入力ビットマスクを設定
                    inputBitMask = 0x20;
                }
                else if (inputKey == "cancel")
                {
                    // キャンセル入力ビットマスクを設定
                    inputBitMask = 0x10;
                }


                // もしキーの押し込みなら
                if (inputStatus == "keydown")
                {
                    // OR演算でお仕込み状態にする
                    Context.inputPress |= inputBitMask;
                }
                else if (inputStatus == "keyup")
                {
                    // 離したのなら入力反転してANDでマスクする
                    Context.inputPress &= ~inputBitMask;
                }
            },
            error: function(err) {},
            complete: function() {},
        });
    },


    // RPGアツマールのコントローラ入力通知のリスンを停止します
    // リスンを停止すると入力状態の制御が停止され GetInputState() 関数から得られる押し込み状態が更新されなくなります
    StopControllerListen: function()
    {
        // 未初期化 または 購読していない なら
        if (!Context.initialized || Context.controllerSubscription == null)
        {
            // 直ちに終了する（応答するにも応答先を知らない）
            return;
        }


        // コントローラの入力状態を初期化する
        Context.inputPress = 0;


        // 購読を停止する
        Context.controllerSubscription.unsubscribe();
        Context.controllerSubscription = null;
    },


    // RPGアツマールのコントローラ入力状態を取得します
    // 入力状態を取得する前に必ず StartControllerListen() 関数で入力状態をリスンしてください
    GetInputState: function()
    {
        // 入力状態をそのまま返す
        return Context.inputPress;
    },


    // RPGアツマール上に指定されたスコアボードを表示します
    // boardId : 表示したいボードID（RPGアツマールの仕様上 1 ～ 10 までです（10個以上の場合は管理ページから上限を指定できます））
    ShowScoreBoard: function(boardId)
    {
        // スコアボードを表示する
        window.RPGAtsumaru.experimental.scoreboards.display(boardId)
            .then(function()
            {
                // エラーは発生しなかったJSONデータを作って結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:false});
                SendMessage(Context.unityObjectName, Context.unityMethodNames.scoreDisplay, jsonData);
            })
            .catch(function(error)
            {
                // 発生したエラーを包んでJSONデータを作り結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:true,Error:error})
                SendMessage(Context.unityObjectName, Context.unityMethodNames.scoreDisplay, jsonData);
            });
    },


    // 指定されたスコアボードにスコアを送信します
    // boardId : 送信する先のスコアボードID
    // score : 送信するスコア
    SendScoreRecord: function(boardId, score)
    {
        // スコアボードにスコアを送る
        window.RPGAtsumaru.experimental.scoreboards.setRecord(boardId, score)
            .then(function()
            {
                // エラーは発生しなかったJSONデータを作って結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:false});
                SendMessage(Context.unityObjectName, Context.unityMethodNames.setScore, jsonData);
            })
            .catch(function(error)
            {
                // 発生したエラーを包んでJSONデータを作り結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:true,Error:error})
                SendMessage(Context.unityObjectName, Context.unityMethodNames.setScore, jsonData);
            });
    },


    // 指定されたスコアボードからスコア情報を取得します
    // boardId : スコア情報を取得したいスコアボードID
    GetScoreRecord: function(boardId)
    {
        // スコアボードからスコア情報を取得する
        window.RPGAtsumaru.experimental.scoreboards.getRecords(boardId)
            .then(function(scoreboardData)
            {
                // スコアボードのデータをC#で扱えるように調整した変数を宣言
                var csScoreboardData = {};
                csScoreboardData.boardId = scoreboardData.boardId;
                csScoreboardData.boardName = scoreboardData.boardName;


                // マイレコードが存在するなら
                csScoreboardData.myRecord = {};
                if (scoreboardData.myRecord != null)
                {
                    // 有効なデータが存在するとマークをしてデータを詰める
                    csScoreboardData.myRecord.Available = true;
                    csScoreboardData.myRecord.rank = scoreboardData.myRecord.rank;
                    csScoreboardData.myRecord.score = scoreboardData.myRecord.score;
                    csScoreboardData.myRecord.isNewRecord = scoreboardData.myRecord.isNewRecord;
                }
                else
                {
                    // 有効なデータが存在しないことをマークする
                    csScoreboardData.myRecord.Available = false;
                }


                // 自己ベストレコードが存在するなら
                csScoreboardData.myBestRecord = {};
                if (scoreboardData.myBestRecord != null)
                {
                    // 有効なデータが存在するマークをしてデータを詰める
                    csScoreboardData.myBestRecord.Available = true;
                    csScoreboardData.myBestRecord.userName = scoreboardData.myBestRecord.userName;
                    csScoreboardData.myBestRecord.rank = scoreboardData.myBestRecord.rank;
                    csScoreboardData.myBestRecord.score = scoreboardData.myBestRecord.score;
                }
                else
                {
                    // 有効なデータが存在しないことをマークする
                    csScoreboardData.myBestRecord.Available = false;
                }


                // ランキングデータがあろうとなかろうと空配列を定義して長さ分ループ
                csScoreboardData.ranking = new Array(scoreboardData.ranking.length);
                for (var i = 0; i < scoreboardData.ranking.length; ++i)
                {
                    // ランキング配列に詰めていく
                    csScoreboardData.ranking[i] = {};
                    csScoreboardData.ranking[i].rank = scoreboardData.ranking[i].rank;
                    csScoreboardData.ranking[i].userName = scoreboardData.ranking[i].userName;
                    csScoreboardData.ranking[i].score = scoreboardData.ranking[i].score;
                }


                // エラーは発生しなかったJSONデータを作って結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:false,ScoreboardData:csScoreboardData});
                SendMessage(Context.unityObjectName, Context.unityMethodNames.getScore, jsonData);
            })
            .catch(function(error)
            {
                // 発生したエラーを包んでJSONデータを作り結果を通知する
                var jsonData = JSON.stringify({ErrorOccured:true,Error:error})
                SendMessage(Context.unityObjectName, Context.unityMethodNames.setScore, jsonData);
            });
    },


    // RPGアツマールコメント機能の内部状態をリセットしてからシーンを切り替えます（タイトルシーンに戻る時やゲームの状態がリセットされる時など）
    // sceneName : 最大64文字のASCII文字列で表されるシーン名。また、文字列の先頭にアンダースコアが2つをつけることは予約語と衝突するため使用できません。
    ResetAndChangeScene: function(sceneName)
    {
        // C#の文字列ポインタからJS用文字列を取り出してアツマールAPIを叩く
        var jsSceneName = Pointer_stringify(sceneName);
        window.RPGAtsumaru.comment.resetAndChangeScene(jsSceneName);
    },


    // RPGアツマールコメントのシーンを切り替えます
    // sceneName : 最大64文字のASCII文字列で表されるシーン名。また、文字列の先頭にアンダースコアが2つをつけることは予約語と衝突するため使用できません。
    ChangeScene: function(sceneName)
    {
        // C#の文字列ポインタからJS用文字列を取り出してアツマールAPIを叩く
        var jsSceneName = Pointer_stringify(sceneName);
        window.RPGAtsumaru.comment.changeScene(jsSceneName);
    },


    // RPGアツマールコメントのシーン内で特定のコンテキストを設定します
    // context : 最大64文字のASCII文字列で表される、設定するコンテキストの文字列。
    SetContext: function(context)
    {
        // C#の文字列ポインタからJS用文字列を取り出してアツマールAPIを叩く
        var jsContext = Pointer_stringify(context);
        window.RPGAtsumaru.comment.setContext(jsContext);
    },


    // RPGアツマールコメントの現在のコンテキストに対して状態を進めます
    // factor : 現在のコンテキストに対して状態の内容を示す文字列
    PushContextFactor: function(factor)
    {
        // C#の文字列ポインタからJS用文字列を取り出してアツマールAPIを叩く
        var jsFactor = Pointer_stringify(factor);
        window.RPGAtsumaru.comment.pushContextFactor(jsFactor);
    },


    // RPGアツマールコメントの現在のコンテキストが特定コンテキストファクタの状態におけるマイナーコンテキストを進めます
    PushMinorContext: function()
    {
        // そのままアツマールAPIを叩く
        window.RPGAtsumaru.comment.pushMinorContext();
    },
};


// 内部依存定義の追加
autoAddDeps(RpgAtsumaruApiForUnity, "$Context");


// Unityライブラリにライブラリオブジェクトを登録
mergeInto(LibraryManager.library, RpgAtsumaruApiForUnity);