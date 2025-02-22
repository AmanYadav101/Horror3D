using System.Collections;
using UnityEngine;

public class JumpScareTrigger : MonoBehaviour
{
    public GameObject jumpscareModel;
    public AudioClip firstSound; 
    public AudioClip secondSound;
    public Camera mainCamera;
    public float shakeDuration = 0.5f; 
    public float shakeIntensity = 0.1f; 
    public float moveSpeed = 10f; 
    public GameObject flashScreen; 

    private bool _hasTriggered = false;
    private bool _hasLeft = false;
    private Transform _player;

    private void Awake()
    {
        jumpscareModel.SetActive(false);
        if (flashScreen != null)
        {
            flashScreen.SetActive(false); 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasTriggered)
        {
            _hasTriggered = true;
            PlaySound(firstSound); 
            _player = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !_hasLeft)
        {
            _hasLeft = true;
            StartCoroutine(TriggerJumpscare());
        }
    }

    IEnumerator TriggerJumpscare()
    {
        jumpscareModel.SetActive(true);

        while (Vector3.Distance(jumpscareModel.transform.position, _player.position) > 1.5f)
        {
            jumpscareModel.transform.position = Vector3.MoveTowards(jumpscareModel.transform.position, _player.position, moveSpeed * Time.deltaTime);
            jumpscareModel.transform.LookAt(_player);
            yield return null;
        }

        PlaySound(secondSound);
        jumpscareModel.SetActive(false);
        StartCoroutine(CameraShake());

        StartCoroutine(FlashEffect());

        yield return new WaitForSeconds(secondSound.length / 2);

        gameObject.SetActive(false);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }

    IEnumerator CameraShake()
    {
        Vector3 originalPos = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeIntensity, shakeIntensity);
            float y = Random.Range(-shakeIntensity, shakeIntensity);
            mainCamera.transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPos;
    }

    IEnumerator FlashEffect()
    {
        if (flashScreen == null) yield break;

        flashScreen.SetActive(true); 

        yield return new WaitForSeconds(0.1f); 

        CanvasGroup flashCanvas = flashScreen.GetComponent<CanvasGroup>();
        if (flashCanvas == null)
        {
            flashCanvas = flashScreen.AddComponent<CanvasGroup>(); 
        }

        float fadeTime = 0.5f; 
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            flashCanvas.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        flashScreen.SetActive(false); 
    }
}
