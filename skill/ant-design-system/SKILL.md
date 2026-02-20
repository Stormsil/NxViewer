---
name: ant-design-system
description: >
  Comprehensive enterprise design-system guidance combining Ant Design 5.x, IBM Carbon, and VMware Clarity for web and desktop interfaces. Use for admin panel, dashboard, and CRUD UI design with consistent layout, typography, color, spacing, navigation, forms, data display, feedback, motion, dark mode, accessibility, internationalization, density modes, tokens, and notification hierarchy. Includes patterns for list/detail pages, workbenches, filtering, loading, empty/error states, dialogs, onboarding, search, status indicators, and multi-step workflows. Trigger when requests mention design system, Ant Design/antd, Carbon, Clarity, enterprise UI, polished consistency, or system-level UX architecture.
---

# Enterprise Design System — Comprehensive Design Skill

This skill encodes the complete Ant Design 5.x design philosophy, synthesized with
the best patterns from IBM's Carbon Design System and VMware's Clarity Design System.
Any AI agent reading it can design interfaces as if it had spent weeks studying all
three documentation sets. The principles are **platform-agnostic** — they apply equally
to React web apps, WPF desktop apps, mobile UIs, or any other technology.
The goal: produce interfaces that are Natural, Certain, Meaningful, and Growing.

## Before You Start

Read the relevant reference files based on the task:

| Need | Reference File |
|------|---------------|
| **Always** | `references/design-values-and-principles.md` — the philosophical foundation |
| **Always** | `references/visual-foundation.md` — color, typography, spacing, layout, shadow, dark mode |
| **Layout/Structure** | `references/page-templates.md` — form pages, list pages, detail pages, workbenches, dashboards |
| **Interaction** | `references/interaction-patterns.md` — the 10 interaction principles in full depth |
| **Components** | `references/component-patterns.md` — buttons, navigation, data entry, data display, feedback |
| **Copywriting** | `references/copywriting-and-data.md` — text rules, data formatting, empty states |
| **WPF/XAML** | `references/cross-platform-xaml.md` — porting Ant Design to WPF and desktop apps |
| **Enterprise/Advanced** | `references/enterprise-patterns-advanced.md` — tokens, density, accessibility, i18n, notifications, multi-step workflows, advanced datagrid |
| **UX Patterns** | `references/ux-patterns-comprehensive.md` — empty states, filtering, loading, dialogs, disabled/read-only states, common actions, search, login, onboarding, disclosures, status indicators, text toolbar |

**CRITICAL**: Always read `references/design-values-and-principles.md` and `references/visual-foundation.md`
before designing anything. These two files are the DNA of the entire system. For enterprise/data-heavy
applications, also read `references/enterprise-patterns-advanced.md` for token architecture, density,
accessibility, and notification patterns. For any feature that involves empty states, filtering, loading,
dialogs, onboarding, or search, read `references/ux-patterns-comprehensive.md` for synthesis of best
practices from Carbon (IBM), Clarity (VMware), and Ant Design.

## Core Philosophy (Quick Reference)

Ant Design is built on **four design values**:

1. **Natural** — Interfaces should follow natural cognitive and behavioral patterns. Minimize
   cognitive load. Use visual hierarchy derived from how humans naturally scan information.
2. **Certain** — Reduce ambiguity. Every element should have clear purpose. Maintain consistency
   across products, screens, and platforms. Use modular, object-oriented design thinking.
3. **Meaningful** — Every interaction should serve the user's mission. Clear goals → immediate
   feedback. Keep users in flow state with moderate challenge and zero distraction.
4. **Growing** — Design for evolution. Products and users grow together. Make features discoverable
   progressively. Build systems that scale.

## Design Decision Framework

When facing any UI decision, apply this checklist:

### 1. Information Architecture
- What is the user's primary mission on this screen?
- What information hierarchy exists? (Primary → Secondary → Tertiary)
- Can I reduce? ("Perfection is achieved when there is nothing left to take away.")

