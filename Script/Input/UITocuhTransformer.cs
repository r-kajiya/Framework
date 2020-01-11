using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
    public class UITocuhTransformer : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        RectTransform _rectTransform;
        float _baseScale;
        float _baseScaleDistance;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Input.touchCount > 1)
            {
                SetupScaling();
            }
        }

        void SetupScaling()
        {
            Touch one = Input.GetTouch(0);
            Touch two = Input.GetTouch(1);

            _baseScale = _rectTransform.localScale.x;
            _baseScaleDistance = Vector2.Distance(one.position, two.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1)
            {
                Scaling();
            }
            else
            {
                Moving(eventData);
            }
        }

        void Moving(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta;
        }

        void Scaling()
        {
            Touch one = Input.GetTouch(0);
            Touch two = Input.GetTouch(1);

            float scaleDistance = Vector2.Distance(one.position, two.position);
            float scale = _baseScale + (scaleDistance - _baseScaleDistance) / 300.0f;

            Vector3 localScale = new Vector3(scale, scale, scale);

            if (localScale.x < 1f)
            {
                localScale = new Vector3(1f, 1f, 1f);
                ;
            }

            if (localScale.x > 2f)
            {
                localScale = new Vector3(2f, 2f, 1f);
                ;
            }

            _rectTransform.localScale = localScale;
        }
    }
}