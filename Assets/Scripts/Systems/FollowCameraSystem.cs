using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class FollowCameraSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((ref Translation translation, ref FollowCameraComponent camera) =>
            {
                Camera.main.transform.position = translation.Value + camera.offset;
            });
    }
}
