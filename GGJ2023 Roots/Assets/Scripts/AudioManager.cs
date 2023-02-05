using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    static public AudioManager Instance;

    [SerializeField] AudioMixer _mixer;
    [SerializeField] List<AudioSource> _depthAudioLayers = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        for (int i = 0; i < _depthAudioLayers.Count; i++)
        {
            AudioSource src = _depthAudioLayers[i];
            src.volume = 0f;
            src.Play();
        }

        FadeAudioForDepth(true, DepthLevel.GroundLevel);
    }

    AudioSource GetAudioSrcForLevel(DepthLevel level)
    {
        int idx = (int)level;
        Debug.Assert(_depthAudioLayers.Count >= idx + 1);
        return _depthAudioLayers[idx];
    }

    public void FadeAudioForDepth(bool on, DepthLevel level)
    {
        AudioSource src = GetAudioSrcForLevel(level);
        src.DOFade(on ? 1f : 0f, 1.5f);
    }
}