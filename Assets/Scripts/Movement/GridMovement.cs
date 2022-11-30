using UnityEngine;
using UnityEngine.EventSystems;

namespace ArkadiumTest.Movement
{
    public class GridMovement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, ICancelHandler
    {
        enum State : byte
        {
            None = default,
            Down = 1,
            Dragging = 2
        }
        private State _state = State.None;
        private Vector2 _position = Vector2.zero;

        [SerializeField,Range(0.001f, 1)]
        private float _dragEase = 0.1f;

        [SerializeField]
        private Transform _selectionMarker;

        private Transform _selected;

        public void OnCancel(BaseEventData eventData)
        {
            _state = State.None;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_state == State.Down)
                _state = State.Dragging;

            float drag = (_position - eventData.position).x;
            drag *= _dragEase;
            transform.localRotation *= Quaternion.Euler(0, drag, 0);

            _position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _state = State.None;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _state = State.Down;
            _position = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_state == State.Down)
            {
                _selected = eventData.pointerCurrentRaycast.gameObject.transform;
                bool selected = _selected != null;

                _selectionMarker.gameObject.SetActive(selected);
                if (selected)
                    _selectionMarker.position = _selected.position;

#if UNITY_EDITOR && false
                UnityEditor.Selection.activeGameObject = eventData.pointerCurrentRaycast.gameObject;
#endif
            }
        }
    }
}
