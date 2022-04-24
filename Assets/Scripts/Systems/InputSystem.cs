using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public class InputSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<ControllableComponent>()
            .ForEach((ref Translation translation, ref SpeedComponent speed) =>
        {
            translation.Value.x += Input.GetAxisRaw("Horizontal") * speed.Value * Time.DeltaTime;
            translation.Value.y += Input.GetAxisRaw("Vertical") * speed.Value * Time.DeltaTime;
        });
    }
}
