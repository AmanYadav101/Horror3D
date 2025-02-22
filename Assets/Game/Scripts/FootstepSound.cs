using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;

public class FootstepSound : MonoBehaviour
{
    [Header("Footstep Sounds")] public AudioSource footstepSource;
    public AudioClip[] woodFootsteps;
    public AudioClip[] dirtFootsteps;

    [Header("Heartbeat Sound")] public AudioSource heartbeatSource;
    public float normalHeartbeatPitch = 1.0f; 
    public float sprintHeartbeatPitch = 2f; 
    public float heartbeatLerpSpeed = 0.5f; 

    [Header("Movement")] public CharacterController controller;
    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.3f;

    private float stepTimer = 0f;
    private Vector3 lastPosition;
    float targetPitch = 0;

    void Start()
    {
        lastPosition = transform.position;

        if (!heartbeatSource.isPlaying)
        {
            heartbeatSource.pitch = normalHeartbeatPitch;
            heartbeatSource.Play();
        }
    }

    void Update()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        if (controller.isGrounded && speed > 0.1f)
        {
            stepTimer += Time.deltaTime;
            float currentStepInterval = Input.GetKey(KeyCode.LeftShift) ? sprintStepInterval : walkStepInterval;

            if (stepTimer >= currentStepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        PlayHeartbeat();
    }

    void PlayHeartbeat()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            targetPitch = sprintHeartbeatPitch;
        }
        else
        {
            targetPitch = normalHeartbeatPitch;
        }

        heartbeatSource.pitch = Mathf.Lerp(heartbeatSource.pitch, targetPitch, Time.deltaTime * heartbeatLerpSpeed);
    }

    void PlayFootstep()
    {
        AudioClip[] selectedClips = GetFootstepSounds();

        if (selectedClips.Length > 0)
        {
            footstepSource.clip = selectedClips[Random.Range(0, selectedClips.Length)];
            footstepSource.Play();
        }
    }

    AudioClip[] GetFootstepSounds()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
        {
            if (hit.collider.CompareTag("Wood")) return woodFootsteps;
            if (hit.collider.CompareTag("Dirt")) return dirtFootsteps;
        }

        return woodFootsteps;
    }
}