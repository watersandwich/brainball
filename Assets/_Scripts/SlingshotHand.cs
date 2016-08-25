using UnityEngine;

namespace Assets._Scripts
{
    public class SlingshotHand : MonoBehaviour
    {
        [AssignedInUnity]
        public SteamVR_TrackedObject Controller;

        [AssignedInUnity]
        public GameObject ProjectilePrefab;

        [AssignedInUnity]
        public GameObject GhostPrefab;

        [AssignedInUnity]
        public Collider GrabArea;

        [AssignedInUnity]
        public Transform LaunchPoint;

        [Header("Adjustment"), AssignedInUnity]
        public float MaxDistanceBack;

        [AssignedInUnity]
        public float DistanceMultiplier;

        [AssignedInUnity]
        public bool PauseOnNextShot;

        private new Rigidbody rigidbody;

        private GrabHand currentGrabHand;
        private GameObject currentProjectileHolding;

        private LineRenderer lineRenderer;

        [UnityMessage]
        public void Start()
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
            rigidbody = GetComponent<Rigidbody>();

            lineRenderer.SetWidth(0.05f, 0.05f);

            Controller.GetComponent<SteamVR_RenderModel>().gameObject.SetActive(false);
        }

        [UnityMessage]
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

        [UnityMessage]
        public void FixedUpdate()
        {
            rigidbody.MovePosition(Controller.transform.position);
        }

        public void CreateProjectileInSlingshot(GrabHand grabHand)
        {
            currentGrabHand = grabHand;

            currentProjectileHolding = Instantiate(GhostPrefab);
        }

        public void Release()
        {
            var ghost = currentProjectileHolding;
            currentProjectileHolding = null;
            currentGrabHand = null;

            var projectile = (GameObject)Instantiate(ProjectilePrefab, ghost.transform.position, Quaternion.identity);
            Destroy(ghost);
            
            var delta = LaunchPoint.transform.position - projectile.transform.position;

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
