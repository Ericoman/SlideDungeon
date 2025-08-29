using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interactions
{
    public interface IInteractableHeld
    {
        void InteractHeld(GameObject interactor)
        {
            Debug.Log("InteractHeld not implemented");
        }

        void InteractHeldRelease(GameObject interactor)
        {
            Debug.Log("InteractHeldRelease not implemented");
        }

        void InteractHeldRight(GameObject interactor)
        {
            Debug.Log("InteractHeldRight not implemented");
        }
        void InteractHeldLeft(GameObject interactor)
        {
            Debug.Log("InteractHeldLeft not implemented");
        }

    }
}
