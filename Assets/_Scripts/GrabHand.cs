using UnityEngine;
using VRTK;

namespace Assets._Scripts
{
    [UnityComponent]
    public class GrabHand : MonoBehaviour
    {
        [AssignedInUnity]
        public SteamVR_TrackedObject Controller;

        [AssignedInUnity]
        public Collider GrabArea;

        private bool isInSlingshotGrabArea;
        private bool isHoldingSlingshot;
        private SlingshotHand currentSlingshotHand;

        private new Rigidbody rigidbody;

        [UnityMessage]
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

        [UnityMessage]
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

        [UnityMessage]
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

        [UnityMessage]
        public void FixedUpdate()
        {
            rigidbody.MovePosition(Controller.transform.position);
        }
    }
}
