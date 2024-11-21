using Content.Server.Administration.Logs;
using Content.Server.Cargo.Systems;
using Content.Shared._Impstation.Interaction;
using Content.Shared.Database;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Random;
using static Content.Shared.Storage.EntitySpawnCollection;

namespace Content.Server._Impstation.Interaction;

public sealed partial class SpawnItemsOnDoAfterSystem : SharedSpawnItemsOnDoAfterSystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly PricingSystem _pricing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpawnItemsOnDoAfterComponent, SpawnItemsDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<SpawnItemsOnDoAfterComponent, PriceCalculationEvent>(CalculatePrice, before: [typeof(PricingSystem)]);
    }

    private void CalculatePrice(Entity<SpawnItemsOnDoAfterComponent> entity, ref PriceCalculationEvent args)
    {
        var ungrouped = CollectOrGroups(entity.Comp.Items, out var orGroups);

        foreach (var entry in ungrouped)
        {
            var protUid = Spawn(entry.PrototypeId, MapCoordinates.Nullspace);

            // Calculate the average price of the possible spawned items
            args.Price += _pricing.GetPrice(protUid) * entry.SpawnProbability * entry.GetAmount(getAverage: true);

            QueueDel(protUid);
        }

        foreach (var group in orGroups)
        {
            foreach (var entry in group.Entries)
            {
                var protUid = Spawn(entry.PrototypeId, MapCoordinates.Nullspace);

                // Calculate the average price of the possible spawned items
                args.Price += _pricing.GetPrice(protUid) *
                                (entry.SpawnProbability / group.CumulativeProbability) *
                                entry.GetAmount(getAverage: true);

                QueueDel(protUid);
            }
        }

        args.Handled = true;
    }

    private void OnDoAfter(EntityUid uid, SpawnItemsOnDoAfterComponent comp, SpawnItemsDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        var spawnEntities = GetSpawns(comp.Items, _random);

        foreach (var proto in spawnEntities)
        {
            var ent = SpawnNextToOrDrop(proto, uid);
            _adminLogger.Add(LogType.EntitySpawn, LogImpact.Low, $"{ToPrettyString(args.User)} used {ToPrettyString(uid)} which spawned {ToPrettyString(ent)}");
        }

        if (comp.Sound != null)
            _audio.PlayPvs(comp.Sound, Transform(uid).Coordinates);

        comp.Uses -= 1;

        if (comp.Uses <= 0)
        {
            args.Handled = true;
            QueueDel(uid);
        }
    }
}
