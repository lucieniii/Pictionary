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
        public GeometryColor geometryColor;
        public GeometryContainer geometryContainer;
        public Transform parent;

        private struct Message
        {
            public NetworkId newId, newGeometryId;
            public Color color;

            public Message(NetworkId newId, NetworkId newGeometryId, Color color)
            {
                this.newId = newId;
                this.newGeometryId = newGeometryId;
                this.color = color;
            }
        }

        public void ProcessMessage (ReferenceCountedSceneGraphMessage msg)
        {
            var data = msg.FromJson<Message>();
            GeometryNetworking newGeometry = GameObject.Instantiate(geometryCopy, parent);
            geometryContainer.AddGeometry(gameObject);
            newGeometry.SetNetworkIdAndColor(data.newId, data.newGeometryId, data.color);
            // Debug.Log("Copying.");
        }

        private void Start()
        {
            context = NetworkScene.Register(this);
            Debug.Log(context.Id);
        }

        public void SetNetworkIdAndColor(NetworkId newId, NetworkId newGeometryId, Color color)
        {
            context.Id = newId;
            Geometry geometry = GetComponentInChildren<Geometry>();
            geometry.context.Id = newGeometryId;
            geometryColor.newestGeometry = geometry;
            geometry.GetComponent<MeshRenderer>().material.color = color;
        }

        public void CopySelf(Color color)
        {
            GeometryNetworking newGeometry = GameObject.Instantiate(geometryCopy, parent);
            geometryContainer.AddGeometry(gameObject);
            NetworkId newId = NetworkId.Unique(), newGeometryId = NetworkId.Unique();
            newGeometry.SetNetworkIdAndColor(newId, newGeometryId, color);
            context.SendJson(new Message(newId, newGeometryId, color));
            // Debug.Log("Copying.");
        }
    }
}