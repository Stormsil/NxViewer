# Copywriting, Data Format & Empty States

This document covers the textual and data presentation aspects of the design system.
Good copywriting is invisible — users read, understand, and act without noticing the text.
Bad copywriting creates friction, confusion, and frustration.

---

## Copywriting Principles

### Language

**1. Articulate Foothold — Write from the User's Perspective**

All text should center on what the USER can do, not what the SYSTEM does.

| ❌ System-centered | ✅ User-centered |
|-------------------|-----------------|
| "We provide cloud storage" | "Your files, backed up automatically" |
| "The system will send a notification" | "You'll get a notification when it's ready" |
| "Error: process terminated" | "Something went wrong. Try again?" |

**Exception**: When the user is reporting problems, use "we" to show ownership:
"We'll look into this" / "We're working on fixing this."

**2. Concise Statement — Say More with Less**

| ❌ Verbose | ✅ Concise |
|-----------|----------|
| "You are required to fill in your email address in order to proceed" | "Enter your email to continue" |
| "The operation has been completed successfully" | "Done!" or "Saved successfully" |
| "Are you sure you want to delete this item? This action cannot be undone." | "Delete this item? This can't be undone." |

Rule: Every word must earn its place. If you can remove a word without losing meaning, remove it.

**3. Use Words Familiar to the User**

Avoid technical jargon unless your audience is technical.

| ❌ Technical | ✅ Familiar |
|------------|-----------|
| "Authentication failed" | "Wrong email or password" |
| "Invalid input format" | "Please enter a valid email" |
| "Connection timeout" | "Couldn't connect. Check your internet." |
| "400 Bad Request" | "Something's not right. Please check your input." |

**4. Express Consistently**

