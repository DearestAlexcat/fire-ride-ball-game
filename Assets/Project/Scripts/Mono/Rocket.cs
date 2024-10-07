using UnityEngine;
using DG.Tweening;

namespace Client
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] ParticleSystem ring;
        [SerializeField] ParticleSystem glow;
        [SerializeField] ParticleSystem sparks;

        public int Pointer { get; set; }

        public void PlayRingFx()
        {
            ring.Play();
        }

        public void PlayPickupFx()
        {
            glow.Play();
            sparks.Play();
        }

        public void Disappearance()
        {
            GetComponent<Collider>().enabled = false;

            var scale = transform.localScale;

            transform.DOScale(0f, 0.2f).OnComplete(() => {
                gameObject.SetActive(false);
                transform.localScale = scale;
                GetComponent<Collider>().enabled = true;
            });
        }
    }
}