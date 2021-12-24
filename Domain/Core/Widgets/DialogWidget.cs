using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Exceptions;
using Pastel;

namespace Domain.Core.Widgets;

public sealed class DialogWidget : Widget
{
    private Action OnCancel { get; set; }
    private Action OnOk { get; set; }

    public DialogWidget(string text, Action onCancel, Action onOk)
    {
        _textForegroundColor = ColorScheme!.TextForeground;
        _horizontalSymbol = "─".Pastel(ColorScheme.WidgetBorderForeground);
        _verticalSymbol = "│".Pastel(ColorScheme.WidgetBorderForeground);
        _topLeftCorner = "╭".Pastel(ColorScheme.WidgetBorderForeground);
        _topRightCorner = "╮".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomLeftCorner = "╰".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomRightCorner = "╯".Pastel(ColorScheme.WidgetBorderForeground);
        _activeButtonIndex = 0;

        OnCancel = onCancel;
        OnOk = onOk;

        UpdateWidth();
        UpdateHeight();

        Text = text;
        Items = new();

        Refresh();
    }

    protected override Dictionary<string, WidgetAction> ButtonsMap { get; set; } = new(new[]
    {
        new KeyValuePair<string, WidgetAction>("OK", WidgetAction.OK),
        new KeyValuePair<string, WidgetAction>("CANCEL", WidgetAction.CANCEL),
    });

    protected override void HandleSelectedOption()
    {
        string activeButton = ButtonsMap.Keys.ToArray()[_activeButtonIndex];

        if (activeButton == "OK")
            OnOk();
        else if (activeButton == "CANCEL")
            OnCancel();
    }

    public override IEnumerator<string> GetEnumerator()
        => new DialogWidgetEnumerator(Lines);
}

internal class DialogWidgetEnumerator : WidgetEnumerator
{
    public DialogWidgetEnumerator(IEnumerable<string> lines) : base(lines) { }
}