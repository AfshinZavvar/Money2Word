---
version: alpha
name: Money2Word
description: A precise, compact conversion slip in mineral gray and ink blue.
colors:
  background: "#E9EDF1"
  surface: "#FFFFFF"
  output: "#F4F6F8"
  border: "#CBD3DC"
  control: "#7B8A9A"
  text: "#253344"
  secondary: "#59697B"
  primary: "#345D87"
  hover: "#284C71"
  active: "#203E5D"
  error: "#A33040"
typography:
  body:
    fontFamily: "'Segoe UI', 'Helvetica Neue', sans-serif"
    fontSize: "1rem"
    lineHeight: "1.5"
  heading:
    fontFamily: "'Bahnschrift', 'DIN Alternate', 'Segoe UI', sans-serif"
    fontSize: "1.625rem"
    lineHeight: "1.25"
  data:
    fontFamily: "'Consolas', 'Liberation Mono', monospace"
    fontSize: "1.0625rem"
    lineHeight: "1.5"
rounded:
  surface: "0.625rem"
  control: "0.375rem"
spacing:
  small: "0.75rem"
  medium: "1.5rem"
  large: "2rem"
  page: "38rem"
components:
  converter:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.text}"
    rounded: "{rounded.surface}"
    padding: "{spacing.large}"
  button:
    backgroundColor: "{colors.primary}"
    textColor: "{colors.surface}"
    rounded: "{rounded.control}"
    height: "3rem"
  input:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.text}"
    typography: "{typography.data}"
    rounded: "{rounded.control}"
  output:
    backgroundColor: "{colors.output}"
    textColor: "{colors.text}"
    typography: "{typography.body}"
---

# Money2Word design

## Overview

A compact conversion slip: the precision of a currency engraving, reduced to an everyday tool. A person enters dollars and cents and receives uppercase English words. This is a product surface, not a landing page. The README, InputModel, service and API are the business authorities; no conversion rules change in this redesign.

The audience uses English dollar amounts on desktop or phone; no country-specific dollar denomination is implied. There is no Japanese locale or Japan-specific market requirement. The interface has one action and no marketing, navigation, or redundant brand strip.

The signature is three close, short engraved rules across the boundary between figures and words. Everything else serves reading and input. The initial idea included a denomination badge and a second brand label; critique removed both because they repeated information and made a simple tool feel ornamental. Warm editorial colors, serif headlines, glows, gradients, glass, oversized heroes, pill controls and animated decoration are anti-references.

Runtime ownership is **Model B**: `Money2Word/wwwroot/css/site.css` owns tokens; this file mirrors accepted values and intent. No theme adapter or independent token system is introduced. Update both in the same change and verify the mapping below. The unused scaffold `_Layout.cshtml.css` is not linked by the runtime layout.

## Colors

Mineral gray background (`--bg`), white entry surface (`--surface`), and a quieter gray output (`--surface-2`) establish depth. Slate text and muted ink blue carry hierarchy. Use blue for action, keyboard focus and scale-word emphasis. Red is reserved for an error, always accompanied by text and `aria-invalid` where relevant. This is an intentionally light-only interface; forced-colors mode uses system focus and scrollbar colors.

## Typography

Heading: locally available Bahnschrift/DIN, 26px, medium weight, compact tracking. Body and output: Segoe UI/Helvetica Neue sans serif, 16px. Data: Consolas/Liberation Mono with tabular figures, 17px desktop and 16px mobile. Utility labels: 13–14px. The complete system uses local fonts, eliminating network font swaps. Fallback sans serif and monospace faces must remain usable.

Results retain the API's uppercase English, at 1.85 line height with restrained tracking; scale/currency words use blue semibold, never italics. Long amounts wrap naturally without clipping or internal scrolling. No typography scales with viewport width.

## Layout

Single column, maximum 608px. A short heading sits above one joined entry/output surface. At widths above 560px, input and action share a row; at or below 560px the button moves below the input and spans its width. Horizontal page padding is 16px mobile / 20px desktop. Surface padding is 32px desktop, 24px mobile, 16px on phones at or below 360px.

