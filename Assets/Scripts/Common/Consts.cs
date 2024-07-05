using System.Collections.Generic;
using UnityEngine;

public enum SceneName
{
    Title,
    InGame,
    Result
}

namespace Constants
{
    /// <summary> 定数管理ファイル </summary>
    public static class Consts
    {
        /// <summary> SEの同時再生上限 </summary>
        public const int SEPlayableLimit = 5;

        /// <summary> enumとシーン名のDictionary </summary>
        public static readonly Dictionary<SceneName, string> Scenes = new()
        {
            { SceneName.Title, "TitleScene" },
            { SceneName.InGame, "GameScene" },
            { SceneName.Result, "ResultScene" }
        };
    }

    public static class ConsoleLogs
    {
        public static void Log(object message)
        {
#if UNITY_EDITOR
            Debug.Log(message);
#endif
        }

        public static void LogWarning(object message)
        {
#if UNITY_EDITOR
            Debug.LogWarning(message);
#endif
        }

        public static void LogError(object message)
        {
#if UNITY_EDITOR
            Debug.LogError(message);
#endif
        }
    }
}