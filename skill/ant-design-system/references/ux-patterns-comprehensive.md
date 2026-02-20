# UX Patterns — Comprehensive (Carbon + Clarity + Ant Design Synthesis)

This reference synthesizes enterprise UX patterns from IBM's Carbon Design System,
VMware's Clarity Design System, and Ant Design. It covers patterns that are either
missing from or less detailed in the core Ant Design documentation. Each pattern
represents the best-of-breed approach drawn from all three systems.

---

## 1. Empty States

Empty states are among the most overlooked yet most impactful moments in an application.
They are the first thing new users see and the fallback when data is unavailable.

### Types of Empty States

#### First-Use Empty State
The user has just arrived and no data exists yet.
- **Goal**: Educate and motivate the user to create their first item
- **Anatomy**: Illustration (optional) + Title + Description + Primary CTA
- **Title**: Explain what will appear here ("No projects yet")
- **Description**: Brief benefit ("Create your first project to start tracking tasks")
- **CTA**: Single primary button ("Create Project") — THIS is the key element
- **Anti-pattern**: Never show a blank page with just a table header and no rows

#### No-Results Empty State
A search or filter returned zero matches.
- **Goal**: Help the user recover
- **Title**: "No results found"
- **Description**: Suggest adjustments ("Try adjusting your search or filters")
- **Actions**: "Clear filters" link, suggest related terms, show recent searches
- **Anti-pattern**: Never just say "No results" with no guidance

#### Error Empty State
Data exists but can't be loaded.
- **Goal**: Explain and provide recovery path
- **Title**: Specific error ("Unable to load messages")
- **Description**: What happened + what to do ("Connection failed. Check your network and try again")
- **Actions**: "Retry" button, link to help/status page
- **Anti-pattern**: Generic "Something went wrong" without actionable guidance

#### Completed/All-Clear Empty State
All items have been addressed (e.g., empty inbox, no alerts).
- **Goal**: Reassure — this is a GOOD state
- **Title**: Positive message ("All caught up!")
- **Description**: Brief context ("No alerts require your attention")
- **Actions**: None required. Optional secondary suggestion ("Review past alerts")
- **Anti-pattern**: Showing the same empty state as "first-use" when the user has
  actually completed their work

#### Insufficient Permissions Empty State
User doesn't have access to view this content.
- **Goal**: Explain and direct to resolution
- **Title**: "You don't have access to this resource"
- **Description**: Why + who to contact ("Contact your administrator to request access")
- **Actions**: "Request Access" or "Go Back" button

### Alternative Strategies for First-Use

Beyond the simple illustration + CTA approach:

1. **Inline Documentation** — Place educational content directly where data will
   eventually appear. Show the data area with annotated placeholders explaining
   what each column/section means.
2. **Starter Content** — Pre-populate with sample/template data the user can
   explore, tinker with, and delete. Reduces anxiety and demonstrates features
   by example.
3. **Guided Onboarding** — Replace the empty state with a step-by-step setup
   flow (see Onboarding section below).

### Design Rules

- Empty state area should match the same layout dimensions as the populated state
- Illustrations should be subtle and on-brand, not cartoonish or distracting
- The primary CTA must be the most prominent visual element
- Keep copy concise — three lines maximum for description
- If the empty state is in a small container (card, sidebar), use a minimal
  variant: icon + one line + link

---

## 2. Filtering Patterns

Filtering enables users to narrow a displayed data set by toggling predefined attributes.

### Filter Selection Methods

#### Single Selection
User picks ONE attribute from a category.
- **Implementation**: Radio buttons, dropdown
- **When**: Exclusive options (status: Active/Inactive, type: Input/Output)
- **Behavior**: Results update immediately on selection

#### Multi-Selection
User picks MULTIPLE attributes from a category.
- **Implementation**: Checkboxes, multi-select dropdown, tag filter
- **When**: Combinable options (tags, categories, date ranges)
- **Behavior**: Two strategies — see below

### Filter Interaction Models

#### Interactive (Real-Time)
Each selection immediately updates results.
- **Best for**: Single-category filtering, small data sets, fast server response
- **Behavior**: Select filter → results update instantly
- **When**: User is making few selections, one category at a time

