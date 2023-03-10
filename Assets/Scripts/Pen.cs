using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

namespace DrawAndGuess.DrawAndGuess
{
    // Adds simple networking to the 3d pen. The approach used is to draw locally
    // when a remote user tells us they are drawing, and stop drawing locally when
    // a remote user tells us they are not.
    public class Pen : MonoBehaviour, IGraspable, IUseable
    {
        private NetworkContext context;
        private bool owner;
        private Hand controller;
        private Transform nib;
        public Material drawingMaterial;
        private GameObject currentDrawing;
        public Color color;

        // Amend message to also store current drawing state
        private struct Message
        {
            public Vector3 position;
            public Quaternion rotation;
            public bool isDrawing; // new

            public Message(Transform transform, bool isDrawing)
            {
                this.position = transform.position;
                this.rotation = transform.rotation;
                this.isDrawing = isDrawing; // new
            }
        }

        private void Start()
        {
            nib = transform.Find("Grip/Nib");
            context = NetworkScene.Register(this);
            // context = NetworkScene.Register(this, NetworkScene.GenerateUniqueId());
            Debug.Log(context.Id);
            // var shader = Shader.Find("Unlit/Color");
            // drawingMaterial = new Material(shader);
        }

        public void ProcessMessage (ReferenceCountedSceneGraphMessage msg)
        {
            var data = msg.FromJson<Message>();
            transform.position = data.position;
            transform.rotation = data.rotation;

            // new
            // Also start drawing locally when a remote user starts
            if (data.isDrawing && !currentDrawing)
            {
                BeginDrawing();
            }
            if (!data.isDrawing && currentDrawing)
            {
                EndDrawing();
            }
        }

        private void FixedUpdate()
        {
            if (owner)
            {
                // new
                context.SendJson(new Message(transform,isDrawing:currentDrawing));
            }
        }

        private void LateUpdate()
        {
            if (controller)
            {
                transform.position = controller.transform.position;
                transform.rotation = controller.transform.rotation;
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

        void IUseable.Use(Hand controller)
        {
            BeginDrawing();
        }

        void IUseable.UnUse(Hand controller)
        {
            EndDrawing();
        }

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
            currentDrawing.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
            currentDrawing.transform.localRotation = Quaternion.identity;
        }

        private void EndDrawing()
        {
            var trail = currentDrawing.GetComponent<TrailRenderer>();
            currentDrawing.transform.parent = null;
            currentDrawing.GetComponent<TrailRenderer>().emitting = false;
            currentDrawing = null;
        }
    }
}