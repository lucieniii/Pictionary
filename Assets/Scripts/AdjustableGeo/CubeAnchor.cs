using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class CubeAnchor : GeoAnchor
    {
        public Geometry cube;

        public override void adjustGeoScale(Vector3 translate)
        {
            Vector3 geoPos = cube.transform.localPosition;
            Vector3 geoScale = cube.transform.localScale;
            Vector3 vertice = cube.transform.localRotation * Vector3.Scale(geoScale / 2.0f, relativeCoef) + geoPos;
            Vector3 newVertice = vertice + translate;
            Vector3 newScale = Vector3.Scale(Quaternion.Inverse(cube.transform.localRotation) * (newVertice - geoPos), relativeCoef) * 2.0f;
            cube.transform.localScale = newScale;
        }
    }
}