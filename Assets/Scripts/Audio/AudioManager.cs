using System.Collections.Generic;
using UnityEngine;

/// <summary> ゲーム内のサウンド管理クラス </summary>
public class AudioManager
{
    private static AudioSource _bgmSource = default;
    private static AudioSource _seSource = default;

    private static AudioHolder _soundHolder = default;

    private static AudioManager _instance = default;

    private readonly Queue<AudioClip> _seQueue = new();

    public AudioSource BGMSource => _bgmSource;
    public AudioSource SeSource => _seSource;

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
        var sound = new GameObject("AudioManager");
        _instance = new();

        var bgm = new GameObject("BGM");
        _bgmSource = bgm.AddComponent<AudioSource>();
        bgm.transform.parent = sound.transform;

        var se = new GameObject("SE");
        _seSource = se.AddComponent<AudioSource>();
        se.transform.parent = sound.transform;

        //引数に設定するのはResourcesフォルダからの相対パス
        _soundHolder = Resources.Load<AudioHolder>("AudioHolder");

        var bgmVolume = 1f;
        var seVolume = 1f;

        //音量設定
        _bgmSource.volume = bgmVolume;
        _seSource.volume = seVolume;

        Object.DontDestroyOnLoad(sound);
    }

    /// <summary> BGM再生 </summary>
    /// <param name="bgm"> どのBGMか </param>
    /// <param name="isLoop"> ループ再生するか </param>
    public void PlayBGM(BGMType bgm, bool isLoop = true)
    {
        var index = -1;
        foreach (var clip in _soundHolder.BGMClips)
        {
            index++;
            if (clip.BGMType == bgm) { break; }
        }

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
        //再生するSEを追加
        _seQueue.Enqueue(_soundHolder.SEClips[index].SEClip);

        //再生するSEがあれば、最後に追加したSEを再生
        if (_seQueue.Count > 0 && !_seSource.isPlaying) { _seSource.PlayOneShot(_seQueue.Dequeue()); }
    }

    /// <summary> BGMの再生を止める </summary>
    public void StopBGM() => _bgmSource.Stop();

    /// <summary> SEの再生を止める </summary>
    public void StopSE() { _seSource.Stop(); _seQueue.Clear(); }

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

    #region 以下Audio系パラメーター設定用の関数
    /// <summary> BGMの音量設定 </summary>
    public void VolumeSettingBGM(float value) => _bgmSource.volume = value;

    /// <summary> SEの音量設定 </summary>
    public void VolumeSettingSE(float value) => _seSource.volume = value;
    #endregion
}