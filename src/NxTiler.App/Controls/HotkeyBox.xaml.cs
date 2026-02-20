using System.Windows;
using System.Windows.Controls;

namespace NxTiler.App.Controls;

public partial class HotkeyBox : UserControl
{
    public static readonly DependencyProperty KeyProperty =
        DependencyProperty.Register(nameof(Key), typeof(int), typeof(HotkeyBox),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnKeyOrModifiersChanged));

    public static readonly DependencyProperty ModifiersProperty =
        DependencyProperty.Register(nameof(Modifiers), typeof(uint), typeof(HotkeyBox),
            new FrameworkPropertyMetadata(0u, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnKeyOrModifiersChanged));

    public static readonly DependencyProperty DisplayTextProperty =
        DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(HotkeyBox), new PropertyMetadata(string.Empty));

    public int Key
    {
        get => (int)GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    public uint Modifiers
    {
        get => (uint)GetValue(ModifiersProperty);
        set => SetValue(ModifiersProperty, value);
    }

    public string DisplayText
    {
        get => (string)GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    public HotkeyBox()
    {
        InitializeComponent();
        UpdateText();
    }
}
