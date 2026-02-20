using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Hardcodet.Wpf.TaskbarNotification;
using NxTiler.Application.Abstractions;
using NxTiler.Application.Messaging;

namespace NxTiler.Infrastructure.Tray;

public sealed class TrayService(IMessenger messenger) : ITrayService
{
    private TaskbarIcon? _tray;
    private MenuItem? _autoItem;
    private readonly IMessenger _messenger = messenger;

    public void Initialize(bool autoArrangeEnabled)
    {
        if (_tray is not null)
        {
            return;
        }

        _tray = new TaskbarIcon
        {
            ToolTipText = "NxTiler",
            Icon = ResolveIcon(),
            Visibility = Visibility.Visible,
        };

        _tray.TrayMouseDoubleClick += (_, _) => _messenger.Send(new TrayShowRequestedMessage());

        var contextMenu = new ContextMenu();

        var showItem = new MenuItem { Header = "Show" };
        showItem.Click += (_, _) => _messenger.Send(new TrayShowRequestedMessage());

        var arrangeItem = new MenuItem { Header = "Arrange Now" };
        arrangeItem.Click += (_, _) => _messenger.Send(new TrayArrangeRequestedMessage());

        _autoItem = new MenuItem { Header = "Auto-Arrange", IsCheckable = true, IsChecked = autoArrangeEnabled };
        _autoItem.Click += (_, _) => _messenger.Send(new TrayAutoArrangeToggledMessage(_autoItem.IsChecked));

        var exitItem = new MenuItem { Header = "Exit" };
        exitItem.Click += (_, _) => _messenger.Send(new TrayExitRequestedMessage());

        contextMenu.Items.Add(showItem);
        contextMenu.Items.Add(arrangeItem);
        contextMenu.Items.Add(new Separator());
        contextMenu.Items.Add(_autoItem);
        contextMenu.Items.Add(new Separator());
        contextMenu.Items.Add(exitItem);

        _tray.ContextMenu = contextMenu;
    }

    public void ShowBalloon(string title, string message)
    {
        _tray?.ShowBalloonTip(title, message, BalloonIcon.Info);
    }

    public void SetAutoArrangeState(bool enabled)
    {
        if (_autoItem is not null)
        {
            _autoItem.IsChecked = enabled;
        }
    }

    public void Dispose()
    {
        _tray?.Dispose();
        _tray = null;
    }

    private static Icon ResolveIcon()
    {
        var iconPath = Path.Combine(AppContext.BaseDirectory, "app.ico");
        if (File.Exists(iconPath))
        {
            return new Icon(iconPath);
        }

        return SystemIcons.Application;
    }
}
