using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class CylinderAnchor : GeoAnchor
    {
        public override void adjustGeoScale(Vector3 translate)
        {
            // translate = geometry.transform.localRotation * translate;
            Vector3 geoPos = geometry.transform.localPosition;
            Vector3 geoScale = geometry.transform.localScale;

            Vector3 vertice;
            Vector3 newVertice;
            Vector3 newScale;

            if (relativeCoef[1] > 0.1f) 
            {
                vertice = geometry.transform.localRotation * Vector3.Scale(geoScale, relativeCoef) + geoPos;
                newVertice = vertice + translate;
                newScale = Vector3.Scale(Quaternion.Inverse(geometry.transform.localRotation) * (newVertice - geoPos), relativeCoef);
                
            }
            else 
            {
                vertice = geometry.transform.localRotation * Vector3.Scale(geoScale / 2.0f, relativeCoef) + geoPos;
                newVertice = vertice + translate;
                newScale = Vector3.Scale(Quaternion.Inverse(geometry.transform.localRotation) * (newVertice - geoPos), relativeCoef) * 2.0f;
            }

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