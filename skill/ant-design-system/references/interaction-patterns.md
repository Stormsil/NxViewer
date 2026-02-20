# Interaction Patterns — The 10 Principles in Practical Depth

This document expands each interaction principle with concrete implementation patterns
and decision trees for real-world scenarios.

---

## Principle 5: Make it Direct — In-Depth

### In-Page Editing Patterns

**Pattern A: Click-to-Edit (Invisible Edit)**
```
State 1: Browsing — text displayed normally, no edit affordance visible
State 2: Hover — background highlights subtly, cursor changes, tooltip says "Click to edit"
State 3: Active — text becomes input field + OK/Cancel buttons, cursor in input
State 4: Save — returns to State 1 with updated value, brief green flash confirms save
```
- Use when: Readability matters more than editability.
- The user doesn't know it's editable until they hover.

**Pattern B: Explicit Edit Link**
```
State 1: Browsing — text displayed with "Edit" icon/link next to it
State 2: Click Edit — text becomes input field + OK/Cancel
State 3: Save — returns to State 1 with updated value
```
- Use when: Readability AND discoverability of editing are both important.

**Pattern C: Multi-Field Inline Edit**
```
State 1: View Mode — entire section shows data as text
State 2: Toggle Edit Mode — all fields become editable simultaneously
State 3: Save All — validate and save, return to View Mode
```
- Use when: Multiple related fields need to be edited together.
- IMPORTANT: Use transition animation when switching between view/edit modes to reduce 
  visual jarring. The sudden appearance of many input fields is disorienting without animation.

### Drag and Drop Patterns

**Reorder**:
```
State 1: Hover item — draggable icon (⋮⋮) appears
State 2: Start drag — item lifts (shadow increases), original position shows placeholder
State 3: Drag over — drop target highlights, items shift to make space
State 4: Drop — item settles into new position with animation
```

**Move between containers**:
```
State 1: Drag from Container A
State 2: Hover over Container B — B highlights as valid drop zone
State 3: Drop in B — item animates from A to B
```

