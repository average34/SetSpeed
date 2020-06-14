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

namespace RpgAtsumaruApiForUnity.Editor
{
    /// <summary>
    /// RPGアツマールAPI for Unityのエディタユーティリティクラスです
    /// </summary>
    public static class RpgAtsumaruEditorUtility
    {
        // クラス変数定義
        private static Dictionary<string, string> mimeTypeTable;



        /// <summary>
        /// RpgAtsumaruEditorUtility クラスの初期化をします
        /// </summary>
        static RpgAtsumaruEditorUtility()
        {
            // MIMEタイプテーブルの初期化
            mimeTypeTable = new Dictionary<string, string>()
            {
                { ".bmp", "image/bmp" },
                { ".gif", "image/gif" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
            };
        }


        /// <summary>
        /// 指定された画像ファイルをDataUrls形式の文字列へ変換します
        /// </summary>
        /// <param name="filePath">DataUrls形式に変換する画像ファイルのパス</param>
        /// <returns>変換されたDataUrls文字列を返します</returns>
        /// <exception cref="ArgumentException">filePath が null または 空白 です</exception>
        /// <exception cref="FileNotFoundException">変換する画像のファイルが見つかりませんでした</exception>
        /// <exception cref="NotSupportedException">不明な画像ファイルの拡張子です Extension={fileInfo.Extension}</exception>
        public static string ConvertImageToDataUrls(string filePath)
        {
            // ファイル情報を生成してファイルが見つからないなら
            var fileInfo = new FileInfo(string.IsNullOrWhiteSpace(filePath) ? throw new ArgumentException($"{nameof(filePath)} が null または 空白 です", nameof(filePath)) : filePath);
            if (!fileInfo.Exists)
            {
                // ファイルが見つからない例外を吐く
                throw new FileNotFoundException("変換する画像のファイルが見つかりませんでした", filePath);
            }


            // 拡張子に応じたMIMEタイプを取得するが、取得できなかったら
            if (!mimeTypeTable.TryGetValue(fileInfo.Extension, out var mimeType))
            {
                // 指定されたファイルの拡張子で知るMIMEタイプを知らない
                throw new NotSupportedException($"不明な画像ファイルの拡張子です Extension={fileInfo.Extension}");
            }


            // ファイル全体のBase64を計算してDataUrlsとして返す
            var base64 = Convert.ToBase64String(File.ReadAllBytes(fileInfo.FullName));
            return $"data:{mimeType};base64,{base64}";
        }
    }
}