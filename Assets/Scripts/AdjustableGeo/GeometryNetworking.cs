using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class GeometryNetworking: MonoBehaviour
    {
        public NetworkContext context;
        public GeometryNetworking geometryCopy;
        public Transform parent;

        private struct Message
        {
            public NetworkId newId, newGeometryId;

            public Message(NetworkId newId, NetworkId newGeometryId)
            {
                this.newId = newId;
                this.newGeometryId = newGeometryId;
            }
        }

        public void ProcessMessage (ReferenceCountedSceneGraphMessage msg)
        {
            var data = msg.FromJson<Message>();
            var geometry = GetComponentInChildren<Geometry>();
            GeometryNetworking newGeometry = GameObject.Instantiate(geometryCopy, parent);
            newGeometry.SetNetworkId(data.newId, data.newGeometryId);
            Debug.Log("Copying.");
        }

        private void Start()
        {
            // anchor.setAnchorPosition(transform.localPosition, transform.localScale, transform.localRotation);
            // Debug.Log(transform.position);
            // Debug.Log(transform.lossyScale);
            context = NetworkScene.Register(this);
            Debug.Log(context.Id);
            //Debug.Log(transform.parent);
        }

        public void SetNetworkId(NetworkId newId, NetworkId newGeometryId)
        {
            context.Id = newId;
            Geometry geometry = GetComponentInChildren<Geometry>();
            geometry.context.Id = newGeometryId;
        }

        public void CopySelf()
        {
            GeometryNetworking newGeometry = GameObject.Instantiate(geometryCopy, parent);
            NetworkId newId = NetworkId.Unique(), newGeometryId = NetworkId.Unique();
            newGeometry.SetNetworkId(newId, newGeometryId);
            context.SendJson(new Message(newId, newGeometryId));
            Debug.Log("Copying.");
        }
    }
}