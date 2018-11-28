using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCountdownPanel : MonoBehaviour {

    void StartMatchAudio()
    {
        AudioManager.Instance.PlayTempSound(AudioManager.AudioSoundType.StartMatch);
    }
}
