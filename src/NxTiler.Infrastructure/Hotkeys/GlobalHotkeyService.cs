using System.Collections.ObjectModel;
using System.Windows.Interop;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Enums;

namespace NxTiler.Infrastructure.Hotkeys;

public sealed partial class GlobalHotkeyService(ILogger<GlobalHotkeyService> logger, IMessenger messenger) : IHotkeyService
{
    private const int WmHotkey = 0x0312;

    private readonly Dictionary<int, HotkeyAction> _idToAction = new();
    private readonly Collection<int> _registeredIds = [];

    private HwndSource? _source;
    private readonly IMessenger _messenger = messenger;
    private bool _disposed;
}
