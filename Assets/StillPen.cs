using UnityEngine;
using Ubiq.XR;

// Implement Graspable interface, part of Ubiq XR interaction
// You can use any interaction toolkit you like with Ubiq!
// For the sake of keeping this tutorial simple, we use our simple in-built
// option.
public class StillPen : MonoBehaviour, IGraspable
{
    private Hand controller;

    private void LateUpdate()
    {
        if (controller)
        {
            transform.position = controller.transform.position;
            transform.rotation = controller.transform.rotation;
        }
    }

    void IGraspable.Grasp(Hand controller)
    {
        this.controller = controller;
    }

    void IGraspable.Release(Hand controller)
    {
        this.controller = null;
    }
}