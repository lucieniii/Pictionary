using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class CubeAnchor : GeoAnchor
    {
        public override void adjustGeoScale(Vector3 translate)
        {
            Vector3 geoPos = geometry.transform.localPosition;
            Vector3 geoScale = geometry.transform.localScale;
            Vector3 vertice = geometry.transform.localRotation * Vector3.Scale(geoScale / 2.0f, relativeCoef) + geoPos;
            Vector3 newVertice = vertice + translate;
            Vector3 newScale = Vector3.Scale(Quaternion.Inverse(geometry.transform.localRotation) * (newVertice - geoPos), relativeCoef) * 2.0f;
            geometry.transform.localScale = newScale;
        }
    }
}