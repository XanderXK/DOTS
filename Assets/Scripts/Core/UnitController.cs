using Game.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private RectTransform _selectorRect;
        private Camera _mainCamera;
        private bool _isSelecting;
        private Vector2 _startPosition;

        private void Awake()
        {
            _mainCamera = Camera.main;
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
                SelectUnitsInRect();
                _selectorRect.gameObject.SetActive(false);
                _isSelecting = false;
            }

            if (_isSelecting)
            {
                var currentPosition = Mouse.current.position.ReadValue();
                var sizeDelta = currentPosition - _startPosition;
                _selectorRect.sizeDelta = new Vector2(Mathf.Abs(sizeDelta.x), Mathf.Abs(sizeDelta.y));
                _selectorRect.anchoredPosition = _startPosition + sizeDelta / 2;
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                MoveSelectedUnits();
            }
        }
        
        private void SelectUnitsInRect()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform, UnitSelect>().Build(entityManager);
            
            var selectsArray = query.ToComponentDataArray<UnitSelect>(Allocator.Temp);
            var localTransformArray = query.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            for (var i = 0; i < localTransformArray.Length; i++)
            {
                var unitTransform = localTransformArray[i];
                var screenPosition = _mainCamera.WorldToScreenPoint(unitTransform.Position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _selectorRect,
                    screenPosition,
                    null, 
                    out var localPoint);
                var isInside = _selectorRect.rect.Contains(localPoint);
                var unitSelect = selectsArray[i];
                unitSelect.IsSelected = isInside;
                
                
                selectsArray[i] = unitSelect;
            }

            query.CopyFromComponentDataArray(selectsArray);
        }

        private void MoveSelectedUnits()
        {
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out var hit))
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var query = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<UnitMove, UnitSelect>().Build(entityManager);

                var selectsArray = query.ToComponentDataArray<UnitSelect>(Allocator.Temp);
                var moveArray = query.ToComponentDataArray<UnitMove>(Allocator.Temp);
                for (var i = 0; i < moveArray.Length; i++)
                {
                    if (!selectsArray[i].IsSelected) continue;
                    var unitMove = moveArray[i];
                    unitMove.TargetPosition = hit.point;
                    moveArray[i] = unitMove;
                }

                query.CopyFromComponentDataArray(moveArray);
            }
        }
    }
}