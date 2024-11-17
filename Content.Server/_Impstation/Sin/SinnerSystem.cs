using System.Collections.Immutable;
using Content.Server.Antag.Components;
using Content.Server.Body.Components;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Events;
using Content.Server.Mind;
using Content.Server.Objectives.Components;
using Content.Server.Station.Systems;
using Content.Shared.Clothing;
using Content.Shared.Damage;
using Content.Shared.Mind;
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
    [Dependency] private readonly DamageableSystem _damageable = default!;

    private const string MapPath = "Maps/_Impstation/Nonstations/hell.yml";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundStartingEvent>(OnRoundStart);
        SubscribeLocalEvent<SinnerComponent, MobStateChangedEvent>(OnMobStateChanged);
        SubscribeLocalEvent<SinnerComponent, EntityTerminatingEvent>(OnEntityTerminating, before: [typeof(MindSystem)]);
        SubscribeLocalEvent<SinnerComponent, BeforeGibbedEvent>(OnGibbed);
        SubscribeLocalEvent<InHellComponent, DamageChangedEvent>(OnDamageChange);
    }

    private void OnRoundStart(RoundStartingEvent ev)
    {
        _mapSystem.CreateMap(out var mapId);

        var options = new MapLoadOptions { LoadMap = true };
        if (_mapLoader.TryLoad(mapId, MapPath, out _, options))
            _mapSystem.SetPaused(mapId, false);
    }

    private void OnDamageChange(EntityUid uid, InHellComponent comp, ref DamageChangedEvent args)
    {
        args.Damageable.Damage.ClampMax(0);
    }

    public void SendToHell(EntityUid uid)
    {
        _mind.TryGetMind(uid, out var mindId, out var mind);
        if (mind == null)
        {
            Log.Warning("Sending an entity without a mind to hell");
        }

        Log.Debug($"Sending {uid} to hell!!!!!!!!");

        var spawnPoints = EntityManager.GetAllComponents(typeof(HellSpawnComponent)).ToImmutableList();
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
        EnsureComp<InHellComponent>(mobUid);

        if (mind != null)
        {
            _mind.TransferTo(mindId, mobUid, mind: mind);
            mind.PreventGhosting = true;
        }
        Log.Debug($"Created sinner entity {mobUid}");
    }

    private void OnGibbed(EntityUid uid, SinnerComponent comp, BeforeGibbedEvent args)
    {
        SendToHell(uid);
        RemCompDeferred<SinnerComponent>(uid);
    }

    private void OnEntityTerminating(EntityUid uid, SinnerComponent comp, EntityTerminatingEvent args)
    {
        SendToHell(uid);
        RemCompDeferred<SinnerComponent>(uid);
    }

    private void OnMobStateChanged(EntityUid uid, SinnerComponent comp, MobStateChangedEvent args)
    {
        Log.Debug("SinnerSystem.OnMobStateChanged entered");
        if (args.NewMobState != MobState.Dead)
            return;

        SendToHell(uid);
        RemCompDeferred<SinnerComponent>(uid);
    }
}
