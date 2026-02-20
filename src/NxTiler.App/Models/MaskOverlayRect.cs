using CommunityToolkit.Mvvm.ComponentModel;

namespace NxTiler.App.Models;

public partial class MaskOverlayRect : ObservableObject
{
    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    [ObservableProperty]
    private double _width;

    [ObservableProperty]
    private double _height;
}