#### Batch
Selections are collected, then applied together.
- **Best for**: Multi-category filtering, slow data loading, complex queries
- **Behavior**: Select multiple filters → click "Apply" → results update
- **When**: User needs time to think through combinations, or each query is expensive

### Filter Placement

| Position | When to Use |
|----------|-------------|
| **Above table** | Horizontal bar for ≤ 3 filter categories, common quick filters |
| **Side panel (left)** | Many categories (4+), faceted search, e-commerce style |
| **Popover/dropdown** | Space-constrained views, secondary filters |
| **In-table header** | Per-column filtering for data tables |

### Filter State Management

1. **Default state**: Either all selected or all unselected (choose per category based
   on which scenario users encounter more: "show me only X" vs "hide just Y")
2. **Active filter indicators**: Show chips/tags above results for each active filter
3. **Clear per category**: Each category has its own "Clear" control
4. **Clear all**: A global "Clear all filters" control resets everything
5. **Persistence**: Remember filter state across navigation and sessions
6. **Count feedback**: Show result count after filtering ("23 of 150 items")
7. **Zero results**: Show empty state with suggestions to adjust filters

### Filter Anti-Patterns
- Filters that produce zero results without warning (disable unavailable options
  or show counts per filter value)
- Filter changes that cause page reload instead of in-page update
- No indication of which filters are active
- No way to clear all filters at once
- Filters that reset when returning from a detail page

---

## 3. Loading Patterns

Loading states bridge the gap between user action and system response.
Use the minimum viable indicator for each situation.

### Loading Types

