using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using Debug = UnityEngine.Debug;
#else
using Debug = Constants.ConsoleLogs;
#endif

namespace Network
{
    [Serializable]
    public class ConnectorModel
    {
        [Tooltip("再接続を行う回数")]
        [Range(0, 10)]
        [SerializeField]
        private int _rerunCount = 1;
        [Tooltip("1回あたりのリクエストの実行時間")]
        [Range(1f, 10f)]
        [SerializeField]
        private float _executionTime = 1f;
        [Tooltip("再起処理の実行回数")]
        [SerializeField]
        private int _runCount = 0;

        private CancellationTokenSource _cancellationTokenSource = default;
        /// <summary> 処理の実行時間を調べる </summary>
        private Stopwatch _stopWatch = default;
        private string _serverURL = "";

        public void Initialize(string url)
        {
            _cancellationTokenSource = new();
            _stopWatch = new();
            _serverURL = url;
        }

        public async Task<bool> SendGetRequest(CancellationToken token = default)
        {
            try
            {
                if (token == default) { token = _cancellationTokenSource.Token; }
                _stopWatch.Start();

                Debug.Log(_serverURL);
                using UnityWebRequest request = UnityWebRequest.Get(_serverURL);
                var send = request.SendWebRequest();
                while (!send.isDone)
                {
                    //一回あたりの実行時間が一定時間超える
                    if (_stopWatch.ElapsedMilliseconds >= _executionTime * 1000f)
                    {
                        //指定回数分だけ再実行
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

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"{request.result} : {request.error}");
                    return false;
                }
                else
                {
                    var result = request.downloadHandler.text;
                    Debug.Log($"Request Result : {result}");
                    if (result == "Success") { return true; }
                    else { return false; }
                }
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed : {exception.Message}");
                return false;
            }
            finally
            {
                _runCount = 0;
                _stopWatch.Reset();
            }
        }

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
                    //一回あたりの実行時間が一定時間超える
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
                    //一回あたりの実行時間が一定時間超える
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
