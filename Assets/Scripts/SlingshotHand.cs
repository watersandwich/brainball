using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class SlingshotHand : MonoBehaviour
    {
        public SteamVR_TrackedObject Controller;

        public GameObject ProjectilePrefab;

        public Collider GrabArea;

        public Transform LaunchPoint;

        [Header("Adjustment")]
        public float MaxDistanceBack;

        public float DistanceMultiplier;

        public bool PauseOnNextShot;

        private new Rigidbody rigidbody;

        private GrabHand currentGrabHand;
        private GameObject currentProjectileHolding;

        private LineRenderer lineRenderer;

        public void Start()
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
            rigidbody = GetComponent<Rigidbody>();

            lineRenderer.SetWidth(0.05f, 0.05f);

            Controller.GetComponent<SteamVR_RenderModel>().gameObject.SetActive(false);
        }

        public void Update()
        {
            if(currentProjectileHolding != null)
            {
                currentProjectileHolding.transform.position = currentGrabHand.transform.position;

                lineRenderer.enabled = true;
                var aimVector = (LaunchPoint.transform.position - currentProjectileHolding.transform.position).normalized;

                RaycastHit hit;
                var didHit = Physics.Raycast(LaunchPoint.position, aimVector, out hit, float.MaxValue);
                if(didHit)
                {
                    lineRenderer.SetPositions(new[] 
                    {
                        LaunchPoint.transform.position,
                        hit.point
                    });
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }

            transform.rotation = Controller.transform.rotation * Quaternion.AngleAxis(90, Vector3.right);
        }

        public void FixedUpdate()
        {
            rigidbody.MovePosition(Controller.transform.position);
        }

        public void CreateProjectileInSlingshot(GrabHand grabHand)
        {
            this.currentGrabHand = grabHand;

            currentProjectileHolding = Instantiate(ProjectilePrefab);
        }

        public void Release()
        {
            var projectile = currentProjectileHolding;
            currentProjectileHolding = null;
            currentGrabHand = null;

            var delta = LaunchPoint.transform.position - projectile.transform.position;

            Debug.Log("Delta: " + delta);
            Debug.Log("Launch Point: " + LaunchPoint.transform.position);
            Debug.Log("Projectile Position: " + projectile.transform.position);

            if (PauseOnNextShot)
            {
                UnityEditor.EditorApplication.isPaused = true;
                return;
            }
                
            var launchVector = delta.normalized;
            var distance = delta.magnitude;

            projectile.GetComponent<Rigidbody>().velocity = launchVector * distance * DistanceMultiplier;
        }
    }
}
