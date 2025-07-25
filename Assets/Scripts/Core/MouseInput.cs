using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core
{
    public class MouseInput : MonoBehaviour
    {
        private Camera _mainCamera;
        public static event Action<Vector3> OnMouseClick; 
        public static Vector3 LastClickPosition { get; set; }

        private void Awake()
        {
            _mainCamera= Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out var hit))
                {
                    var targetPosition = hit.point;
                    OnMouseClick?.Invoke(targetPosition);
                    Debug.Log($"Clicked at position: {targetPosition}");
                    LastClickPosition = targetPosition;
                }
            }
        }

        private void OnDisable()
        {
            OnMouseClick=null;
        }
    }
}
