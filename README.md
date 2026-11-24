# CASE ZERO — Godot 4.4.1 .NET

A bilingual (Persian/English) 2D detective mystery prototype designed for portrait Android screens and tablet-friendly touch controls.

## Project stack

- Godot **4.4.1 .NET**
- C# / .NET 8
- Android export preset (ARMv7 + ARM64)
- GitHub Actions builds a debug APK in the cloud
- No external art or paid assets are required by this starter project

## Open and build from GitHub using an Android tablet

You do not need to install the Godot editor on the Redmi Pad SE to run the GitHub build workflow.

1. Open `https://github.com/new` in Chrome and create a repository named `CASE-ZERO`.
2. Extract this ZIP on the tablet.
3. In the new repository choose **Add file → Upload files**.
4. Upload the contents *inside* the `CASE-ZERO` folder (the `.github` folder must be included).
5. Commit the upload to the `main` branch.
6. Open the repository's **Actions** tab.
7. Select **Build CASE ZERO Android APK** → **Run workflow**.
8. Wait for the workflow to finish successfully.
9. Open the completed run, scroll to **Artifacts**, and download `CASE-ZERO-Android-APK`.
10. Extract the artifact ZIP; it contains `CASE-ZERO-debug.apk`. Install it on the tablet after Android asks for permission to install that file.

If the Actions tab is not visible, open the repository's **Settings → Actions → General** and ensure Actions are enabled.

## Important: C# Android support

Godot supports exporting C# projects to Android from Godot 4.2 onward, but the engine documentation still labels this support experimental. This project pins Godot 4.4.1 .NET and .NET 8 to keep the editor, C# SDK, and export templates aligned. If the workflow fails, open the failed job and inspect the first red step; do not change random versions.

## Project layout

```text
CASE-ZERO/
├── .github/workflows/android-apk.yml
├── data/
│   ├── cases.json
│   └── localization.json
├── Scenes/
│   └── Main.tscn
├── Scripts/
│   └── Main.cs
├── CaseZero.csproj
├── export_presets.cfg
├── project.godot
├── .gitignore
└── README.md
```

## Current playable systems

- Three dossier entries
- Case detail view
- Recovered group chat
- Evidence collection board
- Persons-of-interest screen
- Investigator report screen
- Persian/English text switching
- Touch-sized controls and scrollable content

## Notes

- This is a playable starter/prototype, not a finished commercial game.
- The current case material is fictional.
- The Android workflow creates a **debug APK** for testing. A Play Store release needs a separately configured signing key and release process.
- Keep the `.github` directory and `export_presets.cfg` in the repository; the workflow depends on them.
