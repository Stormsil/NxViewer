# Enterprise Patterns — Advanced

This reference covers advanced enterprise-grade patterns that strengthen the design system.
These concepts are drawn from VMware's Clarity Design System and other mature enterprise
design systems, integrated with the Ant Design philosophical foundation.

---

## 1. Design Token Architecture

A mature design system operates on THREE levels of abstraction, not just raw values.

### Level 1: Global Tokens (Raw Values)

These are the atomic building blocks — every color, size, spacing value in the system.
They are named by WHAT they are, not WHERE they're used.

```
// Colors (use HSL for human readability)
--global-color-blue-700:  hsl(198, 100%, 34%)
--global-color-green-700: hsl(93, 67%, 38%)
--global-color-red-700:   hsl(9, 85%, 50%)
--global-color-gray-100:  hsl(198, 10%, 95%)   // Cool grays, not neutral
--global-color-gray-900:  hsl(198, 10%, 15%)

// Spacing
--global-space-1: 2px     // Micro
--global-space-2: 4px     // Sub-grid
--global-space-3: 8px     // Base unit
--global-space-4: 12px    // Compact
--global-space-5: 16px    // Standard
--global-space-6: 20px    // Comfortable  
--global-space-7: 24px    // Section
--global-space-8: 32px    // Large section
--global-space-9: 48px    // Page level
```

### Level 2: Semantic Aliases (Functional Tokens)

Aliases map globals to their PURPOSE. This is the critical middle layer.

```
// Status colors
--alias-status-success:       var(--global-color-green-700)
--alias-status-success-tint:  var(--global-color-green-200)   // For backgrounds
--alias-status-success-shade: var(--global-color-green-900)   // For hover
--alias-status-warning:       var(--global-color-yellow-700)
--alias-status-danger:        var(--global-color-red-700)
--alias-status-info:          var(--global-color-blue-700)

// Interaction colors
--alias-interactive-default:  var(--global-color-blue-700)
--alias-interactive-hover:    var(--global-color-blue-800)
--alias-interactive-active:   var(--global-color-blue-900)
--alias-interactive-disabled: var(--global-color-gray-400)

// Container colors  
--alias-container-background:   var(--global-color-white)
--alias-container-border:       var(--global-color-gray-300)
--alias-container-shadow-color: var(--global-color-gray-500)

// Typography colors
--alias-text-primary:   var(--global-color-gray-900)
--alias-text-secondary: var(--global-color-gray-600)
--alias-text-disabled:  var(--global-color-gray-400)
--alias-text-inverse:   var(--global-color-white)
```

### Level 3: Component Tokens

Specific to individual components. Reference aliases, not globals.

```
--button-primary-bg:    var(--alias-interactive-default)
--button-primary-hover: var(--alias-interactive-hover)
--button-primary-text:  var(--alias-text-inverse)
--table-header-bg:      var(--global-color-gray-50)
--table-border:         var(--alias-container-border)
```

### Why Three Levels Matter

1. **Theming becomes trivial** — Switch dark/light theme by remapping ~50 aliases
2. **Consistency guaranteed** — Components that use the same alias always match
3. **Maintenance scales** — Change one global, everything using it via aliases updates
4. **Accessibility preserved** — Contrast ratios are validated at the alias level

### Tint/Shade Pattern (No Transparency)

**Critical rule**: Use dedicated tint and shade color tokens instead of opacity/transparency
for hover and active states. Transparency confounds accessibility evaluation because
the resulting color depends on the background it sits on.

```
BAD:  background: rgba(22, 119, 255, 0.1)    // Result varies by background
GOOD: background: var(--alias-status-info-tint)  // Always the exact same color
```

### Cool Grays

Pure neutral grays (0% saturation) feel dull and lifeless. Add a tiny amount of color
saturation (2-5% in your brand hue direction) to construction grays:

```
FLAT:  hsl(0, 0%, 90%)     // Dead, clinical
COOL:  hsl(198, 5%, 90%)   // Alive, modern, subtle warmth
```

Use cool grays for: borders, backgrounds, dividers, shadows, secondary text.
The difference is largely invisible in isolation but creates a more vibrant feel
in the overall composition.

---

## 2. Density Model

Enterprise applications serve different user types. Power users want maximum information
density; casual users need breathing room. A density model makes this a system-level decision.

### Two Modes