### 2. Layout
- Use 4px sub-grid for all internal spacing; 8px multiples for layout spacing
- 24px baseline grid for vertical rhythm
- 24-column grid for page layout
- Three spacing levels: 8px (tight/related), 16px (standard), 24px (section separation)
- Top-to-bottom single-column for forms; cards for complex content grouping
- Support density modes: regular (default) and compact (power users, data-heavy views)

### 3. Visual Hierarchy
- Typography scale: keep to 3–5 font sizes maximum
- Font weights: Regular (400) and Medium (500) cover 95% of cases; Semibold (600) for English headers
- Base font size: 14px, line-height: 22px (standard), 18px (compact), 28px (expanded)
- Letter spacing: tighten on large sizes (≥24px), open on small sizes (≤12px)
- Color is restrained — used for information delivery, not decoration
- Use cool grays (2–5% saturation) instead of pure neutral grays
- Contrast ratio ≥ 7:1 (WCAG AAA) for body text against background

### 4. Color Architecture
- Three-tier token system: Global Tokens → Semantic Aliases → Component Tokens
- Use HSL for code (human-readable), HSB for design tool communication
- Use dedicated tint/shade tokens, NOT transparency for hover/active states
- Never rely on color alone — always pair with text, icons, or patterns (WCAG)

### 5. Interaction
- Where there is output, let there be input (Make it Direct)
- Solve problems on the same page (Stay on the Page)
- Reduce target acquisition cost (Keep it Lightweight — Fitts's Law)
- Make hidden features discoverable (Provide an Invitation)
- Smooth state changes with animation (Use Transition)
- Immediate feedback for every action (React Immediately)

### 6. States
- **Enabled** → normal interactive state
- **Disabled** → temporarily non-interactive (dependencies not met), reduced opacity
- **Read-only** → visible but not modifiable (user needs the data), accessible contrast
- **Hidden** → removed from UI entirely (no permission to view)
- **Loading** → skeleton states for content, inline spinner for actions, progress bar for long ops

### 7. Feedback (Five-Tier Notification Hierarchy)
- **Snackbar** (lowest): Quick confirmation of successful user actions, auto-dismiss 3–5s
- **Alert** (inline): Low-priority contextual info about specific sections
- **Toast** (notification): Medium-priority system events, top-right, auto-dismiss 6–8s
- **Banner** (page-level): High-priority issues affecting the whole app, persists
- **Modal** (blocking): Highest priority, requires immediate user decision

### 8. Accessibility (Built-In, Not Bolted-On)
- Keyboard navigation: all interactive elements reachable via Tab, logical order
- Focus management: on route change, focus first meaningful content element
- Screen reader: proper heading hierarchy, aria-current, aria-live, aria-expanded
- Touch targets: ≥ 44x44px (WCAG 2.5.5) on touch, ≥ 32x32px mouse-only
- Color independence: never use color alone to convey meaning
- Respect `prefers-reduced-motion` for all animations
- Focus-visible indicators distinct from hover states

## Component Selection Guide

When choosing components, follow this decision tree:

### Navigation
- **< 5 top-level sections** → Top Navigation bar
- **5–20 sections** → Side Navigation (collapsible)
- **Deep hierarchy** → Side Nav + Breadcrumbs
- **Parallel content views** → Tabs
- **Sequential workflow** → Steps/Stepper
- **Long list pagination** → Pagination component

### Data Entry
- **1 of few options (< 5)** → Radio buttons (all visible)
- **1 of many options (≥ 5)** → Dropdown/Select
- **Multiple of many** → Checkbox group or Transfer
- **Binary toggle** → Switch (immediate effect) or Checkbox (with submit)
- **Free text** → Input (single line) or Textarea (multi-line)
- **Date/time** → DatePicker/TimePicker
- **File** → Upload (click, thumbnail, or drag-drop based on context)
- **Range value** → Slider

### Data Display
- **Structured rows + columns** → Table
- **Grouped detail fields** → Descriptions
- **Collapsible sections** → Collapse/Accordion
- **Content preview cards** → Card grid
- **Chronological events** → Timeline
- **Hierarchical data** → Tree
- **Image galleries** → Carousel or Image grid

### Feedback
- **Global success/info (lightweight)** → Message (top-center toast, auto-dismiss)
- **Global with detail** → Notification (top-right, with content)
- **Inline warning/info** → Alert banner
- **Needs user decision** → Modal dialog or Popconfirm
- **Operation progress** → Progress bar or Spinner
- **Completed workflow** → Result page
- **Contextual help** → Tooltip (simple) or Popover (complex)
- **Quick confirmation** → Snackbar (bottom, auto-dismiss, optional "Undo")
- **Page-level critical** → Banner (full-width, persistent, action button)

### Dialogs (Modal Button Rules — Carbon Standard)
- Cancel is ALWAYS the leftmost button
- Primary action is ALWAYS the rightmost button
- Maximum ONE primary action per dialog
- 1 button: right-aligned, 50% width (acknowledgment)
- 2 buttons: side-by-side, each 50% (confirm/cancel)
- 3 buttons: each 25%, right-aligned
- Validate BEFORE closing — keep dialog open if invalid
- Focus trap: Tab stays inside the dialog

## Anti-Patterns to Avoid

These are the most common mistakes that violate design system principles:

1. **Over-decoration** — Using color, borders, shadows everywhere. Enterprise UI is restrained.
2. **Inconsistent spacing** — Random padding/margins. Always use 4px sub-grid, 8px layout grid.
3. **Too many font sizes** — Keep to 3–5 sizes. Each size should have a clear semantic role.
4. **Multi-column forms** — Single-column vertical layout is fastest for task completion.
5. **Page redirects for simple actions** — Use overlays, inline edits, or expandable sections.
6. **No loading states** — Every async operation needs visual feedback.
7. **Confirmation dialogs for everything** — Only for destructive/irreversible actions.
8. **Hidden critical actions** — Primary actions must be immediately visible.
9. **Walls of text** — Chunk information. Use hierarchy. Progressive disclosure.
10. **Ignoring empty states** — Always design the zero-data state. Guide users to take action.
11. **Hardcoded values** — Use design tokens, not magic numbers. Components should reference
    aliases, not raw color/spacing values.
12. **Transparency for states** — Using rgba() for hover/active states. Use dedicated tint/shade
    tokens for predictable, accessible results.
13. **Color-only meaning** — Using red/green alone without icons or text. Violates WCAG.
14. **Single density** — Not supporting compact mode for data-heavy views and power users.
15. **No i18n planning** — Fixed-width text containers that break with longer translations.
16. **Wrong notification channel** — Using modals for success confirmations, or toasts for
    critical errors. Match urgency to the correct tier.
17. **Disabled where read-only is needed** — Disabled elements aren't screen-reader accessible.
    If the user needs to READ the value, use read-only, not disabled.
18. **No filter state indicators** — Active filters must be visible as chips/tags above results,
    with a "Clear all" option.
19. **Full-page spinner for single-field save** — Use inline loading (spinner in the component)
    for single-component operations. Full-screen loading is for page-level operations only.
20. **Skeleton states lasting > 10 seconds** — Switch to a progress indicator with estimated time.
21. **Generic "Something went wrong"** — Error states must explain WHAT happened + WHAT the user
    can do about it. Include specific recovery actions.
22. **Login errors revealing which field is wrong** — Use the same generic error for wrong username
    or wrong password (security: don't reveal which is correct).
23. **Accordion forms in dialogs** — Never. Accordion sections create confusion about whether
    buttons apply to the section or the full form.
24. **Onboarding that blocks the UI** — Tours/checklists should work alongside the real interface,
    not replace it. Always provide a skip option.

## Quick Start Recipes

### Recipe: Admin Dashboard
```
Layout: Sidebar (240px collapsed 80px) + Header (64px) + Content
Navigation: Side NavigationView with icons + labels, collapsible
Content: Card-based grid (24-col, gutters 16px or 24px)
Top cards: Statistic components (4-across = span-6 each)
Middle: Chart area (span-16) + Recent activity (span-8)  
Bottom: Data table with search + filters above
Color: Brand primary as accent only. Neutral grays for structure.
```

### Recipe: CRUD List Page
```
Layout: Full-width content area within shell
Top: Page header with title + primary "Create" button (right-aligned)
Filters: Horizontal filter bar (collapse if > 3 filters)  
Content: Table with sortable columns, row actions, batch actions
Empty state: Illustration + description + "Create First Item" CTA
Pagination: Bottom-right, show total count
```

### Recipe: Form Page
```
Layout: Centered content, max-width 600–800px
Structure: Single-column, top-to-bottom
Sections: Card grouping if > 2 screens of content
Labels: Left-aligned or top-aligned (top for long labels)
Validation: Inline, real-time where possible
Actions: "Submit" (primary) + "Cancel" (default) — bottom of form
```

### Recipe: Detail Page  
```
Layout: Full-width within shell
Top: PageHeader with title, status, breadcrumb, action buttons
Content: Descriptions component for key-value pairs
Sections: Tabs for logically grouped content, or card-based sections
Related data: Table within tab or below main content
Timeline: For activity/history log
```

### Recipe: Login Page
```
Layout: Centered or side-aligned (split-screen with illustration)
Flow: Email → Continue → Password → Log In (or SSO redirect)
Fields: Fluid inputs, stacked vertically
Primary CTA: Directly below last input, never separated by other elements
Alternate logins (SSO/Google): BELOW primary CTA, never above or between
Error: Generic "Incorrect username or password" (never reveal which)
Post-logout: Confirm message → redirect to login page
Post-timeout: Re-login redirects to LAST visited page
```

### Recipe: Empty State
```
Layout: Centered in the data area (same dimensions as populated state)
Content: Illustration (subtle, optional) + Title + Description + Primary CTA
First-use: "No [items] yet" + benefit description + "Create [Item]" button
No-results: "No results" + "Try adjusting search or filters" + "Clear filters" link
Error: Specific error + recovery action ("Retry" button)
Permissions: "No access" + who to contact + "Request Access" button
All-clear: Positive message ("All caught up!") + optional secondary action
```

### Recipe: Search Results Page
```
Top: Search field (full-width) with query preserved
Below: Result count ("23 results for 'servers'") + active filters as chips
Content: Results list (cards or rows) with highlighted matching terms
Sidebar (optional): Faceted filters for refinement
No results: Suggest alternatives, "Clear search" link, show recent searches
Loading: Skeleton states matching result layout + "Searching..." text
```

## Platform Adaptation Notes

This design system is platform-agnostic. When implementing:

- **React/Web**: Use Ant Design component library directly (`antd`), or Carbon React
  components for IBM-aligned projects.
- **WPF/XAML**: See `references/cross-platform-xaml.md` for detailed porting guide.
  Use WPF UI (Fluent) as base, apply design system spacing/color/typography system on top.
- **Angular**: Clarity Design System provides Angular-native components aligned with
  these same design principles.
- **Mobile**: Adapt to Ant Design Mobile patterns. Reduce information density. Ensure
  44x44px minimum touch targets. No compact density mode on touch devices.
- **Any platform**: The principles (spacing, hierarchy, color restraint, feedback patterns,
  token architecture, accessibility) are universal. Only the component implementation changes.

### Token Architecture is Platform-Agnostic

The three-tier token system (global → alias → component) works identically across platforms:
- **Web**: CSS Custom Properties (`--token-name`)
- **WPF/XAML**: ResourceDictionary with `SolidColorBrush` and `Thickness` resources
- **Mobile**: Theme constants or design token JSON files
- **Design tools**: Figma/Sketch variables mapped to the same token names

Theme switching (light ↔ dark, regular ↔ compact) becomes trivial when all components
reference tokens instead of hardcoded values.

## Final Principle

> "Don't make a decision before you figure it out."

When in doubt: less is more. Remove rather than add. The interface should disappear —
the user should see only their task, not your design.
