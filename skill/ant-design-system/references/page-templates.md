# Page Templates — Form, List, Detail, Workbench, Dashboard, Result, Exception

This document provides battle-tested templates for the most common enterprise page types.
Each template has been verified through thousands of production applications.

---

## Form Page

### Design Goals
Guide users to complete data entry efficiently with minimal errors.

### Core Principles
1. **Single-column vertical layout** is the fastest for task completion (verified by research).
   Users scan top-to-bottom naturally. Multi-column forms within one section are PROHIBITED.
2. **When space is limited**: Only group SHORT, RELATED fields on one line (e.g., First Name + Last Name).
3. **Progressive disclosure**: Show advanced fields only when needed.
4. **Reduce input**: Use smart defaults, auto-complete, and data inheritance where possible.

### Layout Methods

**Flat Layout** (default):
- All fields in a single vertical column within one container.
- Best for: Simple forms (login, contact, settings) with < 10 fields.

**In-Area Grouping**:
- Fields within one container, separated by group titles.
- Best for: Medium forms (10–20 fields) with logical categorization.

**Card Grouping**:
- Each category gets its own Card container with a title.
- Best for: Complex forms (> 20 fields, > 2 screens) with clear categories.
- Each card should have a meaningful title. "Basic Information", "Configuration", "Permissions."

### Choosing Layout Method

| Information Complexity | Information Relevance | Layout |
|----------------------|---------------------|---------|
| Low (< 10 fields) | Highly related | Flat (single area) |
| Medium (10–20 fields) | Some grouping | In-area grouping with titles |
| High (> 20 fields) | Multiple categories | Card grouping |
| Very high + sequential | Dependent steps | Step-by-step wizard |

### Form Structure Checklist

1. **Title/Header**: Clear description of what this form does.
2. **Required indicators**: Mark required fields with asterisk (*) next to label.
3. **Labels**: Concise, unambiguous. Top-aligned for scanning speed, left-aligned for density.
4. **Placeholder text**: Use for format hints ("YYYY-MM-DD"), NOT as a replacement for labels.
5. **Help text**: Below the field. For format requirements, limits, or explanations.
6. **Validation**: Inline, real-time. Show error message directly below the field.
7. **Actions**: At the bottom of the form. Primary action left ("Submit"), secondary right ("Cancel").
   Or primary right and secondary left — be consistent across the app.
8. **Confirmation for destructive forms**: Show a summary before final submission for irreversible actions.

### Step-by-Step Form (Wizard)

Use when:
- Form has > 3 logical sections with dependencies.
- Later sections depend on earlier inputs.
- Users benefit from seeing progress.

Structure:
```
[Steps Component: Step 1 → Step 2 → Step 3 → Complete]
        ↓
[Current Step Content — form fields]
        ↓
[Navigation: "Previous" (default) | "Next" (primary) | "Save Draft"]
```

Rules:
- Users should be able to go back to previous steps.
- Show a summary/review step before final submission.
- Validate each step before allowing "Next."
- Save progress automatically or offer "Save Draft."

---

## List Page

### Design Goals
Allow users to view, search, filter, compare, and act on a large number of entries.
Provide clear navigation to detail pages.

### Core Principles
1. **Readability + Operability** are the two keys.
2. Show only essential information in the list; fold details into the detail page.
3. Progressive access: List → Preview → Full Detail.

### Layout Pattern

```
[Page Header: Title + Primary Action Button ("Create New")]
[Filter Area: Search bar + filter controls]
[Data Area: Table / Card Grid / List]
[Pagination: Bottom, with total count]
```

### List Variants

**Table List** (most common):
- Use when: Each entry has many fields to compare. Users have precise search criteria.
- Structure: Sortable columns, row hover actions, batch actions toolbar.
- Columns should be prioritized left-to-right by importance.
- Right-most column: Action buttons (Edit, Delete, More...).
- For tables with many columns: Allow column toggle, horizontal scroll, or fixed key columns.

