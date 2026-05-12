use std::collections::BTreeMap;
use std::path::PathBuf;
use std::sync::mpsc::{self, Receiver};

use eframe::egui;

use crate::package::{self, ExtractMessage};

#[derive(Clone, Copy)]
enum LogLevel {
    Info,
    Warning,
    Error,
}

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
                    ui.add_space(18.0);
                    ui.label(name);
                });
            } else {
                egui::CollapsingHeader::new(name)
                    .default_open(depth < 1)
                    .show(ui, |ui| {
                        node.show(ui, depth + 1);
                    });
            }
        }
    }
}

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
                        let status = if errors > 0 {
                            format!(
                                "Completed with errors: {errors} errors, {warnings} warnings"
                            )
                        } else if warnings > 0 {
                            format!("Done! {total} files extracted ({warnings} warnings)")
                        } else {
                            format!("Done! {total} files extracted successfully")
                        };
                        self.log.push((LogLevel::Info, status));
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

        egui::CentralPanel::default().show(ctx, |ui| {
            ui.heading("Unity Package Extractor");
            ui.add_space(8.0);

            // --- File Selection ---
            let pkg_name = self
                .package_path
                .as_ref()
                .and_then(|p| p.file_name())
                .map(|n| n.to_string_lossy().to_string());

            let frame = if hovering {
                egui::Frame::group(ui.style())
                    .stroke(egui::Stroke::new(2.0, egui::Color32::LIGHT_BLUE))
            } else {
                egui::Frame::group(ui.style())
            };

            frame.show(ui, |ui| {
                ui.set_min_height(50.0);
                ui.set_width(ui.available_width());
                ui.vertical_centered(|ui| {
                    if let Some(ref name) = pkg_name {
                        ui.strong(name);
                    } else if hovering {
                        ui.strong("Drop here!");
                    } else {
                        ui.label("Drag & drop a .unitypackage file here");
                    }
                    if ui.button("Browse...").clicked() {
                        if let Some(path) = rfd::FileDialog::new()
                            .add_filter("Unity Package", &["unitypackage"])
                            .pick_file()
                        {
                            self.load_file(path);
                        }
                    }
                });
            });

            // --- Loading ---
            if self.preview_loading {
                ui.add_space(8.0);
                ui.horizontal(|ui| {
                    ui.spinner();
                    ui.label("Loading package contents...");
                });
            }

            // --- Error ---
            if let Some(ref err) = self.preview_error {
                ui.add_space(8.0);
                ui.colored_label(
                    egui::Color32::from_rgb(255, 80, 80),
                    format!("Error: {err}"),
                );
            }

            // --- Preview Tree ---
            if !self.entries.is_empty() {
                ui.add_space(8.0);
                ui.label(format!("Contents ({} entries):", self.entries.len()));

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

                ui.add_space(8.0);
                ui.separator();

                // --- Output Settings ---
                ui.horizontal(|ui| {
                    ui.label("Output:");
                    let text_width = (ui.available_width() - 80.0).max(100.0);
                    ui.add(
                        egui::TextEdit::singleline(&mut self.output_dir)
                            .desired_width(text_width),
                    );
                    if ui.button("Browse...").clicked() {
                        if let Some(folder) = rfd::FileDialog::new().pick_folder() {
                            self.output_dir = folder.to_string_lossy().to_string();
                        }
                    }
                });

                ui.checkbox(&mut self.include_meta, "Include .meta files");
                ui.add_space(4.0);

                let can_extract = !self.extracting && !self.output_dir.is_empty();
                ui.add_enabled_ui(can_extract, |ui| {
                    if ui.button("Extract").clicked() {
                        self.start_extraction();
                    }
                });
            }

            // --- Progress & Log ---
            if self.extracting || self.done {
                ui.add_space(8.0);
                ui.separator();

                let (current, total) = self.progress;
                if total > 0 {
                    let fraction = current as f32 / total as f32;
                    ui.add(
                        egui::ProgressBar::new(fraction).text(format!("{current}/{total}")),
                    );
                }

                ui.add_space(4.0);

                egui::ScrollArea::vertical()
                    .id_salt("log")
                    .max_height(300.0)
                    .stick_to_bottom(true)
                    .show(ui, |ui| {
                        for (level, msg) in &self.log {
                            let color = match level {
                                LogLevel::Info => ui.visuals().text_color(),
                                LogLevel::Warning => egui::Color32::from_rgb(255, 200, 50),
                                LogLevel::Error => egui::Color32::from_rgb(255, 80, 80),
                            };
                            ui.colored_label(color, msg);
                        }
                    });

                if self.done {
                    ui.add_space(4.0);
                    if ui.button("Open Output Folder").clicked() {
                        if let Some(ref dir) = self.final_output_dir {
                            open_folder(dir);
                        }
                    }
                }
            }
        });
    }
}

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