| Property | Regular (Default) | Compact |
|----------|------------------|---------|
| **Use when** | Forms, content pages, first-time users | Tables, dashboards, power users |
| **Base font** | 14px | 13px |
| **Line height** | 22px (standard) | 18px (compact) |
| **Component padding** | 8–12px vertical | 4–8px vertical |
| **Row height (table)** | 48px | 36px |
| **Input height** | 36px | 28px |
| **Button height** | 32px | 28px |
| **Minimum touch target** | 44x44px (WCAG) | 32x32px (mouse only) |

### Implementation Rules

1. **Never hardcode** — All spacing within components must reference density tokens
2. **User preference** — Let users choose density in app settings
3. **Context-aware defaults** — Tables default to compact; forms to regular
4. **Accessibility gate** — Compact mode disables on touch devices where 44px
   minimum target can't be met
5. **Progressive** — Only reduce spacing, never remove information or controls

### Variable Line Heights

Instead of one fixed line-height, provide three options per typography size:

```
Body text (14px):
  Standard:  line-height 22px  — standalone text, descriptions
  Compact:   line-height 18px  — inside containers (tables, cards, alerts)
  Expanded:  line-height 28px  — marketing pages, hero text

All line heights must be divisible by 4 (for sub-grid alignment).
```

**When to use each**:
- **Standard**: Default. Standalone text, form labels, descriptions
- **Compact**: Inside bounded containers (table cells, card bodies, sidebar items,
  alert text). The container boundary provides the visual grouping that the line-height
  normally creates.
- **Expanded**: Marketing/landing pages, hero sections, single-line titles where 
  vertical rhythm defines the entire page structure

---

## 3. Notification Priority Hierarchy

Different events require different notification channels. A common mistake is using
the wrong level — either interrupting the user unnecessarily or failing to alert them
to critical issues.

### Five-Tier Notification System

Ordered from LEAST to MOST interruptive:

#### Tier 1: Snackbar (Confirmation)
- **Purpose**: Quick confirmation that an action succeeded
- **Examples**: "Saved", "Copied", "Item deleted", "Settings updated"
- **Behavior**: Bottom-center or bottom-left, auto-dismiss 3–5s, no action required
- **Design**: Single-line text, optional "Undo" link, no icon necessary
- **When**: After every successful user-initiated action

#### Tier 2: Alert (Contextual, Inline)
- **Purpose**: Low-priority information related to specific objects or page sections
- **Examples**: "This feature is in beta", "3 items need attention", form validation
- **Behavior**: Inline within content, persists until dismissed or resolved
- **Design**: Banner strip with status color, icon, text, optional dismiss
- **When**: State information the user should know about a specific context

#### Tier 3: Toast (Notification)
- **Purpose**: Medium-priority events the user may reference later
- **Examples**: "Deploy completed", "New comment on your ticket", "Approval needed"
- **Behavior**: Top-right stack, auto-dismiss 6–8s, clickable for details
- **Design**: Card with icon, title, description, timestamp, optional action button
- **When**: System events that don't require immediate action

#### Tier 4: Banner (Page-Level Critical)
- **Purpose**: High-priority issues that affect the entire application
- **Examples**: "License expiring", "Payment failed", "Maintenance in 2 hours"
- **Behavior**: Fixed at top of viewport (below header), persists until resolved
- **Design**: Full-width bar with warning/danger color, action button, dismiss option
- **When**: Issues the user MUST know about but can still use the app

#### Tier 5: Modal (Blocking Critical)
- **Purpose**: Highest priority — requires immediate user decision
- **Examples**: "Session expired — log in again", "Unsaved changes — discard?",
  "Account suspended"
- **Behavior**: Overlay blocking all interaction until resolved
- **Design**: Dialog with clear title, description, primary + secondary actions
- **When**: ONLY when the user absolutely cannot proceed without deciding

### Decision Framework

```
Is action required?
  NO → Was this triggered by the user?
        YES → Snackbar (Tier 1)
        NO → Is it about a specific section?
              YES → Alert (Tier 2)
              NO → Toast (Tier 3)
  YES → Can the user continue using the app?
        YES → Banner (Tier 4)
        NO → Modal (Tier 5)
```

---

## 4. Multi-Step Workflow Selection

Enterprise apps frequently need multi-step flows. The critical question is WHICH pattern
to use. There are three distinct components for different use cases.

### Wizard (Modal Multi-Step)
- **When**: Complex setup flows, configurations, onboarding sequences
- **Where**: Opens as a full-screen or large modal overlay
- **Navigation**: Step sidebar on the left, content on the right
- **Rules**:
  - 2–10 steps maximum (combine if > 10)
  - Steps are shown in sidebar with completed/active/pending states
  - Completed steps marked with green indicator
  - Error states shown with red indicator + icon replacement
  - "Back" / "Next" / "Finish" buttons at bottom-right (Z-pattern placement)
  - Validate on each step transition, not just at the end
  - Content should not scroll — if it does, split the step or use larger wizard

