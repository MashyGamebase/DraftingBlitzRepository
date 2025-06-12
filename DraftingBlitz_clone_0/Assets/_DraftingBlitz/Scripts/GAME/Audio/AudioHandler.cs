using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public void ToggleBGM(bool toggle)
    {
        AudioManager.Instance.ToggleBGM(toggle);
    }

    public void ToggleSFX(bool toggle)
    {
        AudioManager.Instance.ToggleSFX(toggle);
    }
}