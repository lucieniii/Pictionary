using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class AdjustableCube: Geometry
    {
        private void ChangeScale(Vector3 afterScale)
        {
            transform.localScale = afterScale;
        }
    }
}