**Card List** (visual):
- Use when: Entries have a visual component (images, thumbnails). 
  Order doesn't matter. Browsing > searching.
- Structure: Grid of cards (3–4 per row). Each card has image, title, key metadata, actions.
- Add item: An "Add New" card at the beginning or end of the grid.

**Simple List**:
- Use when: Entries are simple (just a title and basic metadata).
- Structure: Vertical list with avatar/icon, title, description, meta.
- Good for: Activity logs, notifications, message lists.

**Search Results List**:
- Use when: Results span multiple content types (articles, files, users).
- Structure: Search bar prominent at top. Results grouped by type with type labels.
- Highlight matched search terms in results.

### Filter Design

**Top Filter Bar** (default):
- Place above the table/list.
- Stack top-to-bottom: search bar on first row, filter dropdowns on second row.
- Collapse filters behind "More Filters" if > 3 filter controls.

**Side Filter Panel**:
- Use when: Many filter dimensions AND ample horizontal space.
- Fixed left panel with filter groups in a sidebar.

### Empty State

ALWAYS design the empty state. It should:
1. Have a clear illustration or icon.
2. Explain WHY there's no data ("No projects yet" not just "No data").
3. Provide a CTA to create the first item or adjust filters.

### Batch Operations

For operations on multiple items:
1. Checkbox column on the left.
2. "Select All" in the header checkbox.
3. Batch action bar appears when items are selected (floating above table or inline).
4. Show selected count: "3 items selected".
5. Batch actions: "Delete", "Export", "Assign", etc.

---

## Detail Page

### Design Goals
Display complete information about an entity. Enable editing and related operations.
Increase information viewing and searching efficiency.

### Core Principles
1. **Prioritize information by importance**: Most critical info first, details progressively.
2. **Clear structure**: Users should be able to scan and find specific info quickly.
3. **Action accessibility**: Edit, Delete, Status Change should be accessible but not overwhelming.

### Basic Layout

```
[Breadcrumb: List > Item Name]
[Page Header: Title + Status Badge + Action Buttons]
[Overview: Key-value pairs via Descriptions component]
[Content Sections: Cards or Tabs]
[Related Data: Table of related entities]
[Activity Log: Timeline component]
```

### Content Organization

**Basic Details** (Descriptions component):
- Use for key-value pairs: "Name: John", "Status: Active", "Created: 2024-01-01".
- Layout: 2–3 columns for horizontal Descriptions.
- Group related fields visually.

**Sections**:
- **Flat sections** (Card per section): When sections are independent.
- **Tab sections**: When sections are parallel views of the same entity
  (e.g., "Overview", "Settings", "History", "Permissions").
- **Collapse sections**: When some sections are usually not needed (progressive disclosure).

### Complex Layout

For very complex entities:
- Use a **two-column layout**: Left column (16-span) for primary content, 
  Right column (8-span) for sidebar information (metadata, related items, activity).
- Or use **anchor navigation**: A mini-nav on the right side that scrolls to sections.

### Action Design

- **Primary action** (e.g., "Edit"): Top-right of Page Header.
- **Status change** (e.g., "Approve", "Reject"): Top-right as button group.
- **Destructive action** (e.g., "Delete"): In a "More" dropdown. Requires confirmation.
- **Inline edit**: Click-to-edit for quick updates. Full edit page for complex changes.

---

## Workbench (Homepage/Dashboard)

### Design Goals
Serve as the landing page and command center. Provide shortcuts to frequent tasks,
surface important information, and guide users to relevant sections.

### Core Principles
1. **Understand core goals**: Why does the user return to this page? Provide shortest paths.
2. **Hub-and-spoke navigation**: From the workbench, users navigate to various modules.
3. **Information freshness**: Show what needs attention NOW.

### Workbench Modules

| Module | Purpose | Position |
|--------|---------|----------|
| Help/Quick Start | Guide new users | Top, dismissible |
| Core Data/KPIs | Key metrics at a glance | Prominent top area |
| Shortcuts | Frequent actions as icon buttons | Top or sidebar |
| To-Do List | Items needing user action | Central area |
| Focus/Attention | Notifications, alerts, approvals | Central area |
| Operational/News | Product updates, announcements | Lower area or sidebar |

