using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.Teg;

[Serializable, NetSerializable]
public sealed class TegUIState : BoundUserInterfaceState
{
    public float PotentialEnergy;
    public float ActualEnergy;
    public float TargetEnergy;

    public TegCirculatorState? CircA;
    public TegCirculatorState? CircB;
}

[Serializable, NetSerializable]
public sealed class TegCirculatorState
{
    public float LastPressureDelta;
    public float LastMolesTransferred;
    public float LastTemperature;
}

[Serializable, NetSerializable]
public enum TegInterfaceKey
{
    Key
}
