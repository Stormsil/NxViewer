# Component Patterns — Buttons, Navigation, Data Entry, Data Display, Feedback

This document provides design guidelines for the most critical component categories.
These patterns define WHEN and HOW to use each component type.

---

## Button Design

### Design Principle
Buttons are the primary way users take action. Their design must communicate importance,
availability, and consequence clearly.

### Button Types & Emphasis Hierarchy

| Type | Visual | Usage | When |
|------|--------|-------|------|
| Primary | Filled with brand color | THE main action on the page | Only ONE per visual area |
| Default | Outlined, neutral border | Secondary actions | Most buttons |
| Dashed | Dashed border | Add/create actions in context | "Add Field", "Add Row" |
| Text | No border, no fill | Tertiary actions, links | Table row actions, breadcrumbs |
| Link | Blue text, underlined on hover | Navigation actions | Navigate to another page |
| Danger | Red fill or red outline | Destructive actions | Delete, Remove, Revoke |

### Emphasis Rules

1. **One primary button per visual area** (per card, per modal, per form section).
   Multiple primary buttons compete for attention and create confusion.
2. **If ALL actions are equally important**: Use Default buttons for all. Don't pick a false primary.
3. **If choices are Accept/Reject (user decision)**: Use Default for both.
   The UI should NOT influence user decisions by making one more prominent.
4. **Destructive actions**: Use Danger type. Consider Popconfirm before execution.

### Button Placement

**How to decide where buttons go**:
1. Follow the **user's natural reading flow** — buttons at the end of the content they act upon.
2. **Navigation flow**: A "Back" button goes on the LEFT (implies going to previous step).
   A "Next" button goes on the RIGHT (implies progress).
3. **Page-level actions**: Top-right of the page header (for "Create", "Export", etc.).
4. **Form actions**: Bottom of the form, aligned with the form fields.
5. **Modal actions**: Bottom-right of the modal (standard) or bottom centered.
6. **Table row actions**: Right-most column of the table.

**When to use a footer bar for buttons**:
- When the content is scrollable and the action buttons would scroll off-screen.
- Fix the footer bar to the bottom so buttons are always accessible.
- Show clear visual separation (border-top or shadow) between content and footer.

### Button Ordering

Standard ordering (left to right):
```
[Secondary/Cancel (Default)] [space] [Primary/Submit (Primary)]
```

For modal dialogs:
```
[Cancel (Default)] [space] [OK/Confirm (Primary)]
```

For multiple actions:
```
[Cancel] [Save Draft] [Submit (Primary)]
```

### Button Groups

- **Related actions on the same object**: Group them together with small spacing.
- **Don't use zero-gap button groups** for actions (confuses with Toggle Button).
- **Too many buttons?** Group into: visible primary actions + "More" dropdown for rest.
- **Flat display**: Separate groups using space; use dividers within groups if needed.

### Button Labels

Labels should describe the action result, not the UI mechanism:
- ✅ "Save Changes" — describes result
- ❌ "Click Here" — describes mechanism
- ✅ "Delete Project" — specific result
- ❌ "OK" — vague (acceptable only in generic confirmation dialogs)

---

## Navigation

### Principle
Navigation tells users where they are, where they can go, and how to get there.

### Menu Navigation

**Top Navigation**:
- Use when: < 5 top-level sections. Content area needs maximum width.
- Structure: Horizontal menu in the header bar. Logo left, menu center or left-aligned, user actions right.
- Supports 1 level of dropdown sub-menus.

**Side Navigation**:
- Use when: 5–20 sections, possibly with deep hierarchy.
- Structure: Vertical menu in left sidebar. Supports multi-level nesting.
- Collapsible: Full sidebar (240px with text) ↔ Collapsed (80px with icons only).
- Active item should be clearly highlighted (brand color background or indicator).

### Breadcrumb
- Use when: Navigation hierarchy is > 2 levels deep.
- Shows the path from root to current page.
- Each level (except current) is a clickable link.
- Current page is non-clickable, displayed as plain text.
- Place above the page title.

### Tabs

**Tab Types**:
| Type | Usage |
|------|-------|
| Basic Line Tabs | Default. Switches between content views at the same hierarchy level. |
| Card Tabs | Visually heavier. Good for top-level content switches. |
| Pill/Button Tabs | Compact. Good for filters or view toggles. |
| Vertical Tabs | When there are many tabs (> 8) or labels are long. |

**Rules**:
- Tabs switch content, they DON'T navigate to new pages.
- Keep tab labels short (1–3 words).
- Don't use more than 8 horizontal tabs. Beyond that, use vertical tabs or a dropdown.
- The default selected tab should be the most commonly used view.

### Steps (Stepper)

**Horizontal Steps**: For linear workflows with 3–7 steps.
**Vertical Steps**: For detailed step descriptions or many steps.

