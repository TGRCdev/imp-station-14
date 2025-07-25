using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Cargo.Components;

[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class CargoBountyConsoleComponent : Component
{
    /// <summary>
    /// The id of the label entity spawned by the print label button.
    /// </summary>
    [DataField("bountyLabelId", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string BountyLabelId = "PaperCargoBountyManifest";

    /// <summary>
    /// The time at which the console will be able to print a label again.
    /// </summary>
    [DataField("nextPrintTime", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextPrintTime = TimeSpan.Zero;

    /// <summary>
    /// The time between prints.
    /// </summary>
    [DataField("printDelay")]
    public TimeSpan PrintDelay = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The sound made when printing occurs
    /// </summary>
    [DataField("printSound")]
    public SoundSpecifier PrintSound = new SoundPathSpecifier("/Audio/Machines/printer.ogg");

    /// <summary>
    /// The sound made when the bounty is skipped.
    /// </summary>
    [DataField("skipSound")]
    public SoundSpecifier SkipSound = new SoundPathSpecifier("/Audio/Effects/Cargo/ping.ogg");

    /// <summary>
    /// The sound made when bounty skipping is denied due to lacking access.
    /// </summary>
    [DataField("denySound")]
    public SoundSpecifier DenySound = new SoundPathSpecifier("/Audio/Effects/Cargo/buzz_two.ogg");

    /// <summary>
    /// The time at which the console will be able to make the denial sound again.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan NextDenySoundTime = TimeSpan.Zero;

    /// <summary>
    /// The time between playing a denial sound.
    /// </summary>
    [DataField]
    public TimeSpan DenySoundDelay = TimeSpan.FromSeconds(2);
}

[NetSerializable, Serializable]
public sealed class CargoBountyConsoleState : BoundUserInterfaceState
{
    public List<CargoBountyData> Bounties;
    public List<CargoBountyHistoryData> History;
    public TimeSpan UntilNextSkip;

    public CargoBountyConsoleState(List<CargoBountyData> bounties, List<CargoBountyHistoryData> history, TimeSpan untilNextSkip)
    {
        Bounties = bounties;
        History = history;
        UntilNextSkip = untilNextSkip;
    }
}

[Serializable, NetSerializable]
public sealed class BountyPrintLabelMessage : BoundUserInterfaceMessage
{
    public string BountyId;

    public BountyPrintLabelMessage(string bountyId)
    {
        BountyId = bountyId;
    }
}

[Serializable, NetSerializable]
public sealed class BountySkipMessage : BoundUserInterfaceMessage
{
    public string BountyId;

    public BountySkipMessage(string bountyId)
    {
        BountyId = bountyId;
    }
}

//imp edit start - cargo bounty claiming & status
[Serializable, NetSerializable]
public sealed class BountyClaimedMessage : BoundUserInterfaceMessage
{
    public string BountyId;

    public BountyClaimedMessage(string bountyId)
    {
        BountyId = bountyId;
    }
}

[Serializable, NetSerializable]
public sealed class BountySetStatusMessage : BoundUserInterfaceMessage
{
    public string BountyId;
    public int Status;

    public BountySetStatusMessage(string bountyId, int status)
    {
        BountyId = bountyId;
        Status = status;
    }
}
//imp edit end
