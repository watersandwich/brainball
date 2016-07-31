using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRTK;

namespace Assets.Scripts
{
    public class GrabHand : MonoBehaviour
    {
        public SteamVR_TrackedObject Controller;

        public Collider GrabArea;

        private bool isInSlingshotGrabArea;
        private bool isHoldingSlingshot;
        private SlingshotHand currentSlingshotHand;

        private new Rigidbody rigidbody;

        public void Start()
        {
            rigidbody = GetComponent<Rigidbody>();

            var controllerEvents = Controller.GetComponent<VRTK_ControllerEvents>();
            controllerEvents.TriggerPressed += GrabHand_TriggerPressed;
            controllerEvents.TriggerReleased += ControllerEvents_TriggerReleased;

            Controller.GetComponent<SteamVR_RenderModel>().gameObject.SetActive(false);
        }

        private void GrabHand_TriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (isInSlingshotGrabArea)
            {
                Debug.Log("Grabbed Slingshot");
                isHoldingSlingshot = true;

                currentSlingshotHand.CreateProjectileInSlingshot(this);
            }
        }

        private void ControllerEvents_TriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            if(isHoldingSlingshot)
            {
                currentSlingshotHand.Release();
                isHoldingSlingshot = false;
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody == null || other.attachedRigidbody.GetComponent<SlingshotHand>() == null)
                return;

            var slingshotHand = other.attachedRigidbody.GetComponent<SlingshotHand>();

            Debug.Log("Entered collider " + other.attachedRigidbody.name);

            if (slingshotHand.GrabArea == other)
            {
                isInSlingshotGrabArea = true;
                currentSlingshotHand = slingshotHand;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.attachedRigidbody == null || other.attachedRigidbody.GetComponent<SlingshotHand>() == null)
                return;

            Debug.Log("Exited collider " + other.attachedRigidbody.name);

            if (other.attachedRigidbody.GetComponent<SlingshotHand>().GrabArea == other)
            {
                isInSlingshotGrabArea = false;
            }
        }

        public void FixedUpdate()
        {
            rigidbody.MovePosition(Controller.transform.position);
        }
    }
}
