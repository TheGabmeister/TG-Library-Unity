use std::collections::BTreeMap;
use std::path::PathBuf;
use std::sync::mpsc::{self, Receiver};

use eframe::egui;

use crate::package::{self, ExtractMessage};

// ── Color Palette ──────────────────────────────────────────────────────────

const BG_DARK: egui::Color32 = egui::Color32::from_rgb(12, 12, 18);
const BG_CARD: egui::Color32 = egui::Color32::from_rgb(20, 20, 28);
const BG_ELEVATED: egui::Color32 = egui::Color32::from_rgb(30, 30, 42);
const BG_TERMINAL: egui::Color32 = egui::Color32::from_rgb(8, 8, 12);
const BORDER: egui::Color32 = egui::Color32::from_rgb(42, 42, 58);
const BORDER_SUBTLE: egui::Color32 = egui::Color32::from_rgb(32, 32, 45);
const TEXT_PRIMARY: egui::Color32 = egui::Color32::from_rgb(228, 228, 238);
const TEXT_SECONDARY: egui::Color32 = egui::Color32::from_rgb(125, 125, 150);
const TEXT_DIM: egui::Color32 = egui::Color32::from_rgb(80, 80, 100);
const ACCENT: egui::Color32 = egui::Color32::from_rgb(99, 102, 241);
const ACCENT_GLOW: egui::Color32 = egui::Color32::from_rgb(22, 22, 45);
const SUCCESS: egui::Color32 = egui::Color32::from_rgb(52, 211, 113);
const WARNING: egui::Color32 = egui::Color32::from_rgb(251, 191, 36);
const ERROR: egui::Color32 = egui::Color32::from_rgb(248, 88, 88);

// ── Theme Setup ────────────────────────────────────────────────────────────

pub fn setup_theme(ctx: &egui::Context) {
    let mut visuals = egui::Visuals::dark();

    visuals.panel_fill = BG_DARK;
    visuals.window_fill = BG_DARK;
    visuals.extreme_bg_color = BG_TERMINAL;
    visuals.faint_bg_color = BG_CARD;

    let r = egui::CornerRadius::same(6);

    visuals.widgets.noninteractive.bg_fill = BG_CARD;
    visuals.widgets.noninteractive.weak_bg_fill = BG_CARD;
    visuals.widgets.noninteractive.bg_stroke = egui::Stroke::new(1.0, BORDER_SUBTLE);
    visuals.widgets.noninteractive.fg_stroke = egui::Stroke::new(1.0, TEXT_SECONDARY);
    visuals.widgets.noninteractive.corner_radius = r;

    visuals.widgets.inactive.bg_fill = BG_ELEVATED;
    visuals.widgets.inactive.weak_bg_fill = BG_ELEVATED;
    visuals.widgets.inactive.bg_stroke = egui::Stroke::new(1.0, BORDER);
    visuals.widgets.inactive.fg_stroke = egui::Stroke::new(1.0, TEXT_PRIMARY);
    visuals.widgets.inactive.corner_radius = r;

    visuals.widgets.hovered.bg_fill = egui::Color32::from_rgb(44, 44, 64);
    visuals.widgets.hovered.weak_bg_fill = egui::Color32::from_rgb(44, 44, 64);
    visuals.widgets.hovered.bg_stroke = egui::Stroke::new(1.0, ACCENT);
    visuals.widgets.hovered.fg_stroke = egui::Stroke::new(1.5, egui::Color32::WHITE);
    visuals.widgets.hovered.corner_radius = r;

    visuals.widgets.active.bg_fill = ACCENT;
    visuals.widgets.active.weak_bg_fill = ACCENT;
    visuals.widgets.active.bg_stroke = egui::Stroke::new(0.0, egui::Color32::TRANSPARENT);
    visuals.widgets.active.fg_stroke = egui::Stroke::new(1.0, egui::Color32::WHITE);
    visuals.widgets.active.corner_radius = r;

    visuals.widgets.open.bg_fill = egui::Color32::from_rgb(28, 28, 42);
    visuals.widgets.open.weak_bg_fill = egui::Color32::from_rgb(28, 28, 42);
    visuals.widgets.open.bg_stroke = egui::Stroke::new(1.0, BORDER);
    visuals.widgets.open.fg_stroke = egui::Stroke::new(1.0, TEXT_PRIMARY);
    visuals.widgets.open.corner_radius = r;

    visuals.selection.bg_fill = egui::Color32::from_rgb(50, 52, 110);
    visuals.selection.stroke = egui::Stroke::new(1.0, ACCENT);

    visuals.window_corner_radius = egui::CornerRadius::same(10);
    visuals.window_stroke = egui::Stroke::new(1.0, BORDER);

    ctx.set_visuals(visuals);

    let mut style = (*ctx.style()).clone();
    style.spacing.item_spacing = egui::vec2(8.0, 6.0);
    style.spacing.button_padding = egui::vec2(14.0, 6.0);
    ctx.set_style(style);
}

