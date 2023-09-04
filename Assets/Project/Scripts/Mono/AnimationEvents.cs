using System.Collections.Generic;
using UnityEngine;

namespace KVSL
{
    public class AnimationEvents : MonoBehaviour
    {
        [field: SerializeField] List<CustomEvent> Events { get; set; }

        public void EnableEvent(int value)
        {
            Events[value].ThisEvent.Invoke();
        }

        public void EnableEventWithName(string name)
        {
            for(int i = 0; i < Events.Count; i++)
            {
                if(Events[i].Name == name)
                {
                    Events[i].ThisEvent.Invoke();
                    return;
                }
            }
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        //public void PlaySound(int key)
        //{
        //    AudioController.Instance.PlaySound((AudioKey)key);
        //}
    }
}
