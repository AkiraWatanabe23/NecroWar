using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary> フェードを管理するクラス </summary>
public class Fade : MonoBehaviour
{
    [Tooltip("フェードさせるUI")]
    [SerializeField]
    private Image _fadePanel = default;
    [Tooltip("実行時間")]
    [SerializeField]
    private float _fadeTime = 1f;

    public static Fade Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    /// <summary> フェードイン開始 </summary>
    public void StartFadeIn(params Action[] onCompleteFadeIn) => StartCoroutine(FadeIn(onCompleteFadeIn));

    /// <summary> フェードアウト開始 </summary>
    public void StartFadeOut(params Action[] onCompleteFadeOut) => StartCoroutine(FadeOut(onCompleteFadeOut));

    private IEnumerator FadeIn(params Action[] onCompleteFadeIn)
    {
        _fadePanel.gameObject.SetActive(true);

        //α値(透明度)を 1 -> 0 にする(少しずつ明るくする)
        float alpha = 1f;
        Color color = _fadePanel.color;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / _fadeTime;

            if (alpha <= 0f) { alpha = 0f; }

            color.a = alpha;
            _fadePanel.color = color;
            yield return null;
        }

        _fadePanel.gameObject.SetActive(false);

        if (onCompleteFadeIn != null)
        {
            foreach (var action in onCompleteFadeIn) { action?.Invoke(); }
        }
    }

    private IEnumerator FadeOut(params Action[] onCompleteFadeOut)
    {
        _fadePanel.gameObject.SetActive(true);

        //α値(透明度)を 0 -> 1 にする(少しずつ暗くする)
        float alpha = 0f;
        Color color = _fadePanel.color;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime / _fadeTime;

            if (alpha >= 1f) { alpha = 1f; }

            color.a = alpha;
            _fadePanel.color = color;
            yield return null;
        }

        if (onCompleteFadeOut != null)
        {
            foreach (var action in onCompleteFadeOut) { action?.Invoke(); }
        }
    }
}