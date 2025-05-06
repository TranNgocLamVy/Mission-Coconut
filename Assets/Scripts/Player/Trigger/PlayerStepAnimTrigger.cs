using System;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerStepAnimTrigger : MonoBehaviourPun
{
    [NonSerialized] private AudioSource stepAudioSource;
    [NonSerialized] private Animator animator;
    [SerializeField] private List<AudioClip> stepAudioClips = new List<AudioClip>();
    private float stepTimer;

    private void Start()
    {
        stepAudioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        ConfigureAudioSource();
    }

    private void Update()
    {
        bool isGround = animator.GetBool("IsGrounded");
        if (!isGround) return;

        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        if (Mathf.Abs(horizontal) < 1 && Mathf.Abs(vertical) < 1) return;

        stepTimer -= Time.deltaTime;
        if (stepTimer > 0) return;
        if (vertical > 7)
        {
            stepTimer = 0.3f; 
        }
        else
        {
            stepTimer = 0.4f;
        }
        photonView.RPC("RPC_PlayStepSound", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_PlayStepSound()
    {
        AudioClip randomClip = stepAudioClips[UnityEngine.Random.Range(0, stepAudioClips.Count)];
        stepAudioSource.PlayOneShot(randomClip, 1f);
    }

    private void ConfigureAudioSource()
    {
        // Set up 3D sound properties
        stepAudioSource.spatialBlend = 1f; // Full 3D sound
        stepAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        stepAudioSource.minDistance = 1;
        stepAudioSource.maxDistance = 100;
        stepAudioSource.playOnAwake = false;
    }
}
