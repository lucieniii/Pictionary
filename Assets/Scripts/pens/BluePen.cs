using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

namespace DrawAndGuess.DrawAndGuess
{
    public class BluePen : Pen
    {
        private void Start()
        {
            nib = transform.Find("Grip/BlueNib");
            context = NetworkScene.Register(this);
            Debug.Log(context.Id);
            // var shader = Shader.Find("Unlit/Color");
            // drawingMaterial = new Material(shader);
        }
    }
}