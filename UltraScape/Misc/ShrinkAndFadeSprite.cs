using UnityEngine;

namespace UltraScape.Misc
{
    public class ShrinkAndFadeSprite : MonoBehaviour
    {
        public float shrinkSpeed;

        public float fadeSpeed;

        public bool destroyOnComplete = true;

        [HideInInspector]
        public bool completed = false;

        private Vector3 originalScale;

        private float originalAlpha;

        private SpriteRenderer spr = null!;

        private void Awake()
        {
            spr = GetComponent<SpriteRenderer>();
            if (spr == null)
            {
                Destroy(gameObject);
            }

            originalAlpha = spr!.color.a;
            originalScale = transform.localScale;

            shrinkSpeed = Mathf.Clamp(shrinkSpeed, 0.0f, 100f);
            fadeSpeed = Mathf.Clamp(fadeSpeed, 0.0f, 100f);
        }

        private void Update()
        {
            if (shrinkSpeed == 0 && fadeSpeed == 0)
            {
                return;
            }

            if (completed)
            {
                Destroy();
                return;
            }

            if (shrinkSpeed > 0)
            {
                transform.localScale -= originalScale * shrinkSpeed * Time.deltaTime;

                if (originalScale.x / 100 >= transform.localScale.x)
                {
                    completed = true;
                }
            }
            if (fadeSpeed > 0)
            {
                float nextAlpha = spr.color.a - originalAlpha * fadeSpeed * Time.deltaTime;
                spr.color = new(spr.color.r, spr.color.g, spr.color.b, nextAlpha);

                if (spr.color.a <= 0)
                {
                    completed = true;
                }
            }
        }

        private void Destroy()
        {
            if (destroyOnComplete)
            {
                Destroy(gameObject);
            }
        }
    }
}