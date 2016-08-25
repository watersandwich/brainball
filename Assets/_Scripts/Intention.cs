using System.Collections;
using UnityEngine;

namespace Assets._Scripts
{
    /// <summary>An intention is the projectile you fire.</summary>
    [UnityComponent]
    public class Intention : MonoBehaviour
    {
        [AssignedInUnity, Range(0, 10)]
        public int MaxBouncesNotOnNeuronsBeforeDying = 4;

        [AssignedInUnity, Range(0, 60)]
        public float MaxSecondsNotTouchingAnythingBeforeDying = 10;

        private int bounceCountOnNonNeurons;

        private Coroutine deathAfterTimeCoroutine;

        [UnityMessage]
        public void Start()
        {
            bounceCountOnNonNeurons = 0;

            StartCountdown();
        }

        private void StartCountdown()
        {
            if (deathAfterTimeCoroutine != null)
                StopCoroutine(deathAfterTimeCoroutine);

            deathAfterTimeCoroutine = StartCoroutine(DieAfterTime());
        }

        private IEnumerator DieAfterTime()
        {
            yield return new WaitForSeconds(MaxSecondsNotTouchingAnythingBeforeDying);
            DestroyNeuron();
        }

        [UnityMessage]
        public void OnCollisionEnter(Collision collision)
        {
            var other = collision.gameObject;

            StartCountdown();

            var neuron = other.GetComponentInChildren<Neuron>() ?? other.GetComponentInParent<Neuron>();
            if (neuron == null)
            {
                BounceOnNonNeuron();
            }
            else
            {
                BounceOnNeuron();
            }
        }

        private void BounceOnNeuron()
        {
            bounceCountOnNonNeurons = 0;
        }

        private void BounceOnNonNeuron()
        {
            bounceCountOnNonNeurons++;

            if (bounceCountOnNonNeurons >= MaxBouncesNotOnNeuronsBeforeDying)
            {
                DestroyNeuron();
            }
        }

        private void DestroyNeuron()
        {
            Destroy(gameObject);
        }
    }
}