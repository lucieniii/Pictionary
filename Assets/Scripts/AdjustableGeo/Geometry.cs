using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class Geometry: MonoBehaviour, IGraspable
    {
        protected NetworkContext context;

        private Vector3 localGrabPoint;
        private Quaternion localGrabRotation;
        private Quaternion grabHandRotation;
        private Transform follow;

        private Vector3[] localAnchorPoints;
        private Quaternion[] localAnchorRotations;

        public GeoAnchor[] anchors;
        private int anchorNumber;

        private struct Message
        {
            public Vector3 position, localScale;
            public Quaternion rotation;

            public Message(Transform transform)
            {
                this.position = transform.position;
                this.rotation = transform.rotation;
                this.localScale = transform.localScale;
            }
        }

        public void ProcessMessage (ReferenceCountedSceneGraphMessage msg)
        {
            var data = msg.FromJson<Message>();
            transform.position = data.position;
            transform.rotation = data.rotation;
            transform.localScale = data.localScale;
        }

        public void broadCastChange ()
        {
            context.SendJson(new Message(transform));
        }


        private void Start()
        {
            // anchor.setAnchorPosition(transform.localPosition, transform.localScale, transform.localRotation);
            // Debug.Log(transform.position);
            // Debug.Log(transform.lossyScale);
            context = NetworkScene.Register(this);
            anchorNumber = anchors.Length;
            localAnchorPoints = new Vector3[anchorNumber];
            localAnchorRotations = new Quaternion[anchorNumber];
        }

        public void Grasp(Hand controller)
        {
            var handTransform = controller.transform;
            localGrabPoint = handTransform.InverseTransformPoint(transform.position); //transform.InverseTransformPoint(handTransform.position);
            localGrabRotation = Quaternion.Inverse(handTransform.rotation) * transform.rotation;
            grabHandRotation = handTransform.rotation;
            follow = handTransform;
            for (int i = 0; i < anchorNumber; i++)
            {
                GeoAnchor anchor = anchors[i];
                localAnchorPoints[i] = handTransform.InverseTransformPoint(anchor.transform.position);
                localAnchorRotations[i] = Quaternion.Inverse(handTransform.rotation) * anchor.transform.rotation;
            }
            
        }

        public void Release(Hand controller)
        {
            follow = null;
        }

        private void Update()
        {
            // anchor.setAnchorPosition(transform.localPosition, transform.localScale, transform.localRotation);
            if (follow)
            {
                transform.rotation = follow.rotation * localGrabRotation;
                transform.position = follow.TransformPoint(localGrabPoint);
                for (int i = 0; i < anchorNumber; i++)
                {
                    GeoAnchor anchor = anchors[i];
                    // anchor.transform.rotation = follow.rotation * localAnchorRotations[i];
                    // anchor.transform.position = follow.TransformPoint(localAnchorPoints[i]);
                    anchor.setPosition();
                }
                broadCastChange();
            }

            foreach (GeoAnchor anchor in anchors)
            {
                if (follow || anchor.touchAnchor())
                {
                    anchor.ShowAnchor();
                }
                else
                {
                    anchor.HideAnchor();
                }
            }
        }
    }
}