using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

namespace DrawAndGuess.Draw
{
    public class GreenPen : Pen
    {
        private void Start()
        {
            nib = transform.Find("Grip/GreenNib");
            context = NetworkScene.Register(this);
            Debug.Log(context.Id);
            // var shader = Shader.Find("Unlit/Color");
            // drawingMaterial = new Material(shader);
        }
    }
}