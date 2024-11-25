using Content.Shared._Impstation.Teg;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Impstation.Teg.UI;

[UsedImplicitly]
public sealed class TegBoundUserInterface : BoundUserInterface
{
    private TegWindow? _window;

    public TegBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<TegWindow>();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not TegUIState tegState)
            return;

        _window?.UpdateState(tegState);
    }
}
