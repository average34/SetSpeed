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
    /// このライブラリでのみ動作する内部ロガークラスです
    /// </summary>
    internal static class InternalLogger
    {
        // クラス変数宣言
        private static ILogger logger;



        /// <summary>
        /// InternalLogger クラスの初期化を行います
        /// </summary>
        static InternalLogger()
        {
#if UNITY_EDITOR
            // 通常のUnityログを使う
            logger = new UnityLogLogger();
#else
            // エディタ以外はNULLログを使う
            logger = new NullLogger();
#endif
        }


        /// <summary>
        /// 現在のロガーを使ってログを出力します
        /// </summary>
        /// <param name="level">出力するログレベル</param>
        /// <param name="message">出力するログメッセージ</param>
        public static void Log(LogLevel level, string message)
        {
            // ロガーに横流し
            logger.Log(level, message);
        }
    }



    /// <summary>
    /// ログレベルを表します
    /// </summary>
    internal enum LogLevel
    {
        /// <summary>
        /// 動作の結果を残すログレベルです
        /// </summary>
        Log = UnityEngine.LogType.Log,

        /// <summary>
        /// 動作を復帰することが出来ますが想定した挙動にはならないログレベルです
        /// </summary>
        Warning = UnityEngine.LogType.Warning,

        /// <summary>
        /// 動作を復帰することの出来ない問題が発生したログレベルです
        /// </summary>
        Error = UnityEngine.LogType.Error,
    }



    /// <summary>
    /// ロガーが実装するべきインターフェイスです
    /// </summary>
    internal interface ILogger
    {
        /// <summary>
        /// 指定されたログレベルでログメッセージを出力します
        /// </summary>
        /// <param name="level">出力するログレベル</param>
        /// <param name="message">出力するログメッセージ</param>
        void Log(LogLevel level, string message);
    }



    /// <summary>
    /// 何もしないロガークラスです
    /// </summary>
    internal class NullLogger : ILogger
    {
        /// <summary>
        /// このクラスでは何もしません
        /// </summary>
        /// <param name="level">入力は処理されません</param>
        /// <param name="message">入力は処理されません</param>
        public void Log(LogLevel level, string message)
        {
        }
    }



    /// <summary>
    /// Unityのロガーを使ったログ出力をするロガークラスです
    /// </summary>
    internal class UnityLogLogger : ILogger
    {
        /// <summary>
        /// 指定されたログレベルでログメッセージを出力します
        /// </summary>
        /// <param name="level">出力するログレベル</param>
        /// <param name="message">出力するログメッセージ</param>
        public void Log(LogLevel level, string message)
        {
            // Unityのロガーにそのまま流し込む
            UnityEngine.Debug.unityLogger.Log((UnityEngine.LogType)level, message);
        }
    }
}