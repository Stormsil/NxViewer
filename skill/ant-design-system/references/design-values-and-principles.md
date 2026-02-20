# Design Values & Principles — The Philosophical Foundation

This document is the soul of the Ant Design system. Every visual decision, every interaction
pattern, every component choice flows from these values and principles. Internalize them before
designing anything.

---

## The Four Design Values

### 1. Natural (自然)

The digital world is increasingly complex, but human cognitive resources are limited. Design
must bridge this gap by following natural patterns.

**Natural User Cognition**:
- ~80% of external information comes through visual channels. Visual design is paramount.
- Layout, colors, illustrations, icons should all follow natural cognitive laws.
- Reduce cognitive cost — interfaces should feel intuitive, not learned.
- Consider multi-sensory feedback where appropriate (sound, haptics for confirmations).

**Natural User Behavior**:
- Understand the triangle: User ↔ System Role ↔ Task Objective.
- Organize functions and services contextually — what the user needs, when they need it.
- Use behavior analysis to assist decisions and reduce unnecessary operations.
- Save mental and physical resources. Make human-computer interaction invisible.

**Design Implication**: When you design a screen, ask "Would a user who has never seen this
before understand what to do within 3 seconds?" If not, it's not natural enough.

### 2. Certain (确定)

Uncertainty is the enemy of productive work. Interfaces must provide high certainty with
low cooperative entropy.

**Designer Certainty** — Reduce the chaos of collaborative design:
- **Keep Restraint**: Focus on the most valuable features with minimal design elements.
  "Perfection is achieved, not when there is nothing more to add, but when there is nothing
  left to take away." — Antoine de St. Exupéry
- **Object-Oriented Design**: Abstract design rules as reusable "objects" — color systems,
  spacing scales, component patterns. This reduces subjective judgment.
- **Modular Design**: Encapsulate complex or reusable parts. Provide limited, clean
  interfaces between modules. This reduces system complexity and improves reliability.

**User Certainty** — Consistency across everything:
- Same patterns within a single product.
- Same patterns across products in a suite.
- Same patterns across terminals (web, mobile, desktop).
- Familiarity reduces learning cost, cognitive cost, and operating cost.

**Design Implication**: If you've used a pattern on page A, use the same pattern on page B.
If the design system has a component for it, use that component. Don't reinvent.

### 3. Meaningful (有意义)

Products exist to serve user missions, not designer egos. Every element must earn its place.

**Meaning of Result** — Clear goals, immediate feedback:
- Understand the user's main objective for this screen.
- Decompose into sub-objectives along the use process.
- Every interaction should revolve around achieving the main objective.
- Provide appropriate, immediate feedback for every action.
- Use emotional design to calm negative emotions, enhance positive ones.

