using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace Network
{
    [Serializable]
    public class ConnectorModel
    {
        [Tooltip("リクエストに失敗した時に再接続を行う回数")]
        [Range(0, 10)]
        [SerializeField]
        private int _rerunCount = 1;
        [Tooltip("1回あたりのリクエストの実行時間")]
        [Range(1f, 10f)]
        [SerializeField]
        private float _executionTime = 1f;
        [ReadOnly]
        [Tooltip("再起処理の実行回数")]
        [SerializeField]
        private int _runCount = 0;

        private CancellationTokenSource _cancellationTokenSource = default;
        /// <summary> 処理の実行時間を調べる </summary>
        private Stopwatch _stopWatch = default;
        private string _serverURL = "";

        /// <summary> 正常な処理が行われた場合にサーバーから返ってくる文字列 </summary>
        private const string Success = "Request Success";
        /// <summary> 何かしらリクエストに対する処理が失敗した時にサーバーから返ってくる文字列 </summary>
        private const string Failed = "Request Failed";

        public void Initialize(string url)
        {
            _cancellationTokenSource = new();
            _stopWatch = new();
            _serverURL = url;
        }

        /// <summary> Getリクエストを送信する </summary>
        /// <returns> アクセスに成功したか </returns>
        public async Task<bool> SendGetRequest(CancellationToken token = default)
        {
            try
            {
                if (token == default) { token = _cancellationTokenSource.Token; }
                _stopWatch.Start();

                //GetRequest ... 接続先のURLのみを指定してリクエストを送信する（固有のパラメータ等は渡せない）
                using UnityWebRequest request = UnityWebRequest.Get(_serverURL);
                var send = request.SendWebRequest();

                //接続結果が返ってくるまで待機
                while (!send.isDone)
                {
                    if (token.IsCancellationRequested) { break; }
                    //リクエストの待機時間が一定時間を超えた場合
                    if (_stopWatch.ElapsedMilliseconds >= _executionTime * 1000f)
                    {
                        //指定回数分だけ再実行する
                        if (_runCount < _rerunCount)
                        {
                            _runCount++;
                            _stopWatch.Reset();
                            return await SendGetRequest(_cancellationTokenSource.Token);
                        }
                        else { break; }
                    }
                    await Task.Delay(1, token);
                }

                //返ってきた結果が Success ではない → 何か問題があった
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"{request.result} : {request.error}");
                    return false;
                }
                else
                {
                    var result = request.downloadHandler.text;
                    Debug.Log($"Request Result : {result}");
                    if (result == Success) { return true; }
                    else { return false; }
                }
            }
            catch (Exception exception)
            {
                //通信に不備があった場合等にここを通る
                Debug.LogError($"Failed : {exception.Message}");
                return false;
            }
            finally
            {
                //再帰実行に用いたデータのリセット
                _runCount = 0;
                _stopWatch.Reset();
            }
        }

        /// <summary> Postリクエストを送信する </summary>
        /// <returns> 実行結果の文字列 </returns>
        public async Task<string> SendPostRequest(WWWForm form, CancellationToken token = default)
        {
            try
            {
                if (token == default) { token = _cancellationTokenSource.Token; }
                _stopWatch.Start();

                using UnityWebRequest request = UnityWebRequest.Post(_serverURL, form);
                var send = request.SendWebRequest();
                while (!send.isDone)
                {
                    if (token.IsCancellationRequested) { break; }
                    if (_stopWatch.ElapsedMilliseconds >= _executionTime * 1000f)
                    {
                        if (_runCount < _rerunCount)
                        {
                            _runCount++;
                            _stopWatch.Reset();

                            return await SendPostRequest(form, _cancellationTokenSource.Token);
                        }
                        else { break; }
                    }
                    await Task.Delay(1, token);
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.error);
                    return "None";
                }
                else
                {
                    var result = request.downloadHandler.text;
                    Debug.Log($"Request Result : {result}");
                    return result;
                }
            }
            catch (Exception exception)
            {
                var message = exception.Message;

                Debug.LogError($"Failed : {message}");
                return message;
            }
            finally
            {
                _runCount = 0;
                _stopWatch.Reset();
            }
        }

        /// <summary> Putリクエストを送信する </summary>
        /// <returns> 実行結果の文字列 </returns>
        public async Task<string> SendPutRequest(string json, string requestMessage, CancellationToken token = default)
        {
            try
            {
                if (token == default) { token = _cancellationTokenSource.Token; }
                _stopWatch.Start();

                using UnityWebRequest request = UnityWebRequest.Put(_serverURL, Encoding.UTF8.GetBytes(json + $"^{requestMessage}"));
                var send = request.SendWebRequest();

                while (!send.isDone)
                {
                    if (token.IsCancellationRequested) { break; }
                    if (_stopWatch.ElapsedMilliseconds >= _executionTime * 1000f)
                    {
                        if (_runCount < _rerunCount)
                        {
                            _runCount++;
                            _stopWatch.Reset();

                            return await SendPutRequest(json, requestMessage, _cancellationTokenSource.Token);
                        }
                        else { break; }
                    }
                    await Task.Delay(1, token);
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.error);
                    return "None";
                }
                else
                {
                    var result = request.downloadHandler.text;
                    Debug.Log($"Request Result : {result}");
                    return result;
                }
            }
            catch (Exception exception)
            {
                var message = exception.Message;

                Debug.LogError($"Failed : {message}");
                return message;
            }
            finally
            {
                _runCount = 0;
                _stopWatch.Reset();
            }
        }

        public void OnDestroy()
        {
            _stopWatch.Stop();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
