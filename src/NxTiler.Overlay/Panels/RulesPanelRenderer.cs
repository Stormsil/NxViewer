using ImGuiNET;
using NxTiler.Domain.Rules;
using NxTiler.Domain.Settings;
using NxTiler.Overlay.State;
using System.Numerics;

namespace NxTiler.Overlay.Panels;

/// <summary>
/// ImGui window rules panel: replaces WPF rules page.
/// GlazeWM-style rules editor with condition and action editing.
/// </summary>
public sealed class RulesPanelRenderer
{
    private static readonly string[] ConditionKindNames =
    {
        "Имя процесса", "Путь EXE (regex)", "Командная строка (regex)",
        "Класс окна", "Заголовок (regex)",
    };

    private static readonly string[] ActionNames =
    {
        "Включить", "Плавающее", "Игнорировать", "Назначить группу",
    };

    public Action<WindowRulesSettings>? OnSave { get; set; }
    public Action? OnClose { get; set; }

    private List<WindowRule> _rules = new();
    private int _editingIndex = -1;
    private string _editName = string.Empty;
    private int _editPriority;
    private bool _editEnabled;
    private int _editAction;
    private string _editGroupId = string.Empty;
    private List<(int kind, string pattern, bool negated)> _editConditions = new();

    private bool _rulesLoaded;

    public void Render(OverlayState state)
    {
        if (!state.IsRulesPanelOpen)
        {
            _rulesLoaded = false;
            _editingIndex = -1;
            return;
        }

        if (!_rulesLoaded)
        {
            _rules = state.Rules.Select(r => r).ToList();
            _rulesLoaded = true;
        }

        ImGui.SetNextWindowSize(new Vector2(700, 500), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowBgAlpha(0.96f);
        var open = true;
        if (!ImGui.Begin("Правила окон##rules", ref open, ImGuiWindowFlags.NoCollapse))
        {
            ImGui.End();
            if (!open) OnClose?.Invoke();
            return;
        }

        if (!open)
        {
            ImGui.End();
            OnClose?.Invoke();
            return;
        }

        // Rules table
        if (ImGui.BeginTable("##rulesTable", 5,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingStretchProp |
                ImGuiTableFlags.ScrollY, new Vector2(0, _editingIndex >= 0 ? 160 : 360)))
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableSetupColumn("Имя", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("При.", ImGuiTableColumnFlags.WidthFixed, 40);
            ImGui.TableSetupColumn("Вкл.", ImGuiTableColumnFlags.WidthFixed, 40);
            ImGui.TableSetupColumn("Действие", ImGuiTableColumnFlags.WidthFixed, 100);
            ImGui.TableSetupColumn("##ruleOps", ImGuiTableColumnFlags.WidthFixed, 70);
            ImGui.TableHeadersRow();

            for (var i = 0; i < _rules.Count; i++)
            {
                var rule = _rules[i];
                ImGui.TableNextRow();

                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(rule.Name);

                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted(rule.Priority.ToString());

                ImGui.TableSetColumnIndex(2);
                ImGui.TextUnformatted(rule.IsEnabled ? "Да" : "Нет");

                ImGui.TableSetColumnIndex(3);
                ImGui.TextUnformatted(ActionNames.ElementAtOrDefault((int)rule.Action) ?? rule.Action.ToString());

                ImGui.TableSetColumnIndex(4);
                if (ImGui.SmallButton($"✎##{i}"))
                {
                    StartEditing(i, rule);
                }

                ImGui.SameLine();
                if (ImGui.SmallButton($"✗##{i}"))
                {
                    _rules.RemoveAt(i);
                    if (_editingIndex == i) _editingIndex = -1;
                    break;
                }
            }

            ImGui.EndTable();
        }

        if (ImGui.Button("+ Добавить правило##addRule"))
        {
            var newRule = new WindowRule(
                Guid.NewGuid().ToString(), "Новое правило", 0, true,
                Array.Empty<RuleCondition>(), RuleAction.Include);
            _rules.Add(newRule);
            StartEditing(_rules.Count - 1, newRule);
        }

        // Inline editor
        if (_editingIndex >= 0)
        {
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Text("Редактирование правила:");
            ImGui.Spacing();

            ImGui.InputText("Имя##editName", ref _editName, 256);
            ImGui.InputInt("Приоритет##editPriority", ref _editPriority);
            ImGui.Checkbox("Включено##editEnabled", ref _editEnabled);
            ImGui.Combo("Действие##editAction", ref _editAction, ActionNames, ActionNames.Length);

            if (_editAction == 3) // AssignGroup
            {
                ImGui.InputText("ID группы##editGroupId", ref _editGroupId, 64);
            }

            ImGui.Spacing();
            ImGui.Text("Условия:");

            for (var ci = 0; ci < _editConditions.Count; ci++)
            {
                var (kind, pattern, negated) = _editConditions[ci];
                ImGui.SetNextItemWidth(140);
                ImGui.Combo($"##ckind{ci}", ref kind, ConditionKindNames, ConditionKindNames.Length);
                ImGui.SameLine();
                ImGui.SetNextItemWidth(180);
                ImGui.InputText($"##cpat{ci}", ref pattern, 512);
                ImGui.SameLine();
                ImGui.Checkbox($"Инверт##cneg{ci}", ref negated);
                ImGui.SameLine();
                if (ImGui.SmallButton($"✗##cdel{ci}"))
                {
                    _editConditions.RemoveAt(ci);
                    break;
                }

                _editConditions[ci] = (kind, pattern, negated);
            }

            if (ImGui.SmallButton("+ Условие##addCond"))
            {
                _editConditions.Add((0, string.Empty, false));
            }

            ImGui.Spacing();
            if (ImGui.Button("Применить##applyRule"))
            {
                ApplyEdit();
            }

            ImGui.SameLine();
            if (ImGui.Button("Отмена##cancelRule"))
            {
                _editingIndex = -1;
            }
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        if (ImGui.Button("Сохранить##saveRules"))
        {
            var settings = new WindowRulesSettings(_rules.AsReadOnly(), true);
            OnSave?.Invoke(settings);
        }

        ImGui.SameLine();
        if (ImGui.Button("Отмена##cancelRules"))
        {
            OnClose?.Invoke();
        }

        ImGui.End();
    }

    private void StartEditing(int index, WindowRule rule)
    {
        _editingIndex = index;
        _editName = rule.Name;
        _editPriority = rule.Priority;
        _editEnabled = rule.IsEnabled;
        _editAction = (int)rule.Action;
        _editGroupId = rule.GroupId ?? string.Empty;
        _editConditions = rule.Conditions
            .Select(c => ((int)c.Kind, c.Pattern, c.IsNegated))
            .ToList();
    }

    private void ApplyEdit()
    {
        if (_editingIndex < 0 || _editingIndex >= _rules.Count) return;

        var conditions = _editConditions
            .Where(c => !string.IsNullOrWhiteSpace(c.pattern))
            .Select(c => new RuleCondition((RuleConditionKind)c.kind, c.pattern, c.negated))
            .ToList();

        var updated = _rules[_editingIndex] with
        {
            Name = _editName,
            Priority = _editPriority,
            IsEnabled = _editEnabled,
            Action = (RuleAction)_editAction,
            Conditions = conditions,
            GroupId = _editAction == 3 ? _editGroupId : null,
        };

        _rules[_editingIndex] = updated;
        _editingIndex = -1;
    }
}
