using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class Geometry: MonoBehaviour, IGraspable
    {
        private Vector3 localGrabPoint;
        private Quaternion localGrabRotation;
        private Quaternion grabHandRotation;
        private Transform follow;

        private Vector3 localAnchorPoint;
        private Quaternion localAnchorRotation;

        public GeoAnchor anchor;

        private void Start()
        {
            // anchor.setAnchorPosition(transform.localPosition, transform.localScale, transform.localRotation);
            // Debug.Log(transform.position);
            // Debug.Log(transform.lossyScale);
        }

        public void Grasp(Hand controller)
        {
            var handTransform = controller.transform;
            localGrabPoint = handTransform.InverseTransformPoint(transform.position); //transform.InverseTransformPoint(handTransform.position);
            localGrabRotation = Quaternion.Inverse(handTransform.rotation) * transform.rotation;
            grabHandRotation = handTransform.rotation;
            follow = handTransform;

            localAnchorPoint = handTransform.InverseTransformPoint(anchor.transform.position);
            localAnchorRotation = Quaternion.Inverse(handTransform.rotation) * anchor.transform.rotation;
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
                anchor.transform.rotation = follow.rotation * localAnchorRotation;
                anchor.transform.position = follow.TransformPoint(localAnchorPoint);
            }

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