// ── Helpers ────────────────────────────────────────────────────────────────

fn card_frame() -> egui::Frame {
    egui::Frame::NONE
        .fill(BG_CARD)
        .stroke(egui::Stroke::new(1.0, BORDER_SUBTLE))
        .corner_radius(10)
        .inner_margin(16.0)
}

fn section_heading(ui: &mut egui::Ui, label: &str) {
    ui.label(
        egui::RichText::new(label.to_uppercase())
            .size(10.5)
            .strong()
            .color(TEXT_DIM),
    );
    ui.add_space(6.0);
}

fn progress_bar(ui: &mut egui::Ui, fraction: f32, text: &str) {
    let desired = egui::vec2(ui.available_width(), 26.0);
    let (rect, _) = ui.allocate_exact_size(desired, egui::Sense::hover());
    let painter = ui.painter();
    let rounding = egui::CornerRadius::same(13);

    painter.rect_filled(rect, rounding, BG_ELEVATED);

    if fraction > 0.0 {
        let w = (rect.width() * fraction.clamp(0.0, 1.0)).max(rect.height());
        let fill = egui::Rect::from_min_size(rect.min, egui::vec2(w, rect.height()));
        painter.rect_filled(fill, rounding, ACCENT);
    }

    painter.text(
        rect.center(),
        egui::Align2::CENTER_CENTER,
        text,
        egui::FontId::proportional(11.0),
        egui::Color32::WHITE,
    );
}

// ── Tree ───────────────────────────────────────────────────────────────────

struct TreeNode {
    children: BTreeMap<String, TreeNode>,
}

impl TreeNode {
    fn new() -> Self {
        Self {
            children: BTreeMap::new(),
        }
    }

    fn insert(&mut self, path: &str) {
        let parts: Vec<&str> = path.split('/').filter(|p| !p.is_empty()).collect();
        self.insert_parts(&parts);
    }

    fn insert_parts(&mut self, parts: &[&str]) {
        if parts.is_empty() {
            return;
        }
        let child = self
            .children
            .entry(parts[0].to_string())
            .or_insert_with(TreeNode::new);
        if parts.len() > 1 {
            child.insert_parts(&parts[1..]);
        }
    }

    fn show(&self, ui: &mut egui::Ui, depth: usize) {
        for (name, node) in &self.children {
            if node.children.is_empty() {
                ui.horizontal(|ui| {
                    ui.add_space(20.0);
                    ui.label(egui::RichText::new(name).size(12.5).color(TEXT_SECONDARY));
                });
            } else {
                egui::CollapsingHeader::new(
                    egui::RichText::new(name).size(12.5).color(TEXT_PRIMARY),
                )
                .default_open(depth < 1)
                .show(ui, |ui| {
                    node.show(ui, depth + 1);
                });
            }
        }
    }
}

// ── Log Level ──────────────────────────────────────────────────────────────

#[derive(Clone, Copy)]
enum LogLevel {
    Info,
    Success,
    Warning,
    Error,
}

// ── App State ──────────────────────────────────────────────────────────────

pub struct App {
    package_path: Option<PathBuf>,
    output_dir: String,
    include_meta: bool,

    entries: Vec<String>,
    preview_loading: bool,
    preview_rx: Option<Receiver<Result<Vec<String>, String>>>,
    preview_error: Option<String>,

    extracting: bool,
    extract_rx: Option<Receiver<ExtractMessage>>,
    progress: (usize, usize),
    log: Vec<(LogLevel, String)>,

    done: bool,
    final_output_dir: Option<PathBuf>,
}

