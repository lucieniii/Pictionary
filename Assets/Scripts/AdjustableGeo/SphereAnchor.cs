using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class SphereAnchor : GeoAnchor
    {
        public override void adjustGeoScale(Vector3 translate)
        {
            // translate = geometry.transform.localRotation * translate;
            Vector3 geoPos = geometry.transform.localPosition;
            Vector3 geoScale = geometry.transform.localScale;
            Vector3 vertice = geometry.transform.localRotation * Vector3.Scale(geoScale / 2.0f, relativeCoef) + geoPos;
            Vector3 newVertice = vertice + translate;
            Vector3 newScale = Vector3.Scale(Quaternion.Inverse(geometry.transform.localRotation) * (newVertice - geoPos), relativeCoef) * 2.0f;
            Vector3 originScale = geometry.transform.localScale;
            
            for (int i = 0; i < 3; i++)
            {
                if (relativeCoef[i] > 0.1f || relativeCoef[i] < -0.1f)
                {   
                    originScale[i] = newScale[i];
                    geometry.transform.localScale = originScale;
                }
            }
        }
    }
}