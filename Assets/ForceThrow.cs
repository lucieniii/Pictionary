using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

// Better to be the same with file name
// Backup of Pen.cs
public class ForceThrow : MonoBehaviour, IGraspable, IUseable // new
{
    private NetworkContext context;
    private bool owner;
    private Hand controller;
    private Transform nib; // new
    private Material drawingMaterial; // new
    private GameObject currentDrawing; // new

    public GameObject ball;
    public GameObject playerCamera;

    public float ballThrowingForce =5f;

    private bool holdingBall = false;

    private struct Message
    {
        public Vector3 position;
        public Quaternion rotation;

        public Message(Transform transform)
        {
            this.position = transform.position;
            this.rotation = transform.rotation;
        }
    }

    private void Start()
    {
        nib = transform.Find("Grip/Nib"); // new
        context = NetworkScene.Register(this);
        var shader = Shader.Find("Particles/Standard Unlit"); // new
        drawingMaterial = new Material(shader); // new
    }

    public void ProcessMessage (ReferenceCountedSceneGraphMessage msg)
    {
        var data = msg.FromJson<Message>();
        transform.position = data.position;
        transform.rotation = data.rotation;
    }

    private void FixedUpdate()
    {
        if (owner)
        {
            context.SendJson(new Message(transform));
        }
    }

    private void LateUpdate()
    {
        if (controller)
        {
            holdingBall = true;
            transform.position = controller.transform.position;
            transform.rotation = controller.transform.rotation;
            if (Input.GetMouseButtonDown(0)){
                holdingBall = false;
                ball.GetComponent<Rigidbody>().AddForce(playerCamera.transform.forward * ballThrowingForce);
            }
        }

    }

    void IGraspable.Grasp(Hand controller)
    {
        owner = true;
        this.controller = controller;
    }

    void IGraspable.Release(Hand controller)
    {
        owner = false;
        this.controller = null;
    }

    // new
    void IUseable.Use(Hand controller)
    {
        BeginDrawing();
    }

    // new
    void IUseable.UnUse(Hand controller)
    {
        EndDrawing();
    }

    // new
    private void BeginDrawing()
    {
        currentDrawing = new GameObject("Drawing");
        var trail = currentDrawing.AddComponent<TrailRenderer>();
        trail.time = Mathf.Infinity;
        trail.material = drawingMaterial;
        trail.startWidth = .05f;
        trail.endWidth = .05f;
        trail.minVertexDistance = .02f;

        currentDrawing.transform.parent = nib.transform;
        currentDrawing.transform.localPosition = Vector3.zero;
        currentDrawing.transform.localRotation = Quaternion.identity;
    }

    // new
    private void EndDrawing()
    {
        var trail = currentDrawing.GetComponent<TrailRenderer>();
        currentDrawing.transform.parent = null;
        currentDrawing.GetComponent<TrailRenderer>().emitting = false;
        currentDrawing = null;
    }
}