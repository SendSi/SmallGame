using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace xcharts
{
    [System.Serializable]
    public class ProgressInfo
    {
        public Color backgroundColor = new Color32(42, 55, 62, 255);
        public Color progressColor = new Color32(122, 216, 159, 255);
        public float insideRadius = 150f;
        public float outsideRadius = 170f;
        public float space;
        public float left;
        public float right;
        public float top;
        public float bottom;
        public float progress = 0f;
    }

    public class ProgressChart : BaseChart
    {
        [SerializeField]
        private ProgressInfo progressInfo = new ProgressInfo();

        private float pieCenterX = 0f;
        private float pieCenterY = 0f;
        private float pieRadius = 0;

        public void SetProgress(float progress)         {             progressInfo.progress = progress;             SetVerticesDirty();         }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            UpdatePieCenter();
            float totalDegree = 360;
            float startDegree = 0;

            float degree = totalDegree * progressInfo.progress / 100;
            float toDegree = startDegree + degree;

            ChartUtils.DrawDoughnut(vh, new Vector3(pieCenterX, pieCenterY), progressInfo.insideRadius,
                progressInfo.outsideRadius, startDegree, toDegree, progressInfo.progressColor);
            startDegree = toDegree;

            ChartUtils.DrawDoughnut(vh, new Vector3(pieCenterX, pieCenterY), progressInfo.insideRadius,
                progressInfo.outsideRadius, startDegree, totalDegree, progressInfo.backgroundColor);

        }

        protected override void OnLegendButtonClicked()
        {
            base.OnLegendButtonClicked();

        }


        private void UpdatePieCenter()
        {
            float diffX = chartWid - progressInfo.left - progressInfo.right;
            float diffY = chartHig - progressInfo.top - progressInfo.bottom;
            float diff = Mathf.Min(diffX, diffY);
            if(progressInfo.outsideRadius <= 0)
            {
                pieRadius = diff / 3 * 2;
                pieCenterX = progressInfo.left + pieRadius;
                pieCenterY = progressInfo.bottom + pieRadius;
            }
            else
            {
                pieRadius = progressInfo.outsideRadius;
                pieCenterX = chartWid / 2;
                pieCenterY = chartHig / 2;
                if (progressInfo.left > 0) pieCenterX = progressInfo.left + pieRadius;
                if (progressInfo.right > 0) pieCenterX = chartWid - progressInfo.right - pieRadius;
                if (progressInfo.top > 0) pieCenterY = chartHig - progressInfo.top - pieRadius;
                if (progressInfo.bottom > 0) pieCenterY = progressInfo.bottom + pieRadius;
            }
        }
    }
}
