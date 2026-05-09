# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Design System

The UI uses a warm editorial aesthetic — not dark, not bright. The palette and typography are intentional; do not swap them for defaults.

### CSS Variables (`css/site.css`)

| Variable | Value | Role |
|---|---|---|
| `--bg` | `#F2EDE4` | Page background (warm ivory) |
| `--surface` | `#FDFAF6` | Card background |
| `--surface-2` | `#F7F2EA` | Inset areas (currency prefix, footer strip) |
| `--border` | `#E0D6C8` | Default borders |
| `--green` | `#1E4D35` | Primary action colour (button, focus ring, highlights) |
| `--green-hover` | `#2A6647` | Button hover state |
| `--green-pale` | `#EBF2EE` | Result panel background |
| `--copper` | `#B87333` | Secondary accent ($ prefix, eyebrow label, footer accent) |
| `--error` | `#C0392B` | Error text and border |
| `--error-pale` | `#FDEEEC` | Error panel background |

### Typography

- **Display / headings**: `Playfair Display` (Google Fonts) — used for `.card-title`, `.brand-name`, `.currency-prefix`, `.result-text`
- **UI / body**: `Plus Jakarta Sans` (Google Fonts) — used for everything else

Both are loaded in `Views/Shared/_Layout.cshtml`. Do not add a third font family.

### JS-Coupled Selectors

These IDs and classes are referenced directly in `js/site.js` — renaming them breaks runtime behaviour:

| Selector | Purpose |
|---|---|
| `#Amount` | Input field — value read, `aria-invalid` toggled, focused on load |
| `#btnSubmit` | Convert button — `disabled` prop toggled during AJAX |
| `.btn-text` | Text span inside button — text swapped to "Converting…" during AJAX |
| `#resultPanel` | Result container — `.show()` / `.hide()` |
| `#responseAmount` | Result text — `.empty()` then rebuilt word-by-word |
| `#responseError` | Error message — `.text(msg)` set directly |
| `.word-highlight` | Applied to scale words (MILLION, HUNDRED, etc.) in result |

### Layout Notes

- `.card::before` — the 4px green→copper gradient left border; purely decorative, no HTML element
- `.error-message:not(:empty)` — the error panel only gains its background/border when it has content; the div is always present in the DOM (required for `aria-live`)
- `white-space: nowrap` on `.card-title` keeps "Money to Word" on one line — do not remove