### Stepper (Inline Accordion)
- **When**: In-page sequential data entry, settings that build on each other
- **Where**: Inline within the page content
- **Navigation**: Vertical accordion — each step expands while others collapse
- **Rules**:
  - Steps are numbered with title and optional description
  - Only one step expanded at a time
  - Completed steps show summary in collapsed state
  - User can go back to edit completed steps
  - Best for forms where context from previous steps informs the next

### Timeline (Status Tracker)
- **When**: Showing progress of an ongoing process, historical events
- **Where**: Inline within page
- **Navigation**: Read-only timeline with status indicators
- **Rules**:
  - Horizontal or vertical layout
  - Five visual states: not started, current/active, processing, completed, error
  - Each step has: header (timestamp), icon (state), title, description
  - Can include contextual actions on current step
  - Loading state uses spinner icon instead of status icon

### Decision Framework

```
Is the user actively completing steps RIGHT NOW?
  YES → Are they in a separate, focused flow?
        YES → Wizard (modal, immersive)
        NO → Stepper (inline, accordion-style)
  NO → Is it a process status / history view?
        YES → Timeline (read-only progression)
```

---

## 5. Advanced Datagrid Patterns

Enterprise datagrids are the most complex UI component. These patterns go beyond
basic table display.

### Expandable Rows (Master-Detail Inline)
- Click a row to expand additional detail below it
- Use when additional information for a row doesn't need to be visible at all times
- The expanded area can show: additional columns, a custom layout, a form
- Eliminates the need for a separate detail page for quick inspection
- Only one row expanded at a time (or optionally multiple)

### Detail Pane (Side Panel)
- Click a row to open a panel on the RIGHT side of the datagrid
- Datagrid condenses to show only the primary column
- Panel displays full record details in a scrollable view
- Fully accessible for keyboard and screen reader users
- Not compatible with expandable rows — choose one pattern

### Lazy Loading Detail
- Expandable rows or detail pane can load data on-demand
- Show a spinner while loading
- Cache the loaded data to avoid re-fetching on collapse/expand cycle

### Column Management
- Allow users to show/hide columns via a column picker
- Persist user column preferences across sessions
- Disable column management while detail pane is open

### Batch Operations
- Checkbox selection for batch actions
- "Select all" selects only the current page, with an option to "Select all N items"
- Batch action toolbar appears above the table when items are selected
- Show count of selected items
- Clear selection after batch action completes

### Pagination vs. Infinite Scroll
- **Pagination** (preferred for enterprise): User maintains orientation, can bookmark pages,
  server-side friendly, consistent performance
- **Infinite scroll**: Only for feeds/timelines where total count is irrelevant
  and the user is browsing rather than searching

---

## 6. Accessibility as Architecture

Accessibility is not a checklist — it's an architectural concern that must be built into
every component from the beginning. WCAG 2.1 AA is the minimum standard.

### Core Requirements

#### Color and Contrast
- NEVER rely on color alone to convey meaning — always add text, icons, or patterns
- Body text contrast ratio ≥ 4.5:1 (AA) against background
- Large text (≥ 18px or 14px bold) contrast ratio ≥ 3:1
- Target WCAG AAA (7:1) for body text when possible
- Interactive elements need focus-visible indicators distinct from hover

#### Keyboard Navigation
- Every interactive element reachable via Tab key
- Logical tab order following visual layout (top-to-bottom, left-to-right)
- Arrow keys for navigation within composite widgets (menus, tabs, tree)
- Escape to close overlays (modals, dropdowns, popovers)
- Enter/Space to activate buttons and controls
- Focus trap inside modals — Tab cycles within the modal, not the page

#### Screen Reader Support
- Meaningful heading hierarchy (h1 → h2 → h3, no skipping levels)
- `aria-current="page"` on active navigation items
- `aria-live="polite"` for notification areas
- `aria-expanded` for collapsible sections and nav items
- Descriptive labels for all form inputs (never placeholder-only)
- Error messages linked to inputs via `aria-describedby`
- Table headers properly associated with data cells

#### Focus Management on Navigation
- When a route change loads new content, manage focus explicitly
- Focus the main heading or first meaningful content of the new view
- Announce the page change to assistive technologies
- Use a focus-on-view-init pattern for SPA navigation

