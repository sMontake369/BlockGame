using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource BGMSource;
    AudioSource SESource;

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
    // Start is called before the first frame update
    void Awake()
    {
        SESource = this.AddComponent<AudioSource>();
        BGMSource = this.AddComponent<AudioSource>();
        SetBGM();
    }

    public void SetBGM()
    {
        BGMSource.volume = 0.1f;
        BGMSource.clip = BGM;
        BGMSource.loop = true;
        BGMSource.Play();
        BGMSource.DOFade(endValue: 0.5f, duration: 5f);
    }

    void PlaySound(AudioClip clip)
    {
        SESource.PlayOneShot(clip);
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
            case NormalSound.NextStage:
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
        NextStage,
        StageClear
    }