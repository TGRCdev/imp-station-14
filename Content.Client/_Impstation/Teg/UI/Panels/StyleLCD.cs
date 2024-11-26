using System.Linq;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using static Robust.Client.UserInterface.StylesheetHelpers;


namespace Content.Client._Impstation.Teg.UI.Panels;

public sealed partial class StyleLCD : StyleBase
{
    public override Stylesheet Stylesheet { get; }

    private const string FontPath = "/Fonts/Tight7Segment/lcd-calculator-display-tight-7-segment.otf";

    public StyleLCD(
        IResourceCache resCache,
        Color fontColor,
        int fontSize
    ) : base(resCache)
    {
        var font = resCache.GetFont(FontPath, fontSize);

        Stylesheet = new Stylesheet(BaseRules.Concat([
            Element<Label>()
                .Class("LCD")
                .Prop(Label.StylePropertyFont, font)
                .Prop(Label.StylePropertyFontColor, fontColor),

            Element<PanelContainer>()
                .Class("LCD")
                .Prop(PanelContainer.StylePropertyPanel, new StyleBoxFlat(Color.Black))
        ]).ToList());
    }

    public StyleLCD(
        IResourceCache resCache,
        int fontSize = 30
    ) : this(resCache, Color.Red, fontSize)
    {
    }
}