**Meaning of Process** — Moderate challenge, full devotion:
- Adjust difficulty contextually. Match function triggers to user skill level.
- "If not necessary, do not add entities" (Occam's Razor for UI).
- Don't distract. Let users focus on the task, not the interface.
- Neither too simple (boring) nor too complex (overwhelming).
- As user capability grows, present higher challenges progressively.
- Goal: keep users in "flow state" — fully immersed in productive work.

**Design Implication**: Before adding any element, ask "Does this help the user complete
their mission?" If the answer is unclear, remove it.

### 4. Growing (成长)

Products and users evolve together. Design for the long game.

**Value Connection**:
- Product growth depends on expanding user base and deeper feature usage.
- User growth depends on product capability growth.
- Designers must understand product function value AND user scenario needs.
- Build connections between product value and user requirements.
- Make product value discoverable. Help users build efficient workflows.

**Human-Computer Symbiosis**:
- Users and systems should not be treated separately.
- They form a dynamic group — flexible, inclusive, full of vitality.
- Design progressively: simple entry → advanced mastery.
- Never lock users into beginner mode or overwhelm them with expert complexity.

**Design Implication**: Design onboarding that grows with the user. Day-1 and Day-100
should feel like the same product but offer different depths.

---

## The 10 Interaction Principles

These principles guide HOW users interact with interfaces. They are derived from
cognitive psychology, Gestalt theory, and decades of HCI research.

### Principle 1: Proximity

**Law**: When items are close together, they form a visual unit. Distance implies separation.

**Rules**:
- Three spacing levels: 8px (small — related items), 16px (medium — sibling groups), 24px (large — sections)
- Formula: y = 8 + 8n, where n ≥ 0
- Related form fields? Small spacing. Different field groups? Medium spacing. Separate sections? Large spacing.
- Add visual guides (lines, backgrounds) to reinforce grouping when spacing alone is ambiguous.

**Application**:
- In a form: label + input = small gap. Input group A and Input group B = medium gap.
  Form section 1 and Form section 2 = large gap (or card boundary).
- In a dashboard: KPI value + KPI label = small gap. KPI card A and KPI card B = medium gap (gutter).

### Principle 2: Alignment

**Law** (Gestalt Continuity): Humans perceive aligned elements as related and ordered.

**Rules**:
- Left-align text by default. It creates a strong visual anchor ("river of white space" on the right is fine).
- Center-align only for symmetrical, decorative elements (hero sections, empty states).
- Right-align numbers in columns for easy comparison.
- In forms, choose ONE alignment system: top-aligned labels OR left-aligned labels. Never mix.
- Every element should visually align to at least one other element on the screen.

**Application**:
- Form labels: top-aligned is fastest for scanning. Left-aligned with colon is traditional.
- Table: text columns left-aligned, number columns right-aligned, status columns center-aligned.
- Dashboard cards: ensure titles, values, and descriptions all share alignment edges.

### Principle 3: Contrast

**Law**: Contrast creates hierarchy. Without strong contrast, everything looks equal and nothing stands out.

**Rules**:
- Contrast must be STRONG to be effective. Subtle contrast is worse than no contrast.
- Methods: size difference, color difference, weight difference, spatial difference.
- Primary actions: high-contrast (filled button, brand color). Secondary: medium-contrast (outlined).
- Status indicators: use distinct, high-contrast colors (green/red/orange/blue — never pastel).
- Interactive states (hover, active, disabled) must be clearly differentiated.

**Application**:
- "Submit" button = Primary (filled, brand color). "Cancel" = Default (outlined, neutral).
- In a table, the row being hovered gets a distinct background color change.
- Page title = large + bold. Section title = medium + medium-weight. Body = base size + regular.

### Principle 4: Repetition

**Law**: Repeating visual elements across the interface reduces learning cost and signals relationships.

**Rules**:
- Same element type = same visual treatment everywhere.
- Repeated patterns: consistent line weights, consistent icon style, consistent card structure,
  consistent button placement, consistent color usage for semantics.
- A "Delete" button should look the same on every screen.
- A "status badge" should use the same color coding everywhere.

**Application**:
- If page A uses a Card with a title bar and a body, page B's cards should have the same structure.
- If error states are red on the settings page, they must be red on the form page.
- Navigation patterns must be identical across all pages.

### Principle 5: Make it Direct

**Law** (Alan Cooper): "Where there is output, let there be input."

This is the principle of direct manipulation. Users should be able to act on objects directly,
not through intermediate interfaces.

**Patterns**:
- **In-page editing**: Click-to-edit text fields. Click a value → it becomes an input → edit → save.
  - Mode 1: Click on the text itself (no visible edit affordance until hover).
  - Mode 2: Explicit "Edit" link/icon next to the text (more discoverable).
- **Multi-field inline edit**: Toggle an entire section into edit mode.
- **Drag and drop**: Reorder items, move between lists, file upload by dragging.

**Key rule**: If "readability" is the priority, use click-to-edit. If "editability" is the
priority, show persistent form fields.

### Principle 6: Stay on the Page

**Law**: Page redirects cause change blindness and break mental flow.

**Rules**:
- Solve most problems on the SAME page. Avoid navigating away.
- Use overlays (Modal, Drawer) for complex sub-tasks that need focus.
- Use inline expansion (Collapse, expandable rows) for progressive detail.
- Use virtual pages (Tabs within content) for multi-view without navigation.
- Use process flows (Steps component) for multi-step tasks on one page.

**Application**:
- Editing a table row? Use a Drawer or inline editing. Don't navigate to a new page.
- Creating a new item? Use a Modal form. Only use a dedicated page for very complex forms.
- Viewing details? Consider expandable rows or a side panel before a full detail page.

### Principle 7: Keep it Lightweight (Fitts's Law)

**Law** (Fitts): The time to reach a target depends on its size and distance from the cursor.

**Rules**:
- **Always-visible tools**: Primary actions should always be visible with clear clickable areas.
  Hover states should change cursor to hand, button fill darkens — clear call to action.
- **Hover-reveal tools**: Secondary actions can appear on hover (e.g., row actions in a table).
- **Toggle-reveal tools**: Infrequent actions can be revealed via mode toggle (e.g., edit mode).
- **Visible area ≠ Clickable area**: The clickable target should be LARGER than the visible element.
  Pad click targets generously. Minimum 32x32px touch target for accessibility.

**Application**:
- Table row actions: Show "Edit" and "Delete" on hover, not always.
- Button in a toolbar: Full-width clickable area, not just the text.
- Hyperlink text in a table cell: Entire cell can be clickable, not just the text string.

### Principle 8: Provide an Invitation

**Law**: Rich interactions are useless if users can't discover them.

**Invitations** tell users "you can do something here" before they interact.

**Static Invitations** (always visible):
- Descriptive text: "Click to edit", "Drag to reorder".
- Affordance cues: Draggable handles, resize cursors, underlined links.
- Empty state CTAs: "No items yet. Create your first one →"

**Dynamic Invitations** (appear contextually):
- On hover: Cursor change, element highlight, tooltip.
- On focus: Input field hint text, contextual help.
- On state change: Animation drawing attention to new affordance.

**Application**:
- A sortable table should show sort icons in column headers.
- A droppable zone should highlight when a draggable item is near.
- An empty dashboard should show a clear guide on how to add widgets.

### Principle 9: Use Transition

**Law**: Motion communicates change and maintains spatial awareness.

**Rules**:
- **Maintain context during view changes**: Slide transitions show spatial relationship.
  Expand/collapse maintains awareness of parent-child relationship.
- **Explain what just happened**: After an action, a brief animation shows the result
  (item slides out of list on delete, new item fades in on create).
- **Improve perceived performance**: Skeleton loading, progressive content reveal.
- **Natural motion**: Follow physics. Use ease-in-out. Avoid linear motion.
  Objects accelerate from rest and decelerate to rest.

**Timing guidelines**:
- Micro-interactions (hover, toggle): 100–200ms
- Content transitions (expand, slide): 200–300ms  
- Page transitions: 300–500ms
- Disappearing animations should be faster than appearing animations (users don't need
  to study something that's going away).

### Principle 10: React Immediately

**Law** (Newton's Third): For every action, there is an equal and opposite reaction.

**Rules**:
- Every user action MUST produce immediate visual feedback. Zero exceptions.
- Button click → visual depression immediately, then action result.
- Typing → characters appear instantly (or loading indicator if processing).
- Mistake → error indication appears inline, immediately.
- A system with no feedback feels "sluggish and thickheaded."

**Patterns**:
- **Auto Complete**: As user types, matching results appear in dropdown.
- **Live Suggest**: Real-time search term suggestions.
- **Live Validation**: Password strength, format validation as user types.
- **Live Preview**: Show result preview before submission.
- **Lookup**: Instant search results (certain category or uncertain category display).

**Application**:
- Form field with email format: Validate on blur, show green checkmark or red error immediately.
- Search field: Show results after 300ms debounce, with loading indicator.
- Password field: Show strength meter updating in real-time.

---

## Applying Values to Design Decisions

When you face any design decision, run it through this filter:

| Question | Value |
|----------|-------|
| Would a new user understand this in 3 seconds? | Natural |
| Is this consistent with how we've done it elsewhere? | Certain |
| Does this element help the user complete their mission? | Meaningful |
| Will this still work when the user becomes an expert? | Growing |
| Will this still work when we add 10 more features? | Growing |
| Can I remove this without losing function? | Certain (Restraint) |
| Is the feedback immediate and appropriate? | Meaningful + React Immediately |
