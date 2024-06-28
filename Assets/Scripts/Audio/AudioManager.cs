using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using Debug = UnityEngine.Debug;
#else
using Debug = Constants.EditorDebug;
#endif

/// <summary> ゲーム内のサウンド管理クラス </summary>
public class AudioManager
{
    private static GameObject _audioObject = default;
    private static AudioSource _bgmSource = default;
    private static List<AudioSource> _seSources = default;

    private static AudioHolder _soundHolder = default;

    private static AudioManager _instance = default;

    private readonly Queue<AudioClip> _seQueue = new();

    public AudioSource BGMSource => _bgmSource;
    public List<AudioSource> SeSource => _seSources;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null) { Init(); }

            return _instance;
        }
    }

    /// <summary> AudioManagerの初期化処理 </summary>
    private static void Init()
    {
        _audioObject = new GameObject("AudioManager");
        _instance = new();

        var bgm = new GameObject("BGM");
        _bgmSource = bgm.AddComponent<AudioSource>();
        bgm.transform.parent = _audioObject.transform;

        var se = new GameObject("SE");
        _seSources = new() { se.AddComponent<AudioSource>() };
        se.transform.parent = _audioObject.transform;

        _soundHolder = Resources.Load<AudioHolder>("AudioHolder");

        //音量設定
        _bgmSource.volume = 1f;
        _seSources[0].volume = 1f;

        Object.DontDestroyOnLoad(_audioObject);
    }

    /// <summary> BGM再生 </summary>
    /// <param name="bgm"> どのBGMか </param>
    /// <param name="isLoop"> ループ再生するか（基本的にループする） </param>
    public void PlayBGM(BGMType bgm, bool isLoop = true)
    {
        var index = -1;
        foreach (var clip in _soundHolder.BGMClips)
        {
            index++;
            if (clip.BGMType == bgm) { break; }
        }
        if (index >= _soundHolder.BGMClips.Length) { Debug.LogError("指定したBGMが見つかりませんでした"); return; }

        _bgmSource.Stop();

        _bgmSource.loop = isLoop;
        _bgmSource.clip = _soundHolder.BGMClips[index].BGMClip;
        _bgmSource.Play();
    }

    /// <summary> SE再生 </summary>
    /// <param name="se"> どのSEか </param>
    public void PlaySE(SEType se)
    {
        var index = -1;
        foreach (var clip in _soundHolder.SEClips)
        {
            index++;
            if (clip.SEType == se) { break; }
        }
        if (index >= _soundHolder.SEClips.Length) { Debug.LogError("指定したSEが見つかりませんでした"); return; }
        //再生するSEを追加
        _seQueue.Enqueue(_soundHolder.SEClips[index].SEClip);

        //再生するSEがあれば、最後に追加したSEを再生
        if (_seQueue.Count > 0)
        {
            for (int i = 0; i < _seSources.Count; i++)
            {
                if (!_seSources[i].isPlaying) { _seSources[i].PlayOneShot(_seQueue.Dequeue()); return; }
            }

            var newSource = new GameObject("SE");
            _seSources.Add(newSource.AddComponent<AudioSource>());
            newSource.transform.parent = _audioObject.transform;

            _seSources[^1].PlayOneShot(_seQueue.Dequeue());
        }
    }

    /// <summary> BGMの再生を止める </summary>
    public void StopBGM() => _bgmSource.Stop();

    /// <summary> SEの再生を止める </summary>
    public void StopSE()
    {
        foreach (var source in _seSources) { source.Stop(); }
        _seQueue.Clear();
    }

    /// <summary> 指定したシーンのBGMを取得する </summary>
    public AudioClip GetBGMClip(BGMType bgm)
    {
        var index = -1;
        foreach (var clip in _soundHolder.BGMClips)
        {
            index++;
            if (clip.BGMType == bgm) { break; }
        }

        return _soundHolder.BGMClips[index].BGMClip;
    }

    public IEnumerator BGMPlayingWait()
    {
        yield return new WaitUntil(() => !_bgmSource.isPlaying);
    }

    public async Task BGMPlaying()
    {
        while (_bgmSource.isPlaying) { await Task.Yield(); }
    }

    public IEnumerator SEPlayingWait()
    {
        foreach (var source in _seSources)
        {
            yield return new WaitUntil(() => !source.isPlaying);
        }
    }

    public async Task SEPlaying()
    {
        foreach (var source in _seSources)
        {
            while (source.isPlaying) { await Task.Yield(); }
        }
    }

    #region 以下Audio系パラメーター設定用の関数
    /// <summary> BGMの音量設定 </summary>
    public void VolumeSettingBGM(float value) => _bgmSource.volume = value;

    /// <summary> SEの音量設定 </summary>
    public void VolumeSettingSE(float value)
    {
        foreach (var source in _seSources) { source.volume = value; }
    }
    #endregion
}