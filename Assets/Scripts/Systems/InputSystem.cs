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
            .ForEach((ref Translation translation) =>
        {
            translation.Value.x += Input.GetAxisRaw("Horizontal") * Time.DeltaTime;
            translation.Value.y += Input.GetAxisRaw("Vertical") * Time.DeltaTime;
        });
    }
}
