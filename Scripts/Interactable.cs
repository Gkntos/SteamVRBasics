using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    [HideInInspector]
    public Hand m_activeHand;
    public Vector3 m_Offset = Vector3.zero;

    public Material hoverMaterial = null;

    [HideInInspector]
    public virtual void Action()
    {
        print("Action");
    }

    public void ApplyOffset(Transform hand)
    {
        transform.SetParent(hand);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = m_Offset;
        transform.SetParent(null);
    }

    public void onContact(bool inside){
        if(hoverMaterial)
        {
            if(inside)
                this.GetComponent<Renderer>().materials[1] = hoverMaterial;
            else
                this.GetComponent<Renderer>().materials[1] = null;
        }
    }
}
