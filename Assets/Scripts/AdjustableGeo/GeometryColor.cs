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

        public GameController gameController;

        public Slider redSlider, greenSlider, blueSlider;

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
            // Hide();
        }

        public void changeColor()
        {
            newestGeometry.ChangeRedChannel(redSlider);
            newestGeometry.ChangeGreenChannel(greenSlider);
            newestGeometry.ChangeBlueChannel(blueSlider);
            context.SendJson(new Message(newestGeometry.GetComponent<MeshRenderer>().material.color));
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}