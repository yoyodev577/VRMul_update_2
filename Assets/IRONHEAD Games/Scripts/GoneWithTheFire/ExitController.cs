using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoneWithTheFire
{
    public class ExitController : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip winClip;
        public GameObject panel;
        // Start is called before the first frame update

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            panel.SetActive(false);
        }

        public void EnablePanel() {
            
            panel.SetActive(true);
        }

        public void EnableSFX() {
            audioSource.PlayOneShot(winClip);
        }
    }
}
