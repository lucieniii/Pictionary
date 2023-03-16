using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class GeoGenerator : MonoBehaviour, IGraspable
    {
        public GameObject GeoPrefab;
        
        private Geometry geo;

        public void Grasp(Hand controller)
        {
            GameObject adjustableGeo = Instantiate(GeoPrefab, transform.position, transform.rotation);
            geo = GetComponentInChildren<Geometry>();
            geo.Grasp(controller);
        }

        public void Release(Hand controller)
        {
            geo.Release(controller);
        }
    }
}