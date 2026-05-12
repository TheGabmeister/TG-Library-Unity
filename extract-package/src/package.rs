use std::collections::{HashMap, HashSet};
use std::io::{BufReader, Read};
use std::path::{Component, Path};
use std::sync::mpsc::Sender;

use flate2::read::GzDecoder;
use tar::Archive;

pub enum ExtractMessage {
    Progress {
        current: usize,
        total: usize,
        path: String,
    },
    Warning(String),
    Error(String),
    Done {
        total: usize,
        errors: usize,
        warnings: usize,
    },
}

struct GuidInfo {
    pathname: String,
    has_asset: bool,
}

fn extract_parts(tar_path: &Path) -> Option<(String, String)> {
    let mut components = tar_path.components().filter_map(|c| match c {
        Component::Normal(s) => Some(s.to_string_lossy().to_string()),
        _ => None,
    });

    let guid = components.next()?;
    let filename = components.next()?;

    if components.next().is_some() {
        return None;
    }

    Some((guid, filename))
}

fn scan_archive(package_path: &Path) -> Result<HashMap<String, GuidInfo>, String> {
    let file =
        std::fs::File::open(package_path).map_err(|e| format!("Failed to open file: {e}"))?;
    let decoder = GzDecoder::new(BufReader::new(file));
    let mut archive = Archive::new(decoder);

    let mut pathnames: HashMap<String, String> = HashMap::new();
    let mut has_asset: HashSet<String> = HashSet::new();

    for entry_result in archive
        .entries()
        .map_err(|e| format!("Invalid archive: {e}"))?
    {
        let mut entry = match entry_result {
            Ok(e) => e,
            Err(_) => continue,
        };

        let tar_path = match entry.path() {
            Ok(p) => p.to_path_buf(),
            Err(_) => continue,
        };

        let Some((guid, filename)) = extract_parts(&tar_path) else {
            continue;
        };

        match filename.as_str() {
            "pathname" => {
                let mut s = String::new();
                if entry.read_to_string(&mut s).is_ok() {
                    let trimmed = s
                        .lines()
                        .next()
                        .unwrap_or("")
                        .trim()
                        .to_string();
                    if !trimmed.is_empty() {
                        pathnames.insert(guid, trimmed);
                    }
                }
            }
            "asset" => {
                has_asset.insert(guid);
            }
            _ => {}
        }
    }

    let mut guids = HashMap::new();
    for (guid, pathname) in pathnames {
        let has = has_asset.contains(&guid);
        guids.insert(guid, GuidInfo { pathname, has_asset: has });
    }

    Ok(guids)
}

pub fn list_contents(package_path: &Path) -> Result<Vec<String>, String> {
    let guids = scan_archive(package_path)?;
    let mut paths: Vec<String> = guids.into_values().map(|info| info.pathname).collect();
    paths.sort();
    Ok(paths)
}

fn sanitize_path(path: &str) -> String {
    path.replace('\0', "")
        .replace('\\', "/")
        .split('/')
        .map(|c| c.trim_matches(|ch: char| ch.is_control()))
        .filter(|c| !c.is_empty() && *c != ".." && *c != ".")
        .collect::<Vec<_>>()
        .join("/")
}

pub fn extract(
    package_path: &Path,
    output_dir: &Path,
    include_meta: bool,
    tx: Sender<ExtractMessage>,
) {
    // Pass 1: scan for pathnames and asset presence
    let guids = match scan_archive(package_path) {
        Ok(g) => g,
        Err(e) => {
            let _ = tx.send(ExtractMessage::Error(e));
            let _ = tx.send(ExtractMessage::Done {
                total: 0,
                errors: 1,
                warnings: 0,
            });
            return;
        }
    };

    if guids.is_empty() {
        let _ = tx.send(ExtractMessage::Warning(
            "No entries found in package".into(),
        ));
        let _ = tx.send(ExtractMessage::Done {
            total: 0,
            errors: 0,
            warnings: 1,
        });
        return;
    }

    let total = guids.values().filter(|info| info.has_asset).count();
    let pathnames: HashMap<String, String> = guids
        .into_iter()
        .map(|(guid, info)| (guid, info.pathname))
        .collect();

    // Pass 2: extract files
    let file = match std::fs::File::open(package_path) {
        Ok(f) => f,
        Err(e) => {
            let _ = tx.send(ExtractMessage::Error(format!("Failed to open: {e}")));
            let _ = tx.send(ExtractMessage::Done {
                total,
                errors: 1,
                warnings: 0,
            });
            return;
        }
    };

    let decoder = GzDecoder::new(BufReader::new(file));
    let mut archive = Archive::new(decoder);

    let tar_entries = match archive.entries() {
        Ok(e) => e,
        Err(e) => {
            let _ = tx.send(ExtractMessage::Error(format!(
                "Failed to read archive: {e}"
            )));
            let _ = tx.send(ExtractMessage::Done {
                total,
                errors: 1,
                warnings: 0,
            });
            return;
        }
    };

    let mut errors = 0usize;
    let mut warnings = 0usize;
    let mut current = 0usize;
    let mut reported: HashSet<String> = HashSet::new();

    for entry_result in tar_entries {
        let mut entry = match entry_result {
            Ok(e) => e,
            Err(_) => continue,
        };

        let tar_path = match entry.path() {
            Ok(p) => p.to_path_buf(),
            Err(_) => continue,
        };

        let Some((guid, filename)) = extract_parts(&tar_path) else {
            continue;
        };

        let is_asset = filename == "asset";
        let is_meta = filename == "asset.meta";

        if !is_asset && !(is_meta && include_meta) {
            continue;
        }

        let Some(pathname) = pathnames.get(&guid) else {
            continue;
        };

        let sanitized = sanitize_path(pathname);
        if sanitized.is_empty() {
            warnings += 1;
            let _ = tx.send(ExtractMessage::Warning(format!(
                "Invalid path: {pathname}"
            )));
            continue;
        }

        let target_rel = if is_meta {
            format!("{sanitized}.meta")
        } else {
            sanitized.clone()
        };

        let target = output_dir.join(&target_rel);

        if !target.starts_with(output_dir) {
            warnings += 1;
            let _ = tx.send(ExtractMessage::Warning(format!(
                "Path traversal blocked: {pathname}"
            )));
            continue;
        }

        if is_asset && reported.insert(guid) {
            current += 1;
            let _ = tx.send(ExtractMessage::Progress {
                current,
                total,
                path: sanitized.clone(),
            });
        }

        if let Some(parent) = target.parent() {
            if let Err(e) = std::fs::create_dir_all(parent) {
                errors += 1;
                let _ = tx.send(ExtractMessage::Error(format!(
                    "Failed to create directory for {target_rel}: {e}"
                )));
                continue;
            }
        }

        let mut data = Vec::new();
        if let Err(e) = entry.read_to_end(&mut data) {
            errors += 1;
            let _ = tx.send(ExtractMessage::Error(format!(
                "Failed to read {target_rel}: {e}"
            )));
            continue;
        }

        if let Err(e) = std::fs::write(&target, &data) {
            errors += 1;
            let _ = tx.send(ExtractMessage::Error(format!(
                "Failed to write {target_rel}: {e}"
            )));
        }
    }

    let _ = tx.send(ExtractMessage::Done {
        total,
        errors,
        warnings,
    });
}
