# Changelog

## Version Alpha 0.0.15a_02 (2023-06-04) [PUBLIC]
* Fixed per-monitor mouse clipping.

## Version Alpha 0.0.15a_01 (2023-06-04) [PUBLIC]
* Fixed not giving absolute coords over multiple monitors.

## Version Alpha 0.0.15a (2023-06-04) [PUBLIC]
* Now, capture areas can be selected trough the OCR Overlay.
* Fixed app window not being activated when a second app instance is invoked.
### Known issues
* Per-monitor mouse clipping isn't supported yet.
* OCR Overlay not gives absolute coords over multiple monitors.

## Version Alpha 0.0.14a_08 (2023-05-28) [PUBLIC]
* Added support for Win32 API interoperability.
* Fixed first-launch crash after install.

## Version Alpha 0.0.13a_03 (2023-05-22) [PUBLIC]
* Reduced the size of the delete logs button in Logs history page.
* Improved the style of Start/Stop buttons when disabled.
* Fixed log viewer height. (Finally! :D)
* Fixed spacing between buttons and list in capture areas page.
* Fixed strange files being accessed more than once at startup.
* Fixed log lines not appending correctly to file (getting erased upon service re-starts).

## Version Alpha 0.0.13a (2023-05-22) [PUBLIC]
* Changed application name from "WifftOCR" to "OCR Studio".
* Make app icon to be displayed in title bar instead of just red circle.
* Added Logs History page.
* Now, every session generates (and keeps) it's unique log file.
* Temp files generated during recording now are cleaned up when OCR recorder service stops.
* Improved the style of Start and Stop buttons.
### Known issues
* Log info is overwritten when OCR recorder service is restarted.

## Version Alpha 0.0.12a_03 (2023-05-19) [PUBLIC]
* Fixed log file not being locked while reading.

## Version Alpha 0.0.12a_02 (2023-05-19) [PUBLIC]
* Now, the log file is cleaned up when application starts.

## Version Alpha 0.0.12a_01 (2023-05-19) [PUBLIC]
* Fixed some log messages not being saved to file.
* Fixed capture areas checking not considering disabled ones.

## Version Alpha 0.0.12a (2023-05-19) [PUBLIC]
* Only a single instance of the application can run at once.
* Dark theme is now enforced.
* Improved error handling of the HTTP Client.
* Renamed `OcrService` to `OcrRecorderService`.
* Moved the OCR recorder service into its own host.
* The OCR recorder service won't start if the capture areas list is empty.
* Logs are now stored in a physical file (`system.log`) instead of in RAM memory.
* Fixed compatibility issues with Windows 10.
	* Windows 10 now uses Acrylic as a replacement for Mica.
* Fixed incorrect handling of the `TaskCancelledException` exception.
* Fixed Welcome page log viewer not updating until the OCR recorder service was stopped.
* Fixed inconsistent availability of the Start/Stop buttons.
* Fixed crash when double-clicking a settings item in the side menu.
* Fixed WelcomePage losing its state when navigating to other pages.
* Fixed inability to restart the OCR recorder service after it has been stopped.
### Known issues
* Sometimes, the app stops logging content into the log file for unknown reasons.
* Sometimes, the app fails to log content into the log file because the handle isn't closed from the previous operation, despite the lock semaphore.

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

## Version Alpha 0.0.1 (2023-05-11) [PRIVATE]
* Initial release.
