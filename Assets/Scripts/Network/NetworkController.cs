using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    /// <summary> ホストのみが保持する、サーバー管理を行うクラス </summary>
    public class NetworkController
    {
        private const string ServerSceneName = "ServerScene";

        /// <summary> サーバーシーンの立ち上げ </summary>
        public void Build()
        {
            SceneManager.LoadScene(ServerSceneName, LoadSceneMode.Additive);
        }

        public IEnumerator UnLoad()
        {
            yield return SceneManager.UnloadSceneAsync(ServerSceneName);
            yield return Resources.UnloadUnusedAssets();
        }
    }
}
