using System.Collections.Generic;
using Main;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    Master,
    BGM,
    Eff,
}

public class AudioController : MonoBehaviour
{
    public AudioMixer mAudioMixer;

    //옵션에서 설정된 현재 배경음악과 효과 사운드의 불륨이다. 효과는 BGM을 제외한 모든 소리의 불륨을 담당한다.
    private float mCurrentBGMVolume, mCurrentEffectVolume;

    /// <summary>
    /// 클립들을 담는 딕셔너리
    /// </summary>
    private Dictionary<string, AudioClip> mClipsDictionary;

    /// <summary>
    /// 사전에 미리 로드하여 사용할 클립들
    /// </summary>
    [SerializeField] private AudioClip[] mPreloadClips;

    private List<TemporarySoundPlayer> mInstantiatedSounds;

    private void Awake()
    {
        GSC.audioController = this;
    }

    private void Start()
    {
        mClipsDictionary = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in mPreloadClips)
        {
            mClipsDictionary.Add(clip.name, clip);
        }

        mInstantiatedSounds = new List<TemporarySoundPlayer>();
    }

    /// <summary>
    /// 오디오의 이름을 기반으로 찾는다.
    /// </summary>
    /// <param name="clipName">오디오의 이름(파일 이름 기준)</param>
    /// <returns></returns>
    private AudioClip GetClip(string clipName)
    {
        AudioClip clip = mClipsDictionary[clipName];

        if (clip == null) { Debug.LogError(clipName + "이 존재하지 않습니다."); }

        return clip;
    }

    /// <summary>
    /// 사운드를 재생할 때, 루프 형태로 재생된경우에는 나중에 제거하기위해 리스트에 저장한다.
    /// </summary>
    /// <param name="soundPlayer"></param>
    private void AddToList(TemporarySoundPlayer soundPlayer)
    {
        mInstantiatedSounds.Add(soundPlayer);
    }

    /// <summary>
    /// 루프 사운드 중 리스트에 있는 오브젝트를 이름으로 찾아 제거한다.
    /// </summary>
    /// <param name="clipName"></param>
    public void StopLoopSound(string clipName)
    {
        foreach (TemporarySoundPlayer audioPlayer in mInstantiatedSounds)
        {
            if (audioPlayer.ClipName == clipName)
            {
                mInstantiatedSounds.Remove(audioPlayer);
                Destroy(audioPlayer.gameObject);
                return;
            }
        }
    
        Debug.LogWarning(clipName + "을 찾을 수 없습니다.");
    }

    /// <summary>
    /// 2D 사운드로 재생한다. 거리에 상관 없이 같은 소리 크기로 들린다.
    /// </summary>
    /// <param name="clipName">오디오 클립 이름</param>
    /// <param name="type">오디오 유형(BGM, EFFECT 등.)</param>
    public void PlaySound2D(string clipName, float delay = 0f, bool isLoop = false, SoundType type = SoundType.Eff)
    {
        GameObject obj = transform.Find("TemporarySoundPlayer 2D")?.gameObject;
        TemporarySoundPlayer soundPlayer = null;

        if (obj == null)
        {
            obj = new GameObject("TemporarySoundPlayer 2D");
            soundPlayer = obj.AddComponent<TemporarySoundPlayer>();
        }
        else if(obj.GetComponent<TemporarySoundPlayer>() != null)
        {

        }

        
        obj.transform.SetParent(transform, false);

        soundPlayer = obj.GetComponent<TemporarySoundPlayer>();

        if(soundPlayer == null)
            

        //루프를 사용하는경우 사운드를 저장한다.
        if (isLoop) { AddToList(soundPlayer); }
    
        soundPlayer.InitSound2D(GetClip(clipName));
        soundPlayer.Play(mAudioMixer.FindMatchingGroups(type.ToString())[0], delay, isLoop);
    }

    /// <summary>
    /// 3D 사운드로 재생한다.
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="audioTarget"></param>
    /// <param name="type"></param>
    /// <param name="attachToTarget"></param>
    /// <param name="minDistance"></param>
    /// <param name="maxDistance"></param>
    public void PlaySound3D(string clipName, Transform audioTarget, float delay = 0f, bool isLoop = false, SoundType type = SoundType.Eff, bool attachToTarget = true, float minDistance = 0.0f, float maxDistance = 50.0f)
    {
        GameObject obj = new GameObject("TemporarySoundPlayer 3D");
        obj.transform.localPosition = audioTarget.transform.position;
        if (attachToTarget) { obj.transform.parent = audioTarget; }

        TemporarySoundPlayer soundPlayer = obj.AddComponent<TemporarySoundPlayer>();

        //루프를 사용하는경우 사운드를 저장한다.
        if (isLoop) { AddToList(soundPlayer); }

        soundPlayer.InitSound3D(GetClip(clipName), minDistance, maxDistance);
        soundPlayer.Play(mAudioMixer.FindMatchingGroups(type.ToString())[0], delay, isLoop);
    }

    //씬이 로드될 때 옵션 매니저에의해 모든 사운드 불륨을 저장된 옵션의 크기로 초기화시키는 함수.
    public void InitVolumes(float bgm, float effect)
    {
        SetVolume(SoundType.BGM, bgm);
        SetVolume(SoundType.Eff, effect);
    }

    //옵션을 변경할 때 소리의 불륨을 조절하는 함수
    public void SetVolume(SoundType type, float value)
    {
        mAudioMixer.SetFloat(type.ToString(), value);
    }

    /// <summary>
    /// 무작위 사운드를 실행하기위해 랜덤 값을 리턴 (included)
    /// </summary>
    /// <param name="from">시작하는 인덱스 번호</param>
    /// <param name="includedTo">끝나는 인덱스 번호(포함)</param>
    /// <param name="isStartZero">한자리일경우 0으로 시작하는가? 예)01</param>
    /// <returns></returns>
    public static string Range(int from, int includedTo, bool isStartZero = false)
    {
        if (includedTo > 100 && isStartZero) { Debug.LogWarning("0을 포함한 세자리는 지원하지 않습니다."); }

        int value = UnityEngine.Random.Range(from, includedTo + 1);

        return value < 10 && isStartZero ? '0' + value.ToString() : value.ToString();
    }


}