impl Default for App {
    fn default() -> Self {
        Self {
            package_path: None,
            output_dir: String::new(),
            include_meta: true,
            entries: Vec::new(),
            preview_loading: false,
            preview_rx: None,
            preview_error: None,
            extracting: false,
            extract_rx: None,
            progress: (0, 0),
            log: Vec::new(),
            done: false,
            final_output_dir: None,
        }
    }
}

// ── Logic ──────────────────────────────────────────────────────────────────

impl App {
    fn load_file(&mut self, path: PathBuf) {
        if let Some(parent) = path.parent() {
            let stem = path
                .file_stem()
                .map(|s| s.to_string_lossy().to_string())
                .unwrap_or_else(|| "output".to_string());
            self.output_dir = parent.join(stem).to_string_lossy().to_string();
        }

        self.package_path = Some(path.clone());
        self.entries.clear();
        self.preview_error = None;
        self.extracting = false;
        self.extract_rx = None;
        self.progress = (0, 0);
        self.log.clear();
        self.done = false;
        self.final_output_dir = None;

        let (tx, rx) = mpsc::channel();
        self.preview_rx = Some(rx);
        self.preview_loading = true;

        std::thread::spawn(move || {
            let result = package::list_contents(&path);
            let _ = tx.send(result);
        });
    }

    fn start_extraction(&mut self) {
        let Some(ref pkg_path) = self.package_path else {
            return;
        };
        let output = PathBuf::from(&self.output_dir);

        self.extracting = true;
        self.done = false;
        self.log.clear();
        self.progress = (0, 0);
        self.final_output_dir = Some(output.clone());

        let (tx, rx) = mpsc::channel();
        self.extract_rx = Some(rx);

        let pkg = pkg_path.clone();
        let meta = self.include_meta;

        std::thread::spawn(move || {
            package::extract(&pkg, &output, meta, tx);
        });
    }

    fn poll_channels(&mut self) {
        if let Some(rx) = &self.preview_rx {
            if let Ok(result) = rx.try_recv() {
                match result {
                    Ok(entries) => {
                        self.entries = entries;
                        self.preview_error = None;
                    }
                    Err(e) => {
                        self.preview_error = Some(e);
                        self.entries.clear();
                    }
                }
                self.preview_loading = false;
                self.preview_rx = None;
            }
        }

        if let Some(rx) = &self.extract_rx {
            let mut finished = false;
            while let Ok(msg) = rx.try_recv() {
                match msg {
                    ExtractMessage::Progress {
                        current,
                        total,
                        path,
                    } => {
                        self.progress = (current, total);
                        self.log.push((LogLevel::Info, path));
                    }
                    ExtractMessage::Warning(w) => {
                        self.log.push((LogLevel::Warning, w));
                    }
                    ExtractMessage::Error(e) => {
                        self.log.push((LogLevel::Error, e));
                    }
                    ExtractMessage::Done {
                        total,
                        errors,
                        warnings,
                    } => {
                        self.progress = (total, total);
                        let (level, status) = if errors > 0 {
                            (
                                LogLevel::Error,
                                format!("Completed with {errors} errors, {warnings} warnings"),
                            )
                        } else if warnings > 0 {
                            (
                                LogLevel::Success,
                                format!("Done! {total} files extracted ({warnings} warnings)"),
                            )
                        } else {
                            (
                                LogLevel::Success,
                                format!("Done! {total} files extracted successfully"),
                            )
                        };
                        self.log.push((level, status));
                        self.extracting = false;
                        self.done = true;
                        finished = true;
                    }
                }
            }
            if finished {
                self.extract_rx = None;
            }
        }
    }
}

// ── Rendering ──────────────────────────────────────────────────────────────

