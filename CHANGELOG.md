# Changelog

## Version Alpha 0.0.12a (2023-05-19) [PUBLIC]
* Only single instance of application can run at once.
* Dark theme are always enforced.
* Improved error handling of the HTTP Client.
* Renamed `OcrService` to `OcrRecorderService` 
* OCR recorder service won't start if capture areas list is empty.
* Log are now stored on a physical file (`system.log`) instead of in RAM memory.
* Fixed Windows 10 compatibility.
	* Windows 10 now uses Acrylic as a replacement of Mica.
* Fixed wrong handling of `TaskCancelledException` exception.
* Fixed Welcome page log viewer not updating until OCR recorder service was stopped.
* Fixed inconsistent Start/Stop buttons availability.

## Version Alpha 0.0.11a (2023-05-17) [PUBLIC]
* Added app image assets (logos, icons, badges...)
* Added app packaging support.
* First public release.

## Version Alpha 0.0.7 (2023-05-16) [PRIVATE]
* Added ability to start and stop OCR recording.
* Added console log to the welcome page.
* Added link to documentation (README.md) in footer.
* Added copyright notice at the bottom right corner.
* Fixed rendering of some unicode glyphs.
* Fixed style incosistences between loading and loaded capture areas list views.

## Version Alpha 0.0.6 (2023-05-14) [PRIVATE]
* Fixed a certain potentially unhandled exception.

## Version Alpha 0.0.5 (2023-05-14) [PRIVATE]
* Added HTTP client for sending data.
* Added support for complex logging.
* Added a background service to execute OCR-related tasks.
* Added ability to capture screenshots of data capture areas and save them, and send them to the OCR queue.
* Renamed `VectorA` and `VectorB` properties of the `CaptureArea` data model to `Location` and `Size`, respectively.
* Minor code cleanups.

## Version Alpha 0.0.4 (2023-05-13) [PRIVATE]
* Fixed issue with filesystem watcher triggering on intended file changes.

## Version Alpha 0.0.3 (2023-05-13) [PRIVATE]
* Added filesystem watcher to track unintended file changes.
* Added links to Github repository and changelog in the welcome page.
* Added version number to title bar.
* Added tooltip to "Share feedback" button indicating that it redirects to the author's Twitter profile.
* Improved formatting of CHANGELOG.md.
* Fixed positioning of information bars.
* Fixed a potential deadlock in the `ReloadCaptureAreasCommand`.

## Version Alpha 0.0.2 (2023-05-13) [PRIVATE]
* Added filters to the capture areas list.
* Added form validation for capture areas.
* Added copyright notice throughout to comply with the MIT license.

## Version Alpha 0.0.1 (2023-05-12) [PRIVATE]
* Initial release.
