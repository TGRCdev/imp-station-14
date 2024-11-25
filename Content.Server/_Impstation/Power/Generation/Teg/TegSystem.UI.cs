using Content.Server.Power.Components;
using Content.Shared._Impstation.Teg;
using Robust.Server.GameObjects;

namespace Content.Server.Power.Generation.Teg;

public sealed partial class TegSystem
{
    [Dependency] private readonly UserInterfaceSystem _bui = default!;

    private TegCirculatorState? GetCirculatorState(EntityUid? circ)
    {
        if (circ == null)
            return null;

        if (!TryComp<TegCirculatorComponent>(circ, out var comp))
            return null;

        return new TegCirculatorState()
        {
            LastPressureDelta = comp.LastPressureDelta,
            LastMolesTransferred = comp.LastMolesTransferred,
            LastTemperature = comp.LastTemperature,
        };
    }

    private void UpdateUI(
        EntityUid uid,
        TegGeneratorComponent component,
        TegNodeGroup nodeGroup,
        PowerSupplierComponent supplier
    )
    {
        TegCirculatorState?
            circA = GetCirculatorState(nodeGroup.CirculatorA?.Owner),
            circB = GetCirculatorState(nodeGroup.CirculatorB?.Owner);

        _bui.SetUiState(
            uid,
            TegInterfaceKey.Key,
            new TegUIState()
            {
                PotentialEnergy = supplier.MaxSupply,
                ActualEnergy = supplier.CurrentSupply,
                TargetEnergy = component.RampPosition,
                CircA = circA,
                CircB = circB,
            }
        );
    }
}
