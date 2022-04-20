using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct FollowCameraComponent : IComponentData
{
    public float3 offset;
}
