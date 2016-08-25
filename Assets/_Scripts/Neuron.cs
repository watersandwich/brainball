using UnityEngine;

namespace Assets._Scripts
{
    [UnityComponent]
    public class Neuron : MonoBehaviour
    {
        [AssignedInUnity]
        public Material OffMaterial;

        [AssignedInUnity]
        public Material OnMaterial;

        [AssignedInUnity]
        public GameObject TurnOnWhenActivated;

        [UnityMessage]
        public void Start()
        {
            TurnOnWhenActivated.SetActive(false);
            GetComponentInChildren<MeshRenderer>().material = OffMaterial;
        }

        [UnityMessage]
        public void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("Projectile"))
            {
                GetComponentInChildren<MeshRenderer>().material = OnMaterial;
                TurnOnWhenActivated.SetActive(true);
            }
        }
    }
}
