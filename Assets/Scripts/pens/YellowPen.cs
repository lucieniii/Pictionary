using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;

namespace DrawAndGuess.Draw
{
    // Adds simple networking to the 3d pen. The approach used is to draw locally
    // when a remote user tells us they are drawing, and stop drawing locally when
    // a remote user tells us they are not.
    public class YellowPen : Pen
    {
        // Override
        private void Start()
        {
            nib = transform.Find("Grip/YellowNib");
            context = NetworkScene.Register(this);
            // Debug.Log(context.Id);
            // var shader = Shader.Find("Unlit/Color");
            // drawingMaterial = new Material(shader);
        }
    }
}