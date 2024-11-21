using Content.Shared.DoAfter;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.Interaction;

[RegisterComponent, NetworkedComponent]
public sealed partial class SpawnItemsOnDoAfterComponent : Component
{
    /// <summary>
    ///     The list of entities to spawn, with amounts and orGroups.
    /// </summary>
    [DataField("items", required: true)]
    public List<EntitySpawnEntry> Items = new();

    /// <summary>
    ///     A sound to play when the items are spawned. For example, gift boxes being unwrapped.
    /// </summary>
    [DataField("sound")]
    public SoundSpecifier? Sound = null;

    /// <summary>
    ///     How many uses before the item should delete itself.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("uses")]
    public int Uses = 1;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("duration", required: true)]
    public TimeSpan Duration;
}

[Serializable, NetSerializable]
public sealed partial class SpawnItemsDoAfterEvent : SimpleDoAfterEvent
{
}