impl eframe::App for App {
    fn update(&mut self, ctx: &egui::Context, _frame: &mut eframe::Frame) {
        self.poll_channels();

        if self.preview_loading || self.extracting {
            ctx.request_repaint();
        }

        let dropped = ctx.input(|i| i.raw.dropped_files.clone());
        if let Some(file) = dropped.first() {
            if let Some(ref path) = file.path {
                if path.extension().map_or(false, |e| e == "unitypackage") {
                    self.load_file(path.clone());
                }
            }
        }

        let hovering = ctx.input(|i| !i.raw.hovered_files.is_empty());

        egui::CentralPanel::default()
            .frame(egui::Frame::NONE.fill(BG_DARK).inner_margin(24.0))
            .show(ctx, |ui| {
                egui::ScrollArea::vertical()
                    .id_salt("main_scroll")
                    .show(ui, |ui| {
                        self.render_header(ui);
                        ui.add_space(20.0);
                        self.render_drop_zone(ui, hovering);

                        if self.preview_loading {
                            ui.add_space(14.0);
                            ui.horizontal(|ui| {
                                ui.spinner();
                                ui.label(
                                    egui::RichText::new("Scanning package contents...")
                                        .size(13.0)
                                        .color(TEXT_SECONDARY),
                                );
                            });
                        }

                        if let Some(err) = self.preview_error.clone() {
                            ui.add_space(14.0);
                            ui.label(
                                egui::RichText::new(format!("Error: {err}"))
                                    .size(13.0)
                                    .color(ERROR),
                            );
                        }

                        if !self.entries.is_empty() {
                            ui.add_space(20.0);
                            self.render_preview(ui);
                            ui.add_space(20.0);
                            self.render_settings(ui);
                            ui.add_space(16.0);
                            self.render_extract_button(ui);
                        }

                        if self.extracting || self.done {
                            ui.add_space(20.0);
                            self.render_progress_section(ui);
                        }
                    });
            });
    }
}

impl App {
    fn render_header(&self, ui: &mut egui::Ui) {
        ui.horizontal(|ui| {
            ui.label(egui::RichText::new("//").size(28.0).strong().color(ACCENT));
            ui.add_space(6.0);
            ui.vertical(|ui| {
                ui.add_space(2.0);
                ui.label(
                    egui::RichText::new("Unity Package Extractor")
                        .size(20.0)
                        .strong()
                        .color(TEXT_PRIMARY),
                );
                ui.label(
                    egui::RichText::new("Preview and extract .unitypackage files")
                        .size(12.0)
                        .color(TEXT_DIM),
                );
            });
        });
    }

    fn render_drop_zone(&mut self, ui: &mut egui::Ui, hovering: bool) {
        let pkg_name = self
            .package_path
            .as_ref()
            .and_then(|p| p.file_name())
            .map(|n| n.to_string_lossy().to_string());

        let bg = if hovering { ACCENT_GLOW } else { BG_CARD };
        let stroke = if hovering {
            egui::Stroke::new(1.5, ACCENT)
        } else {
            egui::Stroke::new(1.0, BORDER_SUBTLE)
        };

        egui::Frame::NONE
            .fill(bg)
            .stroke(stroke)
            .corner_radius(12)
            .inner_margin(egui::Margin::symmetric(24, 28))
            .show(ui, |ui| {
                ui.set_width(ui.available_width());
                ui.vertical_centered(|ui| {
                    if let Some(ref name) = pkg_name {
                        ui.label(
                            egui::RichText::new(name)
                                .size(15.0)
                                .strong()
                                .color(TEXT_PRIMARY),
                        );
                        ui.add_space(8.0);
                        if styled_button(ui, "Change File", false) {
                            self.browse_file();
                        }
                    } else if hovering {
                        ui.label(
                            egui::RichText::new("Release to open")
                                .size(16.0)
                                .strong()
                                .color(ACCENT),
                        );
                    } else {
                        ui.label(
                            egui::RichText::new("Drag & drop a .unitypackage file")
                                .size(14.0)
                                .color(TEXT_SECONDARY),
                        );
                        ui.add_space(10.0);
                        if styled_button(ui, "Browse Files", true) {
                            self.browse_file();
                        }
                    }
                });
            });
    }

    fn browse_file(&mut self) {
        if let Some(path) = rfd::FileDialog::new()
            .add_filter("Unity Package", &["unitypackage"])
            .pick_file()
        {
            self.load_file(path);
        }
    }

    fn render_preview(&self, ui: &mut egui::Ui) {
        card_frame().show(ui, |ui| {
            ui.set_width(ui.available_width());

            section_heading(ui, "Contents");

            ui.label(
                egui::RichText::new(format!("{} entries", self.entries.len()))
                    .size(12.0)
                    .color(TEXT_DIM),
            );
            ui.add_space(8.0);

            let mut tree = TreeNode::new();
            for path in &self.entries {
                tree.insert(path);
            }

            egui::ScrollArea::vertical()
                .id_salt("preview_tree")
                .max_height(200.0)
                .show(ui, |ui| {
                    tree.show(ui, 0);
                });
        });
    }

