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
using System.Runtime.ExceptionServices;
using UnityEditor;
using UnityEngine;

namespace RpgAtsumaruApiForUnity.Editor
{
    /// <summary>
    /// 画像ファイルをDataUrls形式に変換するツールウィンドウクラスです
    /// </summary>
    public class ImageToDataUrlsConvertWindow : EditorWindow
    {
        // 定数定義
        private int RecommendMaxDataSize = (63 << 10);

        // メンバ変数定義
        [NonSerialized]
        private bool initialized;
        private Queue<Action> messageQueue;
        private string selectedFilePath;
        private string convertedDataUrls;
        private string displayText;
        private Vector2 scrollPosition;



        /// <summary>
        /// エディタウィンドウの起動をします
        /// </summary>
        private void Awake()
        {
            // タイトルを変えて初期化をする
            titleContent = new GUIContent("画像をDataUrlsに変換する");
            Initialize();
        }


        /// <summary>
        /// エディタウィンドウの初期化をします
        /// </summary>
        private void Initialize()
        {
            // 初期化済みなら
            if (initialized)
            {
                // 何もしない
                return;
            }


            // 初期化をする
            messageQueue = new Queue<Action>();
            selectedFilePath = string.Empty;
            convertedDataUrls = string.Empty;
            displayText = string.Empty;
            scrollPosition = Vector2.zero;


            // 初期化済みマーク
            initialized = true;
        }


        /// <summary>
        /// エディタUIの描画ループを処理します
        /// </summary>
        private void OnGUI()
        {
            try
            {
                // 初期化を行い続ける
                Initialize();


                // メッセージを処理して描画をする
                DoProcessMessage();
                DoRender();


                // もし処理するべきメッセージが存在するなら描画ループを継続する
                ContinueRenderLoopIfMessageAvailable();
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                // ファイルが見つからなかった通知をする
                EditorUtility.DisplayDialog("エラー", $"ファイル '{fileNotFoundException.FileName}' が見つかりませんでした", "OK");
            }
            catch (Exception exception)
            {
                // 未処理の例外が発生したら、自身閉じるリクエストをして例外を再スローする
                Close();
                ExceptionDispatchInfo.Capture(exception).Throw();
            }
        }


        /// <summary>
        /// 送られてきたメッセージを処理します
        /// </summary>
        private void DoProcessMessage()
        {
            // 現在のメッセージキューの数分だけループする
            var processMessageQueueCount = messageQueue.Count;
            for (int i = 0; i < processMessageQueueCount; ++i)
            {
                // メッセージを取り出して処理をする
                messageQueue.Dequeue()();
            }
        }


        /// <summary>
        /// エディタウィンドウのUIをレンダリングします
        /// </summary>
        private void DoRender()
        {
            // ボタンを描画するローカル関数を定義
            void RenderButton(string name, Action message, bool enable)
            {
                // 現在のGUIカラーを覚えて有効状態に応じて色を設定する
                var currentColo = GUI.color;
                GUI.color = enable ? GUI.color : Color.gray;


                // ボタンが押されてかつ有効なら
                if (GUILayout.Button(name) && enable)
                {
                    // 該当のメッセージ関数を登録する
                    messageQueue.Enqueue(message);
                }


                // 元の色に戻す
                GUI.color = currentColo;
            }


            // renderの内容を横並びにレイアウトするローカル関数を定義
            void LayoutHorizontal(Action render)
            {
                // 横並びレイアウト制御関数で描画関数を挟む
                GUILayout.BeginHorizontal();
                render();
                GUILayout.EndHorizontal();
            }


            // renderの内容をスクロールレイアウトするロー感関数を定義
            void LayoutScroll(Action render)
            {
                // スクロールレイアウト制御関数で描画関数を挟む
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                render();
                GUILayout.EndScrollView();
            }


            // 横並びレイアウトをする
            LayoutHorizontal(() =>
            {
                // コマンドボタンを描画する
                RenderButton("変換する画像ファイルを選択する", OnFileSelectButtonClick, true);
                RenderButton("選択した画像を変換する", OnConvertButtonClick, !string.IsNullOrWhiteSpace(selectedFilePath));
                RenderButton("変換した画像をブラウザで開く", OnOpenWebBrowserButtonClick, !string.IsNullOrWhiteSpace(convertedDataUrls));
                RenderButton("クリップボードにコピーする", OnCopyClipboardButtonClick, !string.IsNullOrWhiteSpace(convertedDataUrls));
            });


            GUILayout.TextField(selectedFilePath);
            LayoutScroll(() => GUILayout.TextArea(displayText, GUILayout.ExpandWidth(true)));
        }


        /// <summary>
        /// 処理するべきメッセージが存在する場合に再描画を要求してレンダーループを継続します
        /// </summary>
        private void ContinueRenderLoopIfMessageAvailable()
        {
            // 処理するべきメッセージが存在するなら
            if (messageQueue.Count > 0)
            {
                // 再描画
                Repaint();
            }
        }


        /// <summary>
        /// 変換する画像ファイルを選択するボタンをクリックされた時の処理をします
        /// </summary>
        private void OnFileSelectButtonClick()
        {
            // 変換するファイルを選択するダイアログを表示して結果を受け取る
            var filePath = EditorUtility.OpenFilePanelWithFilters("変換する画像ファイルを選択", string.Empty, new string[] { "画像ファイル", "bmp,gif,png,jpg,jpeg" });
            selectedFilePath = string.IsNullOrWhiteSpace(filePath) ? selectedFilePath : filePath.Replace("\\", "/");
        }


        /// <summary>
        /// 変換するボタンをクリックされた時の処理をします
        /// </summary>
        private void OnConvertButtonClick()
        {
            // 画像をDataUrls形式に変換して表示用変数にも参照を渡す
            convertedDataUrls = RpgAtsumaruEditorUtility.ConvertImageToDataUrls(selectedFilePath);
            displayText = convertedDataUrls;


            // 変換された文字列の長さが推奨の文字列の長さを超えている場合は
            if (convertedDataUrls.Length > RecommendMaxDataSize)
            {
                // 推奨範囲を超えている事を表示して表示用文字列を調整する
                EditorUtility.DisplayDialog("警告", $"変換されたデータが推奨サイズ'{RecommendMaxDataSize >> 10} KiB'を超えています。\n表示できない可能性のブラウザがあります。", "OK");
                displayText = displayText.Remove(RecommendMaxDataSize);
            }
        }


        /// <summary>
        /// ブラウザで表示するボタンをクリックされた時の処理をします
        /// </summary>
        private void OnOpenWebBrowserButtonClick()
        {
            // 一時ファイルとしてDataUrlsの表示用imgタグを書き込む
            var path = Path.Combine(Path.GetTempPath(), "preview.html");
            File.WriteAllText(path, $"<img src=\"{convertedDataUrls}\"/>");


            // 保存したページを開く
            System.Diagnostics.Process.Start(path);
        }


        /// <summary>
        /// クリップボードにコピーするボタンをクリックされた時の処理をします
        /// </summary>
        private void OnCopyClipboardButtonClick()
        {
            // クリップボードにコピーしてコピーしたことを表示する
            GUIUtility.systemCopyBuffer = convertedDataUrls;
            EditorUtility.DisplayDialog("成功", "変換済みDataUrlsをクリップボードにコピーしました", "OK");
        }
    }
}