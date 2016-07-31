using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Neuron : MonoBehaviour
    {
        public Material OffMaterial;

        public Material OnMaterial;

        public GameObject TurnOnWhenActivated;

        public void Start()
        {
            TurnOnWhenActivated.SetActive(false);
            GetComponentInChildren<MeshRenderer>().material = OffMaterial;
        }

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
