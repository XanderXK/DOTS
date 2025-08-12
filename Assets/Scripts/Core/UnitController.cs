using System;
using Game.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using Plane = UnityEngine.Plane;

namespace Game.Core
{
    public class UnitController : MonoBehaviour
    {
        private Camera _mainCamera;
        private Selector _selector;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _selector = FindAnyObjectByType<Selector>();
        }

        private void OnEnable()
        {
            _selector.OnSelectionEnded += OnSelectionEnded;
        }

        private void OnSelectionEnded(Vector2 sizeDelta)
        {
            UnselectAllUnits();
            if (sizeDelta.magnitude > 0.1f)
            {
                SelectMultipleUnits();
            }
            else
            {
                SelectSingleUnit();
            }
        }

        private void Update()
        {
            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                SpawnUnits();
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                MoveSelectedUnits();
            }
        }

        private void SpawnUnits()
        {
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out var enter))
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var query = entityManager.CreateEntityQuery(typeof(Spawner));
               
                if (query.IsEmptyIgnoreFilter) return;

                var spawnerEntity = query.GetSingletonEntity();
                var spawner = entityManager.GetComponentData<Spawner>(spawnerEntity);
                if (spawner.SpawnRequested) return;
                spawner.SpawnRequested = true;
                spawner.SpawnPosition = ray.GetPoint(enter) + Vector3.up;
                entityManager.SetComponentData(spawnerEntity, spawner);
                query.Dispose();
            }
        }

        private void SelectSingleUnit()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            var physicsWorldSingleton = query.GetSingleton<PhysicsWorldSingleton>();
            var collisionWorld = physicsWorldSingleton.CollisionWorld;
            var cameraRay = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            var raycastInput = new RaycastInput
            {
                Start = cameraRay.GetPoint(0f),
                End = cameraRay.GetPoint(999f),
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1 << 6,
                    GroupIndex = 0
                }
            };

            if (collisionWorld.CastRay(raycastInput, out var hit))
            {
                var entity = hit.Entity;
                if (entityManager.HasComponent<UnitSelect>(entity))
                {
                    var unitSelect = entityManager.GetComponentData<UnitSelect>(entity);
                    unitSelect.IsSelected = true;
                    entityManager.RemoveComponent<Disabled>(unitSelect.VisualEntity);
                    entityManager.SetComponentData(entity, unitSelect);
                }
            }
        }

        private void SelectMultipleUnits()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform, UnitSelect>().Build(entityManager);

            var selectsArray = query.ToComponentDataArray<UnitSelect>(Allocator.Temp);
            var localTransformArray = query.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            for (var i = 0; i < localTransformArray.Length; i++)
            {
                var unitTransform = localTransformArray[i];
                var unitSelect = selectsArray[i];
                if (_selector.IsInsideSelectionArea(unitTransform.Position))
                {
                    unitSelect.IsSelected = true;
                    entityManager.RemoveComponent<Disabled>(unitSelect.VisualEntity);
                }

                selectsArray[i] = unitSelect;
            }

            query.CopyFromComponentDataArray(selectsArray);
        }

        private void UnselectAllUnits()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitSelect>().Build(entityManager);

            var selectsArray = query.ToComponentDataArray<UnitSelect>(Allocator.Temp);
            for (var i = 0; i < selectsArray.Length; i++)
            {
                var unitSelect = selectsArray[i];
                unitSelect.IsSelected = false;
                entityManager.AddComponent<Disabled>(unitSelect.VisualEntity);
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
                var movePositions = GetMovePositions(hit.point, selectsArray.Length, 1.5f);
                for (var i = 0; i < moveArray.Length; i++)
                {
                    if (!selectsArray[i].IsSelected) continue;
                    var unitMove = moveArray[i];
                    unitMove.TargetPosition = movePositions[i];
                    moveArray[i] = unitMove;
                }

                query.CopyFromComponentDataArray(moveArray);
            }
        }

        private NativeArray<float3> GetMovePositions(float3 centerPosition, int unitCount, float unitSpacing)
        {
            var columns = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
            var rows = Mathf.CeilToInt((float)unitCount / columns);

            var result = new NativeArray<float3>(unitCount, Allocator.Temp);
            var totalWidth = (columns - 1) * unitSpacing;
            var totalHeight = (rows - 1) * unitSpacing;
            var index = 0;
            for (var row = 0; row < rows && index < unitCount; row++)
            {
                for (var column = 0; column < columns && index < unitCount; column++)
                {
                    var xOffset = column * unitSpacing - totalWidth / 2f;
                    var zOffset = row * unitSpacing - totalHeight / 2f;
                    result[index] = new float3(centerPosition.x + xOffset, centerPosition.y, centerPosition.z + zOffset);
                    index++;
                }
            }

            return result;
        }

        private void OnDisable()
        {
            _selector.OnSelectionEnded -= OnSelectionEnded;
        }
    }
}