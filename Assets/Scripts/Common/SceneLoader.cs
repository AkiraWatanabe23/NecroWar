using Constants;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    /// <summary> フェード -> シーン遷移 </summary>
    public static void FadeLoad(SceneName scene) => Fade.Instance.StartFadeOut(() => LoadToScene(scene));

    /// <summary> シーン遷移 </summary>
    public static void LoadToScene(SceneName scene) => SceneManager.LoadScene(Consts.Scenes[scene]);

    public static IEnumerator LoadAdditiveScene(SceneName scene)
    {
        SceneManager.LoadScene(Consts.Scenes[scene], LoadSceneMode.Additive);

        yield return SceneManager.UnloadSceneAsync(Consts.Scenes[scene]);
        yield return Resources.UnloadUnusedAssets();
    }
}