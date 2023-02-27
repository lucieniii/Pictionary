using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;
using Ubiq.XR;

public class Ball : MonoBehaviour, IGraspable
{
    // Start is called before the first frame update
    // Hand grasped;



    // public void Grasp(Hand controller){
    //     grasped = controller;
    // }

    // public void Release(Hand controller){
    //     grasped = null;
    // }
    private Hand controller;

    // private void LateUpdate()
    // {
        
    // }

    void IGraspable.Grasp(Hand controller)
    {
        this.controller = controller;
    }

    void IGraspable.Release(Hand controller)
    {
        this.controller = null;
    }
    //public void
    NetworkContext context;


    void Start()
    {
       context = NetworkScene.Register(this);
        
    }
    Vector3 lastPosition;

    

    // Update is called once per frame
    void Update()
    {
        if (controller)
        {
            transform.position = controller.transform.position;
            transform.rotation = controller.transform.rotation;
        }
        if(lastPosition != transform.localPosition)
        {
            lastPosition = transform.localPosition;
            context.SendJson(new Message()
            {
                position = transform.localPosition
            });
        }



    }
    struct Message{
        public Vector3 position;
    }   


    public void PrecessMessage(ReferenceCountedSceneGraphMessage message){
       var m = message.FromJson<Message>();

        // Use the message to update the Component
        transform.localPosition = m.position;

        // Make sure the logic in Update doesn't trigger as a result of this message
        lastPosition = transform.localPosition;
    }
    
}