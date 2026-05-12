# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
cargo build                  # dev build
cargo build --release        # release build
cargo run                    # build and launch the app
```

No tests or linting are configured yet.

## Architecture

Desktop GUI app (egui/eframe) that extracts `.unitypackage` files to disk. Three modules:

- **main.rs** — Entry point. Creates the 900x700 eframe window, applies the custom dark theme via `app::setup_theme()`, and launches the app.
- **app.rs** — UI and state management. Renders the egui interface with a custom dark color palette (indigo accent `#6366F1`), card-based layout, and custom progress bar. Drives two background operations (preview loading, extraction) via `std::sync::mpsc` channels polled each frame with `try_recv()`.
- **package.rs** — Extraction engine. Uses a two-pass algorithm over the gzip-compressed tar archive:
  1. `scan_archive()` — Reads all `pathname` entries to build a GUID-to-path map and flags which GUIDs have asset data.
  2. `extract()` — Re-reads the archive, streaming each `asset`/`asset.meta` entry to disk using the pathname map from pass 1. Reports progress via `ExtractMessage` sent over an mpsc channel.

This two-pass design avoids buffering all asset data in memory, which matters for large packages (hundreds of MB).

## egui API notes

This project uses eframe 0.33 (egui 0.33). Key API details for this version:

- `Rounding` has been renamed to `CornerRadius`. Use `CornerRadius::same(u8)` (takes `u8`, not `f32`).
- Widget visuals use the field `corner_radius`, not `rounding`.
- Use `Frame::NONE` instead of the deprecated `Frame::none()`.
- Use `.corner_radius()` instead of `.rounding()` on `Frame` and `Button`.
- `Margin::symmetric(i8, i8)` takes `i8` values, but `Frame::inner_margin(f32)` accepts `f32`.
- eframe 0.34 has a `windows-core` version conflict in its wgpu dependency tree on Windows; 0.33 is the latest that builds cleanly.

## .unitypackage format

A `.unitypackage` is a `.tar.gz` where each asset lives under a GUID directory:

```
<guid>/pathname    — text file: asset path on line 1, Unity metadata on line 2
<guid>/asset       — the actual file bytes
<guid>/asset.meta  — Unity import settings
```

**Important:** The `pathname` file has two lines. Only the first line is the asset path. Always use `.lines().next()` when reading it — using `trim()` alone leaves the second-line metadata embedded in the path, which breaks on Windows (OS error 123).