Rules:
- Show completed steps, current step, and upcoming steps.
- Allow clicking on completed steps to go back.
- Show step status: completed (green check), current (brand color), upcoming (gray).
- Use error status (red) to mark steps that need correction.

### Pagination

**Types**:
| Type | Usage |
|------|-------|
| Basic | Page numbers with prev/next. Default for most tables. |
| Mini | Compact version for space-constrained areas. |
| Simple | Just prev/next, no page numbers. For infinite-scroll-like browsing. |

**Rules**:
- Show total item count alongside pagination.
- Position: Bottom-right of the data area.
- Default page size: 10 or 20. Offer page size selector for large datasets.
- Always highlight the current page number.

---

## Data Entry

### Text Input

**Input (single line)**:
- Standard text entry. Width should match expected content length.
- Provide placeholder hint text for format guidance.
- For inputs with a prefix/suffix: currency symbol, URL prefix, unit label.

**Textarea (multi-line)**:
- For content > 1 sentence. Comments, descriptions, notes.
- Show character count if there's a limit.
- Allow resize (vertical only, typically).

**Search**:
- Prominent search bar at the top of search-oriented pages.
- Support: auto-complete, recent searches, search suggestions.
- Clear button (X) inside the input when text is entered.

### Tips and Help for Inputs

1. **Placeholder text**: Appears inside the empty input. For format hints only, NOT as labels.
2. **Help text**: Below the input. For constraints, explanations, examples.
3. **Tooltip**: Next to the label with a `?` icon. For additional context that's not always needed.
4. **Inline validation message**: Below the input, in red for errors, green for success.

### Selection Controls

**Radio Button**:
- Select ONE from few options (2–5). All options visible.
- Use when comparison between options is important.
- MUST have > 1 option. Usually ≤ 5.

**Checkbox**:
- Select MULTIPLE from options.
- A single checkbox = boolean toggle (with submit).
- Group checkboxes vertically for readability when > 3.

**Switch**:
- Binary toggle with IMMEDIATE effect (no submit needed).
- Labels should be clear states: "Enable/Disable", "Allow/Block".
- Different from checkbox: Switch acts immediately. Checkbox waits for form submission.

**Select (Dropdown)**:
- Choose one (or multiple) from MANY options (≥ 5).
- Supports search/filter within options for long lists.
- Use when screen space is limited (options are hidden until clicked).

**Transfer**:
- Move items between two lists (available → selected).
- Use for complex many-to-many selection with search.

**Slider**:
- Select a value from a continuous range.
- Show min/max labels. Show current value.
- Good for: volume, brightness, price range, quantity.

**DatePicker / TimePicker**:
- Standard date/time selection.
- Support range selection (start date → end date).
- Show common shortcuts: "Today", "This Week", "Last 30 Days".

### Upload

| Pattern | Usage |
|---------|-------|
| Click Upload | Single file, no preview needed. Shows filename after upload. |
| Thumbnail Upload | Images. Shows thumbnail previews in a grid. Upload button disappears at limit. |
| Drag & Drop Upload | User-friendly for any files. Drag zone with clear invitation text. |

**Rules**:
- Always specify allowed file types and max size: "PDF, ZIP, up to 5MB."
- Show upload progress bar for large files.
- Allow removing/replacing uploaded files.
- Show clear error message if upload fails (file too large, wrong type).

---

## Data Display

### Table

**The most important data display component in enterprise applications.**

Design rules:
1. **Column priority**: Most important columns left, actions far right.
2. **Alignment**: Text left-aligned, numbers right-aligned, status center-aligned.
3. **Sorting**: Sortable columns show sort indicator in header. Click to toggle asc/desc.
4. **Filtering**: Column-level filters in header dropdowns or a filter row.
5. **Fixed columns**: Pin important columns (name, status) and the action column when table scrolls.
6. **Row hover**: Subtle background change on hover.
7. **Row actions**: Show on hover or always-visible based on importance.
8. **Expandable rows**: For nested data, use expandable rows to show sub-details.
9. **Empty state**: When table has no data, show centered empty illustration + message.
10. **Loading**: Show skeleton rows or spinner overlay while data loads.

### Card

- Use for: Content preview, visual browsing, grid layouts.
- Structure: Optional cover image → Title → Description → Actions (in footer).
- Consistent card sizing within a grid.
- Hover effect: Subtle shadow elevation or border color change.

### Descriptions

- Use for: Key-value pair display (detail pages, summaries).
- Bordered or borderless style.
- Horizontal layout (label: value side by side) or vertical (label above value).
- 1–3 columns depending on available width.

### Collapse / Accordion

- Use for: Progressive disclosure. Show/hide sections.
- Only one section open at a time (accordion) or multiple (collapse).
- Indicate expanded/collapsed state with arrow icon.

### Timeline

- Use for: Chronological events — activity logs, order history, process tracking.
- Most recent event at the top (reverse chronological).
- Color-code events: green (success), red (error), blue (info), gray (routine).
- Show timestamp + description for each event.