### Layout Pattern

```
[Top Area]
  [Welcome Message / Quick Start Guide (dismissible for returning users)]
  [KPI Cards: 3-4 Statistic components in a row]

[Middle Area (Main)]
  [Left Column (16-span): To-Do List / Recent Activity / Key Data Table]
  [Right Column (8-span): Shortcuts / Calendar / Announcements]

[Bottom Area (Optional)]
  [Charts / Trends / Additional analytics]
```

### Design Suggestions

1. **Don't overload**: The workbench is a hub, not a destination. Keep it scannable.
2. **Personalize**: Show data relevant to the logged-in user's role and history.
3. **Progressive complexity**: New users see guidance. Power users see data.
4. **Update frequency**: Real-time for critical data. Periodic for analytics.
5. **Actions, not just displays**: Let users act directly from the workbench 
   (approve, assign, quick-create) without navigating away.

---

## Result Page

### Design Goals
Communicate the outcome of a user-initiated process clearly and guide next steps.

### Patterns

**Success Result**:
```
[Success Icon (green checkmark)]
[Title: "Operation Successful"]
[Description: Brief summary of what was accomplished]
[Actions: "Back to List" (default) | "Create Another" (primary)]
[Optional: Key details of the created/updated entity]
```

**Failure Result**:
```
[Error Icon (red X)]
[Title: "Operation Failed"]
[Description: What went wrong, in user-friendly language]
[Actions: "Try Again" (primary) | "Back to List" (default)]
[Optional: Error details in expandable section]
```

**Processing Result**:
```
[Processing Icon (blue spinner or clock)]
[Title: "Submitting..."]
[Description: "Your request is being processed. This may take a few minutes."]
[Actions: "View Progress" | "Back to List"]
```

### Rules
1. **Always provide a clear next step**. Never leave the user at a dead end.
2. **Error messages must be actionable**. "Something went wrong" is useless.
   "Your session expired. Please log in again." is actionable.
3. **Celebrate success briefly**. Don't over-celebrate. Acknowledge and move on.

---

## Exception Page

### Design Goals
Handle error states (404, 403, 500) gracefully with clear communication and recovery paths.

### Patterns

| Code | Title | Description | Actions |
|------|-------|-------------|---------|
| 403 | No Permission | You don't have access to this page. | "Request Access" / "Go Home" |
| 404 | Page Not Found | The page you're looking for doesn't exist. | "Go Back" / "Go Home" |
| 500 | Server Error | Something went wrong on our end. We're working on it. | "Retry" / "Go Home" |

### Rules
1. Use a distinctive illustration or icon — not just a text error code.
2. Explain in plain language (not HTTP codes).
3. Always provide at least one navigation action.
4. For 500 errors: reassure the user. "We're aware and working on it."
5. Log the actual error for developers; show the friendly version for users.

---

## Visualization Page

### Design Goals
Present data visually for analysis and decision-making.

### Principles
1. **Effective**: Charts should answer a specific question. Don't visualize for decoration.
2. **Clear**: Use appropriate chart types. Bar for comparison, line for trend, pie for composition.
3. **Accurate**: Don't mislead. Start axes at zero unless there's good reason not to.
4. **Beautiful**: Clean, uncluttered, consistent with the design system's color palette.

### Layout Pattern
```
[Header: Title + Date Range Selector + Filters]
[Key Metrics Row: 3-4 Statistic/KPI cards]
[Primary Chart: Full-width main visualization]
[Secondary Charts: 2-column grid of supporting charts]
[Data Table: Raw data below charts (collapsible)]
```

### Rules
1. Use AntV's data visualization color palette for chart colors.
2. Always include legend, axis labels, and units.
3. Interactive: hover for tooltips with exact values.
4. Responsive: charts should resize gracefully.
5. Loading state: skeleton chart shape while data loads.