#### Touch Targets
- Minimum 44x44px for touch interfaces (WCAG 2.5.5)
- Minimum 32x32px for mouse-only interfaces
- Touch targets on mobile: expand tap area beyond visible element if needed
- 8px minimum spacing between adjacent touch targets

#### Motion and Animation
- Respect `prefers-reduced-motion` media query
- Provide mechanism to disable animations
- No content should flash more than 3 times per second
- Progress indicators should not rely solely on animation

---

## 7. Internationalization and RTL Support

Enterprise software must support global audiences. Design with i18n in mind from day one.

### Layout Mirroring (RTL)
- In RTL languages (Arabic, Hebrew, Farsi), the ENTIRE layout mirrors horizontally
- Navigation moves to the right side
- Text alignment flips (left → right for body text)
- Icons that imply direction must flip (arrows, chevrons, back buttons)
- Icons that are symmetric or represent real objects do NOT flip (search, phone, email)
- Progress indicators reverse direction (right to left)

### Text Expansion
- Translated text is often 30–50% longer than English
- German, Finnish, Russian can expand up to 200% for short strings
- Design flexible containers — avoid fixed-width text containers
- Test with the longest target language string, not just English
- Button text should accommodate expansion without breaking layout

### Number and Date Formatting
- Date format varies: MM/DD/YYYY (US), DD/MM/YYYY (EU), YYYY-MM-DD (ISO)
- Number format varies: 1,000.00 (US/UK) vs 1.000,00 (DE/FR/RU)
- Currency symbol position: $100 (US) vs 100€ (EU) vs 100 ₽ (RU)
- Use locale-aware formatting libraries, never hardcode format

### Content Guidelines for i18n
- Avoid idioms, metaphors, and cultural references in UI text
- Use complete sentences — don't concatenate strings ("You have " + n + " items")
- Design for the longest reasonable translation from the start
- Icons + text > text alone (icons are more universally understood)

---

## 8. Charts Color System

Data visualization colors should be derived from the same token system as the UI.

### Chart Palette Rules

1. **Derive from globals** — Chart colors are aliases pointing to global color tokens
2. **Accessible pairs** — Every chart color must have ≥ 3:1 contrast against white
   AND against its adjacent colors in the chart
3. **Semantic first** — If data has inherent meaning (positive/negative, pass/fail),
   use status colors (green/red) before decorative ones
4. **Sequential palettes** — For ordered data, use a single hue with varying lightness
5. **Categorical palettes** — For unrelated categories, use distinct hues spaced
   evenly around the color wheel
6. **Colorblind-safe** — Test with deuteranopia, protanopia, and tritanopia simulations.
   Prefer blue-orange over red-green distinctions.
7. **Light/dark themes** — Charts need separate palettes for each theme,
   derived from the same alias system
8. **Emphasis** — Allow interactive highlighting: dim non-focused series to gray,
   bright-up the focused series

---

## 9. Collapsible Navigation Patterns

Enterprise sidebar navigation should gracefully adapt through multiple states.

### Three-State Sidebar

1. **Expanded** (default, ~240px): Full icons + text labels + group headers
2. **Collapsed** (~64px): Icons only with tooltip labels on hover
   - Items with children show a small caret indicator
   - Clicking an icon with children expands the sidebar
   - Clicking an icon without children navigates directly
   - Active item shown with highlighted icon
3. **Hidden** (0px, responsive): Below breakpoint (typically 768px), sidebar fully hidden
   - Toggle button in header reveals sidebar as overlay
   - Same design as expanded state but overlays content

### Sidebar Rules

- Double-caret button in upper corner toggles expanded ↔ collapsed
- Labels that are too long are trimmed with ellipsis (…)
- Use dividers to separate logical navigation groups
- Section headers are optional but useful for 10+ items
- Active link indicated by: colored left bar + white/highlight background
- Touch targets span the full width of the navigation bar
- `aria-expanded` attribute on collapsible groups
- On route change, focus first meaningful content element in new view

---

## 10. Stack View Pattern

For key-value data that has expandable sub-levels — more structured than a simple
descriptions list but not a full table.

### When to Use
- Configuration panels with nested properties
- Settings views where categories contain multiple key-value pairs
- Inspector panels in developer tools / admin interfaces

### Design Rules
- Two columns: label (left) and value (right)
- Clickable rows expand to reveal child key-value pairs
- Expanded sections have lighter background for visual hierarchy
- Edit button (top-right) opens editable mode in a modal
- Editing in modal prevents accidental value changes
- Editing controls: inputs, selects, checkboxes, radio buttons
- Terse labels, one line each, noun phrases, sentence-case, no trailing punctuation
