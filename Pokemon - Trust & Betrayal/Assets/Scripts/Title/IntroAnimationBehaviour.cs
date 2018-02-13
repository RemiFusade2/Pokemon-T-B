/* 
 * This file is subject to the terms and conditions defined in
 * file 'LICENSE.txt', which is part of this source code package.
 * 
 * AUTHOR: Rémi Fusade
 */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class deals with all sounds that are played during the intro cutscene.
/// Each method is called from the animation.
/// </summary>
public class IntroAnimationBehaviour : MonoBehaviour {

    public Animator introAnimator;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip animationMusicClip;
    public AudioClip titleMusicClip;

    public AudioClip jumpSoundClip;
    public AudioClip attackSoundClip;

    [Header("Title screen")]
    public Animator titleAnimator;

    // Use this for initialization
    void Start ()
    {
        musicSource.clip = animationMusicClip;
        sfxSource.clip = jumpSoundClip;
        musicSource.Play();
        musicSource.GetComponent<Animator>().SetBool("Playing", true);
        introAnimator.SetTrigger("Start");
    }

    public void PlayJumpSound()
    {
        sfxSource.clip = jumpSoundClip;
        sfxSource.Play();
    }
    public void PlayAttackSound()
    {
        sfxSource.clip = attackSoundClip;
        sfxSource.Play();
    }

    public void ShowTitleScreen()
    {
        titleAnimator.gameObject.SetActive(true);
        musicSource.Stop();
        musicSource.clip = titleMusicClip;
        musicSource.Play();
        titleAnimator.SetTrigger("Appears");
    }
}
