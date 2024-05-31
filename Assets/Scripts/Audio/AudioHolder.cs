using System;
using UnityEngine;

/// <summary> ゲーム内で使用する音をInspectorで設定するためのクラス </summary>
public class AudioHolder : MonoBehaviour
{
    #region Audio Controllers
    [Serializable]
    public class BGMController
    {
        [SerializeField]
        private BGMType _bgmType = default;
        [SerializeField]
        private AudioClip _bgmClip = default;

        public BGMType BGMType => _bgmType;
        public AudioClip BGMClip => _bgmClip;
    }

    [Serializable]
    public class SEController
    {
        [SerializeField]
        private SEType _seType = default;
        [SerializeField]
        private AudioClip _seClip = default;

        public SEType SEType => _seType;
        public AudioClip SEClip => _seClip;
    }
    #endregion

    [SerializeField]
    private BGMController[] _bgmClips = default;
    [SerializeField]
    private SEController[] _seClips = default;

    public BGMController[] BGMClips => _bgmClips;
    public SEController[] SEClips => _seClips;
}

public enum BGMType
{
    None,
    Title,
    InGame,
    P1Win,
    P2Win,
    Draw
}

public enum SEType
{
    None,
    P1Attack,
    P2Attack,
    P1Damaged,
    P2Damaged,
    UIClick,
    Wind,
    P1Damaged2,
    UIClick2,
}