using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource BGMSource;
    List<AudioSource> SESourceList;

    public AudioClip BGM;
    public AudioClip moveBlock;
    public AudioClip rotateBlock;
    public AudioClip onGroundBlock;
    public AudioClip lined;
    public AudioClip blockStacking;
    public AudioClip attack;
    public AudioClip throwBlock;
    public AudioClip hold;
    public AudioClip damage;
    public AudioClip nextStage;
    public AudioClip stageClear;

    int audioIndex = 0;
    // Start is called before the first frame update
    public void Init()
    {
        BGMSource = this.AddComponent<AudioSource>();
        BGMSource.volume = 0.05f;
        BGMSource.clip = BGM;
        BGMSource.loop = true;

        SESourceList = new List<AudioSource>();
        for(int i = 0; i < 20; i++)
        {
            GameObject obj = new GameObject("AudioSource" + i);
            obj.transform.SetParent(this.transform);
            AudioSource audioSource = obj.AddComponent<AudioSource>();
            audioSource.volume = 0.5f;
            audioSource.playOnAwake = false;
            SESourceList.Add(audioSource);
        }
    }

    public void SetBGM()
    {
        BGMSource.Play();
        BGMSource.DOFade(endValue: 0.05f, duration: 5f);
    }

    public void PlaySound(AudioClip clip, float volume = 1)
    {
        if(clip == null) return;
        SESourceList[audioIndex].Stop();
        SESourceList[audioIndex].PlayOneShot(clip, volume);
        audioIndex = ++audioIndex % 20;
    }

    public void PlayNormalSound(NormalSound normalSound)
    {
        switch (normalSound)
        {
            case NormalSound.MoveBlock:
                PlaySound(moveBlock);
                break;
            case NormalSound.RotateBlock:
                PlaySound(rotateBlock);
                break;
            case NormalSound.OnGroundBlock:
                PlaySound(onGroundBlock);
                break;
            case NormalSound.Lined:
                PlaySound(lined);
                break;
            case NormalSound.Attack:
                PlaySound(attack);
                break;
            case NormalSound.ThrowBlock:
                PlaySound(throwBlock);
                break;
            case NormalSound.BlockStacking:
                PlaySound(blockStacking);
                break;
            case NormalSound.Hold:
                PlaySound(hold);
                break;
            case NormalSound.Damage:
                PlaySound(damage);
                break;
            case NormalSound.NextBattle:
                PlaySound(nextStage);
                break;
            case NormalSound.StageClear:
                PlaySound(stageClear);
                break;
        }
    }
}

public enum NormalSound
{
    MoveBlock,
    RotateBlock,
    OnGroundBlock,
    Lined,
    Attack,
    ThrowBlock,
    BlockStacking,
    Hold,
    Damage,
    NextBattle,
    StageClear
}

public enum BGM
{
    Title,
    Game,
}