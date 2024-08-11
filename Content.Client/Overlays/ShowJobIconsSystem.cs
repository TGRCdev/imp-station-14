using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Overlays;
using Content.Shared.PDA;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class ShowJobIconsSystem : EquipmentHudSystem<ShowJobIconsComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly AccessReaderSystem _accessReader = default!;

    [ValidatePrototypeId<StatusIconPrototype>]
    private const string JobIconForNoId = "JobIconNoId";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StatusIconComponent, GetStatusIconsEvent>(OnGetStatusIconsEvent);
    }

    private void OnGetStatusIconsEvent(EntityUid uid, StatusIconComponent _, ref GetStatusIconsEvent ev)
    {
        if (!IsActive)
            return;

        var iconId = JobIconForNoId;

        if (_accessReader.FindAccessItemsInventory(uid, out var items))
        {
            foreach (var item in items)
            {
                // PDA
                if (TryComp<PdaComponent>(item, out var pda)
                    && pda.ContainedId != null
                    && TryComp<IdCardComponent>(pda.ContainedId, out var id))
                {
                    iconId = id.JobIcon;
                    break;
                }

                // ID Card
                if (TryComp<IdCardComponent>(item, out id))
                {
                    iconId = id.JobIcon;
                    // If we find a PDA, prioritize that
                }
            }
        }

        if (_prototype.TryIndex<StatusIconPrototype>(iconId, out var iconPrototype))
            ev.StatusIcons.Add(iconPrototype);
        else
            Log.Error($"Invalid job icon prototype: {iconPrototype}");
    }
}
