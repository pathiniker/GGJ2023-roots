using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class AudioManager : MonoBehaviour
{
    public const float ENGINE_MAX_PITCH = 1.5f;

    static public AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] AudioMixer _mixer;
    [SerializeField] List<AudioSource> _depthAudioLayers = new List<AudioSource>();
    [SerializeField] AudioSource _sfx;
    [SerializeField] AudioSource _sfxTwo;
    [SerializeField] AudioSource _engineSrc;
    [SerializeField] AudioSource _movementSrc;

    [Header("Audio Clips")]
    [SerializeField] List<AudioClip> _breakOreSfx = new List<AudioClip>();
    [SerializeField] List<AudioClip> _breakDirtSfx = new List<AudioClip>();
    [SerializeField] List<AudioClip> _collectOreSfx = new List<AudioClip>();

    [Header("Settings")]
    [SerializeField, PropertyRange(-3f, 3f)] public float EngineMaxPitch = 1.5f;

    bool _engineMoving = false;

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

    void HandleStartStop(AudioSource src, bool start)
    {
        if (start && src.isPlaying)
            return;

        if (!start && !src.isPlaying)
            return;

        if (start)
            src.Play();
        else
            src.Stop();
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
        float fadeTime = 5f;
        float maxVolume = 0.85f;
        src.DOFade(on ? maxVolume : 0f, fadeTime);
    }

    public void PlayBreakDirtSfx()
    {
        if (_breakDirtSfx.Count < 1)
            return;

        float pitch = Random.Range(0.8f, 1.2f);
        _sfx.pitch = pitch;
        _sfx.PlayOneShot(_breakDirtSfx[Random.Range(0, _breakDirtSfx.Count - 1)]);
    }

    public void PlayCollectOreSfx()
    {
        if (_collectOreSfx.Count < 1)
            return;

        float pitch = Random.Range(0.8f, 1.2f);
        _sfxTwo.pitch = pitch;
        _sfxTwo.PlayOneShot(_collectOreSfx[Random.Range(0, _collectOreSfx.Count - 1)]);
    }

    public void PlayBreakCellSfx()
    {
        if (_breakOreSfx.Count < 1)
            return;

        float pitch = Random.Range(0.8f, 1.2f);
        _sfx.pitch = pitch;
        _sfx.PlayOneShot(_breakOreSfx[Random.Range(0, _breakOreSfx.Count - 1)]);
    }

    public void PlayGroundMovementSfx(bool play)
    {
        HandleStartStop(_movementSrc, play);
    }

    public void NotifyEngineMoving(bool go, float engineMaxMultiplyer = 1f)
    {
        float rampTime = 1f;

        if (go && !_engineMoving)
        {
            // Ramp up
            rampTime /= engineMaxMultiplyer;
            _engineSrc.DOPitch(EngineMaxPitch * engineMaxMultiplyer, rampTime);
        } else if (!go && _engineMoving)
        {
            // Ramp down
            _engineSrc.DOPitch(1f, rampTime);
        }

        // Do nothing

        _engineMoving = go;
    }
}
