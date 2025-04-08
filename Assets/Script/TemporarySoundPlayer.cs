using UnityEngine;
using UnityEngine.Audio;

public class TemporarySoundPlayer : MonoBehaviour
{
    public bool isPlaying;
    private AudioSource mAudioSource;
    private float mTimer;
    public string ClipName { get; private set; }

    public void InitSound2D(AudioClip clip)
    {
        if(gameObject.GetComponent<AudioSource>() != null)
        {
            mAudioSource = gameObject.GetComponent<AudioSource>();
            if(mAudioSource.isPlaying)
            {
                mAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        else
            mAudioSource = gameObject.AddComponent<AudioSource>();

        mAudioSource.clip = clip;
        mAudioSource.spatialBlend = 0f; // 2D 사운드
        ClipName = clip.name;
    }

    public void InitSound3D(AudioClip clip, float minDistance, float maxDistance)
    {
        if (gameObject.GetComponent<AudioSource>() != null)
        {
            mAudioSource = gameObject.GetComponent<AudioSource>();
            if (mAudioSource.isPlaying)
            {
                mAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        else
            mAudioSource = gameObject.AddComponent<AudioSource>();

        mAudioSource.clip = clip;
        mAudioSource.spatialBlend = 1f; // 3D 사운드
        mAudioSource.minDistance = minDistance;
        mAudioSource.maxDistance = maxDistance;
        ClipName = clip.name;
    }

    public void Play(AudioMixerGroup mixerGroup, float delay, bool isLoop)
    {
        mAudioSource.outputAudioMixerGroup = mixerGroup;
        mAudioSource.playOnAwake = false;
        mAudioSource.loop = isLoop;
        mAudioSource.PlayDelayed(delay);
        isPlaying = true;

        if (!isLoop)
        {
            mTimer = mAudioSource.clip.length + delay;
            StartCoroutine(DestroyAfterPlay());
        }
    }

    private System.Collections.IEnumerator DestroyAfterPlay()
    {
        yield return new WaitForSeconds(mTimer);
        isPlaying = false;
        Destroy(gameObject);
    }
}