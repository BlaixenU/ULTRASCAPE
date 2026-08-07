using UnityEngine;

namespace UltraScape.API.Enemies
{
    public class BabyTrail : MonoBehaviour
    {
        private LineRenderer lr;

        [Tooltip("1 = one tile scrolled per unit time. Can be negative.")]
        public float scrollSpeed;

        [Tooltip("1 = opacity decreased from 1.0 to 0.0 in unit time.")]
        public float fadeSpeed;

        private float originalWidth;

        public TrailState State = TrailState.scroll;

        void Awake()
        {
            lr = GetComponent<LineRenderer>();
            originalWidth = lr.startWidth;
        }

        void Update()
        {
            if (!lr)
            {
                return;
            }

            if (lr.startWidth <= 0)
            {
                Destroy(gameObject);
            }


            Material mat = lr.materials[1];

            if (State == TrailState.scroll)
            {
                mat.mainTextureOffset += new Vector2(scrollSpeed, 0) * Time.deltaTime;
            }
            else // State == TrailState.fade
            {
                lr.startWidth -= fadeSpeed * originalWidth * Time.deltaTime;
                // figure out how to fade out
            }
        }


    }

    public enum TrailState
    {
        scroll,
        fade
    }
}