### Tree

- Use for: Hierarchical data (file systems, org charts, category trees).
- Support expand/collapse, search, checkbox selection.
- Show levels with indentation.
- Lazy-load deep nodes for performance.

---

## Feedback Components

### When to Use What

| Scenario | Component | Behavior |
|----------|-----------|----------|
| Lightweight success/info after action | **Message** | Top-center toast, auto-dismiss (3s) |
| Important notification with detail | **Notification** | Top-right card, manual or auto-dismiss |
| Inline page-level warning/info | **Alert** | Banner at top of content area, persistent |
| Requires user decision | **Modal** | Center overlay, blocks interaction |
| Quick confirmation for simple action | **Popconfirm** | Popover near the trigger element |
| Long-running operation | **Progress** | Bar or circle, shows percentage |
| Loading data | **Spin** / **Skeleton** | Skeleton for layout, spin for actions |
| Completed multi-step process | **Result** | Full-page result with next-step actions |
| Contextual explanation | **Tooltip** | On hover, simple text |
| Contextual explanation with actions | **Popover** | On hover/click, card with content |

### Alert

- For persistent, page-level messages.
- Types: success, info, warning, error.
- Can include description text and close button.
- Place at the top of the relevant content area.
- Close button optional — hide when message is no longer relevant.

### Message

- For temporary, global feedback after actions.
- Shows at top-center of viewport.
- Auto-disappears after 3 seconds (configurable).
- Types: success, error, warning, info, loading.
- Stacks if multiple messages appear.

### Notification

- For important, system-pushed information.
- Shows in top-right corner.
- Includes icon + title + description (optionally + actions).
- Auto-dismiss optional (set duration or require manual close).
- Use for: new notifications, background task completion, external events.

### Modal Dialog

- For operations needing user focus and decision.
- Blocks page interaction (overlay backdrop).
- Types:
  - **Confirmation Modal**: "Are you sure you want to delete?" [Cancel] [Delete]
  - **Information Modal**: Display important info that must be acknowledged.
  - **Form Modal**: Simple form that doesn't warrant a new page.
- Rules:
  - Keep modal content concise. If it's too complex, use a Drawer or dedicated page.
  - Always provide Cancel/Close.
  - Primary action button should match the modal's purpose ("Delete" for delete confirmation).

### Drawer

- For detailed sub-tasks without leaving the page.
- Slides in from right (default) or other edges.
- Appropriate for: editing table rows, viewing entity details, configuration panels.
- Can be nested (Drawer opens another Drawer).
- Width: 256px (small), 378px (medium), 736px (large). Or responsive percentage.

### Loading States

1. **Spin**: For action buttons and small areas. Shows the UI is processing.
2. **Skeleton**: For content areas. Shows the SHAPE of content that's loading.
   Preferred for initial page loads and list/card areas.
3. **Progress Bar**: For quantifiable progress (file upload, data processing).
   Show percentage and estimated time remaining.
4. **Rule**: If an operation takes > 2 seconds, show loading state. < 300ms, no indicator needed.
   300ms–2s, show a simple spinner. > 10s, show progress with explanation.

### Notification Priority Decision Framework

Different events require different notification channels. Using the wrong level either
interrupts the user unnecessarily or fails to alert them to critical issues.

```
Is action required?
  NO → Was this triggered by the user?
        YES → Message/Snackbar (auto-dismiss, bottom or top-center)
        NO → Is it about a specific section?
              YES → Alert (inline, persistent)
              NO → Notification/Toast (top-right, auto-dismiss 6–8s)
  YES → Can the user continue using the app?
        YES → Banner (full-width top bar, persistent until resolved)
        NO → Modal (overlay, blocks all interaction)
```

**Long-Running Operations**: Display a progress indicator with these elements:
- Progress bar (required — indeterminate if no percentage available)
- Progress counter (only if determinate progress is available)
- Description text for status updates
- Dismiss button (always required)
- Cancel button (if the operation is cancellable)

---

## Advanced Datagrid Patterns

For enterprise data-heavy applications, the basic Table component often needs enhancement.
See `enterprise-patterns-advanced.md` for full details. Summary of key patterns:

### Expandable Rows (Master-Detail Inline)
- Click to expand additional detail below a row
- Use for: additional columns, custom layouts, sub-forms
- Only one row expanded at a time (recommended)
- Eliminates need for separate detail page for quick inspection

### Detail Pane (Side Panel)
- Click row to open panel on RIGHT side of datagrid
- Datagrid condenses to primary column only
- Panel shows full record in scrollable view
- NOT compatible with expandable rows — choose one

### Batch Operations
- Checkbox selection for batch actions
- "Select all" selects current page only, with option to "Select all N items"
- Action toolbar appears above table when items selected
- Show selection count; clear after batch action completes

### Column Management
- Show/hide columns via column picker
- Persist user column preferences across sessions
- Disable column management while detail pane is open
