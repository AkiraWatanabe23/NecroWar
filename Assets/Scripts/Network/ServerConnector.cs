﻿using Data;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Network
{
    /// <summary> サーバー接続を行うサンプルクラス </summary>
    public class ServerConnector : SingletonMonoBehaviour<ServerConnector>
    {
        [Tooltip("保持しているデータ一覧")]
        [SerializeField]
        private UserDataHolder _userData = default;
        [Tooltip("ポート番号")]
        [SerializeField]
        private int _port = 7000;
        [SerializeField]
        private ConnectorView _connectorView = default;
        [SerializeField]
        private ConnectorModel _connectorModel = new();

        [Header("For Debug")]
        [Tooltip("取得したいデータのクラス名")]
        [AbstractClass(typeof(AbstractData))]
        [SerializeField]
        private string _targetClassName = "";

        /// <summary> リクエストを送信して閉じたか </summary>
        private bool _isRequestClosed = false;
        /// <summary> サーバーに接続しているか </summary>
        private bool _isConnected = false;
        /// <summary> 対象サーバーのIPAddress </summary>
        private string _serverIPAddress = default;
        /// <summary> アクセスサーバーのリンク </summary>
        private string _serverURL = "";

        /// <summary> 正常な処理が行われた場合にサーバーから返ってくる文字列 </summary>
        private const string Success = "Request Success";
        /// <summary> 何かしらリクエストに対する処理が失敗した時にサーバーから返ってくる文字列 </summary>
        private const string Failed = "Request Failed";

        protected override bool DontDestroyOnLoad => true;

        private async void Start()
        {
            BootstrapServer bootstrap = new(_port);
            _serverIPAddress = await bootstrap.SendServerAddressRequest();

            _serverURL = $"http://{_serverIPAddress}:{_port}/";
            if (!IsValidURL(_serverURL)) { Debug.LogError("適切なURLが取得されませんでした"); ApplicationClose(false); }

            _connectorModel.Initialize(_serverURL);

            RegisterViewActions();
        }

        /// <summary> 文字列がURLとして成立しているか </summary>
        private bool IsValidURL(string candidateURL)
        {
            if (Uri.TryCreate(candidateURL, UriKind.Absolute, out Uri result))
            {
                return (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
            }
            return false;
        }

        private void RegisterViewActions()
        {
            _connectorView.OnUpdatePortText(_port.ToString());
            _connectorView.OnUpdateServerAddressText(_serverIPAddress);

            _connectorView.ConnectionStartButton.onClick.AddListener(async () =>
            {
                if (await AccessServer())
                {
                    if (_userData == null) { _userData = FindObjectOfType<UserDataHolder>(); }
                    _userData.Initalize();

                    if (!_userData.IsHoldID())
                    {
                        Debug.Log("Create User");
                        _userData.OnUpdateID(await PostRequest("GenerateID", _userData.ID));

                        _connectorView.OnUpdateIDText(_userData.ID);
                    }
                    else
                    {
                        Debug.Log("Get Data");
                        _userData.OnUpdateDataInfo(_targetClassName, await PutRequest("GetUserData", _userData.ID, _targetClassName));
                        _connectorView.OnUpdateIDText(_userData.ID);
                    }
                }
                else { Debug.LogError("接続に失敗しました"); }
            });
            _connectorView.NameApply.onClick.AddListener(async () =>
            {
                _userData.OnUpdateName(_connectorView.Name);
                _ = await PutRequest("SetName", _userData.ID, _connectorView.Name);
            });
            _connectorView.ScoreApply.onClick.AddListener(async () =>
            {
                if (int.TryParse(_connectorView.Score, out int score))
                {
                    _userData.OnUpdateScore(score);
                    _ = await PutRequest("SetScore", _userData.ID, score.ToString());
                }
                else { Debug.LogError("不正な値が割り当てられています"); }
            });

            _connectorView.GetNameButton.onClick.AddListener(async () => await PostRequest("GetName", _userData.ID));
            _connectorView.GetScoreButton.onClick.AddListener(async () => await PostRequest("GetScore", _userData.ID));
            //サンプルとして1～5位のランキングを取得
            _connectorView.GetRankingButton.onClick.AddListener(async () => await PutRequest("GetRanking", _userData.ID, "1", "5"));
            _connectorView.CloseButton.onClick.AddListener(async () =>
            {
                bool isDeleteID = !_userData.IsDataSaveOnClosed();
                if (isDeleteID && await PostRequest("DeleteUserData", _userData.ID) == Success ||
                    !isDeleteID && await PostRequest("CloseClient", _userData.ID) == Success)
                {
                    ApplicationClose(isDeleteID);
                }
            });
        }

        private async Task<bool> AccessServer()
        {
            if (_isConnected) { return true; }

            return _isConnected = await _connectorModel.SendGetRequest();
        }

        /// <summary> サーバーに対して実行したい処理を送信する </summary>
        /// <param name="id"> ユーザーの固有ID </param>
        /// <param name="requestMessage"> 実行したい処理の形式（得点を取得、など） </param>
        private async Task<string> PostRequest(string requestMessage, string id)
        {
            //「誰が」「何をしたいか」を送信する
            var form = new WWWForm();
            form.AddField("UserID", id);
            form.AddField("RequestMessage", requestMessage);

            var requestData = await _connectorModel.SendPostRequest(form);

            _connectorView.OnUpdateAccessResultText(requestData);
            return requestData;
        }

        private async Task<string> PutRequest(string requestMessage, string id, params string[] parameters)
        {
            var requestData = await _connectorModel.SendPutRequest($"{id},{string.Join(",", parameters)}", requestMessage);

            _connectorView.OnUpdateAccessResultText(requestData);
            return requestData;
        }

        private void ApplicationClose(bool deleteKey)
        {
            _isRequestClosed = true;
            if (deleteKey) { _userData.DeleteID(); }
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private async void OnDisable()
        {
            if (_isRequestClosed || !_isConnected) { return; }

            var closeRequest = await PostRequest("CloseClient", _userData.ID);
            if (closeRequest == Success) { ApplicationClose(false); }
        }

        private void OnDestroy() => _connectorModel.OnDestroy();
    }
}
