#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

mod app;
mod package;

fn main() -> eframe::Result {
    let options = eframe::NativeOptions {
        viewport: eframe::egui::ViewportBuilder::default()
            .with_inner_size([900.0, 700.0])
            .with_min_inner_size([600.0, 450.0]),
        ..Default::default()
    };

    eframe::run_native(
        "Unity Package Extractor",
        options,
        Box::new(|cc| {
            app::setup_theme(&cc.egui_ctx);
            Ok(Box::new(app::App::default()))
        }),
    )
}
