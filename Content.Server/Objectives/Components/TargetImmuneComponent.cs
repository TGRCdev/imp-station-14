namespace Content.Server.Objectives.Components;

/// <summary>
/// Entities with this component should not be considered for
/// things like kill targets
/// </summary>
[RegisterComponent]
public sealed partial class TargetImmuneComponent : Component
{
}
