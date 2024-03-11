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
    [SerializeField]
    private AudioSource _ambientAud;
    
    public bool isAmbientAudEnabled = false;
    public float _ambientVol = 100;
    public float _sfxVol =100;
    public float _masterVol = 100;
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

        if (isAmbientAudEnabled)
        {
            _ambientVol = 100;
        }
        else
        {
            _ambientVol = 0.001f;
        }
        
        
    }
    
    void SetValues()
    {


        _audMix.SetFloat("ambientVol", Mathf.Log(_ambientVol) * 20);
        _audMix.SetFloat("sfxVol", Mathf.Log(_sfxVol) * 20);
        _audMix.SetFloat("masterVol", Mathf.Log(_masterVol) * 20);
    }
    public void PlayOneShotAtPos1(AudioClip audioClip, Transform audTransform)
    {
        _aud1.clip = audioClip;
        _aud1.gameObject.transform.position = audTransform.position;
        _aud1.Play();
    }

    public void PlayOneShotAtPos2(AudioClip audioClip, Transform audTransform)
    {
        _aud2.clip = audioClip;
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