Rules:
- Always show a drag handle affordance (don't make the entire row draggable without indication).
- Show clear drop target feedback.
- Support undo after drag (Ctrl+Z or undo toast).

---

## Principle 6: Stay on the Page — In-Depth

### Overlay Strategy

| Sub-task Complexity | Component | Behavior |
|--------------------|-----------|----------|
| Simple confirmation | Popconfirm | Small popover with Yes/No |
| Single field edit | Inline edit / Popover | Edit in place |
| Short form (< 5 fields) | Modal | Center overlay |
| Medium form (5-15 fields) | Drawer | Side panel (right) |
| Complex task | Drawer (large) or New page | Full editing experience |

### Inlay Strategy (Inline Expansion)

**Expandable Table Rows**:
- Click a row → a detail panel expands below the row.
- Shows additional fields, mini-tables, or charts.
- Collapse by clicking again or clicking another row.

**Inline Panel**:
- Click "Details" → a panel slides open within the page.
- Similar to a Drawer but doesn't overlay — it pushes content aside.

**Accordion Sections**:
- Click section header → content expands/collapses.
- Good for FAQ, settings groups, nested information.

### Virtual Pages (Tabs as Pages)

Use Tabs to simulate multi-page navigation within a single actual page.
Each tab holds substantial content but shares the same URL/context.

Benefits:
- No page reload.
- Shared context (header, breadcrumb, sidebar remain stable).
- Tab content can be lazy-loaded.

### Process Flows (Steps Component)

Multi-step workflows on a single page:
```
[Step 1: Basic Info] → [Step 2: Configuration] → [Step 3: Review] → [Step 4: Complete]
```
- Show all steps in the Steps component at the top.
- Only one step's content visible at a time.
- Previous/Next navigation.
- User stays on the same page throughout.

---

## Principle 7: Keep it Lightweight — In-Depth

### Tool Visibility Strategy

**Always Visible**: Primary actions that users need constantly.
- The main "Create" button on a list page.
- Navigation items.
- Search bar.

**Hover Reveal**: Secondary actions that need to be accessible but not distracting.
- Table row actions (Edit, Delete) appear on row hover.
- Card action buttons appear on card hover.
- Toolbar options for selected items.

**Toggle Reveal**: Infrequent actions behind a mode switch.
- "Edit Mode" toggle reveals editing controls.
- "Advanced" toggle reveals power-user options.
- "Developer Mode" in settings.

### Click Target Guidelines (Fitts's Law)

| Element | Minimum Clickable Size | Padding Strategy |
|---------|----------------------|------------------|
| Button | 32px height | 8px vertical, 16px horizontal padding |
| Icon button | 32x32px total | 8px padding around icon |
| Table row action | Full row height | Row is the clickable area |
| Hyperlink text | Extend clickable area | Padding or parent cell clickable |
| Checkbox/Radio | 16x16px visual + 32x32px target | Invisible padding |
| Menu item | 40px height minimum | Full-width clickable area |

**Key rule**: The CLICKABLE area should always be larger than the VISIBLE element.
Users clicking "near" an element should still hit it.

---

## Principle 8: Provide an Invitation — In-Depth

### Static Invitation Patterns

**Empty State Invitation**:
```
[Illustration of the empty concept]
[Title: "No Projects Yet"]
[Description: "Create your first project to get started."]
[CTA Button: "Create Project" (Primary)]
```

**Affordance Cues**:
- Drag handles (⋮⋮) on reorderable items.
- Resize cursor on resizable panel edges.
- Underline and color on hoverable links.
- Dashed border on drop zones.
- "+" icon on add-capable areas.

**Annotation/Help**:
- First-time user tooltips (coach marks) pointing to key features.
- Subtle pulsing indicator on new features.
- "What's New" panel on first login after an update.

### Dynamic Invitation Patterns

**Hover Invitations**:
- Table row: Background highlights + action buttons appear.
- Editable field: Background changes to indicate "this is clickable."
- Card: Subtle shadow elevation increase.

**Cursor Invitations**:
- Pointer (hand) for clickable elements.
- Text cursor for editable text.
- Grab cursor for draggable elements.
- Resize cursor for resizable edges.

**Contextual Invitations**:
- Drag an item near a valid drop zone → zone highlights with dashed border.
- Select items in a table → batch action bar slides in.
- Type in a search field → auto-complete dropdown appears.

---

## Principle 9: Use Transition — In-Depth

### Transition Categories

**Adding (Entering)**:
- New element slides/fades IN.
- Explain to the user where it came from and how to interact with it.
- Example: New list item slides down from the "Add" button's position.

**Receding (Exiting)**:
- Element slides/fades OUT.
- Explain where it went and confirm it's gone.
- Example: Deleted item fades out and row collapses.

**Reorganizing**:
- Elements smoothly reposition.
- Example: After deletion, remaining items animate to close the gap.

### Transition Choreography

**Page Load**:
1. Shell (sidebar, header) loads immediately or is persistent.
2. Content area shows skeleton.
3. Content fades in section by section (top to bottom, with slight stagger).
4. Avoid: Everything popping in at once. Avoid: Long delays before anything appears.

**Modal Open/Close**:
1. Open: Backdrop fades in (150ms) + Modal scales from 95% to 100% with fade (200ms).
2. Close: Modal scales to 95% with fade out (150ms) + Backdrop fades out (100ms).
3. Close animation is faster than open animation.

**Drawer Open/Close**:
1. Open: Backdrop fades in + Drawer slides from right edge (250ms, ease-out).
2. Close: Drawer slides out (200ms, ease-in) + Backdrop fades.

**List Operations**:
1. Add item: New row slides down from insertion point, pushing others down smoothly.
2. Remove item: Row fades out and collapses (200ms), siblings slide up to close gap.
3. Reorder: Dragged item floats, other items shift positions smoothly.

### Performance Consideration

- Only animate `transform` and `opacity` — these use GPU compositing.
- Avoid animating `width`, `height`, `top`, `left` — these trigger layout recalculation.
- Use `will-change: transform` for elements that will animate.
- If performance is an issue, reduce transition duration or remove animation entirely.
  A snappy, responsive UI without animation > a janky UI with animation.

---

## Principle 10: React Immediately — In-Depth

### Feedback Speed Requirements

| Action | Expected Feedback Time |
|--------|----------------------|
| Button click | < 50ms — visual button press state |
| Form field input | Immediate — characters appear |
| Hover state | < 100ms — visual change |
| Validation | < 300ms after blur/pause — error/success message |
| Search results | < 500ms — results or "searching..." indicator |
| Data save | < 200ms — optimistic update OR loading indicator |
| Page navigation | < 300ms — new content or loading state |

### Optimistic Updates

When feasible, update the UI BEFORE the server responds:
1. User clicks "Like" → Heart fills immediately.
2. Server request fires in background.
3. If server confirms → nothing changes (already shown).
4. If server fails → revert the UI + show error message.

This makes the UI feel instant. Use for:
- Toggles (like, favorite, archive)
- Reordering operations
- Status updates

DON'T use for:
- Destructive operations (delete)
- Financial transactions
- Operations where rollback is complex

### Validation Patterns

**Real-time validation** (as user types):
- Password strength meter
- Username availability check (with debounce)
- Format validation (email, phone, URL)

**On-blur validation** (when field loses focus):
- Required field check
- Format validation
- Cross-field validation

**On-submit validation** (last resort):
- Server-side validation
- Complex business rule validation
- Cross-form validation

**Rule**: Validate as early as possible without being annoying.
Don't show "This field is required" while the user is still on the first field.
Wait for blur or submit. But DO show format errors in real-time (e.g., "Invalid email format").

### Auto-Complete & Live Suggest

**Auto-Complete** (form field):
- User types → after 300ms debounce → dropdown shows matching options.
- Options categorized if multiple types exist.
- Show recent/frequent options at the top.
- Keyboard navigation (↑↓ to select, Enter to confirm).

**Live Search Suggest** (search bar):
- User types → suggestions appear below.
- Categories: recent searches, popular searches, matching results.
- Highlight the matching text portion in results.
- "See all results" link at the bottom.

### Live Preview

Show the result of the user's input before they submit:
- Markdown editor: live rendered preview alongside the editor.
- Color picker: live preview of the color applied to the target element.
- Template selection: show the template applied to sample content.
- Profile editor: show how the profile will look to others.
