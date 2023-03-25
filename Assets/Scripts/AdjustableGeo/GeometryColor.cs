using UnityEngine;
using UnityEngine.UI;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class GeometryColor: MonoBehaviour
    {
        public Geometry newestGeometry;

        public NetworkContext context;

        private struct Message
        {
            public Color color;

            public Message(Color color)
            {
                this.color = color;
            }
        }

        public void ProcessMessage (ReferenceCountedSceneGraphMessage msg)
        {
            var data = msg.FromJson<Message>();
            newestGeometry.GetComponent<MeshRenderer>().material.color = data.color;
        }

        private void Start()
        {
            context = NetworkScene.Register(this);
        }

        public void ChangeRedChannel(Slider slider)
        {
            newestGeometry.ChangeRedChannel(slider);
            context.SendJson(new Message(newestGeometry.GetComponent<MeshRenderer>().material.color));
        }

        public void ChangeGreenChannel(Slider slider)
        {
            newestGeometry.ChangeGreenChannel(slider);
            context.SendJson(new Message(newestGeometry.GetComponent<MeshRenderer>().material.color));
        }

        public void ChangeBlueChannel(Slider slider)
        {
            newestGeometry.ChangeBlueChannel(slider);
            context.SendJson(new Message(newestGeometry.GetComponent<MeshRenderer>().material.color));
        }
    }
}