using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class GeoAnchor : MonoBehaviour, IGraspable
    {
        public GameObject leftHand, rightHand;
        public float anchorSize = 0.2f;
        public float distanceToGeo = 0.15f;
        public Vector3 relativeCoef = new Vector3(1.0f, -1.0f, -1.0f);

        private Vector3 localGrabPoint;
        private Quaternion localGrabRotation;
        private Quaternion grabHandRotation;
        protected Transform follow;

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
            Debug.Log(transform.localPosition);
        }

        public void HideAnchor()
        {
            this.GetComponent<Renderer>().enabled = false;
        }

        public void ShowAnchor()
        {
            this.GetComponent<Renderer>().enabled = true;
        }

        public bool touchAnchor()
        {
            float distLeft = Vector3.Distance(this.transform.position, leftHand.transform.position);
            float distRight = Vector3.Distance(this.transform.position, rightHand.transform.position);
            return distLeft <= this.anchorSize || distRight <= this.anchorSize;
        }

        public virtual void adjustGeoScale(Vector3 translate)
        {
            return;
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
            }
        }

        public void Grasp(Hand controller)
        {
            var handTransform = controller.transform;
            localGrabPoint = handTransform.InverseTransformPoint(transform.position); //transform.InverseTransformPoint(handTransform.position);
            localGrabRotation = Quaternion.Inverse(handTransform.rotation) * transform.rotation;
            grabHandRotation = handTransform.rotation;
            follow = handTransform;
        }

        public void Release(Hand controller)
        {
            follow = null;
        }
    }
}