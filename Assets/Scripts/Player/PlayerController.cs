using Network;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Button _hostButton = default;
    [SerializeField]
    private Button _joinButton = default;

    private NetworkController _networkController = default;

    private void Start()
    {
        Register();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { StartCoroutine(DropOut()); }
    }

    /// <summary> サーバーの立ち上げ処理 </summary>
    private void Register()
    {
        if (_hostButton == null) { return; }

        _hostButton.onClick.AddListener(() =>
        {
            _networkController ??= new NetworkController();
            _networkController.Build();
        });
    }

    /// <summary> サーバーシーンのアンロード </summary>
    private IEnumerator DropOut()
    {
        if (_networkController == null) { yield break; }

        yield return _networkController.UnLoad();

        _networkController = null;
        yield return null;
    }

    private void OnDestroy()
    {
        if (!gameObject.activeSelf) { return; }

        StartCoroutine(DropOut());
    }
}
