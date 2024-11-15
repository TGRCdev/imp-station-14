using System.Collections.Immutable;
using Content.Server._Goobstation.Ghostbar.Components;
using Content.Server.Antag.Components;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Events;
using Content.Server.Maps;
using Content.Server.Objectives.Components;
using Content.Server.Station.Systems;
using Content.Shared.Clothing;
using Content.Shared.Humanoid;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Robust.Server.GameObjects;
using Robust.Server.Maps;
using Robust.Shared.Random;

namespace Content.Server._Impstation.Sin;

public sealed partial class SinnerSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly StationSpawningSystem _spawningSystem = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly LoadoutSystem _loadout = default!;
    [Dependency] private readonly GameTicker _ticker = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;

    [ValidatePrototypeId<GameMapPrototype>]
    private const string MapPath = "Maps/_Goobstation/Nonstations/ghostbar.yml";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundStartingEvent>(OnRoundStart);
        SubscribeLocalEvent<SinnerComponent, MobStateChangedEvent>(OnMobStateChanged);
    }

    private void OnRoundStart(RoundStartingEvent ev)
    {
        _mapSystem.CreateMap(out var mapId);

        var options = new MapLoadOptions { LoadMap = true };
        if (_mapLoader.TryLoad(mapId, MapPath, out _, options))
            _mapSystem.SetPaused(mapId, false);
    }

    private void OnMobStateChanged(EntityUid uid, SinnerComponent comp, MobStateChangedEvent args)
    {
        Log.Debug("SinnerSystem.OnMobStateChanged entered");
        if (args.NewMobState != MobState.Dead)
            return;

        if (!TryComp<MindContainerComponent>(uid, out var mindContainer))
            return;

        if (!mindContainer.HasMind)
            return;

        Log.Debug($"Sending {mindContainer.Mind} to hell!!!!!!!!");

        var mindEnt = mindContainer.Mind.Value;

        var spawnPoints = EntityManager.GetAllComponents(typeof(GhostBarSpawnComponent)).ToImmutableList();
        if (spawnPoints.IsEmpty)
        {
            Log.Warning("Hell has no spawners! Failed to send sinner to hell.");
            return;
        }
        var newSpawn = _random.Pick(spawnPoints);

        var profile = _loadout.GetProfile(uid);

        var mobUid = _spawningSystem.SpawnPlayerMob(Transform(newSpawn.Uid).Coordinates, null, profile, null);
        EnsureComp<AntagImmuneComponent>(mobUid);
        EnsureComp<TargetImmuneComponent>(mobUid);

        _mind.TransferTo(mindEnt, mobUid);
        Log.Debug($"Created sinner entity {mobUid}");
    }
}
