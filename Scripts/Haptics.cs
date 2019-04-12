using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Haptics : MonoBehaviour
{
    public SteamVR_Action_Vibration m_haptic;
    
    //Vibracion del Haptico
    public void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source){
        m_haptic.Execute(0,duration, frequency,amplitude,source);
        print("Pulso from " + source.ToString());
    }

}