    fn render_settings(&mut self, ui: &mut egui::Ui) {
        card_frame().show(ui, |ui| {
            ui.set_width(ui.available_width());

            section_heading(ui, "Output");

            ui.horizontal(|ui| {
                let text_width = (ui.available_width() - 90.0).max(100.0);
                ui.add(
                    egui::TextEdit::singleline(&mut self.output_dir).desired_width(text_width),
                );
                if styled_button(ui, "Browse", false) {
                    if let Some(folder) = rfd::FileDialog::new().pick_folder() {
                        self.output_dir = folder.to_string_lossy().to_string();
                    }
                }
            });

            ui.add_space(4.0);
            ui.checkbox(&mut self.include_meta, "Include .meta files");
        });
    }

    fn render_extract_button(&mut self, ui: &mut egui::Ui) {
        let can_extract = !self.extracting && !self.output_dir.is_empty();

        ui.vertical_centered(|ui| {
            let button = egui::Button::new(
                egui::RichText::new(if self.extracting {
                    "Extracting..."
                } else {
                    "Extract Package"
                })
                .size(15.0)
                .strong()
                .color(if can_extract {
                    egui::Color32::WHITE
                } else {
                    TEXT_DIM
                }),
            )
            .fill(if can_extract { ACCENT } else { BG_ELEVATED })
            .corner_radius(8)
            .min_size(egui::vec2(220.0, 42.0));

            if ui.add_enabled(can_extract, button).clicked() {
                self.start_extraction();
            }
        });
    }

    fn render_progress_section(&self, ui: &mut egui::Ui) {
        card_frame().show(ui, |ui| {
            ui.set_width(ui.available_width());

            section_heading(
                ui,
                if self.done {
                    "Complete"
                } else {
                    "Extracting"
                },
            );

            let (current, total) = self.progress;
            if total > 0 {
                let fraction = current as f32 / total as f32;
                progress_bar(ui, fraction, &format!("{current} / {total}"));
            }

            ui.add_space(10.0);

            // Terminal-style log
            egui::Frame::NONE
                .fill(BG_TERMINAL)
                .stroke(egui::Stroke::new(1.0, BORDER_SUBTLE))
                .corner_radius(6)
                .inner_margin(10.0)
                .show(ui, |ui| {
                    ui.set_width(ui.available_width());
                    egui::ScrollArea::vertical()
                        .id_salt("log")
                        .max_height(220.0)
                        .stick_to_bottom(true)
                        .show(ui, |ui| {
                            for (level, msg) in &self.log {
                                let color = match level {
                                    LogLevel::Info => TEXT_DIM,
                                    LogLevel::Success => SUCCESS,
                                    LogLevel::Warning => WARNING,
                                    LogLevel::Error => ERROR,
                                };
                                ui.label(egui::RichText::new(msg).size(11.5).color(color));
                            }
                        });
                });

            if self.done {
                ui.add_space(12.0);
                ui.vertical_centered(|ui| {
                    if styled_button(ui, "Open Output Folder", false) {
                        if let Some(ref dir) = self.final_output_dir {
                            open_folder(dir);
                        }
                    }
                });
            }
        });
    }
}

// ── Styled Button ──────────────────────────────────────────────────────────

fn styled_button(ui: &mut egui::Ui, label: &str, primary: bool) -> bool {
    let button = if primary {
        egui::Button::new(egui::RichText::new(label).size(13.0).color(egui::Color32::WHITE))
            .fill(ACCENT)
            .corner_radius(6)
    } else {
        egui::Button::new(egui::RichText::new(label).size(13.0).color(TEXT_PRIMARY)).corner_radius(6)
    };
    ui.add(button).clicked()
}

// ── Open Folder ────────────────────────────────────────────────────────────

fn open_folder(path: &std::path::Path) {
    #[cfg(target_os = "windows")]
    {
        let _ = std::process::Command::new("explorer").arg(path).spawn();
    }
    #[cfg(target_os = "macos")]
    {
        let _ = std::process::Command::new("open").arg(path).spawn();
    }
    #[cfg(target_os = "linux")]
    {
        let _ = std::process::Command::new("xdg-open").arg(path).spawn();
    }
}
