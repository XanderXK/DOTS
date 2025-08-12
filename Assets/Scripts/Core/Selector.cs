using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core
{
    public class Selector : MonoBehaviour
    {
        [SerializeField] private RectTransform _selectorRect;
        private RectTransform _rootRectTransform;
        private Camera _mainCamera;
        private bool _isSelecting;
        private Vector2 _startPosition;

        public event Action<Vector2> OnSelectionEnded;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _rootRectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _selectorRect.gameObject.SetActive(true);
                _selectorRect.position = Mouse.current.position.ReadValue();
                _startPosition = Mouse.current.position.ReadValue();
                _selectorRect.sizeDelta = Vector2.zero;
                _isSelecting = true;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                OnSelectionEnded?.Invoke(_selectorRect.sizeDelta);
                _selectorRect.gameObject.SetActive(false);
                _isSelecting = false;
            }

            if (_isSelecting)
            {
                var currentPosition = Mouse.current.position.ReadValue();
                var sizeDelta = currentPosition - _startPosition;
                _selectorRect.sizeDelta = new Vector2(Mathf.Abs(sizeDelta.x), Mathf.Abs(sizeDelta.y)) / _rootRectTransform.localScale;
                _selectorRect.anchoredPosition = (_startPosition + sizeDelta / 2f) / _rootRectTransform.localScale;
            }
        }

        public bool IsInsideSelectionArea(Vector3 worldPosition)
        {
            var screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _selectorRect,
                screenPosition,
                null,
                out var localPoint);
            var isInside = _selectorRect.rect.Contains(localPoint);
            return isInside;
        }
    }
}