using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class Selector : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out var hit))
                {
                    var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                    var query = new EntityQueryBuilder(Allocator.Temp)
                        .WithAll<UnitMove>().Build(entityManager);
                    
                    var entityArray = query.ToEntityArray(Allocator.Temp);
                    var moveArray = query.ToComponentDataArray<UnitMove>(Allocator.Temp);
                    for (var i = 0; i < moveArray.Length; i++)
                    {
                        var unitMove = moveArray[i];
                        unitMove.TargetPosition = hit.point;
                        // entityManager.SetComponentData(entityArray[i], unitMove);
                        moveArray[i] = unitMove;
                    }
                    
                    query.CopyFromComponentDataArray(moveArray);
                }
            }
        }
    }
}