The page is anchored from the top rather than vertically centered, so long results grow downward without moving the form. Normal document scrolling owns overflow. The output has a 72px minimum content reserve. The inline error reserves 56px for two lines, avoiding normal validation shifts. Primary controls are at least 48px high. Classic scrollbars reserve a stable gutter. On screens at or below 360px, the amount uses tighter tracking and 8px inner padding to keep the maximum value visible at 16px text size.

## Elevation & Depth

A barely visible monochrome grain gives the mineral background material depth. One 2px low-opacity contact shadow separates the surface from the background; no floating shadows, blur, or lighting gradients. The output's tonal contrast and boundary convey its relationship to the entry.

## Shapes

10px outer radius; 6px input and button radii. Borders are single pixels. The input border is darker than the static container border to meet control contrast requirements. Focus uses a 2px ink-blue outline, separated by 3px. The sole currency motif is a 48px engraved-rule detail at the output boundary.

## Components

### Canonical UI Map

| Capability | Canonical owner | Source of truth | Allowed variants | Verification |
| --- | --- | --- | --- | --- |
| Form | Index.cshtml native form + site.js | README + InputModel + this document | single converter, novalidate, decimal keyboard, formatting, first invalid focus | ConverterPageTests |
| Feedback | Persistent output live region + #responseError | this document | polite results/loading, assertive errors | ConverterPageTests |
| Scrollbar | site.css global baseline | this document | standards + WebKit fallback, forced-colors auto | browser computed styles |
| Conversion | Existing API/controller/service | README + InputModel + Money2WordService | $0.01–$999,999,999,999,999.99, 15 whole digits and two decimals | unit + browser tests |

There are no tables, selects, dates, CRUD, overlays, sessions, or sibling conversion flows. The shared error page uses the same surface, typography and return-link language. A separate UX-CONTRACT is unnecessary for this single workflow.

### States and behavior

- Empty: an unfocused amount field and an honest placeholder in the output area. Avoid autofocus so mobile keyboards do not open on arrival.
- Focus: high-contrast outline; labels focus the field; input → button is the natural keyboard order.
- Hover/pressed: primary blue deepens without transforms or moving borders.
- Loading: disabled button retains its grid width/height, label reads “Converting…”, arrow space remains reserved, `aria-busy` is true, and the persistent status announces progress. Duplicate Enter submissions are blocked.
- Success: placeholder gives way to the full result. All existing IDs and `.word-highlight` remain. Input edits clear old output; responses for an edited amount are ignored.
- Validation: entered value stays, inline text explains correction, field is marked invalid and receives focus. Editing clears the old error. API/network/timeout failures retain a retryable form and existing problem-details parsing.
- Motion: none is needed. Reduced-motion overrides protect this decision if later shared styling introduces motion.

### Runtime token mapping

| Document tokens | CSS owner | Consumers |
| --- | --- | --- |
| colors.background/surface/output | --bg / --surface / --surface-2 | body, converter, result |
| colors.border/control | --border / --border-control | surface, input, engraving |
| colors.text/secondary | --text-primary / --text-secondary | content, labels, hints, busy button |
| colors.primary/hover/active | --primary / --primary-hover / --primary-active | button, focus, emphasis, links |
| colors.error | --error | inline errors, invalid field |
| typography.body/heading/data | --font-body / --font-heading / --font-data | body + result / title / amount |
| type sizes | --text-body / --text-heading / --text-small | 16px body, 26px heading, 13px utility |
| rounded.surface/control | --radius / --radius-control | container / controls |
| spacing.small/medium/large/page | --space-sm / --space-md / --space-lg / --page-max | gaps, padding, page width |
| scrollbar state aliases | --scrollbar-thumb/track/hover/active | global owned scroll regions |

Scrollbar values map to control/background/secondary/primary respectively. Component recipes consume CSS variables; there are no duplicated theme adapters. Verify these values against the `:root` block, lint this document, run the strict Premium audit and capture browser states after visual changes.

## Do's and Don'ts

- Do keep one compact action and a complete, selectable result.
- Do preserve grouping, cents, full range, error recovery and Enter submission.
- Do let the result grow naturally at narrow widths and enlarged text sizes.
- Don't add a slogan, denomination badge, repetitive range footer or decorative logo.
- Don't add external font loading, result entrance animation, color-only errors or truncation.

