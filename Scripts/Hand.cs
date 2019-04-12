using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
   //Accion para obtener objeto - definido en acciones VR
    public SteamVR_Action_Boolean m_triggerAction = null;
    public SteamVR_Action_Boolean m_touchPadAction = null;

    //pose to hand
    private SteamVR_Behaviour_Pose m_pose = null;
    //componente Joint de la mano
    private FixedJoint m_joint = null;

    private Interactable m_currentInteractable = null;
    public List<Interactable> m_contactInteractables = new List<Interactable>();

    private Haptics m_hapticActions = null;
    // Start is called before the first frame update
    void Awake()
    {
        //Obtenemos el objeto de la mano de Steam VR y el joint.
        m_pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_joint = GetComponent<FixedJoint>();
        m_hapticActions = GetComponent<Haptics>();
    }

    // Update is called once per frame
    void Update()
    {
        //Trigger presionado
        if(m_triggerAction.GetStateDown(m_pose.inputSource)){
            print(m_pose.inputSource + "Trigger presionado.");

            //Ejecutamos accion del interactable si es que lo tenemos
            if(m_currentInteractable)
            {
                m_currentInteractable.Action();
                //Salimos del update
                return;
            }

            //recojemos objeto
            PickUp();
        }

        // //Triger soltado
        // if(m_triggerAction.GetStateUp(m_pose.inputSource)){
        //     print(m_pose.inputSource + "Trigger soltado.");
        //     Drop();
        // }

        if(m_touchPadAction.GetStateDown(m_pose.inputSource)){
            print(m_pose.inputSource + "Touchpad down.");
            Drop();
        }


    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Interactable")){
            print("Contacto con objeto.");
            
            //Hacemos vibrar el haptico para ofrecer mejor feedback
            if(m_hapticActions)
                m_hapticActions.Pulse(1,150,75,m_pose.inputSource);

            
            other.gameObject.GetComponent<Interactable>().onContact(true);
            m_contactInteractables.Add(other.gameObject.GetComponent<Interactable>());
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.gameObject.CompareTag("Interactable")){
            print("Saliendo de objeto.");

            other.gameObject.GetComponent<Interactable>().onContact(false);
            m_contactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
        }
    }

    public void PickUp(){
        //Obtener interactable mas cercano
        m_currentInteractable = GetNearestInteractable();
        
        //Verificar que no sea null
        if(!m_currentInteractable)
            return;

        //Que no este sujetado por otra mano
        if(m_currentInteractable.m_activeHand)
            m_currentInteractable.m_activeHand.Drop();

        //Establecer position del objeto
        //m_currentInteractable.transform.position = transform.position;
        m_currentInteractable.ApplyOffset(transform);

        //Agregar como cuerpo conectado al joint de la mano
        Rigidbody targetBody = m_currentInteractable.GetComponent<Rigidbody>();
        m_joint.connectedBody = targetBody;
        
        //Establecer active hand en interactable
        m_currentInteractable.m_activeHand = this;
    }

    public void Drop(){
        //verificar que no sea null
        if(!m_currentInteractable)
            return;

        // Aplciar fuerza si existe
         Rigidbody targetBody = m_currentInteractable.GetComponent<Rigidbody>();
         targetBody.velocity = m_pose.GetVelocity();
         targetBody.angularVelocity = m_pose.GetAngularVelocity();

        //Desacoplar
        m_joint.connectedBody = null;

        //Restablecer valores - clean
        m_currentInteractable.m_activeHand = null;
        m_currentInteractable = null;
    }

    //Obtenemos el objeto mas cercano de todos los que esten en el area de accion
    private Interactable GetNearestInteractable(){
       Interactable nearest = null;
       float minDistance = float.MaxValue;
       float distance =  0.0f;

       foreach(Interactable interactable in m_contactInteractables){
           distance = (interactable.transform.position - transform.position).sqrMagnitude;

           if( distance < minDistance ){
               minDistance = distance;
               nearest = interactable;
           }
       }
        return nearest;
    }
}
