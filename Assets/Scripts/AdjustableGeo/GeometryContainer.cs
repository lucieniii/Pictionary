using UnityEngine;
using Ubiq.XR;
using Ubiq.Messaging;
using DrawAndGuess.Procedure;

namespace DrawAndGuess.Draw
{
    public class GeometryContainer: MonoBehaviour
    {
        public GameObject[] container;
        public int geometryCount;
        public int maxNumberOfGeometry = 100;

        private void Start()
        {
            container = new GameObject[maxNumberOfGeometry];
            geometryCount = 0;
        }

        public void Reset()
        {
            for (int i = 0; i < geometryCount; i++)
            {
                Destroy(container[i]);
            }
            geometryCount = 0;
        }

        public void AddGeometry(GameObject geometry)
        {
            container[geometryCount++] = geometry;
        }
    }
}