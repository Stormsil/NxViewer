# Visual Foundation — Color, Typography, Layout, Spacing, Shadow, Dark Mode

This document defines the visual building blocks of the Ant Design system. These rules create
visual order and ensure consistency across any interface.

---

## Color System

Ant Design's color system operates on two levels: system-level and product-level.
The attitude toward color in enterprise applications is **restrained**. Color serves
information delivery, operational guidance, and interactive feedback — never decoration.

### Color Model

Use the **HSB (Hue-Saturation-Brightness)** color model for design decisions.
HSB gives designers clear psychological expectations when adjusting colors and
facilitates team communication.

**HSL Alternative (recommended for code)**: In implementation, **HSL** (Hue-Saturation-Lightness)
is preferred because values are human-readable in code. Looking at three HSL numbers gives a
clear idea of the resulting color. Tints and shades stay pegged to a central hue. Calculating
complementary colors is simple (add/subtract 180° from hue). Hue shifting is also useful for
multi-color gradients and improving colorblind accessibility.

### Cool Grays (Enhanced Neutrals)

Pure neutral grays (0% saturation) appear dull and lifeless. Adding 2–5% saturation in the
brand hue direction creates "cool grays" that feel more vibrant and alive:

```
Neutral gray:  hsl(0, 0%, 90%)    → flat, clinical feel
Cool gray:     hsl(210, 5%, 90%)  → subtle warmth, modern feel
```

Use cool grays for all construction elements: borders, backgrounds, dividers, shadows,
and secondary text. The difference is nearly invisible in isolation but creates a
significantly more vibrant composition overall.

### System-Level: Base Color Palette

12 primary colors, each with 10 gradient steps (120 total colors):

| Color | Name | Mood/Association | Hex (Primary, Step 6) |
|-------|------|-----------------|----------------------|
| Red | Dust Red | Fighting spirit, passion | #F5222D |
| Volcano | Volcano | Eye-catching, surging | #FA541C |
| Orange | Sunset Orange | Warm, cheerful | #FA8C16 |
| Gold | Calendula Gold | Energetic, positive | #FAAD14 |
| Yellow | Sunrise Yellow | Birth, sunshine | #FADB14 |
| Lime | Lime | Natural, vitality | #A0D911 |
| Green | Polar Green | Healthy, innovative | #52C41A |
| Cyan | Cyan | Hope, strong | #13C2C2 |
| Blue | Daybreak Blue | Inclusive, technology | #1677FF |
| GeekBlue | Geek Blue | Exploration, research | #2F54EB |
| Purple | Golden Purple | Elegant, romantic | #722ED1 |
| Magenta | French Magenta | Bright, emotional | #EB2F96 |

Each color has 10 steps: step 1 (lightest, backgrounds) → step 6 (primary) → step 10 (darkest, pressed states).

### System-Level: Neutral Color Palette

10 shades of gray from near-white to near-black. Used for:
- Text (multiple hierarchy levels)
- Borders
- Dividers
- Backgrounds

### Product-Level: Brand Color

Choose a brand color from the base palette's step 6 as the primary accent.
Default Ant Design brand: **#1677FF** (Daybreak Blue-6).

Usage: key action buttons, active states, important information highlights, link text.

**Rule**: Brand color should appear sparingly. It marks what's important. If everything is blue,
nothing is important.

### Product-Level: Functional Colors

Semantic colors that MUST be consistent across the entire product:

