using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class InputSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<ControllableComponent>()
            .ForEach((ref Translation translation, ref SpeedComponent speed, ref FollowCameraComponent camera) =>
        {
            translation.Value.x += Input.GetAxisRaw("Horizontal") * speed.Value * Time.DeltaTime;
            translation.Value.y += Input.GetAxisRaw("Vertical") * speed.Value * Time.DeltaTime;

            Camera.main.transform.position = translation.Value + camera.offset;
        });
    }
}