On the same page, in the same area:
- Use the **same word order** pattern.
- Use the **same grammar** (all imperative, all declarative — don't mix).
- Use the **same terms** for the same concept (don't say "delete" here and "remove" there
  for the same action — pick one and use it everywhere).
- **Operation name must match the target page title**:
  If a button says "Settings", the page it opens must be titled "Settings" (not "Preferences").

**5. Place Important Information First**

In limited space, the most important word goes first.

| ❌ Important info buried | ✅ Important info first |
|------------------------|----------------------|
| "Enter your password. Note: minimum 8 characters." | "Minimum 8 characters for your password." |
| "After completing, click Submit." | "Click Submit when done." |

### Tone

**1. Bring Each Other Closer**
- Address the user directly: "You" not "The user."
- Be conversational but not overly casual.
- Match the tone to the context (error page = empathetic, success = brief celebration).

**2. Be Friendly and Respectful**
- Don't blame the user for errors: ❌ "You entered an invalid email" → ✅ "That doesn't look like a valid email."
- Don't be condescending: ❌ "Obviously, you need to fill all required fields."
- Don't use ALL CAPS for emphasis (it reads as shouting).

**3. Don't Be Too Extreme**
- Don't over-promise: ❌ "Your data is 100% safe" → ✅ "We use encryption to protect your data."
- Don't be overly dramatic about errors: ❌ "CRITICAL ERROR!" → ✅ "Something went wrong."
- Don't over-celebrate: ❌ "AMAZING! YOU DID IT! 🎉🎉🎉" → ✅ "All set!"

---

## Data Format Standards

### Numerical Data

| Type | Format | Example |
|------|--------|---------|
| Integer | No decimal, thousand separator | 1,234,567 |
| Decimal | Fixed decimal places | 1,234.56 |
| Percentage | Number + % sign | 45.2% |
| Large numbers | Abbreviate with unit | 1.2M, 3.5K |
| Approximate | "≈" or "~" prefix | ~1,200 |
| Range | En-dash between values | 100–200 |

Rules:
- Right-align numbers in columns for easy comparison.
- Use consistent decimal places within the same context.
- Thousand separators improve readability for numbers > 999.

### Amount / Currency

| Scenario | Format | Example |
|----------|--------|---------|
| Standard | Currency symbol + number | $1,234.56 |
| Negative | Negative sign before symbol | -$1,234.56 |
| Range | Symbol on first value | $100–$200 |
| Abbreviated | Symbol + abbreviated number | $1.2M |

Rules:
- Currency symbol before the number (for USD, EUR, etc.).
- Always show 2 decimal places for money.
- Use the user's locale for formatting when possible.

### Date & Time

| Type | Format | Example |
|------|--------|---------|
| Full date | YYYY-MM-DD | 2024-01-15 |
| Date (readable) | MMM D, YYYY | Jan 15, 2024 |
| Date + time | YYYY-MM-DD HH:mm:ss | 2024-01-15 14:30:00 |
| Time only | HH:mm | 14:30 |
| Relative | Smart relative | "Just now", "5 min ago", "2 hours ago", "Yesterday" |
| Duration | Human readable | "3 hours 25 minutes" or "3h 25m" |

**Relative Time Rules**:
- < 1 minute: "Just now"
- 1–59 minutes: "X minutes ago"
- 1–23 hours: "X hours ago"
- 1 day: "Yesterday"
- 2–7 days: "X days ago"
- > 7 days: Show actual date
- Show exact time on hover (tooltip) for relative times.

### Data Redaction (Masking)

For sensitive information:

| Type | Pattern | Example |
|------|---------|---------|
| Phone | Show last 4 digits | ****-****-1234 |
| Email | Show first/last chars + domain | j****n@gmail.com |
| ID number | Show last 4 digits | ****-****-5678 |
| Bank card | Show last 4 digits | **** **** **** 5678 |

Rules:
- Default to redacted. Offer "Show" toggle for authorized users.
- "Click to reveal" or eye icon to toggle visibility.

### Data Status Indicators

| State | Visual | Meaning |
|-------|--------|---------|
| Success | Green dot/badge | Completed, active, approved |
| Error | Red dot/badge | Failed, rejected, expired |
| Warning | Orange/yellow dot/badge | Needs attention, expiring soon |
| Processing | Blue dot/badge or spinner | In progress, pending |
| Default | Gray dot/badge | Inactive, draft, archived |

Rules:
- Always use color + text label (not color alone — colorblind accessibility).
- Status badges should be consistent across the entire application.
- Position: Usually after the item name or in a dedicated "Status" column.

---

## Empty States

### Design Goals
Empty states are NOT dead ends. They are opportunities to guide, educate, and encourage.

### Types of Empty States

**1. First-Time/No Data**
The user hasn't created anything yet.
```
[Illustration: Relevant to the content type]
[Title: "No [Items] Yet"]
[Description: Brief explanation of what this area is for]
[CTA: "Create Your First [Item]" — Primary button]
```

**2. No Results (Search/Filter)**
The user searched or filtered but nothing matches.
```
[Illustration: Search-related]
[Title: "No Results Found"]
[Description: "Try adjusting your search or filter criteria."]
[Actions: "Clear Filters" link / Suggestions for better search terms]
```

**3. Error State**
Something went wrong loading data.
```
[Illustration: Error-related]
[Title: "Couldn't Load Data"]
[Description: "There was a problem loading this page."]
[CTA: "Try Again" — Primary button]
```

**4. Completed/No Pending**
All tasks are done (e.g., empty inbox, all to-dos completed).
```
[Illustration: Celebration or calm]
[Title: "All Caught Up!"]
[Description: "You've handled everything. Nice work."]
```

### Empty State Rules

1. **Always provide an illustration or icon** — a blank white area is confusing.
2. **Explain WHY it's empty** — "No data" is useless. "You haven't created any projects yet" is helpful.
3. **Provide a clear action** when possible — a button to create, a link to import, a suggestion to search differently.
4. **Match the tone** — first-time empty states should be encouraging. Error empty states should be empathetic.
5. **Don't blame the user** — ❌ "You have no items" → ✅ "No items yet."
6. **Keep it compact** — empty states shouldn't be louder than actual content.

---

## Message & Feedback Copywriting

### Success Messages

| Context | Message Style |
|---------|--------------|
| Save operation | "Changes saved" / "Saved successfully" |
| Create operation | "[Item name] created" |
| Delete operation | "[Item name] deleted" |
| Send operation | "Message sent" |

### Error Messages

Structure: **What happened** + **What to do about it**

| ❌ Bad | ✅ Good |
|-------|--------|
| "Error" | "Couldn't save your changes. Try again." |
| "Invalid input" | "Email address isn't valid. Check the format." |
| "403" | "You don't have permission for this. Contact your admin." |
| "Network error" | "Can't connect to the server. Check your internet." |

### Confirmation Messages

For destructive actions:
```
Title: "Delete [item name]?"
Description: "This action cannot be undone. All associated data will be permanently removed."
Actions: [Cancel] [Delete] (danger button)
```

Rules:
- Be specific about what will be deleted/changed.
- Mention if the action is irreversible.
- Use the danger button type for the destructive action.
- Default focus should be on the safe option (Cancel).

### Loading Messages

| Duration | Message |
|----------|---------|
| < 3 seconds | No message needed (just show spinner) |
| 3-10 seconds | "Loading..." or "Processing..." |
| 10-30 seconds | "This may take a moment..." |
| 30+ seconds | "Processing your request. You can continue working." (with background processing) |