| Function | Color | Usage |
|----------|-------|-------|
| Success | Green (#52C41A) | Completed operations, positive states |
| Error/Danger | Red (#FF4D4F) | Failed operations, destructive actions, errors |
| Warning | Gold/Orange (#FAAD14) | Attention needed, risky operations |
| Info/Processing | Blue (#1677FF) | Informational states, loading, links |

**Rule**: Never customize functional colors frivolously. Users associate these colors
universally with their meaning. Red = danger. Green = success. Violating this causes confusion.

### Product-Level: Neutral Colors

Based on transparency for maximum flexibility on different backgrounds:

| Purpose | Light Mode | Dark Mode |
|---------|-----------|-----------|
| Heading text | rgba(0,0,0,0.88) | rgba(255,255,255,0.85) |
| Body text | rgba(0,0,0,0.88) | rgba(255,255,255,0.85) |
| Secondary text | rgba(0,0,0,0.65) | rgba(255,255,255,0.65) |
| Disabled text | rgba(0,0,0,0.25) | rgba(255,255,255,0.25) |
| Border | #D9D9D9 | #424242 |
| Divider | rgba(5,5,5,0.06) | rgba(253,253,253,0.12) |
| Layout background | #F5F5F5 | #000000 |
| Component background | #FFFFFF | #141414 |

### Color Application Rules

1. **Restraint**: Use color based on information delivery, not aesthetics.
2. **Hierarchy**: Don't use more than 3 non-neutral colors on one screen.
3. **Consistency**: Same semantic meaning = same color everywhere.
4. **Accessibility**: WCAG AAA contrast ratio (7:1) for body text.
5. **Illustrations/Marketing**: Only illustrations and display pages may break restraint rules.
6. **Tint/Shade, Not Transparency**: Use dedicated tint and shade color values for hover/active
   states instead of rgba transparency. Transparent overlays produce unpredictable contrast
   on different backgrounds, making accessibility evaluation unreliable.
7. **Token Architecture**: Organize colors in three tiers: global tokens (raw palette) →
   semantic aliases (function-based, e.g. `status-success`) → component tokens (specific).
   See `enterprise-patterns-advanced.md` for the full token architecture.

---

## Typography System

### Font Family Strategy

Use the platform's native font stack for stability, performance, and familiarity:

```
font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 
  'Helvetica Neue', Arial, 'Noto Sans', sans-serif, 'Apple Color Emoji',
  'Segoe UI Emoji', 'Segoe UI Symbol', 'Noto Color Emoji';
```

For code: `'SFMono-Regular', Consolas, 'Liberation Mono', Menlo, Courier, monospace`

**Why system fonts?** They render natively, load instantly, feel familiar on each platform,
and handle CJK characters well.

### Font Size Scale

Base size: **14px** with line-height **22px** (ratio ≈ 1.57).

Recommended scale:

| Token | Size | Line-height | Usage |
|-------|------|-------------|-------|
| fontSizeSM | 12px | 20px | Help text, captions, table secondary |
| fontSize | 14px | 22px | Body text, form inputs, table cells |
| fontSizeLG | 16px | 24px | Subtitles, card titles |
| fontSizeXL | 20px | 28px | Page section titles |
| fontSizeHeading3 | 24px | 32px | Page titles |
| fontSizeHeading2 | 30px | 38px | Major headings |
| fontSizeHeading1 | 38px | 46px | Display/hero titles |

**Rule of Restraint**: In one design system, use 3–5 font sizes maximum. Each size must
have a clear semantic role. "Less is more" — don't add sizes for marginal visual differences.

### Font Weight

| Weight | Name | Usage |
|--------|------|-------|
| 400 | Regular | Body text, form inputs, table cells (95% of UI) |
| 500 | Medium | Subtitles, emphasis, selected states |
| 600 | Semibold | English headings (when extra punch is needed) |

**Rule**: Regular + Medium cover almost everything. Adding Bold or Light introduces
complexity with marginal benefit.

### Typography Rules

1. **Systematic thinking**: Plan heading, body, secondary, caption sizes upfront as a system.
   Don't decide font sizes ad-hoc per element.
2. **Contrast ratio**: Body text ≥ 7:1 (AAA) against background. Secondary text ≥ 4.5:1 (AA).
3. **Line length**: Optimal reading width is 45–75 characters per line (≈ 600px for 14px text).
4. **Paragraph spacing**: Between paragraphs, use the line-height value as the gap.

### Variable Line Heights (Enhanced)

A single fixed line-height is too rigid for enterprise applications. Provide three
line-height options per text size to serve different contexts:

| Mode | Body (14px) | When to Use |
|------|------------|-------------|
| **Standard** | 22px | Default. Standalone text, form labels, descriptions |
| **Compact** | 18px | Inside containers (table cells, cards, alerts, sidebars) |
| **Expanded** | 28px | Marketing pages, hero text, landing pages |

**Rule**: All line-heights must be divisible by 4 (aligns with 4px sub-grid).

**Compact rationale**: When text is inside a bounded container (a table cell, alert box, card),
the container boundary provides the visual grouping that generous line-height normally creates.
Power users prefer this density, and the structural context makes it equally readable.

**Expanded rationale**: When type is both the structure AND the content (landing pages, marketing),
vertical rhythm must be strong because there's no other device giving unity to the page.

### Letter Spacing (Tracking) Optimization

Adjust letter-spacing based on text size for optimal readability and space economy:

| Text size | Letter-spacing | Rationale |
|-----------|---------------|-----------|
| ≥ 24px (titles) | -0.3px to -0.5px | Tighten tracking on large sizes to save horizontal space |
| 14–20px (body) | 0 (default) | No adjustment needed at reading sizes |
| ≤ 12px (captions) | +0.2px to +0.5px | Open tracking on tiny sizes to aid legibility |

This is especially important for geometric typefaces with wide characters.

### Typography for Information Hierarchy

The primary purpose of having a variety of type sizes and weights is to assist users in
scanning content effectively. Select the most appropriate sizes and weights per screen
so users can quickly identify what they're looking for. Rules:

- Large classes (Display, Headline, Title) should mostly sit on single lines
- If a large size wraps to multiple lines, consider whether a smaller size is needed
- Each typographic element should sit solidly upon the baseline grid

---

## Layout System

### Design Board

Standard design width: **1440px**. 
Content area: **1168px** (within a 1440px container with margins).
Mainstream resolutions to support: 1920, 1440, 1366, 1280.

### Grid System

- **24-column grid** for fine-grained control.
- **Base unit**: 8px.
- **Gutter** (space between columns): Fixed width (16px or 24px recommended).
  Columns flex; gutters stay fixed.

Common column distributions:

| Layout | Columns | Usage |
|--------|---------|-------|
| Full width | 24 | Single content area |
| Main + Side | 16 + 8 or 18 + 6 | Content + sidebar |
| Three columns | 8 + 8 + 8 | Dashboard cards |
| Four columns | 6 + 6 + 6 + 6 | Statistic cards, gallery |

**Rule**: Content goes inside columns. Only columns go inside rows.
If column spans exceed 24 in a row, overflow starts a new line.

### Spacing System

The spacing system operates on TWO levels:

**Sub-grid**: **4px** is the atomic unit. All sizes and positions must be divisible by 4.
This ensures pixel-perfect alignment at any density level. Line-heights, paddings, margins,
and component dimensions should all snap to the 4px grid.

**Functional scale**: Built on 4px increments for component-internal spacing,
and 8px multiples for layout-level spacing:

| Token | Value | Usage |
|-------|-------|-------|
| marginXXS | 4px | Micro-spacing (icon-to-text, inline elements) |
| marginXS | 8px | Tight spacing (related elements within a group) |
| marginSM | 12px | Compact spacing |
| margin | 16px | Standard spacing (between sibling elements) |
| marginMD | 20px | Comfortable spacing |
| marginLG | 24px | Section spacing (between groups) |
| marginXL | 32px | Large section spacing |
| marginXXL | 48px | Page section spacing |

**Baseline grid**: 24px baseline for vertical rhythm. All major elements should align
to 24px increments vertically, creating harmonious layout rhythm.

**Two-tier spacing model** (from Clarity's architecture):
1. **Component-space tokens** — Control spacing WITHIN components (internal padding, gaps
   between icon and label). These tokens respond to density mode changes.
2. **Layout-space tokens** — Control spacing BETWEEN components (margins, section gaps).
   These are more stable and less affected by density changes.

Three tiers of visual hierarchy through spacing:
1. **8px (Small)**: Elements within a single logical group.
2. **16px (Medium)**: Between sibling groups at the same hierarchy level.
3. **24px (Large)**: Between major sections or categories.

### Adaptive Layout Patterns

**Left-Right Layout** (most common for apps):
- Fixed-width left sidebar navigation.
- Dynamic right content area scales with viewport.

**Top-Bottom Layout** (for portals/websites):
- Fixed-height top navigation.
- Define minimum margins on both sides.
- Center content area scales dynamically.

**Key rule for designers → developers communication**:
1. Define blocks by start column and end column (not by width alone).
2. Know the gutter value being used.
3. Distinguish "margin between columns" from "padding inside columns."
4. Annotate whether layout is fixed, fluid, or responsive.

---

## Shadow System

Shadows indicate elevation — the distance between an element and the surface below it.

### Shadow Principles

- **Higher = lighter shadow, more blur, larger spread.**
- **Lower = darker shadow, less blur, smaller spread.**
- Shadows follow the interface's conceptual light source.

### Shadow Direction by Context

| Direction | Usage |
|-----------|-------|
| Downward (↓) | Components and elements within the page (most common) |
| Upward (↑) | Bottom navigation bars, bottom toolbars |
| Leftward (←) | Right-side navigation, right-side Drawer, fixed right columns |
| Rightward (→) | Left-side navigation, left-side Drawer, fixed left columns |

### Shadow Tokens

| Token | Value | Usage |
|-------|-------|-------|
| boxShadow | 0 6px 16px 0 rgba(0,0,0,0.08), 0 3px 6px -4px rgba(0,0,0,0.12), 0 9px 28px 8px rgba(0,0,0,0.05) | Dropdown, Popover, Modal elevation |
| boxShadowSecondary | 0 1px 2px 0 rgba(0,0,0,0.03), 0 1px 6px -1px rgba(0,0,0,0.02), 0 2px 4px 0 rgba(0,0,0,0.02) | Card, subtle elevation |
| boxShadowTertiary | 0 1px 2px 0 rgba(0, 0, 0, 0.03), 0 1px 6px -1px rgba(0, 0, 0, 0.02), 0 2px 4px 0 rgba(0, 0, 0, 0.02) | Minimal elevation |

### Shadow Usage Rules

1. Shadows simulate real-world layering. Don't use them decoratively.
2. Fixed elements (fixed headers, sidebars) NEED shadows to indicate they float above content.
3. Dropdowns, popovers, modals are higher elevation = more prominent shadows.
4. Cards on a page use subtle shadow (or just border) to indicate they are distinct areas.
5. In dark mode, shadows are less effective. Use lighter background surfaces for elevation instead.

---

## Dark Mode

Dark mode darkens all UI elements. Recommended for low-light environments.

### When to Use

- User preference (always offer a toggle).
- Low-light environments (monitoring dashboards, media applications).
- Content-first displays (image galleries, video interfaces).

### Design Principles for Dark Mode

1. **Avoid pure black (#000000)**. Use dark gray (#141414 or similar) as the base background.
   Pure black causes extreme contrast fatigue and OLED "smearing."
2. **Layer with surface elevation**: Higher surfaces = lighter shade.
   - Base background: #000000 or #141414
   - Surface level 1 (cards): #1F1F1F
   - Surface level 2 (elevated): #2A2A2A
   - Surface level 3 (modals): #333333
3. **Reduce color saturation**: Bright, saturated colors on dark backgrounds cause visual vibration.
   Use desaturated versions of brand and functional colors.
4. **Text opacity**: Primary text at 85% white, secondary at 65%, disabled at 25%.
5. **Shadows are less visible**: Use surface color differences for elevation instead of shadows.
6. **Test contrast ratios**: Dark mode makes contrast issues more pronounced.

### Color Palette Adaptation

Ant Design provides an algorithm to auto-generate dark mode palettes from light mode palettes.
The principle: invert the brightness curve while adjusting saturation.

For functional colors in dark mode:
- Success: Slightly desaturated green.
- Error: Slightly desaturated red (avoid pure bright red on dark — it's harsh).
- Warning: Desaturated gold.
- Info: Desaturated blue.

---

## Motion & Animation

### Values

1. **Natural**: Animations follow physics. Objects have mass and inertia. Use ease-in-out curves.
2. **Performant**: Minimal transition time. Serve purpose in the most effective way.
3. **Concise**: Avoid dramatic or complicated animations. Change just enough to indicate the transition.

### Timing Guidelines

| Type | Duration | Example |
|------|----------|---------|
| Micro-interaction | 100–150ms | Button hover, toggle switch |
| Small transition | 150–250ms | Dropdown open, tooltip show |
| Medium transition | 250–350ms | Modal open, drawer slide |
| Large transition | 300–500ms | Page transition, route change |
| Disappearing | 60–80% of appearing duration | List items removed, notifications dismissed |

### Motion Rules

1. **Disappearing < Appearing**: Users don't need to study things leaving the screen.
   Exit animations should be faster with no stagger delays between items.
2. **One animation per transition**: Don't animate multiple properties independently.
   Compose them into a single choreographed motion.
3. **Don't animate for animation's sake**: An arrow icon rotating 180° when a menu opens
   is enough — it doesn't need to bounce, glow, or do backflips.
4. **Loading**: Use skeleton screens over spinner for content-heavy pages.
   Spinner for action feedback. Progress bar for quantifiable progress.
5. **Easing**: ease-in-out (natural) for most. ease-out for entering elements.
   ease-in for exiting elements. Never use linear.

---

## Icon System

### Guidelines

1. **Consistency**: All icons should share the same visual weight, line thickness, and style.
   Pick outlined OR filled, not a mix. Ant Design defaults to outlined with filled for selected states.
2. **Size**: Icons should be on the 8px grid. Common sizes: 14px (inline with text), 
   16px (in buttons/menus), 24px (standalone), 32px+ (feature illustrations).
3. **Meaning**: Icons should be universally recognized or paired with text labels.
   When in doubt, use text. An ambiguous icon is worse than no icon.
4. **Padding**: Icons in clickable contexts need padding to meet minimum click target (32x32px).
5. **Color**: Icons inherit the text color of their context by default.
   Active/selected icons use the brand color.
