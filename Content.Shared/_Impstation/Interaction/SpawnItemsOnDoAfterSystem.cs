using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;

namespace Content.Shared._Impstation.Interaction;

public abstract partial class SharedSpawnItemsOnDoAfterSystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpawnItemsOnDoAfterComponent, ActivateInWorldEvent>(OnInteract);
    }

    private void OnInteract(Entity<SpawnItemsOnDoAfterComponent> ent, ref ActivateInWorldEvent args)
    {
        if (args.Handled)
            return;

        if (ent.Comp.Uses <= 0)
            return;

        var doargs = new DoAfterArgs(EntityManager, args.User, ent.Comp.Duration, new SpawnItemsDoAfterEvent(), ent, ent)
        {
            NeedHand = false,
            BreakOnDamage = true,
        };
        if (_doAfter.TryStartDoAfter(doargs))
            args.Handled = true;
    }
}
