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

using UnityEditor;

namespace RpgAtsumaruApiForUnity.Editor
{
    /// <summary>
    /// RPGアツマールAPI for Unityにおけるエディタのメニューアイテムの定義を提供するクラスです
    /// </summary>
    public static class EditorMenuItems
    {
        // 定数定義
        private const string RootMenuItemName = "RPGアツマール";
        private const string WindowSubMenuItemName = "ウィンドウ";
        private const string WindowGroupMenuItemName = RootMenuItemName + "/" + WindowSubMenuItemName;



        /// <summary>
        /// ImageToDataUrlsConvertWindowを表示します
        /// </summary>
        [MenuItem(WindowGroupMenuItemName + "/画像をDataUrls形式に変換")]
        public static void ShowImageToDataUrlsConvertWindow()
        {
            // ウィンドウを開く
            EditorWindow.GetWindow<ImageToDataUrlsConvertWindow>();
        }


        /// <summary>
        /// WebGlTemplateGenerateWindowを表示します
        /// </summary>
        [MenuItem(WindowGroupMenuItemName + "/WebGLテンプレート作成")]
        public static void ShowWebGlTemplateGenerateWindow()
        {
            // ウィンドウを開く
            EditorWindow.GetWindow<WebGlTemplateGenerateWindow>();
        }
    }
}