using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField]
    private AudioSource _aud1;
    [SerializeField]
    private AudioSource _aud2;
    [SerializeField]
    private AudioSource _audFoot;
    
    
    
    public float _ambientVol = 1;
    public float _sfxVol = 1;
    public float _masterVol = 1;
    [SerializeField]
    private AudioMixer _audMix;
    



    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
            
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        SetValues();

        
        
        
    }
    
    void SetValues()
    {
        _ambientVol = Mathf.Clamp(_ambientVol, 0.001f, 1f);
        _sfxVol = Mathf.Clamp(_sfxVol, 0.001f, 1f);
        _masterVol = Mathf.Clamp(_masterVol, 0.001f, 1f);

        _audMix.SetFloat("ambientVol", Mathf.Log(_ambientVol) * 20);
        _audMix.SetFloat("sfxVol", Mathf.Log(_sfxVol) * 20);
        _audMix.SetFloat("masterVol", Mathf.Log(_masterVol) * 20);
    }
    public void PlayOneShotAtPos1(AudioClip audioClip, Transform audTransform, float pitch)
    {
        _aud1.clip = audioClip;
        _aud1.pitch = pitch;
        _aud1.gameObject.transform.position = audTransform.position;
        _aud1.Play();
    }

    public void PlayOneShotAtPos2(AudioClip audioClip, Transform audTransform, float pitch)
    {
        _aud2.clip = audioClip;
        _aud2.pitch = pitch;
        _aud2.gameObject.transform.position = audTransform.position;
        _aud2.Play();
    }

    public void PlayOneShotAtPosRandPitch(AudioClip audioClip, Transform audTransform,float pitchMin, float pitchMax)
    {
        float randNumb = Random.Range(pitchMin, pitchMax);
        _audFoot.clip = audioClip;
        _audFoot.pitch = randNumb;
        _audFoot.gameObject.transform.position = audTransform.position;
        _audFoot.Play();
    }

}
