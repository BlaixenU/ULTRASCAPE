using UnityEngine;

namespace UltraScape.Enemies
{
    public class BabyTrail : MonoBehaviour
    {
        private LineRenderer lr = null!;

        [Tooltip("1 = one tile scrolled per unit time. Can be negative.")]
        public float scrollSpeed = 8;

        public float scrollDecaySpeed = 0.25f;

        public float scrollSpeedWhenFading = 0.5f;

        [Tooltip("1 = opacity decreased from 1.0 to 0.0 in unit time.")]
        public float fadeSpeed = 1;

        public float shrinkSpeed = 1;

        private float originalScrollSpeed;

        private float originalWidth;

        private Color originalColor;


        [HideInInspector]
        public bool fading;

        private MaterialPropertyBlock propBlock = null!;

        private static readonly int tilingPropertyID = Shader.PropertyToID("_MainTex_ST");

        private static readonly int colorPropertyID = Shader.PropertyToID("_Color");

        private Color alphaToSubtract;

        private Vector4 tilingOffsetToAdd = new();

        public void StartFade()
        {
            fading = true;

            originalWidth = lr.startWidth;
            originalColor = lr.sharedMaterial.GetColor(colorPropertyID);
            originalScrollSpeed = scrollSpeed;
        }

        private void Awake()
        {
            lr = GetComponent<LineRenderer>();

            propBlock = new MaterialPropertyBlock();

            alphaToSubtract = new Color(0, 0, 0, 0);
            tilingOffsetToAdd = lr.sharedMaterial.GetVector(tilingPropertyID);
        }

        private void Update()
        {
            if (!lr)
            {
                return;
            }

            if (lr.startWidth <= 0)
            {
                Destroy(gameObject);
            }


            lr.GetPropertyBlock(propBlock);

            // delta uv offset

            tilingOffsetToAdd += new Vector4(0f, 0f, -scrollSpeed * (fading ? scrollSpeedWhenFading : 1) * Time.deltaTime, 0f);

            propBlock.SetVector(tilingPropertyID, tilingOffsetToAdd);

            // delta scrollspeed

            scrollSpeed = Mathf.MoveTowards(scrollSpeed, 0.0f, originalScrollSpeed * scrollDecaySpeed * Time.deltaTime);

            if (fading)
            {
                lr.startWidth -= shrinkSpeed * originalWidth * Time.deltaTime;

                alphaToSubtract += new Color(0, 0, 0, fadeSpeed * Time.deltaTime);

                var og = originalColor;

                propBlock.SetColor(colorPropertyID, og - alphaToSubtract);
            }

            lr.SetPropertyBlock(propBlock);
        }

    }
}