#### Skeleton States (Structural Preview)
Simplified, animated placeholders matching the SHAPE of the content that will load.
- **When**: Initial page load, lazy-loaded sections, content-heavy components
- **Duration**: Should appear for only a few seconds maximum
- **Apply to**: Cards, tables, lists, text blocks, image placeholders
- **DO NOT apply to**: Buttons, inputs, checkboxes, toggles (action components
  don't need skeletons), modals, toast notifications, dropdown menus
- **Motion**: Subtle shimmer/pulse animation (left-to-right wave) to convey
  that the page is NOT stuck
- **White space**: Elements that don't need skeletons can be plain white/empty
  space (e.g., a 600x400 image area stays blank until loaded)

#### Inline Loading (Component-Level)
A spinner or progress indicator within a specific component.
- **When**: A single component is processing (e.g., saving a row, submitting a field)
- **Duration**: Replaces or overlays the trigger element
- **Examples**: Spinner inside a button after click, spinner replacing a table row
  during update, loading bar in a file upload
- **Rule**: Don't block the entire page for a single component operation

#### Full-Screen Loading (Page-Level)
An overlay or centered spinner for the entire page.
- **When**: Full form submission, page-level data processing, authentication
- **Duration**: If > 30 seconds, provide a progress bar with estimated time
- **Add notification**: If > 2 minutes, send an email/notification when complete

#### Progressive Loading
Content loads incrementally as it becomes available.
- **When**: Lists, feeds, search results, image galleries
- **Methods**:
  - **Lazy loading**: Below-the-fold content loads on scroll
  - **Load more button**: Explicit trigger to fetch the next batch
  - **Infinite scroll**: Auto-loads as user nears bottom (feeds only, not tables)
  - **Pagination**: Traditional page-by-page (preferred for enterprise data)

### Loading Decision Framework

```
Is the entire page loading for the first time?
  YES → Skeleton states for content areas
Is a single component processing?
  YES → Inline loading (spinner in component)
Is a user action processing (form submit, save)?
  YES → Will it complete in < 5 seconds?
        YES → Full-screen spinner with overlay
        NO → Progress bar + optional notification
Is content streaming in gradually?
  YES → Progressive loading (lazy load or load more)
```

### Loading Anti-Patterns
- No loading indicator at all (the page appears frozen)
- Full-screen spinner for a single field saving
- Skeleton states that last more than 10 seconds (switch to progress indicator)
- Progress bars that jump or move backwards
- Infinite loading with no timeout or error state
- Loading indicators without accessible text (screen readers must be notified)

---

## 4. Dialog Patterns

Dialogs interrupt the user's current task. Use them sparingly and correctly.

### Modal Dialogs (Blocking)
The user MUST interact before continuing.

**When to use:**
- Confirming destructive actions (delete, discard changes)
- Urgent system messages (session expired, account suspended)
- Quick creation/editing of records (< 5 form fields)
- Critical information the user must acknowledge

**When NOT to use:**
- Simple confirmations ("Saved successfully" — use a snackbar)
- Tasks with many fields (> 5 — use a side panel or dedicated page)
- Repeated tasks (if done frequently, embed in the main page)
- System-generated interruptions unrelated to user's current task

**Button Layout Rules (Carbon standard):**
- Cancel is ALWAYS the leftmost button
- Primary action is ALWAYS the rightmost button
- Maximum ONE primary action per dialog
- Buttons are full-bleed to the bottom edge of the dialog

| Buttons | Layout |
|---------|--------|
| **1 button** (acknowledgment) | Right-aligned, spans 50% width |
| **2 buttons** (confirm/cancel) | Side by side, each 50% width |
| **3 buttons** | Each 25% width, right-aligned |
| **Progress buttons** | "Back" on left, "Next"/"Finish" on right |

**Validation:**
- Validate BEFORE closing the dialog, not after
- If invalid, keep the dialog open with inline error messages
- Disable the submit button until required fields are valid

**Focus Management:**
- On open: Focus the first interactive element (or the close button if no inputs)
- Focus trap: Tab cycling stays WITHIN the dialog
- On close: Return focus to the element that triggered the dialog

### Non-Modal Dialogs (Non-Blocking)
The user CAN continue interacting with the main page.

**When to use:**
- Reference information the user needs alongside their task
- Side panels for detail views or secondary forms
- Floating toolbars or inspector panels

**Design:** No backdrop overlay, can be moved/resized, auto-close options.

### Destructive Action Confirmation

Three tiers of protection based on severity:

1. **Simple Popconfirm**: Quick inline confirmation bubble near the trigger.
   For: Removing an item from a list (recoverable).
2. **Danger Modal**: Red-themed modal with clear consequences.
   For: Deleting a record, discarding unsaved changes.
3. **Manual Confirmation Modal**: User must TYPE the name of the resource
   to confirm deletion. For: Deleting irreplaceable data, bulk deletions,
   resource destruction with cascading effects.

---

## 5. Disabled, Read-Only, and Hidden States

Three distinct states for non-interactive elements. They are NOT interchangeable.

### Disabled State
The component exists but is temporarily non-interactive.

**When**: Dependencies not met, prerequisites unfulfilled, processing in progress.
**Visual**: Reduced opacity (≈ 50%), `not-allowed` cursor, no hover effects.
**Behavior**: Present in DOM, NOT focusable by keyboard, NOT announced by screen
readers as interactive. Returns to enabled state when condition resolves.
**Warning**: If a disabled element affects the primary action or multiple items,
show an inline notification explaining HOW to enable it.

### Read-Only State
The component shows data that the user can review but NOT modify.

**When**: Reviewing submitted data, locked fields, compliance/audit views,
viewing another user's data.
**Visual**: Transparent background (no input field styling), borders de-emphasized,
text color same as enabled state (maintains contrast), NO interactive affordances
(no chevrons, no underlines, no hover states).
**Behavior**: Navigable by keyboard (can be reached via Tab), but NOT operable
(values cannot be changed). Screen readers announce the value.
**Critical distinction from disabled**: Read-only content MUST be accessible.
Disabled content may NOT be. Use read-only when the user NEEDS to read the value.

### Hidden State
The component is completely removed from the interface.

**When**: User lacks permissions to even SEE the element. Permission-based UI.
**Visual**: Element is not rendered at all.
**Behavior**: Cannot be discovered by any means until permissions change.
**Example**: "Add Member" button visible only to organization owners.

### Decision Framework

```
Does the user have permission to SEE this element?
  NO → Hidden state
  YES → Does the user have permission to CHANGE this element?
        NO → Read-only state
        YES → Is the element temporarily unavailable due to dependencies?
              YES → Disabled state
              NO → Enabled state (normal)
```

**Anti-pattern**: Using disabled state where read-only is appropriate. Disabled
elements are not accessible to screen readers and don't pass visual contrast
requirements. If the user needs to READ the value, use read-only, not disabled.

---

## 6. Common Action Standardization

Consistency in action verbs, icons, and placement across an entire product.

### Action Dictionary

| Action | Description | Icon | Button Style | Notes |
|--------|------------|------|-------------|-------|
| **Add** | Insert existing object to list/set | Plus (+) | Varies by prominence | Doesn't create new — adds existing |
| **Create** | Generate new object | Plus (+) | Primary (high emphasis) | Creates something that didn't exist |
| **Edit** | Modify existing object | Pencil | Secondary or menu option | Triggers edit mode or opens form |
| **Delete** | Permanently destroy object | Trash | Danger button or menu | Irreversible — requires confirmation |
| **Remove** | Detach object from list (not destroy) | Minus (−) | Subtle/icon-only | Object still exists elsewhere |
| **Copy** | Create identical instance | Copy icon | Icon with "Copied" tooltip | Brief confirmation tooltip post-click |
| **Save** | Persist current state | — | Primary button | Must give success feedback |
| **Cancel** | Abort current action | — | Secondary button | Warn if data will be lost |
| **Close** | Dismiss UI element | × icon | Top-right icon | Never use "Close" as a button label |
| **Clear** | Empty field/selections | × icon (in field) | Inline icon | Resets to default, doesn't delete data |
| **Next/Back** | Sequential navigation | Arrow icons | Button + icon | "Next" is primary, "Back" is secondary |
| **Refresh** | Reload current view | Refresh icon | Icon button | For stale/unsynchronized views |
| **Reset** | Revert to last saved state | — | Link style | Less prominent than Cancel |

### Delete Confirmation Tiers

| Severity | Confirmation | Use When |
|----------|-------------|----------|
| Low | Popconfirm ("Delete this item?") | Single item, recoverable, low value |
| Medium | Danger modal with consequences | Multiple items, or medium value data |
| High | Modal + type-to-confirm | Irreplaceable data, cascading effects, bulk deletion |

### Post-Delete Behavior
1. Return to the list page that contained the deleted item
2. Animate the removal from the list (don't just reload)
3. Show success snackbar with optional "Undo" (if soft-delete is supported)
4. If deletion fails, show error notification

### Error Message Rules
- Be brief, honest, and supportive
- State WHAT happened + WHAT the user can do
- Full-page modals: ≤ 3 paragraph lines
- Form-field errors: ≤ 2 lines
- For inline errors in small components (text input), use inline notification
  placed directly below the field

---

## 7. Search Patterns

Search is a primary navigation pattern with high user expectations.

### Search Types by Scope

| Type | Scope | Placement | Behavior |
|------|-------|-----------|----------|
| **Global** | Entire application | In header bar | Opens results page, navigates away |
| **Contextual** | Within current view | Above table/list | Filters current data set in real time |
| **Scoped** | Within pre-selected category | Header with scope selector | Results limited to chosen scope |

### Search Behavior

1. **Type-ahead**: Begin showing suggestions as user types (≥ 2 characters)
2. **Recent searches**: Show recent queries when the search field is focused
3. **Result count**: Always display ("23 results for 'servers'")
4. **No-results state**: Specific guidance — suggest alternatives, show "Clear search"
5. **Loading state**: For async search, show skeleton that matches empty state layout
6. **Keyboard**: Enter to submit, Escape to clear, Up/Down to navigate suggestions

### Search Field Rules
- NO label needed (search icon is universally understood)
- Placeholder text: "Search..." or "Search [context]..." (e.g., "Search users...")
- Clear button (×) appears when text is entered
- Full-width within its container (don't make users guess the search field)

---

## 8. Login Pattern

Authentication is the gateway to every application. Get it right.

### Flow Structure

1. **Username/Email** → "Continue" button
   - Validates format and routes to SSO or password flow
2. **Password entry** → "Log in" button
   - Shows/hides password toggle
   - "Forgot password?" link
3. **Success** → Redirect to last page or dashboard

### Design Rules

- Primary button ("Continue" / "Log in") must be directly below the last input
  field — do NOT place other buttons or links between input and primary CTA
- Alternate login buttons (SSO, Google, etc.) go BELOW the primary CTA
- Never between input field and primary button
- Never above the form
- Same generic error for wrong username OR password (security: don't reveal
  which field was incorrect)
- Server errors: Reload page, clear password, focus username, show inline
  notification at top of form
- After voluntary logout: Show confirmation, then redirect to login page
- After inactivity logout: After re-login, redirect to the LAST page visited
- Fluid inputs in centered container for focused experience; default inputs
  in side-aligned panel for split-screen with marketing content

---

## 9. Disclosure Pattern

Disclosures toggle visibility of additional content via a trigger element.

### Types

| Type | Trigger | Content | Dismissal |
|------|---------|---------|-----------|
| **Tooltip** | Hover/focus | Text only, non-interactive | Move away |
| **Toggletip** | Click | Text, can include links | Click trigger again, click outside |
| **Settings menu** | Icon button | Interactive controls (checkboxes, dropdowns) | Apply/Cancel buttons, click outside |
| **Profile menu** | Avatar/icon | Account info, settings links, logout | Click outside, selection |
| **Combo button** | Split button | List of related actions | Selection, click outside |

### Disclosure Rules
- Trigger must be keyboard accessible (Enter/Space to toggle)
- Focus management: on open, focus first interactive element inside
- `aria-expanded` attribute on trigger reflects state
- Content must be accessible — toggletips support interactive content that
  tooltips do not
- Settings/filter popovers: include Cancel + Apply buttons if changes affect
  other content on the page
- Never nest disclosures (no popover inside a popover)

---

## 10. Status Indicators

Visual communication of state across the application.

### Severity Levels

| Level | Color | Icon Shape | Use For |
|-------|-------|-----------|---------|
| **Critical/Danger** | Red | Filled circle / ⬡ hexagon | Errors, failures, security issues |
| **Warning** | Yellow/Amber | △ triangle | Approaching limits, degraded service |
| **Success/Stable** | Green | ✓ checkmark circle | Completed, active, healthy |
| **Informational** | Blue | ⓘ info circle | Tips, neutral updates, FYI items |
| **In-Progress** | Blue/Gray | Spinner / ◐ half-circle | Running processes, pending |
| **Not Started** | Gray | ○ empty circle | Queued, draft, disabled tasks |
| **New** | Teal/Purple | Star / dot | New items, recently added |

### Status Indicator Rules

1. **Don't rely on color alone** — Always pair color with icon shape or text
2. **Filled vs outlined** — Filled icons carry more weight (high severity);
   outlined for lower severity. Be consistent within a product.
3. **Consolidated status** — When multiple statuses merge, show the HIGHEST
   severity (if children are green/yellow/red, parent shows red)
4. **Don't overuse** — If status isn't important or no action is needed,
   use plain text instead of status indicators
5. **Cultural awareness** — Shapes have cultural associations (hexagon = stop,
   triangle = yield). Don't violate learned recognition patterns.

---

## 11. Onboarding Patterns

Guiding new users from first launch to first value.

### Onboarding Approaches

#### Guided Tour
Step-by-step walkthrough highlighting key UI elements.
- **When**: Complex interface with non-obvious features
- **Implementation**: Spotlight overlay + tooltip pointing to each feature
- **Rules**: 3–5 steps maximum, skip button always available, progress indicator,
  don't force completion, persist "skipped" state (don't show again)

#### Checklist
Task list the user completes at their own pace.
- **When**: Setup requires multiple independent steps
- **Implementation**: Side panel or dashboard widget with checkable items
- **Rules**: Show progress ("3 of 5 complete"), allow non-sequential completion,
  mark completed items clearly, celebrate final completion

#### Inline Education
Contextual tips that appear where and when relevant.
- **When**: Features that are discovered gradually over time
- **Implementation**: Tooltips, callout banners, signpost indicators
- **Rules**: Show once per feature, dismiss permanently, don't overlap multiple
  tips at once, context-sensitive (show only when the feature is relevant)

#### Progressive Disclosure
Reveal complexity gradually as the user gains experience.
- **When**: Application has both basic and advanced modes
- **Implementation**: "Advanced options" toggle, feature gating by usage level,
  gradually introducing sidebar sections
- **Rules**: Default to simple, let the user opt into complexity,
  never hide critical functionality behind progressive disclosure

### Onboarding Anti-Patterns
- Overwhelming the user with a 10+ step tour before they can do anything
- Showing the full feature set on first use (cognitive overload)
- No way to skip or dismiss onboarding
- Re-showing onboarding after the user has dismissed it
- Onboarding that blocks the actual interface (let users explore alongside)

---

## 12. Text Toolbar Pattern

For rich text editing areas within applications.

### Toolbar Anatomy
1. **Actions**: Undo, Redo, Cut, Copy, Paste
2. **Formatting**: Bold, Italic, Underline, Strikethrough, Font size, Color
3. **Paragraph**: Alignment (left/center/right), Indent, Lists (bullet/numbered/checklist)
4. **Insert**: Attachments, Links, Tables, Images
5. **Search**: Find within text

### Toolbar Rules
- Position: Fixed above the text area, spans full width of the editor
- Grouping: Separate tool groups visually with dividers
- Tooltips: Every button has a tooltip with the action name + keyboard shortcut
- Keyboard: Tab/Shift+Tab to enter/exit toolbar; Arrow keys within toolbar;
  Enter to activate; Escape to return to text area
- Drafts: Support autosave with timestamp ("Saved 2 minutes ago") or
  explicit "Save Draft" button
- Overflow: On narrow screens, less-used tools collapse into an overflow menu (⋯)

---

## 13. Internationalization Deep-Dive

Beyond basic RTL and text expansion (covered in enterprise-patterns-advanced.md).

### Number and Currency Best Practices

| Locale | Number | Currency | Date |
|--------|--------|----------|------|
| en-US | 1,234.56 | $1,234.56 | 01/15/2025 |
| de-DE | 1.234,56 | 1.234,56 € | 15.01.2025 |
| ru-RU | 1 234,56 | 1 234,56 ₽ | 15.01.2025 |
| tr-TR | 1.234,56 | ₺1.234,56 | 15.01.2025 |
| ja-JP | 1,234 | ¥1,234 | 2025/01/15 |
| ar-SA | ١٬٢٣٤٫٥٦ | ١٬٢٣٤٫٥٦ ر.س | ١٥/٠١/٢٠٢٥ |

### Rules
- Never concatenate strings for plurals: use ICU MessageFormat or equivalent
  ```
  BAD:  "You have " + count + " item" + (count > 1 ? "s" : "")
  GOOD: "{count, plural, one {# item} other {# items}}"
  ```
- Icons that imply direction (arrows, chevrons, back) MUST flip for RTL
- Icons representing real objects (phone, email, search) do NOT flip
- Progress bars and steppers reverse direction in RTL
- Scrollbars move to the left side in RTL
- Test all UI with German text (typically 30–40% longer than English)
- Test all UI with CJK characters (may need different font stack and line-height)
- Avoid text in images — it cannot be translated

---

## 14. Form Variants by Context

Beyond the basic form page, forms appear in different containers.

### Form Contexts

| Context | Max Fields | Container | Button Placement |
|---------|-----------|-----------|-----------------|
| **Full-page form** | Unlimited | Centered content area (600–800px) | Bottom of form, left-aligned |
| **Dialog form** | ≤ 5 | Modal dialog | Full-bleed bottom of dialog |
| **Side panel form** | 5–15 | Right panel (400–480px) | Fixed bottom of panel |
| **Inline form** | 1–3 | Directly in page content | Inline with the fields |
| **Accordion form** | 6+ grouped | Expandable sections | Per-section or at bottom |
| **Multi-step form** | 6+ sequential | Wizard/Stepper | Per-step (Back/Next/Finish) |

### Accordion Form Caution
Accordion forms improve completion speed and reduce page load, BUT create
confusion around primary action buttons. Users may not know if "Submit" applies
to the current section or the entire form. **Never use accordion forms inside
dialogs.** Always make it clear that the primary action applies to the ENTIRE form.

### Required vs Optional Labeling
- If MOST fields are required → Mark only the optional ones "(optional)"
- If MOST fields are optional → Mark only the required ones "(required)"
- Be consistent across the entire product
- Asterisks (*) are an accessibility hazard — use the text "(required)" for clarity

### AI-Generated Content in Forms
When AI assists in filling form fields:
- Mark AI-generated content with a visible indicator (AI label/icon)
- Provide an "explain" action so users can understand why AI suggested a value
- Two scopes: full-form AI presence (entire form is AI-assisted) or
  per-component AI presence (only specific fields are AI-generated)

---

## 15. Design Token Architecture — Enhanced

Building on the three-tier system in enterprise-patterns-advanced.md,
here are additional token patterns from Clarity and Carbon.

### T-Shirt Size Spacing Tokens

Instead of numbered spacing (space-1, space-2...), use semantic t-shirt sizes
for layout spacing between components:

```
xxxs: 2px     — micro gaps (icon-to-text)
xxs:  4px     — tight internal padding
xs:   8px     — compact internal padding, inline spacing
sm:   12px    — standard internal padding
md:   16px    — section padding, card padding
lg:   20px    — comfortable section spacing
xl:   24px    — major section separation
xxl:  32px    — page-level spacing
xxxl: 48px    — hero/banner spacing
```

**Why t-shirt sizes**: Designers and developers share a common language.
"Use SM here" → "Let's try MD instead" is faster than debating pixel values.

### Functional Color Aliases (Clarity Pattern)

Colors should be named by FUNCTION, not by appearance:

```
Status colors:       --status-success, --status-warning, --status-danger, --status-info
Interaction colors:  --interactive-default, --interactive-hover, --interactive-active
Container colors:    --container-background, --container-border, --container-shadow
Typography colors:   --text-primary, --text-secondary, --text-disabled, --text-heading
```

Each status color should have three variants:
```
--status-success:       hsl(93, 67%, 38%)     // Primary
--status-success-shade: hsl(93, 67%, 28%)     // Hover/active
--status-success-tint:  hsl(93, 30%, 92%)     // Background/highlight

Warning and Danger may need a 4th (dark) shade for additional contrast needs.
```

### Cool Grays with Brand Tint

Clarity's approach: instead of pure neutral grays (0% saturation), add 2–5%
saturation in your brand's hue direction. This creates a more vibrant, cohesive
palette without being obviously tinted:

```
Flat gray:   hsl(0, 0%, 90%)      // Clinical, lifeless
Cool gray:   hsl(210, 5%, 90%)    // Subtle blue tint (brand-aligned)
Warm gray:   hsl(30, 5%, 90%)     // Subtle warm tint (alternate brand)
```

Apply to: borders, backgrounds, dividers, shadows, secondary text.
The effect is invisible in isolation but creates a more alive overall composition.

### Line-Height Eraser Technique (Clarity)

Browsers apply line-height padding equally above and below text, even for single
lines. This creates unpredictable spacing when centering text (e.g., button labels).
The "line-height eraser" technique uses negative margins or padding adjustments
to compensate, giving precise control of vertical text placement.

Use this pattern when:
- Vertically centering text in buttons, badges, or labels
- Achieving exact alignment between text and icons
- Maintaining pixel-perfect vertical rhythm in compact layouts

---

## Summary: What Each System Does Best

| Aspect | Best Source | Key Insight |
|--------|-----------|-------------|
| **Design philosophy** | Ant Design | Four values (Natural, Certain, Meaningful, Growing) |
| **Interaction principles** | Ant Design | 10 principles (Make it Direct, Stay on Page, etc.) |
| **Token architecture** | Clarity | Three tiers + cool grays + t-shirt sizing |
| **Empty states** | Carbon | Five types with specific anatomy for each |
| **Filtering** | Carbon | Batch vs interactive, placement matrix |
| **Loading** | Carbon | Skeleton states vs inline vs progressive |
| **Dialog rules** | Carbon | Button layout rules, validation, focus management |
| **Disabled/read-only** | Carbon | Three distinct states with decision framework |
| **Common actions** | Carbon | Standardized verb dictionary for entire product |
| **Multi-step workflows** | Clarity | Wizard vs Stepper vs Timeline selection |
| **Notifications** | Clarity | Five-tier priority hierarchy |
| **Density** | Clarity | Regular/compact with variable line heights |
| **Accessibility** | All three | Built-in architecture, not bolt-on |
| **i18n** | All three | RTL, text expansion, locale formatting |
| **Forms** | All three | Context-based variants, required/optional labeling |
| **Onboarding** | Clarity | Guided tour, checklist, inline education |
| **Copywriting** | Ant Design | User-centered, concise, consistent |
| **Page templates** | Ant Design | Form, list, detail, workbench, result, exception |
| **Cross-platform** | This skill | WPF/XAML porting guide |
