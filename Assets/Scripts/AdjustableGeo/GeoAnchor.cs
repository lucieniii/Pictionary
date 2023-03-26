using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class GeoAnchor : MonoBehaviour, IGraspable
    {
        protected NetworkContext context;

        public GameObject leftHand, rightHand;
        public float anchorSize = 0.2f;
        // public float distanceToGeo = 0.15f;
        public Vector3 relativeCoef = new Vector3(1.0f, -1.0f, -1.0f);

        private Vector3 localGrabPoint;
        private Quaternion localGrabRotation;
        private Quaternion grabHandRotation;
        private Transform follow;

        public GameObject hiddenAnchor;
        public Geometry geometry;

        public GameController gameController;

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

        public void setAnchorPosition(Vector3 geoPos, Vector3 geoScale, Quaternion geoRotation)
        {
            // Parameters all local
            // transform.localPosition = Vector3.Scale(geoScale / 2.0f, relativeCoef) + distanceToGeo * relativeCoef;
            Vector3 dir = Vector3.Scale(geoScale / 2.0f, relativeCoef);
            transform.localPosition = geoRotation * dir + geoPos;
        }

        private void Start()
        {
            HideAnchor();
            context = NetworkScene.Register(this);
            // Debug.Log(transform.localPosition);
        }

        public void HideAnchor()
        {
            this.GetComponent<Renderer>().enabled = false;
        }

        public void ShowAnchor()
        {
            if (gameController.CanUse())
            {
                this.GetComponent<Renderer>().enabled = true;
            }
        }

        public bool touchAnchor()
        {
            if (follow)
            {
                return true;
            }
            float distLeft = Vector3.Distance(this.transform.position, leftHand.transform.position);
            float distRight = Vector3.Distance(this.transform.position, rightHand.transform.position);
            return distLeft <= this.anchorSize || distRight <= this.anchorSize;
        }

        public virtual void adjustGeoScale(Vector3 translate)
        {
            return;
        }

        public void setPosition()
        {
            transform.position = hiddenAnchor.transform.position;
            transform.rotation = hiddenAnchor.transform.rotation;
            broadCastChange();
        }

        private void Update()
        {
            if (follow)
            {
                Vector3 previousAnchorPos, afterAnchorPos;
                previousAnchorPos = transform.localPosition;

                // transform.rotation = follow.rotation * localGrabRotation;
                transform.position = follow.TransformPoint(localGrabPoint);

                afterAnchorPos = transform.localPosition;
                adjustGeoScale(afterAnchorPos - previousAnchorPos);

                // setPosition();
            }
        }

        public void Grasp(Hand controller)
        {
            if (gameController.CanUse())
            {
                var handTransform = controller.transform;
                localGrabPoint = handTransform.InverseTransformPoint(transform.position); //transform.InverseTransformPoint(handTransform.position);
                localGrabRotation = Quaternion.Inverse(handTransform.rotation) * transform.rotation;
                grabHandRotation = handTransform.rotation;
                follow = handTransform;
            }
        }

        public void Release(Hand controller)
        {
            setPosition();
            geometry.broadCastChange();
            follow = null;
        }
    }
}